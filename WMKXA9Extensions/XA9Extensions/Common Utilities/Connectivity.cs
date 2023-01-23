using FacetsControlLibrary;
using ErCoreIfcExtensionData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Xml;

namespace CommonUtilities
{
    public class Connectivity
    {
        public class QueryExecutionInfo
        {
            private String _Sql = String.Empty;
            private String _XmlResult = String.Empty;

            private Facets _QEI_Facets = new Facets();
            private Custom _QEI_Custom = new Custom();

            public virtual String Sql
            {
                get { return _Sql; }
                set { _Sql = value; }
            }

            public virtual Int64 SqlReturnCode
            {
                get
                {
                    if (QEI_Custom != null)
                    {
                        return QEI_Custom.SqlReturnCode;
                    }
                    else if (QEI_Facets != null)
                    {
                        return QEI_Facets.SqlReturnCode;
                    }
                    return 0;
                }
            }

            public virtual Int64 SqlErrorNumber
            {
                get
                {
                    if (QEI_Custom != null)
                    {
                        return QEI_Custom.SqlErrorNumber;
                    }
                    else if (QEI_Facets != null)
                    {
                        return QEI_Facets.SqlErrorNumber;
                    }
                    return 0;
                }
            }

            public virtual Int64 SqlErrorSeverity
            {
                get
                {
                    if (QEI_Custom != null)
                    {
                        return QEI_Custom.SqlErrorSeverity;
                    }
                    return 0;
                }
            }

            public virtual Int64 SqlErrorState
            {
                get
                {
                    if (QEI_Custom != null)
                    {
                        return QEI_Custom.SqlErrorState;
                    }
                    return 0;
                }
            }

            public virtual String SqlErrorProcedure
            {
                get
                {
                    if (QEI_Custom != null)
                    {
                        return QEI_Custom.SqlErrorSource;
                    }
                    else if (QEI_Facets != null)
                    {
                        return QEI_Facets.ErrorSource;
                    }
                    return String.Empty;
                }
            }

            public virtual Int64 SqlErrorLine
            {
                get
                {
                    if (QEI_Custom != null)
                    {
                        return QEI_Custom.SqlErrorLine;
                    }
                    return 0;
                }
            }

            public virtual String SqlErrorMessage
            {
                get
                {
                    if (QEI_Custom != null)
                    {
                        return QEI_Custom.SqlErrorMessage;
                    }
                    else if (QEI_Facets != null)
                    {
                        return QEI_Facets.SqlErrorMessage;
                    }
                    return String.Empty;
                }
            }

            public virtual String ErrorData
            {
                get
                {
                    if (QEI_Facets != null)
                    {
                        return QEI_Facets.ErrorData;
                    }
                    return String.Empty;
                }
            }

            public virtual String ErrorCode
            {
                get
                {
                    if (QEI_Facets != null)
                    {
                        return QEI_Facets.ErrorCode;
                    }
                    return String.Empty;
                }
            }

            public virtual String ErrorMessage
            {
                get
                {
                    if (QEI_Facets != null)
                    {
                        return QEI_Facets.ErrorMessage;
                    }
                    return String.Empty;
                }
            }

            public virtual Int64 SqlRowCount
            {
                get
                {
                    if (QEI_Custom != null)
                    {
                        return QEI_Custom.SqlRowCount;
                    }
                    else if (QEI_Facets != null)
                    {
                        return QEI_Facets.SqlRowCount;
                    }
                    return 0;
                }
            }

            public virtual Int64 SqlColumnCount
            {
                get
                {
                    if (QEI_Custom != null)
                    {
                        return QEI_Custom.SqlColumnCount;
                    }
                    else if (QEI_Facets != null)
                    {
                        return QEI_Facets.SqlColumnCount;
                    }
                    return 0;
                }
            }

            public virtual Int64 SqlStepNumber
            {
                get
                {
                    if (QEI_Custom != null)
                    {
                        return QEI_Custom.SqlStepNumber;
                    }
                    return 0;
                }
            }

            public virtual Enumerations.Sql.CustomCodeTypes SqlCustomCodeType
            {
                get
                {
                    if (QEI_Custom != null)
                    {
                        return QEI_Custom.SqlCustomCodeType;
                    }
                    return Enumerations.Sql.CustomCodeTypes.None;
                }
            }

