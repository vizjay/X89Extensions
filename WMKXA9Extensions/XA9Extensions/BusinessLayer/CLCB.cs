/*************************************************************************************************************************************************
 *  Class               : CLCB
 *  Description         : Stores CLCB data fields
 *  Used by             : 
 *  Author              : TriZetto Inc. (Wen-Man Liu)
 *************************************************************************************************************************************************
 * Ver                  Date                    Modification Description
 * 1.0                  05/20/2016              Initial creation
 * 
 *************************************************************************************************************************************************
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XA9Extensions.BusinessLayer
{
    public class CLCB
    {
        /// <summary>
        /// Claim ID
        /// </summary>
        public string CLCL_ID { get; set; }

        /// <summary>
        /// Member Contrieved Key
        /// </summary>
        public int MEME_CK { get; set; }

        /// <summary>
        /// COB Type
        /// </summary>
        public string CLCB_COB_TYPE { get; set; }

        /// <summary>
        /// COB Reason Code
        /// </summary>
        public string CLCB_COB_REAS_CD { get; set; }

        /// <summary>
        /// COB 7 Dollar Fields
        /// </summary>
        public double CLCB_COB_AMT { get; set; }

        public double CLCB_COB_DISALLOW { get; set; }

        public double CLCB_COB_ALLOW { get; set; }

        public double CLCB_COB_SANCTION { get; set; }

        public double CLCB_COB_DED_AMT { get; set; }

        public double CLCB_COB_COPAY_AMT { get; set; }

        public double CLCB_COB_COINS_AMT { get; set; }

        public List<CDCBALL> ClaimLineCOB = null;
        //constructor
        public CLCB()
        {

        }
    }

    public class CDCBALL
    {
        /// <summary>
        /// Claim ID
        /// </summary>
        public string CLCL_ID { get; set; }
        /// <summary>
        /// Claim Detail Sequence Number
        /// </summary>
        public int CDML_SEQ_NO { get; set; }
        /// <summary>
        /// Member Contrived Key
        /// </summary>
        public int MEME_CK { get; set; }
        /// <summary>
        /// COB/Medicare Type; A-Auto, C-Commercial Medical, F-No Fault, H-Homeowners, M-Medicare (Part A and B), N-Medicare Part B Only, O-Medicare A and/or B plus D, S-Subrogation, W-Workers Comp
        /// </summary>
        public string CDCB_COB_TYPE { get; set; }
        /// <summary>
        /// COB/Medicare Paid Amount
        /// </summary>
        public double CDCB_COB_AMT { get; set; }
        /// <summary>
        /// COB/Medicare Disallow Amount
        /// </summary>
        public double CDCB_COB_DISALLOW { get; set; }
        /// <summary>
        /// COB/Medicare (Higher/Lower) Allowable Amount
        /// </summary>
        public double CDCB_COB_ALLOW { get; set; }

        
    }

}
