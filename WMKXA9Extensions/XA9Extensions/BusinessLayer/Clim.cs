using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XA9Extensions.BusinessLayer
{
    public class Clim
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
        /// Message Code type (i.e SF, DF..)
        /// </summary>
        public string CLIM_TYP { get; set; }

        /// <summary>
        /// Message Code value
        /// </summary>
        public string CLIM_ITS_MSG_CD { get; set; }



    }
}
