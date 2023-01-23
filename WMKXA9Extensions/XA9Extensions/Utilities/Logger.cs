/*************************************************************************************************************************************************
 *  Class               : Logger
 *  Description         : Logger for Extensions
 *  Author              : Trizetto
 *************************************************************************************************************************************************
 * Ver                  Date                    Modification Description
 * 1.0                  04/25/2016              Initial class creation 
 *************************************************************************************************************************************************
 */
// Logger.cs

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using XA9Extensions.DataAccessLayer;
using XA9Extensions.BusinessLayer;

namespace XA9Extensions.Utilities
{
    /// <summary>
    /// Handles message reporting, e.g. to the audit log
    /// </summary>
    public sealed class Logger
    {

        private static volatile Logger _loggerInstance;
        private static object syncRoot = new object();

        public static Logger LoggerInstance
        {
            get
            {
                if (_loggerInstance == null)
                {
                    lock (syncRoot)
                    {
                        if (_loggerInstance == null)
                            _loggerInstance = new Logger();
                    }
                }
                return _loggerInstance;
            }
        }

        private string _logFileFullyQualifiedName;
        private LoggingLevelEnum _loggingLevel;
        //private string _facetsMethodName;
        private string _currentDatasource;
        private string _currentDatabase;
        private string _currentClaimId;

        public string CurrentClaimId
        {
            get { return _currentClaimId; }
            set { _currentClaimId = value; }
        }
        private DateTime _loggerStartTime;

        private const string _writeToEventLog = XA9Constants.LOG_EVNT_LGR; // AppConfig.LogFileLocation value with special meaning
        private const string _eventLogSource = XA9Constants.LOG_EVNT_SOURCE;
        private const int _maxLoggerTries = 5;
        private int _LoggerTries;

        //Version information: (KEEP CURRENT!)
        internal const string STR_PROJECT_VERSION = "1.0";
        private const string STR_BUILD_SUFFIX = ".0.0";
        internal const string STR_ASSEMBLY_VERSION = STR_PROJECT_VERSION + ".0.0";  
        internal const string STR_BUILD_VERSION = STR_PROJECT_VERSION + STR_BUILD_SUFFIX;

      
        internal Logger()
        {
            LoggerCreate();
        }
        /// <summary>
        /// Initialized Logger object (to last through the processing of the COM method)
        /// </summary>
        /// <param name="facetsMethodName">Name of the method called by Facets</param>
        /// <param name="datasource">Datasource (server) Facets is connected to</param>
        /// <param name="database">Database Facets is connected to</param>
        /// <param name="facetsData">Facets data passed by Facets</param>
        internal void LoggerCreate()
        {
            _loggingLevel = AppConfig.LoggingLevel;
            _currentClaimId = string.Empty;
            if (_loggingLevel .Equals ( LoggingLevelEnum.None)) return; 

            _currentDatasource = ContextData.ContextInstance.DatabaseSourceId;
            _currentDatabase = ContextData.ContextInstance.DatabaseId;
            _loggerStartTime = DateTime.Now;

            if (_loggingLevel >= LoggingLevelEnum.Short)
            {
                _loggerStartTime = DateTime.Now;
            }

        } // constructor

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p_facetsMethodName"></param>
        internal void ReportStart(string p_facetsMethodName)
        {
            if (_loggingLevel >= LoggingLevelEnum.Short)
            {
                WriteLogEntry(new StringBuilder(XA9Constants.LOG_STRT_FRMT_LINE + Environment.NewLine), EventLogEntryType.Information);
                ProcessLogEntry(p_facetsMethodName, DateTime.Now,
                                XA9Constants.PRC_ASMBLY + XA9Constants.PRC_START,
                                EventLogEntryType.Information
                               );
            }
            _loggerStartTime = DateTime.Now;

        } // ReportFinish
         
        
        /// <summary>
        /// 
        /// </summary>
        internal void ReportFinish(string p_facetsMethodName)
        {
            if (_loggingLevel >= LoggingLevelEnum.Short)
            {
                DateTime now = DateTime.Now;
                ProcessLogEntry(p_facetsMethodName, now,
                                string.Format(
                                              XA9Constants.PRC_END ,
                                              FormattedDuration(_loggerStartTime, now)
                                             ),
                                EventLogEntryType.Information
                               );
            }

        } // ReportFinish
        

