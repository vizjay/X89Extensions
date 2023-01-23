/*************************************************************************************************************************************************
 *  Class               : XA9BusinessLayer
 *  Description         : Perform business logic for each extension entry points
 *  Used by             : 
 *  Author              : TriZetto Inc.
 *************************************************************************************************************************************************
 * Ver                  Date                    Modification Description
 * 1.0                  05/01/2016              Initial creation; (Viswan Jayaraman)
                        05/20/2016              Revised by Wen-Man Liu; modified for COBEntryPoint 
                        06/15/2016              Revised by Wen-Man Liu; modified for PRTBEntryPoint 
                        09/15/2016              Revised by Viswan Jayaraman; Defect 3726. Now including Status 11 for One Copay Per Day
                        09/30/2016              Revised by Viswan Jayaraman; ITS Non Par. For Hold Harmless SF Msg, don't just exist the extension. Force apply HOST Price. Defect 3574. 
                        10/03/2016              Revised by Viswan Jayaraman; ITS Non Par. Defect 3574. When there is an emergency place of service, force ITS as price
                        03/10/2017              Revised by Viswan Jayaraman; Excluding Non Par claims from Reduction of Benefits
 *************************************************************************************************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml.Linq;
using ErCoreIfcExtensionData;
using System.Windows.Forms;
using XA9Extensions.BusinessLayer;
using XA9Extensions.DataAccessLayer;
using XA9Extensions.Utilities;
using System.Xml.Linq;
using System.Xml.XPath;


namespace XA9Extensions
{
    

    [ComVisible(true)]
    [Guid("02B1D063-1E0D-4187-9986-88FEC7E3A62C")]
    [ProgId("XA9Extensions.XA9XCom")]
    [ClassInterface(ClassInterfaceType.None)]
    public class XA9XCom
    {
        //private bool isOnline = false;

        /// <summary>
        /// Empty Public constructor needed for COM registration
        /// </summary>
        public XA9XCom()
        {

        }

        public bool RollingUpEntryPoint(IFaExtensionData extensionDataObtained, string facetsID)
        {
            XA9BusinessLayer businessLayer = null;
            try
            {
                // Obtain extension data
                Initiate(extensionDataObtained);

                //Logger.LoggerInstance.ReportMessage("RollingUpEntryPoint", "  ENTERED AT EXIT POINT " + ContextData.ContextInstance.ExitTiming);

                businessLayer = new XA9BusinessLayer();

                //Obtain Claim Data
                businessLayer.GetClaimData();

                if (businessLayer.CurrentClaim.CLCL_PRE_PRICE_IND == "H")
                {
                    string strProvClass = FacetsData.FacetsInstance.GetSingleDataItem("CLPP", "CLPP_CLASS_PROV", false);

                    //If Provider Classification is not available in Header, then pick the first non null provider classification from ITS provider detail
                    if (string.IsNullOrEmpty(strProvClass))
                    {
                        //Logger.LoggerInstance.ReportMessage("RollingUpEntryPoint", "Provider Classification is empty at header level. So picking up from the line level");

                        strProvClass = FacetsData.FacetsInstance.GetMultipleDataElements("CDPPALL", "CDPPALL")
                                         .Descendants().Where(cdpp => cdpp.Attribute("name").Value.Equals("CDPP_CLASS_PROV") && !string.IsNullOrEmpty(cdpp.Value))
                                         .FirstOrDefault().Value;
                        //Logger.LoggerInstance.ReportMessage("RollingUpEntryPoint", "Provider Classification at line level is " + strProvClass);
                    }

                    //Logger.LoggerInstance.ReportMessage("RollingUpEntryPoint", "strProvClass is  " + strProvClass);

                    /* Check if it is a Non Par Provider. Else exit -- Begin */
                    if (AppConfig.NonParProvClass.Contains(strProvClass))
                    {
                        //Logger.LoggerInstance.ReportMessage("RollingUpEntryPoint", " Provider is Non Par with Classification of : " + strProvClass + ". So exit and handle this in Non Par");
                        return true;
                    }


                }

            }
            catch (Exception ex)
            {
                //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint INSIDE EXCEPTION : ", "EXCEPTION MESSAGE AT EXITPOINT " + ContextData.ContextInstance.ExitTiming + "  IS : " + ex.Message);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(extensionDataObtained);
                extensionDataObtained = null;
            }
            return true;
        }
        public bool PRDenyEntryPoint(IFaExtensionData extensionDataObtained, string facetsID)
        {
            XA9BusinessLayer businessLayer = null;
            string strQuery = string.Empty;
            string strDeliveryMtd = string.Empty;
            string strQueryResult = string.Empty;
            string strProvClass = string.Empty;
            string strProvState = string.Empty;
            bool blnPRDeny = false;

            try
            {
                // Obtain extension data
                Initiate(extensionDataObtained);

                //Logger.LoggerInstance.ReportMessage("PRDenyEntryPoint", "  ENTERED AT EXIT POINT " + ContextData.ContextInstance.ExitTiming);

                businessLayer = new XA9BusinessLayer();

                //Obtain Claim Data
                businessLayer.GetClaimData();

                /* If the claim is an ITS HOME then exit - begin*/
                if (businessLayer.CurrentClaim.CLCL_PRE_PRICE_IND.Equals("H"))
                {
                    return true;
                }

                if (!businessLayer.CurrentClaim.CLED_TRAD_PARTNER.Equals("COBA"))
                {
                    return true;
                }
                else
                {
                    //Logger.LoggerInstance.ReportMessage("PRDenyEntryPoint", "  IS COBA Claim");
                }

                if (businessLayer.CurrentClaim.PDDS_MCTR_BCAT.Equals("MSUP"))
                {
                    return true;
                }

                businessLayer.GetProviderData();

                //strQuery = "SELECT PRAD_STATE FROM CMC_PRAD_ADDRESS WHERE PRAD_ID = '{0}' AND PRAD_TYPE = 'REM' AND CONVERT(DATE,'{1}') BETWEEN PRAD_EFF_DT AND PRAD_TERM_DT;"; // Commenting for ALM 5244
                strQuery = "SELECT TOP 1 PRAD_STATE FROM CMC_PRAD_ADDRESS WHERE PRAD_ID = '{0}' AND PRAD_PRACTICE_IND = 'Y' AND PRAD_STATE IN ('IA','MN','SD')  AND CONVERT(DATE,'{1}') BETWEEN PRAD_EFF_DT AND PRAD_TERM_DT;"; // Adding for ALM 5244
                Logger.LoggerInstance.ReportMessage("PRDenyEntryPoint", " Provider Address Query is : " + string.Format(strQuery, businessLayer.CurrentClaim.ServicingProvider.PRAD_ID, businessLayer.CurrentClaim.CLCL_LOW_SVC_DT));
                strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(string.Format(strQuery, businessLayer.CurrentClaim.ServicingProvider.PRAD_ID, businessLayer.CurrentClaim.CLCL_LOW_SVC_DT));
                Logger.LoggerInstance.ReportMessage("PRDenyEntryPoint", " Provider Address Query result is : " + strQueryResult);

                if (FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult))
                {
                    Logger.LoggerInstance.ReportMessage("PRDenyEntryPoint", " At least one of the active practicing address of the provider has the state IA, MN or SD. So EXIT");
                    return true;
                }
                else
                {
                    Logger.LoggerInstance.ReportMessage("PRDenyEntryPoint", " None of the active practicing states of the provider falls within IA, MN or SD");
                }

                /*
                if (AppConfig.PRDenyInStates.Contains(strProvState))
                {
                    return true;
                }
                else
                {
                    //Logger.LoggerInstance.ReportMessage("PRDenyEntryPoint", "  Provider State is " + strProvClass);
                }
                */
                businessLayer.GetClcbData();

                if (!AppConfig.PRDenyClcbType.Contains(businessLayer.CurrentClaim.ClaimCLCB.CLCB_COB_TYPE))
                {
                    return true;
                }

                businessLayer.GetClaimLineData();

                // if sum of CDML_ALLOW is 0.00 then return true
                if (businessLayer.CurrentClaim.claimLines.Sum(cdml => cdml.CDML_ALLOW) == 0.00)
                {
                    return true;
                }

                blnPRDeny = businessLayer.ProcessPRDeny();

                if (blnPRDeny == true)
                {
                    FacetsData.FacetsInstance.CompleteProcess();
                }

            }
            catch (Exception ex)
            {
                //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint INSIDE EXCEPTION : ", "EXCEPTION MESSAGE AT EXITPOINT " + ContextData.ContextInstance.ExitTiming + "  IS : " + ex.Message);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(extensionDataObtained);
                extensionDataObtained = null;
            }
            return true;

        }

        /// <summary>
        /// Entry Point for Once in a LF process
        /// </summary>
        /// <param name="extensionDataObtained"></param>
        /// <param name="facetsID"></param>
        /// <returns></returns>
        public bool OLFTMEntryPoint(IFaExtensionData extensionDataObtained, string facetsID)
        {
            XA9BusinessLayer businessLayer = null;
            bool blnDenyPendOverride = false;
            string strDeliveryMtd = string.Empty;
            bool isOnceInALFClaim = false;
            string strProvClass = string.Empty;

            try
            {
                // Obtain extension data
                Initiate(extensionDataObtained);

                businessLayer = new XA9BusinessLayer();

                //Obtain Claim Data
                businessLayer.GetClaimData();

                /* If the claim is an ITS HOME then exit - begin*/
                if (businessLayer.CurrentClaim.CLCL_PRE_PRICE_IND.Equals("H"))
                    return true;
                /* If the claim is an ITS HOME then exit - end*/

                /* If the claim is not a medical claim then exit -- Begin */
                if (!businessLayer.CurrentClaim.CLCL_CL_SUB_TYPE.Equals("M"))
                    return true;
                /* If the claim is not a medical claim then exit -- End */

                /*Eliminate MedSup Claims -- Begin*/
                if (AppConfig.MedSupTradingPartners.Contains(businessLayer.CurrentClaim.CLED_TRAD_PARTNER) ||
                    AppConfig.MedSupUserData.Contains(businessLayer.CurrentClaim.CLED_USER_DATA1))
                    return true;
                /* Eliminate MedSup Claims -- End*/


                /* Exclude Claim based on status -- Begin */
                /*
                if (AppConfig.ExcludeClaimStatus.Contains(businessLayer.CurrentClaim.CLCL_CUR_STS))
                    return true;
                 * */
                /* Exclude Claim based on status -- Begin */

                
                
                /************* MEDSUP / NON PAR CLAIM CHECK - BEGIN **************/

                /* IF THE CLAIM IS AN ITS HOME THEN EXIT. DO NOT HANDLE ITS HOME HERE - BEGIN 

                if (businessLayer.CurrentClaim.CLCL_PRE_PRICE_IND == "H")
                {
                    Check for MedSup claim for ITS Home - Begin 
                    strDeliveryMtd = FacetsData.FacetsInstance.GetSingleDataItem("CLMI", "CLMI_DELIVERY_METH", false);
                    if (AppConfig.ITSNonParMedSupDeliveryMtd.Contains(strDeliveryMtd))
                        return true;

                    strProvClass = FacetsData.FacetsInstance.GetSingleDataItem("CLPP", "CLPP_CLASS_PROV", false);
                    if (string.IsNullOrEmpty(strProvClass))
                    {
                        strProvClass = FacetsData.FacetsInstance.GetMultipleDataElements("CDPPALL", "CDPPALL")
                                    .Descendants().Where(cdpp => cdpp.Attribute("name").Value.Equals("CDPP_CLASS_PROV") && !string.IsNullOrEmpty(cdpp.Value))
                                    .FirstOrDefault().Value;
                    }

                    // If provider classification is not a NON Par then exit.
                    if (!AppConfig.NonParProvClassForOLFTM.Contains(strProvClass))
                        return true;

                    Check for MedSup claim for ITS Home- End 
                }
                IF THE CLAIM IS AN ITS HOME THEN EXIT. DO NOT HANDLE ITS HOME HERE - END */
                /*if(businessLayer.CurrentClaim.CLCL_PRE_PRICE_IND == "H")
                {
                    return true;
                }
                else
                {
                    Eliminate MedSup Claims -- Begin
                    if (AppConfig.MedSupTradingPartners.Contains(businessLayer.CurrentClaim.CLED_TRAD_PARTNER) ||
                        AppConfig.MedSupUserData.Contains(businessLayer.CurrentClaim.CLED_USER_DATA1))
                        return true;
                     Eliminate MedSup Claims -- End
                }
                */
                /************* MEDSUP / NON PAR CLAIM CHECK - END **************/


                // Load claim lines of current claim into Memory
                businessLayer.GetClaimLineData();

                /************* FULLY DENIED CLAIM CHECK - BEGIN **************/
                // If claim is fulle denied, return true
                /*
                if (businessLayer.CurrentClaim.IsDeniedClaim)
                    return true;
                 */
                /************* FULLY DENIED CLAIM CHECK - END **************/

                //Logger.LoggerInstance.ReportMessage("OLFTMEntryPoint", "Calling PutOnceInALFStatusForEachLine");
                isOnceInALFClaim = businessLayer.PutOnceInALFStatusForEachLine();
                //Logger.LoggerInstance.ReportMessage("OLFTMEntryPoint", " Is Once In A LF Claim? " + isOnceInALFClaim.ToString());
                //If Deny (For Local claims) or Pend (For ITS Home claims) is set, then save data back into Facets and reprocess
                if (isOnceInALFClaim)
                {
                    /* For Logging ONLY - Begin */
                    businessLayer.GetClaimLineOverrideData();
                    //businessLayer.CurrentClaim.claimLines.Where(line => !string.IsNullOrEmpty(line.OnceInALFGroup)).ToList()
                        //.ForEach(line => Logger.LoggerInstance.ReportMessage("Line Lifetime Family","Line Seq Number " + line.CDML_SEQ_NO.ToString() + " is having proc code of " + line.IPCD_ID.ToString() + "  with FAMILY " + line.OnceInALFGroup));
                    /* For Logging ONLY - End */
                    //Logger.LoggerInstance.ReportMessage("OLFTMEntryPoint", "Calling ProcessOnceInALfTmClaim");
                    blnDenyPendOverride = businessLayer.ProcessOnceInALfTmClaim();
                    if (blnDenyPendOverride == true)
                    {
                        FacetsData.FacetsInstance.CompleteProcess();
                    }
                }
                else
                    return true;

            }
            catch (Exception ex)
            {
                //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint INSIDE EXCEPTION : ", "EXCEPTION MESSAGE AT EXITPOINT " + ContextData.ContextInstance.ExitTiming + "  IS : " + ex.Message);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(extensionDataObtained);
                extensionDataObtained = null;
            }
            return true;

        }

        /// <summary>
        /// Entry point for DME
        /// </summary>
        /// <param name="extensionDataObtained"></param>
        /// <param name="facetsID"></param>
        /// <returns></returns>
        public bool DMEEntryPoint(IFaExtensionData extensionDataObtained, string facetsID)
        {
            XA9BusinessLayer businessLayer = null;
            string strDeliveryMtd = string.Empty;
            bool blnSaveDME = false;
            bool isDMEClaim = false;
            try
            {
                Initiate(extensionDataObtained);
                businessLayer = new XA9BusinessLayer();

                // Load Current Claim into Memory
                businessLayer.GetClaimData();


                /* If the claim is not a medical claim then exit -- Begin */
                if (!businessLayer.CurrentClaim.CLCL_CL_SUB_TYPE.Equals("M"))
                    return true;
                /* If the claim is not a medical claim then exit -- End */

                /* If the claim is in status 11,81,15 then exclude this claim -- Begin*/
                //if (AppConfig.ExcludeClaimStatus.Contains(businessLayer.CurrentClaim.CLCL_CUR_STS))
                  //  return true;
                /* If the claim is in status 11,81,15 then exclude this claim -- End*/

                businessLayer.GetClhpData();
                ////Logger.LoggerInstance.ReportMessage(XA9Constants.PRC_PROC_XCOM_OCPD, "Claim Type is " + businessLayer.CurrentClaim.HospClaimData.HOSP_TYPE);
                if (businessLayer.CurrentClaim.HospClaimData.POS_IND == "I")
                    return true;



                if (businessLayer.CurrentClaim.CLCL_PRE_PRICE_IND == "H")
                {
                    /* Check for MedSup claim for ITS Home - Begin */
                    strDeliveryMtd = FacetsData.FacetsInstance.GetSingleDataItem("CLMI", "CLMI_DELIVERY_METH", false);
                    if (AppConfig.ITSNonParMedSupDeliveryMtd.Contains(strDeliveryMtd))
                        return true;
                    /* Check for MedSup claim for ITS Home- End */
                }
                else
                {
                    /*Eliminate MedSup Claims -- Begin*/
                    if (AppConfig.MedSupTradingPartners.Contains(businessLayer.CurrentClaim.CLED_TRAD_PARTNER) ||
                        AppConfig.MedSupUserData.Contains(businessLayer.CurrentClaim.CLED_USER_DATA1))
                        return true;
                    /* Eliminate MedSup Claims -- End*/
                }

                // Load claim lines of current claim into Memory
                businessLayer.GetClaimLineData();

                // If claim is fully denied, return true
                if (businessLayer.CurrentClaim.IsDeniedClaim)
                    return true;

                // Check if this is a DME claim. If False then the current claim is not a DME claim
                isDMEClaim = businessLayer.PutDMEStatusForEachClaimLine();

                // If none of the procedure code is a DME procedure code, then exit
                if(!isDMEClaim)
                    return true;
                else
                    blnSaveDME = businessLayer.ProcessDME();

                // If DME is processed, save the result back to Facets
                if (blnSaveDME == true)
                    FacetsData.FacetsInstance.CompleteProcess();


            }
            catch (Exception ex)
            {

            }
            return true;
        }

        /// <summary>
        /// This is the entry point for ITS Non Par extension.
        /// Exit timings for this extension are PREPROC and POSTPROC
        /// </summary>
        /// <param name="extensionDataObtained"></param>
        /// <param name="facetsID"></param>
        /// <returns></returns>
        public bool ITSNonParEntryPoint(IFaExtensionData extensionDataObtained, string facetsID)
        {
            
            XA9BusinessLayer businessLayer = null;
            bool blnPriceOverride = false;
            string strProvClass = string.Empty;
            bool isHoldHarmless = false;
            bool isForceApplyWMKPrice = false;
            string strDeliveryMtd = string.Empty;
            
            string[] arrEmergencyProcCodes;
            string[] arrEmergencyRevCodes;
            int intEmergencyProcCodeLowerBound = 0;
            int intEmergencyProcCodeUpperBound = 0;
            int intEmergencyRevCodeLowerBound = 0;
            int intEmergencyRevCodeUpperBound = 0;
            int intProcCodeFromClaim = 0;
            int intRevCodeFromClaim = 0;
            string[] procCodes;
            string[] revCodes;
            string strForcePricing = string.Empty;
            bool isOnceInALftm = false;
            bool isAdjustedClaim = false;
            string strOriginalClaimPricedAt = string.Empty;
            
            try
            {
                

                Initiate(extensionDataObtained);

                //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", "-------------------------------------------ENTERED AT " + ContextData.ContextInstance.ExitTiming + "------------------------------------------");
                businessLayer = new XA9BusinessLayer();
                businessLayer.GetClaimData();

                /* Exclude Non ITS Claim -- Begin */
                if (businessLayer.CurrentClaim.CLCL_PRE_PRICE_IND != "H")
                    return true;
                /* Exclude Non ITS Claim -- End */

                //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", "Preprice indicator is " + businessLayer.CurrentClaim.CLCL_PRE_PRICE_IND);
                
                
                strProvClass = FacetsData.FacetsInstance.GetSingleDataItem("CLPP", "CLPP_CLASS_PROV", false);

                // Defect 3534 - If Provider Classification is not available in Header, then pick the first non null provider classification from ITS provider detail
                if(string.IsNullOrEmpty(strProvClass))
                {
                    //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", "Provider Classification is empty at header level. So picking up from the line level");
                    
                   strProvClass = FacetsData.FacetsInstance.GetMultipleDataElements("CDPPALL", "CDPPALL")
                                    .Descendants().Where(cdpp => cdpp.Attribute("name").Value.Equals("CDPP_CLASS_PROV") && !string.IsNullOrEmpty(cdpp.Value))
                                    .FirstOrDefault().Value;
                   //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", "Provider Classification at line level is " + strProvClass);
                }
                // Defect 3534 - End
                //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", "strProvClass is  " + strProvClass);

                /* Check if it is a Non Par Provider. Else exit -- Begin */
                if (!AppConfig.NonParProvClass.Contains(strProvClass))
                    return true;

                /* Check if it is a Non Par Provider. Else exit -- End */

                /* Check for MedSup claim - Begin */
                strDeliveryMtd = FacetsData.FacetsInstance.GetSingleDataItem("CLMI", "CLMI_DELIVERY_METH", false);
                //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", "strDeliveryMtd is  " + strDeliveryMtd);
                if (AppConfig.ITSNonParMedSupDeliveryMtd.Contains(strDeliveryMtd))
                {
                    return true;
                }
                /* Check for MedSup claim - End */

               

                /*Exclude MedSup Claims -- Begin*/
                if (AppConfig.MedSupTradingPartners.Contains(businessLayer.CurrentClaim.CLED_TRAD_PARTNER) ||
                    AppConfig.MedSupUserData.Contains(businessLayer.CurrentClaim.CLED_USER_DATA1))
                {
                    //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", "MedSup Claim");
                    return true;
                }

                //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", "Not a MeSup Claim");

                string CLMI_INTL_CD = FacetsData.FacetsInstance.GetSingleDataItem("CLMI", "CLMI_INTL_CD", false);
                if(CLMI_INTL_CD.Equals("Y"))
                {
                    //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", " This is an international claim");
                    return true;
                }
                /*Exclude MedSup Claims -- End*/

                //if (ContextData.ContextInstance.ExitTiming.Equals("POSTELIG"))
				if (AppConfig.FacetsOnlineApps.Contains(ContextData.ContextInstance.ApplicationId) && ContextData.ContextInstance.ExitTiming.Equals("POSTELIG"))
                {
                    #region POSTELIG MEDICAL
                    if (businessLayer.CurrentClaim.CLCL_CL_SUB_TYPE == "M")
                    {
                        XElement clorILElement = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CLOR", "CLOR"))
                                    .FirstOrDefault(cdorElm => cdorElm.Elements().Any(elm => elm.Attribute("name").Value.Equals("CLOR_OR_ID") && elm.Value.Equals("IL")) &&
                                                              cdorElm.Elements().Any(elm => elm.Attribute("name").Value.Equals("CLOR_OR_VALUE") && elm.Value.Equals("EXTN"))
                                                              );

                        XElement clor00Element = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CLOR", "CLOR"))
                                    .FirstOrDefault(cdorElm => cdorElm.Elements().Any(elm => elm.Attribute("name").Value.Equals("CLOR_OR_ID") && elm.Value.Equals("00")) &&
                                                              cdorElm.Elements().Any(elm => elm.Attribute("name").Value.Equals("CLOR_OR_VALUE") && elm.Value.Equals("EXTN"))
                                                              );

                        XElement clorIKElement = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CLOR", "CLOR"))
                                    .FirstOrDefault(cdorElm => cdorElm.Elements().Any(elm => elm.Attribute("name").Value.Equals("CLOR_OR_ID") && elm.Value.Equals("IK")) &&
                                                              cdorElm.Elements().Any(elm => elm.Attribute("name").Value.Equals("CLOR_OR_VALUE") && elm.Value.Equals("EXTN"))
                                                              );

                        if (clorILElement != null && clor00Element != null) // When both IL and 00 elements are present then the claim failed for manual price. Skip HOST price retrival step
                        {
                            if (clorIKElement != null)
                                clorIKElement.Remove();

                            


                            Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", " At " + ContextData.ContextInstance.ExitTiming + " - **** Removing NW Overrides ");
                            businessLayer.RemoveExtnNWOverrides();
                            Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", " At " + ContextData.ContextInstance.ExitTiming + " - **** Removing AA Overrides ");
                            businessLayer.RemoveExtnAAOverrides();

                            FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL")).
                            Where(cdor => cdor.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && e.Value.Equals("MP")))
                            .ToList().ForEach(cdorMP => cdorMP.Remove());

                            Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", " At " + ContextData.ContextInstance.ExitTiming + " - **** Removing MP Overrides ");

                            FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL")).
                            Where(cdor => cdor.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && e.Value.Equals("EP")) &&
                                          cdor.Elements().Any(e => e.Attribute("name").Value.Equals("EXCD_ID") && e.Value.Equals(AppConfig.ITSEPManualPriceExcdID))
                                  ).ToList()
                                  .ForEach(cdorEPBWPElm => businessLayer.InsertCdorRecord
                                    (
                                    cdorEPBWPElm.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CLCL_ID")).Value,
                                    int.Parse(cdorEPBWPElm.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value),
                                    "MP",
                                    int.Parse(cdorEPBWPElm.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("MEME_CK")).Value),
                                    double.Parse(cdorEPBWPElm.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDOR_OR_AMT")).Value),
                                    "EXTN",
                                    AppConfig.ITSEPManualPriceExcdID
                                    ));
                            Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", " At " + ContextData.ContextInstance.ExitTiming + " - **** Putting All EP BWP into MP Overrides ");

                            FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL")).
                            Where(cdor => cdor.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && e.Value.Equals("MP")))
                            .ToList().ForEach(cdorMP => Logger.LoggerInstance.ReportMessage("*** MP Override exists in line # ", cdorMP.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value));

                                
                            //businessLayer.RemoveExternalPrice();
                        }

                      

                    }
                    #endregion POSTELIG MEDICAL
                }

                
                // This CLEAN Up step is required because Non Par claims can error for various reasons and any point and before saving to Facets we will ensure that all custom data elements are completely removed.
                if (ContextData.ContextInstance.ExitTiming.Equals("PRESAVE"))
                {
                    #region PRESAVE - Clean up unprocessed data elements
                    //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", " At " + ContextData.ContextInstance.ExitTiming + " - Entered PRESAVE ");
                    //bool found = false;
                    blnPriceOverride = true;
                    string strOrigProviderID = string.Empty;

                    //businessLayer.GetClaimOverrideData();

                    // Remove all IL and IK CLOR elements that we put in by the extension
                    FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CLOR", "CLOR"))
                        //.Where(clor => clor.Elements().Any(e => e.Attribute("name").Value.Equals("CLOR_OR_ID") && (new string[] {"IK","IL"}).Contains(e.Value)) &&
                        .Where(clor => clor.Elements().Any(e => e.Attribute("name").Value.Equals("CLOR_OR_ID") && (new string[] { "IK", "IL", "00" }).Contains(e.Value)) && // REMOVE 00 OVERRIDE AS WELL. DEFECT 5093
                                       clor.Elements().Any(e => e.Attribute("name").Value.Equals("CLOR_OR_VALUE") && e.Value.Equals("EXTN"))).ToList()
                                       .ForEach(clorElmToRemove => clorElmToRemove.Remove());

                    // Remove all NP CDOR elements that were used to hold HOST Price
                    FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL")).
                            //Where(cdor => cdor.Elements().Any(e => e.Attribute("name").Value.Equals("CLOR_OR_ID") && e.Value.Equals("NP"))) COMMENTED ON 03/04 TO CORRECT CLOR_OR_ID TO CDOR_OR_ID
                            Where(cdor => cdor.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && e.Value.Equals("NP"))) 
                            .ToList().ForEach(cdorNp => cdorNp.Remove());

                    businessLayer.RevertSRAndPAOverrides();

                    if (businessLayer.CurrentClaim.CLCL_CL_SUB_TYPE.Equals("H"))
                    {
                        XElement clorNPElement = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CLOR", "CLOR"))
                                                .FirstOrDefault(clor => clor.Elements().Any(e => e.Attribute("name").Value.Equals("CLOR_OR_ID") && e.Value.Equals("NP")));
                        if (clorNPElement != null)
                            clorNPElement.Remove();
                    }
                    //found = false;
                    else if (businessLayer.CurrentClaim.CLCL_CL_SUB_TYPE.Equals("M"))
                    {
                        //businessLayer.GetClaimLineOverrideData();

                        XElement clorNPElement = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CLOR", "CLOR"))
                                                .FirstOrDefault(clor => clor.Elements().Any(e => e.Attribute("name").Value.Equals("CLOR_OR_ID") && e.Value.Equals("NP")));
                        if (clorNPElement != null)
                        {
                            strOrigProviderID = clorNPElement.Elements().FirstOrDefault(elmPrprID => elmPrprID.Attribute("name").Value.Equals("CLOR_OR_VALUE")).Value;
                            if (!string.IsNullOrEmpty(strOrigProviderID))
                            {
                                FacetsData.FacetsInstance.SetSingleDataItem("CLCL", "PRPR_ID", strOrigProviderID); // Replace with Original Provider ID
                            }
                            clorNPElement.Remove();
                        }

                        XElement cdorToRemove = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL")).
                                                FirstOrDefault(cdor => cdor.Elements().Any(e => e.Attribute("name").Value.Equals("CDML_SEQ_NO") && e.Value.Equals("1000")));
                        if (cdorToRemove != null)
                        {
                            cdorToRemove.Remove();
                            
                        }
                    }
                    //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", " At " + ContextData.ContextInstance.ExitTiming + " - Exiting Presave ");
                    #endregion PRESAVE

                } // End Of PRESAVE

                else
                {
                    businessLayer.GetClaimLineData();
                    businessLayer.GetClaimLineOverrideData();
                    /* Exclude Manually processed claim -- Begin */
                    if (businessLayer.CurrentClaim.claimLineOverrides.Any(cdor => cdor.CDOR_OR_ID.Equals("EP") && AppConfig.ITSNPManualEPOverride.Contains(cdor.EXCD_ID)))
                        return true;
                    /* Exclude Manually processed claim -- End */

                    businessLayer.GetClaimITSData();

                    /* Exclude Hold Harmless Claim -- Begin */
                    isHoldHarmless = businessLayer.CurrentClaim.claimITS.Any(claim => AppConfig.ITSHoldHarmlessSF.Contains(claim.CLIM_ITS_MSG_CD));
                    //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", "blnHoldHarmless " + isHoldHarmless.ToString());

                    isForceApplyWMKPrice = businessLayer.CurrentClaim.claimITS.Any(claim => AppConfig.WMKForcePricingSF.Contains(claim.CLIM_ITS_MSG_CD));
                    //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", "isForceApplyWMKPrice " + isForceApplyWMKPrice.ToString());
                    /* Exclude Hold Harmless Claim -- End */

                    isAdjustedClaim = businessLayer.CurrentClaim.IsAdjustedClaim;
                    if (isAdjustedClaim)
                        strOriginalClaimPricedAt = businessLayer.GetAdjustedClaimPricingMethod();


                    /* Exclude Emergency claim -- Begin */
                    if (businessLayer.CurrentClaim.CLCL_CL_SUB_TYPE.Equals("M"))
                    {
                        if (isHoldHarmless)
                        {
                            strForcePricing = "ITS";
                        }
                        else if (businessLayer.CurrentClaim.claimLines.Any(line => AppConfig.ITSNPProfEmergencyPOS.Contains(line.PSCD_ID))) // 3574
                        {
                            strForcePricing = "ITS";
                        }
                        else
                        {
                            arrEmergencyProcCodes = AppConfig.ITSNPProfEmergencyProcCodes;

                            if (arrEmergencyProcCodes.Length > 0)
                            {
                                foreach (var proc in arrEmergencyProcCodes)
                                {
                                    procCodes = proc.Split('-');
                                    int.TryParse(procCodes[0], out intEmergencyProcCodeLowerBound);
                                    int.TryParse(procCodes[1], out intEmergencyProcCodeUpperBound);

                                    //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", "intEmergencyProcCodeLowerBound is  " + intEmergencyProcCodeLowerBound.ToString());
                                    //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", "intEmergencyProcCodeUpperBound is  " + intEmergencyProcCodeUpperBound.ToString());

                                    if (intEmergencyProcCodeLowerBound > 0 && intEmergencyProcCodeUpperBound > 0)
                                    {
                                        foreach (var line in businessLayer.CurrentClaim.claimLines)
                                        {
                                            int.TryParse(line.IPCD_ID, out intProcCodeFromClaim);
                                            if (intProcCodeFromClaim > 0)
                                            {

                                                if (intProcCodeFromClaim.IsBetween(intEmergencyProcCodeLowerBound, intEmergencyProcCodeUpperBound))
                                                {
                                                    //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " PROFESSIONAL EMERGENCY CLAIM SO FORCE HOST PRICE");
                                                    /* Defect 3329 - Begin */
                                                    //return true; -- Defect 3329. For professional claims if HOST price need to be applied, then do not exit. Force the pricing to ITS HOST
                                                    strForcePricing = "ITS"; // When the claim is an emergency claim, use the HOST pricing. Force pricing to use HOST Price and do not compare pricing.
                                                    break;
                                                    /* Defect 3329 - End */
                                                }
                                            }
                                        }
                                    }

                                } // foreach (var proc in arrEmergencyProcCodes)
                            }
                        }// if(arrEmergencyProcCodes.Length > 0)

                        if (string.IsNullOrEmpty(strForcePricing) && isForceApplyWMKPrice)
                        {
                            strForcePricing = "NWX";
                        }
                        if (string.IsNullOrEmpty(strForcePricing) && isAdjustedClaim)
                            strForcePricing = strOriginalClaimPricedAt;


                    }
                    else if (businessLayer.CurrentClaim.CLCL_CL_SUB_TYPE.Equals("H"))
                    {
                        if (isHoldHarmless)
                        {
                            //return true;
                            strForcePricing = "ITS";
                        }
                        else
                        {
                            arrEmergencyRevCodes = AppConfig.ITSNPHospEmergencyRevCodes;
                            if (arrEmergencyRevCodes.Length > 0)
                            {
                                foreach (var rev in arrEmergencyRevCodes)
                                {
                                    revCodes = rev.Split('-');
                                    int.TryParse(revCodes[0], out intEmergencyRevCodeLowerBound);
                                    int.TryParse(revCodes[1], out intEmergencyRevCodeUpperBound);

                                    if (intEmergencyRevCodeLowerBound > 0 && intEmergencyRevCodeUpperBound > 0)
                                    {
                                        foreach (var line in businessLayer.CurrentClaim.claimLines)
                                        {
                                            int.TryParse(line.RCRC_ID, out intRevCodeFromClaim);
                                            if (intRevCodeFromClaim > 0)
                                            {
                                                if (intRevCodeFromClaim.IsBetween(intEmergencyRevCodeLowerBound, intEmergencyRevCodeUpperBound))
                                                {
                                                    //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " HOSPITAL EMERGENCY CLAIM SO EXIT");
                                                    //return true;
                                                    strForcePricing = "ITS";
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                } // foreach (var proc in arrEmergencyProcCodes)
                            } // if(arrEmergencyProcCodes.Length > 0)
                        }

                        //strForcePricing = "NWX";  // TESTING ONLY REMOVE IT
                        if (string.IsNullOrEmpty(strForcePricing) && isForceApplyWMKPrice)
                        {
                            strForcePricing = "NWX";
                        }

                        if (string.IsNullOrEmpty(strForcePricing) && isAdjustedClaim)
                            strForcePricing = strOriginalClaimPricedAt;

                        /*if (isForceApplyWMKPrice)
                        {
                            strForcePricing = "NWX";
                        }*/

                    }
                    /* Exclude Emergency claim -- End */



                    //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", "Calling  ProcessITSNonParPricing with parameter " + strForcePricing);
                    /* Defect 3329 - Begin */
                    //blnPriceOverride = businessLayer.ProcessITSNonParPricing();
                    //blnPriceOverride = businessLayer.ProcessITSNonParPricing(strForcePricing);
                    blnPriceOverride = businessLayer.ProcessITSNonPar(strForcePricing);
                    /* Defect 3329 - End */
                }

                //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", "blnPriceOverride is  " + blnPriceOverride.ToString());
                if (blnPriceOverride == true)
                {
                    FacetsData.FacetsInstance.CompleteProcess();
                }

            }
            catch (Exception ex)
            {
                Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint INSIDE EXCEPTION : ", "EXCEPTION MESSAGE AT EXITPOINT " +ContextData.ContextInstance.ExitTiming +"  IS : " + ex.Message);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(extensionDataObtained);
                extensionDataObtained = null;
            }
            return true;
        }

        /// <summary>
        /// This is the entry point for BDC / BDC+ extension
        /// Exit Timing for this extension is PREPROC
        /// </summary>
        /// <param name="extensionDataObtained"></param>
        /// <param name="facetsID"></param>
        /// <returns></returns>
        public bool BDCEntryPoint(IFaExtensionData extensionDataObtained, string facetsID)
        {
            
            bool blnSetData = false;
            string strProcBdcPmdId = string.Empty;
            string strProcBdcSubPgmId = string.Empty;
            string strBdcFlag = string.Empty;
            string strClaimID = string.Empty;
            string strBdcProgName = string.Empty;
            string strQuery = string.Empty;

            XA9BusinessLayer businessLayer = null;
            try
            {
                
                Initiate(extensionDataObtained);

                Logger.LoggerInstance.ReportStart(XA9Constants.PRC_PROC_XCOM_BDC);


                if(ContextData.ContextInstance.ExitTiming.Equals("WINPOSTOPEN"))
                {
                    Logger.LoggerInstance.ReportMessage("Indide Win Post Open", " Clear CUSTOM DATA ELEMETS");
                    if (!string.IsNullOrEmpty(FacetsData.FacetsInstance.GetSingleDataItem("CUSTOM", "POSTSAVEBDC", false)))
                    {
                        XElement elementToRemove = null;
                        elementToRemove = FacetsData.FacetsInstance.FacetsXml.XPathSelectElement(string.Format(XA9Constants.XML_COLLECTION_COLUMN_XPATH, "CUSTOM", "POSTSAVEBDC"));
                        if (elementToRemove != null)
                            elementToRemove.Remove();

                        elementToRemove = FacetsData.FacetsInstance.FacetsXml.XPathSelectElement(string.Format(XA9Constants.XML_COLLECTION_COLUMN_XPATH, "CUSTOM", "BDCPGMID"));
                        if (elementToRemove != null)
                            elementToRemove.Remove();

                        elementToRemove = FacetsData.FacetsInstance.FacetsXml.XPathSelectElement(string.Format(XA9Constants.XML_COLLECTION_COLUMN_XPATH, "CUSTOM", "BDCSUBPGMID"));
                        if (elementToRemove != null)
                            elementToRemove.Remove();

                        elementToRemove = FacetsData.FacetsInstance.FacetsXml.XPathSelectElement(string.Format(XA9Constants.XML_COLLECTION_COLUMN_XPATH, "CUSTOM", "BDCPGMNM"));
                        if (elementToRemove != null)
                            elementToRemove.Remove();

                        elementToRemove = FacetsData.FacetsInstance.FacetsXml.XPathSelectElement(string.Format(XA9Constants.XML_COLLECTION_COLUMN_XPATH, "CUSTOM", "BDCFLAG"));
                        if (elementToRemove != null)
                            elementToRemove.Remove();

                        FacetsData.FacetsInstance.CompleteProcess();
                    }
                    return true;
                }

                businessLayer = new XA9BusinessLayer();
                businessLayer.GetClaimData();



                /*
                if (AppConfig.FacetsOnlineApps.Contains(ContextData.ContextInstance.ApplicationId))
                {
                    //Logger.LoggerInstance.ReportMessage("BDCEntryPoint", "Its an online claim");
                    isOnline = true;
                }
                */

                if (businessLayer.CurrentClaim.CLCL_PRE_PRICE_IND != "H")
                {
                    if (businessLayer.CurrentClaim.CLCL_CL_SUB_TYPE == "M")
                        return true;
                    else if (AppConfig.MedSupTradingPartners.Contains(businessLayer.CurrentClaim.CLED_TRAD_PARTNER) ||
                             AppConfig.MedSupUserData.Contains(businessLayer.CurrentClaim.CLED_USER_DATA1))
                    {
                        //Logger.LoggerInstance.ReportMessage("BDCEntryPoint", "MedSup Claim");
                        return true;
                    }
                }
                else
                {
                    string strProvClass = FacetsData.FacetsInstance.GetSingleDataItem("CLPP", "CLPP_CLASS_PROV", false);

                    // Defect 3534 - If Provider Classification is not available in Header, then pick the first non null provider classification from ITS provider detail
                    if (string.IsNullOrEmpty(strProvClass))
                    {
                        //Logger.LoggerInstance.ReportMessage("BDCEntryPoint", "Provider Classification is empty at header level. So picking up from the line level");

                        strProvClass = FacetsData.FacetsInstance.GetMultipleDataElements("CDPPALL", "CDPPALL")
                                         .Descendants().Where(cdpp => cdpp.Attribute("name").Value.Equals("CDPP_CLASS_PROV") && !string.IsNullOrEmpty(cdpp.Value))
                                         .FirstOrDefault().Value;
                        //Logger.LoggerInstance.ReportMessage("BDCEntryPoint", "Provider Classification at line level is " + strProvClass);
                    }
                    // Defect 3534 - End
                    //Logger.LoggerInstance.ReportMessage("BDCEntryPoint", "strProvClass is  " + strProvClass);

                    /* Check if it is a Non Par Provider. Else exit -- Begin */
                    if (AppConfig.NonParProvClass.Contains(strProvClass))
                    {
                        //Logger.LoggerInstance.ReportMessage("BDCEntryPoint", " Provider is Non Par with Classification of : " + strProvClass + ". So exit and handle this in Non Par");
                        return true;
                    }

                    /* Check for MedSup claim - Begin */
                    string strDeliveryMtd = FacetsData.FacetsInstance.GetSingleDataItem("CLMI", "CLMI_DELIVERY_METH", false);
                    //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", "strDeliveryMtd is  " + strDeliveryMtd);
                    if (AppConfig.ITSNonParMedSupDeliveryMtd.Contains(strDeliveryMtd))
                    {
                        return true;
                    }
                    /* Check for MedSup claim - End */

                    /* Check if it is a Non Par Provider. Else exit -- End */
                }

                /* CR 86 - Exclude Non Blue Card Home Professional Claims - Begin */
                /*if (businessLayer.CurrentClaim.CLCL_PRE_PRICE_IND != "H" && businessLayer.CurrentClaim.CLCL_CL_SUB_TYPE == "M")
                {
                    //Logger.LoggerInstance.ReportMessage("BDCEntryPoint", "Professional Non ITS Home Claim");
                    return true;
                }*/
                /* CR 86 - Exclude Non Blue Card Home Professional Claims - End */

               

                if (ContextData.ContextInstance.ExitTiming.Equals("POSTSAVE"))
                {
                    //Logger.LoggerInstance.ReportMessage("BDCEntryPoint", "Now in POSt SAVE");

                    //Logger.LoggerInstance.ReportMessage("With CUSTOM Data at POST SAVE", FacetsData.FacetsInstance.FacetsXml.ToString());
                    if (!string.IsNullOrEmpty(FacetsData.FacetsInstance.GetSingleDataItem("CUSTOM", "POSTSAVEBDC", false)))
                    {
                        
                        XA9DataLayer dataLayer = new XA9DataLayer();

                        strClaimID = FacetsData.FacetsInstance.GetSingleDataItem("CLCL", "CLCL_ID", false);
                        //Logger.LoggerInstance.ReportMessage("BDCEntryPoint", "strClaimID in post save is " + strClaimID);

                        strProcBdcPmdId = FacetsData.FacetsInstance.GetSingleDataItem("CUSTOM", "BDCPGMID", false);
                        //Logger.LoggerInstance.ReportMessage("BDCEntryPoint", "strProcBdcPmdId in post save is " + strProcBdcPmdId);

                        strProcBdcSubPgmId = FacetsData.FacetsInstance.GetSingleDataItem("CUSTOM", "BDCSUBPGMID", false);
                        //Logger.LoggerInstance.ReportMessage("BDCEntryPoint", "strProcBdcSubPgmId in post save is " + strProcBdcSubPgmId);

                        strBdcProgName = FacetsData.FacetsInstance.GetSingleDataItem("CUSTOM", "BDCPGMNM", false);
                        //Logger.LoggerInstance.ReportMessage("BDCEntryPoint", "strBdcProgName in post save is " + strBdcProgName);


                        strBdcFlag = FacetsData.FacetsInstance.GetSingleDataItem("CUSTOM", "BDCFLAG", false);
                        //Logger.LoggerInstance.ReportMessage("BDCEntryPoint", "strBdcFlag in post save is " + strBdcFlag);

                        bool blnSaved = dataLayer.SetBdcCodeForClaim(businessLayer.CurrentClaim.CLCL_ID, strProcBdcPmdId, strProcBdcSubPgmId, strBdcProgName, strBdcFlag, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQuery);
                        //Logger.LoggerInstance.ReportMessage("BDCEntryPoint", "blnSaved in post save is " + blnSaved.ToString());
                        //Logger.LoggerInstance.ReportMessage("BDCEntryPoint", "strQuery in post save is " + strQuery);

                        //Logger.LoggerInstance.ReportMessage("Indide Win Post Open", " Clear CUSTOM DATA ELEMETS");
                        
                        XElement elementToRemove = null;
                        elementToRemove = FacetsData.FacetsInstance.FacetsXml.XPathSelectElement(string.Format(XA9Constants.XML_COLLECTION_COLUMN_XPATH, "CUSTOM", "POSTSAVEBDC"));
                        if (elementToRemove != null)
                            elementToRemove.Remove();

                        elementToRemove = FacetsData.FacetsInstance.FacetsXml.XPathSelectElement(string.Format(XA9Constants.XML_COLLECTION_COLUMN_XPATH, "CUSTOM", "BDCPGMID"));
                        if (elementToRemove != null)
                            elementToRemove.Remove();

                        elementToRemove = FacetsData.FacetsInstance.FacetsXml.XPathSelectElement(string.Format(XA9Constants.XML_COLLECTION_COLUMN_XPATH, "CUSTOM", "BDCSUBPGMID"));
                        if (elementToRemove != null)
                            elementToRemove.Remove();

                        elementToRemove = FacetsData.FacetsInstance.FacetsXml.XPathSelectElement(string.Format(XA9Constants.XML_COLLECTION_COLUMN_XPATH, "CUSTOM", "BDCPGMNM"));
                        if (elementToRemove != null)
                            elementToRemove.Remove();

                        elementToRemove = FacetsData.FacetsInstance.FacetsXml.XPathSelectElement(string.Format(XA9Constants.XML_COLLECTION_COLUMN_XPATH, "CUSTOM", "BDCFLAG"));
                        if (elementToRemove != null)
                            elementToRemove.Remove();

                         FacetsData.FacetsInstance.CompleteProcess();
                       
                    }
                    return true; //if (ContextData.ContextInstance.ExitTiming.Equals("POSTSAVE"))
                }
             

                /*Eliminate MedSup Claims -- Begin*/
                if (AppConfig.MedSupTradingPartners.Contains(businessLayer.CurrentClaim.CLED_TRAD_PARTNER) ||
                    AppConfig.MedSupUserData.Contains(businessLayer.CurrentClaim.CLED_USER_DATA1))
                {
                    //Logger.LoggerInstance.ReportMessage("BDCEntryPoint", "MedSup Claim");
                    return true;
                }
                /* Eliminate MedSup Claims -- End*/

                //  If its not a hospital claim then exit
                /*  Removing this for CR 86 - Begin
                if (!businessLayer.CurrentClaim.CLCL_CL_SUB_TYPE.Equals("H"))
                {
                    return true;
                }
                 Removing this for CR 86 - End */

                //Logger.LoggerInstance.ReportMessage("BDCEntryPoint", "Calling Method SetClaimTOSOverride");
                blnSetData = businessLayer.SetClaimTOSOverride();

                if (blnSetData == true)
                {
                    FacetsData.FacetsInstance.CompleteProcess();
                    
                }
                //Logger.LoggerInstance.ReportFinish(XA9Constants.PRC_PROC_XCOM_BDC);
            }
            catch (Exception ex)
            {
                Logger.LoggerInstance.ReportMessage(XA9Constants.PRC_PROC_XCOM_BDC, "Exception : " + ex.Message);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(extensionDataObtained);
                extensionDataObtained = null;
            }
            return true;
        }

        /// <summary>
        /// This is the entry point for Once Copay Per Day Extension.
        /// Exit Timing for this extension is POSTPROC
        /// This extension requires REPROC
        /// </summary>
        /// <param name="extensionDataObtained"></param>
        /// <param name="facetsID"></param>
        /// <returns></returns>
        public bool OCPDEntryPoint(IFaExtensionData extensionDataObtained, string facetsID)
        {
            bool blnCopayOverridden = false;
            XA9BusinessLayer businessLayer = null;
            
            try
            {
                
                // Initiate Extension Objects
                Initiate(extensionDataObtained);

                //Print the Initial Message
                

                businessLayer = new XA9BusinessLayer();
                businessLayer.GetClaimData();

                //if (AppConfig.ExcludeClaimStatus.Contains(businessLayer.CurrentClaim.CLCL_CUR_STS)) // Defect 3276
                if (AppConfig.ExcludeClaimStatusForOCPD.Contains(businessLayer.CurrentClaim.CLCL_CUR_STS))
                    return true;

                businessLayer.GetClhpData();
                ////Logger.LoggerInstance.ReportMessage(XA9Constants.PRC_PROC_XCOM_OCPD, "Claim Type is " + businessLayer.CurrentClaim.HospClaimData.HOSP_TYPE);
                if (businessLayer.CurrentClaim.HospClaimData.POS_IND == "I")
                    return true;

                blnCopayOverridden = businessLayer.SetClaimCopayOverride();
                
                if (blnCopayOverridden == true)
                {
                    //Logger.LoggerInstance.ReportMessage("Final XML at " + ContextData.ContextInstance.ExitTiming + " is : ", FacetsData.FacetsInstance.FacetsXml.ToString());
                    FacetsData.FacetsInstance.CompleteProcess();
                }
            }
            catch (Exception ex)
            {
                //Logger.LoggerInstance.ReportMessage("Inside Copay Exception", ex.Message);
            }
            finally
            {
                //FacetsData = null;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(extensionDataObtained);
                extensionDataObtained = null;
                blnCopayOverridden = false;
            }
            return true;
        }

        //v1.1
        /// <summary>
        /// This is the entry point for COB Pay Zero Extension.
        /// Exit Timing for this extension is PREPROC or POSTPROC
        /// This extension might trigger REPROC at POSTPROC 
        /// </summary>
        /// <param name="extensionDataObtained"></param>
        /// <param name="facetsID"></param>
        /// <returns></returns>
        public bool COBEntryPoint(IFaExtensionData extensionDataObtained, string facetsID)
        {

            //Logger.LoggerInstance.ReportMessage("cob", "ENTERED");
            XA9BusinessLayer businessLayer = null;
            try
            {

                //FacetsData and ContextData
                Initiate(extensionDataObtained);
                //Check Exit Timing
                string exitTiming = ContextData.ContextInstance.ExitTiming;
                string appID = ContextData.ContextInstance.ApplicationId;
                //Exit extension if it incorrect applciation
                if (appID != "ICC2" && appID != "ICH2" && appID != "EADJ") { return true; }
                //BusinessLayer
                businessLayer = new XA9BusinessLayer();
                businessLayer.GetClaimData();
                //Logger.LoggerInstance.ReportMessage("COB CLaim Status", businessLayer.CurrentClaim.CLCL_CUR_STS);
                if (AppConfig.ExcludeClaimStatus.Contains(businessLayer.CurrentClaim.CLCL_CUR_STS))
                    return true;
                //Logger.LoggerInstance.ReportMessage("Calling", "GetClcbData");
                businessLayer.GetClcbData(exitTiming);
            }
            catch (Exception ex)
            {
                //Logger.LoggerInstance.ReportMessage("Exception in COB ", ex.Message);
            }
            finally
            {
                //Release Com Object
                System.Runtime.InteropServices.Marshal.ReleaseComObject(extensionDataObtained);
            }
            return true;
        }

        //v1.2
        /// <summary>
        /// This is the entry point for No PartB Medicare Extension.
        /// Exit Timing for this extension is PREPROC or POSTPROC or PRESAVE
        /// This extension might trigger REPROC at POSTPROC 
        /// </summary>
        /// <param name="extensionDataObtained"></param>
        /// <param name="facetsID"></param>
        /// <returns></returns>
        public bool PRTBEntryPoint(IFaExtensionData extensionDataObtained, string facetsID)
        {
            string strProvClass = string.Empty; // CR 128 Exclude Non Par Claims for now
            bool _blRtn = true;
            string appID = "";
            ////Logger.LoggerInstance.ReportMessage("PartB", "ENTERED");
            XA9BusinessLayer businessLayer = null;
            try
            {
                //FacetsData and ContextData
                Initiate(extensionDataObtained);
                //Check Exit Timing
                appID = ContextData.ContextInstance.ApplicationId;
                string exitTiming = ContextData.ContextInstance.ExitTiming;
                string userID = ContextData.ContextInstance.UserId;
                string customDB = ContextData.ContextInstance.DatabaseId + "custom";
                //Exit extension if incorrect applciation
                if (appID != "ICC2" && appID != "ICH2" && appID != "EADJ" && appID != "CLH2" && appID != "CLC2") { return true; }  //testing CLC2 and CLH2
                //EADJ and PREPROC
                if (appID == "EADJ" && exitTiming == "PREPROC") { return true; }
                //BusinessLayer
                businessLayer = new XA9BusinessLayer();
                businessLayer.GetClaimData(); //CLCL
                
                ///* CR 128 - Exclude Non Par Claims for now - begin */

                if (businessLayer.CurrentClaim.CLCL_PRE_PRICE_IND.Equals("H"))
                {
                    Logger.LoggerInstance.ReportMessage("***PRTBEntryPoint", "Its a HOME Claim");

                    strProvClass = FacetsData.FacetsInstance.GetSingleDataItem("CLPP", "CLPP_CLASS_PROV", false);
                    if (string.IsNullOrEmpty(strProvClass))
                    {

                        Logger.LoggerInstance.ReportMessage("***PRTBEntryPoint", "Provider Classification is empty at header level. So picking up from the line level");
                        strProvClass = FacetsData.FacetsInstance.GetMultipleDataElements("CDPPALL", "CDPPALL")
                                         .Descendants().Where(cdpp => cdpp.Attribute("name").Value.Equals("CDPP_CLASS_PROV") && !string.IsNullOrEmpty(cdpp.Value))
                                         .FirstOrDefault().Value;
                        //Logger.LoggerInstance.ReportMessage("ITSNonParEntryPoint", "Provider Classification at line level is " + strProvClass);
                    }
                    if (AppConfig.NonParProvClass.Contains(strProvClass)) // If Non Par then exclude
                    {
                        Logger.LoggerInstance.ReportMessage("***PRTBEntryPoint", "Provider Classification is : " + strProvClass + ". So EXIT Processing.");
                        return true;
                    }
                }

                /* CR 128 - Exclude Non Par Claims for now - end */

                //GetPartBCalculation
               //Validation for no medicare part B
                //Check CLCB
                businessLayer.GetClcbData();
                //Exit extension if incorrect COB //testing use C isntead of N
                if (businessLayer.CurrentClaim.ClaimCLCB.CLCB_COB_TYPE != "N" ||
                    Math.Round(businessLayer.CurrentClaim.ClaimCLCB.CLCB_COB_AMT, 2, MidpointRounding.AwayFromZero) > 0 ||
                    Math.Round(businessLayer.CurrentClaim.ClaimCLCB.CLCB_COB_ALLOW, 2, MidpointRounding.AwayFromZero) > 0 ||
                    Math.Round(businessLayer.CurrentClaim.ClaimCLCB.CLCB_COB_DED_AMT, 2, MidpointRounding.AwayFromZero) > 0 ||
                    Math.Round(businessLayer.CurrentClaim.ClaimCLCB.CLCB_COB_COPAY_AMT, 2, MidpointRounding.AwayFromZero) > 0 ||
                    Math.Round(businessLayer.CurrentClaim.ClaimCLCB.CLCB_COB_COINS_AMT, 2, MidpointRounding.AwayFromZero) > 0)
                { return true; }

                //CLMI_INTL_CD; International Indicator; exit
                string CLMI_INTL_CD = FacetsData.FacetsInstance.GetSingleDataItem("CLMI", "CLMI_INTL_CD", false);
                if (!string.IsNullOrEmpty(CLMI_INTL_CD) && CLMI_INTL_CD.Equals("I")) { return true; }
           

                string _errMsg = businessLayer.GetPartBCalculation(exitTiming, appID, userID, customDB);
                if (!string.IsNullOrEmpty(_errMsg))
                {
                    if (appID != "EADJ") { FacetsData.FacetsInstance.ExtensionDataObject.AddMessage(8, 8, "PRTBEntryPoint GetPartBCalculation Error: " + _errMsg); }
                    _blRtn = false;
                }
            }
            catch (Exception ex)
            {
                if (appID != "EADJ") { FacetsData.FacetsInstance.ExtensionDataObject.AddMessage(8, 8, "PRTBEntryPoint Exception: " + ex.Message); }
                _blRtn = false;
            }
            finally
            {
                //Release Com Object
                System.Runtime.InteropServices.Marshal.ReleaseComObject(extensionDataObtained);
            }
            return _blRtn;
        }

        /// <summary>
        /// Initialize ExtensionData and ContextData objects
        /// </summary>
        /// <param name="extData"></param>
        private void Initiate(IFaExtensionData pExtData)
        {
            //ExtensionData
            FacetsData.FacetsInstance.Initialize(pExtData);
            //ContextData
            ContextData.ContextInstance.Initialize(pExtData);
            //Logger.LoggerInstance.CurrentClaimId = FacetsData.FacetsInstance.GetSingleDataItem(XA9Constants.CLCL_DATAID, XA9Constants.CLCL_COL_DATAID, false);


        }

       
    }
}
