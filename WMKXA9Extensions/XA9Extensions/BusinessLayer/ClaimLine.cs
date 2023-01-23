/*************************************************************************************************************************************************
 *  Class               : ClaimLine
 *  Description         : Stores CDMLALL line data fields and associated records
 *  Used by             : 
 *  Author              : TriZetto Inc.
 *************************************************************************************************************************************************
 * Ver                  Date                    Modification Description
 * 1.0                  05/01/2016              Initial creation (Viswan Jayaraman)
 * 1.1                  05/20/2016              Revised by Wen-Man Liu; added additional line fields
 * 1.2                  06/15/2016              Revised by Wen-Man Liu; added additional line fields
 *************************************************************************************************************************************************
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using XA9Extensions.Utilities;

namespace XA9Extensions.BusinessLayer
{
    
    public class ClaimLine
    {
        /// <summary>
        /// Claim ID
        /// </summary>
        public string CLCL_ID { get; set; }

        /// <summary>
        /// Claims Sub Type
        /// </summary>
        public string CLCL_CL_SUB_TYPE { get; set; }
        /// <summary>
        /// Claim Line Sequence Number
        /// </summary>
        public int CDML_SEQ_NO { get; set; }

        /// <summary>
        /// Member Contrieved Key
        /// </summary>
        public int MEME_CK { get; set; }

        /// <summary>
        /// Claim Line From Date
        /// </summary>
        public DateTime CDML_FROM_DT { get; set; }

        /// <summary>
        /// Service ID
        /// </summary>
        public string SESE_ID { get; set; }

        /// <summary>
        /// Service Rule
        /// </summary>
        public string SESE_RULE { get; set; }

        /// <summary>
        /// Claim line Copay Amount
        /// </summary>
        public double CDML_COPAY_AMT { get; set; }

        /// <summary>
        /// Procedure Code
        /// </summary>
        public string IPCD_ID { get; set; }

        /// <summary>
        /// Place of Service Indicator. O = Outpatient, I = Inpatient
        /// </summary>
        public string CDML_POS_IND { get; set; }

        /// <summary>
        /// Related Service ID. Populated as needed as this will have to be fetched through a SQL Query.
        /// </summary>
        public string SERL_REL_ID {get;set;}

        /// <summary>
        /// SETR Copay Amount for SESE_ID and SESE_RULE
        /// </summary>
        public double SETR_COPAY { get; set; }

        /// <summary>
        /// CDML Units
        /// </summary>
        public int CDML_UNITS { get; set; }

        /// <summary>
        /// Line Charge Amount
        /// </summary>
        public double CDML_CHG_AMT { get; set; }

        //v1.1
        /// <summary>
        /// Line Paid Amount
        /// </summary>
        public double CDML_PAID_AMT { get; set; }

        /// <summary>
        /// Line Subscriber Paid Amount
        /// </summary>
        public double CDML_SB_PYMT_AMT { get; set; }

        /// <summary>
        /// Line Provider Paid Amount
        /// </summary>
        public double CDML_PR_PYMT_AMT { get; set; }

        //v1.2
        /// <summary>
        /// Allowable Amount
        /// </summary>
        public double CDML_ALLOW { get; set; }

        /// <summary>
        /// Disallow Amount
        /// </summary>
        public double CDML_DISALL_AMT { get; set; }
        /// <summary>
        /// Diagnosis Code
        /// </summary>
        public string IDCD_ID { get; set; }

        /// <summary>
        /// Translated Related Diagnosis Code
        /// </summary>
        public string IDCD_ID_TRANS_REL { get; set; }

        /// <summary>
        /// Claim Line From Date; string format
        /// </summary>
        public string strCDML_FROM_DT { get; set; }

        /// <summary>
        /// Considered Charge
        /// </summary>
        public double CDML_CONSIDER_CHG { get; set; }

        /// <summary>
        /// This property is for Non Par claims. For medical claims when CDML_ALLOW is 0.00 then return true. For Hospital claims, when sum of CDMD (Disallow) is equal to Charge then returns true. Else false
        /// </summary>
        public bool IsDeniedNP
        {
            get
            {
                if (CLCL_CL_SUB_TYPE.Equals("M"))
                {
                    if (CDML_ALLOW == 0.00)
                        return true;
                    else
                        return false;
                }
                else if (CLCL_CL_SUB_TYPE.Equals("H"))
                {
                    if (FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDMDALL", "CDMDALL"))
                        .Where(cdmd => cdmd.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value.Equals(this.CDML_SEQ_NO.ToString()))
                        .Sum(e => double.Parse(e.Elements().FirstOrDefault(e1 => e1.Attribute("name").Value.Equals("CDMD_DISALL_AMT")).Value)) == this.CDML_CHG_AMT)

                    {
                        Logger.LoggerInstance.ReportMessage("CDMD Amount for Line # " + this.CDML_SEQ_NO.ToString() + " is ",
                            FacetsData.FacetsInstance.FacetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, "CDMDALL", "CDMDALL"))
                        .Where(cdmd => cdmd.Elements().FirstOrDefault(e => e.Attribute("name").Value.Equals("CDML_SEQ_NO")).Value.Equals(this.CDML_SEQ_NO.ToString()))
                        .Sum(e => double.Parse(e.Elements().FirstOrDefault(e1 => e1.Attribute("name").Value.Equals("CDMD_DISALL_AMT")).Value)).ToString());

                        Logger.LoggerInstance.ReportMessage("IsDeniedNP", "For Line #  " + this.CDML_SEQ_NO.ToString() + "  is  TRUE");
                        return true;
                    }
                    else
                    {
                        Logger.LoggerInstance.ReportMessage("IsDeniedNP", "For Line #  " + this.CDML_SEQ_NO.ToString() + "  is  false");
                        return false;
                    }
                }
                else
                    return false;
            }
        }
        /// <summary>
        /// Returns true when Allowed Amount is 0.00
        /// </summary>
        public bool IsDenied
        {
            get
            {
                if (CDML_ALLOW == 0.00)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// Place of Service Code ID
        /// </summary>
        public string PSCD_ID { get; set; }

        /// <summary>
        /// Revenue Code
        /// </summary>
        public string RCRC_ID { get; set; }

        /// <summary>
        /// Agreement price
        /// </summary>
        public double CDML_AG_PRICE { get; set; }
        /// <summary>
        /// Sets true if the procedure code is a DME. ELse False. 
        /// By default, the value is false
        /// </summary>
        public bool IsDMELine { get; set; }

        /// <summary>
        /// Sets the Once in a LF family group if this procedure is a once in a LF procedure
        /// </summary>
        public string OnceInALFGroup { get; set; }

        /// <summary>
        /// Contains the first two characters for TOS override
        /// </summary>
        public string BDCTOSVal { get; set; }

        /// <summary>
        /// SERL for SESE_ID and SESE_RULE
        /// </summary>
        public SERL RelatedServiceID { get; set; }
        
    }
}
