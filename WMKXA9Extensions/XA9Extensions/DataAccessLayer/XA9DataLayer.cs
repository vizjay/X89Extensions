using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XA9Extensions.BusinessLayer;
using XA9Extensions.Utilities;

namespace XA9Extensions.DataAccessLayer
{
    public class XA9DataLayer
    {

        /// <summary>
        /// Returns Once in a LF Procedure Code and its Family from the list of procedure codes passed to the stored procedure.
        /// </summary>
        /// <param name="_AccessLevel"></param>
        /// <param name="_PZAP_ID"></param>
        //public string GetAuthProcCodesForBDC(string strBdcPgmID, string strBdcSubPgmID, string strAggregatedProcCodes, string strClaimLowSvcDT, string strPlaceOfSvcInd, string strDBPrefix, out string strDataOutput)
        public string GetAuthProcCodesForBDC(string strAggregatedProcCodes, string strClaimLowSvcDT, bool isBDCPlus, string strDBPrefix, out string strDataOutput)
        {
            string strQuery = string.Empty;
            StringBuilder SqlGetCommand = new StringBuilder();
            strDataOutput = string.Empty;
            try
            {
                SqlGetCommand.Append(string.Format(XA9Constants.SP_GET_BDC_PROC_CODES_FOR_AUTH_, strDBPrefix));
                /*
                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.PARM_BDC_PGM_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strBdcPgmID);
                SqlGetCommand.Append(XA9Constants.STR_APOST);

                SqlGetCommand.Append(XA9Constants.CHR_COMMA);
                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.PARM_BDC_SUB_PGM_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strBdcSubPgmID);
                SqlGetCommand.Append(XA9Constants.STR_APOST);

                SqlGetCommand.Append(XA9Constants.CHR_COMMA);
                */

                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.PARM_IPCD_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(strAggregatedProcCodes);

                SqlGetCommand.Append(XA9Constants.CHR_COMMA);
                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.PARM_CLCL_LOW_SVC_DT);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strClaimLowSvcDT);
                SqlGetCommand.Append(XA9Constants.STR_APOST);

