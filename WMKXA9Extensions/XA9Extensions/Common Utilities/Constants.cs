using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace CommonUtilities
{
    public class Constants
    {
        public class XmlNodeDataID
        {
            public const String Blank = Constants.DefaultValues.String;
            public const String Custom = "DATA";
            public const String SGN0 = "SGN0";
            public const String SGN0_FBC = "$$SGN0";
            public const String SGN0_FBC_All = "$$SGN0++";
            public const String ODBC_CON_FBC = "$$ODBCCON0";
            public const String CTXT_PZAP_FBC = "$CTXT_PZAP";
            public const String CTXT_SYIN_FBC = "$CTXT_SYIN";
            public const String EXIT = "EXIT";
            public const String GRGR = "GRGR";
            public const String SGSG = "SGSG";
            public const String SGSC = "SGSC";
            public const String CLCL = "CLCL";
            public const String CDML = "CDML";
        }

        public class Utilities
        {
            public const String EventLogName = "Application";
            public const Int32 Duration = 5000;
            public const Char PadCharString = ' ';
            public const Char PadCharNumber = '0';
            public const Int32 PadWidth = 30;
        }

        public class DefaultValues
        {
            public const Int16 Integer = 0;
            public const UInt16 UInteger = 0;
            public const String String = "";
            public const String StringNull = null;

            public static DateTime Date
            {
                get { return new DateTime(1900, 1, 1); }
            }

            public static DateTime TerminationDate
            {
                get { return new DateTime(9999, 12, 31); }
            }
        }

        public class Messages
        {
            public const String TechnicalError = "Technical Error => Query Execution Failed. Contact System Administrator";
        }

        public class Fonts
        {
            public static Font Calibri
            {
                get { return new Font("Calibri", 10, FontStyle.Regular); }
            }

            public static Font CourierNew
            {
                get { return new Font("Courier New", 9, FontStyle.Regular); }
            }
        }

        public class Sizes
        {
            public static Size ControlSmall
            {
                get
                {
                    return new Size(180, 23);
                }
            }

            public static Size ControlMedium
            {
                get
                {
                    return new Size(120, 23);
                }
            }

            public static Size ControlLarge
            {
                get
                {
                    return new Size(180, 90);
                }
            }
        }
    }
}
