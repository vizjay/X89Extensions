/*************************************************************************************************************************************************
 *  Class               : Claim
 *  Description         : Stores CLCL data fields and associated records
 *  Used by             : 
 *  Author              : TriZetto Inc.
 *************************************************************************************************************************************************
 * Ver                  Date                    Modification Description
 * 1.0                  05/01/2016              Initial creation (Viswan Jayaraman)
 *                      05/20/2016              Revised by Wen-Man Liu; added additional CLCL fields
 *                      06/15/2016              Revised by Wen-Man Liu; added additional CLCL fields
 * 1.2                  10/21/2016              Revised by Viswan Jayaraman; added PDDS_MCTR_BCAT
 
 *************************************************************************************************************************************************
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XA9Extensions.BusinessLayer
{
    public class Claim
    {
        private List<string> _uniqueProcedureCodes;
        private List<string> _uniqueDiagCodes;
        private List<string> _notNullUniqueLineLevelProcCodes;
        private List<string> _notNullUniqueHeaderLevelProcCodes;
        private List<string> _notNullUniqueHeaderLevelDiagCodes;
        /// <summary>
        /// Claim ID
        /// </summary>
        public string CLCL_ID { get; set; }
        /// <summary>
        /// Claim Status
        /// </summary>
        public string CLCL_CUR_STS { get; set; }
        /// <summary>
        /// Member Contrieved Key
        /// </summary>
        public int MEME_CK { get; set; }
        
        /// <summary>
        /// Claim Pre Price Indicator (H = ITS Home)
        /// </summary>
        public string CLCL_PRE_PRICE_IND { get; set; }

        /// <summary>
        /// Claim Type
        /// </summary>
        public string CLCL_CL_TYPE { get; set; }

        /// <summary>
        /// Claim Sub Type
        /// </summary>
        public string CLCL_CL_SUB_TYPE { get; set; }

        /// <summary>
        /// Product ID
        /// </summary>
        public string PDPD_ID { get; set; }

        /// <summary>
        /// Provider Agreement ID
        /// </summary>
        public string AGAG_ID { get; set; }

        /// <summary>
        /// Provider Address ID
        /// </summary>
        //public string PRAD_ID { get; set; }

        //v1.1 BEGIN
        /// <summary>
        /// Claim's Earliest From Date; could be "NULL" for new claims; <Column name="CLCL_LOW_SVC_DT">NULL</Column>
        /// </summary>
        public string CLCL_LOW_SVC_DT { get; set; }

        /// <summary>
        /// Claim's Latest To Date; could be "NULL" for new claims; <Column name="CLCL_HIGH_SVC_DT">NULL</Column>
        /// </summary>
        public string CLCL_HIGH_SVC_DT { get; set; }

        /// <summary>
        /// Claim Level Capitation Indicator; F, N, P, space(blank)
        /// </summary>
        public string CLCL_CAP_IND { get; set; }

        public CLCB ClaimCLCB { get; set; }
        // v1.1 END

        //v1.2 BEGIN
        /// <summary>
        /// Subscriber Contrieved Key
        /// </summary>
        public int SBSB_CK { get; set; }

        /// <summary>
        /// Provider ID
        /// </summary>
        public string PRPR_ID { get; set; }

        /// <summary>
        /// REPROC_FLAG from CUSTOM data collection
        /// </summary>
        public string REPROC_FLAG { get; set; }

        /// <summary>
        /// CLST_STS from CLST data collection
        /// </summary>
        public string CLST_STS { get; set; }

        /// <summary>
        /// Identifies the claim that was adjusted
        /// </summary>
        public string CLCL_ID_ADJ_FROM { get; set; }
        //v1.2 END

        /// <summary>
        /// Trading Partner ID for electronic claims
        /// </summary>
        public string CLED_TRAD_PARTNER { get; set; }

        /// <summary>
        /// User Data to determine if it is a MedSup claim
        /// </summary>
        public string CLED_USER_DATA1 { get; set; }

        public string CLED_USER_DATA2 { get; set; }

        /// <summary>
        /// Flag to indicate whether BDC should be saved at POSTSAVE
        /// </summary>
        public string SAVE_BDC { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ClaimLine> claimLines { get; set; }

        /// <summary>
        /// Non Null Line level procedure codes for the current claim
        /// </summary>
        public List<string> UniqueLineLevelNotNullProcedureCodes
        {
            get
            {
                if (_notNullUniqueLineLevelProcCodes == null)
                    _notNullUniqueLineLevelProcCodes = this.claimLines.Where(line => !string.IsNullOrEmpty(line.IPCD_ID)).Select(line => line.IPCD_ID).Distinct().ToList();
                return _notNullUniqueLineLevelProcCodes;
            }
        }
        /*
        /// <summary>
        /// Claim Lines where IPCD ID is not null
        /// </summary>
        public List<ClaimLine> claimLinesForBDC
        {
            get
            {
                return this.claimLines.Where(l => !string.IsNullOrEmpty(l.IPCD_ID)).ToList();
            }
        }
         * */
        /// <summary>
        /// Returns the unique procedure codes for this claim
        /// </summary>
        //public List<string> UniqueProcedureCodes
        public List<string> UniqueNotNullProcedureCodes
        {
            get
            {
                if (_uniqueProcedureCodes == null)
                {
                    _uniqueProcedureCodes = this.UniqueHeaderLevelNotNullProcCodes
                                                //.Union(this.claimLines.Where(line => !string.IsNullOrEmpty(line.IPCD_ID)).Select(line => line.IPCD_ID).Distinct().ToList()).ToList();
                                                .Union(this.claimLines.Where(line => !string.IsNullOrEmpty(line.IPCD_ID)).Select(line => line.IPCD_ID.Substring(0,line.IPCD_ID.Length > 5 ? 5 : line.IPCD_ID.Length)).Distinct().ToList()).ToList();
                                                //.Union(new XA9BusinessLayer().GetClhiProcCodes()).ToList();
                }
                return _uniqueProcedureCodes;
            }
        }

        /// <summary>
        /// Returns the unique Diag (header and line level) codes for this claim
        /// </summary>
        //public List<string> UniqueProcedureCodes
        public List<string> UniqueNotNullDiagCodes
        {
            get
            {
                if (_uniqueDiagCodes == null)
                {
                    _uniqueDiagCodes = this.UniqueHeaderLevelNotNullDiagCodes
                                                .Union(this.claimLines.Where(line => !string.IsNullOrEmpty(line.IDCD_ID)).Select(line => line.IDCD_ID).Distinct().ToList()).ToList();
                    //.Union(new XA9BusinessLayer().GetClhiProcCodes()).ToList();
                }
                return _uniqueDiagCodes;
            }
        }

        /// <summary>
        /// Header Level Procedure codes
        /// </summary>
        public List<string> UniqueHeaderLevelNotNullDiagCodes
        {
            get
            {
                if (_notNullUniqueHeaderLevelDiagCodes == null)
                {
                    _notNullUniqueHeaderLevelDiagCodes = new XA9BusinessLayer().GetClmdDiagCodes();
                }
                return _notNullUniqueHeaderLevelDiagCodes;
            }
        }

        /// <summary>
        /// Header Level Procedure codes
        /// </summary>
        public List<string> UniqueHeaderLevelNotNullProcCodes
        {
            get
            {
                if (_notNullUniqueHeaderLevelProcCodes == null)
                {
                    _notNullUniqueHeaderLevelProcCodes = new XA9BusinessLayer().GetClhiProcCodes();
                }
                return _notNullUniqueHeaderLevelProcCodes;
            }
        }
        /// <summary>
        /// Is a Fully Denied Claim
        /// </summary>
        public bool IsDeniedClaim
        {
            get
            {
                if(this.claimLines.Any(line => line.IsDenied.Equals(false)))
                    return false;
                else
                    return true;

            }
        }
        /// <summary>
        /// 
        /// </summary>
        public List<Cdor> claimLineOverrides { get; set; }

        /// <summary>
        /// CLOR data
        /// </summary>
        public List<CLOR> claimOverrides { get; set; }

        /// <summary>
        /// ITS Claim Line Details
        /// </summary>
        public List<Clim> claimITS { get; set; }

        /// <summary>
        /// CLHP Data
        /// </summary>
        public CLHP HospClaimData { get; set; }

       /// <summary>
       /// Returns true if the claim is adjusted. Else false
       /// </summary>
        public bool IsAdjustedClaim
        {
            get
            {
                if (string.IsNullOrEmpty(this.CLCL_ID_ADJ_FROM))
                    return false;
                else
                    return true;

            }
        }

        /// <summary>
        /// Servicing Provider Data
        /// </summary>
        public PRPR ServicingProvider { get; set; }
        /// <summary>
        /// Non-Participating Provider Prefix
        /// </summary>
        public string PDBC_PFX_NPPR {get;set;}

        /// <summary>
        /// List of WMK MEME_CK for the member on the claim
        /// </summary>
        public List<int> WellmarkMemberContrievedKeys { get; set; }

        /// <summary>
        /// Product Business Category code
        /// </summary>
        public string PDDS_MCTR_BCAT { get; set; }

        public Claim()
        {

        }
        
    }

    /*public class onceinalf
    {
        public object Family { get; set; }

        public List<ClaimLine> claimLines { get; set; }
    }*/
}
