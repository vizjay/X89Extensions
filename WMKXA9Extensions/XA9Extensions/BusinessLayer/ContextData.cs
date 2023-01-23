/*************************************************************************************************************************************************
 *  Class               : ContextData
 *  Description         : This class will make Facets Contexdata values available for extension to connect to database.
 *  Used by             : ContextData.cs
 *  Author              : TriZetto Inc. (Wen-Man Liu)
 *************************************************************************************************************************************************
 * Ver                  Date                    Modification Description
 * 1.0                  11/15/2011              Initial creation
 *************************************************************************************************************************************************
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data.OleDb;
using ErCoreIfcExtensionData;
using XA9Extensions.Utilities;

namespace XA9Extensions.BusinessLayer
{
    /// <summary>
    /// Represents Facets Context Data
    /// </summary>
    public class ContextData
    {

        #region Class Variables

        private static ContextData _instance;
        private string _applicationId;
        private string _exitApplicationId;
        private string _password;
        private string _databaseSourceId;
        private string _databaseId;
        private string _userId;
        private string _pzpz_id;

        private IFaExtensionData _extensionData;
        private string _contextdata;
        private static object syncRoot = new object();
        #endregion Class Variables

        public static ContextData ContextInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new ContextData();
                    }
                }
                return _instance;
            }
        }

        #region Constructor

        /// <summary>
        /// Constructor is private as this class is a singleton.
        /// </summary>
        private ContextData()
        {
        }

        #endregion Constructor

        #region Public Functions

        /// <summary>
        /// Initializes the class by loading information from the Facets extension data object.
        /// </summary>
        /// <param name="extensionData">The Facets extension data object.</param>
        public void Initialize(IFaExtensionData extensionData)
        {
            _instance._extensionData = extensionData;
            _contextdata = ContextInstance._extensionData.GetContextData(string.Empty);
            
            //normal facets context data
            _instance._applicationId = GetDataItem(null, XA9Constants.PZAP_ITEM);
            _instance._exitTiming = GetDataItem(XA9Constants.EXIT_COL, XA9Constants.TMNG_ITEM);
            _instance._exitApplicationId = GetDataItem(XA9Constants.EXIT_COL, XA9Constants.PZAP_ITEM);
            _instance._userId = GetDataItem(XA9Constants.SIGNON_ID, XA9Constants.USUS_ITEM);
            _instance._password = GetDataItem(XA9Constants.SIGNON_ID, XA9Constants.MNGL_PWD_ITEM);
            _instance._databaseId = GetDataItem(XA9Constants.SIGNON_ID, XA9Constants.DATABASE_ITEM);
            _instance._databaseSourceId = GetDataItem(XA9Constants.SIGNON_ID, XA9Constants.DATA_SOURCE_ITEM);
            _instance._pzpz_id = GetDataItem(null, XA9Constants.PZPZ_ITEM);
        }

        /// <summary>
        /// Initializes the class by loading information from the Facets extension data object.
        /// </summary>
        /// <param name="extensionData">The Facets extension data object.</param>
        public void Initialize(string pFacetsConnection)
        {
            _contextdata = pFacetsConnection;

            //normal facets context data
            
            _instance._userId = GetDataItem(null, XA9Constants.USUS_ITEM);
            _instance._password = GetDataItem(null, XA9Constants.MNGL_PWD_ITEM);
            _instance._databaseId = GetDataItem(null, XA9Constants.DATABASE_ITEM);
            _instance._databaseSourceId = GetDataItem(null, XA9Constants.DATA_SOURCE_ITEM);
            _instance._pzpz_id = GetDataItem(null, XA9Constants.PZPZ_ITEM);
            _instance._exitApplicationId = GetDataItem(null, XA9Constants.PZPZ_ITEM);
            //_instance._pzap_id = pPZAP_APP_ID;
        }



        /// <summary>
        /// Removes reference to the Facets extension data object from memory.
        /// </summary>
        /// <param name="extensionData">The Facets extension data object.</param>
        public void ReleaseExtensionObject()
        {
            _instance._extensionData = null;
        }

        /// <summary>
        /// Gets a specified value from the Facets runtime.
        /// </summary>
        /// <param name="data">The XML data.</param>
        /// <param name="dataId">The data Id.</param>
        /// <param name="itemId">The Item Id</param>
        /// <returns></returns>
        private string GetDataItem(string dataId, string itemId)
        {

            XmlDocument xmlData = new XmlDocument();
            xmlData.LoadXml(_contextdata);
            XmlNode selectedNode = null;

            if (dataId == null)
                //selectedNode = xmlData.SelectSingleNode(@"FacetsData/Column[@name='" + itemId + @"']");
                selectedNode = xmlData.SelectSingleNode(string.Format(XA9Constants.XML_COLLUMN_XPATH, itemId));
            else
                //selectedNode = xmlData.SelectSingleNode(@"FacetsData/Collection[@name='" + dataId + @"']/Column[@name='" + itemId + @"']");
                selectedNode = xmlData.SelectSingleNode(string.Format(XA9Constants.XML_COLLECTION_COLUMN_XPATH, dataId, itemId));


            if (selectedNode == null)
                return string.Empty;
            else
                return selectedNode.InnerText;

        }

        #endregion Public Functions

        #region Public Properties

        

        public string PZPZ_ID
        {
            get { return _pzpz_id; }
            set { _pzpz_id = value; }
        }

        /// <summary>
        /// Gets or sets the application id.
        /// </summary>
        public string ApplicationId
        {
            get
            {
                return _applicationId;
            }
        }

        private string _exitTiming;

        /// <summary>
        /// Gets or sets the exit timing.
        /// </summary>
        public string ExitTiming
        {
            get
            {
                return _exitTiming;
            }
        }


        /// <summary>
        /// Gets or sets the exit application id.
        /// </summary>
        public string ExitApplicationId
        {
            get
            {
                return _exitApplicationId;
            }
        }


        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password
        {
            get
            {
                return _password;
            }
        }


        /// <summary>
        /// Gets or sets the database source id.
        /// </summary>
        public string DatabaseSourceId
        {
            get
            {
                return _databaseSourceId;
            }
        }


        /// <summary>
        /// Gets or sets the database id.
        /// </summary>
        public string DatabaseId
        {
            get
            {
                return _databaseId;
            }
        }


        /// <summary>
        /// Gets or sets the Facets user Id.
        /// </summary>
        public string UserId
        {
            get
            {
                return _userId;
            }
        }
        #endregion Public Properties

    }//End of class ContextData

}//End of namespace ClmDMEProc.DataAccessLayer
