using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XA9Extensions.BusinessLayer
{
    public class SERL
    {

        /// <summary>
        /// Service ID
        /// </summary>
        public string SESE_ID { get; set; }

        /// <summary>
        /// Service Rule
        /// </summary>
        public string SESE_RULE { get; set; }

        /// <summary>
        /// Related Service ID
        /// </summary>
        public string SERL_REL_ID { get; set; }

        /// <summary>
        /// Service Relation Type. Possible Values are C , E , F and P
        /// </summary>
        public string SERL_REL_TYPE { get; set; }

        /// <summary>
        /// Service Related Period Indicator. Values are D- Day, M-Month, Y-Year
        /// </summary>
        public string SERL_REL_PER_IND { get; set; }

        /// <summary>
        /// Related Period. Length of the related period.
        /// </summary>
        public int SERL_PER { get; set; }

        /// <summary>
        /// Maximum Allowed
        /// </summary>
        public double SETR_ALLOW_AMT { get; set; }

        /// <summary>
        /// Maximum Counter Allowed
        /// </summary>
        public int SETR_ALLOW_CTR { get; set; }

    }
}