                SqlGetCommand.Append(XA9Constants.CHR_COMMA);
                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.PARM_BDC_PLUS_IND);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                if(isBDCPlus)
                    SqlGetCommand.Append("Y");
                else
                    SqlGetCommand.Append("N");
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(XA9Constants.SEMICOLON);
                strQuery = SqlGetCommand.ToString();
                strDataOutput = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(SqlGetCommand.ToString());
            }
            catch (Exception ex)
            {

            }
            finally
            {
                SqlGetCommand.Clear(); ;
            }
            return strQuery;
        }

       /// <summary>
        /// Returns Related Services Data for the combination of Service ID and Service Rule
       /// </summary>
       /// <param name="strSeseIDs">Service IDs separated by comma</param>
       /// <param name="strSeseRule">Service Rule separated by comma</param>
       /// <param name="strDBPrefix">Custom DB prefix based on the region</param>
       /// <param name="strDataOutput">Output XML</param>
       /// <returns></returns>
        public string GetRelatedServicesDataForServiceIDAndRule(string strSeseIDs, string strSeseRule, string strDBPrefix, out string strDataOutput)
        {
            string strQuery = string.Empty;
            StringBuilder SqlGetCommand = new StringBuilder();
            strDataOutput = string.Empty;
            try
            {
                SqlGetCommand.Append(string.Format(XA9Constants.SP_GET_GET_REL_SVC_DATA, strDBPrefix));
                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.PARM_SESE_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(strSeseIDs);

                SqlGetCommand.Append(XA9Constants.CHR_COMMA);
                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.PARM_SESE_RULE);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(strSeseRule);

                SqlGetCommand.Append(XA9Constants.SEMICOLON);
                strQuery = SqlGetCommand.ToString();
                strDataOutput = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(SqlGetCommand.ToString());
            }
            catch (Exception ex)
            {

            }
            finally
            {
                SqlGetCommand.Clear(); ;
            }
            return strQuery;
        }

        /// <summary>
        /// Returns Once in a LF Procedure Code and its Family from the list of procedure codes passed to the stored procedure.
        /// </summary>
        /// <param name="_AccessLevel"></param>
        /// <param name="_PZAP_ID"></param>
        public string GetHistoryClaimsForWMKMembers(string strWmkMemberIDs,string strUniqueFamily, string strDBPrefix, out string strDataOutput)
        {
            string strQuery = string.Empty;
            StringBuilder SqlGetCommand = new StringBuilder();
            strDataOutput = string.Empty;
            try
            {
                SqlGetCommand.Append(string.Format(XA9Constants.SP_GET_ONCE_IN_A_LFTM_HIST, strDBPrefix));
                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.PARM_MEME_CK);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(strWmkMemberIDs);

                SqlGetCommand.Append(XA9Constants.CHR_COMMA);
                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.PARM_PROC_FAMILY);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(strUniqueFamily);

                SqlGetCommand.Append(XA9Constants.SEMICOLON);
                strQuery = SqlGetCommand.ToString();
                strDataOutput = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(SqlGetCommand.ToString());
            }
            catch (Exception ex)
            {

            }
            finally
            {
                SqlGetCommand.Clear(); ;
            }
            return strQuery;
        }

        /// <summary>
        /// Returns Once in a LF Procedure Code and its Family from the list of procedure codes passed to the stored procedure.
        /// </summary>
        /// <param name="_AccessLevel"></param>
        /// <param name="_PZAP_ID"></param>
        public string GetMemberCorrespondingWMKIDs(int intMemeCkFromClaim, string strDBPrefix, out string strDataOutput)
        {
            string strQuery = string.Empty;
            StringBuilder SqlGetCommand = new StringBuilder();
            strDataOutput = string.Empty;
            try
            {
                SqlGetCommand.Append(string.Format(XA9Constants.SP_GET_MEME_CORRSP_WMK_MEME_CK, strDBPrefix));
                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.PARM_MEME_CK);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(intMemeCkFromClaim);

                SqlGetCommand.Append(XA9Constants.SEMICOLON);
                strQuery = SqlGetCommand.ToString();
                strDataOutput = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(SqlGetCommand.ToString());
            }
            catch (Exception ex)
            {

            }
            finally
            {
                SqlGetCommand.Clear(); ;
            }
            return strQuery;
        }

        /// <summary>
        /// Returns Once in a LF Procedure Code and its Family from the list of procedure codes passed to the stored procedure.
        /// </summary>
        /// <param name="_AccessLevel"></param>
        /// <param name="_PZAP_ID"></param>
        public string GetOnceInALFProcCodesFromTheList(string strProcCodes, string strDBPrefix, out string strDataOutput)
        {
            string strQuery = string.Empty;
            StringBuilder SqlGetCommand = new StringBuilder();
            strDataOutput = string.Empty;
            try
            {
                SqlGetCommand.Append(string.Format(XA9Constants.SP_GET_LFTM_PROC_CODE_FROM_LIST, strDBPrefix)); 
                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.DME_PROC_CODES);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(strProcCodes);

                SqlGetCommand.Append(XA9Constants.SEMICOLON);
                strQuery = SqlGetCommand.ToString();
                strDataOutput = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(SqlGetCommand.ToString());
            }
            catch (Exception ex)
            {

            }
            finally
            {
                SqlGetCommand.Clear(); ;
            }
            return strQuery;
        }

        /// <summary>
        /// Returns DME Procedure Codes from the list of procedure codes passed to the stored procedure.
        /// </summary>
        /// <param name="_AccessLevel"></param>
        /// <param name="_PZAP_ID"></param>
        public string GetDMEProcCodesFromTheList(string strProcCodes, string strDBPrefix, out string strDataOutput)
        {
            string strQuery = string.Empty;
            StringBuilder SqlGetCommand = new StringBuilder();
            strDataOutput = string.Empty;
            try
            {
                SqlGetCommand.Append(string.Format(XA9Constants.SP_GET_DME_PROC_CODE_FROM_LIST, strDBPrefix)); // Viswan - Get the custom db name from the variable
                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.DME_PROC_CODES);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(strProcCodes);
                
                SqlGetCommand.Append(XA9Constants.SEMICOLON);
                strQuery = SqlGetCommand.ToString();
                strDataOutput = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(SqlGetCommand.ToString());
            }
            catch (Exception ex)
            {

            }
            finally
            {
                SqlGetCommand.Clear(); ;
            }
            return strQuery;
        }

        /// <summary>
        /// Gets the Access level for the current user for the application opened.
        /// </summary>
        /// <param name="_AccessLevel"></param>
        /// <param name="_PZAP_ID"></param>
        //public string GetUserAccessLevel(string strUserID, string strPzpzID, string strHostMachine, string strDBPrefix)
        public string GetUserAccessLevel(string strPzapAppID, string strUserID, string strPzpzID, string strDBPrefix)
        {
            string strQuery = string.Empty;
            StringBuilder SqlGetCommand = new StringBuilder();

            try
            {
                SqlGetCommand.Append(string.Format(XA9Constants.SP_GET_USR_ACCESS_LVL, strDBPrefix)); // Viswan - Get the custom db name from the variable
                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);

                SqlGetCommand.Append(XA9Constants.PARM_PZAP_APP_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strPzapAppID);
                SqlGetCommand.Append(XA9Constants.STR_APOST);

                SqlGetCommand.Append(XA9Constants.CHR_COMMA);
                SqlGetCommand.Append(XA9Constants.PARM_ACCESS_USUS_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strUserID);
                SqlGetCommand.Append(XA9Constants.STR_APOST);

                SqlGetCommand.Append(XA9Constants.CHR_COMMA);
                SqlGetCommand.Append(XA9Constants.PARM_ACCESS_PZPZ_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strPzpzID);
                SqlGetCommand.Append(XA9Constants.STR_APOST);

                /*
                SqlGetCommand.Append(XA9Constants.CHR_COMMA);
                SqlGetCommand.Append(XA9Constants.PARM_HOST_MACHINE);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strHostMachine);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                */

                SqlGetCommand.Append(XA9Constants.SEMICOLON);
                strQuery = SqlGetCommand.ToString();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                SqlGetCommand.Clear(); ;
            }
            return strQuery;
        }

        /// <summary>
        /// Get Related Service ID for Service ID and Service Rule.
        /// </summary>
        /// <param name="strCLCL_ID">Claim ID</param>
        /// <returns type="string">Record set xml string</returns>
        //public string GetBdcCodeForClaim(string strCLCL_ID, string strDBPrefix,out string strDataOutput)
        public string GetNpprPfxForProduct(string strPDPD_ID, string dtCLCL_LOW_SVC_DT, string strDBPrefix, out string strDataOutput)
        {
            string strQuery = string.Empty;
            StringBuilder SqlGetCommand = new StringBuilder();
            strDataOutput = "";

            try
            {
                SqlGetCommand.Append(string.Format(XA9Constants.SP_NPPR_PFX_FOR_PDPD, strDBPrefix)); // Viswan - Get the custom db name from the variable
                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.PARM_PDPD_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strPDPD_ID);
                SqlGetCommand.Append(XA9Constants.STR_APOST);

                SqlGetCommand.Append(XA9Constants.CHR_COMMA);
                SqlGetCommand.Append(XA9Constants.PARM_CLCL_LOW_SVC_DT);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(dtCLCL_LOW_SVC_DT);
                SqlGetCommand.Append(XA9Constants.STR_APOST);

                SqlGetCommand.Append(XA9Constants.SEMICOLON);
                strQuery = SqlGetCommand.ToString();
                strDataOutput = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(SqlGetCommand.ToString());
            }
            catch (Exception ex)
            {
                //ObjWmkLib.errorLogEntry(ex.ToString());
            }
            finally
            {
                SqlGetCommand.Clear(); ;
            }
            return strQuery;

        }

        /// <summary>
        /// Get Related Service ID for Service ID and Service Rule.
        /// </summary>
        /// <param name="strCLCL_ID">Claim ID</param>
        /// <returns type="string">Record set xml string</returns>
        //public string GetBdcCodeForClaim(string strCLCL_ID, string strDBPrefix,out string strDataOutput)
        public string GetBdcCodeForClaim(string strCLCL_ID, string strDBPrefix)
        {
            string strQuery = string.Empty;
            StringBuilder SqlGetCommand = new StringBuilder();
            //strDataOutput = "";

            try
            {
                //SqlGetCommand.Append(string.Format(ClsConstants.strSP_wmkp_isss_is_saved_state, ObjWmkLib.GetCustomDatabaseName));
                SqlGetCommand.Append(string.Format(XA9Constants.SP_GET_BDC_CODE, strDBPrefix)); // Viswan - Get the custom db name from the variable
                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.PARM_CLCL_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strCLCL_ID);
                SqlGetCommand.Append(XA9Constants.STR_APOST);

                SqlGetCommand.Append(XA9Constants.SEMICOLON);
                strQuery = SqlGetCommand.ToString();
                //strDataOutput = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(SqlGetCommand.ToString());
            }
            catch (Exception ex)
            {
                //ObjWmkLib.errorLogEntry(ex.ToString());
            }
            finally
            {
                SqlGetCommand.Clear(); ;
            }
            return strQuery;

        }


        /// <summary>
        ///  Gets Stop Loss value for the claim 
        /// </summary>
        /// <param name="strIPCD_ID">Procedure Code</param>
        /// <param name="strPRPR_NPI">Servicing Provider NPI</param>
        /// <param name="dtCDML_FROM_DT">Claim From Date</param>
        /// <param name="strPDPD_ID">Product ID</param>
        /// <param name="strDBPrefix"></param>
        /// <returns></returns>
        public string GetStopLossForClaim(string strCLCL_ID, string strDBPrefix)
        {

            string strQuery = string.Empty;
            StringBuilder SqlGetCommand = new StringBuilder();
            try
            {
                //SqlGetCommand.Append(string.Format(ClsConstants.strSP_wmkp_isss_is_saved_state, ObjWmkLib.GetCustomDatabaseName));
                SqlGetCommand.Append(string.Format(XA9Constants.SP_GET_STOP_LOSS_DATA, strDBPrefix)); // Viswan - Get the custom db name from the variable
                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.PARM_CLCL_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strCLCL_ID);
                SqlGetCommand.Append(XA9Constants.STR_APOST);

                SqlGetCommand.Append(XA9Constants.SEMICOLON);
                strQuery = SqlGetCommand.ToString();

                //strDataOutput = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);

            }
            catch (Exception ex)
            {
                //ObjWmkLib.errorLogEntry(ex.ToString());
            }
            finally
            {
                SqlGetCommand.Clear(); ;
            }
            return strQuery;

        }
       

        /// <summary>
        ///  Get BDC info for the claim 
        /// </summary>
        /// <param name="strIPCD_ID">Procedure Code</param>
        /// <param name="strPRPR_NPI">Servicing Provider NPI</param>
        /// <param name="dtCDML_FROM_DT">Claim From Date</param>
        /// <param name="strPDPD_ID">Product ID</param>
        /// <param name="strDBPrefix"></param>
        /// <returns></returns>
        //public string GetBDCInfoForClaimLine(string strIPCD_ID, string strPRPR_NPI, string dtCDML_FROM_DT, string strPDPD_ID, string strDBPrefix, out string strDataOutput)
        //public string GetBDCInfoForClaimLine(string strIPCD_ID, string strPRPR_NPI, string dtCDML_FROM_DT, string strPDPD_ID, string strSFMsgs, string strDBPrefix, out string strDataOutput)
        public string GetBDCInfoForClaimLine(string strArrayProcCodes,string strArrayDiagCodes, string strPRPR_NPI, string dtCDML_FROM_DT, string strPDPD_ID, string strSFMsgs, string strDBPrefix, out string strDataOutput)
        {

            string strQuery = string.Empty;
            StringBuilder SqlGetCommand = new StringBuilder();
            strDataOutput = "";
            try
            {
                //SqlGetCommand.Append(string.Format(ClsConstants.strSP_wmkp_isss_is_saved_state, ObjWmkLib.GetCustomDatabaseName));
                SqlGetCommand.Append(string.Format(XA9Constants.SP_GET_BDC_DATA, strDBPrefix)); // Viswan - Get the custom db name from the variable
                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.PARM_IPCD_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                //SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strArrayProcCodes);
                //SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(XA9Constants.CHR_COMMA);

                SqlGetCommand.Append(XA9Constants.PARM_IDCD_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                //SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strArrayDiagCodes);
                //SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(XA9Constants.CHR_COMMA);

                SqlGetCommand.Append(XA9Constants.PARM_PRPR_NPI);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strPRPR_NPI);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(XA9Constants.CHR_COMMA);

                SqlGetCommand.Append(XA9Constants.PARM_CDML_FROM_DT);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(dtCDML_FROM_DT);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(XA9Constants.CHR_COMMA);

                SqlGetCommand.Append(XA9Constants.PARM_PDPD_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strPDPD_ID);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(XA9Constants.CHR_COMMA);

                SqlGetCommand.Append(XA9Constants.PARM_SF_MSG);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(strSFMsgs);

                SqlGetCommand.Append(XA9Constants.SEMICOLON);
                strQuery = SqlGetCommand.ToString();

                //////Logger.LoggerInstance.ReportMessage("SqlGetCommand", strIPCD_ID + " is " + SqlGetCommand);
                strDataOutput = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
                
            }
            catch (Exception ex)
            {
                //ObjWmkLib.errorLogEntry(ex.ToString());
            }
            finally
            {
                SqlGetCommand.Clear(); ;
            }
            return strQuery;

        }

      

        /// <summary>
        /// Get Related Service ID for Service ID and Service Rule.
        /// </summary>
        /// <param name="strCLCL_ID">Claim ID</param>
        /// <returns type="string">Record set xml string</returns>
        //public string GetSerlIdForService(string strCLCL_ID, string strDBPrefix, out string strDataOutput)
        public string GetSerlIdForService(string strSESE_ID,string strSESE_RULE, string strDBPrefix, out string strDataOutput)
        {
            string strQuery = string.Empty;
            StringBuilder SqlGetCommand = new StringBuilder();
            strDataOutput = "";

            try
            {
                //SqlGetCommand.Append(string.Format(ClsConstants.strSP_wmkp_isss_is_saved_state, ObjWmkLib.GetCustomDatabaseName));
                SqlGetCommand.Append(string.Format(XA9Constants.SP_GET_SERL_ID, strDBPrefix)); // Viswan - Get the custom db name from the variable
                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.PARM_SESE_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                //SqlGetCommand.Append(strCLCL_ID);
                SqlGetCommand.Append(strSESE_ID);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(XA9Constants.CHR_COMMA);

                SqlGetCommand.Append(XA9Constants.PARM_SESE_RULE);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                //SqlGetCommand.Append(strCLCL_ID);
                SqlGetCommand.Append(strSESE_RULE);
                SqlGetCommand.Append(XA9Constants.STR_APOST);

                SqlGetCommand.Append(XA9Constants.SEMICOLON);
                strQuery = SqlGetCommand.ToString();
                strDataOutput = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
                //////Logger.LoggerInstance.ReportMessage("GetSerlIdForService is : ", strDataOutput);

            }
            catch (Exception ex)
            {
                //ObjWmkLib.errorLogEntry(ex.ToString());
            }
            finally
            {
                SqlGetCommand.Clear(); ;
            }
            return strQuery;

        }

        /// <summary>
        /// Get Related Service ID for Service ID and Service Rule.
        /// </summary>
        /// <param name="strSESE_ID">Service ID (SESE_ID)</param>
        /// <param name="strSESE_RULE">Service Rule (SESE_RULE)</param>
        /// <returns type="string">Record set xml string</returns>
        public string GetTotalOfMemberCopay(int intMEME_CK, string dtCDML_FRM_DT, string strSERL_REL_ID,string strCLCL_ID, string strDBPrefix, out string strDataOutput)
        {
            string strQuery = string.Empty;
            StringBuilder SqlGetCommand = new StringBuilder();
            strDataOutput = "";

            try
            {
                //SqlGetCommand.Append(string.Format(ClsConstants.strSP_wmkp_isss_is_saved_state, ObjWmkLib.GetCustomDatabaseName));
                SqlGetCommand.Append(string.Format(XA9Constants.SP_GET_MEME_COPAY, strDBPrefix)); // Viswan - Get the custom db name from the variable
                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.PARM_CLCL_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strCLCL_ID);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(XA9Constants.CHR_COMMA);
                SqlGetCommand.Append(XA9Constants.PARM_MEME_CK);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(intMEME_CK);
                SqlGetCommand.Append(XA9Constants.CHR_COMMA);
                SqlGetCommand.Append(XA9Constants.PARM_CDML_FROM_DT);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(dtCDML_FRM_DT);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(XA9Constants.CHR_COMMA);
                SqlGetCommand.Append(XA9Constants.PARM_SERL_REL_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strSERL_REL_ID);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(XA9Constants.SEMICOLON);
                strQuery = SqlGetCommand.ToString();
                strDataOutput = FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
                //////Logger.LoggerInstance.ReportMessage("GetTotalOfMemberCopay is : ", strDataOutput);
            }
            catch (Exception ex)
            {
                //ObjWmkLib.errorLogEntry(ex.ToString());
            }
            finally
            {
                SqlGetCommand.Clear(); ;
            }
            return strQuery;

        }

        /// <summary>
        /// Set / Update StopLoss value for the claim
        /// </summary>
        /// <param name="strCLCL_ID">Claim ID</param>
        /// <param name="strStopLoss">2 char stop loss value</param>
        /// <param name="strUserID">Facets User ID</param>
        /// <param name="strDBPrefix">Custom DB Prefix</param>
        /// <param name="strQuery">Query to return</param>
        /// <returns></returns>
        public void SetStopLossForClaim(string strCLCL_ID, string strStopLoss, string strUserID, string strDBPrefix, out string strQuery)
        {
            StringBuilder SqlGetCommand = new StringBuilder();
            strQuery = "";
            try
            {
                //SqlGetCommand.Append(string.Format(ClsConstants.strSP_wmkp_isss_is_saved_state, ObjWmkLib.GetCustomDatabaseName));
                SqlGetCommand.Append(string.Format(XA9Constants.SP_SET_STOP_LOSS, strDBPrefix)); // Viswan - Get the custom db name from the variable

                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.PARM_CLCL_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strCLCL_ID);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(XA9Constants.CHR_COMMA);

                SqlGetCommand.Append(XA9Constants.PARM_STOP_LOSS);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strStopLoss);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(XA9Constants.CHR_COMMA);

                SqlGetCommand.Append(XA9Constants.PARM_USUS_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strUserID);
                SqlGetCommand.Append(XA9Constants.STR_APOST);

                SqlGetCommand.Append(XA9Constants.SEMICOLON);

                strQuery = SqlGetCommand.ToString();

                //////Logger.LoggerInstance.ReportMessage("SetBdcCodeForClaim", SqlGetCommand.ToString());
                FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// Update BDC Code for the claim
        /// </summary>
        /// <param name="strSESE_ID">Service ID (SESE_ID)</param>
        /// <param name="strSESE_RULE">Service Rule (SESE_RULE)</param>
        /// <returns type="string">Record set xml string</returns>
        public bool SetBdcCodeForClaim(string strCLCL_ID, string strBdcPgmID, string strBdcSubPgmID, string strBdcName, string strBdcFlag, string strDBPrefix, out string strQuery)
        {
            StringBuilder SqlGetCommand = new StringBuilder();
            bool blnBdcSaved = true;
            strQuery = "";
            try
            {
                //SqlGetCommand.Append(string.Format(ClsConstants.strSP_wmkp_isss_is_saved_state, ObjWmkLib.GetCustomDatabaseName));
                SqlGetCommand.Append(string.Format(XA9Constants.SP_SET_BDC_CODE, strDBPrefix)); // Viswan - Get the custom db name from the variable

                SqlGetCommand.Append(XA9Constants.SINGLE_SPACE);
                SqlGetCommand.Append(XA9Constants.PARM_CLCL_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strCLCL_ID);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(XA9Constants.CHR_COMMA);

                SqlGetCommand.Append(XA9Constants.PARM_PGM_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strBdcPgmID);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(XA9Constants.CHR_COMMA);

                SqlGetCommand.Append(XA9Constants.PARM_SUB_PGM_ID);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strBdcSubPgmID);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(XA9Constants.CHR_COMMA);

                SqlGetCommand.Append(XA9Constants.PARM_BDC_FLAG);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strBdcFlag);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(XA9Constants.CHR_COMMA);

                SqlGetCommand.Append(XA9Constants.PARM_PGM_NAME);
                SqlGetCommand.Append(XA9Constants.EQUALS);
                SqlGetCommand.Append(XA9Constants.STR_APOST);
                SqlGetCommand.Append(strBdcName);
                SqlGetCommand.Append(XA9Constants.STR_APOST);

                SqlGetCommand.Append(XA9Constants.SEMICOLON);

                strQuery = SqlGetCommand.ToString();

                //////Logger.LoggerInstance.ReportMessage("SetBdcCodeForClaim", SqlGetCommand.ToString());
                FacetsData.FacetsInstance.ExtensionDataObject.GetDbRequest(strQuery);

            }
            catch (Exception ex)
            {
                blnBdcSaved = false;
                //ObjWmkLib.errorLogEntry(ex.ToString());
            }
            finally
            {
                SqlGetCommand.Clear(); ;
            }
            return blnBdcSaved;

        }
    }
}