            public virtual Int64 SqlCustomCode
            {
                get
                {
                    if (QEI_Custom != null)
                    {
                        return QEI_Custom.SqlCustomCode;
                    }
                    return 0;
                }
            }

            public virtual String SqlCustomMessage
            {
                get
                {
                    if (QEI_Custom != null)
                    {
                        return QEI_Custom.SqlCustomMessage;
                    }
                    return String.Empty;
                }
            }

            public virtual String XmlResult
            {
                get { return _XmlResult; }
                set
                {
                    _XmlResult = value;
                    _QEI_Facets = Facets.GetObject(value);
                    _QEI_Custom = Custom.GetObject(value);
                }
            }

            public virtual Facets QEI_Facets
            {
                get { return _QEI_Facets; }
                protected set { _QEI_Facets = value; }
            }

            public virtual Custom QEI_Custom
            {
                get { return _QEI_Custom; }
                protected set { _QEI_Custom = value; }
            }

            public virtual Enumerations.Status Status
            {
                get
                {
                    if (QEI_Custom != null)
                    {
                        switch (QEI_Custom.SqlCustomCodeType)
                        {
                            case Enumerations.Sql.CustomCodeTypes.None:
                                break;
                            case Enumerations.Sql.CustomCodeTypes.Information:
                                return Enumerations.Status.Normal;
                            case Enumerations.Sql.CustomCodeTypes.Warning:
                                return Enumerations.Status.Warning;
                            case Enumerations.Sql.CustomCodeTypes.Error:
                                return Enumerations.Status.Error;
                            default:
                                break;
                        }
                    }

                    if (SqlReturnCode == 0)
                    {
                        return Enumerations.Status.Normal;
                    }
                    return Enumerations.Status.CriticalError;
                }
            }

            public QueryExecutionInfo()
            {
                Sql = String.Empty;
                XmlResult = String.Empty;
                QEI_Facets = new Facets();
                QEI_Custom = new Custom();
            }

            public QueryExecutionInfo(String Sql, String XmlResult)
            {
                this.Sql = Sql;
                this.XmlResult = XmlResult;
            }

            public QueryExecutionInfo(QueryExecutionInfo objQEI)
            {
                Sql = objQEI.Sql;
                XmlResult = objQEI.XmlResult;
                ObjectEx objOE = new ObjectEx();
                QEI_Facets = new Facets(objQEI.QEI_Facets);
                QEI_Custom = new Custom(objQEI.QEI_Custom);
            }

            public class Facets
            {
                private Int64 _SqlReturnCode = 0;
                private Int64 _SqlRowCount = 0;
                private Int64 _SqlColumnCount = 0;
                private String _ErrorCode = String.Empty;
                private String _ErrorData = String.Empty;
                private String _ErrorSource = String.Empty;
                private String _ErrorMessage = String.Empty;
                private Int64 _SqlErrorNumber = 0;
                private String _SqlErrorMessage = String.Empty;

                [Description("RETURN_CODE")]
                public virtual Int64 SqlReturnCode
                {
                    get { return _SqlReturnCode; }
                    set { _SqlReturnCode = value; }
                }

                [Description("NUM_RESULTS")]
                public virtual Int64 SqlRowCount
                {
                    get { return _SqlRowCount; }
                    set { _SqlRowCount = value; }
                }

                [Description("FIELD_COUNT")]
                public virtual Int64 SqlColumnCount
                {
                    get { return _SqlColumnCount; }
                    set { _SqlColumnCount = value; }
                }

                [Description("ERROR_CODE")]
                public virtual String ErrorCode
                {
                    get { return _ErrorCode; }
                    set { _ErrorCode = value; }
                }

                [Description("ERROR_DATA")]
                public virtual String ErrorData
                {
                    get { return _ErrorData; }
                    set { _ErrorData = value; }
                }

                [Description("ERROR_SOURCE")]
                public virtual String ErrorSource
                {
                    get { return _ErrorSource; }
                    set { _ErrorSource = value; }
                }

                [Description("ERROR_MESSAGE")]
                public virtual String ErrorMessage
                {
                    get
                    {
                        if (_ErrorMessage == "mcsErrorMsg")
                        {
                            return String.Empty;
                        }
                        return _ErrorMessage;
                    }
                    set { _ErrorMessage = value; }
                }

                [Description("SQL_ERROR_CODE")]
                public virtual Int64 SqlErrorNumber
                {
                    get { return _SqlErrorNumber; }
                    set { _SqlErrorNumber = value; }
                }

