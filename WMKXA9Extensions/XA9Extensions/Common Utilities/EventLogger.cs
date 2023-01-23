using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CommonUtilities
{
    public class EventLogger
    {
        private static Boolean _IsAutoIncrementEventID = false;
        private static EventSourceCreationData _objESCD = null;

        public static Boolean IsAutoIncrementEventID
        {
            get { return EventLogger._IsAutoIncrementEventID; }
            set { EventLogger._IsAutoIncrementEventID = value; }
        }

        public static Boolean IsEventSourceCreated
        {
            get
            {
                try
                {
                    if (objESCD != null)
                    {
                        if (!String.IsNullOrEmpty(objESCD.Source))
                        {
                            return EventLog.SourceExists(objESCD.Source);
                        }
                    }
                }
                catch
                {
                }
                return false;
            }
        }

        public static EventSourceCreationData objESCD
        {
            get { return EventLogger._objESCD; }
            set { EventLogger._objESCD = value; }
        }

        public EventLogger()
        {
        }

        public static Boolean SetEventID(Int32 EventID, Boolean IsAutoIncrement = false)
        {
            try
            {
                ApplicationData.EventID = EventID;
                EventLogger.IsAutoIncrementEventID = IsAutoIncrementEventID;
            }
            catch
            {
            }
            return false;
        }

        public static Boolean ResetEventID(Boolean IsAutoIncrement = false)
        {
            try
            {
                ApplicationData.EventID = 0;
                EventLogger.IsAutoIncrementEventID = IsAutoIncrementEventID;
            }
            catch
            {
            }
            return false;
        }

        public static Boolean CreateEventSource(String Source, String LogName = Constants.Utilities.EventLogName)
        {
            try
            {
                objESCD = new EventSourceCreationData(Source, LogName);
                objESCD.MachineName = Environment.MachineName;
                if (!EventLog.SourceExists(objESCD.Source))
                {
                    EventLog.CreateEventSource(objESCD);
                }
                return IsEventSourceCreated;
            }
            catch
            {
            }
            return false;
        }

        public static Boolean WriteMessage(String Message, EventLogEntryType Type = EventLogEntryType.Information, Int16 Category = 0)
        {
            try
            {
                if (IsEventSourceCreated)
                {
                    Int32 EventID = ApplicationData.EventID;
                    if (IsAutoIncrementEventID)
                    {
                        EventID = ApplicationData.NextEventID;
                    }
                    EventLog.WriteEntry(objESCD.Source, Message, Type, EventID, Category);
                }
            }
            catch
            {
            }
            return false;
        }

        public static Boolean WriteException(Exception objException, Boolean WriteInnerException = false, Boolean ShowMessage = true)
        {
            try
            {
                if (ShowMessage)
                {
                    MessageBoxEx.ShowException(objException);
                }
                return WriteMessage(MessageBoxEx.GetExceptionMessage(objException, WriteInnerException), EventLogEntryType.Error);
            }
            catch
            {
            }
            return false;
        }

        public static Boolean WriteSqlErrorMessage(Connectivity.QueryExecutionInfo objQEI)
        {
            try
            {
                return WriteMessage(MessageBoxEx.GetSqlMessage(objQEI, true), EventLogEntryType.Error);
            }
            catch
            {
            }
            return false;
        }
    }
}
