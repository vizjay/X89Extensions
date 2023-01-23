/*************************************************************************************************************************************************
 *  Class               : ClaimLine
 *  Description         : Stores CDMLALL line data fields and associated records
 *  Used by             : 
 *  Author              : TriZetto Inc.
 *************************************************************************************************************************************************
 * Ver                  Date                    Modification Description
 * 1.0                  05/01/2016              Initial creation (Viswan Jayaraman)
 * 1.1                  05/20/2016              Revised by Wen-Man Liu; added new methods
 * 1.2                  06/15/2016              Revised by Wen-Man Liu; added new methods
 *************************************************************************************************************************************************
 */
//FacetsData.vb
using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using ErCoreIfcExtensionData;
using XA9Extensions.Utilities;

namespace XA9Extensions.BusinessLayer
{
    /// <summary>
    /// A class to parse and manipulate XML data sent to and from Facets
    /// </summary>
    /// <remarks></remarks>
    public class FacetsData
    {

        private XDocument _facetsXml;
        private static FacetsData _instance;
        private IFaExtensionData _extensionDataObject;
        private static object syncRoot = new object();

        /// <summary>
        /// Creates a new instance based on xml data received from Facets
        /// </summary>
        /// <param name="xmlFromFacets">Data returned by GetData or equivalent method</param>
        /// <remarks></remarks>

