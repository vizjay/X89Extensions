using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonUtilities
{
    public class ApplicationData
    {
        private static Enumerations.Status _AppStatus = Enumerations.Status.Normal;
        private static Int32 _EventID = Constants.DefaultValues.Integer;

        public static Int32 EventID
        {
            get { return ApplicationData._EventID; }
            set { ApplicationData._EventID = value; }
        }

        public static Int32 NextEventID
        {
            get
            {
                if (EventID < Int32.MaxValue)
                {
                    EventID++;
                }
                else
                {
                    EventID = Constants.DefaultValues.Integer;
                }
                return EventID;
            }
        }

        public static Boolean SetNextEventID()
        {
            try
            {
                Int32 EventID = NextEventID;
                return true;
            }
            catch
            {
            }
            return false;
        }

        public static Enumerations.Status AppStatus
        {
            get { return ApplicationData._AppStatus; }
            set { ApplicationData._AppStatus = value; }
        }

        public class Table
        {
            protected String _FacetsData = String.Empty;
            protected Connectivity.QueryExecutionInfo _objQEI = new Connectivity.QueryExecutionInfo();

            public static String DataID
            {
                get { return Constants.XmlNodeDataID.Custom; }
            }

            public virtual Connectivity.QueryExecutionInfo objQEI
            {
                get { return _objQEI; }
            }

            public virtual String FacetsData
            {
                get { return _FacetsData; }
            }

            public Table()
            {
                _FacetsData = String.Empty;
                _objQEI = new Connectivity.QueryExecutionInfo();
            }
        }
    }
}
