/*************************************************************************************************************************************************
 *  Class               : XA9BusinessLayer
 *  Description         : Perform business logic for each extension entry points
 *  Used by             : 
 *  Author              : TriZetto Inc.
 *************************************************************************************************************************************************
 * Ver                  Date                    Modification Description 
 * 1.0                  05/01/2016              Initial creation; (Viswan Jayaraman)
                        05/20/2016              Revised by Wen-Man Liu; modified for COBEntryPoint 
                        06/15/2016              Defect 1221 - XA9_System Testing_QI-674: Claims updated with reason code ‘COB0’ when Subscriber does not have a COB. TEST1. 
 *                                              Test CLCL_ID = '400150305600' (Viswan Jayaraman)
                        06/17/2016              CR 86 - Include Professional Claims. 
 *                                                      Read the last two chars of Product attachment for the first two chars of SESE_ID   (Viswan Jayaraman) 
 *                      09/02/2016              Defect 2235 - Trim BDC Program name when looking for BDC+ indicator   
 *                                              Defect 2231 - When identifying BDC status for IPCD_ID, 
 *                                                            1) Use the entire length of IPCD_ID for inpatient claims  (Viswan Jayaraman)
 *                                                            2) Use only the first 5 characters for other claims       (Viswan Jayaraman)
 *                      09/08/2016              Defect 3329 : For professional emergency claim, use HOST price and do not compare. (Viswan Jayaraman)
 *                      09/16/2016              Defect 2606 : Trim SF Msg and the first two chars of TOS available in the Product attachment (Viswan Jayaraman)
 *                      09/20/2016              Defect 3636 : Under Medical Claims POSTPRICECLM MEDICAL, Cann GetProviderData() method. (Viswan Jayaraman)
 *                      09/22/2016              Defect 3747 : Blue Card Inpatient Home claim should not go thru the extension since Facets claim denied. (Viswan Jayaraman)
 *                      10/06/2016              Defect 3980 : When allowed amount for a line is zero, then do not send EP override for that line (Viswan Jayaraman)
 *                      11/05/2016              Defect XXXX : Manual DF Message: When there are more than one Disallow explaination code associated with a claim line, then Facets is not able to send the 
 *                                              appropriate DF message. Modifying the code to send the appropriate DF message automatically when there are more then one disallow explaination 
 *                                              code and there is no DF message being automatically sent by Facets.
 *                      11/11/2016              Defect 3747:  For hospital claims as well feed in the IK override when we put in EP
 *************************************************************************************************************************************************
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text;
using XA9Extensions.DataAccessLayer;
using XA9Extensions.Utilities;
using ErCoreIfcExtensionData;
using System.Windows.Forms;
using CommonUtilities;

namespace XA9Extensions.BusinessLayer
{
    /// <summary>
    /// This is the business layer class for XA9 Extensions
    /// </summary>
    public class XA9BusinessLayer
    {
        //ClaimsData
        private Claim _claim;

        internal Claim CurrentClaim
        {
            get { return _claim; }
        }

       
        /// <summary>
        /// Retrieves the NPPR Prefix for the Product for the the low service date
        /// </summary>
        /// <returns></returns>
        public string GetNpprPfxForProduct()
        {
            XA9DataLayer dataLayer = null;
            string strQueryResult = string.Empty;
            bool recordsReturned = true;
            string strQuery = string.Empty;
            string strNpprPfx = string.Empty;
            try
            {

                dataLayer = new XA9DataLayer();
                strQuery = dataLayer.GetNpprPfxForProduct(_claim.PDPD_ID, _claim.CLCL_LOW_SVC_DT, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQueryResult);
                recordsReturned = FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult);
                if (recordsReturned == true)
                {
                    strNpprPfx = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "PDBC_PFX", false);
                }

            }
            catch (Exception ex)
            {
                //Logger.LoggerInstance.ReportMessage("Exception in GetNpprPfxForProduct", ex.Message);
            }
            return strNpprPfx;

        }

        /// <summary>
        /// Checks whether unresolved messages resides in WF for the current pended claim
        /// </summary>
        /// <returns>True if unresolved messages exists in WF</returns>
        internal bool CheckIfUnResolvedMsgExistsInWF()
        {
            string strQuery = string.Empty;
            string strQueryResult = string.Empty;
            bool recordsReturned = false;
            try
            {
                strQuery = "SELECT TOP 1 WWMS_RESOLVED_IND FROM NWX_WWMS_WARNMSG WHERE WWMS_MESSAGE_ID = '" + _claim.CLCL_ID + "' AND WWMS_RESOLVED_IND = 0;";
                //Logger.LoggerInstance.ReportMessage("CheckIfUnResolvedMsgExistsInWF strQuery : ", strQuery);
                strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
                //Logger.LoggerInstance.ReportMessage("CheckIfUnResolvedMsgExistsInWF Returned : ", strQueryResult);
                recordsReturned = FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult);
            }
            catch (Exception ex)
            {

            }
            return recordsReturned;
        }

        /// <summary>
        /// Determine whether the claim is in Workflow Queue
        /// </summary>
        /// <returns>True if the claim is in Workflow Queue. Else False</returns>
        internal bool CheckIfClaimIsInWFQueue()
        {
            string strQuery = string.Empty;
            string strQueryResult = string.Empty;
            bool recordsReturned = false;
            try
            {
                strQuery = "SELECT TOP 1 WQDF_QUEUE_ID FROM NWX_WQMS_QUEUE_MSG WHERE WQMS_MESSAGE_ID = '" + _claim.CLCL_ID + "';";
                //Logger.LoggerInstance.ReportMessage("CheckIfClaimIsInWFQueue strQuery : ", strQuery);
                strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
                //Logger.LoggerInstance.ReportMessage("CheckIfClaimIsInWFQueue Returned : ", strQueryResult);
                recordsReturned = FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult);
            }
            catch (Exception ex)
            {

            }
            return recordsReturned;
        }

       
        
        /// <summary>
        /// Populates DME status for each claim line for the current claim and returns the count of DME procedure codes contained within the current claim
        /// </summary>
        /// <returns>Integer representing the count of DME procedure codes for the current claim</returns>
        public bool PutDMEStatusForEachClaimLine()
        {
            bool recordsReturned = false;
            string strAggregatedProcCodes = string.Empty;
            XA9DataLayer dataLayer = null;
            List<string> lstProcCodesForCurrentClaim = null;
            string strQueryResult = string.Empty;
            string strQuery = string.Empty;
            List<string> lstDMEProcCodes = null;
            try
            {
                //Get all the unique procedure codes for the claim where the procedure code is not empty
                lstProcCodesForCurrentClaim = this._claim.UniqueLineLevelNotNullProcedureCodes;
                // if the procedure code count is zero then exit. Else concatenate the procedure codes separated by comma.
                if (lstProcCodesForCurrentClaim.Count > 0)
                {
                    // strAggregatedProcCodes contains the concatenated procedure codes where each prrocedure code starts and ends with '
                    //strAggregatedProcCodes = lstProcCodesForCurrentClaim.Aggregate((start, end) => "'" + start + "'" + ", " + "'" + end + "'");
                    strAggregatedProcCodes = "'" + string.Join(",", lstProcCodesForCurrentClaim) + "'";
                    dataLayer = new XA9DataLayer();
                    strQuery = dataLayer.GetDMEProcCodesFromTheList(strAggregatedProcCodes, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQueryResult);
                    //Logger.LoggerInstance.ReportMessage("PutDMEStatusForEachClaimLine", "Query for Getting DME Proc Codes " + strQuery);
                    recordsReturned = FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult);
                    if (recordsReturned == true)
                    {
                        lstDMEProcCodes = FacetsData.FacetsInstance.GetDbMultipleDataItem(strQueryResult, "DATA", "DME_PROC_CD", false);
                        _claim.claimLines.ForEach(line => {if(lstDMEProcCodes.Contains(line.IPCD_ID)) { line.IsDMELine = true;} });

                    }
                }
            }
            catch (Exception ex)
            {

            }

            return recordsReturned;
        }

      
        /// <summary>
        /// Populate Claim Data
        /// </summary>
        public void GetClaimData()
        {
            if (_claim == null)
            {
                _claim = new Claim();
                _claim.CLCL_ID = FacetsData.FacetsInstance.GetSingleDataItem("CLCL", "CLCL_ID", false);
                _claim.CLCL_CUR_STS = FacetsData.FacetsInstance.GetSingleDataItem("CLCL", "CLCL_CUR_STS", false);
                _claim.MEME_CK = int.Parse(FacetsData.FacetsInstance.GetSingleDataItem("CLCL", "MEME_CK", false));
                _claim.CLCL_PRE_PRICE_IND = FacetsData.FacetsInstance.GetSingleDataItem("CLCL", "CLCL_PRE_PRICE_IND", false);
                _claim.CLCL_CL_TYPE = FacetsData.FacetsInstance.GetSingleDataItem("CLCL", "CLCL_CL_TYPE", false);
                _claim.CLCL_CL_SUB_TYPE = FacetsData.FacetsInstance.GetSingleDataItem("CLCL", "CLCL_CL_SUB_TYPE", false);

                _claim.PDPD_ID = FacetsData.FacetsInstance.GetSingleDataItem("CLCL", "PDPD_ID", false);
                if (string.IsNullOrEmpty(_claim.PDPD_ID)) { _claim.PDPD_ID = _claim.PDPD_ID; }

                _claim.CLED_TRAD_PARTNER = FacetsData.FacetsInstance.GetSingleDataItem("CLED", "CLED_TRAD_PARTNER", false);
                _claim.CLED_USER_DATA1 = FacetsData.FacetsInstance.GetSingleDataItem("CLED", "CLED_USER_DATA1", false);
                _claim.CLED_USER_DATA2 = FacetsData.FacetsInstance.GetSingleDataItem("CLED", "CLED_USER_DATA2", false);
                _claim.AGAG_ID = FacetsData.FacetsInstance.GetSingleDataItem("CLCL", "AGAG_ID", false);
                _claim.SAVE_BDC = "N";


                //v1.1
                _claim.CLCL_CAP_IND = FacetsData.FacetsInstance.GetSingleDataItem("CLCL", "CLCL_CAP_IND", false);
                _claim.CLCL_LOW_SVC_DT = FacetsData.FacetsInstance.GetSingleDataItem("CLCL", "CLCL_LOW_SVC_DT", false);  //could ne "NULL"

                //v1.2
                _claim.SBSB_CK = int.Parse(FacetsData.FacetsInstance.GetSingleDataItem("CLCL", "SBSB_CK", false));
                _claim.PRPR_ID = FacetsData.FacetsInstance.GetSingleDataItem("CLCL", "PRPR_ID", false);
                _claim.CLCL_ID_ADJ_FROM = FacetsData.FacetsInstance.GetSingleDataItem("CLCL", "CLCL_ID_ADJ_FROM", false);
                //_claim.CLCL_ID_ADJ_FROM = "XXX";
                //CUSTOM
                _claim.REPROC_FLAG = FacetsData.FacetsInstance.GetSingleDataItem("CUSTOM", "REPROC_FLAG", false); //could be null
                //CLST
                _claim.CLST_STS = FacetsData.FacetsInstance.GetSingleDataItem("CLST", "CLST_STS", false);
                if (string.IsNullOrEmpty(_claim.CLST_STS)) { _claim.CLST_STS = _claim.CLCL_CUR_STS; }

                _claim.WellmarkMemberContrievedKeys = new List<int>();
                _claim.PDDS_MCTR_BCAT = FacetsData.FacetsInstance.GetSingleDataItem("CLCL", "PDDS_MCTR_BCAT", false);
                //_claim.PRAD_ID = FacetsData.FacetsInstance.GetSingleDataItem("CLCL", "PRAD_ID", false);
            }
            
        }

        /// <summary>
        /// Populate Servicing Provider Data for the claim when needed
        /// </summary>
        public void GetProviderData()
        {
            GetClaimData();
            if (_claim.ServicingProvider == null)
            {
                _claim.ServicingProvider = new PRPR();
                _claim.ServicingProvider.PRPR_ID = FacetsData.FacetsInstance.GetSingleDataItem("SVCPR", "PRPR_ID", false);
                _claim.ServicingProvider.PRPR_ENTITY = FacetsData.FacetsInstance.GetSingleDataItem("SVCPR", "PRPR_ENTITY", false);
                _claim.ServicingProvider.PRPR_MCTR_TYPE = FacetsData.FacetsInstance.GetSingleDataItem("SVCPR", "PRPR_MCTR_TYPE", false);
                _claim.ServicingProvider.PRCF_MCTR_SPEC = FacetsData.FacetsInstance.GetSingleDataItem("SVCPR", "PRCF_MCTR_SPEC", false);
                _claim.ServicingProvider.PRAD_ID = FacetsData.FacetsInstance.GetSingleDataItem("SVCPR", "PRAD_ID", false);
            }

        }

        /// <summary>
        /// Get claim line details when needed
        /// </summary>
        public void GetClaimLineData()
        {

            GetClaimData();
            if (CurrentClaim.claimLines == null)
            {
                _claim.claimLines = new List<ClaimLine>();
                //Logger.LoggerInstance.ReportMessage("Insede GetClaimLineData", "Claim ID " + CurrentClaim.CLCL_ID);
                List<XElement> d = FacetsData.FacetsInstance.GetMultipleDataElements("CDMLALL", "CDMLALL");
                //Logger.LoggerInstance.ReportMessage("Claim Line Count", d.Count.ToString());

                FacetsData.FacetsInstance.GetMultipleDataElements("CDMLALL","CDMLALL")
                    .ForEach(element => _claim.claimLines.Add(new ClaimLine()
                        {   

                            CLCL_ID = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CLCL_ID").Value,
                            CLCL_CL_SUB_TYPE = _claim.CLCL_CL_SUB_TYPE,
                            CDML_SEQ_NO = int.Parse(element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDML_SEQ_NO").Value),
                            MEME_CK = int.Parse(element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "MEME_CK").Value),
                            CDML_FROM_DT = DateTime.Parse(element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDML_FROM_DT").Value),
                            SESE_ID = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "SESE_ID").Value,
                            SESE_RULE = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "SESE_RULE").Value,
                            CDML_COPAY_AMT = double.Parse(element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDML_COPAY_AMT").Value),
                            IPCD_ID = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "IPCD_ID").Value,
                            CDML_POS_IND = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDML_POS_IND").Value,
                            CDML_UNITS = int.Parse(element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDML_UNITS").Value),
                            CDML_CHG_AMT = double.Parse(element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDML_CHG_AMT").Value),
                             //v1.1
                            CDML_PAID_AMT = Double.Parse(element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDML_PAID_AMT").Value),
                            CDML_SB_PYMT_AMT = Double.Parse(element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDML_SB_PYMT_AMT").Value),
                            CDML_PR_PYMT_AMT = Double.Parse(element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDML_PR_PYMT_AMT").Value),

                             //v1.2
                            CDML_ALLOW = Double.Parse(element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDML_ALLOW").Value),
                            CDML_DISALL_AMT = Double.Parse(element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDML_DISALL_AMT").Value),
                            IDCD_ID = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "IDCD_ID").Value,
                            IDCD_ID_TRANS_REL = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "IDCD_ID_TRANS_REL").Value,
                            strCDML_FROM_DT = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDML_FROM_DT").Value,
                            PSCD_ID = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "PSCD_ID").Value,
                            RCRC_ID = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "RCRC_ID").Value,
                            CDML_CONSIDER_CHG = double.Parse(element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDML_CONSIDER_CHG").Value)
                            
                        }));

                //Logger.LoggerInstance.ReportMessage("Done GetClaimLineData", "Claim ID " + CurrentClaim.CLCL_ID);
                //Logger.LoggerInstance.ReportMessage("Line Count", CurrentClaim.claimLines.Count().ToString());

            }
        }

        /// <summary>
        /// Returns a List<string> of Procedure Codes for the current claim from CMC_CLHI_PROC table where IPCD_ID is not NULL or Empty
        /// </summary>
        internal List<string> GetClhiProcCodes()
        {
            List<string> lstClhiProcCpdes = null;
            try
            {
                lstClhiProcCpdes = FacetsData.FacetsInstance.GetMultipleDataElements("CLHI", "CLHI").Descendants()
                                    .Where(clhi => clhi.Attribute("name").Value.Equals("IPCD_ID") && !string.IsNullOrEmpty(clhi.Value))
                                    .Select(elmProcCode => elmProcCode.Value).ToList();
                //Logger.LoggerInstance.ReportMessage("*********INSIDE GetClhiProcCodes***************", "*******COUNT IS *********" + lstClhiProcCpdes.Count.ToString());
            }
            catch (Exception ex)
            {

            }
            return lstClhiProcCpdes;
        }

        /// <summary>
        /// Returns a List<string> of DIag Codes for the current claim from CMC_CLMD_DIAG table where IDCD_ID is not NULL or Empty
        /// </summary>
        internal List<string> GetClmdDiagCodes()
        {
            List<string> lstClmdDiagCodes = null;
            try
            {
                lstClmdDiagCodes = FacetsData.FacetsInstance.GetMultipleDataElements("CLMD", "CLMD").Descendants()
                                    .Where(clmd => clmd.Attribute("name").Value.Equals("IDCD_ID") && !string.IsNullOrEmpty(clmd.Value))
                                    .Select(elmDiagCode => elmDiagCode.Value).ToList();

                //Logger.LoggerInstance.ReportMessage("*********INSIDE GetClmdDiagCodes***************", "*******COUNT IS *********" + lstClmdDiagCodes.Count.ToString());
            }
            catch (Exception ex)
            {

            }
            return lstClmdDiagCodes;
        }

        /// <summary>
        /// Get Claim ITS Data when needed
        /// </summary>

        public void GetClaimITSData()
        {
            if (CurrentClaim.claimITS == null)
            {
                _claim.claimITS = new List<Clim>();

                FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CLIM", "CLIM"))
                 .Where(clim => clim.Elements().Any(e => e.Attribute("name").Value.Equals("CLIM_TYP") && e.Value.Equals("S"))).ToList()
                 .ForEach(climelm => _claim.claimITS.Add(new Clim()
                 {
                     CLCL_ID = climelm.Elements().FirstOrDefault(e => e.Attribute("name").Value == "CLCL_ID").Value,
                     CLIM_ITS_MSG_CD = climelm.Elements().FirstOrDefault(e => e.Attribute("name").Value == "CLIM_ITS_MSG_CD").Value,
                     CLIM_TYP = climelm.Elements().FirstOrDefault(e => e.Attribute("name").Value == "CLIM_TYP").Value,
                     MEME_CK = int.Parse(climelm.Elements().FirstOrDefault(e => e.Attribute("name").Value == "MEME_CK").Value)
                 }));
                

                /*
                FacetsData.FacetsInstance.GetMultipleDataElements("CLIM",string.Empty)
                    .SelectMany(climElm => climElm.Descendants().Where(climAttr => climAttr.Attribute("name").Value.Equals("CLIM_TYP") && climAttr.Value.Equals("S"))) // SF Message
                    .Select(climPrnt => climPrnt.Parent).ToList()
                    .ForEach(element => _claim.claimITS.Add(new Clim()
                        {   
                            CLCL_ID = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CLCL_ID").Value,
                            CLIM_ITS_MSG_CD = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CLIM_ITS_MSG_CD").Value,
                            CLIM_TYP = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CLIM_TYP").Value,
                            MEME_CK = int.Parse(element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "MEME_CK").Value)
                        }));
                 */
            }
        }

        /// <summary>
        /// Get Claim Hospital Data
        /// </summary>
        public void GetClhpData()
        {
            GetClaimData();
            _claim.HospClaimData = new CLHP()
            {
                CLCL_ID = FacetsData.FacetsInstance.GetSingleDataItem("CLHP", "CLCL_ID", false),
                CLHP_BILL_CLASS = FacetsData.FacetsInstance.GetSingleDataItem("CLHP", "CLHP_BILL_CLASS", false),
                CLHP_FAC_TYPE = FacetsData.FacetsInstance.GetSingleDataItem("CLHP", "CLHP_FAC_TYPE", false),
                MEME_CK = FacetsData.FacetsInstance.GetSingleDataItem("CLHP", "MEME_CK", false),
            };

        }

        /// <summary>
        /// Get Claim Coordination of benefits data
        /// </summary>
        
        public void GetClcbData()
        {
            GetClaimData();
            _claim.ClaimCLCB = new CLCB()
            {
                CLCL_ID = FacetsData.FacetsInstance.GetSingleDataItem("CLCB", "CLCL_ID", false),
                MEME_CK = int.Parse(FacetsData.FacetsInstance.GetSingleDataItem("CLCB", "MEME_CK", false)),
                CLCB_COB_TYPE = FacetsData.FacetsInstance.GetSingleDataItem("CLCB", "CLCB_COB_TYPE", false),
                CLCB_COB_REAS_CD = FacetsData.FacetsInstance.GetSingleDataItem("CLCB", "CLCB_COB_REAS_CD", false),
                CLCB_COB_ALLOW = double.Parse(FacetsData.FacetsInstance.GetSingleDataItem("CLCB", "CLCB_COB_ALLOW", false)),
                CLCB_COB_AMT = double.Parse(FacetsData.FacetsInstance.GetSingleDataItem("CLCB", "CLCB_COB_ALLOW", false))
            };
        }

        /// <summary>
        /// Get Coordination of Benefits (COB) data at the claim line item level
        /// </summary>
        public void GetCdcbData()
        {
            GetClaimData();

            if(_claim.ClaimCLCB == null)
                GetClcbData();

            _claim.ClaimCLCB.ClaimLineCOB = new List<CDCBALL>();

            FacetsData.FacetsInstance.GetMultipleDataElements("CDCBALL","CDCBALL")
                .ForEach(cdcb => _claim.ClaimCLCB.ClaimLineCOB.Add(new CDCBALL()
                {
                    CLCL_ID = cdcb.Elements().SingleOrDefault(clclEml => clclEml.Attribute("name").Value.Equals("CLCL_ID")).Value,
                    CDML_SEQ_NO = int.Parse(cdcb.Elements().SingleOrDefault(cdmlSeqNoElm => cdmlSeqNoElm.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value),
                    MEME_CK = int.Parse(cdcb.Elements().SingleOrDefault(memeCkElm => memeCkElm.Attribute("name").Value.Equals("MEME_CK")).Value),
                    CDCB_COB_TYPE = cdcb.Elements().SingleOrDefault(cobTypeElm => cobTypeElm.Attribute("name").Value.Equals("CDCB_COB_TYPE")).Value,
                    CDCB_COB_ALLOW = double.Parse(cdcb.Elements().SingleOrDefault(cobAllowElm => cobAllowElm.Attribute("name").Value.Equals("CDCB_COB_ALLOW")).Value),
                    CDCB_COB_AMT = double.Parse(cdcb.Elements().SingleOrDefault(cobAmtElm => cobAmtElm.Attribute("name").Value.Equals("CDCB_COB_AMT")).Value),
                    CDCB_COB_DISALLOW = double.Parse(cdcb.Elements().SingleOrDefault(cobDisAllowElm => cobDisAllowElm.Attribute("name").Value.Equals("CDCB_COB_DISALLOW")).Value)
                })
            );
        }
        //v1.1
        /// <summary>
        /// Get Claim Level COB Info
        /// </summary>
        /// <param name="exitTiming"></param>
        public void GetClcbData(string exitTiming)
        {
            //GetClaimData();
            _claim.ClaimCLCB = new CLCB()
            {
                CLCL_ID = FacetsData.FacetsInstance.GetSingleDataItem("CLCB", "CLCL_ID", false),
                MEME_CK = int.Parse(FacetsData.FacetsInstance.GetSingleDataItem("CLCB", "MEME_CK", false)),
                CLCB_COB_TYPE = FacetsData.FacetsInstance.GetSingleDataItem("CLCB", "CLCB_COB_TYPE", false),
                CLCB_COB_REAS_CD = FacetsData.FacetsInstance.GetSingleDataItem("CLCB", "CLCB_COB_REAS_CD", false),
            };

            string CobType = _claim.ClaimCLCB.CLCB_COB_TYPE;
            int cobMEME_CK = _claim.ClaimCLCB.MEME_CK;
            string cobREAS_CD = _claim.ClaimCLCB.CLCB_COB_REAS_CD;
            string clmCLCL_CAP_IND = _claim.CLCL_CAP_IND;
            string clmCLCL_ID = _claim.CLCL_ID;
            string clmMEME_CK = _claim.MEME_CK.ToString();
            string clmCLCL_LOW_SVC_DT = _claim.CLCL_LOW_SVC_DT;

            //PREPROC
            if (exitTiming == "PREPROC")  //testing
            {
                //Logger.LoggerInstance.ReportMessage("EXIT POINT", exitTiming);
                //Logger.LoggerInstance.ReportMessage("CobType is ", CobType);
                //Logger.LoggerInstance.ReportMessage("cobMEME_CK is ", cobMEME_CK.ToString());
                //no CLCB
                //if (string.IsNullOrEmpty(CobType)) // && cobMEME_CK == 0) -- commented out on 6/7
                //if (string.IsNullOrEmpty(CobType)) // && cobMEME_CK == 0) -- commented out on 6/8
                if (string.IsNullOrEmpty(CobType) || CobType.Equals("X")) // COB Type of "X" means COB not received
                {
                    //check CLCL_LOW_SVC_DT
                    if (string.IsNullOrEmpty(clmCLCL_LOW_SVC_DT) || clmCLCL_LOW_SVC_DT == "NULL" || clmCLCL_LOW_SVC_DT == "01/01/1753")
                    {
                        this.GetClaimLineData();
                        foreach (ClaimLine line in _claim.claimLines)
                        {
                            clmCLCL_LOW_SVC_DT = line.CDML_FROM_DT.ToString();
                            break;
                        }
                    }

                    //check MECB
                    if (clmCLCL_LOW_SVC_DT == "NULL") { clmCLCL_LOW_SVC_DT = "01/01/1753"; }
                    string strQuery = "SELECT TOP 1 MECB_INSUR_TYPE FROM dbo.CMC_MECB_COB WHERE MEME_CK = " + clmMEME_CK +
                                      " AND MECB_INSUR_ORDER = 'P' AND '" + clmCLCL_LOW_SVC_DT + "' BETWEEN MECB_EFF_DT AND MECB_TERM_DT" +
                                      " ORDER BY MECB_INSUR_TYPE;";

                    string strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
                    //Logger.LoggerInstance.ReportMessage("strQuery is ", strQuery);
                    //Logger.LoggerInstance.ReportMessage("strQueryResult is ", strQueryResult);
                    if (FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult))
                    {
                        CobType = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "MECB_INSUR_TYPE", false);
                    }

                    //update CLCB
                    //Logger.LoggerInstance.ReportMessage("CobType is ", CobType);
                    //if (!string.IsNullOrEmpty(CobType)) -- Commented on 6/8
                    //if (!string.IsNullOrEmpty(CobType) || !CobType.Equals("X")) // COB Type of "X" should be considered as NO COB
                    if (!string.IsNullOrEmpty(CobType) && !CobType.Equals("X")) // 1.2. Defect # 1221
                    {
                        FacetsData.FacetsInstance.SetSingleDataItem("CLCB", "CLCL_ID", clmCLCL_ID);
                        FacetsData.FacetsInstance.SetSingleDataItem("CLCB", "MEME_CK", clmMEME_CK);
                        FacetsData.FacetsInstance.SetSingleDataItem("CLCB", "CLCB_COB_TYPE", CobType);
                        FacetsData.FacetsInstance.SetSingleDataItem("CLCB", "CLCB_COB_REAS_CD", "COB0");
                        FacetsData.FacetsInstance.CompleteProcess();
                    }

                }
                //with CLCB
                else
                {
                    //Logger.LoggerInstance.ReportMessage("cobREAS_CD", cobREAS_CD);
                    
                    if (cobREAS_CD == "XXX")
                    {
                        //blank out CLCB
                        FacetsData.FacetsInstance.SetSingleDataItem("CLCB", "CLCL_ID", "");
                        FacetsData.FacetsInstance.SetSingleDataItem("CLCB", "MEME_CK", "0");
                        FacetsData.FacetsInstance.SetSingleDataItem("CLCB", "CLCB_COB_TYPE", "");
                        FacetsData.FacetsInstance.SetSingleDataItem("CLCB", "CLCB_COB_REAS_CD", "");
                        FacetsData.FacetsInstance.SetSingleDataItem("CLCB", "CLCB_COB_AMT", "0.0000");
                        FacetsData.FacetsInstance.SetSingleDataItem("CLCB", "CLCB_COB_DISALLOW", "0.0000");
                        FacetsData.FacetsInstance.SetSingleDataItem("CLCB", "CLCB_COB_ALLOW", "0.0000");
                        FacetsData.FacetsInstance.SetSingleDataItem("CLCB", "CLCB_COB_SANCTION", "0.0000");
                        FacetsData.FacetsInstance.SetSingleDataItem("CLCB", "CLCB_COB_DED_AMT", "0.0000");
                        FacetsData.FacetsInstance.SetSingleDataItem("CLCB", "CLCB_COB_COPAY_AMT", "0.0000");
                        FacetsData.FacetsInstance.SetSingleDataItem("CLCB", "CLCB_COB_COINS_AMT", "0.0000");
                        FacetsData.FacetsInstance.CompleteProcess();
                    }
                }

            }
            //POSTPROC
            else if (exitTiming == "POSTPROC")
            {
                //Logger.LoggerInstance.ReportMessage("exitTiming is ", exitTiming);
                //Logger.LoggerInstance.ReportMessage("cobREAS_CD is ", cobREAS_CD);
                if (cobREAS_CD == "COB0")
                {
                    //check CAP indicator
                    if (clmCLCL_CAP_IND != "F" && clmCLCL_CAP_IND != "P")
                    {
                        //check total paid
                        double totPaidAmt = 0.00;
                        this.GetClaimLineData();
                        foreach (ClaimLine line in _claim.claimLines)
                        {
                            totPaidAmt += (line.CDML_PR_PYMT_AMT + line.CDML_SB_PYMT_AMT);
                        }

                        //Logger.LoggerInstance.ReportMessage("totPaidAmt is ", totPaidAmt.ToString());
                        //if Paid > 0; update CLCB_COB_REAS_CD to XXX; set ReProc

                        //Math.Round((dblCopayAmtToOverride / claimLine.CDML_UNITS), 2, MidpointRounding.ToEven)
                        totPaidAmt = Math.Round(totPaidAmt,2,MidpointRounding.ToEven);
                        if (totPaidAmt > 0)
                        {
                            FacetsData.FacetsInstance.SetSingleDataItem("CLCB", "CLCB_COB_REAS_CD", "XXX");
                            this.InsertREPROC();
                            FacetsData.FacetsInstance.CompleteProcess();
                        }
                    }
                }
            }
        }

        //v1.2
        /// <summary>
        /// Get No Part B Medicare Benefit calculation
        /// </summary>
        /// <param name="exitTiming"></param>
        public string GetPartBCalculation(string exitTiming, string appID, string userID, string customDB)
        {
            bool _facetsUpdate = false;
            /*CR-128 Begin*/
            //setting Boolean variable to false in the beginning 
            bool RepCount = false;
            bool RepCount1 = false;
            bool RepCountf = false;
          

            //to read XML and setting boolean variables each time during entry 
            if (exitTiming != "PREPROC")
            {

                this.GetClaimLineOverrideData();
               
                foreach (Cdor cdor in _claim.claimLineOverrides)
                {
                    //have appended CDOR_OR_VALUE with variables to inidicate the process flow
                    if (cdor.CDOR_OR_ID == "XR")
                    {
                       
                       
                        if (cdor.CDOR_OR_VALUE.EndsWith("R1"))
                        {
                            //to Indicate to REPROC FOR FIRST ITERATION (if any)
                            RepCount = true;
                        }
                        if (cdor.CDOR_OR_VALUE.EndsWith("R"))
                        {
                            //to Indicate to REPROC FOR SECOND ITERATION (if any)
                            RepCount = true;
                            RepCount1 = true;
                        }
                        if (cdor.CDOR_OR_VALUE.EndsWith("R2"))
                        {
                            //to Indicate to REPROC FOR SECOND ITERATION (if any)
                            RepCountf = true;
                            RepCount = true;
                           
                        }
 
                    }
                }
            }
            //POSTPROC
           


            //PREPROC
            if (exitTiming == "PREPROC")
            {

                //batch mode
                if (appID == "EADJ") { return string.Empty; }
                //No ReProc found 
                if (string.IsNullOrEmpty(_claim.REPROC_FLAG) || _claim.REPROC_FLAG != "Y")
                {

                    int cnt = 0;
                    //CDORALL
                    this.GetClaimLineOverrideData();
                    foreach (Cdor cdor in _claim.claimLineOverrides)
                    {
                        //remove line override; EP or XR or SR
                        if ((cdor.CDOR_OR_ID == "EP" && cdor.EXCD_ID == "A14") ||
                             (cdor.CDOR_OR_ID == "XR" && cdor.EXCD_ID == "A15") ||
                            (cdor.CDOR_OR_ID == "SR" && cdor.EXCD_ID == "A32"))         //CR-128 while entring for the first time
                        {

                            //remove this CDOR override 
                            FacetsData.FacetsInstance.RemoveSingleDataCollection("CDORALL", "CDORALL", cnt);
                            _facetsUpdate = true;
                            cnt--;  //stay at the same count; because of the remove

                        }
                        cnt++;
                    }
                }

            }
            if (exitTiming == "POSTPRICECLM")
            {
               
                string _sqlErr ="";
              

                this.GetClaimLineOverrideData();
                List<int> cdor01 = new List<int>();
                foreach (Cdor cdor in _claim.claimLineOverrides)
                {
                    //identify extension override
                    if (cdor.CDOR_OR_ID == "01")
                    {
                        cdor01.Add(cdor.CDML_SEQ_NO);
                    }
                }
                //To Exit if Dummy variables exists
                if (cdor01.Count > 0) {
                   
                    return string.Empty; }
               
                List<int> cdorEP = new List<int>();
                List<int> cdorUSR = new List<int>();
                List<int> cdorXR = new List<int>();
                foreach (Cdor cdor in _claim.claimLineOverrides)
                {
                    //identify extension override
                    if (cdor.CDOR_OR_ID == "EP")
                    {
                        if (cdor.EXCD_ID == "A14") { cdorEP.Add(cdor.CDML_SEQ_NO); }
                        else { cdorUSR.Add(cdor.CDML_SEQ_NO); }
                    }
                    //XR override
                    if (cdor.CDOR_OR_ID == "XR")
                    {
                        cdorXR.Add(cdor.CDML_SEQ_NO);
                    }
                }
                
                //Call SP wmkp_no_mecr_prtb_fltr_clm
                this.GetClaimLineData();
                string clmCLCL_LOW_SVC_DT = _claim.CLCL_LOW_SVC_DT;
                //check CLCL_LOW_SVC_DT
                if (string.IsNullOrEmpty(clmCLCL_LOW_SVC_DT) || clmCLCL_LOW_SVC_DT == "NULL" || clmCLCL_LOW_SVC_DT == "01/01/1753")
                {
                    foreach (ClaimLine line in _claim.claimLines)
                    {
                        clmCLCL_LOW_SVC_DT = line.strCDML_FROM_DT; //line.CDML_FROM_DT.ToString();
                        break;
                    }
                }

                //Filtering claim
                if (clmCLCL_LOW_SVC_DT == "NULL") { clmCLCL_LOW_SVC_DT = "01/01/1753"; }
                //EXEC fawpmdv0custom.wmkp_no_mecr_prtb_fltr_clm 7100,'H101','10/25/2015','400181818001';
                string strQuery = "EXEC " + customDB + ".dbo.wmkp_no_mecr_prtb_fltr_clm " + _claim.MEME_CK.ToString() + ",'" + _claim.PDPD_ID + "','" +
                                  clmCLCL_LOW_SVC_DT + "','" + _claim.PRPR_ID + "';";
                string strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
                string PASS = "";
                //check sql error
                _sqlErr = FacetsData.FacetsInstance.CheckDbError(strQueryResult);
                if (!string.IsNullOrEmpty(_sqlErr))
                {
                    return _sqlErr + strQuery;
                }
                //With returned result
                if (FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult))
                {
                    PASS = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "PASS", false);
                }
                //Exit if not TRUE (FALSE)
                if (PASS != "TRUE") { return string.Empty; }
                
                //Claim line loop
                //CDMLALL
                double annuDed = 0.00;
                double accumDed = 0.00;
                this.GetClaimLineData();
                _claim.claimLines.ForEach(cdml => Logger.LoggerInstance.ReportMessage("Captured Claim Line price ", " Price for Line # " + cdml.CDML_SEQ_NO.ToString() + "is ---" + cdml.CDML_ALLOW.ToString()));
                foreach (ClaimLine line in _claim.claimLines)
                {
                    //user manual EP override
                    if (cdorUSR.Contains(line.CDML_SEQ_NO)) { continue; } //skip to the next line
                    //CDML_ALLOW rounding
                    double CDML_ALLOW = Math.Round(line.CDML_ALLOW, 2, MidpointRounding.AwayFromZero);
                    //Zero Allowable
                    if (CDML_ALLOW == 0.00) { continue; } //skip to the next line

                    //line filtering logic
                    strQuery = "EXEC " + customDB + ".dbo.wmkp_no_mecr_prtb_fltr_line '" + line.IPCD_ID + "','" + line.IDCD_ID + "','" +
                                line.strCDML_FROM_DT + "','" + _claim.PRPR_ID + "';";
                    strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
                    PASS = "";
                    //check sql error
                    _sqlErr = FacetsData.FacetsInstance.CheckDbError(strQueryResult);
                    if (!string.IsNullOrEmpty(_sqlErr))
                    {
                        return _sqlErr + strQuery;
                    }
                    //With returned result
                    if (FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult))
                    {
                        PASS = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "PASS", false);
                    }
                    //Skip if not TRUE (FALSE)
                    if (PASS != "TRUE") { continue; } //skip to the next line

                    //Annual and accumulated deductible
                    if (annuDed == 0.00)
                    {
                        //annuDed
                        strQuery = "SELECT ISNULL(ANNU_DEDE_AMT, 0) AS ANNU_DEDE_AMT FROM " + customDB + ".dbo.wmkt_no_mecr_prtb_annu_ded WHERE '" + clmCLCL_LOW_SVC_DT + "' BETWEEN EFF_DT AND TERM_DT;";
                        strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
                        //check sql error
                        _sqlErr = FacetsData.FacetsInstance.CheckDbError(strQueryResult);
                        if (!string.IsNullOrEmpty(_sqlErr))
                        {
                            return _sqlErr + strQuery;
                        }
                        if (FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult))
                        {
                            annuDed = Math.Round(double.Parse(FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "ANNU_DEDE_AMT", false)), 2, MidpointRounding.AwayFromZero);
                        }
                        //accumDed
                        strQuery = "SELECT ISNULL(SUM(DED_AMT), 0) AS TOT_DED FROM " + customDB + ".dbo.wmkt_no_mecr_prtb_line_ded WHERE SBSB_CK = " + _claim.SBSB_CK +
                                   " AND MEME_CK = " + _claim.MEME_CK + " AND CLCL_CUR_STS IN ('01', '02') AND CLCL_ID NOT IN ('" +
                                   _claim.CLCL_ID_ADJ_FROM + "','" + _claim.CLCL_ID + "') " +
                                   " AND YEAR(CDML_FROM_DT) = YEAR('" + line.strCDML_FROM_DT + "');";
                        strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
                        //check sql error
                        _sqlErr = FacetsData.FacetsInstance.CheckDbError(strQueryResult);
                        if (!string.IsNullOrEmpty(_sqlErr))
                        {
                            return _sqlErr + strQuery;
                        }
                        if (FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult))
                        {
                            accumDed = Math.Round(double.Parse(FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "TOT_DED", false)), 2, MidpointRounding.AwayFromZero);
                        }
                    }
                    
                    //no annual deduct 
                    if (annuDed == 0) { return string.Empty; } //exit extension
                     if (!cdor01.Contains(line.CDML_SEQ_NO))
                     {

                         this.InsertCdorRecord(_claim.CLCL_ID, line.CDML_SEQ_NO, "01", _claim.MEME_CK, CDML_ALLOW, "CDML_ALLOW", "PTB", "01/01/1753");
                     }
                 }
                 FacetsData.FacetsInstance.CompleteProcess();
                 return string.Empty;

            }

            else if (exitTiming == "POSTPROC")
            {

                
                string _sqlErr = "";
                this.GetClaimLineOverrideData();
                bool _reproc1 = false; 
                bool _reproc2 = false;
                bool _reproc3 = true; 
                foreach (Cdor cdor in _claim.claimLineOverrides)
                {
                    //identify extension override
                    if (cdor.CDOR_OR_ID == "XR")
                    {
                        if (cdor.CDOR_OR_VALUE.EndsWith("R1"))
                        {
                            _reproc1 = true;
                        }
                        if (cdor.CDOR_OR_VALUE.EndsWith("R2"))
                        {
                            _reproc2 = true;
                        }
                        if (cdor.CDOR_OR_VALUE.EndsWith("R"))
                        {
                            _reproc3 = false;
                        }
                    }
                }
                
                //if ReProc found; exit 
                if (RepCount == false && RepCount1 == false)
                {

                    bool _ReProc = false;
                    //CDORALL
                    this.GetClaimLineOverrideData();
                    List<int> cdorEP = new List<int>();
                    List<int> cdorUSR = new List<int>();
                    List<int> cdorXR = new List<int>();
                    foreach (Cdor cdor in _claim.claimLineOverrides)
                    {
                        //identify extension override
                        if (cdor.CDOR_OR_ID == "EP")
                        {
                            if (cdor.EXCD_ID == "A14") { cdorEP.Add(cdor.CDML_SEQ_NO); }
                            else { cdorUSR.Add(cdor.CDML_SEQ_NO); }
                        }
                        //XR override
                        if (cdor.CDOR_OR_ID == "XR")
                        {
                            cdorXR.Add(cdor.CDML_SEQ_NO);
                        }
                    }

                    
                    //Call SP wmkp_no_mecr_prtb_fltr_clm
                    this.GetClaimLineData();
                    string clmCLCL_LOW_SVC_DT = _claim.CLCL_LOW_SVC_DT;
                    //check CLCL_LOW_SVC_DT
                    if (string.IsNullOrEmpty(clmCLCL_LOW_SVC_DT) || clmCLCL_LOW_SVC_DT == "NULL" || clmCLCL_LOW_SVC_DT == "01/01/1753")
                    {
                        foreach (ClaimLine line in _claim.claimLines)
                        {
                            clmCLCL_LOW_SVC_DT = line.strCDML_FROM_DT; //line.CDML_FROM_DT.ToString();
                            break;
                        }
                    }
                  
                    //Filtering claim
                    if (clmCLCL_LOW_SVC_DT == "NULL") { clmCLCL_LOW_SVC_DT = "01/01/1753"; }
                    //EXEC fawpmdv0custom.wmkp_no_mecr_prtb_fltr_clm 7100,'H101','10/25/2015','400181818001';
                    string strQuery = "EXEC " + customDB + ".dbo.wmkp_no_mecr_prtb_fltr_clm " + _claim.MEME_CK.ToString() + ",'" + _claim.PDPD_ID + "','" +
                                      clmCLCL_LOW_SVC_DT + "','" + _claim.PRPR_ID + "';";
                    string strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
                    string PASS = "";
                    //check sql error
                    _sqlErr = FacetsData.FacetsInstance.CheckDbError(strQueryResult);
                    if (!string.IsNullOrEmpty(_sqlErr))
                    {
                        return _sqlErr + strQuery;
                    }
                    //With returned result
                    if (FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult))
                    {
                        PASS = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "PASS", false);
                    }
                    //Exit if not TRUE (FALSE)
                    if (PASS != "TRUE") { return string.Empty; }
                    
                    //Claim line loop
                    //CDMLALL
                    double annuDed = 0.00;
                    double accumDed = 0.00;
                    double availDed = 0.00;
                    this.GetClaimLineData();
                    foreach (ClaimLine line in _claim.claimLines)
                    {
                        //user manual EP override
                        if (cdorUSR.Contains(line.CDML_SEQ_NO)) { continue; } //skip to the next line
                        //CDML_ALLOW rounding
                        double CDML_ALLOW = Math.Round(line.CDML_ALLOW, 2, MidpointRounding.AwayFromZero);
                        //Zero Allowable
                        if (CDML_ALLOW == 0.00) { continue; } //skip to the next line

                        //line filtering logic
                        strQuery = "EXEC " + customDB + ".dbo.wmkp_no_mecr_prtb_fltr_line '" + line.IPCD_ID + "','" + line.IDCD_ID + "','" +
                                    line.strCDML_FROM_DT + "','" + _claim.PRPR_ID + "';";
                        strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
                        PASS = "";
                        //check sql error
                        _sqlErr = FacetsData.FacetsInstance.CheckDbError(strQueryResult);
                        if (!string.IsNullOrEmpty(_sqlErr))
                        {
                            return _sqlErr + strQuery;
                        }
                        //With returned result
                        if (FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult))
                        {
                            PASS = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "PASS", false);
                        }
                        //Skip if not TRUE (FALSE)
                        if (PASS != "TRUE") { continue; } //skip to the next line
                        
                        //Annual and accumulated deductible
                        if (annuDed == 0.00)
                        {
                            //annuDed
                            strQuery = "SELECT ISNULL(ANNU_DEDE_AMT, 0) AS ANNU_DEDE_AMT FROM " + customDB + ".dbo.wmkt_no_mecr_prtb_annu_ded WHERE '" + clmCLCL_LOW_SVC_DT + "' BETWEEN EFF_DT AND TERM_DT;";
                            strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
                            //check sql error
                            _sqlErr = FacetsData.FacetsInstance.CheckDbError(strQueryResult);
                            if (!string.IsNullOrEmpty(_sqlErr))
                            {
                                return _sqlErr + strQuery;
                            }
                            if (FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult))
                            {
                                annuDed = Math.Round(double.Parse(FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "ANNU_DEDE_AMT", false)), 2, MidpointRounding.AwayFromZero);
                            }
                            //accumDed
                            strQuery = "SELECT ISNULL(SUM(DED_AMT), 0) AS TOT_DED FROM " + customDB + ".dbo.wmkt_no_mecr_prtb_line_ded WHERE SBSB_CK = " + _claim.SBSB_CK +
                                       " AND MEME_CK = " + _claim.MEME_CK + " AND CLCL_CUR_STS IN ('01', '02') AND CLCL_ID NOT IN ('" +
                                       _claim.CLCL_ID_ADJ_FROM + "','" + _claim.CLCL_ID + "') " +
                                       " AND YEAR(CDML_FROM_DT) = YEAR('" + line.strCDML_FROM_DT + "');";
                            strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
                            //check sql error
                            _sqlErr = FacetsData.FacetsInstance.CheckDbError(strQueryResult);
                            if (!string.IsNullOrEmpty(_sqlErr))
                            {
                                return _sqlErr + strQuery;
                            }
                            if (FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult))
                            {
                                accumDed = Math.Round(double.Parse(FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "TOT_DED", false)), 2, MidpointRounding.AwayFromZero);
                            }
                        }
                        //no annual deduct 
                        if (annuDed == 0) { return string.Empty; } //exit extension
                        
                        //non-applied deductible
                        availDed = Math.Round((annuDed - accumDed), 2, MidpointRounding.AwayFromZero);
                        if (availDed < 0) { availDed = 0.00; }
                        double calAllowed = 0.00;
                        double appliedDed = 0.00;
                        //compare CDML_ALLOW and availDed
                        //To get CDML_ALLOW amount from POST PRICING CLM
                        //taking CDOR_OR_VALUE value from CDOR collection
                        foreach (Cdor cdor in _claim.claimLineOverrides)
                        {
                            if (cdor.CDOR_OR_ID == "01" && cdor.EXCD_ID == "PTB" && cdor.CDML_SEQ_NO == line.CDML_SEQ_NO)
                            {
                                
                                CDML_ALLOW = cdor.CDOR_OR_AMT;
                                
                            }
                        }
                        
                       //to check if reproc is required
                        _ReProc = true;
                        if (CDML_ALLOW <= availDed)
                        {
                            
                            calAllowed = CDML_ALLOW;  //CDML_ALLOW - CDML_ALLOW = 0
                            accumDed = Math.Round((accumDed + CDML_ALLOW), 2, MidpointRounding.AwayFromZero);
                            appliedDed = CDML_ALLOW;
                            //Service Rule 000 overide
                            this.InsertCdorRecord(_claim.CLCL_ID, line.CDML_SEQ_NO, "SR", _claim.MEME_CK, appliedDed, "000", "A32", line.strCDML_FROM_DT);
                            double remainedDed = Math.Round((availDed - appliedDed), 2, MidpointRounding.AwayFromZero);
                            if (remainedDed < 0) { remainedDed = 0.00; }
                            string XR_CDOR_VAL = "Remained=" + remainedDed.ToString() + ";Applied=" + appliedDed.ToString();
                            //CDOR subcollection
                            XR_CDOR_VAL = XR_CDOR_VAL + "R1";
                            //EP; extension sepcial storage: place applied deductible to CDOR_OR_VAL, place CDML_FROM_DT to CDOR_OR_DT 
                            this.InsertCdorRecord(_claim.CLCL_ID, line.CDML_SEQ_NO, "EP", _claim.MEME_CK, calAllowed, appliedDed.ToString(), "A14", line.strCDML_FROM_DT);
                            //XR; Miscellaneous Data
                            if (!cdorXR.Contains(line.CDML_SEQ_NO))
                            {
                                this.InsertCdorRecord(_claim.CLCL_ID, line.CDML_SEQ_NO, "XR", _claim.MEME_CK, 0.00, XR_CDOR_VAL, "A15", "01/01/1753");
                            }
                        }
                        else  //CDML_ALLOW > availDed
                        {
                            calAllowed = Math.Round(((CDML_ALLOW - availDed) * 0.2), 2, MidpointRounding.AwayFromZero);
                            accumDed = Math.Round((accumDed + availDed), 2, MidpointRounding.AwayFromZero);
                            appliedDed = calAllowed;
                            
                            //Insert EP and XR override
                            double remainedDed = Math.Round(availDed, 2, MidpointRounding.AwayFromZero);
                            if (remainedDed < 0) { remainedDed = 0.00; }
                            string XR_CDOR_VAL = "Remained=" + remainedDed.ToString() + ";Applied=" + appliedDed.ToString();
                            if (remainedDed == 0.00)
                            {
                                XR_CDOR_VAL = XR_CDOR_VAL + "R1";
                                this.InsertCdorRecord(_claim.CLCL_ID, line.CDML_SEQ_NO, "EP", _claim.MEME_CK, calAllowed, appliedDed.ToString(), "A14", line.strCDML_FROM_DT);
                                //XR; Miscellaneous Data
                                if (!cdorXR.Contains(line.CDML_SEQ_NO))
                                {
                                    this.InsertCdorRecord(_claim.CLCL_ID, line.CDML_SEQ_NO, "XR", _claim.MEME_CK, 0.00, XR_CDOR_VAL, "A15", "01/01/1753");
                                }
                            }
                            else
                            {
                                //CDOR subcollection  
                                XR_CDOR_VAL = XR_CDOR_VAL + "R";
                                //EP; extension sepcial storage: place applied deductible to CDOR_OR_VAL, place CDML_FROM_DT to CDOR_OR_DT 
                                this.InsertCdorRecord(_claim.CLCL_ID, line.CDML_SEQ_NO, "EP", _claim.MEME_CK, calAllowed, appliedDed.ToString(), "A14", line.strCDML_FROM_DT);
                                //XR; Miscellaneous Data
                                if (!cdorXR.Contains(line.CDML_SEQ_NO))
                                {
                                    this.InsertCdorRecord(_claim.CLCL_ID, line.CDML_SEQ_NO, "XR", _claim.MEME_CK, 0.00, XR_CDOR_VAL, "A15", "01/01/1753");
                                }
                            }
                        }

                    }

                    
                    //calling REPROC FLAG only if overides exists else nothing should happen
                    if (_ReProc == true)
                    {

                        this.InsertREPROC();
                        if (appID != "EADJ")
                        {
                            if (!string.IsNullOrEmpty(_claim.REPROC_FLAG)) // == "N")
                            {
                              
                                FacetsData.FacetsInstance.SetSingleDataItem("CUSTOM", "REPROC_FLAG", "Y");
                            }
                            //Insert CUSTOM item 
                            else
                            {
                                
                                FacetsData.FacetsInstance.AddSingleSubCollection("CUSTOM", "<Column name=\"REPROC_FLAG\">Y</Column>");

                            }
                        }
                    }
               
                    FacetsData.FacetsInstance.CompleteProcess();
                    return string.Empty;

                }


                if (RepCount == true && RepCount1 == true)
                {
                    
                    this.GetClaimLineOverrideData();
                    List<int> cdorXR = new List<int>();
                    foreach (Cdor cdor in _claim.claimLineOverrides)
                    {
                        //identify extension override which needs to be reprocessed

                        //XR override
                        if (cdor.CDOR_OR_ID == "XR" && cdor.EXCD_ID == "A15" && cdor.CDOR_OR_VALUE.EndsWith("R"))
                        {
                            cdorXR.Add(cdor.CDML_SEQ_NO);

                        }


                    }
                    
                    this.GetClaimLineData();
                    foreach (ClaimLine line in _claim.claimLines)
                    {
                        if (cdorXR.Contains(line.CDML_SEQ_NO))
                        {
                            //take the reamining amount if any

                            string XROveride = "=0.00;";
                            double RemAmtCDOR = 0.00;
                            foreach (Cdor cdor in _claim.claimLineOverrides)
                            {

                                // EP or XR getting previous remaining amount for getting reamining amount
                                if (cdor.CDML_SEQ_NO == line.CDML_SEQ_NO && cdor.CDOR_OR_ID == "XR")
                           
                                {
                                    XROveride = cdor.CDOR_OR_VALUE;

                                }
                            }

                            
                            //Deleting the CURRENT EP ad XR overide
                            bool EPOverridesRemoved = false;
                            bool XROverridesRemoved = false;

                            try
                            {
                                List<XElement> cdorList = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL")).ToList();

                                EPOverridesRemoved = false;
                                XROverridesRemoved = false;
                                foreach (XElement element in cdorList)
                                {
                                    EPOverridesRemoved = element.Elements().Any(CdorId => CdorId.Attribute("name").Value.Equals("CDOR_OR_ID") && CdorId.Value.Equals("EP")) &&
                                                           element.Elements().Any(ExcdID => ExcdID.Attribute("name").Value.Equals("EXCD_ID") && ExcdID.Value.Equals("A14")) &&
                                                           element.Elements().Any(CdmlSeqNo => CdmlSeqNo.Attribute("name").Value.Equals("CDML_SEQ_NO") && CdmlSeqNo.Value.Equals(line.CDML_SEQ_NO.ToString())
                                                           );

                                    if (EPOverridesRemoved)
                                        element.Remove();

                                    XROverridesRemoved = element.Elements().Any(CdorId => CdorId.Attribute("name").Value.Equals("CDOR_OR_ID") && CdorId.Value.Equals("XR")) &&
                                                           element.Elements().Any(ExcdID => ExcdID.Attribute("name").Value.Equals("EXCD_ID") && ExcdID.Value.Equals("A15")) &&
                                                           element.Elements().Any(CdmlSeqNo => CdmlSeqNo.Attribute("name").Value.Equals("CDML_SEQ_NO") && CdmlSeqNo.Value.Equals(line.CDML_SEQ_NO.ToString())
                                                           );

                                    if (XROverridesRemoved)
                                        element.Remove();

                                }
                            }
                            catch (Exception ex)
                            {

                            }



                         
                            //removing only the remaining part in part i.e before ;
                            double.TryParse(XROveride.Split(';').First().Split('=')[1].Trim(), out RemAmtCDOR);

                            

                            //CR-128 POST PROC
                            double remAmt = 0.00;
                            double leftAmt = 0.00;
                            //To add available deductible amount (if any) along with claim paid amount
                            // Fetching available deductible amount (if any) and Adding the claim paid amount with Availble Dedutible amount
                            if (RemAmtCDOR > 0)
                            {
                                remAmt = RemAmtCDOR + Math.Round(line.CDML_PAID_AMT, 2, MidpointRounding.AwayFromZero);


                            }
                            else
                            {
                                remAmt = Math.Round(line.CDML_PAID_AMT, 2, MidpointRounding.AwayFromZero);

                            }
                            // "EP","XR"and Service Rule override 
                            //adding R2 for all overides to indicate REPROC has to be done

                            string PXR_CDOR_VAL = "Remained=" + leftAmt.ToString() + ";" + "Applied=" + RemAmtCDOR.ToString() + "R2";
                            this.InsertCdorRecord(_claim.CLCL_ID, line.CDML_SEQ_NO, "SR", _claim.MEME_CK, remAmt, "000", "A32", line.strCDML_FROM_DT);
                            this.InsertCdorRecord(_claim.CLCL_ID, line.CDML_SEQ_NO, "EP", _claim.MEME_CK, remAmt, remAmt.ToString(), "A14", line.strCDML_FROM_DT);
                            //XR; Miscellaneous Data
                            this.InsertCdorRecord(_claim.CLCL_ID, line.CDML_SEQ_NO, "XR", _claim.MEME_CK, 0.00, PXR_CDOR_VAL, "A15", "01/01/1753");



                        }

                    }
                   
                    this.InsertREPROC();
                    if (appID != "EADJ")
                    {
                        //Update CUSTOM collection
                        if (!string.IsNullOrEmpty(_claim.REPROC_FLAG)) // == "N")
                        {
                            FacetsData.FacetsInstance.SetSingleDataItem("CUSTOM", "REPROC_FLAG", "Y");
                        }
                        //Insert CUSTOM item 
                        else
                        {
                            FacetsData.FacetsInstance.AddSingleSubCollection("CUSTOM", "<Column name=\"REPROC_FLAG\">Y</Column>");
                        }
                    }
                    
                    FacetsData.FacetsInstance.CompleteProcess();
                    return string.Empty;
            }
            // To check REPROC Flag and set to 'N'
                if (RepCount == true && RepCount1 == false && _reproc1 == true && _reproc2 == false && RepCountf == false && _reproc3 == true)
            {
              
                int cnt = 0;
                this.GetClaimLineOverrideData();
              
                foreach (Cdor cdor in _claim.claimLineOverrides)
                {
                    if (cdor.CDOR_OR_ID == "XR")
                    {
                        if (cdor.CDOR_OR_VALUE.EndsWith("R1"))
                        {
                            //to remove the existing indicator and "R1" the entire claims comes under 
                            cdor.CDOR_OR_VALUE = cdor.CDOR_OR_VALUE.Substring(0, cdor.CDOR_OR_VALUE.Length - 2);

                            //updating XML with New value M
                            List<FacetsData.ItemInfo> objListSearch = new List<FacetsData.ItemInfo>();
                            objListSearch.Add(new FacetsData.ItemInfo("CDML_SEQ_NO", cdor.CDML_SEQ_NO.ToString()));
                            objListSearch.Add(new FacetsData.ItemInfo("CDOR_OR_ID", cdor.CDOR_OR_ID));

                            List<FacetsData.ItemInfo> objListTarget = new List<FacetsData.ItemInfo>();
                            objListTarget.Add(new FacetsData.ItemInfo("CDOR_OR_VALUE", cdor.CDOR_OR_VALUE));
                            //set data
                            FacetsData.FacetsInstance.SetSingleDataItem(XA9Constants.CDOR_DATAID, XA9Constants.CDOR_DATAID, objListSearch, objListTarget);

                        }

                    }
                }
                
                //CDORALL
                foreach (Cdor cdor in _claim.claimLineOverrides)
                {
                    //remove line override; EP or XR or SR
                    if (cdor.CDOR_OR_ID == "01" && cdor.EXCD_ID == "PTB")
                    //CR-128 while entring for the first time
                    {
                        //remove this CDOR override 
                        FacetsData.FacetsInstance.RemoveSingleDataCollection("CDORALL", "CDORALL", cnt);
                        cnt--;  //stay at the same count; because of the remove

                    }
                    cnt++;
                }
              
                //Blank out REPROC_FLAG in CUSTOM data collection 
                if (appID != "EADJ")
                {
                    FacetsData.FacetsInstance.SetSingleDataItem("CUSTOM", "REPROC_FLAG", "N");
                }
                
                FacetsData.FacetsInstance.CompleteProcess();
                //exit
                return string.Empty;

            }
                if (RepCountf == true && _reproc1 == false && _reproc2 == true &&  _reproc3 == true)
            {
                int cnt = 0;
               
                this.GetClaimLineOverrideData();
                foreach (Cdor cdor in _claim.claimLineOverrides)
                {
                    if (cdor.CDOR_OR_ID == "XR")
                    {
                        if (cdor.CDOR_OR_VALUE.EndsWith("R2"))
                        {
                            //to remove the existing indicator and "R1" the entire claims comes under 
                            cdor.CDOR_OR_VALUE = cdor.CDOR_OR_VALUE.Substring(0, cdor.CDOR_OR_VALUE.Length - 2);

                            //updating XML with New value M
                            List<FacetsData.ItemInfo> objListSearch = new List<FacetsData.ItemInfo>();
                            objListSearch.Add(new FacetsData.ItemInfo("CDML_SEQ_NO", cdor.CDML_SEQ_NO.ToString()));
                            objListSearch.Add(new FacetsData.ItemInfo("CDOR_OR_ID", cdor.CDOR_OR_ID));

                            List<FacetsData.ItemInfo> objListTarget = new List<FacetsData.ItemInfo>();
                            objListTarget.Add(new FacetsData.ItemInfo("CDOR_OR_VALUE", cdor.CDOR_OR_VALUE));
                            //set data
                            FacetsData.FacetsInstance.SetSingleDataItem(XA9Constants.CDOR_DATAID, XA9Constants.CDOR_DATAID, objListSearch, objListTarget);

                        }

                    }
                }
                
                //CDORALL
                foreach (Cdor cdor in _claim.claimLineOverrides)
                {
                    //remove line override; EP or XR or SR
                    if (cdor.CDOR_OR_ID == "01" && cdor.EXCD_ID == "PTB")
                    //CR-128 while entring for the first time
                    {
                        //remove this CDOR override 
                        FacetsData.FacetsInstance.RemoveSingleDataCollection("CDORALL", "CDORALL", cnt);
                        cnt--;  //stay at the same count; because of the remove

                    }
                    cnt++;
                }
                Logger.LoggerInstance.ReportMessage("POST PROC exit BLOCK 2 ", "After dummy CDOR Deletion");
                //Blank out REPROC_FLAG in CUSTOM data collection 
                if (appID != "EADJ")
                {
                    FacetsData.FacetsInstance.SetSingleDataItem("CUSTOM", "REPROC_FLAG", "N");
                }
               
                FacetsData.FacetsInstance.CompleteProcess();
                //exit
                return string.Empty;

            }
        
            if (RepCountf == true && _reproc1 == true && _reproc2 == true)
            {
                int cnt = 0;
           
                this.GetClaimLineOverrideData();
                foreach (Cdor cdor in _claim.claimLineOverrides)
                {
                    if (cdor.CDOR_OR_ID == "XR")
                    {
                        if (cdor.CDOR_OR_VALUE.EndsWith("R2"))
                        {
                            //to remove the existing indicator and "R1" the entire claims comes under 
                            cdor.CDOR_OR_VALUE = cdor.CDOR_OR_VALUE.Substring(0, cdor.CDOR_OR_VALUE.Length - 2);

                            //updating XML with New value M
                            List<FacetsData.ItemInfo> objListSearch = new List<FacetsData.ItemInfo>();
                            objListSearch.Add(new FacetsData.ItemInfo("CDML_SEQ_NO", cdor.CDML_SEQ_NO.ToString()));
                            objListSearch.Add(new FacetsData.ItemInfo("CDOR_OR_ID", cdor.CDOR_OR_ID));

                            List<FacetsData.ItemInfo> objListTarget = new List<FacetsData.ItemInfo>();
                            objListTarget.Add(new FacetsData.ItemInfo("CDOR_OR_VALUE", cdor.CDOR_OR_VALUE));
                            //set data
                            FacetsData.FacetsInstance.SetSingleDataItem(XA9Constants.CDOR_DATAID, XA9Constants.CDOR_DATAID, objListSearch, objListTarget);

                        }
                        if (cdor.CDOR_OR_VALUE.EndsWith("R1"))
                        {
                            //to remove the existing indicator and "R1" the entire claims comes under 
                            cdor.CDOR_OR_VALUE = cdor.CDOR_OR_VALUE.Substring(0, cdor.CDOR_OR_VALUE.Length - 2);

                            //updating XML with New value M
                            List<FacetsData.ItemInfo> objListSearch = new List<FacetsData.ItemInfo>();
                            objListSearch.Add(new FacetsData.ItemInfo("CDML_SEQ_NO", cdor.CDML_SEQ_NO.ToString()));
                            objListSearch.Add(new FacetsData.ItemInfo("CDOR_OR_ID", cdor.CDOR_OR_ID));

                            List<FacetsData.ItemInfo> objListTarget = new List<FacetsData.ItemInfo>();
                            objListTarget.Add(new FacetsData.ItemInfo("CDOR_OR_VALUE", cdor.CDOR_OR_VALUE));
                            //set data
                            FacetsData.FacetsInstance.SetSingleDataItem(XA9Constants.CDOR_DATAID, XA9Constants.CDOR_DATAID, objListSearch, objListTarget);

                        }

                    }
                }
                
                //CDORALL
                foreach (Cdor cdor in _claim.claimLineOverrides)
                {
                    //remove line override; EP or XR or SR
                    if (cdor.CDOR_OR_ID == "01" && cdor.EXCD_ID == "PTB")
                    //CR-128 while entring for the first time
                    {
                        //remove this CDOR override 
                        FacetsData.FacetsInstance.RemoveSingleDataCollection("CDORALL", "CDORALL", cnt);
                        cnt--;  //stay at the same count; because of the remove

                    }
                    cnt++;
                }
                
                //Blank out REPROC_FLAG in CUSTOM data collection 
                if (appID != "EADJ")
                {
                    FacetsData.FacetsInstance.SetSingleDataItem("CUSTOM", "REPROC_FLAG", "N");
                }
                
                FacetsData.FacetsInstance.CompleteProcess();
                //exit
                return string.Empty;

            }
        }
    
    
        
            //PRESAVE
            else if (exitTiming == "PRESAVE") // || exitTiming == "MENU")  //testing
            {
                string strQuery = "";
                string strQueryResult = "";
                string _sqlErr = "";

                //CDORALL
                this.GetClaimLineOverrideData();
                List<int> cdorEP = new List<int>();
                foreach (Cdor cdor in _claim.claimLineOverrides)
                {
                    //identify extension override
                    if (cdor.CDOR_OR_ID == "EP" && cdor.EXCD_ID == "A14")
                    {
                        cdorEP.Add(cdor.CDML_SEQ_NO);
                    }
                }

                //Batch; No extension EP override; exit
                if (cdorEP.Count == 0 && appID == "EADJ") { return string.Empty; }

                //CDMLALL
                this.GetClaimLineData();
                //Obtain CLCL_LOW_SVC_DT
                string clmCLCL_LOW_SVC_DT = _claim.CLCL_LOW_SVC_DT;
                //check CLCL_LOW_SVC_DT
                if (string.IsNullOrEmpty(clmCLCL_LOW_SVC_DT) || clmCLCL_LOW_SVC_DT == "NULL" || clmCLCL_LOW_SVC_DT == "01/01/1753")
                {
                    foreach (ClaimLine line in _claim.claimLines)
                    {
                        clmCLCL_LOW_SVC_DT = line.strCDML_FROM_DT; //line.CDML_FROM_DT.ToString();
                        break;
                    }
                }

                //Get annual deductible
                //SELECT ANNU_DEDE_AMT FROM fawpmdv0custom.wmkt_no_mecr_prtb_annu_ded WHERE CLCL_LOW_SVC_DT BETWEEN EFF_DT AND TERM_DT
                strQuery = "SELECT ANNU_DEDE_AMT FROM " + customDB + ".dbo.wmkt_no_mecr_prtb_annu_ded WHERE '" + clmCLCL_LOW_SVC_DT + "' BETWEEN EFF_DT AND TERM_DT;";
                strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
                //check sql error
                _sqlErr = FacetsData.FacetsInstance.CheckDbError(strQueryResult);
                if (!string.IsNullOrEmpty(_sqlErr))
                {
                    return _sqlErr + strQuery;
                }
                string ANNU_DEDE_AMT = "0.00";
                if (FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult))
                {
                    ANNU_DEDE_AMT = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "ANNU_DEDE_AMT", false);
                }

                //Line loop
                foreach (ClaimLine line in _claim.claimLines)
                {
                    if (!cdorEP.Contains(line.CDML_SEQ_NO))
                    {
                        //Online; No extension EP override at particular line; clean-up
                        if (appID != "EADJ")
                        {
                            //DELETE fawpmdv0custom.wmkt_no_mecr_prtb_line_ded WHERE CLCL_ID = pCLCL_ID AND CDML_SEQ_NO = pCDML_SEQ_NO;
                            strQuery = "DELETE " + customDB + ".dbo.wmkt_no_mecr_prtb_line_ded WHERE CLCL_ID = '" + _claim.CLCL_ID + "' AND CDML_SEQ_NO = " + line.CDML_SEQ_NO.ToString() + ";";
                            strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
                            //check sql error
                            _sqlErr = FacetsData.FacetsInstance.CheckDbError(strQueryResult);
                            if (!string.IsNullOrEmpty(_sqlErr))
                            {
                                return _sqlErr + strQuery;
                            }
                        }
                    }
                }

                //CDOR Loop
                foreach (Cdor cdor in _claim.claimLineOverrides)
                {
                    //With extension EP override
                    if (cdorEP.Contains(cdor.CDML_SEQ_NO) && cdor.CDOR_OR_ID == "EP" && cdor.EXCD_ID == "A14")
                    {
                        //EP override; extension sepcial storage: place applied deductible to CDOR_OR_VAL, place CDML_FROM_DT to CDOR_OR_DT 
                        double DED_AMT = 0.00;
                        if (!string.IsNullOrEmpty(cdor.CDOR_OR_VALUE))
                        {
                            DED_AMT = Math.Round(Double.Parse(cdor.CDOR_OR_VALUE), 2, MidpointRounding.AwayFromZero);
                        }
                        //call sp to update custom ded table                        
                        //EXEC fawpmdv0custom.wmkp_no_mecr_prtb_line_ded_updt 7100,7100,166.00,'090120000000', '01',1,'12/25/2015',50.00,'liuw';
                        strQuery = "EXEC " + customDB + ".dbo.wmkp_no_mecr_prtb_line_ded_updt " + _claim.SBSB_CK.ToString() + "," + _claim.MEME_CK.ToString()
                                + "," + ANNU_DEDE_AMT + ",'" + _claim.CLCL_ID + "','" + _claim.CLST_STS + "'," + cdor.CDML_SEQ_NO.ToString()
                                + ",'" + cdor.CDOR_OR_DT + "'," + DED_AMT.ToString() + ",'" + userID + "';";
                        strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
                        //check sql error
                        _sqlErr = FacetsData.FacetsInstance.CheckDbError(strQueryResult);
                        if (!string.IsNullOrEmpty(_sqlErr))
                        {
                            return _sqlErr + strQuery;
                        }
                    }
                }

                //adjustment
                if (!string.IsNullOrEmpty(_claim.CLCL_ID_ADJ_FROM))
                {
                    //UPDATE fawpmdv0custom.wmkt_no_mecr_prtb_line_ded SET CLCL_CUR_STS = '91' WHERE CLCL_ID = pCLCL_ID_ADJ_FROM;
                    strQuery = "UPDATE " + customDB + ".dbo.wmkt_no_mecr_prtb_line_ded SET CLCL_CUR_STS = '91' WHERE CLCL_ID = '" + _claim.CLCL_ID_ADJ_FROM + "';";
                    strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
                    //check sql error
                    _sqlErr = FacetsData.FacetsInstance.CheckDbError(strQueryResult);
                    if (!string.IsNullOrEmpty(_sqlErr))
                    {
                        return _sqlErr + strQuery;
                    }
                }

                /* CR 128 - Check if the dummy CDOR of 01 is still there. If it is there then remove it - Begin */

                if (_claim.claimLineOverrides.Any(cdor => cdor.CDOR_OR_ID.Equals("01")))
                {
                    this.RemoveClaimLineOverrides(new string[] { "01" });
                    _facetsUpdate = true;
                }
                /* CR 128 - Check if the dummy CDOR of 01 is still there. If it is there then remove it - end */
            }

            //Final Facets xml update before exit
            if (_facetsUpdate == true)
            {
                FacetsData.FacetsInstance.CompleteProcess();
            }

            //final return
            return string.Empty;
        }


        /// <summary>
        /// Get Claim Line (CDOR) Override Data when needed
        /// </summary>
        public void GetClaimLineOverrideData()
        {
            GetClaimData();
            if (_claim.claimLineOverrides == null)
            {
                _claim.claimLineOverrides = new List<Cdor>();
                //List<XElement> d = FacetsData.FacetsInstance.GetMultipleDataElements("CDORALL", "CDORALL");
                FacetsData.FacetsInstance.GetMultipleDataElements("CDORALL", "CDORALL")
                .ForEach(element => _claim.claimLineOverrides.Add(new Cdor()
                {
                    CLCL_ID = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CLCL_ID").Value,
                    CDML_SEQ_NO = int.Parse(element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDML_SEQ_NO").Value),
                    MEME_CK = int.Parse(element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "MEME_CK").Value),
                    CDOR_OR_ID = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDOR_OR_ID").Value,
                    CDOR_OR_AMT = double.Parse(element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDOR_OR_AMT").Value),
                    CDOR_OR_VALUE = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDOR_OR_VALUE").Value,
                    CDOR_OR_DT = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDOR_OR_DT").Value,
                    EXCD_ID = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "EXCD_ID").Value,
                    CDOR_OR_USID = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CDOR_OR_USID").Value
                }));
            }
        }

        /// <summary>
        /// Get Claim (CLOR) Override Data when needed
        /// </summary>
        public void GetClaimOverrideData()
        {
            GetClaimData();
            if (_claim.claimOverrides == null)
            {
                _claim.claimOverrides = new List<CLOR>();
                //List<XElement> d = FacetsData.FacetsInstance.GetMultipleDataElements("CDORALL", "CDORALL");
                FacetsData.FacetsInstance.GetMultipleDataElements("CLOR", "CLOR")
                .ForEach(element => _claim.claimOverrides.Add(new CLOR()
                {
                    CLCL_ID = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CLCL_ID").Value,
                    MEME_CK = int.Parse(element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "MEME_CK").Value),
                    CLOR_OR_ID = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CLOR_OR_ID").Value,
                    CLOR_OR_AMOUNT = double.Parse(element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CLOR_OR_AMOUNT").Value),
                    CLOR_OR_VALUE = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CLOR_OR_VALUE").Value,
                    CLOR_OR_DT = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CLOR_OR_DT").Value,
                    EXCD_ID = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "EXCD_ID").Value,
                    CLOR_OR_USID = element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == "CLOR_OR_USID").Value
                }));
            }
        }


        /// <summary>
        /// Insert a new override record into CDOR
        /// </summary>
        /// <param name="strClaimID"></param>
        /// <param name="intLineSeqNo"></param>
        /// <param name="strCdorID"></param>
        /// <param name="intMemeCK"></param>
        /// <param name="dblCdorAmt"></param>
        /// <param name="strCdorVal"></param>
        /// <param name="strExcdID"></param>
        internal void InsertCdorRecord(string strClaimID, int intLineSeqNo, string strCdorID, int intMemeCK, double dblCdorAmt, string strCdorVal, string strExcdID)
        {
            string _strXmlRec = "";
            _strXmlRec = "<SubCollection name=\"CDORALL\" type=\"Row\">" +
                         "<Column name=\"_Class\">CMC_APPREC_CDOR</Column>" +
                         "<Column name=\"_Lib\">ccllrec0481.dll</Column>" +
                         "<Column name=\"_Modified\">0</Column>" +
                         "<Column name=\"_AccessFunc\">0</Column>" +
                         "<Column name=\"LOCK_TOKEN\">1</Column>" +
                         "<Column name=\"CLCL_ID\">" + strClaimID+ "</Column>" +
                         "<Column name=\"CDML_SEQ_NO\">" + intLineSeqNo + "</Column>" +
                         "<Column name=\"CDOR_OR_ID\">" + strCdorID + "</Column>" +
                         "<Column name=\"MEME_CK\">" + intMemeCK+ "</Column>" +
                         "<Column name=\"CDOR_OR_AMT\">" + dblCdorAmt + "</Column>" +
                         "<Column name=\"CDOR_OR_VALUE\">" + strCdorVal + "</Column>" +
                         "<Column name=\"CDOR_OR_DT\">" + System.DateTime.Now.Date.ToString() + "</Column>" +
                         "<Column name=\"EXCD_ID\">" + strExcdID + "</Column>" +
                         "<Column name=\"CDOR_OR_USID\">" + ContextData.ContextInstance.UserId + "</Column>" +
                         "<Column name=\"CDOR_AUTO_GEN\"></Column>" +
                         "<Column name=\"CDOR_LOCK_TOKEN\">0</Column>" +
                         "</SubCollection>";
            FacetsData.FacetsInstance.AddSingleSubCollection("CDORALL", _strXmlRec);
            
            //Insert xml node
        }



        /// <summary>
        /// Insert a CLOR record into Facets Extention data
        /// </summary>
        /// <param name="strClaimID">CLCL_ID</param>
        /// <param name="strClorID">CLOR_OR_ID</param>
        /// <param name="intMemeCK">MEME_CK</param>
        /// <param name="dblClorAmount">CLOR_OR_AMOUNT</param>
        /// <param name="strClorVal">CLOR_OR_VALUE</param>
        /// <param name="strExcdID">EXCD_ID</param>
        private void InsertClorRecord(string strClaimID, string strClorID, string intMemeCK, string dblClorAmount, string strClorVal, string strExcdID)
        {
            string _strXmlRec = "";
            /*
             <SubCollection name="CLOR" type="Row">
      <Column name="_Class">CMC_APPREC_CLOR</Column>
      <Column name="_Lib">ccllrec0.dll</Column>
      <Column name="_Modified">1</Column>
      <Column name="_AccessFunc">1</Column>
      <Column name="LOCK_TOKEN">1</Column>
      <Column name="CLCL_ID"></Column>
      <Column name="CLOR_OR_ID">BH</Column>
      <Column name="MEME_CK">0</Column>
      <Column name="CLOR_OR_AMOUNT">0.0000</Column>
      <Column name="CLOR_OR_VALUE"></Column>
      <Column name="CLOR_OR_DT">NULL</Column>
      <Column name="EXCD_ID">059</Column>
      <Column name="CLOR_OR_USID">jayaramanv</Column>
      <Column name="CLOR_AUTO_GEN">S</Column>
      <Column name="CLOR_LOCK_TOKEN">0</Column>
    </SubCollection>
            */
            _strXmlRec = "<SubCollection name=\"CLOR\" type=\"Row\">" +
                         "<Column name=\"_Class\">CMC_APPREC_CLOR</Column>" +
                         "<Column name=\"_Lib\">ccllrec0.dll</Column>" +
                         "<Column name=\"_Modified\">0</Column>" +
                         "<Column name=\"_AccessFunc\">0</Column>" +
                         "<Column name=\"LOCK_TOKEN\">1</Column>" +
                         "<Column name=\"CLCL_ID\">" + strClaimID + "</Column>" +
                         "<Column name=\"CLOR_OR_ID\">" + strClorID + "</Column>" +
                         "<Column name=\"MEME_CK\">" + intMemeCK + "</Column>" +
                         "<Column name=\"CLOR_OR_AMOUNT\">" + dblClorAmount + "</Column>" +
                         "<Column name=\"CLOR_OR_VALUE\">" + strClorVal + "</Column>" +
                         //"<Column name=\"CLOR_OR_DT\">" + System.DateTime.Now.Date.ToString() + "</Column>" +
                         "<Column name=\"CLOR_OR_DT\">" + "NULL" + "</Column>" +
                         "<Column name=\"EXCD_ID\">" + strExcdID + "</Column>" +
                         "<Column name=\"CLOR_OR_USID\">" + ContextData.ContextInstance.UserId + "</Column>" +
                         "<Column name=\"CLOR_AUTO_GEN\"></Column>" +
                         "<Column name=\"CLOR_LOCK_TOKEN\">0</Column>" +
                         "</SubCollection>";
            FacetsData.FacetsInstance.AddSingleSubCollection("CLOR", _strXmlRec);

            //Insert xml node
        }

        //v1.2
        //with CDOR_OR_DT
        private void InsertCdorRecord(string strClaimID, int intLineSeqNo, string strCdorID, int intMemeCK, double dblCdorAmt, string strCdorVal, string strExcdID, string strCdorDt)
        {
            string _strXmlRec = "";
            _strXmlRec = "<SubCollection name=\"CDORALL\" type=\"Row\">" +
                         "<Column name=\"_Class\">CMC_APPREC_CDOR</Column>" +
                         "<Column name=\"_Lib\">ccllrec0481.dll</Column>" +
                         "<Column name=\"_Modified\">0</Column>" +
                         "<Column name=\"_AccessFunc\">0</Column>" +
                         "<Column name=\"LOCK_TOKEN\">1</Column>" +
                         "<Column name=\"CLCL_ID\">" + strClaimID + "</Column>" +
                         "<Column name=\"CDML_SEQ_NO\">" + intLineSeqNo + "</Column>" +
                         "<Column name=\"CDOR_OR_ID\">" + strCdorID + "</Column>" +
                         "<Column name=\"MEME_CK\">" + intMemeCK + "</Column>" +
                         "<Column name=\"CDOR_OR_AMT\">" + dblCdorAmt + "</Column>" +
                         "<Column name=\"CDOR_OR_VALUE\">" + strCdorVal + "</Column>" +
                         "<Column name=\"CDOR_OR_DT\">" + strCdorDt + "</Column>" +
                         "<Column name=\"EXCD_ID\">" + strExcdID + "</Column>" +
                         "<Column name=\"CDOR_OR_USID\">" + ContextData.ContextInstance.UserId + "</Column>" +
                         "<Column name=\"CDOR_AUTO_GEN\"></Column>" +
                         "<Column name=\"CDOR_LOCK_TOKEN\">0</Column>" +
                         "</SubCollection>";
            FacetsData.FacetsInstance.AddSingleSubCollection("CDORALL", _strXmlRec);

            //Insert xml node
        }

        /// <summary>
        /// Insert a new CDMD record
        /// </summary>
        /// <param name="strClaimID"></param>
        /// <param name="intLineSeqNo"></param>
        /// <param name="strCdmdType"></param>
        /// <param name="intMemeCK"></param>
        /// <param name="dblCdmdDisAllAmt"></param>
        /// <param name="strCdorVal"></param>
        /// <param name="strExcdID"></param>
        /// <returns></returns>
        private string InsertCdmdRecord(string strClaimID, string intLineSeqNo, string strCdmdType, string intMemeCK, string dblCdmdDisAllAmt, string strCdorVal, string strExcdID)
        {
            string _strXmlRec = "";
            _strXmlRec = "<SubCollection name=\"CDMDALL\" type=\"Row\">" +
                         "<Column name=\"_Class\">CMC_APPREC_CDMD</Column>" +
                         "<Column name=\"_Lib\">ccllrec0.dll</Column>" +
                         "<Column name=\"_Modified\">0</Column>" +
                         "<Column name=\"_AccessFunc\">0</Column>" +
                         "<Column name=\"LOCK_TOKEN\">1</Column>" +
                         "<Column name=\"CLCL_ID\">" + strClaimID + "</Column>" +
                         "<Column name=\"CDML_SEQ_NO\">" + intLineSeqNo + "</Column>" +
                         "<Column name=\"CDMD_TYPE\">" + strCdmdType + "</Column>" +
                         "<Column name=\"MEME_CK\">" + intMemeCK + "</Column>" +
                         "<Column name=\"CDMD_DISALL_AMT\">" + dblCdmdDisAllAmt + "</Column>" +
                         "<Column name=\"EXCD_ID\">" + strExcdID + "</Column>" +
                         "<Column name=\"CDMD_LOCK_TOKEN\"></Column>" +
                         "<Column name=\"ATXR_SOURCE_ID\">NULL</Column>" +
                         "</SubCollection>";
            FacetsData.FacetsInstance.AddSingleSubCollection("CDMDALL", _strXmlRec);
            return _strXmlRec;

            //Insert xml node
        }

        /// <summary>
        /// Insert a new CDIM record
        /// </summary>
        /// <param name="strClaimID"></param>
        /// <param name="intLineSeqNo"></param>
        /// <param name="strCdmdType"></param>
        /// <param name="intMemeCK"></param>
        /// <param name="dblCdmdDisAllAmt"></param>
        /// <param name="strCdorVal"></param>
        /// <param name="strExcdID"></param>
        /// <returns></returns>
        private string InsertCdimRecord(string strClaimID, string strLineSeqNo, string strCdimMsgCD, string strCdimType, string strMemeCK, string strCdimMapping, string strCdimAmt, string strCdimPct, string strCdimVal, string strCdimAddlData)
        {
            string _strXmlRec = "";
            _strXmlRec = "<SubCollection name=\"CDIMALL\" type=\"Row\">" +
                         "<Column name=\"_Class\">CMC_APPREC_CDIM</Column>" +
                         "<Column name=\"_Lib\">ccllrec0.dll</Column>" +
                         "<Column name=\"_Modified\">1</Column>" +
                         "<Column name=\"_AccessFunc\">1</Column>" +
                         "<Column name=\"LOCK_TOKEN\">1</Column>" +
                         "<Column name=\"CLCL_ID\">" + strClaimID + "</Column>" +
                         "<Column name=\"CDML_SEQ_NO\">" + strLineSeqNo + "</Column>" +
                         "<Column name=\"CDIM_TYP\">" + strCdimType + "</Column>" +
                         "<Column name=\"CDIM_ITS_MSG_CD\">" + strCdimMsgCD + "</Column>" +
                         "<Column name=\"MEME_CK\">" + strMemeCK + "</Column>" +
                         "<Column name=\"CDIM_MAPPING_IND\">" + strCdimMapping + "</Column>" +
                         "<Column name=\"CDIM_AMT\">" + strCdimAmt + "</Column>" +
                         "<Column name=\"CDIM_PCT\">" + strCdimPct + "</Column>" +
                         "<Column name=\"CDIM_VALUE\">" + strCdimVal + "</Column>" +
                         "<Column name=\"CDIM_ADDL_DATA\">" + strCdimAddlData + "</Column>" +
                         "<Column name=\"CDIM_LOCK_TOKEN\">0</Column>" +
                         "</SubCollection>";
            FacetsData.FacetsInstance.AddSingleSubCollection("CDIMALL", _strXmlRec);
            return _strXmlRec;

            //Insert xml node
        }

        /// <summary>
        /// Insert Facets extension REPROC xml collection
        /// </summary>
        internal void InsertREPROC()
        {
            //Insert xml node
            FacetsData.FacetsInstance.AddSingleCollection(XA9Constants.XML_REPROC_COLLECTION, XA9Constants.TOP);
        }

        /// <summary>
        /// Sets the two character Claim Exception code for the claim
        /// </summary>
        /// <param name="strClaimID">Claim ID (CLCL_ID)</param>
        /// <param name="strStopLoss">Two Character Claims Exception code</param>
        /// <returns></returns>
        public string SetStopLossForClaim(string strClaimID, string strStopLoss)
        {
            string strQuery = string.Empty;
            XA9DataLayer dataLayer = null;
            try
            {
                dataLayer = new XA9DataLayer();
                dataLayer.SetStopLossForClaim(strClaimID, strStopLoss, ContextData.ContextInstance.UserId, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQuery);
                //Logger.LoggerInstance.ReportMessage("Set Stop Loss Query is ", strQuery);
            }
            catch (Exception ex)
            {
            }
            return strQuery;
        }

         /// <summary>
        /// This method determines whether to Deny the DME claim
        /// </summary>
        /// <returns></returns>
        public bool ProcessDME()
        {
            bool blnProcessDME = false;
            string strQuery = string.Empty;
            string strAggregatedProcCodes = string.Empty;
            XA9DataLayer dataLayer = null;

            try
            {
                dataLayer = new XA9DataLayer();
                strAggregatedProcCodes = "'" + string.Join(",", _claim.UniqueNotNullProcedureCodes) + "'"; 

                //strQuery = dataLayer.GetDMECount(strAggregatedProcCodes, strAggregatedDiagCodes, strPrprNPI, _claim.CLCL_LOW_SVC_DT, _claim.PDPD_ID, strITSSFMsgs, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQueryResult); // Defect 2231
                    //Logger.LoggerInstance.ReportMessage("GetBDCInfoForClaimLine query for proc and diag is : ", strQuery);
                    //Logger.LoggerInstance.ReportMessage("Data for Proc and Diag is : ", strQueryResult);

                    //Logger.LoggerInstance.ReportMessage("GetBDCInfoForClaimLine query for proc and diag is : ", strQuery);
                    //recordsReturned = FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult);

            }
            catch (Exception ex)
            {

            }
            return blnProcessDME;
        }

        /// <summary>
        /// Set BDC TOS Override
        /// </summary>
        /// <returns></returns>
        public bool SetClaimTOSOverride()
        {
            bool isITSClaim = false; // if the claim is an ITS claim then this will be true
            bool setTOSOverride = false; // If TOS override is required then this will be true. If true, a new CDOR record will be created and inserted with derived TOS
            bool saveBDCPostSave = false; // For New / Adjusted online claims, set this flag to true to BDC can be saved during POSTSAVE


            bool recordsReturned = false;
            string strQueryResult = string.Empty;
            string strBdcFlag = string.Empty;
            XDocument xdocBDCResult = null;
            XA9DataLayer dataLayer = null;
            string strBenefitLevelForTOS = string.Empty; // N = L (Low Benefit), Y = H (High Benefit)
            bool blnBdcSaved = false;

            string strProcBdcPmdId = string.Empty;  // BDC Program ID for the procedure code
            string strProcBdcSubPgmId = string.Empty; // BDC Sub Program ID for the procedure code
            string strBdcProgName = string.Empty; // BDC Program 
            string strProvBdcPmdId = string.Empty;
            string strProvBdcSubPgmId = string.Empty;
            string strBdcSfMsgCode = string.Empty;
            string strProductSfMsgCode = String.Empty;
            string strProductTOSValue = string.Empty;
            string strPrprNPI = string.Empty;
            string strDerivedTOS = string.Empty;
            string strQuery = string.Empty;
            string[] strPdpdAttch = null;
            string strClaimProcCode = string.Empty;
            bool isBDCPlus = false;

            // Defect 2235 -- Begin
            string strITSSFMsgs = string.Empty;
            // Defect 2235 -- End
            try
            {
                GetClaimLineData();
                GetClaimLineOverrideData();
                GetClhpData();
                if (_claim.CLCL_PRE_PRICE_IND == "H")
                {
                    GetClaimITSData();
                    //strAggregatedProcCodes = lstProcCodesForCurrentClaim.Aggregate((start, end) => "'" + start + "'" + ", " + "'" + end + "'");
                    //strITSSFMsgs = this._claim.claimITS.Select(its => its.CLIM_ITS_MSG_CD).ToList().Aggregate((start,end) => "'" + start + "'" + "," +"'" + end + "'");
                    strITSSFMsgs = string.Join(",", this._claim.claimITS.Select(its => its.CLIM_ITS_MSG_CD));
                    strITSSFMsgs = "'" + strITSSFMsgs + "'";
                    //Logger.LoggerInstance.ReportMessage("SetClaimTOSOverride", "ITS SF Messages from the claim is " + strITSSFMsgs);
                    isITSClaim = true;
                }
                else
                {
                    strITSSFMsgs = "''";
                    //strPrprNPI = FacetsData.FacetsInstance.GetSingleDataItem("PRV0", "PRPR_NPI", false);
                    strPrprNPI = FacetsData.FacetsInstance.GetSingleDataItem("SVCPR", "PRPR_NPI", false);
                    //Logger.LoggerInstance.ReportMessage("PRPR_NPI", strPrprNPI);
                }


                //if (_claim.UniqueNotNullProcedureCodes.Count > 0) // Commenting for ALM Defect 5316
                if (_claim.UniqueNotNullProcedureCodes.Count > 0 || _claim.UniqueNotNullDiagCodes.Count > 0) // Adding for ALM 5316. Facility claims may not have any procedure codes.
                {
                    string strAggregatedProcCodes = "'" + string.Join(",", _claim.UniqueNotNullProcedureCodes) + "'"; // CR 86
                    string strAggregatedDiagCodes = "'" + string.Join(",", _claim.UniqueNotNullDiagCodes) + "'"; // CR 86

                    Logger.LoggerInstance.ReportMessage("SetClaimTOSOverride ", " Procedure code array is : " + strAggregatedProcCodes);
                    Logger.LoggerInstance.ReportMessage("SetClaimTOSOverride ", " Diag code array is : " + strAggregatedDiagCodes);
                    dataLayer = new XA9DataLayer();

                    //foreach (string strProcCode in _claim.UniqueNotNullProcedureCodes) cr 86
                    //{ cr 86
                    // Defect 2231 - Begin - For Inpatient claims, use the entire IPCD_ID. For other claims use the first 5 chars while comparing
                    /* Removing for CR 86 - Begin
                    if (_claim.HospClaimData.POS_IND.Equals("I"))
                        strClaimProcCode = strProcCode;
                    else
                        strClaimProcCode = strProcCode.Substring(0, 5);
                      Removing for CR 86 - End */

                    ////Logger.LoggerInstance.ReportMessage("ORIGINAL IPCD ID IS : ", strProcCode); cr 86
                    //Logger.LoggerInstance.ReportMessage("REQUIRED IPCD ID IS : ", strClaimProcCode);
                    //strQuery = dataLayer.GetBDCInfoForClaimLine(strClaimProcCode, strPrprNPI, _claim.CLCL_LOW_SVC_DT, _claim.PDPD_ID,strITSSFMsgs, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQueryResult); // Defect 2231
                    strQuery = dataLayer.GetBDCInfoForClaimLine(strAggregatedProcCodes, strAggregatedDiagCodes, strPrprNPI, _claim.CLCL_LOW_SVC_DT, _claim.PDPD_ID, strITSSFMsgs, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQueryResult); // Defect 2231
                    Logger.LoggerInstance.ReportMessage("GetBDCInfoForClaimLine query for proc and diag is : ", strQuery);
                    Logger.LoggerInstance.ReportMessage("Data for Proc and Diag is : ", strQueryResult);

                    //Logger.LoggerInstance.ReportMessage("GetBDCInfoForClaimLine query for proc and diag is : ", strQuery);
                    recordsReturned = FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult);
                    if (recordsReturned == false)
                    {
                        strQuery = dataLayer.GetBDCInfoForClaimLine(strAggregatedProcCodes, "''", strPrprNPI, _claim.CLCL_LOW_SVC_DT, _claim.PDPD_ID, strITSSFMsgs, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQueryResult); // Defect 2231
                        Logger.LoggerInstance.ReportMessage("GetBDCInfoForClaimLine query for proc only is : ", strQuery);
                        Logger.LoggerInstance.ReportMessage("Data for Proc only is : ", strQueryResult);
                        recordsReturned = FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult);
                    }
                    if (recordsReturned == false)
                    {
                        strQuery = dataLayer.GetBDCInfoForClaimLine("''", strAggregatedDiagCodes, strPrprNPI, _claim.CLCL_LOW_SVC_DT, _claim.PDPD_ID, strITSSFMsgs, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQueryResult); // Defect 2231
                        Logger.LoggerInstance.ReportMessage("GetBDCInfoForClaimLine query for diag only : ", strQuery);
                        Logger.LoggerInstance.ReportMessage("Data for Diag only is : ", strQueryResult);
                        recordsReturned = FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult);
                    }
                    /* Agnes Code 1 */
                    // Check for this
                    /*
                     * 1)	SF Msg matching Prgm CD and Sub Prgm CD but not matching Proc and/or diag
                            Eg: SF = 1020
                            No match on Proc / Diag
                            What to do: Get BDC PGM CD and Sub PGM CD from Prgm Table; Give Priority to BDC +; Sort by PGM CD DESC / SUB PGM CD DESC; Select top 1, Current SP won’t work. You have to code.

                    */
                    if (recordsReturned == false)
                    {
                        /* Agnes Code 2 */


                    }

                    if (recordsReturned == true)
                    {

                        ////Logger.LoggerInstance.ReportMessage("GetBDCInfoForClaimLine", line.CDML_SEQ_NO.ToString() + " is " + strQueryResult);
                        xdocBDCResult = XDocument.Parse(strQueryResult);
                        strProcBdcPmdId = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "PROC_BDC_PGM_ID", false);
                        strProcBdcSubPgmId = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "PROC_BDC_SUB_PGM_ID", false);
                        strBdcProgName = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "BDC_CMBND_PGM_NM", false);
                        strProvBdcPmdId = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "PRPR_BDC_PGM_ID", false);
                        strProvBdcSubPgmId = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "PRPR_BDC_SUB_PGM_ID", false);
                        strBdcSfMsgCode = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "INFO_SUBM_MSG_CD", false);
                        strProductSfMsgCode = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "ATND_TEXT", false);

                        // Defect 2231 - Begin (Trim Program Name when looking for BDC / BDC+ status indicator)
                        if (!string.IsNullOrEmpty(strBdcProgName))
                        {
                            strBdcProgName = strBdcProgName.Trim();
                            if (strBdcProgName.EndsWith(XA9Constants.BDCPLUS))
                            {
                                isBDCPlus = true;
                            }

                        }
                        // Defect 2231 - End

                        //if (string.IsNullOrEmpty(strProcBdcPmdId) || string.IsNullOrEmpty(strProcBdcSubPgmId))
                        //break;

                        if (isITSClaim == true)
                        {
                            Logger.LoggerInstance.ReportMessage("ITS ", isITSClaim.ToString());
                            Clim ITSMatch = _claim.claimITS.FirstOrDefault(e => e.CLIM_ITS_MSG_CD.Equals(strBdcSfMsgCode));
                            if (ITSMatch == null)
                            {
                                //Logger.LoggerInstance.ReportMessage("ITSMatch ", "ITSMatch is null");
                                strBdcFlag = XA9Constants.NON_BDC_FLAG; // ("NN")
                                strBenefitLevelForTOS = XA9Constants.LOW_BENEFIT; // "L"
                            }
                            //else if (strBdcProgName.EndsWith(XA9Constants.BDCPLUS)) // if BDC plus
                            //if(isBDCPlus) -- Commenting for DEFECT 5119
                            else if (isBDCPlus) // Adding for DEFECT 5119
                            {
                                Logger.LoggerInstance.ReportMessage("SetClaimTOSOverride ", "Is BDC Plus : " + isBDCPlus.ToString());
                                //Logger.LoggerInstance.ReportMessage("ITSMatch ", "ITSMatched YP");
                                strBdcFlag = XA9Constants.BDC_PLUS_FLAG;    // "YP"
                                strBenefitLevelForTOS = XA9Constants.HIGH_BENEFIT; // "H"
                            }
                            else
                            {
                                //Logger.LoggerInstance.ReportMessage("ITSMatch ", "ITSMatched YY");
                                strBdcFlag = XA9Constants.BDC_FLAG; // "YY"
                                strBenefitLevelForTOS = XA9Constants.HIGH_BENEFIT; //"H"
                            }

                        }
                        else
                        {
                            //Logger.LoggerInstance.ReportMessage("ITS ", isITSClaim.ToString());
                            if (strProcBdcPmdId.Equals(strProvBdcPmdId) && strProcBdcSubPgmId.Equals(strProvBdcSubPgmId))
                            {
                                //if (strBdcProgName.EndsWith(XA9Constants.BDCPLUS))
                                if (isBDCPlus)
                                {
                                    //Logger.LoggerInstance.ReportMessage("SetClaimTOSOverride ", "Is BDC Plus : " + isBDCPlus.ToString());
                                    strBdcFlag = XA9Constants.BDC_PLUS_FLAG;
                                    strBenefitLevelForTOS = XA9Constants.HIGH_BENEFIT;
                                }
                                else
                                {
                                    strBdcFlag = XA9Constants.BDC_FLAG;
                                    strBenefitLevelForTOS = XA9Constants.HIGH_BENEFIT;
                                }
                            }
                            else if (strProcBdcPmdId.Equals(strProvBdcPmdId) && !strProcBdcSubPgmId.Equals(strProvBdcSubPgmId))
                            {
                                strBdcFlag = XA9Constants.NON_BDC_PROV_FLAG;
                                strBenefitLevelForTOS = XA9Constants.LOW_BENEFIT;
                            }
                            else
                            {
                                strBdcFlag = XA9Constants.NON_BDC_FLAG;
                                strBenefitLevelForTOS = XA9Constants.LOW_BENEFIT;
                            }
                        }

                        // If batch process or if claim id is not empty, save it. If claim id is empty then its a new or adjusted online claim and CLCL_ID 
                        //is not yet created. In that case save BDC data at POST_SAVE
                        //if (AppConfig.FacetsBatchApps.Contains(ContextData.ContextInstance.ApplicationId) || !string.IsNullOrEmpty(_claim.CLCL_ID))
                        if (AppConfig.FacetsBatchApps.Contains(ContextData.ContextInstance.ApplicationId)) // If BDC is executed via batch
                        {
                            blnBdcSaved = dataLayer.SetBdcCodeForClaim(_claim.CLCL_ID, strProcBdcPmdId, strProcBdcSubPgmId, strBdcProgName, strBdcFlag, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQuery);
                        }
                        /*if(ContextData.ContextInstance.ExitTiming.Equals("POSTSAVE"))
                        {
                            blnBdcSaved = dataLayer.SetBdcCodeForClaim(_claim.CLCL_ID, strProcBdcPmdId, strProcBdcSubPgmId, strBdcProgName, strBdcFlag, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQuery);
                        }*/
                        else
                        {

                            AddUpdateBDCValuesIntoCustom(strProcBdcPmdId, strProcBdcSubPgmId, strBdcProgName.Trim(), strBdcFlag);
                            saveBDCPostSave = true;
                            ////Logger.LoggerInstance.ReportMessage("Custom Data Element built", FacetsData.FacetsInstance.FacetsXml.ToString());
                            /*saveBDCPostSave = true;
                            if (string.IsNullOrEmpty(FacetsData.FacetsInstance.GetSingleDataItem("CUSTOM", "POSTSAVEBDC", false)))
                            {

                                FacetsData.FacetsInstance.AddSingleSubCollection("CUSTOM", "<Column name=\"POSTSAVEBDC\">Y</Column>");
                                FacetsData.FacetsInstance.AddSingleSubCollection("CUSTOM", "<Column name=\"BDCPGMID\">" + strProcBdcPmdId + "</Column>");
                                FacetsData.FacetsInstance.AddSingleSubCollection("CUSTOM", "<Column name=\"BDCSUBPGMID\">" + strProcBdcSubPgmId + "</Column>");
                                FacetsData.FacetsInstance.AddSingleSubCollection("CUSTOM", "<Column name=\"BDCPGMNM\">" + strBdcProgName.Trim() + "</Column>");
                                FacetsData.FacetsInstance.AddSingleSubCollection("CUSTOM", "<Column name=\"BDCFLAG\">" + strBdcFlag + "</Column>");

                            }
                            else
                            {
                                FacetsData.FacetsInstance.SetSingleDataItem("CUSTOM", "POSTSAVEBDC", "Y");
                                FacetsData.FacetsInstance.SetSingleDataItem("CUSTOM", "BDCPGMID", strProcBdcPmdId);
                                FacetsData.FacetsInstance.SetSingleDataItem("CUSTOM", "BDCSUBPGMID", strProcBdcSubPgmId);
                                FacetsData.FacetsInstance.SetSingleDataItem("CUSTOM", "BDCPGMNM", strBdcProgName.Trim());
                                FacetsData.FacetsInstance.SetSingleDataItem("CUSTOM", "BDCFLAG", strBdcFlag);
                            }*/

                        }
                        ////Logger.LoggerInstance.ReportMessage("BDC Code Save Query", strQuery);
                        ////Logger.LoggerInstance.ReportMessage("Did BDC Code Save? ", blnBdcSaved.ToString());

                        /* CR 86 - Read Product SF Message and the first two chars of SESE_ID from Product Attachment - Begin */
                        strPdpdAttch = strProductSfMsgCode.Split(','); // Product Attachment contains SF messages followed by a comma and then the first two chars of SESE_ID
                        if (strPdpdAttch.Length > 1)
                        {
                            strProductSfMsgCode = strPdpdAttch[0].Trim(); // Defect 2606 - Added Trim() at the end
                            //Logger.LoggerInstance.ReportMessage("strProductSfMsgCode", " After Trim is " + strProductSfMsgCode);
                            strProductTOSValue = strPdpdAttch[1].Trim(); // Defect 2606 - Added Trim() at the end

                            _claim.claimLines.ForEach
                                (cdml => cdml.BDCTOSVal = strProductTOSValue);

                            //Logger.LoggerInstance.ReportMessage("strProductTOSValue", " After Trim is " + strProductTOSValue);
                        }

                        else
                        {
                            strProductSfMsgCode = string.Empty;
                            strProductTOSValue = string.Empty;
                        }

                        /* CR 86 - Read Product SF Message and the first two chars of SESE_ID from Product Attachment - End */

                        if (strProductSfMsgCode.Equals(strBdcSfMsgCode))
                        {
                            //strDerivedTOS = strProductTOSValue + _claim.HospClaimData.POS_IND + strBenefitLevelForTOS; - CR 103
                            setTOSOverride = true;
                        }
                        else
                        {
                            setTOSOverride = false;
                            //Logger.LoggerInstance.ReportMessage("Derived TOS", strDerivedTOS);
                        }
                        if (setTOSOverride == true) // If true then Type Of Service will be overridden
                        {
                            /* Remove all SR overrides - Begin */
                            bool found = false;
                            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " REMOVING TYPE OF SERVICE  OVERRIDES BEFORE APPLYING BDC SR");
                            List<XElement> cdorList = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL")).ToList();
                            foreach (XElement element in cdorList)
                            {
                                found = element.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && e.Value.Equals("AS"));
                                if (found)
                                    element.Remove();
                            }
                            /* Remove all SR overrides - End */

                            List<string> lstProcCodesForCurrentClaim = this._claim.UniqueLineLevelNotNullProcedureCodes; // getting only the line level proc codes
                            if (lstProcCodesForCurrentClaim.Count > 0)
                            {

                                strAggregatedProcCodes = "'" + string.Join(",", lstProcCodesForCurrentClaim) + "'";
                                //Logger.LoggerInstance.ReportMessage("strAggregatedProcCodes ", " is " + strAggregatedProcCodes);
                                //strQuery = dataLayer.GetAuthProcCodesForBDC(strProcBdcPmdId, strProcBdcSubPgmId, strAggregatedProcCodes, _claim.CLCL_LOW_SVC_DT, _claim.HospClaimData.POS_IND, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQueryResult);
                                //strQuery = dataLayer.GetAuthProcCodesForBDC(strAggregatedProcCodes, _claim.CLCL_LOW_SVC_DT, _claim.HospClaimData.POS_IND, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQueryResult);
                                strQuery = dataLayer.GetAuthProcCodesForBDC(strAggregatedProcCodes, _claim.CLCL_LOW_SVC_DT, isBDCPlus, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQueryResult);
                                Logger.LoggerInstance.ReportMessage("Query for AUth is :  ", strQuery);
                                recordsReturned = FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult);
                                Logger.LoggerInstance.ReportMessage("AUth result is :  ", strQueryResult);
                                if (recordsReturned)
                                {
                                    FacetsData.FacetsInstance.GetMultipleDataElements(strQueryResult, "DATA", string.Empty).ToList()
                                        .ForEach(elm => _claim.claimLines.Where(
                                            cdml => cdml.IPCD_ID.Equals(elm.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("BDC_PROC_CD")).Value)).ToList()
                                            .ForEach(cdmlToUpdate => cdmlToUpdate.BDCTOSVal = elm.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("TOS")).Value
                                                ));

                                    //_claim.claimLines.ForEach
                                      //  (cdml => Logger.LoggerInstance.ReportMessage("TOS Value for line : ", cdml.CDML_SEQ_NO.ToString() + "  is  " + cdml.BDCTOSVal));
                                }
                            }
                            // Override all lines with the derived CDOR value.
                            _claim.claimLines.
                                ForEach
                                (
                                //l => IncdmlsertCdorRecord(l.CLCL_ID, l.CDML_SEQ_NO, XA9Constants.TOS_OVERRIDE, l.MEME_CK, 0, strDerivedTOS, AppConfig.BDCOverrideExcdID)
                                    cdml => InsertCdorRecord(cdml.CLCL_ID, cdml.CDML_SEQ_NO, XA9Constants.TOS_OVERRIDE, cdml.MEME_CK, 0, cdml.BDCTOSVal + _claim.HospClaimData.POS_IND + strBenefitLevelForTOS, AppConfig.BDCOverrideExcdID)
                                );
                            return setTOSOverride; // Once a claim line is selected for BDC, override all claim lines and exit
                        } //if (setTOSOverride == true)

                    } // if (recordsReturned == true)
                    else
                    {

                        if (AppConfig.FacetsOnlineApps.Contains(ContextData.ContextInstance.ApplicationId)) // If BDC is executed via online
                        {
                            saveBDCPostSave = true;
                            AddUpdateBDCValuesIntoCustom(strProcBdcPmdId, strProcBdcSubPgmId, strBdcProgName.Trim(), strBdcFlag);
                        }
                    }

                } // if (_claim.UniqueNotNullProcedureCodes.Count > 0)
                else
                {
                    if (AppConfig.FacetsOnlineApps.Contains(ContextData.ContextInstance.ApplicationId)) // If BDC is executed via online
                    {
                        saveBDCPostSave = true;
                        AddUpdateBDCValuesIntoCustom(strProcBdcPmdId, strProcBdcSubPgmId, strBdcProgName.Trim(), strBdcFlag);
                    }
                }

            } // try
            catch (Exception ex)
            {
                //Logger.LoggerInstance.ReportMessage("Exception", ex.Message);
            }
            finally
            {

            }
            return saveBDCPostSave;
        }

        /// <summary>
        /// Update CUSTOM Data Element Collection with new values
        /// </summary>
        /// <param name="strProcBdcPmdId">BDC Program ID</param>
        /// <param name="strProcBdcSubPgmId">BDC Sub Program ID</param>
        /// <param name="strBdcProgName">BDC Program Name</param>
        /// <param name="strBdcFlag">BDC Flag</param>
        public void AddUpdateBDCValuesIntoCustom(string strProcBdcPmdId, string strProcBdcSubPgmId, string strBdcProgName, string strBdcFlag)
        {
            if (string.IsNullOrEmpty(FacetsData.FacetsInstance.GetSingleDataItem("CUSTOM", "POSTSAVEBDC", false)))
            {

                FacetsData.FacetsInstance.AddSingleSubCollection("CUSTOM", "<Column name=\"POSTSAVEBDC\">Y</Column>");
                FacetsData.FacetsInstance.AddSingleSubCollection("CUSTOM", "<Column name=\"BDCPGMID\">" + strProcBdcPmdId + "</Column>");
                FacetsData.FacetsInstance.AddSingleSubCollection("CUSTOM", "<Column name=\"BDCSUBPGMID\">" + strProcBdcSubPgmId + "</Column>");
                FacetsData.FacetsInstance.AddSingleSubCollection("CUSTOM", "<Column name=\"BDCPGMNM\">" + strBdcProgName.Trim() + "</Column>");
                FacetsData.FacetsInstance.AddSingleSubCollection("CUSTOM", "<Column name=\"BDCFLAG\">" + strBdcFlag + "</Column>");

            }
            else
            {
                FacetsData.FacetsInstance.SetSingleDataItem("CUSTOM", "POSTSAVEBDC", "Y");
                FacetsData.FacetsInstance.SetSingleDataItem("CUSTOM", "BDCPGMID", strProcBdcPmdId);
                FacetsData.FacetsInstance.SetSingleDataItem("CUSTOM", "BDCSUBPGMID", strProcBdcSubPgmId);
                FacetsData.FacetsInstance.SetSingleDataItem("CUSTOM", "BDCPGMNM", strBdcProgName.Trim());
                FacetsData.FacetsInstance.SetSingleDataItem("CUSTOM", "BDCFLAG", strBdcFlag);
            }


        }

        /// <summary>
        /// This function populates an object of type SERL for each claim line based on SESE_ID and SESE_RULE
        /// </summary>
        /// <returns></returns>
        public bool GetRelatedServiceIDData()
        {
            string strQueryResult = string.Empty;
            bool recordsReturned = false;
            string strQuery = string.Empty;
            string strSeseIDs = string.Empty;
            string strSeseRule = string.Empty;
            XA9DataLayer dataLayer = null;
            List<XElement> lstSerlElements = null;
            List<SERL> lstSerl = new List<SERL>();
            try
            {
                GetClaimLineData();
                var uniqueServices = (from cl in _claim.claimLines
                                      group cl by new { cl.SESE_ID, cl.SESE_RULE } into groupedServices
                                      select new { groupedServices.Key.SESE_ID, groupedServices.Key.SESE_RULE }
                                    ).ToList();

                strSeseIDs = "'" + string.Join(",", uniqueServices.Select(sese => sese.SESE_ID)) + "'";
                strSeseRule = "'" + string.Join(",", uniqueServices.Select(sese => sese.SESE_RULE)) + "'";
                strQuery = dataLayer.GetRelatedServicesDataForServiceIDAndRule(strSeseIDs, strSeseRule, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQueryResult);
                if (recordsReturned)
                {
                    lstSerlElements = FacetsData.FacetsInstance.GetMultipleDataElements(strQueryResult, "DATA", string.Empty);

                    lstSerlElements.ForEach(Serl => 
                        lstSerl.Add(new SERL(){
                            SESE_ID = Serl.Elements().FirstOrDefault(elm => elm.Attribute("name").Value.Equals("SESE_ID")).Value,
                            SESE_RULE = Serl.Elements().FirstOrDefault(elm => elm.Attribute("name").Value.Equals("SESE_RULE")).Value,
                            SERL_PER = int.Parse(Serl.Elements().FirstOrDefault(elm => elm.Attribute("name").Value.Equals("SERL_PER")).Value),
                            SERL_REL_ID = Serl.Elements().FirstOrDefault(elm => elm.Attribute("name").Value.Equals("SERL_REL_ID")).Value,
                            SERL_REL_PER_IND = Serl.Elements().FirstOrDefault(elm => elm.Attribute("name").Value.Equals("SERL_REL_PER_IND")).Value,
                            SERL_REL_TYPE = Serl.Elements().FirstOrDefault(elm => elm.Attribute("name").Value.Equals("SERL_REL_TYPE")).Value,
                            SETR_ALLOW_AMT = double.Parse(Serl.Elements().FirstOrDefault(elm => elm.Attribute("name").Value.Equals("SETR_ALLOW_AMT")).Value),
                            SETR_ALLOW_CTR = int.Parse(Serl.Elements().FirstOrDefault(elm => elm.Attribute("name").Value.Equals("SETR_ALLOW_CTR")).Value)
                        }));

                    //lstSerl.ForEach(serl => _claim.claimLines.Where(                                                                
                }
                /*
                 strWmkMemberIDs = "'" + string.Join(",", _claim.WellmarkMemberContrievedKeys.Distinct()) + "'"; // 10222016
                strUniqueFamily = "'" + string.Join(",", _claim.claimLines.Where(line => !string.IsNullOrEmpty(line.OnceInALFGroup)).Select(line => line.OnceInALFGroup).Distinct()) + "'";
                strQuery = dataLayer.GetHistoryClaimsForWMKMembers(strWmkMemberIDs,strUniqueFamily, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strDataOutput);
                //Logger.LoggerInstance.ReportMessage("ProcessOnceInALfTmClaim", " strQuery is " + strQuery);
                //Logger.LoggerInstance.ReportMessage("ProcessOnceInALfTmClaim", " strDataOutput is " + strDataOutput);
                recordsReturned = FacetsData.FacetsInstance.IsDbDataAvailable(strDataOutput);
                if (recordsReturned)
                {
                */
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return recordsReturned;
        }

        /// <summary>
        /// Get SERL_REL_ID for each claim line
        /// </summary>
        /// <param name="strQueryResult"></param>
        /// <returns></returns>
        public bool GetRelatedSvcID()
        {
            string strQueryResult = string.Empty;
            bool recordsReturned = true;
            string strQuery = string.Empty;
            //XDocument xdocSerlResultSet = null;// XDocument.Parse(strQueryResult);
            string strSerlRelID = string.Empty;
            double dblSetrCopay;
            XA9DataLayer dataLayer = null;
            try
            {
                dataLayer = new XA9DataLayer();

                // Call stored procedure wmkp_get_serl_id ONLY for Unique SESE_ID AND SESE_RULE. One claim can have multiple lines with the same SESE_ID and SESE_RULE.
                var uniqueServices = (from cl in _claim.claimLines
                                      group cl by new { cl.SESE_ID, cl.SESE_RULE } into groupedServices
                                      select new { groupedServices.Key.SESE_ID, groupedServices.Key.SESE_RULE }
                                     ).ToList();

                ////Logger.LoggerInstance.ReportMessage("uniqueServices count is : ", uniqueServices.Count.ToString());
                foreach(var item in uniqueServices)
                {
                    strQuery = dataLayer.GetSerlIdForService(item.SESE_ID,item.SESE_RULE,ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQueryResult);
                    ////Logger.LoggerInstance.ReportMessage("strQuery for SESE_ID = " + item.SESE_ID + " and SESE_RULE = " + item.SESE_RULE + " is : ", strQuery);
                    ////Logger.LoggerInstance.ReportMessage("strQueryResult for SESE_ID = " + item.SESE_ID + " and SESE_RULE = " + item.SESE_RULE + " is : ", strQueryResult);
                    recordsReturned = FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult);
                    ////Logger.LoggerInstance.ReportMessage("recordsReturned for SESE_ID = " + item.SESE_ID + " and SESE_RULE = " + item.SESE_RULE + " is : ", recordsReturned.ToString());
                    if (recordsReturned == true)
                    {
                        strSerlRelID = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "SERL_REL_ID", false);
                        dblSetrCopay = double.Parse(FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "SETR_COPAY_AMT", false));
                        ////Logger.LoggerInstance.ReportMessage("strSerlRelID for SESE_ID = " + item.SESE_ID + " and SESE_RULE = " + item.SESE_RULE + " is : ", strSerlRelID);
                        _claim.claimLines.Where(claimLine => claimLine.SESE_ID.Equals(item.SESE_ID) && claimLine.SESE_RULE.Equals(item.SESE_RULE)).ToList()
                            .ForEach(line =>
                                {
                                    line.SERL_REL_ID = strSerlRelID;
                                    line.SETR_COPAY = dblSetrCopay;
                                });
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return recordsReturned;
        }

        /// <summary>
        /// Set Copay Override
        /// </summary>
        /// <returns></returns>
        public bool SetClaimCopayOverride()
        {
            string strQuery = string.Empty;

            bool setCopayOverride = false;
            bool recordsReturned = false;
            //List<ClaimLine> linesForOverride = null;
            string strQueryResult = string.Empty;
            
            XA9DataLayer dataLayer = null;
            double dblSerlCopay;
            double dblTotalCopay;
            double dblRemainingCopay ;
            double dblCopayAmtToOverride ;
            double dblCopayAmtToOverridePerUnit;
            double dblSumOfCopayOfAllLines ;
            string strSerlQualifier = "";

            DateTime cdmlFromDT;
            
            try
            {
                strSerlQualifier = AppConfig.SerlQualifier;
                //Logger.LoggerInstance.ReportMessage(XA9Constants.PRC_PROC_XCOM_OCPD, "Serl Qualifier is   " + strSerlQualifier);
                dataLayer = new XA9DataLayer();
                GetClaimData();

                GetClaimLineData();
                //Logger.LoggerInstance.ReportMessage(XA9Constants.PRC_PROC_XCOM_OCPD, XA9Constants.MSG_CDML_OBTAINED);
                GetClaimLineOverrideData();
                //Logger.LoggerInstance.ReportMessage(XA9Constants.PRC_PROC_XCOM_OCPD, XA9Constants.MSG_CDOR_OBTAINED);

                // Populate SERL_REL_ID for each claim line based on SESE_ID and SESE_RULE
                GetRelatedSvcID();
                //Logger.LoggerInstance.ReportMessage(XA9Constants.PRC_PROC_XCOM_OCPD, XA9Constants.MSG_SERL_OBTAINED);
                // Get all the claim lines where Related Service ID starts with "P"

                // Group Claim Lines based on CDML_FROM_DT. claimLinesGroupedByCdmlFromDT will have the key as CDML_FROM_DT and values as Claim Lines matching the CDML_FROM_DT
                var claimLinesGroupedByCdmlFromDT = _claim.claimLines
                                        .GroupBy(line => line.CDML_FROM_DT)
                                        .Select(group => 
                                                    new { 
                                                            LineFromDate = group.Key, 
                                                            claimLinesForFromDT = group.ToList() 
                                                        }
                                                 );


                //Logger.LoggerInstance.ReportMessage(XA9Constants.PRC_PROC_XCOM_OCPD, string.Format(XA9Constants.MSG_UNIQUE_CDML_DT, claimLinesGroupedByCdmlFromDT.Count().ToString()));
                foreach (var claimFromDT in claimLinesGroupedByCdmlFromDT)
                {
                    
                    cdmlFromDT = claimFromDT.LineFromDate; // Get the value of CDML_FROM_DT 
                    //Logger.LoggerInstance.ReportMessage(XA9Constants.PRC_PROC_XCOM_OCPD, string.Format(XA9Constants.MSG_PROCESS_CDML_DT, cdmlFromDT.ToString()));

                    // Get unique SERL_REL_ID for claims for cdml from date of cdmlFromDT
                    var uniqueSerlRelIDs = claimFromDT.claimLinesForFromDT
                                                .Where(line => line.SERL_REL_ID.StartsWith(strSerlQualifier))//XA9Constants.OCPDSERLPREFIX))
                                                .Select(line => 
                                                            new {
                                                                    SERL_REL_ID = line.SERL_REL_ID
                                                                }
                                                       )
                                                 .Distinct();
                    foreach (var uniqueSerlID in uniqueSerlRelIDs)
                    {
                        dblRemainingCopay = 0;   // reset remaining copay
                        dblTotalCopay = 0;       // reset total copay of the member for this SERL_REL_ID

                        // Get total copay of the member for this CDML_FROM_DT and SERL_REL_ID.
                        strQuery = dataLayer.GetTotalOfMemberCopay(_claim.MEME_CK, cdmlFromDT.ToString(), uniqueSerlID.SERL_REL_ID, _claim.CLCL_ID, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQueryResult);
                        //Logger.LoggerInstance.ReportMessage(XA9Constants.PRC_PROC_XCOM_OCPD, string.Format(XA9Constants.MSG_QUERY_CDML_DT,cdmlFromDT.ToString(), uniqueSerlID.SERL_REL_ID , strQuery));

                        
                        recordsReturned = FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult);

                        // dblTotalCopay = Total Copay for this member for the CDML_FROM_DT and SERL_REL_ID
                        dblTotalCopay = double.Parse(FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "COPAY_AMT", false));

                        //Logger.LoggerInstance.ReportMessage(XA9Constants.PRC_PROC_XCOM_OCPD, string.Format(XA9Constants.MSG_TOT_CPY_FOR_CDML_DT, cdmlFromDT.ToString() , uniqueSerlID.SERL_REL_ID , dblTotalCopay.ToString()));

                       // For this SERL_REL_ID, get the unique list of SESE_ID and SESE_RULE
                        var uniqueSeseIdAndSeseRule = claimFromDT.claimLinesForFromDT
                                                        .Where(line => line.SERL_REL_ID.Equals(uniqueSerlID.SERL_REL_ID))
                                                        .Select(line => 
                                                                        new { 
                                                                                SESE_ID = line.SESE_ID, 
                                                                                SESE_RULE = line.SESE_RULE 
                                                                            }
                                                                )
                                                        .Distinct();


                        //Logger.LoggerInstance.ReportMessage(XA9Constants.PRC_PROC_XCOM_OCPD, string.Format(XA9Constants.MSG_UNIQUE_SESE_CNT, cdmlFromDT.ToString() , uniqueSerlID.SERL_REL_ID , uniqueSeseIdAndSeseRule.Count().ToString()));

                        // For Each SESE_ID and SESE_RULE 
                        foreach (var uniqueService in uniqueSeseIdAndSeseRule)
                        {
                            var claimLinesToOverride = claimFromDT.claimLinesForFromDT.Where(line => line.SESE_ID.Equals(uniqueService.SESE_ID) && line.SESE_RULE.Equals(uniqueService.SESE_RULE)).ToList();
                            dblSumOfCopayOfAllLines = claimFromDT.claimLinesForFromDT.Sum(line => line.CDML_COPAY_AMT);
                            if (dblSumOfCopayOfAllLines > 0 && dblTotalCopay > 0) // Only when the COPAY amounts on the lines for one SESE_ID and SESE_RULE is greater than zero
                            {
                                foreach (ClaimLine claimLine in claimLinesToOverride)
                                {
                                    dblCopayAmtToOverride = 0;

                                    dblSerlCopay = claimLine.SETR_COPAY;
                                    dblRemainingCopay = Math.Round(dblRemainingCopay, 2, MidpointRounding.ToEven) + (dblSerlCopay - dblTotalCopay);

                                    dblRemainingCopay = dblRemainingCopay + Math.Round((dblSerlCopay - dblTotalCopay), 2, MidpointRounding.ToEven);

                                    
                                    if (dblRemainingCopay < 0)
                                    {
                                        //dblRemainingCopay = dblRemainingCopay + (dblSerlCopay - dblTotalCopay);
                                        dblRemainingCopay = dblRemainingCopay + Math.Round((dblSerlCopay - dblTotalCopay), 2, MidpointRounding.ToEven);
                                    }
                                    else
                                    {
                                        //dblRemainingCopay = dblSerlCopay - dblTotalCopay;
                                        dblRemainingCopay = Math.Round((dblSerlCopay - dblTotalCopay), 2, MidpointRounding.ToEven);
                                    }

                                    //Logger.LoggerInstance.ReportMessage(XA9Constants.PRC_PROC_XCOM_OCPD, string.Format(XA9Constants.MSG_SETR_CPY_FOR_LINE, claimLine.CDML_SEQ_NO.ToString(), dblSerlCopay.ToString()));
                                    //Logger.LoggerInstance.ReportMessage(XA9Constants.PRC_PROC_XCOM_OCPD, string.Format(XA9Constants.MSG_REM_CPY_FOR_LINE,claimLine.CDML_SEQ_NO.ToString(),dblRemainingCopay.ToString()));
                                    //Logger.LoggerInstance.ReportMessage(XA9Constants.PRC_PROC_XCOM_OCPD, string.Format(XA9Constants.MSG_CDML_CPY_FOR_LINE, claimLine.CDML_SEQ_NO.ToString(), claimLine.CDML_COPAY_AMT.ToString()));
                                    

                                    if (dblRemainingCopay < claimLine.CDML_COPAY_AMT)
                                    {
                                        if (dblRemainingCopay < 0)
                                        {
                                            dblCopayAmtToOverride = 0;
                                            dblCopayAmtToOverridePerUnit = 0;
                                            setCopayOverride = true;
                                        }
                                        else
                                        {
                                            dblCopayAmtToOverride = dblRemainingCopay;
                                            if (claimLine.CDML_UNITS > 0)
                                            {
                                                dblCopayAmtToOverridePerUnit = Math.Round((dblCopayAmtToOverride / claimLine.CDML_UNITS), 2, MidpointRounding.ToEven);
                                            }
                                            else
                                            {
                                                dblCopayAmtToOverridePerUnit = dblCopayAmtToOverride;
                                            }
                                            setCopayOverride = true;
                                        }
                                        dblTotalCopay = dblTotalCopay + dblCopayAmtToOverride;
                                        InsertCdorRecord(claimLine.CLCL_ID, claimLine.CDML_SEQ_NO, XA9Constants.CDOR_ID_CPY_OVERRIDE, claimLine.MEME_CK, dblCopayAmtToOverridePerUnit, "", AppConfig.CopayOverrideExcdID);
                                    }
                                    else // Do not override if the remaining copay is greater than the copay of the current line
                                    {
                                        dblTotalCopay = dblTotalCopay + claimLine.CDML_COPAY_AMT;
                                    }

                                    dblRemainingCopay = dblRemainingCopay - claimLine.CDML_COPAY_AMT;
                                }
                            }
                        }
                    }
                }
                
                if (setCopayOverride == true)
                {
                    InsertREPROC();
                }
            }
            catch (Exception ex)
            {

            }
            return setCopayOverride;
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

        /// <summary>
        /// Removes NP override 
        /// </summary>
        internal void RemoveNPOverrides()
        {
            bool blnOverridesRemoved = false;
            try
            {
                RemoveClaimLineOverrides(new string[] { "NP", "MP", "01" });
                /*List<XElement> cdorList = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL")).ToList();

                blnOverridesRemoved = false;
                foreach (XElement element in cdorList)
                {

                    10222016 - BEGIN 
                    blnOverridesRemoved = element.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && e.Value.Equals("NP"));
                    if (blnOverridesRemoved)
                        element.Remove();

                    blnOverridesRemoved = element.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && e.Value.Equals("MP"));
                    if (blnOverridesRemoved)
                        element.Remove();

                    blnOverridesRemoved = element.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && e.Value.Equals("01"));
                    if (blnOverridesRemoved)
                        element.Remove();
                     10222016 - END 
                }*/


                /* 10222016 - BEGIN */
                List<XElement> clorList = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CLOR", "CLOR")).ToList();
                foreach (XElement clorElement in clorList)
                {
                    blnOverridesRemoved = clorElement.Elements().Any(e => e.Attribute("name").Value.Equals("CLOR_OR_ID") && e.Value.Equals("NP"));
                    if (blnOverridesRemoved)
                        clorElement.Remove();
                }
                /* 10222016 - END */

            }
            catch (Exception ex)
            {

            }
        }

         /// <summary>
        /// Remove all Additional NetworX Data Overrides from ITS claim
        /// </summary>
        internal void RemoveExtnNWOverrides()
        {
            bool blnOverridesRemoved = false;
            try
            {
                List<XElement> cdorList = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL")).ToList();

                blnOverridesRemoved = false;
                foreach (XElement element in cdorList)
                {
                    blnOverridesRemoved = element.Elements().Any(CdorId => CdorId.Attribute("name").Value.Equals("CDOR_OR_ID") && CdorId.Value.Equals("NW")) &&
                                           element.Elements().Any(CdorVal => CdorVal.Attribute("name").Value.Equals("CDOR_OR_VALUE") && CdorVal.Value.Equals("EXTN"));

                    if (blnOverridesRemoved)
                        element.Remove();
                }
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// Remove all Allowable Amount Overrides from ITS claim
        /// </summary>
        internal void RemoveExtnAAOverrides()
        {
            bool blnOverridesRemoved = false;
            try
            {
                List<XElement> cdorList = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL")).ToList();
                blnOverridesRemoved = false;
                foreach (XElement element in cdorList)
                {
                    blnOverridesRemoved = element.Elements().Any(CdorId => CdorId.Attribute("name").Value.Equals("CDOR_OR_ID") && CdorId.Value.Equals("AA")) &&
                                            element.Elements().Any(CdorVal => CdorVal.Attribute("name").Value.Equals("CDOR_OR_VALUE") && CdorVal.Value.Equals("EXTN"));
                    if (blnOverridesRemoved)
                        element.Remove();
                 
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Removes External Price from all claim lines
        /// </summary>
        internal void RemoveExternalPrice()
        {
            bool found = false;
            List<XElement> cdorList = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL")).ToList();
            foreach (XElement element in cdorList)
            {
                found = element.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && e.Value.Equals("EP"));
                if (found)
                {
                    //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " Removing existing External Price for Line # " + element.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value);
                    element.Remove();
                }
            }
        }

        /// <summary>
        /// Remove all Manual Pricing EP Amount Overrides from ITS claim
        /// </summary>
        internal void RemoveExtnManualPriceEPOverrides()
        {
            bool blnOverridesRemoved = false;
            try
            {
                List<XElement> cdorList = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL")).ToList();
                blnOverridesRemoved = false;
                foreach (XElement element in cdorList)
                {
                    blnOverridesRemoved = element.Elements().Any(CdorId => CdorId.Attribute("name").Value.Equals("CDOR_OR_ID") && CdorId.Value.Equals("EP")) &&
                                            element.Elements().Any(CdorVal => CdorVal.Attribute("name").Value.Equals("EXCD_ID") && CdorVal.Value.Equals("BWP"));
                    if (blnOverridesRemoved)
                        element.Remove();

                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Removes CDOR elements matching the array of input CDOR_OR_ID 
        /// </summary>
        /// <param name="arrCdorID">Array of CDOR_OR_ID that need to be removed</param>
        internal void RemoveClaimLineOverrides(string[] arrCdorID)
        {
            try
            {
                FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL"))
                    .Where(cdor => cdor.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && arrCdorID.Contains(e.Value))).ToList().ForEach
                    (cdor => cdor.Remove());
                
            }
            catch (Exception ex)
            {

            }

        }
        /// <summary>
        /// Removes CDOR elements matching the array of input CDOR_OR_ID and a single valued CDOR_OR_VALUE
        /// </summary>
        /// <param name="arrCdorID">Array of CDOR_OR_ID that need to be removed</param>
        /// <param name="strCdorVal">CDOR_OR_VALUE that need to be matched</param>
        internal void RemoveClaimLineOverrides(string[] arrCdorID, string strCdorVal)
        {
            try
            {
                FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL"))
                    .Where(cdor => cdor.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && arrCdorID.Contains(e.Value)) &&
                                   cdor.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_VALUE") && e.Value.Equals(strCdorVal))).ToList().ForEach
                    (cdor => cdor.Remove());

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Remove all Pricing overrides from ITS claim
        /// </summary>
        internal void RemoveITSOverrides()
        {
            //bool blnOverridesRemoved = false;
            try
            {

                //RemoveClaimLineOverrides(new string[] { "AA", "AX", "AD" }); ORIGINAL CODE

                // If claim is executing through batch, then remove all these overrides
                if (AppConfig.FacetsBatchApps.Contains(ContextData.ContextInstance.ApplicationId)) // 03/05/2017 - Defect 5091 - Allow for manual disallow when done through online
                {
                    Logger.LoggerInstance.ReportMessage("**** INSIDE METHOD RemoveITSOverrides", "*****REMOVING ALL OVERRIDES AS THIS IS DONE THROUGH BATCH");
                    //RemoveClaimLineOverrides(new string[] { "AA", "AX", "AD" });
                    RemoveClaimLineOverrides(new string[] { "AA", "AX", "AD", "AN" }); // Added AN on 03/09/2017
                }
                else // else do not remove the manually out disallow amount override. 03/05/2017 - Defect 5091 - Allow for manual disallow when done through online
                {
                    Logger.LoggerInstance.ReportMessage("**** INSIDE METHOD RemoveITSOverrides", "****REMOVING ONLY AA AND AD OVERRIDES. AND AX WITH IAX EXCD_ID AS THIS IS DONE THROUGH ONLINE");
                    List<XElement> ITSCdorElmsToRemove = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL"))
                        .Where(cdor => cdor.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && new string[] { "AA", "AX", "AD", "AN" }.Contains(e.Value))).ToList(); // Added AN on 03/09/2017

                    foreach (XElement elmCdor in ITSCdorElmsToRemove)
                    {
                        if (elmCdor.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && new string[] { "AA", "AD" }.Contains(e.Value)))
                            elmCdor.Remove();
                        else if (elmCdor.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && e.Value.Equals("AX")) &&
                                elmCdor.Elements().Any(e => e.Attribute("name").Value.Equals("EXCD_ID") && e.Value.Equals("IAX")))
                            elmCdor.Remove();
                        else if (elmCdor.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && e.Value.Equals("AN")) && // Added removal of IAN
                                elmCdor.Elements().Any(e => e.Attribute("name").Value.Equals("EXCD_ID") && e.Value.Equals("IAN")))
                            elmCdor.Remove();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        
        /// <summary>
        /// Returns the pricing method (WMK (or) ITS) that was applied to the original claim by the extension.
        /// If the original claim was not processed by the extension, pricing method returned will be <blank>
        /// </summary>
        /// <returns></returns>
        public string GetAdjustedClaimPricingMethod()
        {
            string strOrigPriceSource = string.Empty;
            string strQuery = string.Empty;
            string strQueryResult = string.Empty;

            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " &&&&&&&&&&&&&&& INSIDE GetAdjustedClaimPricingMethod");
            strQuery = "SELECT TOP 1 ISNULL(CDOR_OR_VALUE,'') AS CDOR_OR_VALUE  FROM " + ContextData.ContextInstance.DatabaseId + ".dbo.CMC_CDOR_LI_OVR WHERE CLCL_ID = '" + _claim.CLCL_ID_ADJ_FROM + "' AND CDOR_OR_ID = 'EP' AND CDOR_OR_VALUE IN (" + AppConfig.ITSNPEXCDID.Aggregate((start, end) => "'" + start + "'" + ", " + "'" + end + "'") + ");";
            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " &&&&&&&&&&&&&&& INSIDE GetAdjustedClaimPricingMethod. strQuery IS " + strQuery);
            strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " &&&&&&&&&&&&&&& INSIDE GetAdjustedClaimPricingMethod. strQueryResult IS " + strQueryResult);
            //check sql error
            string _sqlErr = FacetsData.FacetsInstance.CheckDbError(strQueryResult);
            if (string.IsNullOrEmpty(_sqlErr))
            {
                if (FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult))
                {
                    strOrigPriceSource = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "CDOR_OR_VALUE", false);
                }
            }
            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " &&&&&&&&&&&&&&& INSIDE GetAdjustedClaimPricingMethod. strOrigPriceSource IS " + strOrigPriceSource);
            //strOrigPriceSource = "ITS";
            return strOrigPriceSource;
        }

        /// <summary>
        /// Populates the Family for once in a LF procedure codes on the claim. If no procedure code for the claim is an once in a LF, then returns false
        /// </summary>
        /// <returns>True of the claim has one or more once in a LF procedure codes. Else returns false</returns>
        public bool PutOnceInALFStatusForEachLine()
        {
            bool recordsReturned = false;
            string strAggregatedProcCodes = string.Empty;
            XA9DataLayer dataLayer = null;
            List<string> lstProcCodesForCurrentClaim = null;
            string strQueryResult = string.Empty;
            string strQuery = string.Empty;
            //List<string> lstHeaderProcCodes = null;
            List<XElement> lstElmLfTmData = null;
            //string strHeaderLvlLtfmProcCode = string.Empty;
            string strHeaderLvlLtfmProcCodeFamily = string.Empty;
            Dictionary<string, string> dicLfTmProcFamilyCollection = null;
            try
            {
                /*if (_claim.CLCL_CL_SUB_TYPE.Equals("H"))
                {
                    lstHeaderProcCodes = this._claim.UniqueHeaderLevelNotNullProcCodes;
                    lstProcCodesForCurrentClaim = this._claim.UniqueNotNullProcedureCodes;
                }
                else
                {
                    lstProcCodesForCurrentClaim = this._claim.UniqueLineLevelNotNullProcedureCodes;
                }*/

                lstProcCodesForCurrentClaim = this._claim.UniqueLineLevelNotNullProcedureCodes;

                if (lstProcCodesForCurrentClaim.Count > 0)
                {

                    strAggregatedProcCodes = "'" + string.Join(",", lstProcCodesForCurrentClaim) + "'";
                    dataLayer = new XA9DataLayer();
                    strQuery = dataLayer.GetOnceInALFProcCodesFromTheList(strAggregatedProcCodes, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQueryResult);
                    //Logger.LoggerInstance.ReportMessage("PutOnceInALFStatusForEachLine", " strQuery is " + strQuery);
                    //Logger.LoggerInstance.ReportMessage("PutOnceInALFStatusForEachLine", " strQueryResult is " + strQueryResult);
                    recordsReturned = FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult);
                    if (recordsReturned == true)
                    {
                        lstElmLfTmData = FacetsData.FacetsInstance.GetMultipleDataElements(strQueryResult, "DATA", string.Empty);

                        // Get Key Value pair of Proc Code and Family
                        dicLfTmProcFamilyCollection = lstElmLfTmData.ToDictionary(key => key.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("PROC_CODE")).Value,
                                                                                  value => value.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("FAMILY")).Value);


                        /*No Need to check for Header Level codes
                        //lstOnceInALftmProcCodes = dicLfTmProcFamilyCollection.Keys.ToList(); --> 
                        //strHeaderLvlLtfmProcCode = _claim.UniqueHeaderLevelNotNullProcCodes.Where(procCode => lstOnceInALftmProcCodes.Contains(procCode)).FirstOrDefault(); // Header level proc code matching the list
                       

                        if (!string.IsNullOrEmpty(strHeaderLvlLtfmProcCode))
                            /*strHeaderLvlLtfmProcCodeFamily = lstElmLfTmData.Descendants().Where(lftm => lftm.Attribute("name").Value.Equals("IPCD_ID") && lftm.Value.Equals(strHeaderLvlLtfmProcCode)).FirstOrDefault()
                                .Parent.Elements().Where(lftm => lftm.Attribute("name").Value.Equals("FAMILY")).Select(val => val.Value).ToString();*/
                            //strHeaderLvlLtfmProcCodeFamily = dicLfTmProcFamilyCollection[strHeaderLvlLtfmProcCode]; */
                         

                        foreach (var v in _claim.claimLines)
                        {
                            /*if (v.IPCD_ID.Equals(string.Empty))
                                v.OnceInALFGroup = strHeaderLvlLtfmProcCodeFamily;
                            else if (dicLfTmProcFamilyCollection.ContainsKey(v.IPCD_ID))
                                v.OnceInALFGroup = dicLfTmProcFamilyCollection[v.IPCD_ID];
                            else
                                v.OnceInALFGroup = string.Empty;*/

                            if (dicLfTmProcFamilyCollection.ContainsKey(v.IPCD_ID))
                                v.OnceInALFGroup = dicLfTmProcFamilyCollection[v.IPCD_ID];
                            else
                                v.OnceInALFGroup = string.Empty;
                        }

                    }
                }

            }
            catch (Exception ex)
            {

            }
            return recordsReturned;
        }

        /// <summary>
        /// Populate Member's all Wellmark Member COntrieved Keys
        /// </summary>
        /// <param name="intMemeCK">Current MEME_CK</param>
        /// <returns>True if there exists one or more other Wellmark Member Contrieved Keys. False otherwise</returns>
        public void PopulateMemberCorrespondingWMKIDs()
        {
            XA9DataLayer dataLayer = null;
            string strDataOutput = string.Empty;
            string strQuery = string.Empty;
            bool recordsReturned = false;
            List<string> lstWmkMemeCK = null;
            int intWmkMemeCK;
            try
            {
                if (_claim.WellmarkMemberContrievedKeys == null)
                    _claim.WellmarkMemberContrievedKeys = new List<int>();

                dataLayer = new XA9DataLayer();
                strQuery = dataLayer.GetMemberCorrespondingWMKIDs(_claim.MEME_CK, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strDataOutput);
                //Logger.LoggerInstance.ReportMessage("PopulateMemberCorrespondingWMKIDs", " strQuery is " + strQuery);
                //Logger.LoggerInstance.ReportMessage("PopulateMemberCorrespondingWMKIDs", " strQueryResult is " + strDataOutput);
                recordsReturned = FacetsData.FacetsInstance.IsDbDataAvailable(strDataOutput);
                if (recordsReturned)
                {
                   lstWmkMemeCK = FacetsData.FacetsInstance.GetDbMultipleDataItem(strDataOutput, "DATA", "WMK_MEME_CK", false);
                   if (lstWmkMemeCK.Count > 0)
                   {
                       foreach (var strMemeck in lstWmkMemeCK)
                       {
                           if (!string.IsNullOrEmpty(strMemeck))
                           {
                               int.TryParse(strMemeck, out intWmkMemeCK);
                               if (intWmkMemeCK > 0)
                                   _claim.WellmarkMemberContrievedKeys.Add(intWmkMemeCK);
                           }
                       }
                   }
                }
                _claim.WellmarkMemberContrievedKeys.Add(_claim.MEME_CK);

            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Deny claims for Patient Responsibility for out of state provider
        /// </summary>
        /// <returns></returns>
        public bool ProcessPRDeny()
        {
            string strQuery  = string.Empty;
            string strQueryResult = string.Empty;
            string strProvState = string.Empty;
            List<XElement> lstPRData = null;
            bool blnPRUpdated = false;
            try
            {

                GetClaimLineOverrideData();

                strQuery = "SELECT ISNULL(CLCB_OC_GROUP_CD,'') AS CLCB_OC_GROUP_CD, CDML_SEQ_NO, ISNULL(CDCB_OC_GROUP_CD,'') AS CDCB_OC_GROUP_CD from " + ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM + ".dbo.wmkt_clcl_edi_rd WHERE CLCL_USER_DATA = '{0}';";
                //Logger.LoggerInstance.ReportMessage("ProcessPRDeny", " PR Check Query is : " + string.Format(strQuery, _claim.CLED_USER_DATA2));
                strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(string.Format(strQuery, _claim.CLED_USER_DATA2));
                //Logger.LoggerInstance.ReportMessage("ProcessPRDeny", " PR Check  Query result is : " + strQueryResult);


                if (FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult))
                {
                    lstPRData = FacetsData.FacetsInstance.GetMultipleDataElements(strQueryResult, "DATA", string.Empty);
                    //Logger.LoggerInstance.ReportMessage("ProcessPRDeny ", " Original Result Count : " + lstPRData.Count.ToString());

                    //lstPRData.ForEach(
                      //      elm => Logger.LoggerInstance.ReportMessage("Claim level PR Content of ", elm.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value + "  is  " +
                                                                                                    //elm.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CLCB_OC_GROUP_CD")).Value));

                    if (!lstPRData.All(PRElmts => PRElmts.Elements().FirstOrDefault(PRElm => PRElm.Attribute("name").Value.Equals("CLCB_OC_GROUP_CD")).Value.Equals("PR"))) // When ALL CLCB_OC_GROUP_CD is PR
                    {
                        //Logger.LoggerInstance.ReportMessage("ProcessPRDeny ", " Original Result : NOT All CLCB_OC_GROUP_CD is PR");
                        lstPRData = lstPRData.Where(elm => elm.Elements().Any(e => e.Attribute("name").Value.Equals("CDCB_OC_GROUP_CD") && e.Value.Equals("PR"))).ToList(); // If not all Header level CLCB_OC_GROUP_CD = 'PR' then get line level CDCB_OC_GROUP_CD = 'PR'
                        //Logger.LoggerInstance.ReportMessage("ProcessPRDeny ", " Non LINE LEVEL PR count is : " + lstPRData.Count.ToString());
                        //lstPRData.ToList().ForEach(
                          //  elm => Logger.LoggerInstance.ReportMessage("Line level PR Content of ", elm.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value + "  is  " +
                                     //                                                               elm.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDCB_OC_GROUP_CD")).Value));
                    }
                    else
                    {
                        //Logger.LoggerInstance.ReportMessage("ProcessPRDeny ", " Original Result : All CLCB_OC_GROUP_CD is PR");
                    }

                    if (lstPRData.Count > 0)
                    {
                        //Logger.LoggerInstance.ReportMessage("ProcessPRDeny ", " Final PR count before processing is : " + lstPRData.Count.ToString());
                        //Logger.LoggerInstance.ReportMessage("ProcessPRDeny ", " Getting CDCB by calling GetCdcbData()");
                        GetCdcbData();
                        //Logger.LoggerInstance.ReportMessage("ProcessPRDeny ", " Called GetCdcbData()");

                        (
                             from cdml in _claim.claimLines.Where(cdml => cdml.CDML_ALLOW > 0.00 && !_claim.claimLineOverrides.Any(cdor => cdor.CDML_SEQ_NO.Equals(cdml.CDML_SEQ_NO) && cdor.CDOR_OR_ID.Equals("AX")))
                             join cdcb in _claim.ClaimCLCB.ClaimLineCOB.Where(cdcb => cdcb.CDCB_COB_AMT == 0.00) on
                             cdml.CDML_SEQ_NO equals cdcb.CDML_SEQ_NO
                             join PRElement in lstPRData on
                             cdml.CDML_SEQ_NO.ToString() equals PRElement.Elements().FirstOrDefault(elm => elm.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value
                             select cdml).ToList()
                                    .ForEach(cdml =>
                                    {
                                        blnPRUpdated = true;
                                        InsertCdorRecord(cdml.CLCL_ID, cdml.CDML_SEQ_NO, "AX", cdml.MEME_CK, cdml.CDML_CHG_AMT, "EXTN", "RR1");
                                    }
                                );

                    }

                    if (blnPRUpdated == true)
                    {
                        //Logger.LoggerInstance.ReportMessage("ProcessPRDeny ", " Inserted AX Overrides");
                        //Logger.LoggerInstance.ReportMessage("ProcessPRDeny ", " going to call InsertREPROC");
                        InsertREPROC();
                    }
                } // if (FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult))
            } // try


            catch (Exception ex)
            {
                //Logger.LoggerInstance.ReportError("Error in ProcessPRDeny ", ex.Message);
            }

            return blnPRUpdated;
        }

        

        /// <summary>
        /// Process Once in a LF rule by overriding the line with a Service Rule override
        /// </summary>
        /// <returns></returns>
        public bool ProcessOnceInALfTmClaim()
        {
            bool blFoundOnceInALfTm = false;
            bool recordsReturned = false;

            List<ClaimLine> OnceInALfTmHistForMember = new List<ClaimLine>();
            XA9DataLayer dataLayer = null;
            string strWmkMemberIDs = string.Empty;
            string strUniqueFamily = string.Empty;
            string strDataOutput = string.Empty;
            string strQuery = string.Empty;
            List<XElement> lstElmLfTmHistoryForWMKMeme = null;
            string strLftmFamily = string.Empty;
            DateTime dtCdmlFromDT;
            

            try
            {
                dataLayer = new XA9DataLayer();
                PopulateMemberCorrespondingWMKIDs(); // POPULATE all correponding MEME_CKs for the current member
                //strWmkMemberIDs = string.Join(",", _claim.WellmarkMemberContrievedKeys);
                strWmkMemberIDs = "'" + string.Join(",", _claim.WellmarkMemberContrievedKeys.Distinct()) + "'"; // 10222016
                strUniqueFamily = "'" + string.Join(",", _claim.claimLines.Where(line => !string.IsNullOrEmpty(line.OnceInALFGroup)).Select(line => line.OnceInALFGroup).Distinct()) + "'";
                strQuery = dataLayer.GetHistoryClaimsForWMKMembers(strWmkMemberIDs,strUniqueFamily, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strDataOutput);
                //Logger.LoggerInstance.ReportMessage("ProcessOnceInALfTmClaim", " strQuery is " + strQuery);
                //Logger.LoggerInstance.ReportMessage("ProcessOnceInALfTmClaim", " strDataOutput is " + strDataOutput);
                recordsReturned = FacetsData.FacetsInstance.IsDbDataAvailable(strDataOutput);
                if (recordsReturned)
                {
                    //lstElmLfTmHistoryForWMKMeme = FacetsData.FacetsInstance.GetMultipleDataElements(strDataOutput, "DATA", string.Empty)
                    var OnceInALifeTimeHistory =
                    (
                          from onlfHistory in FacetsData.FacetsInstance.GetMultipleDataElements(strDataOutput, "DATA", string.Empty)
                          group onlfHistory by onlfHistory.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("FAMILY")).Value into subg
                          select new { Family = subg.Key, CdmlFromDT = subg.Elements().Where(e => e.Attribute("name").Value.Equals("CDML_FROM_DT")).Select(e => DateTime.Parse(e.Value)).ToList() }
                    )
                    .ToDictionary(Key => Key.Family, Value => Value.CdmlFromDT.ToList());

                    //Logger.LoggerInstance.ReportMessage("ONCE IN A LF COUNT : ", OnceInALifeTimeHistory.Count.ToString());

                    foreach (var historyRecord in OnceInALifeTimeHistory)
                    {
                        _claim.claimLines.Where(line => line.OnceInALFGroup.Equals(historyRecord.Key) && !historyRecord.Value.Contains(line.CDML_FROM_DT) && 
                            !_claim.claimLineOverrides.Where(cdor => cdor.CDML_SEQ_NO.Equals(line.CDML_SEQ_NO)).Any(cdor => cdor.CDOR_OR_ID.Equals("SR"))).ToList().
                            ForEach(line => InsertCdorRecord(_claim.CLCL_ID, line.CDML_SEQ_NO, "SR", line.MEME_CK, 0.00, AppConfig.OnceInALfTmCdorVal, AppConfig.OnceInALfTmExcdID));

                        blFoundOnceInALfTm = true;
                    }

                }
                /* If no history is found then check once in a LF within the same claim - Begin */
                else
                {
                    var LinesToDenyForCurrentClaim =
                     (from line in _claim.claimLines.Where(line => !string.IsNullOrEmpty(line.OnceInALFGroup))
                      group line by new { line.OnceInALFGroup, line.CDML_FROM_DT } into groupedByFamilyAndFromDT
                      orderby groupedByFamilyAndFromDT.Key.OnceInALFGroup ascending, groupedByFamilyAndFromDT.Key.CDML_FROM_DT ascending
                      let Grouping = new { key = groupedByFamilyAndFromDT.Key, Value = groupedByFamilyAndFromDT.ToList() }
                      group Grouping by Grouping.key.OnceInALFGroup into groupedByFamilyOnly
                      select new { key = groupedByFamilyOnly.Key, Value = groupedByFamilyOnly.Skip(1).ToList() }
                      .Value).ToList();

                    List<ClaimLine> claimLinesToDeny;
                    foreach (var claimLines in LinesToDenyForCurrentClaim)
                    {
                        blFoundOnceInALfTm = true;
                        if (claimLines.Count() > 0)
                        {
                            foreach (var line in claimLines)
                            {
                                claimLinesToDeny = line.Value;
                                claimLinesToDeny.ForEach(
                                    Claimline => InsertCdorRecord(_claim.CLCL_ID, Claimline.CDML_SEQ_NO, "SR", Claimline.MEME_CK, 0.00, AppConfig.OnceInALfTmCdorVal, AppConfig.OnceInALfTmExcdID)
                                        );
                            }
                        }
                    }
                }
                /* If no history is found then check once in a LF within the same claim - End */
            }
            catch (Exception ex)
            {
                //Logger.LoggerInstance.ReportMessage("NON PAR - ProcessOnceInALfTmClaim", "At " + ContextData.ContextInstance.ExitTiming + " Exception IS " + ex.Message);
            }
            return blFoundOnceInALfTm;
        }

        public void RemoveSRandPAOverrides()
        {
            /* Remove SR and PA Overrides - Begin */
            bool found = false;

            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " REMOVING SERVICE RULE OF 000");
            List<XElement> cdorList = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL")).ToList();
            foreach (XElement element in cdorList)
            {
                found = element.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && e.Value.Equals("SR")) &&
                         element.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_VALUE") && e.Value.Equals("000"));

                if (found)
                    element.Remove();

                /* Remove PCA Disallow - Begin */
                found = element.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && e.Value.Equals("PA")) &&
                        element.Elements().Any(e=> e.Attribute("name").Value.Equals("CDOR_OR_VALUE") && e.Value.Equals("EXTN")) &&  // Only remove PCA override when it is being put in by the extension. 
                         element.Elements().Any(e => e.Attribute("name").Value.Equals("EXCD_ID") && e.Value.Equals("OPC"));

                if (found)
                    element.Remove();
                /* Remove PCA Disallow - End */
            }

            /* Remove SR and PA Overrides - End */
        }

        /// <summary>
        /// Revert SR values to its oriiginal state and remove any PA overrides that was put in by the extension to bypass PCA Pends
        /// </summary>
        internal void RevertSRAndPAOverrides()
        {
            try
            {
                RemoveClaimLineOverrides(new string[] { "SR" }, "000");
                RemoveClaimLineOverrides(new string[] { "PA" }, "EXTN");

                /* Put the original SR back - Begin */
                List<XElement> cdorOrigSRList = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL"))
                                                .Where(cdor => cdor.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && e.Value.Equals("01"))).ToList(); //.ToList();
                

                foreach (XElement element in cdorOrigSRList)
                {
                    InsertCdorRecord
                        (
                        element.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CLCL_ID")).Value,
                        int.Parse(element.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value),
                        "SR",
                        int.Parse(element.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("MEME_CK")).Value),
                        double.Parse(element.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDOR_OR_AMT")).Value),
                        element.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDOR_OR_VALUE")).Value,
                        element.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("EXCD_ID")).Value
                        );

                    element.Remove();
                }
                /* Put the original SR back - End */
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// This method includes any missing DF Messages into the claim that is currently being adjudicated by Facets
        /// </summary>
        public void InsertMissingDFMessages()
        {
            /* CHECK FOR THE NEED FOR MANUAL DF MESSAGE AND ATTACH TO THE PROCESS IF NEEDED - Start  */

            /* Defect XXXX - Begin */
            List<XElement> elmCMDMLstForDF = null;
            string strFacetsCode = string.Empty;
            string strITSCode = string.Empty;
            string strQuery = string.Empty;
            string strQueryResult = string.Empty;

            try
            {

                strQuery = "SELECT ICFI_ITS_CODE FROM CMC_ICFI_FAC_ITS WHERE ICFI_CATEGORY = 'MD' AND ICFI_FACETS_CODE = '{0}';";

                var ElementsCheckForManualDF = (
                                        from cdmdElement in FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDMDALL", "CDMDALL"))
                                        //join cdmlElement in _claim.claimLines.Where(line => line.IsDenied.Equals(true))
                                        join cdmlElement in _claim.claimLines.Where(line => line.IsDeniedNP.Equals(true))
                                        on cdmdElement.Elements().FirstOrDefault(cdmdE => cdmdE.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value equals
                                            cdmlElement.CDML_SEQ_NO.ToString()
                                        join cdimElement in FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDIMALL", "CDIMALL"))
                                        .Where(cdmi => cdmi.Elements().Any(cdmielm => cdmielm.Attribute("name").Value.Equals("CDIM_TYP") && cdmielm.Value.Equals("D")))
                                        on cdmdElement.Elements().FirstOrDefault(cdmdE => cdmdE.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value equals
                                        cdimElement.Elements().FirstOrDefault(cdimE => cdimE.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value into SubCdmiCollection
                                        //cdimElement.Elements().FirstOrDefault(cdimE => cdimE.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value 
                                        //where cdimElement.Elements().FirstOrDefault(cdimE => cdimE.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value.Equals("D") select cdimElement into SubCdmiCollection
                                        from cdmi in SubCdmiCollection.DefaultIfEmpty()
                                        where cdmi == null
                                        group
                                            //cdmdElement by cdmdElement.Elements().Where(e => e.Attribute("name").Value.Equals("CDML_SEQ_NO")).FirstOrDefault().Value into cdmdGrouped
                                        cdmdElement by cdmdElement.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value into cdmdGrouped
                                        //where
                                        //cdmdGrouped.Count() > 1
                                        select new { key = cdmdGrouped.Key, Value = cdmdGrouped.ToList() })
                                        .ToDictionary(CDML_SEQ_NO => CDML_SEQ_NO.key, Elements => Elements.Value);

                foreach (var cdmlseqno in ElementsCheckForManualDF.Keys)
                {
                    elmCMDMLstForDF = ElementsCheckForManualDF[cdmlseqno];
                    foreach (XElement elmCDMD in elmCMDMLstForDF)
                    {
                        strFacetsCode = elmCDMD.Elements().SingleOrDefault(elm => elm.Attribute("name").Value.Equals("EXCD_ID")).Value;
                        strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(string.Format(strQuery, strFacetsCode));
                        Logger.LoggerInstance.ReportMessage("ElementsCheckForManualDF", " DF Query for line number " + cdmlseqno + "  For Facets Code " + strFacetsCode + " is " + strQuery);
                        Logger.LoggerInstance.ReportMessage("ElementsCheckForManualDF", " DF Code for line number " + cdmlseqno + "  For Facets Code " + strFacetsCode + " is " + strQueryResult);
                        if (FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult))
                        {
                            strITSCode = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "ICFI_ITS_CODE", false);
                            InsertCdimRecord(
                                elmCDMD.Elements().SingleOrDefault(elm => elm.Attribute("name").Value.Equals("CLCL_ID")).Value,
                                elmCDMD.Elements().SingleOrDefault(elm => elm.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value,
                                strITSCode,
                                "D",
                                elmCDMD.Elements().SingleOrDefault(elm => elm.Attribute("name").Value.Equals("MEME_CK")).Value,
                                "Y",
                                "0.0000",
                                "0.0000",
                                "",
                                //"EXTN"
                                strFacetsCode
                                );
                            break;
                        }
                    }

                    /* Defect XXXX - End x */
                }

                /* Check for IZ and add if any DF message is missing IZ - Begin */

                var IZElementsRequired = (
                    from cdimElement in FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDIMALL", "CDIMALL"))
                    .Where(cdmi => cdmi.Elements().Any(cdmielm => cdmielm.Attribute("name").Value.Equals("CDIM_TYP") && cdmielm.Value.Equals("D"))) // For DF Message
                    join cdorElement in FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL"))
                    .Where(cdor => cdor.Elements().Any(cdorelm => cdorelm.Attribute("name").Value.Equals("CDOR_OR_ID") && cdorelm.Value.Equals("IZ"))) // CDOR_OR_ID for DF Message
                    on cdimElement.Elements().FirstOrDefault(cdmdE => cdmdE.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value equals
                        cdorElement.Elements().FirstOrDefault(cdimE => cdimE.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value into SubIzCdorCollection
                    from cdorIZ in SubIzCdorCollection.DefaultIfEmpty()
                    where cdorIZ == null
                    select cdimElement).ToList();

                IZElementsRequired.ForEach
                    (cdimElm =>
                        InsertCdorRecord(
                                    cdimElm.Elements().SingleOrDefault(elm => elm.Attribute("name").Value.Equals("CLCL_ID")).Value,
                                    int.Parse(cdimElm.Elements().SingleOrDefault(elm => elm.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value),
                                    "IZ",
                                    int.Parse(cdimElm.Elements().SingleOrDefault(elm => elm.Attribute("name").Value.Equals("MEME_CK")).Value),
                                    double.Parse("0"),
                                    string.Empty,
                                    "314"
                                    )
                    );
            }
            catch (Exception ex)
            {
                
            }
            /* Check for IZ and add if any DF message is missing IZ - End */
            /* CHECK FOR THE NEED FOR MANUAL DF MESSAGE AND ATTACH TO THE PROCESS IF NEEDED - END  */
        }
        /// <summary>
        /// Process ITS price
        /// </summary>
        /// <param name="strForcePricing">Will force the extension to apply either NWX or HOST price. Can be empty</param>
        /// <returns></returns>
        //public bool ProcessITSNonParPricing() // Defect 3329 - Commented this non parameterized method
        public bool ProcessITSNonPar(string strForcePricing) // Defect 3329 - Added an input parameter to supply the type of pricing to implement.
        {
            double dblITSPrice = 0.00;
            double dblNWXPrice = 0.00;
            bool blnPriceOverride = false; 
            string strProxyPrprIDPayAt100Pct = string.Empty;
            string strQueryResult = string.Empty; 
            string strQuery = string.Empty;
            string strOrigPriceSource = string.Empty;
            double dblFacDefaultPct = 0.00;
            string[] arrNpprCustomPct = {"1011","1012","1013"};
            bool found = false;
            try
            {
                GetClaimData();
                GetClaimLineData();
                GetClaimLineOverrideData();
                GetClaimOverrideData();
                
                if (string.IsNullOrEmpty(_claim.PDBC_PFX_NPPR))
                {
                    _claim.PDBC_PFX_NPPR = GetNpprPfxForProduct();
                }

                strProxyPrprIDPayAt100Pct = AppConfig.ITSProxyPrprIDPayAt100Pct;
                //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "strProxyPrprIDPayAt100Pct is " + strProxyPrprIDPayAt100Pct);

                if (ContextData.ContextInstance.ExitTiming.Equals("POSTELIG"))
                {
                    #region POSTELIG MEDICAL
                    if (_claim.CLCL_CL_SUB_TYPE == "M")
                    {

                        GetProviderData();

                        // If CLOR NP exists and the PRPR_ID is the Proxy provider then remove NP override. Save it

                        //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " PDBC_PFX_NPPR is " + _claim.PDBC_PFX_NPPR);
                        if (AppConfig.ITSNPProfNPPRRevertToChg.Contains(_claim.PDBC_PFX_NPPR) &&
                            (AppConfig.ITSNPProfMCTRRevertToChg.Contains(_claim.ServicingProvider.PRPR_MCTR_TYPE) || AppConfig.ITSNPProfMCTRRevertToChg.Contains(_claim.ServicingProvider.PRCF_MCTR_SPEC)) &&
                            !_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IK"))
                           )

                        {
                            Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Non Par Claim Revert to Charge ");

                            /* Defect 5345 - Before reverting back to charge, make sure to process BDC and Once in a LF - Begin */
                            #region Medical Claims - Process Once In a Liffetime for Claims that will always be applied Host Price
                            /* Call Once In a LF - Begin */
                            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Force Pricing with ITS. Processing Once in a Life Time - BEGIN");
                            PopulateMemberCorrespondingWMKIDs();
                            bool isOnceInALftm = PutOnceInALFStatusForEachLine();
                            if (isOnceInALftm)
                            {
                                isOnceInALftm = ProcessOnceInALfTmClaim();
                                Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Processed on in a LF for this Non Par Claim Revert to Charge professional claim ");
                            }
                            /* Call Once In a LF - End */
                            Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Force Pricing with ITS. Processing Once in a Life Time - END");
                            #endregion Medical Claims - Process Once In a Liffetime for Claims that will always be applied Host Price

                            #region Medical Claims - Process BDC for Non Par Claims that will always be applied Host Price
                            /* Process BDC - Begin */
                            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Force Pricing Medical Claim with ITS. Processing BDC - BEGIN");
                            SetClaimTOSOverride();
                            Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Processed BDC for this Non Par Claim Revert to Charge professional claim ");
                            /* Process BDC - End */
                            #endregion Medical Claims - Process BDC for Non Par Claims that will always be applied Host Price

                            /* Defect 5345 - Before reverting back to charge, make sure to process BDC and Once in a LF - End */

                           
                            RemoveClaimLineOverrides(new string[] { "EP" });

                            blnPriceOverride = true;
                        }
                        /// IF original price is WMK then no need to capture orig provider data as we are not going to calculate ITS price
                        //else if (!_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IL")) && !_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("NP")))
                        else if (!_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IL"))
                                && !_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IK"))
                                && !_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("NP")))
                        {
                            
                            _claim.claimLineOverrides.Where(cdor => cdor.CDOR_OR_ID.Equals("EP") && cdor.EXCD_ID.Equals(AppConfig.ITSEPManualPriceExcdID)).ToList()
                                .ForEach(cdor => InsertCdorRecord(cdor.CLCL_ID, cdor.CDML_SEQ_NO, "MP", cdor.MEME_CK, cdor.CDOR_OR_AMT,"EXTN", AppConfig.ITSEPManualPriceExcdID));
                            RemoveClaimLineOverrides(new string[] { "EP" }); // Remove existing External Price before processing

                            //InsertClorRecord(_claim.CLCL_ID, "NP", "0", "", _claim.PRPR_ID + "-" + _claim.AGAG_ID, AppConfig.ITSNonParNPClaimLevelOverrideExcdID);
                            InsertClorRecord(_claim.CLCL_ID, "NP", "0", "", _claim.PRPR_ID, AppConfig.ITSNonParNPClaimLevelOverrideExcdID);
                            Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . REPLACEDDDDDDDDDDDD ORIGINAL PROVIDER WITH PROXY PROVIDER ID");
                            FacetsData.FacetsInstance.SetSingleDataItem("CLCL", "PRPR_ID", AppConfig.ITSProxyPrprIDPayAt100Pct); // Replace with Proxy Provider ID

                            if (string.IsNullOrEmpty(strForcePricing)) // If we are not forcing a predefined price, then get the RAW Host price which is required for compare
                            {
                                #region Medical Claims - Override SR to get the RAW Host Price
                                //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " PUTTING SERVICE RULE OF 000");
                                /* Copy the original Service Rule into a dummy CDOR - Begin */

                                RemoveClaimLineOverrides(new string[] { "01" }); // Remove any previously captured SR overrides.
                                _claim.claimLineOverrides.Where(cdor => cdor.CDOR_OR_ID.Equals("SR")).ToList()
                                    .ForEach(cdor => InsertCdorRecord(cdor.CLCL_ID, cdor.CDML_SEQ_NO, "01", cdor.MEME_CK, 0.00, cdor.CDOR_OR_VALUE, cdor.EXCD_ID)); // Capture all SR overrides before replacing with SR 000

                              

                                RemoveClaimLineOverrides(new string[] { "SR" });
                                /* Copy the original Service Rule into a dummy CDOR - End */

                                _claim.claimLines.ForEach(
                                        Claimline => InsertCdorRecord(_claim.CLCL_ID, Claimline.CDML_SEQ_NO, "SR", Claimline.MEME_CK, 0.00, "000", "041")
                                            );

                                Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " PUTTING SERVICE RULE OF 000");

                                /* For lines that does not have a PA override, add it to override PCA disallow - Begin*/

                                _claim.claimLines.Where(cdml => !_claim.claimLineOverrides.Where(cdor => cdor.CDML_SEQ_NO.Equals(cdml.CDML_SEQ_NO)).Any(cdor => cdor.CDOR_OR_ID.Equals("PA")))
                                    .ToList().ForEach(cdor => InsertCdorRecord(cdor.CLCL_ID, cdor.CDML_SEQ_NO, "PA", cdor.MEME_CK, 0.00, "EXTN", "OPC"));

                                /* For lines that does not have a PA override, add it to override PCA disallow - End*/
                                #endregion Medical Claims - Override SR to get the RAW Host Price
                            }
                            else if(strForcePricing.Equals("ITS"))
                            {
                                #region Medical Claims - Process Once In a Liffetime for Claims that will always be applied Host Price
                                /* Call Once In a LF - Begin */
                                //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Force Pricing with ITS. Processing Once in a Life Time - BEGIN");
                                PopulateMemberCorrespondingWMKIDs();
                                bool isOnceInALftm = PutOnceInALFStatusForEachLine();
                                if (isOnceInALftm)
                                {
                                    isOnceInALftm = ProcessOnceInALfTmClaim();
                                    //Logger.LoggerInstance.ReportMessage("******ITSNonParEntryPoint****", "  THIS IS A ONCE IN A LIFE TIME HIT CLAIM****");
                                }
                                /* Call Once In a LF - End */
                                Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Force Pricing with ITS. Processing Once in a Life Time - END");
                                #endregion Medical Claims - Process Once In a Liffetime for Claims that will always be applied Host Price

                                #region Medical Claims - Process BDC for Non Par Claims that will always be applied Host Price
                                /* Process BDC - Begin */
                                //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Force Pricing Medical Claim with ITS. Processing BDC - BEGIN");
                                SetClaimTOSOverride();
                                Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Force Pricing Medical Claim with ITS. Processing BDC - END");
                                /* Process BDC - End */
                                #endregion Medical Claims - Process BDC for Non Par Claims that will always be applied Host Price
                            }
                            blnPriceOverride = true;

                        }
                        /* For Manual Price 10272016 - Begin */
                        // Insert all the manual price as allowable amount override here
                        else if (_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IL")))
                        {

                            if (!_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("00")))
                            {
                                #region Medical Claims - Revert SR to its original value and remove PA overrides that was put in by the extension to get unaltered Host Price
                                if (string.IsNullOrEmpty(strForcePricing))
                                {
                                    RevertSRAndPAOverrides();

                                    _claim.claimLineOverrides = null;
                                    GetClaimLineOverrideData();
                                }
                                #endregion Medical Claims - Revert SR to its original value and remove PA overrides that was put in by the extension to get unaltered Host Price

                                #region Medical Claims - Check for Once in a Lifetime for Non Par Claims
                                /* Call Once In a LF - Begin */
                                Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Processing Once in a Life Time for NWX cycle- BEGIN");
                                PopulateMemberCorrespondingWMKIDs();
                                bool isOnceInALftm = PutOnceInALFStatusForEachLine();
                                if (isOnceInALftm)
                                {
                                    isOnceInALftm = ProcessOnceInALfTmClaim();
                                    //Logger.LoggerInstance.ReportMessage("******ITSNonParEntryPoint****", "  THIS IS A ONCE IN A LIFE TIME HIT CLAIM****");
                                }
                                /* Call Once In a LF - End */
                                #endregion Medical Claims - Check for Once in a Lifetime for Non Par Claims

                                #region Medical Claims - Process BDC for Non Par Claims
                                /* Process BDC - Begin */
                                SetClaimTOSOverride();
                                /* Process BDC - End */
                                #endregion Medical Claims - Process BDC for Non Par Claims

                                InsertClorRecord(_claim.CLCL_ID, "00", _claim.MEME_CK.ToString(), "0.00", "EXTN", ""); // This is a dummy CDOR to indicate that the claim has not been priced yet
                                Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . ****** INSERTED DUMMY 00 OVERRIDE TO INDICATE THAT CLAIM HAS NOT BEEN PRICED");
                               
                            }

                            /*TESTING ONLY - Deny all lines with SR override - Begin
                            _claim.claimLines.
                                ForEach(line => InsertCdorRecord(_claim.CLCL_ID, line.CDML_SEQ_NO, "SR", line.MEME_CK, 0.00,AppConfig.OnceInALfTmCdorVal , AppConfig.OnceInALfTmExcdID));
                              TESTING ONLY - Deny all lines with SR override - End */
                            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Processing Once in a Life Time - END");

                            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Adding all Manual Price into Allowable Amount Overrides");
                            _claim.claimLineOverrides.Where(clor => clor.CDOR_OR_ID.Equals("MP") && clor.EXCD_ID.Equals(AppConfig.ITSEPManualPriceExcdID)).ToList()
                                .ForEach(cdor => InsertCdorRecord(cdor.CLCL_ID, cdor.CDML_SEQ_NO, "AA", cdor.MEME_CK, cdor.CDOR_OR_AMT, "EXTN", ""));

                            _claim.claimLineOverrides.Where(clor => clor.CDOR_OR_ID.Equals("MP") && clor.EXCD_ID.Equals(AppConfig.ITSEPManualPriceExcdID)).ToList()
                                .ForEach(cdor => InsertCdorRecord(cdor.CLCL_ID, cdor.CDML_SEQ_NO, "NW", cdor.MEME_CK, cdor.CDOR_OR_AMT,"EXTN", ""));


                            Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . ****** PUTTING MANUAL PRICE INTO AA AND NW OVERRIDES FOR WMK CYCLE");
                            blnPriceOverride = true;
                        }
                        /* For Manual Price 10272016 - Begin */
                    }
                #endregion  #region POSTELIG MEDICAL

                    #region POSTELIG HOSPITAL
                    else if (_claim.CLCL_CL_SUB_TYPE == "H")
                    {
                        GetClhpData();
                        //if (AppConfig.ITSNPInpHospNPPRRevertToChg.Contains(_claim.PDBC_PFX_NPPR)) 
                        //if (_claim.HospClaimData.POS_IND.Equals("I") && AppConfig.ITSNPInpHospNPPRRevertToChg.Contains(_claim.PDBC_PFX_NPPR) && !_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IK"))) // Commenting for 5345
                        if (_claim.HospClaimData.POS_IND.Equals("I") && AppConfig.ITSNPInpHospNPPRRevertToChg.Contains(_claim.PDBC_PFX_NPPR)) // && !_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IK"))) // Adding for 5345
                        {
                            Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Non Par Claim Revert to Charge ");
                            if (!_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IK")))
                            {
                                #region Hospital Claims - Process BDC for Non Par Claims
                                /* Process BDC - Begin */
                                SetClaimTOSOverride();

                                Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Processed BDC for Non Par Hospital Claim that Reverted to Charge ");
                                /* Process BDC - End */
                                
                                #endregion Hospital Claims - Process BDC for Non Par Claims

                                RemoveExternalPrice();
                            }
                            blnPriceOverride = true;
                        }
                        /* For defect 5345 - begin */
                        else if (_claim.HospClaimData.POS_IND.Equals("I") && arrNpprCustomPct.Contains(_claim.PDBC_PFX_NPPR))
                        {
                            if (!_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("NP")))
                            {
                                RemoveExternalPrice();
                                 #region Hospital Claims - Process BDC for Non Par Claims
                                /* Process BDC - Begin */
                                SetClaimTOSOverride();

                                Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Processed BDC for Non Par Hospital Claim with custom NPPR values to charge at 75 % ");
                                /* Process BDC - End */
                                #endregion Hospital Claims - Process BDC for Non Par Claims
                                blnPriceOverride = true;
                            }
                        }
                        /* For defect 5345 - end */

                        //else if (!_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("NP")) && !_claim.claimLineOverrides.Any(cdor => cdor.CDOR_OR_ID.Equals("NP")))
                        // When the claims doesnt have host price and no price has been confirmed yet, then come here.
                        else if (!_claim.claimLineOverrides.Any(cdor => cdor.CDOR_OR_ID.Equals("NP") && cdor.EXCD_ID.Equals(AppConfig.ITSNonParNPHostLineLevelOverrideExcdID))
                                                                        && !_claim.claimOverrides.Any(clor =>clor.CLCL_ID.Equals(_claim.CLCL_ID) && clor.CLOR_OR_ID.Equals("NP"))
                            )
                        {
                            List<XElement> cdorList = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL")).ToList();
                            found = false;

                            foreach (XElement element in cdorList)
                            {
                                found = element.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && e.Value.Equals("EP"));
                                if (found)
                                {
                                    Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " Removing existing External Price for Line # " + element.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value);
                                    element.Remove();
                                }
                            }


                            //if (string.IsNullOrEmpty(strForcePricing) && !(_claim.HospClaimData.POS_IND.Equals("I") && arrNpprCustomPct.Contains(_claim.PDBC_PFX_NPPR))) // Commenting for Defect 5345
                            if (string.IsNullOrEmpty(strForcePricing))                                                                                                     // Adding for Defect 5345
                            {
                                #region Hospital Claims - Override SR to get the RAW Host Price
                                /* Copy the original Service Rule into a dummy CDOR - Begin  */
                                //****NOTE NOTE NOTE::: before putting this 01 override delete it based on Prasad's email
                                RemoveClaimLineOverrides(new string[] { "01" }); // Remove any previously captured SR overrides.
                                _claim.claimLineOverrides.Where(cdor => cdor.CDOR_OR_ID.Equals("SR")).ToList()
                                    .ForEach(cdor => InsertCdorRecord(cdor.CLCL_ID, cdor.CDML_SEQ_NO, "01", cdor.MEME_CK, 0.00, cdor.CDOR_OR_VALUE, cdor.EXCD_ID));

                                List<XElement> cdorSRList = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL")).ToList();
                                found = false;
                                foreach (XElement element in cdorSRList)
                                {
                                    found = element.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && e.Value.Equals("SR"));
                                    if (found)
                                    {
                                        element.Remove();
                                    }
                                }
                                /*  Copy the original Service Rule into a dummy CDOR - End */

                                /* Include a SR override of 000 to pay as charge for the HOST price - Begin */
                                Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " PUTTING SERVICE RULE OF 000");
                                _claim.claimLines.ForEach(
                                        Claimline => InsertCdorRecord(_claim.CLCL_ID, Claimline.CDML_SEQ_NO, "SR", Claimline.MEME_CK, 0.00, "000", "041")
                                            );

                                Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " PUTTING SERVICE RULE OF 000");
                                /* Include a SR override of 000 to pay as charge for the HOST price - End */

                                /* For lines that does not have a PA override, add it to override PCA disallow - Begin */
                                _claim.claimLines.Where(cdml => !_claim.claimLineOverrides.Where(cdor => cdor.CDML_SEQ_NO.Equals(cdml.CDML_SEQ_NO)).Any(cdor => cdor.CDOR_OR_ID.Equals("PA")))
                                   .ToList().ForEach(cdor => InsertCdorRecord(cdor.CLCL_ID, cdor.CDML_SEQ_NO, "PA", cdor.MEME_CK, 0.00, "EXTN", "OPC"));
                                /* For lines that does not have a PA override, add it to override PCA disallow - End*/
                                #endregion Hospital Claims - Override SR to get the RAW Host Price
                            }
                            //else if(strForcePricing.Equals("ITS"))
                            else
                            {
                                #region Hospital Claims - Process BDC for Non Par Claims
                                /* Process BDC - Begin */
                                Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Force Pricing Hospital Claim with " + strForcePricing + " . Processing BDC - BEGIN");
                                SetClaimTOSOverride();
                                Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Force Pricing Hospital Claim with " + strForcePricing + " . Processing BDC - END");
                                /* Process BDC - End */
                                #endregion Hospital Claims - Process BDC for Non Par Claims
                            }
                            blnPriceOverride = true;
                        }
                        //else
                        //else if(!_claim.claimOverrides.Any(clor=>clor.CLOR_OR_ID.Equals("NP")))
                        else if (string.IsNullOrEmpty(strOrigPriceSource)) // if the prices of the claim has been compared already, then revert 
                        {
                            // Remove the SR and PA (PCA Denial) overrides

                            #region Hospital Claims - Revert SR to its original value and remove PA overrides that was put in by the extension to get unaltered Host Price

                            RevertSRAndPAOverrides();
                            /* Put the original SR back - Begin 
                            if (string.IsNullOrEmpty(strForcePricing))
                            {
                                List<XElement> cdorOrigSRList = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL")).ToList();
                                found = false;

                                foreach (XElement element in cdorOrigSRList)
                                {
                                    found = element.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && e.Value.Equals("01"));
                                    if (found)
                                    {

                                        InsertCdorRecord
                                            (
                                            element.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CLCL_ID")).Value,
                                            int.Parse(element.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value),
                                            "SR",
                                            int.Parse(element.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("MEME_CK")).Value),
                                            double.Parse(element.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDOR_OR_AMT")).Value),
                                            element.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDOR_OR_VALUE")).Value,
                                            element.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("EXCD_ID")).Value
                                            );

                                        element.Remove();
                                    }

                                    /*  Put the original SR back - End 

                                    // Remove PCA override put in by the extension - Begin 
                                    found = element.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_ID") && e.Value.Equals("PA")) &&
                                            element.Elements().Any(e => e.Attribute("name").Value.Equals("CDOR_OR_VALUE") && e.Value.Equals("EXTN"));
                                    if (found)
                                    {
                                        element.Remove();
                                    }
                                     Remove PCA override put in by the extension - End 
                                }
                            }*/
                            #endregion Hospital Claims - Revert SR to its original value and remove PA overrides that was put in by the extension to get unaltered Host Price

                            /* Put the original SR back - End */
                            #region Hospital Claims - Process BDC for Non Par Claims
                            /* Process BDC - Begin */
                            SetClaimTOSOverride();
                            /* Process BDC - End */
                            #endregion Hospital Claims - Process BDC for Non Par Claims

                            blnPriceOverride = true;
                        }
                    }
                    #endregion POSTELIG HOSPITAL
                }
                
                else if (ContextData.ContextInstance.ExitTiming.Equals("POSTPRICECLM")) // POSTPRICECLM
                {
                    #region POSTPRICECLM MEDICAL
                    if (_claim.CLCL_CL_SUB_TYPE == "M")
                    {

                        //Logger.LoggerInstance.ReportMessage("99999999999999999999999", "999999999999999999999999999999999999999999999999999999999999");

                        /* Defect 3636 - Begin */
                        GetProviderData(); // Defect 3636 - Pull provider data
                        /* Defect 3636 - End */
                        if (!(AppConfig.ITSNPProfNPPRRevertToChg.Contains(_claim.PDBC_PFX_NPPR)
                                && (AppConfig.ITSNPProfMCTRRevertToChg.Contains(_claim.ServicingProvider.PRPR_MCTR_TYPE) || AppConfig.ITSNPProfMCTRRevertToChg.Contains(_claim.ServicingProvider.PRCF_MCTR_SPEC)))
                                && (!_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IK")))
                                && (_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IL")))
                            //&& !_claim.claimLineOverrides.Any(cdor => cdor.CDOR_OR_ID.Equals("EP") && AppConfig.ITSNPEXCDID.Contains(cdor.EXCD_ID)) 10152016 Commenting this out. Why do we have to check for EP?
                                )
                        {
                            //Logger.LoggerInstance.ReportMessage("111111111111111111111111111111111111", "11111111111111111111111111111111111111111111111111111111111111111");

                            // If user had put in the IL override manually then do not perform anything
                            if (!_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("NP")))
                            {
                                blnPriceOverride = false;
                            }
                            else
                            {
                                //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", " AT " + ContextData.ContextInstance.ExitTiming + " . Retrieved Wellmark Networx Price as below.");


                                /* 10242016 - For claim line with $0 price but agreement price greater than 0, get the manual price from EP - Begin */

                                List<int> arrManualSeqNos = _claim.claimLineOverrides.Where(cdor => cdor.CDOR_OR_ID.Equals("MP"))
                                                            .Select(codr => codr.CDML_SEQ_NO).ToList();

                                //arrManualSeqNos.ForEach(number => Logger.LoggerInstance.ReportMessage("Manual Pricing exists for line ", number.ToString()));


                                Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", " AT " + ContextData.ContextInstance.ExitTiming + " . Retrieved Wellmark Networx Price as below.");
                                _claim.claimLines
                                    .ForEach(line => Logger.LoggerInstance.ReportMessage("Wellmark NetworX Price for Line ", line.CDML_SEQ_NO.ToString() + " IS " + line.CDML_ALLOW));

                                /* Use Manual Price for Wellmark Price - Begin */

                                
                                _claim.claimLines.Where(cdml => !(arrManualSeqNos.Contains(cdml.CDML_SEQ_NO))).ToList()
                                                         .ForEach(l => InsertCdorRecord(l.CLCL_ID, l.CDML_SEQ_NO, "NP", l.MEME_CK, l.CDML_ALLOW, "NWX", AppConfig.ITSNonParNPWMKLineLevelOverrideExcdID));

                                _claim.claimLineOverrides.Where(cdor => cdor.CDOR_OR_ID.Equals("MP")).ToList()
                                                         .ForEach(l => InsertCdorRecord(l.CLCL_ID, l.CDML_SEQ_NO, "NP", l.MEME_CK, l.CDOR_OR_AMT, "NWX", AppConfig.ITSNonParNPWMKLineLevelOverrideExcdID));


                                /* For Manual Pricing Issue - Begin */
                                XElement clor00Element = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CLOR", "CLOR"))
                                    .FirstOrDefault(cdorElm =>  cdorElm.Elements().Any(elm => elm.Attribute("name").Value.Equals("CLOR_OR_ID") && elm.Value.Equals("00")) && 
                                                              cdorElm.Elements().Any(elm => elm.Attribute("name").Value.Equals("CLOR_OR_VALUE") && elm.Value.Equals("EXTN"))
                                                              );
                                if (clor00Element != null)
                                {
                                    clor00Element.Remove();
                                }

                                Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . ****** REMOVING 00 DUMMY OVERRIDE AFTER CLAIM IS PRICED DURING WMK CYCLE");
                                /* For Manual Pricing Issue - End */
                                
                                blnPriceOverride = true;
                            }
                        }
                    }
                #endregion POSTPRICECLM MEDICAL
                } // else if (ContextData.ContextInstance.ExitTiming.Equals("POSTPRICECLM"))
                
                #region POSTPROC MEDICAL
                else if (ContextData.ContextInstance.ExitTiming.Equals("POSTPROC"))
                {
                    //Logger.LoggerInstance.ReportMessage("888888888888888888888", "88888888888888888888888888888888888888888888888888888888888888888");

                    if (_claim.CLCL_CL_SUB_TYPE.Equals("M"))
                    {
                        /* Defect 3329 - Begin */
                        if (!string.IsNullOrEmpty(strForcePricing)) // If we know upfront what pricing to apply, get it from the input parameter. For example, for emergency medical claim, always use HOST price and do not perform price comparision.
                        {
                            strOrigPriceSource = strForcePricing;
                        }
                        /* Defect 3329 - End 
                        else if (_claim.IsAdjustedClaim)
                        {
                            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " This is an adjusted claim");
                            strOrigPriceSource = GetAdjustedClaimPricingMethod();
                        }*/

                        GetProviderData();
                        //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " PDBC_PFX_NPPR is " + _claim.PDBC_PFX_NPPR);
                        // If NPPR is revert to charge, then insert an IK override and reprocess the claim
                        if (AppConfig.ITSNPProfNPPRRevertToChg.Contains(_claim.PDBC_PFX_NPPR)
                            && (AppConfig.ITSNPProfMCTRRevertToChg.Contains(_claim.ServicingProvider.PRPR_MCTR_TYPE) || AppConfig.ITSNPProfMCTRRevertToChg.Contains(_claim.ServicingProvider.PRCF_MCTR_SPEC))
                            && (!_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IK")))
                            )
                        {
                            Logger.LoggerInstance.ReportMessage("2222222222222222222222222", "2222222222222222222222222222222222222222222222222222222222");

                            //InsertClorRecord(string.Empty, "IK", "0", "0.0000", string.Empty, AppConfig.ITSIKEXCDID);
                            InsertClorRecord(_claim.CLCL_ID, "IK", _claim.MEME_CK.ToString(), "0.0000", string.Empty, AppConfig.ITSIKEXCDID);
                            Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Applied Revert to Charge ");
                            // Insert WMK Consider Charge Amount for all lines as EP BWP
                            //_claim.claimLines.ForEach(cdml => // Defect 5345
                            _claim.claimLines.Where(line => !line.IsDeniedNP).ToList().ForEach(cdml =>
                                    InsertCdorRecord(cdml.CLCL_ID, cdml.CDML_SEQ_NO, "EP", cdml.MEME_CK, cdml.CDML_CHG_AMT, "NWX", "BNW"));

                           
                            Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Non Par Claim - Applied Revert to Charge and applied EP charge");
                            /*

                            _claim.claimLines.ForEach(cdml =>
                                InsertCdorRecord(cdml.CLCL_ID, cdml.CDML_SEQ_NO, "EP", cdml.MEME_CK, cdml.CDML_CONSIDER_CHG, "NWX", "BNW")
                            );
                            */

                            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Applied EP as Consider Charge ");

                            blnPriceOverride = true;
                            InsertMissingDFMessages(); // Defect 5345
                            InsertREPROC();
                        }

                        /// Replace the provider ID and Agreement to Original data and reproc the claim to get the Wellmark Price
                        else if (
                                    (!_claim.claimLineOverrides.Any(cdor => cdor.CDOR_OR_ID.Equals("EP") && AppConfig.ITSNPEXCDID.Contains(cdor.EXCD_ID))) &&
                                    !_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IK"))
                                    && !_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IL"))

                                )
                        {
                            //Logger.LoggerInstance.ReportMessage("3333333333333333333333333", "33333333333333333333333333333333333333333333333333333333333333333");

                            Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . *********** ENTERING POST PROC FOR HOST CYCLE **********");
                            
                            string strOrigProviderID = _claim.claimOverrides.FirstOrDefault(clor => clor.CLOR_OR_ID.Equals("NP")).CLOR_OR_VALUE;
                            //string[] arrOrigPrpr = strOrigProviderID.Split('-');
                            
                            FacetsData.FacetsInstance.SetSingleDataItem("CLCL", "PRPR_ID", strOrigProviderID); // Replace with Original Provider ID
                            blnPriceOverride = true;


                            ///ITS Allowed amount retrieved from ITS Allowable amounts
                            var AllowableAmounts = (from cdml in _claim.claimLines
                                                    join cdor in _claim.claimLineOverrides.
                                                    Where(cdor => cdor.CDOR_OR_ID.Equals("AA")) on cdml.CDML_SEQ_NO equals cdor.CDML_SEQ_NO into subCodrCollections
                                                    from subcdor in subCodrCollections.DefaultIfEmpty((new Cdor()
                                                    {
                                                        CLCL_ID = cdml.CLCL_ID,
                                                        MEME_CK = cdml.MEME_CK,
                                                        CDML_SEQ_NO = cdml.CDML_SEQ_NO,
                                                        CDOR_OR_AMT = cdml.CDML_CHG_AMT - cdml.CDML_DISALL_AMT,
                                                        CDOR_OR_VALUE = "EXTN", // this means that host did not send this value 
                                                        CDOR_OR_ID = "AA"

                                                    }))
                                                    select subcdor).ToList();
                            Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " Calculated ITS price");

                            //_claim.claimLineOverrides.ForEach(cdor => Logger.LoggerInstance.ReportMessage("CDOR VALUE", "SEQ " + cdor.CDML_SEQ_NO + " _ ID " + cdor.CDOR_OR_ID + " _ AMT " + cdor.CDOR_OR_AMT.ToString() + " - EXCD " + cdor.EXCD_ID + " _ VAL " + cdor.CDOR_OR_VALUE));
                            AllowableAmounts.ForEach(cdor => Logger.LoggerInstance.ReportMessage("CDOR VALUE", "SEQ " + cdor.CDML_SEQ_NO + " _ ID " + cdor.CDOR_OR_ID + " _ AMT " + cdor.CDOR_OR_AMT.ToString() + " - EXCD " + cdor.EXCD_ID + " _ VAL " + cdor.CDOR_OR_VALUE));

                            //RemoveITSOverrides(); // Remove all pricing overrides (Allowable, disallow amount) obtained from HOST

                            /* 10052015 - New Code. When the HOST price is 0 then do not do anything - CHECK WITH BUSINESS - Begin */
                            if (AllowableAmounts.Sum(cdor => cdor.CDOR_OR_AMT) > 0.00) // only if host allowable is > 0. If not then do not do anything. Just accept the Host price.
                            {
                                if (strOrigPriceSource.Equals("ITS"))
                                {
                                    //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + "*** PREVIOUS CLAIM WAS PROCESSED AT ITS PRICE. SO USE ITS PRICE");
                                    AllowableAmounts.Where(a => a.CDOR_OR_AMT > 0.00 || !a.CDOR_OR_VALUE.Equals("EXTN")).ToList()
                                        .ForEach(AA => InsertCdorRecord(AA.CLCL_ID, AA.CDML_SEQ_NO, "EP", AA.MEME_CK, AA.CDOR_OR_AMT, "ITS", "BNH"));       // 10222016

                                    if (AllowableAmounts.Any(aa => aa.CDOR_OR_AMT > 0.00 || !aa.CDOR_OR_VALUE.Equals("EXTN")))
                                    {
                                        /* For INH Issue - Begin */
                                        InsertClorRecord(
                                            _claim.CLCL_ID,
                                            "IK",
                                            _claim.MEME_CK.ToString(),
                                            "0.00",
                                            //string.Empty,
                                            "EXTN",
                                            "325"
                                            );
                                        /* For INH Issue - End */
                                    }
                                    //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing ", "Using ITS price. Putting in NW overrides to avoid Manual Price Requirement");
                                    _claim.claimLines.ForEach(cdml => InsertCdorRecord(cdml.CLCL_ID, cdml.CDML_SEQ_NO, "NW", cdml.MEME_CK, cdml.CDML_ALLOW, "EXTN", "")); // If accepting ITS price without comparing, put NW overrides to avoid Manual Pricing requirement.
                                    //RemoveSRandPAOverrides(); // If accepting Host price, we will not calculate Wellmark Price. Remove SR 000 and PA override which we used to bypass WMK benefit calculation
                                }
                                else
                                {
                                    // Capture HOST Price for each line in NP override
                                    AllowableAmounts.ForEach(l => InsertCdorRecord(l.CLCL_ID, l.CDML_SEQ_NO, "NP", l.MEME_CK, l.CDOR_OR_AMT, l.CDOR_OR_VALUE, AppConfig.ITSNonParNPHostLineLevelOverrideExcdID)); // 10222016
                                    //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " Stored ITS Line Level price into NP");
                                    
                                    // Apply IL override to get WMK price in the next cycle
                                    InsertClorRecord(string.Empty, "IL", "0", "0.0000", "EXTN", AppConfig.ITSILExcdID);

                                    /* Manual Price - Put EP BWP back - Begin */
                                    //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing ", " Putting back all manual prices into EP again");
                                    _claim.claimLineOverrides.Where(cdor => cdor.CDOR_OR_ID.Equals("MP")).ToList()
                                        .ForEach(cdor => InsertCdorRecord(cdor.CLCL_ID, cdor.CDML_SEQ_NO, "EP", cdor.MEME_CK, cdor.CDOR_OR_AMT, "EXTN", "BWP"));
                                    /* Manual Price - Put EP BWP back - End */
                                }
                                //RemoveITSOverrides(); // Remove all pricing overrides (Allowable, disallow amount) obtained from HOST -- 10222016 MOVED IT DOWN BELOW  --> 10272016 - Move it at the very end 
                                InsertREPROC();
                            }

                            /* new code - 10222016 Begin */
                            // Entire host price is 0. In this case put EP on those lines where Host specifically sent a dollar 0 amount and reprocess

                            else // if (AllowableAmounts.Sum(cdor => cdor.CDOR_OR_AMT) == 0.00)
                            {
                                // For all the allowable amount for claim lines received as $0 from HOST, put the $0 back.

                                //if (AllowableAmounts.Any(cdor => !cdor.CDOR_OR_VALUE.Equals("EXTN")))
                                //{
                                ////Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " HOST SENT THE PRICE FOR THE ENTIRE CLAIM AS ZERO");

                                AllowableAmounts.Where(cdor => !cdor.CDOR_OR_VALUE.Equals("EXTN")).ToList()
                                    .ForEach(AA => InsertCdorRecord(AA.CLCL_ID, AA.CDML_SEQ_NO, "EP", AA.MEME_CK, AA.CDOR_OR_AMT, "ITS", "BNH"));

                                AllowableAmounts.ForEach(cdor => InsertCdorRecord(cdor.CLCL_ID, cdor.CDML_SEQ_NO, "NW", cdor.MEME_CK, cdor.CDOR_OR_AMT, "EXTN", ""));

                                /* For INH Issue - Begin */
                                InsertClorRecord(
                                    _claim.CLCL_ID,
                                    "IK",
                                    _claim.MEME_CK.ToString(),
                                    "0.00",
                                    //string.Empty,
                                    "EXTN",
                                    "325"
                                    );
                                /* For INH Issue - End */

                                InsertREPROC();

                                //}
                                //else
                                //{
                                //    //RemoveITSOverrides();   // --> 10272016 - Move it at the very end 
                                //    RemoveNPOverrides(); // Remove all NP override
                                //}
                                //RemoveSRandPAOverrides(); // If accepting Host price, we will not calculate Wellmark Price. Remove SR 000 and PA override which we used to bypass WMK benefit calculation
                                // ERROR ERROR ERROR ERROR --- PUT THE ORIGINAL SR VALUE HERE

                                RevertSRAndPAOverrides();
                            }

                            RemoveITSOverrides(); // --> 10272016 - Moving this to very end 

                            /* new code - end 10222016 */

                            /* 10052015 - New Code. When the HOST price is 0 then do not do anything - CHECK WITH BUSINESS - End */
                        }
                        //else if ((_claim.claimLineOverrides.Any(cdor => cdor.CDOR_OR_ID.Equals("EP") && AppConfig.ITSNPEXCDID.Contains(cdor.EXCD_ID))) // 10152016
                        else if ((!_claim.claimLineOverrides.Any(cdor => cdor.CDOR_OR_ID.Equals("EP") && AppConfig.ITSNPEXCDID.Contains(cdor.EXCD_ID)))  // 10152016
                                    && _claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IL"))
                                    && !_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IK"))
                                )
                        {
                            Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . *********** ENTERING POST PROC FOR WMK CYCLE **********");
                            // If a Manual IL exists, then exit the extension
                            if (!_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("NP")))
                            {
                                blnPriceOverride = false;
                            }
                            else
                            {

                                /* Before using the HOST price, ensure that Manual Disallow amounts are updated back into the HOST price - Begin*/

                                if (AppConfig.FacetsOnlineApps.Contains(ContextData.ContextInstance.ApplicationId))
                                {

                                    (from cdml in _claim.claimLines.Where(line => line.IsDeniedNP)
                                     join cdor in _claim.claimLineOverrides.Where(cdorline => cdorline.CDOR_OR_ID.Equals("AX")) on
                                     cdml.CDML_SEQ_NO equals cdor.CDML_SEQ_NO
                                     select cdml).ToList().ForEach
                                     (cdml =>
                                         _claim.claimLineOverrides.FirstOrDefault(host => host.CDOR_OR_ID.Equals("NP") && host.CDML_SEQ_NO.Equals(cdml.CDML_SEQ_NO) && host.EXCD_ID.Equals(AppConfig.ITSNonParNPHostLineLevelOverrideExcdID))
                                         .CDOR_OR_AMT = cdml.CDML_ALLOW
                                      );

                                    Logger.LoggerInstance.ReportMessage("*********Disall Allow Amount", " Update complete");
                                    RemoveExtnAAOverrides();
                                    RemoveExtnManualPriceEPOverrides();
                                }

                                /* Before using the HOST price, ensure that Manual Disallow amounts are updated back into the HOST price - End*/

                                /* 10152016 - Adding new logic - Force Setting NWX Price - begin */
                                if (!string.IsNullOrEmpty(strForcePricing))
                                {
                                    strOrigPriceSource = strForcePricing;
                                }
                                /*else if (_claim.IsAdjustedClaim) // Check whether the claim is an adjusted
                                {
                                    strOrigPriceSource = GetAdjustedClaimPricingMethod(); // If the claim is an adjusted claim, get the type of pricing (WMK Vs Host) that was applied to the original claim
                                }*/
                                if (strOrigPriceSource.Equals("NWX"))
                                {
                                    //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . **** ORIGINAL CLAIM WAS PRICED AT NWX. SO ACCEPT NWX PRICE AND DONT COMPARE PRICE");
                                    _claim.claimLines.Where(line => line.IsDenied.Equals(false)).ToList().
                                  ForEach
                                  (
                                      l => InsertCdorRecord(l.CLCL_ID, l.CDML_SEQ_NO, "EP", l.MEME_CK, l.CDML_ALLOW, "NWX", "BNW")
                                  );
                                    //RemoveNPOverrides(); // 10222016
                                }

                                else
                                {

                                    /* 10152016 - Adding new logic - Force Setting NWX Price - end */

                                    /* 10152016 - Adding new logic - begin */
                                    dblNWXPrice = _claim.claimLineOverrides.Where(cdor => cdor.CDOR_OR_ID.Equals("NP") && cdor.EXCD_ID.Equals(AppConfig.ITSNonParNPWMKLineLevelOverrideExcdID))
                                        .Sum(cdor => cdor.CDOR_OR_AMT); // 120

                                    dblITSPrice = _claim.claimLineOverrides.Where(cdor => cdor.CDOR_OR_ID.Equals("NP") && cdor.EXCD_ID.Equals(AppConfig.ITSNonParNPHostLineLevelOverrideExcdID))
                                        .Sum(cdor => cdor.CDOR_OR_AMT); // 124.8

                                    Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . NetworX price is  " + dblNWXPrice.ToString());
                                    Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . HOST price is  " + dblITSPrice.ToString());



                                    if (dblITSPrice <= dblNWXPrice)
                                    {
                                        Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . HOST price is  less than or equal to Wellmark Price. Accept Host Price");


                                        _claim.claimLineOverrides.Where(cdor => cdor.CDOR_OR_ID.Equals("NP")
                                            && cdor.EXCD_ID.Equals(AppConfig.ITSNonParNPHostLineLevelOverrideExcdID) && _claim.claimLines.Single(cdml => cdml.CDML_SEQ_NO.Equals(cdor.CDML_SEQ_NO)).IsDenied.Equals(false) //.CDML_ALLOW > 0.00
                                                                                                            && (cdor.CDOR_OR_AMT > 0.00 || !cdor.CDOR_OR_VALUE.Equals("EXTN"))).ToList()
                                            .ForEach
                                            (
                                            l => InsertCdorRecord(l.CLCL_ID, l.CDML_SEQ_NO, "EP", l.MEME_CK, l.CDOR_OR_AMT, "ITS", "BNH")
                                            );

                                        /* For INH Issue - Begin */
                                        InsertClorRecord(
                                            _claim.CLCL_ID,
                                            "IK",
                                            _claim.MEME_CK.ToString(),
                                            "0.00",
                                            //string.Empty,
                                            "EXTN",
                                            "325"
                                            );
                                        /* For INH Issue - End */
                                    }
                                    else
                                    {
                                        Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . Wellmark price is  less than Host Price. Accept Wellmark Price");
                                        Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " IS FULLY DENIED CLAIM " + _claim.IsDeniedClaim.ToString());

                                        if (_claim.IsDeniedClaim)
                                        {
                                            InsertCdorRecord(_claim.CLCL_ID, 1000, "EP", _claim.MEME_CK, 0.00, "NWX", "BNW");
                                        }
                                        else
                                        {
                                            _claim.claimLines.Where(line => line.IsDenied.Equals(false)).ToList(). //10222016 SHOULD WE PUT $0 PRICE INTO EP?? FOR EXAMPLE PDC GIVES $0 BUT WE MUST PUT $0 BACK TO EP.
                                            ForEach
                                             (
                                                //                        CLCL_ID,   CDML_SEQ_NO,   CDOR_OR_ID,                 MEME_CK,   CDOR_OR_AMT   CDOR_OR_VALUE   EXCD_ID
                                               l => InsertCdorRecord(l.CLCL_ID, l.CDML_SEQ_NO, "EP", l.MEME_CK, l.CDML_ALLOW, "NWX", "BNW")
                                             );

                                            /* For INH Issue - Begin */
                                            InsertClorRecord(
                                                _claim.CLCL_ID,
                                                "IK",
                                                _claim.MEME_CK.ToString(),
                                                "0.00",
                                                //string.Empty,
                                                "EXTN",
                                                "325"
                                                );
                                            /* For INH Issue - End */
                                        }

                                    } // making the new code available for everything

                                }

                                InsertMissingDFMessages(); // Include any missing DF Messages
                                #region Including DF Messages moved outside of this function

                                /* CHECK FOR THE NEED FOR MANUAL DF MESSAGE AND ATTACH TO THE PROCESS IF NEEDED - Start 

                                 Defect XXXX - Begin
                                List<XElement> elmCMDMLstForDF = null;
                                string strFacetsCode = string.Empty;
                                string strITSCode = string.Empty;
                                strQuery = "SELECT ICFI_ITS_CODE FROM CMC_ICFI_FAC_ITS WHERE ICFI_CATEGORY = 'MD' AND ICFI_FACETS_CODE = '{0}';";
                                var ElementsCheckForManualDF = (
                                                        from cdmdElement in FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDMDALL", "CDMDALL"))
                                                        join cdmlElement in _claim.claimLines.Where(line => line.IsDenied.Equals(true))
                                                        on cdmdElement.Elements().FirstOrDefault(cdmdE => cdmdE.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value equals
                                                            cdmlElement.CDML_SEQ_NO.ToString()
                                                        join cdimElement in FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDIMALL", "CDIMALL"))
                                                        .Where(cdmi => cdmi.Elements().Any(cdmielm => cdmielm.Attribute("name").Value.Equals("CDIM_TYP") && cdmielm.Value.Equals("D")))
                                                        on cdmdElement.Elements().FirstOrDefault(cdmdE => cdmdE.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value equals
                                                        cdimElement.Elements().FirstOrDefault(cdimE => cdimE.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value into SubCdmiCollection
                                                        //cdimElement.Elements().FirstOrDefault(cdimE => cdimE.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value 
                                                        //where cdimElement.Elements().FirstOrDefault(cdimE => cdimE.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value.Equals("D") select cdimElement into SubCdmiCollection
                                                        from cdmi in SubCdmiCollection.DefaultIfEmpty()
                                                        where cdmi == null
                                                        group
                                                            //cdmdElement by cdmdElement.Elements().Where(e => e.Attribute("name").Value.Equals("CDML_SEQ_NO")).FirstOrDefault().Value into cdmdGrouped
                                                        cdmdElement by cdmdElement.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value into cdmdGrouped
                                                        //where
                                                        //cdmdGrouped.Count() > 1
                                                        select new { key = cdmdGrouped.Key, Value = cdmdGrouped.ToList() })
                                                        .ToDictionary(CDML_SEQ_NO => CDML_SEQ_NO.key, Elements => Elements.Value);

                                foreach (var cdmlseqno in ElementsCheckForManualDF.Keys)
                                {
                                    elmCMDMLstForDF = ElementsCheckForManualDF[cdmlseqno];
                                    foreach (XElement elmCDMD in elmCMDMLstForDF)
                                    {
                                        strFacetsCode = elmCDMD.Elements().SingleOrDefault(elm => elm.Attribute("name").Value.Equals("EXCD_ID")).Value;
                                        strQueryResult = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(string.Format(strQuery, strFacetsCode));
                                        //Logger.LoggerInstance.ReportMessage("ElementsCheckForManualDF", " DF Code for line number " + cdmlseqno + "  For Facets Code " + strFacetsCode + " is " + strQueryResult);
                                        if (FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult))
                                        {
                                            strITSCode = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "ICFI_ITS_CODE", false);
                                            InsertCdimRecord(
                                                elmCDMD.Elements().SingleOrDefault(elm => elm.Attribute("name").Value.Equals("CLCL_ID")).Value,
                                                elmCDMD.Elements().SingleOrDefault(elm => elm.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value,
                                                strITSCode,
                                                "D",
                                                elmCDMD.Elements().SingleOrDefault(elm => elm.Attribute("name").Value.Equals("MEME_CK")).Value,
                                                "Y",
                                                "0.0000",
                                                "0.0000",
                                                "",
                                                //"EXTN"
                                                strFacetsCode
                                                );
                                            break;
                                        }
                                    }

                                    Defect XXXX - End x 
                                }

                                Check for IZ and add if any DF message is missing IZ - Begin 

                                var IZElementsRequired = (
                                    from cdimElement in FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDIMALL", "CDIMALL"))
                                    .Where(cdmi => cdmi.Elements().Any(cdmielm => cdmielm.Attribute("name").Value.Equals("CDIM_TYP") && cdmielm.Value.Equals("D"))) // For DF Message
                                    join cdorElement in FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL"))
                                    .Where(cdor => cdor.Elements().Any(cdorelm => cdorelm.Attribute("name").Value.Equals("CDOR_OR_ID") && cdorelm.Value.Equals("IZ"))) // CDOR_OR_ID for DF Message
                                    on cdimElement.Elements().FirstOrDefault(cdmdE => cdmdE.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value equals
                                        cdorElement.Elements().FirstOrDefault(cdimE => cdimE.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value into SubIzCdorCollection
                                    from cdorIZ in SubIzCdorCollection.DefaultIfEmpty()
                                    where cdorIZ == null
                                    select cdimElement).ToList();

                                IZElementsRequired.ForEach
                                    (cdimElm =>
                                        InsertCdorRecord(
                                                    cdimElm.Elements().SingleOrDefault(elm => elm.Attribute("name").Value.Equals("CLCL_ID")).Value,
                                                    int.Parse(cdimElm.Elements().SingleOrDefault(elm => elm.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value),
                                                    "IZ",
                                                    int.Parse(cdimElm.Elements().SingleOrDefault(elm => elm.Attribute("name").Value.Equals("MEME_CK")).Value),
                                                    double.Parse("0"),
                                                    string.Empty,
                                                    "314"
                                                    )
                                    );

                                /* Check for IZ and add if any DF message is missing IZ - End */
                                /* CHECK FOR THE NEED FOR MANUAL DF MESSAGE AND ATTACH TO THE PROCESS IF NEEDED - END  */
                                #endregion


                                //Logger.LoggerInstance.ReportMessage("44444444444444444444", "44444444444444444444444444444444444444444444444444444444444444444444444444");
                                //RemoveITSOverrides(); 10222016 commented as this is already taken care of
                                
                                /*List<XElement> list = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CLOR", "CLOR")).ToList();
                                foreach (XElement element in list)
                                {
                                    if (element.Elements().Where(e => e.Attribute("name").Value == "CLOR_OR_ID").FirstOrDefault().Value == "IL")
                                    {
                                        element.Remove();
                                    }
                                }*/

                                XElement clorILExtn = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CLOR", "CLOR"))
                                                .FirstOrDefault(clor => clor.Elements().Any(e => e.Attribute("name").Value.Equals("CLOR_OR_ID") && e.Value.Equals("IL")) &&
                                                                        clor.Elements().Any(e => e.Attribute("name").Value.Equals("CLOR_OR_VALUE") && e.Value.Equals("EXTN")));
                                if (clorILExtn != null)
                                    clorILExtn.Remove();

                                blnPriceOverride = true;
                                InsertREPROC();
                            }
                        }

                        //else if (!_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IK")) && !_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IL")))  // For INH Issue - Removed
                        else if (_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("NP")))  // For INH Issue - Added
                        {

                            /* For INH Issue - Begin */
                            XElement cdorIKElement = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CLOR", "CLOR"))
                                .FirstOrDefault(cdorElm => cdorElm.Elements().Any(elm => elm.Attribute("name").Value.Equals("CLOR_OR_ID") && elm.Value.Equals("IK")));
                            if (cdorIKElement != null)
                            {
                                cdorIKElement.Remove();
                            }
                            /* For INH Issue - End */

                            RemoveExtnNWOverrides(); //4517

                            #region Commenting the part of updating EXCD_ID for each claim line for now
                            /*For PDC Issue - Begin 
                            List<XElement> claimLines = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDMLALL", "CDMLALL")).ToList();
                            List<XElement> manualDFMsgs = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDIMALL", "CDIMALL"))
                                                            .Where(cdim => cdim.Elements().Any(e => e.Attribute("name").Value.Equals("CDIM_ADDL_DATA") && !string.IsNullOrEmpty(e.Value)))
                                                            .ToList();

                            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . BEFORE PUTTIN IN THE CORRECT EXCD_ID");
                            manualDFMsgs.ForEach(cdimAll => claimLines.SingleOrDefault(claimLine => claimLine.Elements().Any(
                                                                                            cdml => cdml.Attribute("name").Value.Equals("CDML_SEQ_NO") &&
                                                                                                        cdml.Value.Equals(cdimAll.Elements().SingleOrDefault(cdim => cdim.Attribute("name")
                                                                                                        .Value.Equals("CDML_SEQ_NO")).Value)))
                                                                                            .Elements().SingleOrDefault(e => e.Attribute("name").Value.Equals("CDML_DISALL_EXCD")).Value =
                                                                                            cdimAll.Elements().SingleOrDefault(c => c.Attribute("name").Value.Equals("CDIM_ADDL_DATA")).Value);

                            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " . AFTER PUTTIN IN THE CORRECT EXCE_ID");
                            For PDC Issue - End */
                            #endregion

                            /* 10222016 - new Block begin */
                            RemoveITSOverrides();
                            RemoveNPOverrides();
                            List<XElement> cdorList = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDORALL", "CDORALL")).ToList();
                            //XElement cdorToRemove = cdorList.Elements().Where(e => e.Attribute("name").Value.Equals("CDML_SEQ_NO") && e.Value.Equals(1000)).FirstOrDefault();
                            XElement cdorToRemove = cdorList.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDML_SEQ_NO") && e.Value.Equals(1000));
                            if (cdorToRemove != null)
                            {
                                cdorToRemove.Remove();
                            }

                        }
                        blnPriceOverride = true;
                    }//if (_claim.CLCL_CL_SUB_TYPE.Equals("M"))


                #endregion POSTPROC Medical Claim

                #region POSTPROC HOSPITAL Claim

                    else if (_claim.CLCL_CL_SUB_TYPE.Equals("H"))// Hospital Claim
                    {
                        blnPriceOverride = true;
                        if (!string.IsNullOrEmpty(strForcePricing)) // If we know upfront what pricing to apply, get it from the input parameter. For example, for emergency medical claim, always use HOST price and do not perform price comparision.
                        {
                            strOrigPriceSource = strForcePricing;
                        }

                        GetClhpData();
                        if (!_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IK"))
                            && !_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IL"))
                            && !_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("NP"))
                            && (!_claim.claimLineOverrides.Any(cdor => cdor.CDOR_OR_ID.Equals("EP") && AppConfig.ITSNPEXCDID.Contains(cdor.EXCD_ID))))
                        {
                            double.TryParse(AppConfig.ITSNonParFacilityPercent, out dblFacDefaultPct);
                            Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " ITSNonParFacilityPercent is : " + dblFacDefaultPct.ToString());

                            if (_claim.HospClaimData.POS_IND.Equals("I") && AppConfig.ITSNPInpHospNPPRRevertToChg.Contains(_claim.PDBC_PFX_NPPR)) // If its an inpatient hospital claim
                            {
                                Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " CLAIM POS_IND IS :::: " + _claim.HospClaimData.POS_IND);
                                Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " NPPR to revert to charge");

                                // Insert WMK Consider Charge Amount for all lines as EP BWP
                                /*_claim.claimLines.ForEach(cdml =>
                                    InsertCdorRecord(cdml.CLCL_ID, cdml.CDML_SEQ_NO, "EP", cdml.MEME_CK, cdml.CDML_CONSIDER_CHG, "NWX", "BNW")
                                    ); */

                                //InsertClorRecord(string.Empty, "IK", "0", "0.0000", string.Empty, AppConfig.ITSIKEXCDID);
                                InsertClorRecord(_claim.CLCL_ID, "IK", _claim.MEME_CK.ToString(), "0.0000", string.Empty, AppConfig.ITSIKEXCDID);
                                //_claim.claimLines.ForEach(cdml => Defect 5345: Put EP only for line that did not deny fully
                                _claim.claimLines.Where(line => !line.IsDeniedNP).ToList().ForEach(cdml => 
                                    //InsertCdorRecord(cdml.CLCL_ID, cdml.CDML_SEQ_NO, "EP", cdml.MEME_CK, cdml.CDML_CHG_AMT, "ITS", "BNH")); // Defect 5345: EXCD_ID should be BNW
                                    InsertCdorRecord(cdml.CLCL_ID, cdml.CDML_SEQ_NO, "EP", cdml.MEME_CK, cdml.CDML_CHG_AMT, "NWX", "BNW")); // Defectr 5345: Fixed

                                InsertClorRecord(_claim.CLCL_ID, "NP", "0", "", "NWX Applied", AppConfig.ITSNonParNPClaimLevelOverrideExcdID); // Defect 5345
                                RemoveITSOverrides(); // Defect 5345
                                //blnPriceOverride = true;
                                InsertMissingDFMessages(); // Defect 5345
                                InsertREPROC();
                            }
                            else
                            {
                                if (_claim.HospClaimData.POS_IND.Equals("I") && arrNpprCustomPct.Contains(_claim.PDBC_PFX_NPPR))
                                {
                                    dblFacDefaultPct = 75;
                                    Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " CLAIM POS_IND IS :::: " + _claim.HospClaimData.POS_IND);
                                    Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " Custom NPPR. dblFacDefaultPct is " + dblFacDefaultPct.ToString());
                                }
                                dblNWXPrice = _claim.claimLines.Sum(line => line.CDML_CHG_AMT); // SUM OF CHARGE AMOUNT


                                if (strOrigPriceSource.Equals("NWX") ||
                                    (_claim.HospClaimData.POS_IND.Equals("I") && arrNpprCustomPct.Contains(_claim.PDBC_PFX_NPPR))
                                    )
                                {
                                    //blnPriceOverride = true;
                                    Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " Adjusted Claim. Original Pricing method is NWX. Applied " + dblFacDefaultPct + " percentage and EXIT");
                                    Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + dblFacDefaultPct.ToString() + " percentage of charge applied is " + (_claim.claimLines.Sum(c => c.CDML_CHG_AMT) * (dblFacDefaultPct / 100)).ToString());

                                    //_claim.claimLines. Defect 3980
                                    _claim.claimLines.Where(line => line.IsDeniedNP.Equals(false)).ToList().
                                    //_claim.claimLines.Where(line => line.IsDeniedNP.Equals(false) && _claim.claimLineOverrides.Where(cdor => cdor.CDML_SEQ_NO.Equals(line.CDML_SEQ_NO) && cdor.CDOR_OR_ID.Equals("AX")).Sum(cdorAmt => cdorAmt.CDOR_OR_AMT) != line.CDML_CHG_AMT).ToList().
                                    ForEach
                                    (

                                        l => InsertCdorRecord(l.CLCL_ID, l.CDML_SEQ_NO, "EP", l.MEME_CK, (l.CDML_CHG_AMT * (dblFacDefaultPct / 100)), "NWX", "BNW")
                                    );
                                    InsertClorRecord(_claim.CLCL_ID, "NP", "0", "", "NWX Applied", AppConfig.ITSNonParNPClaimLevelOverrideExcdID);
                                    Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + "*** The following Wellmark price were applied***");
                                    _claim.claimLines.Where(cdml => !cdml.IsDeniedNP).ToList().ForEach(cdml => Logger.LoggerInstance.ReportMessage("Applied WMK price for Line " + cdml.CDML_SEQ_NO.ToString() + " is ===>>> ", Math.Round(cdml.CDML_CHG_AMT * (dblFacDefaultPct / 100), 2).ToString())); // THIS IS FOR LOGGING REMOVE IT
                                    // CALL A FUNCTION TO PUT IN NECESSARY DF
                                    RemoveITSOverrides(); // Defect 5345
                                    InsertMissingDFMessages();
                                    InsertREPROC();
                                }
                                //else if(!strOrigPriceSource.Equals("ITS"))

                                //else if (strOrigPriceSource.Equals(string.Empty) && !_claim.claimLineOverrides.Any(cdor => cdor.CDOR_OR_ID.Equals("NP"))) // Commented
                                else if (!_claim.claimLineOverrides.Any(cdor => cdor.CDOR_OR_ID.Equals("NP"))) // added
                                {
                                    //RevertSRAndPAOverrides(); Commented
                                    


                                    var AllowableAmounts = (from cdml in _claim.claimLines
                                                            join cdor in _claim.claimLineOverrides.
                                                            Where(cdor => cdor.CDOR_OR_ID.Equals("AA")) on cdml.CDML_SEQ_NO equals cdor.CDML_SEQ_NO into subCodrCollections
                                                            from subcdor in subCodrCollections.DefaultIfEmpty((new Cdor()
                                                            {
                                                                CLCL_ID = cdml.CLCL_ID,
                                                                MEME_CK = cdml.MEME_CK,
                                                                CDML_SEQ_NO = cdml.CDML_SEQ_NO,
                                                                CDOR_OR_AMT = cdml.CDML_CHG_AMT - cdml.CDML_DISALL_AMT,
                                                                CDOR_OR_VALUE = "EXTN", // this means that host did not send this value 
                                                                CDOR_OR_ID = "AA"
                                                            }))
                                                            select subcdor).ToList();



                                    Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " Hospital claim: *** Below are the HOST price - HOST Price is either zero or Forcefully applying Host Price");
                                    AllowableAmounts.ForEach(cdor => Logger.LoggerInstance.ReportMessage("CDOR VALUE", "SEQ " + cdor.CDML_SEQ_NO + " _ ID " + cdor.CDOR_OR_ID + " _ AMT " + cdor.CDOR_OR_AMT.ToString() + " - EXCD " + cdor.EXCD_ID + " _ VAL " + cdor.CDOR_OR_VALUE));

                                    //if (AllowableAmounts.Sum(cdor => cdor.CDOR_OR_AMT) == 0.00)
                                    if (AllowableAmounts.Sum(cdor => cdor.CDOR_OR_AMT) == 0.00 || strOrigPriceSource.Equals("ITS"))
                                    {

                                        /* remove for logging - begin */
                                        _claim.claimLineOverrides.ForEach(cdor =>
                                            Logger.LoggerInstance.ReportMessage("Claim Line Override ===>>> ", "  For Line " + cdor.CDML_SEQ_NO.ToString() + " - For ID " + cdor.CDOR_OR_ID + " - With EXCD_ID of " + cdor.EXCD_ID + " - IS - " + cdor.CDOR_OR_AMT.ToString()));
                                        /* remove for logging - end */
                                            


                                        Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " Hospital claim: *** HOST SENT ALL ITS LINES AS ZERO***");
                                        AllowableAmounts.Where(cdor => !cdor.CDOR_OR_VALUE.Equals("EXTN")).ToList()
                                       .ForEach(AA => InsertCdorRecord(AA.CLCL_ID, AA.CDML_SEQ_NO, "EP", AA.MEME_CK, AA.CDOR_OR_AMT, "ITS", "BNH"));

                                        /* For INH Issue - Begin */
                                        InsertClorRecord(
                                            _claim.CLCL_ID,
                                            "IK",
                                            _claim.MEME_CK.ToString(),
                                            "0.00",
                                            //string.Empty,
                                            "EXTN",
                                            "325"
                                            );
                                        /* For INH Issue - End */

                                        //if (string.IsNullOrEmpty(strOrigPriceSource)) // When original price was null then we would have processed this claim through SR 000. Revert it back
                                          //  RevertSRAndPAOverrides();

                                        
                                        InsertMissingDFMessages(); // Do we need this? For now add it - 03/09/2017
                                        Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " Hospital claim: *** PUTTING DF MESSAGES BY THE EXTENSION");

                                        InsertREPROC();
                                        InsertClorRecord(_claim.CLCL_ID, "NP", "0", "", "ITS Applied", AppConfig.ITSNonParNPClaimLevelOverrideExcdID);

                                        
                                    }
                                    else
                                    {
                                        //RevertSRAndPAOverrides();

                                        Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " Hospital claim: *** Below are the HOST price");
                                        AllowableAmounts.ForEach(cdor => Logger.LoggerInstance.ReportMessage("CDOR VALUE", "SEQ " + cdor.CDML_SEQ_NO + " _ ID " + cdor.CDOR_OR_ID + " _ AMT " + cdor.CDOR_OR_AMT.ToString() + " - EXCD " + cdor.EXCD_ID + " _ VAL " + cdor.CDOR_OR_VALUE));

                                        AllowableAmounts.ForEach(l => InsertCdorRecord(l.CLCL_ID, l.CDML_SEQ_NO, "NP", l.MEME_CK, l.CDOR_OR_AMT, l.CDOR_OR_VALUE, AppConfig.ITSNonParNPHostLineLevelOverrideExcdID)); // 10222016
                                        Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " Hospital claim: Stored ITS Line Level price into NP");
                                        InsertREPROC();
                                    }
                                }
                                else
                                {
                                    dblITSPrice = _claim.claimLineOverrides.Where(cdor => cdor.CDOR_OR_ID.Equals("NP") && cdor.EXCD_ID.Equals(AppConfig.ITSNonParNPHostLineLevelOverrideExcdID)).Sum(cdorNP => cdorNP.CDOR_OR_AMT);
                                    Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " HOSPITAL CLAIM : Calculated ITS Price. dblITSPrice = " + dblITSPrice.ToString());

                                    dblNWXPrice = _claim.claimLines.Sum(cdml => cdml.CDML_CHG_AMT);
                                    Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "HOSPITAL CLAIM :  Original dblNWXPrice is : " + dblNWXPrice.ToString());
                                    dblNWXPrice = Math.Round(dblNWXPrice * (dblFacDefaultPct / 100), 2);
                                    Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "HOSPITAL CLAIM :  After 80 % dblNWXPrice is : " + dblNWXPrice.ToString());

                                    Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + "*** The following ARE THE Wellmark price ***");
                                    _claim.claimLines.Where(cdml => !cdml.IsDeniedNP).ToList().ForEach(cdml => Logger.LoggerInstance.ReportMessage("Applied WMK price for Line " + cdml.CDML_SEQ_NO.ToString() + " is ===>>> ", Math.Round(cdml.CDML_CHG_AMT * (dblFacDefaultPct / 100), 2).ToString())); // THIS IS FOR LOGGING REMOVE IT

                                    if (dblITSPrice <= dblNWXPrice)
                                    {

                                        _claim.claimLineOverrides.Where(cdor => cdor.CDOR_OR_ID.Equals("NP") && cdor.EXCD_ID.Equals(AppConfig.ITSNonParNPHostLineLevelOverrideExcdID))// && !cdor.CDOR_OR_VALUE.Equals("EXTN"))
                                            .Where(cdorNP => !_claim.claimLines.Any(cdml => cdml.CDML_SEQ_NO.Equals(cdorNP.CDML_SEQ_NO) && cdml.IsDeniedNP
                                                /*(cdml.IsDeniedNP ||
                                                 _claim.claimLineOverrides.Where(cdor => cdor.CDML_SEQ_NO.Equals(cdml.CDML_SEQ_NO) && cdor.CDOR_OR_ID.Equals("AX")).Sum(cdorAmt => cdorAmt.CDOR_OR_AMT) == cdml.CDML_CHG_AMT
                                                )*/
                                                ))// && cdorNP.CDOR_OR_AMT > 0.00))
                                            .ToList()
                                            .ForEach(HostPrice =>
                                                        this.InsertCdorRecord(
                                                            HostPrice.CLCL_ID,
                                                            HostPrice.CDML_SEQ_NO,
                                                            "EP",
                                                            HostPrice.MEME_CK,
                                                            HostPrice.CDOR_OR_AMT,
                                                            "ITS",
                                                            "BNH"
                                                            ));

                                        

                                        InsertClorRecord(_claim.CLCL_ID, "NP", "0", "", "ITS Applied", AppConfig.ITSNonParNPClaimLevelOverrideExcdID);
                                    }
                                    else
                                    {

                                        _claim.claimLines.Where(cdml => !cdml.IsDeniedNP).ToList().
                                            ForEach(WmkPrice =>
                                                        this.InsertCdorRecord(
                                                            WmkPrice.CLCL_ID,
                                                            WmkPrice.CDML_SEQ_NO,
                                                            "EP",
                                                            WmkPrice.MEME_CK,
                                                            Math.Round(WmkPrice.CDML_CHG_AMT * (dblFacDefaultPct / 100), 2),
                                                            "NWX",
                                                            "BNW"
                                                            ));

                                        /*
                                        _claim.claimLineOverrides.Where(cdor => cdor.CDOR_OR_ID.Equals("NP") && cdor.EXCD_ID.Equals(AppConfig.ITSNonParNPHostLineLevelOverrideExcdID))
                                            .Where(cdorNP => !_claim.claimLines.Any(cdml => cdml.CDML_SEQ_NO.Equals(cdorNP.CDML_SEQ_NO) && cdml.IsDeniedNP
                                                ))
                                            .ToList()
                                            .ForEach(HostPrice =>
                                                        this.InsertCdorRecord(
                                                            HostPrice.CLCL_ID,
                                                            HostPrice.CDML_SEQ_NO,
                                                            "EP",
                                                            HostPrice.MEME_CK,
                                                            //HostPrice.CDOR_OR_AMT,
                                                            Math.Round(_claim.claimLines.Single(cdml => cdml.CDML_SEQ_NO.Equals(HostPrice.CDML_SEQ_NO)).CDML_CHG_AMT * (dblFacDefaultPct / 100), 2),
                                                            "NWX",
                                                            "BNW"
                                                            ));
                                        */
                                        InsertMissingDFMessages(); // Applying this only when WMK price is accepted.

                                        
                                            


                                        InsertClorRecord(_claim.CLCL_ID, "NP", "0", "", "WMK Applied", AppConfig.ITSNonParNPClaimLevelOverrideExcdID);
                                    }

                                    //For INH Issue - Begin 
                                    InsertClorRecord(
                                        _claim.CLCL_ID,
                                        "IK",
                                        _claim.MEME_CK.ToString(),
                                        "0.00",
                                        //string.Empty,
                                        "EXTN",
                                        "325"
                                        );
                                    //For INH Issue - End 
                                    //InsertMissingDFMessages();  Applying this only when WMK price is accepted
                                    InsertREPROC();

                                    #region OLD COMMENTED CODE
                                    //}
                                    //        if(!_claim.claimLineOverrides.Any(cdor => cdor.CDOR_OR_ID.Equals("NP")))
                                    //        {

                                    //        }
                                    //        else
                                    //        {

                                    //        }
                                    //        dblITSPrice = AllowableAmounts.Sum(AA => AA.CDOR_OR_AMT);
                                    //        //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " Calculated ITS Price. dblITSPrice = " + dblITSPrice.ToString());
                                    //        //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "Original dblNWXPrice is : " + dblNWXPrice.ToString());
                                    //        dblNWXPrice = Math.Round(dblNWXPrice * (dblFacDefaultPct / 100),2);
                                    //        //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "After 80 % dblNWXPrice is : " + dblNWXPrice.ToString());



                                    //    }

                                    //    if (AllowableAmounts.Sum(cdor => cdor.CDOR_OR_AMT) > 0.00)
                                    //    {

                                    //        /*
                                    //        if (strOrigPriceSource.Equals("ITS"))
                                    //        {
                                    //            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + "Original claim priced at ITS. Use ITS Price");
                                    //            blnPriceOverride = true;

                                    //            RemoveITSOverrides();

                                    //            AllowableAmounts.Where(a => a.CDOR_OR_AMT > 0.00 || !a.CDOR_OR_VALUE.Equals("EXTN")).ToList()
                                    //            .ForEach(AA => InsertCdorRecord(AA.CLCL_ID, AA.CDML_SEQ_NO, "EP", AA.MEME_CK, AA.CDOR_OR_AMT, "ITS", "BNH"));       // 10222016

                                    //            if (AllowableAmounts.Any(aa => aa.CDOR_OR_AMT > 0.00 || !aa.CDOR_OR_VALUE.Equals("EXTN")))
                                    //            {
                                    //                 For INH Issue - Begin 
                                    //                InsertClorRecord(
                                    //                    _claim.CLCL_ID,
                                    //                    "IK",
                                    //                    _claim.MEME_CK.ToString(),
                                    //                    "0.00",
                                    //                    //string.Empty,
                                    //                    "EXTN",
                                    //                    "325"
                                    //                    );
                                    //                 For INH Issue - End 
                                    //            }


                                    //            InsertClorRecord(_claim.CLCL_ID, "NP", "0", "", "ITS Applied", AppConfig.ITSNonParNPClaimLevelOverrideExcdID);
                                    //            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " Applied ITS Price");
                                    //            InsertREPROC();
                                    //        }*/

                                    //        else
                                    //        {
                                    //            dblITSPrice = AllowableAmounts.Sum(AA => AA.CDOR_OR_AMT);
                                    //            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " Calculated ITS Price. dblITSPrice = " + dblITSPrice.ToString());

                                    //            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "Original dblNWXPrice is : " + dblNWXPrice.ToString());


                                    //            dblNWXPrice = Math.Round(dblNWXPrice * (dblFacDefaultPct / 100),2);
                                    //            //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "After 80 % dblNWXPrice is : " + dblNWXPrice.ToString());



                                    //            if (dblITSPrice > dblNWXPrice)
                                    //            {
                                    //                //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + "dblITSPrice > dblNWXPrice");
                                    //                blnPriceOverride = true;

                                    //                RemoveITSOverrides();

                                    //                // Override all lines with the derived CDOR value.
                                    //                //_claim.claimLines. Defect 3980
                                    //                _claim.claimLines.Where(line => line.IsDenied.Equals(false)).ToList().
                                    //                    ForEach
                                    //                    (
                                    //                    //                          CLCL_ID, CDML_SEQ_NO,   CDOR_OR_ID,                MEME_CK,   CDOR_OR_AMT   CDOR_OR_VALUE   EXCD_ID
                                    //                        l => InsertCdorRecord(l.CLCL_ID, l.CDML_SEQ_NO, "EP", l.MEME_CK, (Math.Round(l.CDML_CHG_AMT * (dblFacDefaultPct / 100),2)), "NWX", "BNW")
                                    //                    );
                                    //                InsertClorRecord(_claim.CLCL_ID, "NP", "0", "", "NWX Applied", AppConfig.ITSNonParNPClaimLevelOverrideExcdID);
                                    //                //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " Applied WMK Price");

                                    //                /* For INH Issue - Begin */
                                    //                InsertClorRecord(
                                    //                    _claim.CLCL_ID,
                                    //                    "IK",
                                    //                    _claim.MEME_CK.ToString(),
                                    //                    "0.00",
                                    //                    string.Empty,
                                    //                    "325"
                                    //                    );
                                    //                /* For INH Issue - End */


                                    //                InsertREPROC();
                                    //            }
                                    //            else
                                    //            {
                                    //                //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + "Original claim priced at ITS. Use ITS Price");
                                    //                blnPriceOverride = true;

                                    //                RemoveITSOverrides();

                                    //                // Override all lines with the derived CDOR value.
                                    //                //_claim.claimLines. Defect 3980
                                    //                _claim.claimLines.Where(line => line.IsDenied.Equals(false)).ToList().
                                    //                    ForEach
                                    //                    (
                                    //                    //                          CLCL_ID, CDML_SEQ_NO,   CDOR_OR_ID,                MEME_CK,   CDOR_OR_AMT   CDOR_OR_VALUE   EXCD_ID
                                    //                        l => InsertCdorRecord(l.CLCL_ID, l.CDML_SEQ_NO, "EP", l.MEME_CK, AllowableAmounts.SingleOrDefault(cdor => cdor.CDML_SEQ_NO.Equals(l.CDML_SEQ_NO) && cdor.CDOR_OR_ID.Equals("AA")).CDOR_OR_AMT, "ITS", "BNH")
                                    //                    );
                                    //                InsertClorRecord(_claim.CLCL_ID, "NP", "0", "", "ITS Applied", AppConfig.ITSNonParNPClaimLevelOverrideExcdID);
                                    //                //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " Applied ITS Price");

                                    //                /* For INH Issue - Begin */
                                    //                InsertClorRecord(
                                    //                    _claim.CLCL_ID,
                                    //                    "IK",
                                    //                    _claim.MEME_CK.ToString(),
                                    //                    "0.00",
                                    //                    //string.Empty,
                                    //                    "EXTN",
                                    //                    "325"
                                    //                    );
                                    //                /* For INH Issue - End */

                                    //                InsertREPROC();
                                    //            }
                                    //        }
                                    //    }

                                    //    // If ITS send us $0
                                    //    else
                                    //    {
                                    //        RemoveSRandPAOverrides();
                                    //        blnPriceOverride = true;
                                    //        //Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "At " + ContextData.ContextInstance.ExitTiming + " HOST EXPLICTLY SENT ALL CLAIMS AS $0");
                                    //    }
                                    //}
                                    #endregion OLD COMMENTED CODE
                                }

                                RemoveITSOverrides(); // remove all ITS overrides
                                Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "HOSPITAL CLAIM :  *** REMOVED ITS OVERRIDES***");

                            }
                        }

                        //else
                        //else if (!_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IK")) && !_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("IL"))) // Defect 3747
                        else if (_claim.claimOverrides.Any(clor => clor.CLOR_OR_ID.Equals("NP")))  // For INH Issue - Added
                        {
                            //Logger.LoggerInstance.ReportMessage("5555555555555555555555555", "55555555555555555555555555555555555555555555555555555555555555555555555");

                            /* For INH Issue - Begin */
                            XElement cdorIKElement = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CLOR", "CLOR"))
                                .FirstOrDefault(cdorElm => cdorElm.Elements().Any(elm => elm.Attribute("name").Value.Equals("CLOR_OR_ID") && elm.Value.Equals("IK")) &&
                                                           cdorElm.Elements().Any(elm => elm.Attribute("name").Value.Equals("CLOR_OR_VALUE") && elm.Value.Equals("EXTN")) // Adding this for defect 5345
                                );
                            if (cdorIKElement != null)
                            {
                                cdorIKElement.Remove();
                            }
                            /* For INH Issue - End */

                            RemoveITSOverrides();

                            // REMOVE NP OVERRIDES AT PRESAVE
                            found = false;

                            XElement clorNPElement = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CLOR", "CLOR"))
                                .FirstOrDefault(cdorElm => cdorElm.Elements().Any(elm => elm.Attribute("name").Value.Equals("CLOR_OR_ID") && elm.Value.Equals("NP")));

                            if (clorNPElement != null)
                            {
                                clorNPElement.Remove();
                            }

                            RemoveClaimLineOverrides(new string[] { "NP" });
                            
                            /*
                            List<XElement> clorList = FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CLOR", "CLOR")).ToList();
                            foreach (XElement clorElement in clorList)
                            {
                                found = clorElement.Elements().Any(e => e.Attribute("name").Value.Equals("CLOR_OR_ID") && e.Value.Equals("NP"));
                                if (found)
                                    clorElement.Remove();
                            }
                            */

                            blnPriceOverride = true;

                        }

                    } // else if(_claim.CLCL_CL_SUB_TYPE.Equals("H"))

                } // End of PostProc

                        #endregion POSTPROC Hospital Claim

                


            } // End of Try
            catch (Exception ex)
            {
                Logger.LoggerInstance.ReportMessage("ProcessITSNonParPricing", "Inside EXCEPTION : MESSAGE IS :" + ex.Message);
            }
            return blnPriceOverride; // Set Price Override
        }

        /// <summary>
        /// Returns the access level (Read Only / ReadWrite) of the user for the Facets application user is working on
        /// </summary>
        /// <param name="strPzpzID">Application Product ID</param>
        /// <param name="strUserID">User ID</param>
        /// <returns></returns>
        //public string GetUserAccessLvl(string strPzpzID, string strUserID)
        public string GetUserAccessLvl(string strPzapAppID, string strPzpzID, string strUserID)
        {
            XA9DataLayer dataLayer = null;
            string strQuery = string.Empty;
            try
            {
                dataLayer = new XA9DataLayer();
                //strQuery = dataLayer.GetUserAccessLevel(strUserID,strPzpzID,Environment.MachineName, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM);
                strQuery = dataLayer.GetUserAccessLevel(strPzapAppID,strUserID, strPzpzID,ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM);
            }
            catch (Exception ex)
            {

            }
            return strQuery;
        }

       /// <summary>
       /// Returns the stop loss value for the claim
       /// </summary>
       /// <param name="strClaimID">Claim ID</param>
       /// <returns>Facets DataSet containing Stop Loss value</returns>
        public string GetStopLossValueForClaim(string strClaimID)
        {
            XA9DataLayer dataLayer = null;
            string strQuery = string.Empty;
            
            try
            {
                dataLayer = new XA9DataLayer();
                strQuery = dataLayer.GetStopLossForClaim(strClaimID, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM);
            }
            catch (Exception ex)
            {

            }
            return strQuery;
        }

        /// <summary>
        /// Invokes SP wmkp_get_bdc_code to get BDC PGM ID, SUB PGM ID and BDC Flag for the claim.
        /// </summary>
        /// <param name="strClaimID">Claim ID</param>
        /// <returns></returns>
        public string GetBdcCodeForClaim(string strClaimID)
        {
            XA9DataLayer dataLayer = null;
            string strBdcForClaim = string.Empty;
            string strQueryResult = string.Empty;
            string strQuery = string.Empty;

            string strBdcPgmID = string.Empty;
            string strBdcSubPgmID = string.Empty;
            string strBdcFlag = string.Empty;
            string strBdcName = string.Empty;
            string strCreateDate = string.Empty;

            try
            {
                dataLayer = new XA9DataLayer();
                //strQuery  = dataLayer.GetBdcCodeForClaim(strClaimID,ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM, out strQueryResult);
                strQuery = dataLayer.GetBdcCodeForClaim(strClaimID, ContextData.ContextInstance.DatabaseId + XA9Constants.PFX_CUSTOM);
                

            }
            catch (Exception ex)
            {
                strBdcForClaim = "";
            }
            //return strBdcForClaim;
            return strQuery;
        }

        
    }
}