                [Description("SQL_ERROR_MESSAGE")]
                public virtual String SqlErrorMessage
                {
                    get
                    {
                        if (_SqlErrorMessage == "mcsSqlErrorMsg")
                        {
                            return Constants.Messages.TechnicalError;
                        }
                        return _SqlErrorMessage;
                    }
                    set { _SqlErrorMessage = value; }
                }

                public Facets()
                {
                    SqlReturnCode = 0;
                    SqlRowCount = 0;
                    SqlColumnCount = 0;
                    ErrorCode = String.Empty;
                    ErrorData = String.Empty;
                    ErrorSource = String.Empty;
                    ErrorMessage = String.Empty;
                    SqlErrorNumber = 0;
                    SqlErrorMessage = String.Empty;
                }

                public Facets(String XmlResult)
                {
                    try
                    {
                        Facets obj = GetObject(XmlResult);
                        if (obj != null)
                        {
                            new ObjectEx().CopyObject<Facets>(obj, this);
                        }
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                }

                public Facets(Facets objFacets)
                {
                    try
                    {
                        new ObjectEx().CopyObject<Facets>(objFacets, this);
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                }

                public static Facets GetObject(String XmlResult)
                {
                    try
                    {
                        List<Facets> objList = new DataExtraction().GetRecordsList<Facets>(XmlResult, PropertyMatchFromDescription: true);
                        if (objList.Count > 0)
                        {
                            return objList[0];
                        }
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                    return null;
                }
            }

            public class Custom
            {
                private Int64 _SqlReturnCode = 0;
                private Int64 _SqlErrorNumber = 0;
                private Int64 _SqlErrorSeverity = 0;
                private Int64 _SqlErrorState = 0;
                private String _SqlErrorSource = String.Empty;
                private Int64 _SqlErrorLine = 0;
                private String _SqlErrorMessage = String.Empty;
                private Int64 _SqlRowCount = 0;
                private Int64 _SqlColumnCount = 0;
                private Int64 _SqlStepNumber = 0;
                private Enumerations.Sql.CustomCodeTypes _SqlCustomCodeType = Enumerations.Sql.CustomCodeTypes.None;
                private Int64 _SqlCustomCode = 0;
                private String _SqlCustomMessage = String.Empty;

                [Description("SQL_RTRN_CD")]
                public virtual Int64 SqlReturnCode
                {
                    get { return _SqlReturnCode; }
                    set { _SqlReturnCode = value; }
                }

                [Description("SQL_ERROR_NUM")]
                public virtual Int64 SqlErrorNumber
                {
                    get { return _SqlErrorNumber; }
                    set { _SqlErrorNumber = value; }
                }

                [Description("SQL_ERROR_SEVERITY")]
                public virtual Int64 SqlErrorSeverity
                {
                    get { return _SqlErrorSeverity; }
                    set { _SqlErrorSeverity = value; }
                }

                [Description("SQL_ERROR_ST")]
                public virtual Int64 SqlErrorState
                {
                    get { return _SqlErrorState; }
                    set { _SqlErrorState = value; }
                }

                [Description("SQL_ERROR_SRC")]
                public virtual String SqlErrorSource
                {
                    get { return _SqlErrorSource; }
                    set { _SqlErrorSource = value; }
                }

                [Description("SQL_ERROR_LINE")]
                public virtual Int64 SqlErrorLine
                {
                    get { return _SqlErrorLine; }
                    set { _SqlErrorLine = value; }
                }

                [Description("SQL_ERROR_MSG")]
                public virtual String SqlErrorMessage
                {
                    get { return _SqlErrorMessage; }
                    set { _SqlErrorMessage = value; }
                }

                [Description("SQL_ROW_CNT")]
                public virtual Int64 SqlRowCount
                {
                    get { return _SqlRowCount; }
                    set { _SqlRowCount = value; }
                }

                [Description("SQL_COL_CNT")]
                public virtual Int64 SqlColumnCount
                {
                    get { return _SqlColumnCount; }
                    set { _SqlColumnCount = value; }
                }

                [Description("SQL_STEP_NUM")]
                public virtual Int64 SqlStepNumber
                {
                    get { return _SqlStepNumber; }
                    set { _SqlStepNumber = value; }
                }

                public virtual Enumerations.Sql.CustomCodeTypes SqlCustomCodeType
                {
                    get { return _SqlCustomCodeType; }
                    set { _SqlCustomCodeType = value; }
                }

                [Description("SQL_CUST_CD_TYP_DESC")]
                public virtual String SqlCustomCodeTypeDescription
                {
                    get
                    {
                        return Enumerations.GetDescriptionFromValue(SqlCustomCodeType);
                    }
                    set
                    {
                        _SqlCustomCodeType = Enumerations.GetValueFromDescription<Enumerations.Sql.CustomCodeTypes>(value);
                    }
                }

                [Description("SQL_CUST_CD_TYP_NM")]
                public virtual String SqlCustomCodeTypeName
                {
                    get
                    {
                        return SqlCustomCodeType.ToString();
                    }
                    set
                    {
                        _SqlCustomCodeType = Enumerations.GetEnumFromName<Enumerations.Sql.CustomCodeTypes>(value);
                    }
                }

                [Description("SQL_CUST_CD_TYP_VAL")]
                public virtual Int32 SqlCustomCodeTypeValue
                {
                    get
                    {
                        return (Int32)SqlCustomCodeType;
                    }
                    set
                    {
                        _SqlCustomCodeType = Enumerations.GetEnumFromInteger<Enumerations.Sql.CustomCodeTypes>(value);
                    }
                }

                [Description("SQL_CUST_CD")]
                public virtual Int64 SqlCustomCode
                {
                    get { return _SqlCustomCode; }
                    set { _SqlCustomCode = value; }
                }

                [Description("SQL_CSTM_MSG")]
                public virtual String SqlCustomMessage
                {
                    get { return _SqlCustomMessage; }
                    set { _SqlCustomMessage = value; }
                }

                public Custom()
                {
                    SqlReturnCode = 0;
                    SqlErrorNumber = 0;
                    SqlErrorSeverity = 0;
                    SqlErrorState = 0;
                    SqlErrorSource = String.Empty;
                    SqlErrorLine = 0;
                    SqlErrorMessage = String.Empty;
                    SqlRowCount = 0;
                    SqlColumnCount = 0;
                    SqlStepNumber = 0;
                    SqlCustomCodeType = Enumerations.Sql.CustomCodeTypes.None;
                    SqlCustomCode = 0;
                    SqlCustomMessage = String.Empty;
                }

                public Custom(String XmlResult)
                {
                    try
                    {
                        Custom obj = GetObject(XmlResult);
                        if (obj != null)
                        {
                            new ObjectEx().CopyObject<Custom>(obj, this);
                        }
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                }
                public Custom(Custom objCustom)
                {
                    try
                    {
                        new ObjectEx().CopyObject<Custom>(objCustom, this);
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                }

                public static Custom GetObject(String XmlResult)
                {
                    try
                    {
                        List<Custom> objList = new DataExtraction().GetRecordsList<Custom>(XmlResult, Constants.XmlNodeDataID.Custom, PropertyMatchFromDescription: true);
                        if (objList.Count > 0)
                        {
                            return objList[0];
                        }
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                    return null;
                }
            }
        }

        public class ConnectionInfo
        {
            public class FrontEnd
            {
                private static Enumerations.FrontEndInterfaceModes _InterfaceMode = Enumerations.FrontEndInterfaceModes.None;

                private FacetsBaseControlEx _objFBC = new FacetsBaseControlEx();
                private IFaExtensionData _objIFaExtensionData;

                public static Enumerations.FrontEndInterfaceModes InterfaceMode
                {
                    get { return _InterfaceMode; }
                    set { _InterfaceMode = value; }
                }

                public virtual FacetsBaseControlEx objFBC
                {
                    get { return _objFBC; }
                    set
                    {
                        _objFBC = value;
                        _InterfaceMode = Enumerations.FrontEndInterfaceModes.FacetsBaseControl;
                    }
                }

                public virtual IFaExtensionData objIFaExtensionData
                {
                    get { return _objIFaExtensionData; }
                    set
                    {
                        _objIFaExtensionData = value;
                        _InterfaceMode = Enumerations.FrontEndInterfaceModes.Interop;
                    }
                }

                public FrontEnd()
                {
                }

                public virtual Enumerations.Status Reset()
                {
                    try
                    {
                        return new ObjectEx().CopyObject<FrontEnd>(new FrontEnd(), this);
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                        return Enumerations.Status.Exception;
                    }
                }

                public virtual String ExecuteQueryForOnlyResult(String Sql)
                {
                    try
                    {
                        Reset();
                        String xmlResult = String.Empty;
                        switch (InterfaceMode)
                        {
                            case Enumerations.FrontEndInterfaceModes.None:
                                break;
                            case Enumerations.FrontEndInterfaceModes.Interop:
                                xmlResult = objIFaExtensionData.GetDbRequest(Sql);
                                break;
                            case Enumerations.FrontEndInterfaceModes.FacetsBaseControl:
                                xmlResult = objFBC.GetDbRequest(Sql);
                                break;
                            default:
                                break;
                        }
                        return xmlResult;
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                    return String.Empty;
                }

                public virtual QueryExecutionInfo ExecuteQuery(String Sql)
                {
                    try
                    {
                        return new QueryExecutionInfo(Sql, ExecuteQueryForOnlyResult(Sql));
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                    return null;
                }
            }

            public class BackEnd
            {
                private String _ConnectionString = String.Empty;

                public virtual String ConnectionString
                {
                    get { return _ConnectionString; }
                    set { _ConnectionString = value.Trim(); }
                }

                public BackEnd()
                {
                }

                public BackEnd(String ConnectionString)
                {
                    this.ConnectionString = ConnectionString;
                }

                public virtual Boolean CheckConnection()
                {
                    try
                    {
                        if (!String.IsNullOrEmpty(ConnectionString))
                        {
                            using (OleDbConnection objConnection = new OleDbConnection(ConnectionString))
                            {
                                objConnection.Open();
                            }
                        }
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                    return false;
                }
            }
        }

        public class SignOnInformation
        {
            public class ExitGroup
            {
                private String _TIMING = String.Empty;
                private String _PZAP_APP_ID = String.Empty;
                private String _SYIN_INST = String.Empty;

                public virtual String TIMING
                {
                    get { return _TIMING; }
                    set { _TIMING = value; }
                }

                public virtual String PZAP_APP_ID
                {
                    get { return _PZAP_APP_ID; }
                    set { _PZAP_APP_ID = value; }
                }

                public virtual String SYIN_INST
                {
                    get { return _SYIN_INST; }
                    set { _SYIN_INST = value; }
                }

                public ExitGroup()
                {
                    TIMING = String.Empty;
                    PZAP_APP_ID = String.Empty;
                    SYIN_INST = String.Empty;
                }

                public ExitGroup(String ContextData)
                {
                    try
                    {
                        List<ExitGroup> objList = new DataExtraction().GetRecordsList<ExitGroup>(ContextData, Constants.XmlNodeDataID.EXIT);
                        if (objList.Count > 0)
                        {
                            new ObjectEx().CopyObject<ExitGroup>(objList.First(), this);
                        }
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                }

                public ExitGroup(IFaExtensionData objFacetsExtensionData)
                {
                    try
                    {
                        List<ExitGroup> objList = new DataExtraction().GetRecordsList<ExitGroup>(objFacetsExtensionData.GetContextData(null), Constants.XmlNodeDataID.EXIT);
                        if (objList.Count > 0)
                        {
                            new ObjectEx().CopyObject<ExitGroup>(objList.First(), this);
                        }
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                }
            }

            public class SGN0
            {
                private String _PZPZ_ID = String.Empty;
                private String _USUS_ID = String.Empty;
                private String _DATA_SOURCE_ID = String.Empty;
                private String _DATABASE_ID = String.Empty;
                private String _DATABASE_USER_ID = String.Empty;
                private String _Port = String.Empty;
                private String _SIGNON_PASSWORD = String.Empty;
                private String _MANGLED_PASSWORD = String.Empty;

                public virtual String PZPZ_ID
                {
                    get { return _PZPZ_ID; }
                    set { _PZPZ_ID = value; }
                }

                public virtual String USUS_ID
                {
                    get { return _USUS_ID; }
                    set { _USUS_ID = value; }
                }

                public virtual String DATA_SOURCE_ID
                {
                    get { return _DATA_SOURCE_ID; }
                    set { _DATA_SOURCE_ID = value; }
                }

                public virtual String DATABASE_ID
                {
                    get { return _DATABASE_ID; }
                    set { _DATABASE_ID = value; }
                }

                public virtual String DATABASE_USER_ID
                {
                    get { return _DATABASE_USER_ID; }
                    set { _DATABASE_USER_ID = value; }
                }

                public virtual String Port
                {
                    get { return _Port; }
                    set { _Port = value; }
                }

                public virtual String SIGNON_PASSWORD
                {
                    get { return _SIGNON_PASSWORD; }
                    set { _SIGNON_PASSWORD = value; }
                }

                public virtual String MANGLED_PASSWORD
                {
                    get { return _MANGLED_PASSWORD; }
                    set { _MANGLED_PASSWORD = value; }
                }

                public SGN0()
                {
                    PZPZ_ID = String.Empty;
                    USUS_ID = String.Empty;
                    DATA_SOURCE_ID = String.Empty;
                    DATABASE_ID = String.Empty;
                    DATABASE_USER_ID = String.Empty;
                    Port = String.Empty;
                    SIGNON_PASSWORD = String.Empty;
                    MANGLED_PASSWORD = String.Empty;
                }

                public SGN0(String ContextData)
                {
                    try
                    {
                        List<SGN0> objList = new DataExtraction().GetRecordsList<SGN0>(ContextData, Constants.XmlNodeDataID.SGN0);
                        if (objList.Count > 0)
                        {
                            new ObjectEx().CopyObject<SGN0>(objList.First(), this);
                        }
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                }

                public SGN0(IFaExtensionData objFacetsExtensionData)
                {
                    try
                    {
                        List<SGN0> objList = new DataExtraction().GetRecordsList<SGN0>(objFacetsExtensionData.GetContextData(null), Constants.XmlNodeDataID.EXIT);
                        if (objList.Count > 0)
                        {
                            new ObjectEx().CopyObject<SGN0>(objList.First(), this);
                        }
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                }

                public SGN0(FacetsBaseControlEx objFBC)
                {
                    try
                    {
                        List<SGN0> objList = new DataExtraction().GetRecordsList<SGN0>(objFBC.GetData(Constants.XmlNodeDataID.SGN0_FBC_All));
                        if (objList.Count > 0)
                        {
                            new ObjectEx().CopyObject<SGN0>(objList.First(), this);
                        }
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                }
            }

            public class SIGN_ON
            {
                private String _REGION_ID = String.Empty;
                private String _REGION_LOC = String.Empty;
                private String _PZAP_APP_ID = String.Empty;
                private String _ContextData = String.Empty;

                public virtual String REGION_ID
                {
                    get { return _REGION_ID; }
                    set { _REGION_ID = value; }
                }

                public virtual String REGION_LOC
                {
                    get { return _REGION_LOC; }
                    set { _REGION_LOC = value; }
                }

                public virtual String PZAP_APP_ID
                {
                    get { return _PZAP_APP_ID; }
                    set { _PZAP_APP_ID = value; }
                }

                public virtual String ContextData
                {
                    get { return _ContextData; }
                    set { _ContextData = value; }
                }

                public ExitGroup objExitGroup = new ExitGroup();
                public SGN0 objSGN0 = new SGN0();
                public CTXT_PZAP objCTXT_PZAP = new CTXT_PZAP();
                public CTXT_SYIN objCTXT_SYIN = new CTXT_SYIN();

                public SIGN_ON()
                {
                    REGION_ID = String.Empty;
                    REGION_LOC = String.Empty;
                    PZAP_APP_ID = String.Empty;
                    ContextData = String.Empty;
                    objExitGroup = new ExitGroup();
                    objSGN0 = new SGN0();
                }

                public SIGN_ON(IFaExtensionData objFacetsExtensionData)
                {
                    try
                    {
                        ContextData = objFacetsExtensionData.GetContextData(null);
                        List<SIGN_ON> objList = new DataExtraction().GetRecordsList<SIGN_ON>(ContextData, Constants.XmlNodeDataID.Blank);
                        if (objList.Count > 0)
                        {
                            if (new ObjectEx().CopyObject<SIGN_ON>(objList.First(), this) == Enumerations.Status.Normal)
                            {
                                objExitGroup = new ExitGroup(ContextData);
                                objSGN0 = new SGN0(ContextData);
                            }
                        }
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                }

                public SIGN_ON(FacetsBaseControlEx objFBC)
                {
                    try
                    {
                        objSGN0 = new SGN0(objFBC);
                        objCTXT_PZAP = new CTXT_PZAP(objFBC);
                        objCTXT_SYIN = new CTXT_SYIN(objFBC);
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                }
            }

            public class CTXT_PZAP
            {
                private String _PZPZ_ID = String.Empty;
                private String _PZAP_APP_ID = String.Empty;
                private String _APMD_ID = String.Empty;
                private String _PZAP_ICON = String.Empty;
                private String _PZAP_DISP_LABEL = String.Empty;
                private String _PZAP_DESC = String.Empty;
                private String _PZAP_MENU_TEXT = String.Empty;
                private String _PZAP_AUDIT_IND = String.Empty;
                private String _PZAP_MAX_ENGINES = String.Empty;
                private String _PZAP_WORK_FLOW_IND = String.Empty;
                private String _PZAP_LOCK_TOKEN = String.Empty;

                public virtual String PZPZ_ID
                {
                    get { return _PZPZ_ID; }
                    set { _PZPZ_ID = value; }
                }

                public virtual String PZAP_APP_ID
                {
                    get { return _PZAP_APP_ID; }
                    set { _PZAP_APP_ID = value; }
                }

                public virtual String APMD_ID
                {
                    get { return _APMD_ID; }
                    set { _APMD_ID = value; }
                }

                public virtual String PZAP_ICON
                {
                    get { return _PZAP_ICON; }
                    set { _PZAP_ICON = value; }
                }

                public virtual String PZAP_DISP_LABEL
                {
                    get { return _PZAP_DISP_LABEL; }
                    set { _PZAP_DISP_LABEL = value; }
                }

                public virtual String PZAP_DESC
                {
                    get { return _PZAP_DESC; }
                    set { _PZAP_DESC = value; }
                }

                public virtual String PZAP_MENU_TEXT
                {
                    get { return _PZAP_MENU_TEXT; }
                    set { _PZAP_MENU_TEXT = value; }
                }

                public virtual String PZAP_AUDIT_IND
                {
                    get { return _PZAP_AUDIT_IND; }
                    set { _PZAP_AUDIT_IND = value; }
                }

                public virtual String PZAP_MAX_ENGINES
                {
                    get { return _PZAP_MAX_ENGINES; }
                    set { _PZAP_MAX_ENGINES = value; }
                }

                public virtual String PZAP_WORK_FLOW_IND
                {
                    get { return _PZAP_WORK_FLOW_IND; }
                    set { _PZAP_WORK_FLOW_IND = value; }
                }

                public virtual String PZAP_LOCK_TOKEN
                {
                    get { return _PZAP_LOCK_TOKEN; }
                    set { _PZAP_LOCK_TOKEN = value; }
                }

                public CTXT_PZAP()
                {
                    PZPZ_ID = String.Empty;
                    PZAP_APP_ID = String.Empty;
                    APMD_ID = String.Empty;
                    PZAP_ICON = String.Empty;
                    PZAP_DISP_LABEL = String.Empty;
                    PZAP_DESC = String.Empty;
                    PZAP_MENU_TEXT = String.Empty;
                    PZAP_AUDIT_IND = String.Empty;
                    PZAP_MAX_ENGINES = String.Empty;
                    PZAP_WORK_FLOW_IND = String.Empty;
                    PZAP_LOCK_TOKEN = String.Empty;
                }

                public CTXT_PZAP(FacetsBaseControlEx objFBC)
                {
                    try
                    {
                        List<CTXT_PZAP> objList = new DataExtraction().GetRecordsList<CTXT_PZAP>(objFBC.GetData(Constants.XmlNodeDataID.CTXT_PZAP_FBC));
                        if (objList.Count > 0)
                        {
                            new ObjectEx().CopyObject<CTXT_PZAP>(objList.First(), this);
                        }
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                }
            }

            public class CTXT_SYIN
            {
                private String _SYIN_INST = String.Empty;
                private String _SPID = String.Empty;
                private String _PZPZ_ID = String.Empty;
                private String _PZAP_APP_ID = String.Empty;
                private String _USAP_VERSION = String.Empty;
                private String _SYIN_REF_ID = String.Empty;
                private String _SYIN_HOST = String.Empty;
                private String _SYIN_HOST_PID = String.Empty;
                private String _SYIN_STS = String.Empty;
                private String _SYIN_DESC = String.Empty;
                private String _SYIN_PARENT_INST = String.Empty;
                private String _USUS_ID = String.Empty;
                private String _SYIN_CREATE_DTM = String.Empty;
                private String _SYIN_COMPLETE_DTM = String.Empty;
                private String _SYIN_LOCK_TOKEN = String.Empty;

                public virtual String SYIN_INST
                {
                    get { return _SYIN_INST; }
                    set { _SYIN_INST = value; }
                }

                public virtual String SPID
                {
                    get { return _SPID; }
                    set { _SPID = value; }
                }

                public virtual String PZPZ_ID
                {
                    get { return _PZPZ_ID; }
                    set { _PZPZ_ID = value; }
                }

                public virtual String PZAP_APP_ID
                {
                    get { return _PZAP_APP_ID; }
                    set { _PZAP_APP_ID = value; }
                }

                public virtual String USAP_VERSION
                {
                    get { return _USAP_VERSION; }
                    set { _USAP_VERSION = value; }
                }

                public virtual String SYIN_REF_ID
                {
                    get { return _SYIN_REF_ID; }
                    set { _SYIN_REF_ID = value; }
                }

                public virtual String SYIN_HOST
                {
                    get { return _SYIN_HOST; }
                    set { _SYIN_HOST = value; }
                }

                public virtual String SYIN_HOST_PID
                {
                    get { return _SYIN_HOST_PID; }
                    set { _SYIN_HOST_PID = value; }
                }

                public virtual String SYIN_STS
                {
                    get { return _SYIN_STS; }
                    set { _SYIN_STS = value; }
                }

                public virtual String SYIN_DESC
                {
                    get { return _SYIN_DESC; }
                    set { _SYIN_DESC = value; }
                }

                public virtual String SYIN_PARENT_INST
                {
                    get { return _SYIN_PARENT_INST; }
                    set { _SYIN_PARENT_INST = value; }
                }

                public virtual String USUS_ID
                {
                    get { return _USUS_ID; }
                    set { _USUS_ID = value; }
                }

                public virtual String SYIN_CREATE_DTM
                {
                    get { return _SYIN_CREATE_DTM; }
                    set { _SYIN_CREATE_DTM = value; }
                }

                public virtual String SYIN_COMPLETE_DTM
                {
                    get { return _SYIN_COMPLETE_DTM; }
                    set { _SYIN_COMPLETE_DTM = value; }
                }

                public virtual String SYIN_LOCK_TOKEN
                {
                    get { return _SYIN_LOCK_TOKEN; }
                    set { _SYIN_LOCK_TOKEN = value; }
                }

                public CTXT_SYIN()
                {
                    SYIN_INST = String.Empty;
                    SPID = String.Empty;
                    PZPZ_ID = String.Empty;
                    PZAP_APP_ID = String.Empty;
                    USAP_VERSION = String.Empty;
                    SYIN_REF_ID = String.Empty;
                    SYIN_HOST = String.Empty;
                    SYIN_HOST_PID = String.Empty;
                    SYIN_STS = String.Empty;
                    SYIN_DESC = String.Empty;
                    SYIN_PARENT_INST = String.Empty;
                    USUS_ID = String.Empty;
                    SYIN_CREATE_DTM = String.Empty;
                    SYIN_COMPLETE_DTM = String.Empty;
                    SYIN_LOCK_TOKEN = String.Empty;
                }

                public CTXT_SYIN(FacetsBaseControlEx objFBC)
                {
                    try
                    {
                        List<CTXT_SYIN> objList = new DataExtraction().GetRecordsList<CTXT_SYIN>(objFBC.GetData(Constants.XmlNodeDataID.CTXT_SYIN_FBC));
                        if (objList.Count > 0)
                        {
                            new ObjectEx().CopyObject<CTXT_SYIN>(objList.First(), this);
                        }
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                }
            }

            public class ODBC_CON
            {
                private String _Connection = String.Empty;

                public virtual String Connection
                {
                    get { return _Connection; }
                    set { _Connection = value; }
                }

                public ODBC_CON()
                {
                    Connection = String.Empty;
                }

                public ODBC_CON(FacetsBaseControlEx objFBC)
                {
                    try
                    {
                        List<ODBC_CON> objList = new DataExtraction().GetRecordsList<ODBC_CON>(objFBC.GetData(Constants.XmlNodeDataID.ODBC_CON_FBC));
                        if (objList.Count > 0)
                        {
                            new ObjectEx().CopyObject<ODBC_CON>(objList.First(), this);
                        }
                    }
                    catch (Exception objException)
                    {
                        EventLogger.WriteException(objException);
                    }
                }
            }
        }
    }
}
