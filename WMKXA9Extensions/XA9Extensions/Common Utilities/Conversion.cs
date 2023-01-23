using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CommonUtilities
{
    public class Conversion
    {
        public virtual Int16 ToInt16(Object Value)
        {
            try
            {
                return Convert.ToInt16(Value);
            }
            catch
            {
            }
            return 0;
        }

        public virtual Int32 ToInt32(Object Value)
        {
            try
            {
                return Convert.ToInt32(Value);
            }
            catch
            {
            }
            return 0;
        }

        public virtual Int64 ToInt64(Object Value)
        {
            try
            {
                return Convert.ToInt64(Value);
            }
            catch
            {
            }
            return 0;
        }

        public virtual UInt16 ToUInt16(Object Value)
        {
            try
            {
                return Convert.ToUInt16(Value);
            }
            catch
            {
            }
            return 0;
        }

        public virtual UInt32 ToUInt32(Object Value)
        {
            try
            {
                return Convert.ToUInt32(Value);
            }
            catch
            {
            }
            return 0;
        }

        public virtual UInt64 ToUInt64(Object Value)
        {
            try
            {
                return Convert.ToUInt64(Value);
            }
            catch
            {
            }
            return 0;
        }

        public virtual float ToFloat(Object Value)
        {
            try
            {
                return Convert.ToSingle(Value);
            }
            catch
            {
            }
            return 0;
        }

        public virtual Double ToDouble(Object Value)
        {
            try
            {
                return Convert.ToDouble(Value);
            }
            catch
            {
            }
            return 0;
        }

        public virtual Decimal ToDecimal(Object Value)
        {
            try
            {
                return Convert.ToDecimal(Value);
            }
            catch
            {
            }
            return 0;
        }

        public virtual Boolean ToBoolean(Object Value)
        {
            try
            {
                Type objType = Value.GetType();
                if (objType == typeof(String))
                {
                    String strValue = Value.ToString().ToUpper();
                    switch (strValue)
                    {
                        case "YES":
                        case "Y":
                        case "TRUE":
                            return true;
                        case "NO":
                        case "N":
                        case "FALSE":
                            return false;
                    }
                }

                Decimal decValue = ToDecimal(Value);
                if (decValue > 0)
                {
                    return true;
                }

                return Convert.ToBoolean(Value);
            }
            catch
            {
            }
            return false;
        }

        public virtual Byte ToByte(Object Value)
        {
            try
            {
                return Convert.ToByte(Value);
            }
            catch
            {
            }
            return default(Byte);
        }

        public virtual SByte ToSByte(Object Value)
        {
            try
            {
                return Convert.ToSByte(Value);
            }
            catch
            {
            }
            return default(SByte);
        }

        public virtual Char ToChar(Object Value)
        {
            try
            {
                Type objType = Value.GetType();
                if (objType == typeof(String))
                {
                    String strValue = Value.ToString().ToUpper();
                    if (strValue.Length > 0)
                    {
                        return strValue[0];
                    }
                }

                return Convert.ToChar(Value);
            }
            catch
            {
            }
            return '\0';
        }

        public virtual Char[] ToChars(Object Value)
        {
            try
            {
                return Value.ToString().ToCharArray();
            }
            catch
            {
            }
            return new Char[] { };
        }

        public virtual List<Char> ToCharList(Object Value)
        {
            try
            {
                return ToChars(Value).ToList();
            }
            catch
            {
            }
            return new List<Char>();
        }

        public virtual DateTime ToDateTime(Object Value, Boolean IsAdjustForDateTimePicker = false)
        {
            try
            {
                if (!String.IsNullOrEmpty(Value.ToString()))
                {
                    DateTime objDTM = Convert.ToDateTime(Value);
                    if (IsAdjustForDateTimePicker)
                    {
                        if (objDTM < DateTimePicker.MinimumDateTime)
                        {
                            objDTM = DateTimePicker.MinimumDateTime;
                        }
                        else if (objDTM > DateTimePicker.MaximumDateTime)
                        {
                            objDTM = DateTimePicker.MaximumDateTime;
                        }
                    }
                    return objDTM;
                }
            }
            catch
            {
            }
            return Constants.DefaultValues.Date;
        }

        public virtual String ToDateTimeString(Object Value, String DateFormat, Boolean IsAdjustForDateTimePicker = false)
        {
            try
            {
                return ToDateTime(Value, IsAdjustForDateTimePicker).ToString(DateFormat);
            }
            catch
            {
            }
            return String.Empty;
        }

        public virtual String ToHexaDecimal(String Value)
        {
            try
            {
                Byte[] AsciiArray = Encoding.ASCII.GetBytes(Value);
                return BitConverter.ToString(AsciiArray);
            }
            catch
            {
            }
            return String.Empty;
        }

        public virtual String ToText(String HexValue)
        {
            try
            {
                Boolean IsContinue = false;
                if (HexValue.Length >= 2)
                {
                    Regex objRE = new Regex("\\A\\b[0-9a-fA-F]+\\b\\Z");
                    String strPureHex = HexValue.Replace("-", String.Empty);
                    if (objRE.IsMatch(strPureHex) && strPureHex.Length % 2 == 0)
                    {
                        IsContinue = true;
                    }
                }

                if (IsContinue)
                {
                    Byte[] AsciiBytes = HexValue.Split('-').Select(x => Byte.Parse(x, System.Globalization.NumberStyles.HexNumber)).ToArray();
                    return Encoding.ASCII.GetString(AsciiBytes);
                }
            }
            catch
            {
            }
            return String.Empty;
        }
    }
}
