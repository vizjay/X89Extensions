using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XA9Extensions.BusinessLayer;
using XA9Extensions.Utilities;

namespace XA9Extensions.BusinessLayer
{
    public class CLHP
    {
        public string CLCL_ID { get; set; }
        public string MEME_CK { get; set; }
        public string CLHP_FAC_TYPE { get; set; }
        public string CLHP_BILL_CLASS { get; set; }
        public string HOSP_TYPE
        {
            get
            {
                return CLHP_FAC_TYPE + CLHP_BILL_CLASS;
            }
        }

        public string POS_IND
        {
            get
            {
                if (string.IsNullOrEmpty(HOSP_TYPE))
                {
                    return "P";
                }
                else if (AppConfig.InpatientHospType.Contains(HOSP_TYPE))
                {
                    return "I";
                }
                else
                {
                    return "O";
                }
            }
        }
    }
}