        /// <summary>
        /// Returns a formatted duration between the two times
        /// </summary>
        /// <param name="start">Begin of the time span</param>
        /// <param name="stop">End of the time span</param>
        /// <returns>Duration in seconds (milliseconds after double point) or 'undetermined time' if start or stop is unknown</returns>
        private static string FormattedDuration(DateTime start, DateTime stop)
        {
            
            TimeSpan duration = stop.Subtract(start);

            return duration.TotalSeconds.ToString(XA9Constants.FRMT_SECONDS);

        } // FormattedDuration

        /// <summary>
        /// Handles error message
        /// </summary>
        /// <param name="errorMessage">Error message to report</param>
        internal void ReportError(string p_facetsMethodName, string errorMessage)
        {
            if (_loggingLevel .Equals (LoggingLevelEnum.None)) return;
            ProcessLogEntry(p_facetsMethodName, DateTime.Now, XA9Constants.LOG_ERR_PREFIX  + errorMessage, EventLogEntryType.Error);

        } // ReportError

        /// <summary>
        /// Handle generic informational message
        /// </summary>
        /// <param name="message">Message to report</param>
        internal void ReportMessage(string p_facetsMethodName, string message)
        {
            if (_loggingLevel < LoggingLevelEnum.Full) return;

            ProcessLogEntry(p_facetsMethodName, DateTime.Now, message, EventLogEntryType.Information);

        } // ReportMessage

        /// <summary>
        /// Handle generic warning message
        /// </summary>
        /// <param name="message">Message to report</param>
        internal void ReportWarning(string p_facetsMethodName, string message)
        {
            if (_loggingLevel < LoggingLevelEnum.Full) return;

            ProcessLogEntry(p_facetsMethodName, DateTime.Now, message, EventLogEntryType.Warning);

        } // ReportWarning


        /// <summary>
        /// This method is intended to report long data, mainly for testing/debugging purposes
        /// </summary>
        /// <param name="title">Definition of data reported</param>
        /// <param name="contents">Contents of data reported(e.g. the actual XML data sent between Facets and iCES)</param>
        internal void ReportVerboseMessage(string title, string contents)
        {
            if (_loggingLevel < LoggingLevelEnum.Verbose) return;

            StringBuilder output = new StringBuilder(XA9Constants.LOG_VRBS_FRMT_LINE, 4000);
            output.Append(Environment.NewLine);
            output.Append(XA9Constants.LOG_VRBS_BGN);
            output.Append(title);
            output.Append(Environment.NewLine);
            output.Append(contents);
            output.Append(Environment.NewLine);
            output.Append(XA9Constants.LOG_VRBS_END);
            output.Append(title);
            output.Append(Environment.NewLine);
            output.Append(XA9Constants.LOG_VRBS_CNTNTS);
            output.Append(title);
            output.Append(XA9Constants.LOG_VRBS_IS);
            int length = contents.Length;
            output.Append(length.ToString());
            output.Append(length == 1 ? XA9Constants.LOG_VRBS_CNST : XA9Constants.LOG_VRBS_CNST_S);
            output.Append(Environment.NewLine);

            WriteLogEntry(output, EventLogEntryType.Information);

        } // ReportVerboseMessage