        /// <summary>
        /// static ExtensionData Instance
        /// </summary>
        public static FacetsData FacetsInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new FacetsData();
                    }
                }
                return _instance;
            }
        }
        /// <summary>
        /// Facets Extension Data Object 
        /// </summary>
        public IFaExtensionData ExtensionDataObject
        {
            get { return _extensionDataObject; }
            set { _extensionDataObject = value; }
        }

        /// <summary>
        /// Holds the Facets XML data retrieved through GetData("")
        /// </summary>
        public XDocument FacetsXml
        {
            get { return _facetsXml; }
            set { _facetsXml = value; }

        }

        /// <summary>
        /// Initializes the class by loading information from the Facets extension data object.
        /// </summary>
        /// <param name="extensionData">The Facets extension data object.</param>
        public void Initialize(IFaExtensionData extensionData)
        {

            _instance = new FacetsData();
            _instance._extensionDataObject = extensionData;
            string _extensionDataXml = extensionData.GetData(string.Empty).Replace(Environment.NewLine, string.Empty);
            //_SqlErrMsg = string.Empty;
            _instance._facetsXml = XDocument.Parse(_extensionDataXml);
            //Logger.LoggerInstance.ReportMessage("XML from Facets is  : ", _extensionDataXml);

        }

        /// <summary>
        /// Finalize the transaction by setting data back to Facets
        /// </summary>
        public void CompleteProcess()
        {
            this._extensionDataObject.SetData(string.Empty, _instance._facetsXml.ToString());
            Logger.LoggerInstance.ReportMessage("Final XML at " + ContextData.ContextInstance.ExitTiming + " is  ", _instance._facetsXml.ToString());
            Logger.LoggerInstance.ReportMessage("---------------------------------------------COMPLETED AT ",  ContextData.ContextInstance.ExitTiming + "------------------------------------------------");

            _instance = null;
        }

        
        /// <summary>
        /// Gets a single value for the specific node in the XML.
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="itemID"></param>
        /// <param name="convertNullStringToNull">A flag that determines if nulls should be converted to empty strings</param>
        /// <returns>The value for the specific node in the XML</returns>
        public string GetSingleDataItem(string dataID, string itemID, bool convertNullStringToNull)
        {

            XElement currentNode;
            if (string.IsNullOrEmpty(dataID))
            {
                //"FacetsData/Column[@name='" + itemID + "']"
                currentNode = this._facetsXml.XPathSelectElement(string.Format(XA9Constants.XML_COLLUMN_XPATH, itemID)); //ver 1.1
            }
            else
            {
                //"FacetsData/Collection[@name='" + dataID + "']//Column[@name='" + itemID + "']"
                currentNode = this._facetsXml.XPathSelectElement(string.Format(XA9Constants.XML_COLLECTION_COLUMN_XPATH, dataID, itemID)); //ver 1.1
            }

            if (currentNode == null || currentNode.Value.Equals(string.Empty))
                return (convertNullStringToNull ? null : string.Empty);
            else
                return ((currentNode.Value.Equals(XA9Constants.STR_NULL) || string.IsNullOrEmpty(currentNode.Value))
                         && convertNullStringToNull ? null : currentNode.Value);
        }

       
        /// <summary>
        /// Gets data in facet extension data for any item in subcollection; overloading GetSingleDataItem
        /// This method throws exception when looking for a non existant element.
        /// </summary>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="subCollectionName">The sub-collection name.</param>
        /// <param name="itemId">The item Id.</param>
        /// <param name="row">The row position.</param>
        public string GetSingleDataItem(string collectionName, string subCollectionName, string itemId, int intIndex)
        {
            string itemValue = string.Empty;
            //"FacetsData/Collection[@name='" + collectionName + "']//SubCollection[@name='" + subCollectionName + "']"
            try
            {
                itemValue = this._facetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, collectionName, subCollectionName))
                                    .Select((v, i) => new { element = v, index = i })
                                    .FirstOrDefault(item => item.index == intIndex)
                                    .element.Descendants().FirstOrDefault(e => e.Attribute("name").Value == itemId).Value;
            }
            catch (Exception ex)
            {

            }
            return itemValue;
        }

        /// <summary>
        /// Get single data item from DB return result
        /// </summary>
        /// <param name="rtnXml"></param>
        /// <param name="dataID"></param>
        /// <param name="itemID"></param>
        /// <param name="convertNullStringToNull"></param>
        /// <returns></returns>
        public string GetDbSingleDataItem(string rtnXml, string dataId, string itemId, bool convertNullStringToNull)
        {

            XDocument xmlData = XDocument.Parse(rtnXml);
            XElement currentNode;
            if (string.IsNullOrEmpty(dataId))
            {
                //"FacetsData/Column[@name='" + itemID + "']"
                currentNode = xmlData.XPathSelectElement(string.Format(XA9Constants.XML_COLLUMN_XPATH, itemId));
            }
            else
            {
                //"FacetsData/Collection[@name='" + dataID + "']//Column[@name='" + itemID + "']"
                currentNode = xmlData.XPathSelectElement(string.Format(XA9Constants.XML_COLLECTION_COLUMN_XPATH, dataId, itemId));
            }

            if (currentNode == null || currentNode.Value.Equals(string.Empty))
                return (convertNullStringToNull ? null : string.Empty);
            else
                return ((currentNode.Value.Equals(XA9Constants.STR_NULL) || string.IsNullOrEmpty(currentNode.Value))
                         && convertNullStringToNull ? null : currentNode.Value);
        }

        /// <summary>
        /// Get multiple data items from DB return result
        /// </summary>
        /// <param name="rtnXml"></param>
        /// <param name="dataID"></param>
        /// <param name="itemID"></param>
        /// <param name="convertNullStringToNull"></param>
        /// <returns></returns>
        public List<string> GetDbMultipleDataItem(string rtnXml, string dataId, string itemId, bool convertNullStringToNull)
        {
            List<string> lstValues = new List<string>();

            XDocument xmlData = XDocument.Parse(rtnXml);
            List<XElement> currentNodes;
            if (string.IsNullOrEmpty(dataId))
            {
                //"FacetsData/Column[@name='" + itemID + "']"
                currentNodes = xmlData.XPathSelectElements(string.Format(XA9Constants.XML_COLLUMN_XPATH, itemId)).ToList();
            }
            else
            {
                //"FacetsData/Collection[@name='" + dataID + "']//Column[@name='" + itemID + "']"
                //currentNode = xmlData.XPathSelectElement(string.Format(XA9Constants.XML_COLLECTION_COLUMN_XPATH, dataId, itemId));
                currentNodes = xmlData.XPathSelectElements(string.Format(XA9Constants.XML_COLLECTION_COLUMN_XPATH, dataId, itemId)).ToList();
            }


            if (currentNodes == null || currentNodes.Count == 0)
                return (convertNullStringToNull ? null : lstValues);

            /*if (currentNode == null || currentNode.Value.Equals(string.Empty))
                return (convertNullStringToNull ? null : string.Empty);*/

            else
                currentNodes.ForEach(elm => lstValues.Add(elm.Value));

            return lstValues;
        }

        /// <summary>
        /// Returns collection of XElement based on CollectionName And / Or Sub Collection name from the input xml source
        /// This method throws exception when looking for a non existant element.
        /// </summary>
        /// <param name="rtnXml">Source XML string</param>
        /// <param name="collectionName">Collection Name</param>
        /// <param name="subCollectionName">Sub Collection Name</param>
        /// <returns>List<XElement></returns>
        public List<XElement> GetMultipleDataElements(string rtnXml,string collectionName, string subCollectionName)
        {
            List<XElement> elements = null;
            string itemValue = string.Empty;
            XDocument xdocSource = null;

            try
            {
                xdocSource = XDocument.Parse(rtnXml);
                if (!subCollectionName.Equals(string.Empty))
                {
                    //"FacetsData/Collection[@name='" + collectionName + "']//SubCollection[@name='" + subCollectionName + "']"
                    elements = xdocSource.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, collectionName, subCollectionName)).ToList();
                }
                else
                {
                    //FacetsData/Collection[@name='{0}']"
                    elements = xdocSource.XPathSelectElements(string.Format(XA9Constants.XML_COLLECTION_XPATH, collectionName)).ToList();
                }
            
            }
            catch (Exception ex)
            {

            }
            return elements;
        }

        /// <summary>
        /// Returns the sub collection list from a collection
        /// This method throws exception when looking for a non existant element.
        /// </summary>
        /// <param name="collectionName">The collection name.</param>
        /// <param name="subCollectionName">The sub-collection name.</param>
        /// <param name="itemId">The item Id.</param>
        /// <param name="row">The row position.</param>
        public List<XElement> GetMultipleDataElements(string collectionName, string subCollectionName)
        {
            List<XElement> elements = null;
            string itemValue = string.Empty;
            
            try
            {
                if (!subCollectionName.Equals(string.Empty))
                {
                    //"FacetsData/Collection[@name='" + collectionName + "']//SubCollection[@name='" + subCollectionName + "']"
                    elements = this._facetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, collectionName, subCollectionName)).ToList();
                }
                else
                {
                    //FacetsData/Collection[@name='{0}']"
                    elements = this._facetsXml.XPathSelectElements(string.Format(XA9Constants.XML_COLLECTION_XPATH, collectionName)).ToList();
                }
                
            }
            catch (Exception ex)
            {

            }
            return elements;
        }


        /// <summary>
        /// Insert a new Facets data collection on top or bottom of FacetsData
        /// </summary>
        /// <param name="item">Element to add</param>
        /// <param name="pos">Position to add - Top or Bottom</param>
        public void AddSingleCollection(string item, string pos)
        {
            if (this._facetsXml == null)
            {
                throw new InvalidOperationException(XA9Constants.MSG_EXTN_ERR_INTLZ);
            }
            XElement elementToAdd = XElement.Parse(item);
            if (pos == XA9Constants.TOP)
            {
                //this._facetsXml.FirstNode.AddBeforeSelf(elementToAdd);
                this._facetsXml.Descendants().First().AddFirst(elementToAdd);
            }
            else if (pos == XA9Constants.END)
            {
                this._facetsXml.LastNode.AddAfterSelf(elementToAdd);
            }
        }

        /// <summary>
        /// Adds a Sub Collection to the end of the collection object based on the Data ID
        /// </summary>
        /// <example>If dataID = "CDORALL" then this method adds the xml element contained in parameter item as the last element of the collection</example>
        /// <param name="item"></param>
        /// <param name="pos"></param>
        public void AddSingleSubCollection(string dataID, string item)
        {
            if (this._facetsXml == null)
            {
                throw new InvalidOperationException(XA9Constants.MSG_EXTN_ERR_INTLZ);
            }
            var elem = (from col in (from coll in this._facetsXml.Descendants("Collection") where (string)coll.Attribute("name") == dataID select coll) select col).Single();
            elem.Add(XElement.Parse(item));


        }

        /// <summary>
        /// Save the xml somewhere
        /// </summary>
        /// <param name="strFileNameWithPath"></param>
        /// <returns></returns>
        public bool Save(string strFileNameWithPath)
        {
            bool isDataSaved = false;
            try
            {
                this._facetsXml.Save(strFileNameWithPath);
                isDataSaved = true;
            }
            catch (Exception ex)
            {

            }
            return isDataSaved;
        }

        /// <summary>
        /// Check returned sql result to see the data is available or not, Populate sql error message if any
        /// </summary>
        /// <param name="pstrXml"></param>
        /// <returns></returns>
        public bool IsDbDataAvailable(string pstrXml)
        {
            bool _blRtn = true;
            
           
            try
            {
                //No data or data error
                if (string.IsNullOrEmpty(pstrXml) || pstrXml.Trim().Replace("<FacetsData></FacetsData>",string.Empty).Equals(string.Empty))
                { 
                    _blRtn = false; 
                }
                else
                {
                    string _RETURN_CODE = GetDbSingleDataItem(pstrXml, string.Empty, "RETURN_CODE", false);
                    string _SQL_ERROR_CODE = GetDbSingleDataItem(pstrXml, string.Empty, "SQL_ERROR_CODE", false);
                    string _NUM_RESULTS = GetDbSingleDataItem(pstrXml, string.Empty, "NUM_RESULTS", false);
                    string _ERROR_MESSAGE = GetDbSingleDataItem(pstrXml, string.Empty, "ERROR_MESSAGE", false);
                    string _SQL_ERROR_MESSAGE = GetDbSingleDataItem(pstrXml, string.Empty, "SQL_ERROR_MESSAGE", false);

                    if (_NUM_RESULTS == "0")
                    {
                        _blRtn = false;
                        if (_RETURN_CODE != "0" && _SQL_ERROR_CODE != "0" &&
                            (!string.IsNullOrEmpty(_ERROR_MESSAGE) || !string.IsNullOrEmpty(_SQL_ERROR_MESSAGE)))
                        {
                            //_SqlErrMsg += "SQL_ERROR_CODE: " + _SQL_ERROR_CODE + ". ERROR_MESSAGE: " + _ERROR_MESSAGE + " " + _SQL_ERROR_MESSAGE;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _blRtn = false;
            }
            return _blRtn;
        }

        //v1.2
        /// <summary>
        /// Check and return error from GetDbRequest call
        /// </summary>
        /// <param name="pstrXml"></param>
        /// <returns></returns>
        public string CheckDbError(string pstrXml)
        {
            string _errMsg = "CheckDbError: ";

            try
            {
                //No data or data error
                if (string.IsNullOrEmpty(pstrXml) || pstrXml.Trim().Replace("<FacetsData></FacetsData>", string.Empty).Equals(string.Empty))
                {
                    return "";
                }
                else
                {
                    string _RETURN_CODE = GetDbSingleDataItem(pstrXml, string.Empty, "RETURN_CODE", false);
                    string _SQL_ERROR_CODE = GetDbSingleDataItem(pstrXml, string.Empty, "SQL_ERROR_CODE", false);
                    string _NUM_RESULTS = GetDbSingleDataItem(pstrXml, string.Empty, "NUM_RESULTS", false);
                    string _ERROR_MESSAGE = GetDbSingleDataItem(pstrXml, string.Empty, "ERROR_MESSAGE", false);
                    string _SQL_ERROR_MESSAGE = GetDbSingleDataItem(pstrXml, string.Empty, "SQL_ERROR_MESSAGE", false);

                    if ((_RETURN_CODE == "0" || _RETURN_CODE == "-1") && _SQL_ERROR_CODE == "0")
                    {
                        _errMsg = "";
                    }
                    else if (!string.IsNullOrEmpty(_ERROR_MESSAGE) && _ERROR_MESSAGE != "mcsErrorMsg")
                    {
                        _errMsg = _errMsg + _ERROR_MESSAGE + " " + _SQL_ERROR_MESSAGE + " ";
                    }
                }
            }
            catch (Exception ex)
            {
                _errMsg = ex.Message;
            }

            return _errMsg;
        }

        //v1.1
        /// <summary>
        /// Updates Facets runtime xml data; single node
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="itemID"></param>
        /// <param name="data"></param>
        public void SetSingleDataItem(string dataID, string itemID, string data)
        {

            XElement currentNode;
            if (string.IsNullOrEmpty(dataID))
            {
                //"FacetsData/Column[@name='" + itemID + "']"
                //currentNode = this._facetsXml.XPathSelectElement(string.Format(XA9Constants.XML_FAC_COLLUMN_XPATH, itemID));
                currentNode = this._facetsXml.XPathSelectElement(string.Format(XA9Constants.XML_COLLUMN_XPATH, itemID));
            }
            else
            {
                //"FacetsData/Collection[@name='" + dataID + "']//Column[@name='" + itemID + "']"
                //currentNode = this._facetsXml.XPathSelectElement(string.Format(XA9Constants.XML_FAC_COLLECTION_COLUMN_XPATH, dataID, itemID));
                currentNode = this._facetsXml.XPathSelectElement(string.Format(XA9Constants.XML_COLLECTION_COLUMN_XPATH, dataID, itemID));
            }

            //Update
            if (currentNode != null)
            {
                currentNode.Value = data;
            }
        }

        //v1.2
        /// <summary>
        /// Updates Facets runtime xml data; update single node item in subcollection
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="subID"></param>
        /// <param name="itemID"></param>
        /// <param name="data"></param>
        /// <param name="row"></param>
        public void SetSingleDataItem(string collectionName, string subCollectionName, string itemID, string data, int row)
        {

            List<XElement> elements = null;
            if (string.IsNullOrEmpty(subCollectionName))
            {
                //"FacetsData/Collection[@name='" + dataID + "']//Column[@name='" + itemID + "']"
                elements = this._facetsXml.XPathSelectElements(string.Format(XA9Constants.XML_COLLECTION_COLUMN_XPATH, collectionName, itemID)).ToList();
            }
            else
            {
                //"FacetsData/Collection[@name='" + dataID + "']//SubCollection[@name='" + subID + "']//Column[@name='" + itemID + "']"
                elements = this._facetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_COLUMN_XPATH, collectionName, subCollectionName, itemID)).ToList();
            }

            //Update
            if (elements != null)
            {
                elements[row].Value = data;
            }
        }
        //CR-128 call to hold CDOR SEQ number and CDOR value
        public class ItemInfo
        {
            private String _ID = String.Empty;
            private String _Value = String.Empty;

            public String ID
            {
                get { return _ID; }
                set { _ID = value; }
            }

            public String Value
            {
                get { return _Value; }
                set { _Value = value; }
            }

            public ItemInfo()
            {
                ID = String.Empty;
                Value = String.Empty;
            }

            public ItemInfo(String ID, String Value = "")
            {
                this.ID = ID;
                this.Value = Value;
            }
        }

        //CR-128 call to search CDOR SEQ number and CDOR value in existing collection and sub collection and updating XML in the end
        public void SetSingleDataItem(string collectionName, string subCollectionName, List<ItemInfo> objListSearch, List<ItemInfo> objListTarget)
        {
            List<XElement> objXE_SubCollections = null;

            objXE_SubCollections = this._facetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, collectionName, subCollectionName)).ToList();

           

            if (objXE_SubCollections != null)
            {
                for (Int32 i = 0; i < objXE_SubCollections.Count; i++)
                {
                    Boolean IsFound = false;
                    Boolean IsContinue = true;
                    for (Int32 j = 0; j < objListSearch.Count; j++)
                    {
                        if (IsContinue)
                        {
                            IsContinue = false;
                            XElement objXE_Column = objXE_SubCollections[i].XPathSelectElement(string.Format(XA9Constants.XML_COLUMN_XPATH, objListSearch[j].ID));
                            //if (objXE_Column == null)
                            //{
                            //    IsFound = false;
                            //    break;
                            //}

                            if (objXE_Column != null)
                            {
                                if (objXE_Column.Value == objListSearch[j].Value)
                                {
                                    IsContinue = true;
                                    if (j == objListSearch.Count - 1)
                                    {
                                        IsFound = true;
                                    }
                                }
                            }
                        }
                    }

                    if (IsFound)
                    {

                        foreach (ItemInfo objTarget in objListTarget)
                        {
                            XElement objXE_Column = objXE_SubCollections[i].XPathSelectElement(string.Format(XA9Constants.XML_COLUMN_XPATH, objTarget.ID));
                            if (objXE_Column != null)
                            {
                                objXE_Column.Value = objTarget.Value;
                            }
                        }
                        break;
                    }
                }
            }

        }
        public bool RemoveSingleDataParentElementFromSubCollection(string collectionName, string subCollectionName, string strCol1Value)
        {
            bool blnRemoved = false;


            return blnRemoved;
        }
        public bool RemoveOneParentElementFromSubCollection(string collectionName, string subCollectionName, string strCol1Name, string strCol1Value, string strCol2Name, string strCol2Value)
        {
            bool blnRemoved = false;

            try
            {
                var element = this._facetsXml.XPathSelectElement(string.Format(XA9Constants.XML_SUB_COLLECTION_COLUMN_XPATH_TWO_COLS, collectionName, subCollectionName, strCol1Name, strCol1Value, strCol2Name, strCol2Value)).Parent;
                if (element != null)
                {
                    element.Remove();
                    blnRemoved = true;
                }

            }
            catch (Exception ex)
            {

            }
            return blnRemoved;
        }

        public bool RemoveSingleDataParentElementFromSubCollection(string collectionName, string subCollectionName, string strCol1Value, string strCol2Value, string strCol3Value)
        {
            bool blnRemoved = false;

            return blnRemoved;
        }

        //v1.2
        /// <summary>
        /// Updates Facets runtime xml data; delete the whole collection/subcollection
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="subCollectionName"></param>
        /// <param name="row"></param>
        public void RemoveSingleDataCollection(string collectionName, string subCollectionName, int row)
        {

            List<XElement> elements = null;

            try
            {
                if (!subCollectionName.Equals(string.Empty))
                {
                    //"FacetsData/Collection[@name='" + collectionName + "']//SubCollection[@name='" + subCollectionName + "']"
                    elements = this._facetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, collectionName, subCollectionName)).ToList();
                }
                else
                {
                    //FacetsData/Collection[@name='{0}']"
                    elements = this._facetsXml.XPathSelectElements(string.Format(XA9Constants.XML_COLLECTION_XPATH, collectionName)).ToList();
                }

            }
            catch (Exception ex)
            {

            }

            if (elements != null)
            {
                elements[row].Remove();
                //elements[row].RemoveAll();
            }
        }

        //v1.2
        /// <summary>
        /// Updates Facets runtime xml data; delete the whole collection/subcollection
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="subCollectionName"></param>
        /// <param name="row"></param>
        public void RemoveSingleDataCollection(string collectionName, string subCollectionName)
        {

            List<XElement> elements = null;

            try
            {
                if (!subCollectionName.Equals(string.Empty))
                {
                    //"FacetsData/Collection[@name='" + collectionName + "']//SubCollection[@name='" + subCollectionName + "']"
                    elements = this._facetsXml.XPathSelectElements(string.Format(XA9Constants.XML_SUB_COLLECTION_XPATH, collectionName, subCollectionName)).ToList();
                }
                else
                {
                    //FacetsData/Collection[@name='{0}']"
                    elements = this._facetsXml.XPathSelectElements(string.Format(XA9Constants.XML_COLLECTION_XPATH, collectionName)).ToList();
                }

            }
            catch (Exception ex)
            {

            }

            if (elements != null)
            {
                elements.Remove();
                //elements[row].RemoveAll();
            }
        }
    }

}