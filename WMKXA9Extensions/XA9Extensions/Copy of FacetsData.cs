//FacetsData.vb
using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace XA9Extensions
{
    /// <summary>
    /// A class to parse and manipulate XML data sent to and from Facets
    /// </summary>
    /// <remarks></remarks>
    public class FacetsData1234
    {

        private XElement _facetsData;

        /// <summary>
        /// Creates a new instance based on xml data received from Facets
        /// </summary>
        /// <param name="xmlFromFacets">Data returned by GetData or equivalent method</param>
        /// <remarks></remarks>

        public FacetsData(string xmlFromFacets)
        {
            this._facetsData = XElement.Parse(xmlFromFacets);

        }


        /// <summary>
        /// Value of the first occurence of Facets item for a given Item ID within an "implied" collection
        /// </summary>
        /// <param name="itemID"></param>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Only applies to "implied" collection (no collection node)</remarks>
        public string this[string itemID]
        {
            get
            {
                return
                    (from col in this._facetsData.Elements("Column")
                     where (string)col.Attribute("name") == itemID
                     select col
                     )
                     .FirstOrDefault().Value;
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        /// <summary>
        /// Value of the first occurence of Facets item for a given Item ID within a given collection
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="itemID"></param>
        /// <value></value>
        /// <returns></returns>
        /// <remarks>Only applies to single valued data</remarks>
        /*public string this[string dataID, string itemID]
        {
            get
            {
                return this[dataID, itemID];
            }
            set
            {
                this[dataID, itemID] = value;
            }
        }*/


        /// <summary>
        /// Value of the specified occurence of Facets item for a given Item ID within a given collection
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="itemID"></param>
        /// <param name="seqNo">Occurence (0-based)</param>
        /// <value>Value to set</value>
        /// <returns></returns>
        /// <remarks>Nothing happens if attempting to set a non-existing element</remarks>
        public string this[string collectionID, string itemID]
        {
            //Applies to both multiple collection nodes as well as subcollections within collections
            get
            {
                return
                    (from col in
                         (from coll in this._facetsData.Elements("Collection") where (string)coll.Attribute("name") == dataID select coll)
                             .Descendants("Column")
                     where (string)col.Attribute("name") == itemID
                     select col).FirstOrDefault().Value;
                     //select col).Skip(seqNo).FirstOrDefault().Value;
            }
            set
            {
                //var elem = (from col in (from coll in this._facetsData.Elements("Collection") where (string)coll.Attribute("name") == dataID select coll).Descendants("Column") where (string)col.Attribute("name") == itemID select col).Skip(seqNo).FirstOrDefault();
                var elem = (from col in (from coll in this._facetsData.Elements("Collection") where (string)coll.Attribute("name") == dataID select coll).Descendants("Column") where (string)col.Attribute("name") == itemID select col).FirstOrDefault();
                if (elem != null)
                {
                    elem.SetValue(value);
                }
            }
        }

        /// <summary>
        /// Value of the specified occurence of Facets item for a given Item ID within a given collection
        /// </summary>
        /// <param name="dataID"></param>
        /// <param name="itemID"></param>
        /// <param name="seqNo">Occurence (0-based)</param>
        /// <value>Value to set</value>
        /// <returns></returns>
        /// <remarks>Nothing happens if attempting to set a non-existing element</remarks>
        public string this[string collectionID, string itemID]
        {
            //Applies to both multiple collection nodes as well as subcollections within collections
            get
            {
                return
                    (from col in
                         (from coll in this._facetsData.Elements("Collection") where (string)coll.Attribute("name") == dataID select coll)
                             .Descendants("Column")
                     where (string)col.Attribute("name") == itemID
                     select col).FirstOrDefault().Value;
                //select col).Skip(seqNo).FirstOrDefault().Value;
            }
            set
            {
                //var elem = (from col in (from coll in this._facetsData.Elements("Collection") where (string)coll.Attribute("name") == dataID select coll).Descendants("Column") where (string)col.Attribute("name") == itemID select col).Skip(seqNo).FirstOrDefault();
                var elem = (from col in (from coll in this._facetsData.Elements("Collection") where (string)coll.Attribute("name") == dataID select coll).Descendants("Column") where (string)col.Attribute("name") == itemID select col).FirstOrDefault();
                if (elem != null)
                {
                    elem.SetValue(value);
                }
            }
        }


        /// <summary>
        /// Value of (a first occurence of) an item correlated to a given key item+value
        /// </summary>
        /// <param name="dataID">Name of data collection, e.g. DATA</param>
        /// <param name="itemID">Name of column to return, e.g. CODE_DESCR_TXT</param>
        /// <param name="keyID">Name of key column , e.g. CODE_VALUE_CD</param>
        /// <param name="keyVal">Value of the key column, e.g. LoggingLevel</param> = 
        /// <value></value>
        /// <returns>Value of the column correlated to the key column (in the same (sub)collection instance)</returns>
        /// <remarks></remarks>
        internal string this[string dataID, string itemID, string keyID, string keyVal]
        {

            get
            {
                foreach (var row in this.GetRows(dataID))
                {
                    if (row.GetItem(keyID) == keyVal)
                    {
                        return row.GetItem(itemID);
                    }
                }

                //no match
                return null;

            }
        }

        /// <summary>
        /// Returns a collection of (Sub)Collections, i.e. rows for a given Data ID
        /// </summary>
        /// <param name="dataID"></param>
        /// <returns></returns>
        /// <remarks>Intended for use with GetItemFromRow method</remarks>
        internal IEnumerable<XElement> GetRows(string dataID)
        {
            var rows = from coll in this._facetsData.Descendants("SubCollection") where (string)coll.Attribute("name") == dataID select coll;
            if (rows.Count() == 0)
            {
                //No SubCollection nodes, it could be a collection of Collection nodes
                // however, do not return an element that is an emply Collection node
                rows = from coll in this._facetsData.Elements("Collection") where (string)coll.Attribute("name") == dataID && coll.Elements().Any() select coll;
            }

            return rows;

        }


        /// <summary>
        /// Returns (a first occurence of) an item for a given Item ID from the submitted row
        /// </summary>
        /// <param name="row"></param>
        /// <param name="itemID"></param>
        /// <returns></returns>
        /// <remarks>Use the GetItem extension method instead</remarks>
        static internal string GetItemFromRow(XElement row, string itemID)
        {
            return
                (from col in row.Elements("Column") where (string)col.Attribute("name") == itemID select col)
                .FirstOrDefault().Value;

        }


        /// <summary>
        /// Returns  the entire XML contents of the current state of the object
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        internal string ToXml()
        {
            return this._facetsData.ToString();
        }

        /// <summary>
        /// Returns the XML contents for a single data type
        /// </summary>
        /// <param name="dataID">DataID (i.e. value of the name attribute) of the Collection node(s) to return</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal string ToXml(string dataID)
        {
            var nodeToReturn = new XElement("FacetsData");
            nodeToReturn.Add(from coll in this._facetsData.Elements("Collection") where (string)coll.Attribute("name") == dataID select coll);
            return nodeToReturn.ToString();
        }

        /// <summary>
        /// Insert a new Facets data collection on top or bottom of FacetsData
        /// </summary>
        /// <param name="item">Element to add</param>
        /// <param name="pos">Position to add - Top or Bottom</param>
        public void AddSingleCollection(string item, string pos)
        {
            if (this._facetsData ==  null)
            {
                throw new InvalidOperationException(XA9Constants.MSG_EXTN_ERR_INTLZ);
            }
            XElement elementToAdd = XElement.Parse(item);
            if (pos == XA9Constants.TOP)
            {
                this._facetsData.FirstNode.AddBeforeSelf(elementToAdd);
            }
            else if (pos == XA9Constants.END)
            {
                this._facetsData.LastNode.AddAfterSelf(elementToAdd);
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
            if (this._facetsData == null)
            {
                throw new InvalidOperationException(XA9Constants.MSG_EXTN_ERR_INTLZ);
            }
            var elem = (from col in (from coll in this._facetsData.Elements("Collection") where (string)coll.Attribute("name") == dataID select coll) select col).Single();
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

                this._facetsData.Save(strFileNameWithPath);
                isDataSaved = true;
            }
            catch (Exception ex)
            {

            }
            return isDataSaved;
        }
    }

    internal static class XElementExtensions
    {
        internal static string GetItem(this XElement row, string itemID)
        {
            return FacetsData.GetItemFromRow(row, itemID);
        }
    }
    //FacetsData

    /// <summary>
    /// Extension methods to facilitate easier syntax
    /// </summary>
    /// <remarks></remarks>
    /*internal class XElementExtensions
    {

        /// <summary>
        /// Returns (a first occurence of) an item for a given Item ID
        /// </summary>
        /// <remarks></remarks>
	
        internal string GetItem(this XElement row, string itemID)
        {
            return FacetsData.GetItemFromRow(row, itemID);
        }

    }*/
}