using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace CommonUtilities
{
    public class DataExtraction
    {
        public DataExtraction()
        {
        }

        public virtual List<T> GetRecordsList<T>(String strXMLData, String XPathDataID = Constants.DefaultValues.String, Boolean IsXPath = false, Boolean PropertyMatchFromDescription = false, Boolean IgnoreCaseForPropertyMatch = false, Boolean IsHexData = false, String SubNodeName = "Column", String SubNodeAttributeName = "name")
        {
            try
            {
                if (strXMLData == null)
                {
                    strXMLData = String.Empty;
                }
                strXMLData = strXMLData.Trim();

                if (!String.IsNullOrEmpty(strXMLData) && strXMLData.Trim().StartsWith("<") && strXMLData.Trim().EndsWith(">"))
                {
                    Conversion objCE = new Conversion();
                    ObjectEx objOE = new ObjectEx();
                    Type objType = typeof(T);
                    List<T> objList = new List<T>();
                    List<PropertyInfo> objListPI = objType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).ToList();

                    XmlDocument objXD = new XmlDocument();
                    objXD.LoadXml(strXMLData);

                    if (objXD != null)
                    {
                        XmlNodeList objXNL = null;
                        if (IsXPath && !String.IsNullOrEmpty(XPathDataID))
                        {
                            objXNL = objXD.SelectNodes(XPathDataID);
                        }
                        else if (!String.IsNullOrEmpty(XPathDataID))
                        {
                            objXNL = objXD.SelectNodes(String.Format("FacetsData/Collection[@name='{0}']", XPathDataID));
                        }
                        else
                        {
                            objXNL = objXD.SelectNodes("FacetsData");
                        }

                        foreach (XmlNode objXNRow in objXNL)
                        {
                            Boolean IsAddCurrentObject = false;
                            var objMain = objOE.CreateObjectInstance(objType);
                            if (objMain != null)
                            {
                                XmlNodeList objXNLColumn = objXNRow.SelectNodes(SubNodeName);
                                foreach (XmlElement objXEColumn in objXNLColumn)
                                {
                                    String AttributeValue = objXEColumn.GetAttribute(SubNodeAttributeName);
                                    String NodeValue = objXEColumn.InnerText.TrimEnd();
                                    if (NodeValue.ToUpper().Contains("NULL"))
                                    {
                                        NodeValue = String.Empty;
                                    }

                                    if (IsHexData)
                                    {
                                        String strText = objCE.ToText(NodeValue);
                                        if (!String.IsNullOrEmpty(strText))
                                        {
                                            NodeValue = strText;
                                        }
                                    }

                                    PropertyInfo objPI = objListPI.Find(obj => ((obj.Name == AttributeValue) || (PropertyMatchFromDescription && new Enumerations().GetDescriptionFromProperty(obj) == AttributeValue) || (IgnoreCaseForPropertyMatch && obj.Name.ToUpper() == AttributeValue.ToUpper())));
                                    if (objPI != null)
                                    {
                                        objOE.SetValueUsingPI(objPI, NodeValue, ref objMain);
                                        IsAddCurrentObject = true;
                                    }
                                }
                                if (IsAddCurrentObject)
                                {
                                    objList.Add((T)objMain);
                                }
                            }
                        }
                    }
                    return objList;
                }
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return new List<T>();
        }

    }
}
