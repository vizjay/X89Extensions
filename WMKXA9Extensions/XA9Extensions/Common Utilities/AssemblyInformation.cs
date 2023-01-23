using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommonUtilities
{
    public class AssemblyInformation
    {
        public static Assembly Assembly
        {
            get { return Assembly.GetExecutingAssembly(); }
        }

        public static String AssemblyTitle
        {
            get
            {
                try
                {
                    AssemblyTitleAttribute objAttribute = (AssemblyTitleAttribute)AssemblyTitleAttribute.GetCustomAttribute(Assembly, typeof(AssemblyTitleAttribute));
                    if (objAttribute != null)
                    {
                        return objAttribute.Title;
                    }
                }
                catch
                {
                }
                return String.Empty;
            }
        }

        public static String AssemblyDescription
        {
            get
            {
                try
                {
                    AssemblyDescriptionAttribute objAttribute = (AssemblyDescriptionAttribute)AssemblyDescriptionAttribute.GetCustomAttribute(Assembly, typeof(AssemblyDescriptionAttribute));
                    if (objAttribute != null)
                    {
                        return objAttribute.Description;
                    }
                }
                catch
                {
                }
                return String.Empty;
            }
        }

        public static String AssemblyCompany
        {
            get
            {
                try
                {
                    AssemblyCompanyAttribute objAttribute = (AssemblyCompanyAttribute)AssemblyCompanyAttribute.GetCustomAttribute(Assembly, typeof(AssemblyCompanyAttribute));
                    if (objAttribute != null)
                    {
                        return objAttribute.Company;
                    }
                }
                catch
                {
                }
                return String.Empty;
            }
        }

        public static String AssemblyProduct
        {
            get
            {
                try
                {
                    AssemblyProductAttribute objAttribute = (AssemblyProductAttribute)AssemblyProductAttribute.GetCustomAttribute(Assembly, typeof(AssemblyProductAttribute));
                    if (objAttribute != null)
                    {
                        return objAttribute.Product;
                    }
                }
                catch
                {
                }
                return String.Empty;
            }
        }

        public static String AssemblyCopyright
        {
            get
            {
                try
                {
                    AssemblyCopyrightAttribute objAttribute = (AssemblyCopyrightAttribute)AssemblyCopyrightAttribute.GetCustomAttribute(Assembly, typeof(AssemblyCopyrightAttribute));
                    if (objAttribute != null)
                    {
                        return objAttribute.Copyright;
                    }
                }
                catch
                {
                }
                return String.Empty;
            }
        }

        public static String AssemblyTrademark
        {
            get
            {
                try
                {
                    AssemblyTrademarkAttribute objAttribute = (AssemblyTrademarkAttribute)AssemblyTrademarkAttribute.GetCustomAttribute(Assembly, typeof(AssemblyTrademarkAttribute));
                    if (objAttribute != null)
                    {
                        return objAttribute.Trademark;
                    }
                }
                catch
                {
                }
                return String.Empty;
            }
        }

        public static String AssemblyVersion
        {
            get
            {
                Version Version = Assembly.GetName().Version;
                if (Version != null)
                {
                    return Version.ToString();
                }
                else
                {
                    return "1.0.0.0";
                }
            }
        }
    }
}
