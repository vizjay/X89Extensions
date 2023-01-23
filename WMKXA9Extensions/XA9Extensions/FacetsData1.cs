using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace XA9Extensions
{
    public class FacetsData1
    {
        private XElement _facetsData;

        /// <summary>
        /// Creates a new instance based on xml data received from Facets
        /// </summary>
        /// <param name="xmlFromFacets">Data returned by GetData or equivalent method</param>
        /// <remarks></remarks>

        internal FacetsData1(string xmlFromFacets)
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
        /*internal string Item (string itemID)
        {
            string value = string.Empty;
            try
            {
                value = 
                 (from col in this._facetsData.Elements("Column")
                  where (string)col.Attribute("name") == itemID
                  select col)
                        .FirstOrDefault().Value;

            }
            catch (Exception ex)
            {
                value = string.Empty;
            }
            return value;
        }*/

        public Item this[string itemID]
        {
            get
            {
                return "ss";
            }

        }
    } // end of class FacetsData
}
