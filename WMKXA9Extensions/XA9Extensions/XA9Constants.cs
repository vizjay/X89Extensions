using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XA9Extensions
{
    public static class XA9Constants
    {
        public const string XML_REPROC_COLLECTION = "<Collection name=\"REPROC\" type=\"Group\">" +
                                                        "<Column name=\"REPROC_CLAIM\">Y</Column>" +
                                                        "</Collection>";
        // XPATH 
        public const string XML_COLLUMN_XPATH = @"//FacetsData/Column[@name='{0}']";
        public const string XML_COLLECTION_XPATH = @"//FacetsData/Collection[@name='{0}']";
        public const string XML_SUB_COLLECTION_XPATH = @"//FacetsData/Collection[@name='{0}']/SubCollection[@name='{1}']";
        public const string XML_COLLECTION_COLUMN_XPATH = @"//FacetsData/Collection[@name='{0}']/Column[@name='{1}']";
        public const string XML_SUB_COLLECTION_COLUMN_XPATH = @"//FacetsData/Collection[@name='{0}']/SubCollection[@name='{1}']/Column[@name='{2}']";
        public const string XML_SUB_COLLECTION_COLUMN_XPATH_TWO_COLS = @"//FacetsData/Collection[@name='{0}']/SubCollection[@name='{1}']/Column[@name='{2}'='{3}' and @name='{4}'='{5}']";
        public const string XML_SUB_COLLECTION_COLUMN_XPATH_THREE_COLS = @"//FacetsData/Collection[@name='{0}']/SubCollection[@name='{1}']/Column[@name='{2}' and @name='{3}' and @name='{4}']";
        //CR-128 to search CDOR values
        public const string XML_COLUMN_XPATH = @"Column[@name='{0}']";

        public const string MSG_EXTN_ERR_INTLZ = "ExtensionData has not been initialized.";
        public const string TOP = "Top";
        public const string END = "End";

        //Standard string
        public const string STR_ZERO_MONEY = "0.0000";
        public const string STR_ZERO = "0";
        public const string STR_NULL = "NULL";
        public const string STR_NA = "na";
        public const string STR_DOLLAR = "$";
        public const string STR_PERIOD = ".";
        public const string STR_COMMA = ",";
        public const string STR_APOST = "'";
        public const char CHR_COMMA = ',';
        public const string SINGLE_SPACE = " ";
        public const string EQUALS = "=";
        public const string SEMICOLON = ";";

        //Stored Procedure Parameters
        public const string PARM_PROC_FAMILY = "@l_ProcFamily";
        public const string PARM_SESE_ID = "@l_SESE_ID";
        public const string PARM_SESE_RULE = "@l_SESE_RULE";
        public const string PARM_MEME_CK = "@l_MemeCK";
        public const string PARM_CDML_FROM_DT = "@l_CDML_FROM_DT";
        public const string PARM_SERL_REL_ID = "@l_RelSERL";
        public const string PARM_CLCL_ID = "@l_CLCL_ID";
        public const string PARM_IPCD_ID = "@l_IPCD_ID";
        public const string PARM_IDCD_ID = "@l_IDCD_ID";
        public const string PARM_PRPR_NPI = "@l_PRPR_NPI";
        public const string PARM_PDPD_ID = "@l_PDPD_ID";
        public const string PARM_SF_MSG = "@l_SFMSG"; // DEFECT 2231
        public const string PARM_CLCL_LOW_SVC_DT = "@l_CLCL_LOW_SVC_DT";
        public const string PARM_PGM_ID = "@l_BDC_PGM_ID";
        public const string PARM_SUB_PGM_ID = "@l_BDC_SUB_PGM_ID";
        public const string PARM_PGM_NAME = "@l_BDC_PGM_NM";
        public const string PARM_BDC_FLAG = "@l_BDC_FLAG";
        public const string PARM_STOP_LOSS = "@l_STOP_LOSS";
        public const string PARM_USUS_ID = "@l_USUS_ID";
        public const string PARM_ACCESS_USUS_ID = "@l_USUS_ID";
        public const string PARM_ACCESS_PZPZ_ID = "@l_PZPZ_ID";
        public const string PARM_PZAP_APP_ID = "@lSEDA_ID";
        public const string PARM_HOST_MACHINE = "@l_SYIN_HOST";
        public const string PARM_POS = "@l_POS";
        public const string PARM_BDC_PGM_ID = "@l_BDC_PGM_ID";
        public const string PARM_BDC_SUB_PGM_ID = "@l_SUB_PGM_ID";
        public const string PARM_BDC_PLUS_IND = "@l_BDC_PLUS_IND";

        

        public const string DME_PROC_CODES = "@pProcCodes";
        

        
        //Store procedure 

        public const string SP_GET_MEME_COPAY = "EXEC {0}.dbo.wmkp_check_meme_copay";
        public const string SP_GET_SERL_ID = "EXEC {0}.dbo.wmkp_get_serl_id";
        public const string SP_GET_BDC_DATA = "EXEC {0}.dbo.wmkp_get_bdc_for_claim";
        public const string SP_SET_BDC_CODE = "EXEC {0}.dbo.wmkp_set_bdc_code";
        public const string SP_GET_BDC_CODE = "EXEC {0}.dbo.wmkp_get_bdc_code";
        public const string SP_NPPR_PFX_FOR_PDPD = "EXEC {0}.dbo.wmkp_get_meme_nppr_pfx";
        public const string SP_SET_STOP_LOSS = "EXEC {0}.dbo.wmkp_set_clm_stop_loss";
        public const string SP_GET_USR_ACCESS_LVL = "EXEC {0}.dbo.wmkp_get_user_access";
        public const string SP_GET_STOP_LOSS_DATA = "EXEC {0}.dbo.wmkp_get_clm_stop_loss";
        public const string SP_GET_DME_PROC_CODE_FROM_LIST = "EXEC {0}.dbo.wmkp_identify_dme_codes_from_list";
        public const string SP_GET_LFTM_PROC_CODE_FROM_LIST = "EXEC {0}.dbo.wmkp_identify_lftm_codes_from_list";
        public const string SP_GET_MEME_CORRSP_WMK_MEME_CK = "EXEC {0}.dbo.wmkp_get_assoc_wmk_meme_ck_for_member";
        public const string SP_GET_ONCE_IN_A_LFTM_HIST = "EXEC {0}.dbo.wmkp_get_hist_for_once_lftm";
        public const string SP_GET_BDC_PROC_CODES_FOR_AUTH_ = "EXEC {0}.dbo.wmkp_get_bdc_auth_data";
        public const string SP_GET_GET_REL_SVC_DATA = "EXEC {0}.dbo.wmkp_get_related_svc_data";
        

        // DB Prefixes
        public const string PFX_CUSTOM = "custom";
        public const string PFX_STAGE = "stage";

        // Data Elements
        //BDC
        public const string BDC_PLUS_FLAG = "YP";
        public const string BDC_FLAG = "YY";
        public const string NON_BDC_FLAG = "NN";
        public const string NON_BDC_PROV_FLAG = "NS";
        public const string BDC = "(BDC)";
        public const string BDCPLUS = "(BDC+)";
        public const string HIGH_BENEFIT = "H";
        public const string LOW_BENEFIT = "L";
        public const string INPATIENT = "I";
        public const string OUTPATIENT = "O";

        // One Copay Per Dat
        //public const string OCPDSERLPREFIX = "P";
        //public const string OCPDSERLPREFIX = "E";
            

        //overrides
        public const string CDOR_ID_CPY_OVERRIDE = "AC";
        public const string TOS_OVERRIDE = "AS";

        //Process
        public const string PROCESS_COPAY = "COPAY EXTENSION";
        public const string PROCESS_BDC = "BDC EXTENSION";

        //Logger
        public const string LOG_EVNT_LGR = "|EventLog|";
        public const string LOG_EVNT_SOURCE = "Facets_530_XA9";
        public const string LOG_ERR_PREFIX = "ERROR: ";
        public const string LOG_VERSION_PREFIX = " v";
        public const string LOG_DATASOURCE_PREFIX = " Datasource=";
        public const string LOG_DATABASE_PREFIX = " Database=";
        public const string LOG_CLAIM_PREFIX = " CLCL_ID=";
        public const string LOG_METHD_PREFIX = " Method=";
        public const string LOG_DEFAULT_NAME = "\\XA9Extension.log";
        public const string LOG_VRBS_FRMT_LINE = "------------";
        public const string LOG_VRBS_BGN = "<<<<<<<<<<<< Begin of ";
        public const string LOG_VRBS_END = ">>>>>>>>>>>> End of ";
        public const string LOG_VRBS_CNTNTS = "------------ The contents of ";
        public const string LOG_VRBS_IS = " is ";
        public const string LOG_VRBS_CNST = " character long.";
        public const string LOG_VRBS_CNST_S = " characters long.";
        public const string LOG_PRC_INIT = "Processing initiated";
        public const string LOG_PATH = "\\Log\\{0}";
        public const string LOG_STRT_FRMT_LINE = "@@@@@@@@@@@@@@@@@@@@@@@";

        //Process names
        public const string PRC_DMEProc_BC = "ExecuteClmDMEProc";
        public const string PRC_PROC_XCOM_OCPD = "ClmOCPDXcomProcess";
        public const string PRC_PROC_XCOM_BDC = "ClmBDCXcomProcess";
        public const string PRC_START = "Processing Started. ";
        public const string PRC_END = "Processing finished in {0}. ";
        public const string PRC_ASMBLY = "XA9Extensions.dll : ";

        //Messages

        public const string MSG_OCPD_START = "OCPD Process Started.";
        public const string MSG_BDC_START = "BDC Process Started.";
        public const string MSG_UPDATE_SUCCESS = "Facets Update successful. ";
        public const string MSG_UPDATE_FAILED = "Facets Update failed. ";
        public const string MSG_ERROR_PREFIX = "Error message from Facets : ";
        public const string MSG_NO_UPDATE = "No updation to Facets required. ";
        public const string MSG_EXTN_ERR_XMLNODES = "Incorrect XML nodes selection.";
        public const string MSG_TIME_ERR = "undetermined time";
        public const string MSG_INCORRECT_APP = "Incorrect Application - ";
        public const string MSG_INCORRECT_EXIT = "Incorrect Extension Exit Timing - ";
        public const string MSG_APPLICATION = "Application";
        public const string MSG_CLCL_OBTAINED = "Received Claim Line Data";
        public const string MSG_CDML_OBTAINED = "Received Claim Line Data";
        public const string MSG_CDOR_OBTAINED = "Received Claim Override Data";
        public const string MSG_SERL_OBTAINED = "Received Related Services IDs";
        public const string MSG_SERL_COUNT = "Count of Unique Serl ID is ";
        public const string MSG_UNIQUE_CDML_DT = "Unique count of CDML From Date is  {0}";
        public const string MSG_PROCESS_CDML_DT = "Starting processing cdml from date {0}";
        public const string MSG_QUERY_CDML_DT = "strQuery for CDML_FROM_DT {0} and SERL_REL_ID {1} is {2}";
        public const string MSG_TOT_CPY_FOR_CDML_DT = "Existing Copay for CDML_FROM_DT {0} and SERL_REL_ID {1} is {2}";
        public const string MSG_UNIQUE_SESE_CNT = "Count of unique services for CDML_FROM_DT {0} and SERL_REL_ID {1} is {2}";
        public const string MSG_SETR_CPY_FOR_LINE = "SETR_COPAY_AMT for line {0} is {1}";
        public const string MSG_REM_CPY_FOR_LINE = "Remaining Copay for line {0} is {1}";
        public const string MSG_CDML_CPY_FOR_LINE = "CDML_COPAY_AMT for line {0} is {1}";




        public const string MSG_SERL_PROCESS = "Starting processing for the following SESE_ID, SESE_RULE AND SERL_REL_ID : {0}, {1}, {2}";
        public const string MSG_SERL_PROCESS_FRM_DT = "Starting processing for the following SESE_ID, SESE_RULE AND SERL_REL_ID : {0}, {1}, {2}";

        //XML Facets Context Data Item
        public const string PZPZ_ITEM = "PZPZ_ID";
        public const string PZAP_ITEM = "PZAP_APP_ID";
        public const string TMNG_ITEM = "TIMING";
        public const string EXIT_COL = "EXIT";
        public const string USUS_ITEM = "USUS_ID";
        public const string SIGNON_ID = "SGN0";
        public const string MNGL_PWD_ITEM = "MANGLED_PASSWORD";
        public const string DATABASE_ITEM = "DATABASE_ID";
        public const string DATA_SOURCE_ITEM = "DATA_SOURCE_ID";
        public const string ACSS_LVL_U = "U";
        public const string ACSS_LVL_V = "V";

        //Formats 
        public const string FRMT_SECONDS = "###,##0.000 seconds";
        public const string FRMT_DATE_FULL = "MM-dd-yyyy HH:mm:ss.fff";
        public const string FRMT_DATE_FOLDER = "MM_dd_yyyy";

        // To get the User Login data
        public const string GETDATASGN0 = "$$SGN0++";

        //Facets Dataids and Column names
        //Data ID
        public const string CLCL_DATAID = "CLCL";
        public const string CLCL_COL_DATAID = "CLCL_ID";
        public const string CLMI_DATAID = "CLMI";
        public const string CDML_DATAID = "CDMLALL";
        public const string CDOR_DATAID = "CDORALL";
        public const string CDIM_DATAID = "CDIMALL";
        public const string DATA_DATAID = "DATA";
        public const string REPROC_DATAID = "REPROC";
        public const string PRV0_DATAID = "PRV0";
        public const string MEV0_DATAID = "MEV0";
        public const string FAC_CUSTOM_DATAID = "CUSTOM";

        // Custom EXCD IDs
        //public const string EXCD_ID_COPAY = "A12";
    }
}



