using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommonUtilities
{
    public class ObjectEx
    {
        Conversion objCE = new Conversion();

        public virtual Object CreateObjectInstance(Type objType)
        {
            try
            {
                ConstructorInfo objCI = objType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList().Find(obj => obj.GetParameters().Length == 0);
                if (objCI != null)
                {
                    return objCI.Invoke(new Object[] { });
                }
                throw new Exception(String.Format("Define a Public Parameterless Constructor in the class \"{0}\".", objType.Name));
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return null;
        }

        public virtual T CreateObjectInstance<T>()
        {
            try
            {
                return (T)CreateObjectInstance(typeof(T));
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return default(T);
        }

        public virtual Object ConvertValueOfPIType(PropertyInfo objPI, String Value)
        {
            try
            {
                Type PropertyType = objPI.PropertyType;
                if (PropertyType == typeof(Int16))
                {
                    return objCE.ToInt16(Value);
                }
                else if (PropertyType == typeof(Int32))
                {
                    return objCE.ToInt32(Value);
                }
                else if (PropertyType == typeof(Int64))
                {
                    return objCE.ToInt64(Value);
                }
                else if (PropertyType == typeof(UInt16))
                {
                    return objCE.ToUInt16(Value);
                }
                else if (PropertyType == typeof(UInt32))
                {
                    return objCE.ToUInt32(Value);
                }
                else if (PropertyType == typeof(UInt64))
                {
                    return objCE.ToUInt64(Value);
                }
                else if (PropertyType == typeof(float))
                {
                    return objCE.ToFloat(Value);
                }
                else if (PropertyType == typeof(Double))
                {
                    return objCE.ToDouble(Value);
                }
                else if (PropertyType == typeof(Decimal))
                {
                    return objCE.ToDecimal(Value);
                }
                else if (PropertyType == typeof(Boolean))
                {
                    return objCE.ToBoolean(Value);
                }
                else if (PropertyType == typeof(Byte))
                {
                    return objCE.ToByte(Value);
                }
                else if (PropertyType == typeof(SByte))
                {
                    return objCE.ToSByte(Value);
                }
                else if (PropertyType == typeof(Char))
                {
                    return objCE.ToChar(Value);
                }
                else if (PropertyType == typeof(DateTime))
                {
                    return objCE.ToDateTime(Value);
                }
                else if (PropertyType == typeof(String))
                {
                    return Value;
                }
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return null;
        }

        public virtual Enumerations.Status SetValueUsingPI<T>(PropertyInfo objPI, T objSource, ref T objDestination)
        {
            try
            {
                Type objType = typeof(T);
                if (objSource != null && objDestination != null)
                {
                    if (objPI.CanRead && objPI.CanWrite)
                    {
                        Object objValue = objPI.GetValue(objSource, null);
                        if (objValue != null)
                        {
                            String Value = objValue.ToString();
                            objValue = ConvertValueOfPIType(objPI, Value);
                            if (objValue != null)
                            {
                                objPI.SetValue(objDestination, objValue, null);
                                return Enumerations.Status.Normal;
                            }
                        }
                    }
                }
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
                return Enumerations.Status.Exception;
            }
            return Enumerations.Status.Error;
        }

        public virtual Enumerations.Status SetValueUsingPI(PropertyInfo objPI, String Value, ref Object objDestination)
        {
            try
            {
                Object objValue = ConvertValueOfPIType(objPI, Value);
                if (objValue != null && objPI != null)
                {
                    if (objPI.CanWrite)
                    {
                        objPI.SetValue(objDestination, objValue, null);
                        return Enumerations.Status.Normal;
                    }
                }
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException, true);
                return Enumerations.Status.Exception;
            }
            return Enumerations.Status.Error;
        }

        public virtual Enumerations.Status CopyObjectUsingPI<T>(T objSource, T objDestination, List<PropertyInfo> objListPI = null)
        {
            try
            {
                return CopyObject<T>(objSource, objDestination, objListPI.Select(obj => obj.Name).ToList());
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
                return Enumerations.Status.Exception;
            }
        }

        public virtual Enumerations.Status CopyObject<T>(T objSource, T objDestination, List<String> strListPropertyNames = null)
        {
            try
            {
                foreach (PropertyInfo objPI in GetSelectedProperties<T>(strListPropertyNames))
                {
                    SetValueUsingPI<T>(objPI, objSource, ref objDestination);
                }
                return Enumerations.Status.Normal;
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
                return Enumerations.Status.Exception;
            }
        }

        public virtual List<PropertyInfo> GetProperties(Type objType, List<String> strListPropertiesToExclude = null, List<String> strListPropertyTypesToExclude = null, Enumerations.PropetyReadWriteModes Mode = Enumerations.PropetyReadWriteModes.CanReadWrite)
        {
            try
            {
                List<PropertyInfo> objListPI = objType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();
                switch (Mode)
                {
                    case Enumerations.PropetyReadWriteModes.All:
                        break;
                    case Enumerations.PropetyReadWriteModes.CanRead:
                        objListPI = objListPI.FindAll(obj => obj.CanRead);
                        break;
                    case Enumerations.PropetyReadWriteModes.CanWrite:
                        objListPI = objListPI.FindAll(obj => obj.CanWrite);
                        break;
                    case Enumerations.PropetyReadWriteModes.CanReadWrite:
                        objListPI = objListPI.FindAll(obj => obj.CanRead && obj.CanWrite);
                        break;
                    default:
                        break;
                }

                if (strListPropertiesToExclude != null)
                {
                    foreach (String PropertyName in strListPropertiesToExclude)
                    {
                        objListPI = objListPI.FindAll(obj => obj.Name != PropertyName);
                    }
                }

                if (strListPropertyTypesToExclude != null)
                {
                    foreach (String PropertyType in strListPropertyTypesToExclude)
                    {
                        objListPI = objListPI.FindAll(obj => obj.PropertyType.Name != PropertyType);
                    }
                }

                return objListPI;
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return new List<PropertyInfo>();
        }

        public virtual List<PropertyInfo> GetProperties<T>(List<String> strListPropertiesToExclude = null, List<String> strListPropertyTypesToExclude = null, Enumerations.PropetyReadWriteModes Mode = Enumerations.PropetyReadWriteModes.CanReadWrite)
        {
            try
            {
                return GetProperties(typeof(T), strListPropertiesToExclude, strListPropertyTypesToExclude, Mode);
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return new List<PropertyInfo>();
        }

        public virtual List<PropertyInfo> GetSelectedProperties(Type objType, List<String> strListPropertiesToCompare = null, Enumerations.PropetyReadWriteModes Mode = Enumerations.PropetyReadWriteModes.CanReadWrite)
        {
            try
            {
                List<PropertyInfo> objListPI = GetProperties(objType, Mode: Mode);
                if (strListPropertiesToCompare != null)
                {
                    strListPropertiesToCompare = strListPropertiesToCompare.Select(obj => obj.Trim()).ToList().FindAll(obj => !String.IsNullOrEmpty(obj));
                    if (strListPropertiesToCompare.Count > 0)
                    {
                        List<PropertyInfo> objListPI_Final = new List<PropertyInfo>();
                        foreach (String PropertyName in strListPropertiesToCompare)
                        {
                            objListPI_Final.AddRange(objListPI.FindAll(obj => obj.Name == PropertyName));
                        }
                        return objListPI_Final;
                    }
                }
                return objListPI;
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return new List<PropertyInfo>();
        }

        public virtual List<PropertyInfo> GetSelectedProperties<T>(List<String> strListPropertiesToCompare = null, Enumerations.PropetyReadWriteModes Mode = Enumerations.PropetyReadWriteModes.CanReadWrite)
        {
            try
            {
                return GetSelectedProperties(typeof(T), strListPropertiesToCompare, Mode: Mode);
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return new List<PropertyInfo>();
        }

        public virtual Boolean AreSameObjects<T>(T objSource, T objDestination, List<String> strListPropertiesToExclude = null, List<String> strListPropertyTypesToExclude = null, Enumerations.PropetyReadWriteModes Mode = Enumerations.PropetyReadWriteModes.CanReadWrite)
        {
            try
            {
                List<PropertyInfo> objListPI = GetProperties<T>(strListPropertiesToExclude, strListPropertyTypesToExclude, Mode);
                foreach (PropertyInfo objPI in objListPI)
                {
                    if (objPI.GetValue(objSource, null).ToString() != objPI.GetValue(objDestination, null).ToString())
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return false;
        }

        public virtual String ToString(Object obj, Enumerations.PropetyReadWriteModes Mode = Enumerations.PropetyReadWriteModes.CanRead)
        {
            try
            {
                if (obj != null)
                {
                    List<PropertyInfo> objListPI = new ObjectEx().GetProperties(obj.GetType(), Mode: Mode);
                    if (objListPI.Count > 0)
                    {
                        StringBuilder objSB = new StringBuilder();
                        foreach (PropertyInfo objPI in objListPI)
                        {
                            objSB.AppendLine(String.Format("{0}: {1}", objPI.Name.PadRight(Constants.Utilities.PadWidth, Constants.Utilities.PadCharString), objPI.GetValue(obj, null)));
                        }
                        return objSB.ToString();
                    }
                }
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return String.Empty;
        }

        public virtual String GetQuotesForParameter(PropertyInfo objPI)
        {
            try
            {
                String TypeName = objPI.PropertyType.Name;
                if (TypeName == typeof(String).Name || TypeName == typeof(DateTime).Name)
                {
                    return "'";
                }
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return String.Empty;
        }
    }
}
