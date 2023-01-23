using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Forms;

namespace CommonUtilities
{
    public class MessageBoxEx
    {
        private static String _ApplicationName = String.Empty;

        public static String ApplicationName
        {
            get { return _ApplicationName; }
            set { _ApplicationName = value; }
        }

        public static DialogResult Show(String Message, MessageBoxButtons MessageBoxButton = MessageBoxButtons.OK, Icon SystemIcon = null, Boolean IsWordWrap = true, RichTextBoxScrollBars ScrollBar = RichTextBoxScrollBars.None, HorizontalAlignment Alignment = HorizontalAlignment.Center, Boolean ShowMessageOnlyIfNotEmpty = true)
        {
            try
            {
                return frmMessage.Show(Message, MessageBoxButton, SystemIcon, IsWordWrap, ScrollBar, Alignment, ShowMessageOnlyIfNotEmpty);
            }
            catch
            {
            }
            return DialogResult.Retry;
        }

        public static DialogResult ShowInformation(String Message, Boolean ShowMessageOnlyIfNotEmpty = true)
        {
            return Show(Message, ShowMessageOnlyIfNotEmpty: ShowMessageOnlyIfNotEmpty);
        }

        public static DialogResult ShowWarning(String Message, Boolean ShowMessageOnlyIfNotEmpty = true)
        {
            return Show(Message, SystemIcon: SystemIcons.Warning, ShowMessageOnlyIfNotEmpty: ShowMessageOnlyIfNotEmpty);
        }

        public static DialogResult ShowError(String Message, HorizontalAlignment Alignment = HorizontalAlignment.Left, Boolean ShowMessageOnlyIfNotEmpty = true)
        {
            return Show(Message, SystemIcon: SystemIcons.Error, Alignment: Alignment, ShowMessageOnlyIfNotEmpty: ShowMessageOnlyIfNotEmpty);
        }

        public static DialogResult ShowException(Exception objExcpetion, Boolean WriteInnerException = false, Boolean ShowMessageOnlyIfNotEmpty = true)
        {
            return Show(GetExceptionMessage(objExcpetion, WriteInnerException), SystemIcon: SystemIcons.Hand, IsWordWrap: false, ScrollBar: RichTextBoxScrollBars.Both, Alignment: HorizontalAlignment.Left, ShowMessageOnlyIfNotEmpty: ShowMessageOnlyIfNotEmpty);
        }

        public static DialogResult ShowSqlInformation(Connectivity.QueryExecutionInfo objQEI, Boolean ShowMessageOnlyIfNotEmpty = true)
        {
            return ShowInformation(GetSqlMessage(objQEI), ShowMessageOnlyIfNotEmpty);
        }

        public static DialogResult ShowSqlWarning(Connectivity.QueryExecutionInfo objQEI, Boolean ShowMessageOnlyIfNotEmpty = true)
        {
            return ShowWarning(GetSqlMessage(objQEI), ShowMessageOnlyIfNotEmpty);
        }

        public static DialogResult ShowSqlError(Connectivity.QueryExecutionInfo objQEI, Boolean ShowMessageOnlyIfNotEmpty = true)
        {
            return ShowError(GetSqlMessage(objQEI), ShowMessageOnlyIfNotEmpty: ShowMessageOnlyIfNotEmpty);
        }

        public static DialogResult GetInput(String Message, MessageBoxButtons MessageBoxButton = MessageBoxButtons.YesNoCancel, Boolean ShowMessageOnlyIfNotEmpty = true)
        {
            return Show(Message, MessageBoxButton, SystemIcons.Question, ShowMessageOnlyIfNotEmpty: ShowMessageOnlyIfNotEmpty);
        }

        public static String GetExceptionMessage(Exception objException, Boolean WriteInnerException = false)
        {
            try
            {
                if (objException != null)
                {
                    StringBuilder objSB = new StringBuilder();
                    objSB.AppendLine(String.Format("Error Message: {0}", objException.Message));
                    objSB.AppendLine();
                    objSB.AppendLine("Stack Trace:");
                    objSB.AppendLine(objException.StackTrace);
                    objSB.AppendLine();
                    if (WriteInnerException && objException.InnerException != null)
                    {
                        objSB.AppendLine(String.Empty.PadLeft(50, '#'));
                        objSB.AppendLine();
                        objSB.AppendLine(GetExceptionMessage(objException.InnerException, WriteInnerException));
                    }
                    return objSB.ToString();
                }
            }
            catch
            {
            }
            return String.Empty;
        }

        public static String GetSqlMessage(Connectivity.QueryExecutionInfo objQEI, Boolean IsError = false)
        {
            try
            {
                StringBuilder objSB_Default = new StringBuilder();
                objSB_Default.AppendLine(Constants.Messages.TechnicalError);
                objSB_Default.AppendLine();
                if (objQEI != null)
                {
                    StringBuilder objSB = new StringBuilder();
                    ObjectEx objOE = new ObjectEx();
                    objSB.AppendLine("Query:");
                    objSB.AppendLine(objQEI.Sql);
                    objSB.AppendLine();
                    objSB.AppendLine("Xml Result:");
                    objSB.AppendLine(objQEI.XmlResult);
                    objSB.AppendLine();
                    if (objQEI.QEI_Custom != null)
                    {
                        objSB.AppendLine("Custom Message:");
                        objSB.AppendLine(objOE.ToString(objQEI.QEI_Custom));
                    }
                    else if (objQEI.QEI_Facets != null)
                    {
                        objSB.AppendLine("Facets Message:");
                        objSB.AppendLine(objOE.ToString(objQEI.QEI_Facets));
                    }
                    else if (IsError)
                    {
                        objSB.Append(objSB_Default.ToString());
                    }
                    return objSB.ToString();
                }
                else
                {
                    return objSB_Default.ToString();
                }
            }
            catch
            {
            }
            return String.Empty;
        }
    }
}
