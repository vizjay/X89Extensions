using CommonUtilities;
using FacetsControlLibrary;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CommonUtilities
{
    public class FacetsBaseControlEx : FacetsBaseControl
    {
        public new Boolean Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
                UserControls.FormControlsEx.SetForegroundWindow(Handle);
            }
        }

        public FacetsBaseControlEx()
            : base()
        {
            BackColor = UserControls.ControlsColor.FormColor;
        }

        public FacetsBaseControlEx(FacetsBaseControl objFBC)
            : base()
        {
            new ObjectEx().CopyObject<FacetsBaseControl>(objFBC, this);
        }

        public new virtual Boolean GetDbRequest(String Sql, ref String XmlResult)
        {
            try
            {
                base.GetDbRequest(Sql, ref XmlResult);
                return true;
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return false;
        }

        public virtual String GetDbRequest(String Sql)
        {
            try
            {
                String XmlResult = String.Empty;
                GetDbRequest(Sql, ref XmlResult);
                return XmlResult;
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return String.Empty;
        }

        public new virtual Boolean GetData(String DataId, ref String XmlResult)
        {
            try
            {
                base.GetData(DataId, ref XmlResult);
                return true;
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return false;
        }

        public virtual String GetData(String DataId)
        {
            try
            {
                String XmlResult = String.Empty;
                GetData(DataId, ref XmlResult);
                return XmlResult;
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return String.Empty;
        }

        public new virtual Boolean GetAllData(String DataId, ref String XmlResult)
        {
            try
            {
                base.GetAllData(DataId, ref XmlResult);
                return true;
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return false;
        }

        public virtual String GetAllData(String DataId)
        {
            try
            {
                String XmlResult = String.Empty;
                GetAllData(DataId, ref XmlResult);
                return XmlResult;
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return String.Empty;
        }

        public new virtual Boolean GetAllRelationData(String DataId, String RelDataId, ref String XmlResult)
        {
            try
            {
                base.GetAllRelationData(DataId, RelDataId, ref XmlResult);
                return true;
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return false;
        }

        public virtual String GetAllRelationData(String DataId, String RelDataId)
        {
            try
            {
                String XmlResult = String.Empty;
                GetAllRelationData(DataId, RelDataId, ref XmlResult);
                return XmlResult;
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return String.Empty;
        }

        public new void SetFacetsControlColor(Control objControl)
        {
            Color backColor = objControl.BackColor;
            try
            {
                base.SetFacetsControlColor(objControl);
            }
            catch
            {
                objControl.BackColor = backColor;
            }
        }

        public void SetFacetsControlColor(Control objControl, Color? SkipColor)
        {
            Color backColor = objControl.BackColor;
            try
            {
                SetFacetsControlColor(objControl);
                if (SkipColor != null)
                {
                    if (objControl.BackColor == SkipColor)
                    {
                        objControl.BackColor = backColor;
                    }
                }
            }
            catch
            {
                objControl.BackColor = backColor;
            }
        }

        public new void SetFacetsControlFont(Control objControl)
        {
            Font font = objControl.Font;
            try
            {
                base.SetFacetsControlFont(objControl);
            }
            catch
            {
                objControl.Font = font;
            }
        }
    }
}
