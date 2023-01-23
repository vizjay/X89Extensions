using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommonUtilities
{
    public class Enumerations
    {
        public enum DBConnectionModes
        {
            None = 0,
            FrontEnd,
            BackEnd
        }

        public enum FrontEndInterfaceModes
        {
            None = 0,
            Interop,
            FacetsBaseControl
        }

        public enum LogicalOperators
        {
            EqualTo = 0,
            NotEqaulTo,
            GreaterThan,
            LessThan,
            GreaterThanOrEqualTo,
            LessThanOrEqualTo
        }

        public enum PadTextOptions
        {
            None = 0,
            Left,
            Right,
            Both
        }

        public enum TrimTextOptions
        {
            None = 0,
            BothEnds,
            Start,
            End
        }

        public enum PropetyReadWriteModes
        {
            All = 0,
            CanRead,
            CanWrite,
            CanReadWrite
        }

        public enum ComboBoxValueModes
        {
            OnlyValue = 0,
            OnlyDescription,
            ValueFollowedByDescription,
            DescriptionFollowedByValue
        }

        public enum ControlFocusModes
        {
            None = 0,
            Focus,
            Select
        }

        public enum Status
        {
            Normal = 0,
            Warning,
            Error,
            CriticalError,
            Exception
        }

        public class Sql
        {
            public enum CustomCodeTypes
            {
                [Description("")]
                None = 0,

                [Description("I")]
                Information,

                [Description("W")]
                Warning,

                [Description("E")]
                Error
            }
        }

        public class StringFormats
        {
            public enum DateTime
            {
                [Description("")]
                None = 0,

                [Description("yyyyMMdd_HHmmss")]
                DateTimeForFileName,

                [Description("yyyyMMdd")]
                DateForFileName,

                [Description("HHmmss")]
                TimeForFileName,

                [Description("dd-MM-yyyy hh:mm:ss tt")]
                DateTimeForDisplay12H,

                [Description("yyyy-MM-dd hh:mm:ss.fff tt")]
                DateTimeStampForDisplay12H_YYYYMMDD,

                [Description("dd-MM-yyyy hh:mm:ss.fff tt")]
                DateTimeStampForDisplay12H,

                [Description("dd-MM-yyyy HH:mm:ss")]
                DateTimeForDisplay24H,

                [Description("yyyy-MM-dd HH:mm:ss.fff")]
                DateTimeStampForDisplay24H_YYYYMMDD,

                [Description("dd-MM-yyyy HH:mm:ss.fff")]
                DateTimeStampForDisplay24H
            }
        }

        public static String GetDescriptionFromValue(Enum Value)
        {
            try
            {
                FieldInfo objFI = Value.GetType().GetField(Value.ToString());
                DescriptionAttribute objDA = (DescriptionAttribute)Attribute.GetCustomAttribute(objFI, typeof(DescriptionAttribute));
                if (objDA != null)
                {
                    return objDA.Description;
                }
                else
                {
                    return Value.ToString();
                }
            }
            catch
            {
            }
            return String.Empty;
        }

        public static T GetValueFromDescription<T>(String Description)
        {
            try
            {
                Type objType = typeof(T);
                if (objType.IsEnum)
                {
                    foreach (FieldInfo objFI in objType.GetFields())
                    {
                        DescriptionAttribute objDA = (DescriptionAttribute)Attribute.GetCustomAttribute(objFI, typeof(DescriptionAttribute));
                        if (objDA != null)
                        {
                            if (objDA.Description.ToUpper() == Description.ToUpper())
                            {
                                return (T)objFI.GetValue(null);
                            }
                        }
                        else
                        {
                            if (objFI.Name.ToUpper() == Description.ToUpper())
                            {
                                return (T)objFI.GetValue(null);
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return default(T);
        }

        public static T GetEnumFromName<T>(String Name, Boolean IgnoreCase = true) where T : struct
        {
            try
            {
                List<String> strListNames = Enum.GetNames(typeof(T)).ToList();
                Name = strListNames.Find(obj => ((obj == Name) || ((IgnoreCase) && (obj.ToUpper() == Name.ToUpper()))));
                if (Name != null)
                {
                    T Value = (T)Enum.Parse(typeof(T), Name);
                    if (Enum.IsDefined(typeof(T), Value))
                    {
                        return Value;
                    }
                }
            }
            catch
            {
            }
            return default(T);
        }

        public static T GetEnumFromInteger<T>(Int32 Value)
        {
            try
            {
                Type objType = typeof(T);
                if (Enum.IsDefined(objType, Value))
                {
                    return (T)Enum.ToObject(objType, Value);
                }
            }
            catch
            {
            }
            return default(T);
        }

        public String GetDescriptionFromProperty(PropertyInfo objPI)
        {
            try
            {
                if (objPI != null)
                {
                    List<Object> objListAttributes = objPI.GetCustomAttributes(typeof(DescriptionAttribute), true).ToList();
                    if (objListAttributes.Count > 0)
                    {
                        DescriptionAttribute objDA = (DescriptionAttribute)objListAttributes[0];
                        if (objDA != null)
                        {
                            return objDA.Description;
                        }
                    }
                }
            }
            catch
            {
            }
            return String.Empty;
        }
    }
}
