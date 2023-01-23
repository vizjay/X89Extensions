using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XA9Extensions.BusinessLayer
{
    public class Cdor
    {
        /// <summary>
        /// Claim ID
        /// </summary>
        public string CLCL_ID { get; set; }

        /// <summary>
        /// Claim Line Sequence Number
        /// </summary>
        public int CDML_SEQ_NO { get; set; }

        /// <summary>
        /// Member Contrieved Key
        /// </summary>
        public int MEME_CK { get; set; }

        /// <summary>
        /// Override ID
        /// </summary>
        public string CDOR_OR_ID { get; set; }

        /// <summary>
        /// Override Amount
        /// </summary>
        public double CDOR_OR_AMT { get; set; }

        /// <summary>
        /// Override Value
        /// </summary>
        public string CDOR_OR_VALUE { get; set; }

        /// <summary>
        /// Override Date; could be "NULL"; <Column name="CDOR_OR_DT">NULL</Column>
        /// </summary>
        public string CDOR_OR_DT { get; set; }

        /// <summary>
        /// Overridd Explanation Code
        /// </summary>
        public string EXCD_ID { get; set; }

        /// <summary>
        /// Overridd User ID
        /// </summary>
        public string CDOR_OR_USID { get; set; }

    }
}