        /// <summary>
        /// Write a one line message to the audit log file
        /// Message is prefixed with a standard prefix and suffixed with a new line
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type">Log entry type (e.g. information, warning or message)</param>
        private void ProcessLogEntry(string p_facetsMethodName, DateTime currentTime, string message, EventLogEntryType type)
        {

            StringBuilder output = new StringBuilder(currentTime.ToString(XA9Constants.FRMT_DATE_FULL), 10000);
            output.Append(XA9Constants.LOG_VERSION_PREFIX );
            output.Append(STR_BUILD_VERSION);
            output.Append(XA9Constants.LOG_DATASOURCE_PREFIX );
            output.Append(_currentDatasource);
            output.Append(XA9Constants.LOG_DATABASE_PREFIX );
            output.Append(_currentDatabase);
            output.Append(XA9Constants .LOG_CLAIM_PREFIX);
            output.Append(_currentClaimId.PadRight(13)); //12 + 1 separator space
            output.Append(XA9Constants .LOG_METHD_PREFIX);
            output.Append(p_facetsMethodName.PadRight(24));
            output.Append(message);
            output.Append(Environment.NewLine);

            WriteLogEntry(output, type);

        } // ProcessLogEntry

        /// <summary>
        /// Write the log entry to the proper destination (either log file or the event log)
        /// </summary>
        /// <param name="output">Contents of the log entry</param>
        /// <param name="type">Log entry type (e.g. information, warning or message)</param>
        private void WriteLogEntry(StringBuilder output, EventLogEntryType type)
        {
            //Log entry will be written to the event log (and not the log file) if either config data defines event log
            // as a destination or if a problem (exception) occurred when writing to the log file

            if (GetLogFileName() .Equals ( Logger._writeToEventLog))
            {
                WriteToEventLog(output, type);
            }
            else
            {
                try
                {
                    File.AppendAllText(GetLogFileName(), output.ToString());
                }
                catch
                {
                    WriteToEventLog(output, type);
                }
            }

        } // WriteLogEntry

        /// <summary>
        /// Write the log entry to the Application Log
        /// </summary>
        /// <param name="output">Contents of the log entry</param>
        /// <param name="type">Log entry type (e.g. information, warning or message)</param>
        private void WriteToEventLog(StringBuilder output, EventLogEntryType type)
        {

            EventLog log = new EventLog();
            try
            {
                if (!EventLog.SourceExists(Logger._eventLogSource))
                {
                    EventLog.CreateEventSource(Logger._eventLogSource, null);  // Application log
                }
                ///throw new ArgumentException("Test exception when writing to Application log");  // for testing only
                log.Source = Logger._eventLogSource;
                log.WriteEntry(output.ToString(), type);
            }
            catch
            {
                if (_LoggerTries < _maxLoggerTries)
                {
                    _LoggerTries++;
                    WriteLogEntry(output, System.Diagnostics.EventLogEntryType.Information);
                }
                else
                {
                    _LoggerTries = 0;
                }

            } // try
            finally
            {
                log.Close();
                log.Dispose();
            }

        } // WriteToEventLog

        /// <summary>
        /// Determine audit log destination (e.g. log file name)
        /// </summary>
        /// <returns>Fully qualified name of the audit log file (or special value indicating event log)</returns>
        public string GetLogFileName()
        {
            // (singleton pattern)
            //if (_logFileFullyQualifiedName == null)
            //{
                _logFileFullyQualifiedName = AppConfig.LogFileLocation;
                if (string.IsNullOrEmpty(_logFileFullyQualifiedName))
                {
                    _logFileFullyQualifiedName = Assembly.GetExecutingAssembly().Location;
                    _logFileFullyQualifiedName = Path.GetDirectoryName(_logFileFullyQualifiedName);
                }
                if (_logFileFullyQualifiedName != Logger._writeToEventLog)
                {
                    string _logFileDirectory = _logFileFullyQualifiedName + string.Format(XA9Constants.LOG_PATH, DateTime.Now.ToString(XA9Constants.FRMT_DATE_FOLDER));

                    if (!Directory.Exists(_logFileDirectory))
                    {
                        Directory.CreateDirectory(_logFileDirectory);
                    }
                    _logFileFullyQualifiedName = _logFileDirectory + XA9Constants.LOG_DEFAULT_NAME;
                }
            //}

            return _logFileFullyQualifiedName;

        }  //GetLogFileName

    } // class Logger

} // namespace ClmDMEProc.BusinessLayer
