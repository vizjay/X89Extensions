using FacetsControlLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CommonUtilities
{
    public class UserControls
    {
        public class ControlsColor
        {
            public static Color OnEnter = Color.SkyBlue;
            public static Color OnLeave = Color.White;
            public static Color OnError = Color.Orange;
            public static Color OnMouseHover = Color.LightSteelBlue;
            public static Color OnMouseMove = Color.LightSteelBlue;
            public static Color OnValid = Color.DarkGreen;
            public static Color OnInfo = Color.DarkBlue;
            public static Color OnWarning = Color.Coral;
            public static Color FormColor = Color.WhiteSmoke;
        }

        public class FormControlsEx
        {
            public const String CategoryNameBackColor = "Back Color Properties";
            public const String CategoryNameForeColor = "Fore Color Properties";
            public const String CategoryNameAppearance = "Appearance";
            public const String CategoryNameBehavior = "Behavior";
            public static ToolTip objToolTip = new ToolTip();

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern Boolean SetForegroundWindow(IntPtr hWnd);

            public virtual Boolean FormatGrid(ref DataGridView objDGV, Int32 ColumnHeadersHeight = 30, Int32 RowHeadersWidth = 30, Int32 RowTemplateHeight = 20)
            {
                try
                {
                    objDGV.ColumnHeadersHeight = ColumnHeadersHeight;
                    objDGV.RowHeadersWidth = RowHeadersWidth;
                    objDGV.RowTemplate.Height = RowTemplateHeight;
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean InitToolStripStatusLabel(ref ToolStripStatusLabel objTSSL)
            {
                try
                {
                    if (objTSSL == null)
                    {
                        objTSSL = new ToolStripStatusLabel();
                    }
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean SetDateTimePickerValue(String FieldName, DateTime NewDateTime)
            {
                try
                {
                    Type objType = typeof(DateTimePicker);
                    if (objType != null)
                    {
                        FieldInfo objFI = objType.GetField(FieldName, BindingFlags.Public | BindingFlags.Static);
                        if (objFI != null)
                        {
                            if (objFI.FieldType == typeof(DateTime))
                            {
                                objFI.SetValue(new DateTimePicker(), NewDateTime);
                                return true;
                            }
                        }
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean SetDateTimePickerMinDateTime(DateTime NewDateTime)
            {
                try
                {
                    return SetDateTimePickerValue("MinDateTime", NewDateTime);
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean SetDateTimePickerMinDateTime()
            {
                try
                {
                    return SetDateTimePickerMinDateTime(Constants.DefaultValues.Date);
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean SetDateTimePickerMaxDateTime(DateTime NewDateTime)
            {
                try
                {
                    return SetDateTimePickerValue("MaxDateTime", NewDateTime);
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean SetDateTimePickerMaxDateTime()
            {
                try
                {
                    return SetDateTimePickerMaxDateTime(Constants.DefaultValues.TerminationDate);
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual void ShowToolTip(Control objControl, String StatusText = Constants.DefaultValues.String, Int32 Duration = Constants.Utilities.Duration)
            {
                try
                {
                    ToolTip objTT = new ToolTip();
                    objTT.InitialDelay = 0;
                    objTT.IsBalloon = true;
                    objTT.Show(StatusText, objControl, Duration);
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
            }

            public virtual String GetValueFromDataGrid(DataGridView objDGV, Int32 Row, Int32 Column)
            {
                try
                {
                    if (objDGV != null)
                    {
                        if (Row >= 0 && Row < objDGV.Rows.Count)
                        {
                            return GetValueFromDataGrid(objDGV.Rows[Row], Column);
                        }
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return String.Empty;
            }

            public virtual List<String> GetValuesOfColumnFromDataGrid(DataGridView objDGV, Int32 Column)
            {
                try
                {
                    if (objDGV != null)
                    {
                        List<String> strList = new List<String>();
                        foreach(DataGridViewRow objDGVR in objDGV.Rows)
                        {
                            strList.Add(GetValueFromDataGrid(objDGVR, Column));
                        }
                        return strList;
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return new List<String>();
            }

            public virtual List<String> GetValuesOfRowFromDataGrid(DataGridView objDGV, Int32 Row)
            {
                try
                {
                    if (objDGV != null)
                    {
                        if (Row >= 0 && Row < objDGV.Rows.Count)
                        {
                            List<String> strList = new List<String>();
                            for (Int32 Ix = 0; Ix < objDGV.Rows[Row].Cells.Count; Ix++)
                            {
                                strList.Add(GetValueFromDataGrid(objDGV.Rows[Row], Ix));
                            }
                            return strList;
                        }
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return new List<String>();
            }

            public virtual String GetValueFromDataGrid(DataGridViewRow objDGVR, Int32 Column)
            {
                try
                {
                    if (objDGVR != null)
                    {
                        if (Column >= 0 && Column < objDGVR.Cells.Count)
                        {
                            DataGridViewCell objCell = objDGVR.Cells[Column];
                            if(objCell!=null)
                            {
                                Object objValue = objCell.Value;
                                if (objValue != null)
                                {
                                    return objValue.ToString();
                                }
                            }
                        }
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return String.Empty;
            }
        }

        public class ButtonEx : Button
        {
            private Color _LastUsedBackColor = Control.DefaultBackColor;

            private Color _BackColorOnEnter = ControlsColor.OnEnter;
            private Color _BackColorOnLeave = Control.DefaultBackColor;
            private Color _BackColorOnError = ControlsColor.OnError;
            private Color _BackColorOnMouseHover = ControlsColor.OnMouseHover;
            private Color _BackColorOnMouseMove = ControlsColor.OnMouseMove;
            private String _ToolTipText = String.Empty;

            protected virtual Color LastUsedBackColor
            {
                get { return _LastUsedBackColor; }
                set { _LastUsedBackColor = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the cursor enters it.")]
            [Browsable(true)]
            public virtual Color BackColorOnEnter
            {
                get { return _BackColorOnEnter; }
                set { _BackColorOnEnter = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the cursor leaves it.")]
            [Browsable(true)]
            public virtual Color BackColorOnLeave
            {
                get { return _BackColorOnLeave; }
                set
                {
                    _BackColorOnLeave = value;
                    LastUsedBackColor = value;
                }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the validation error occurs on the control's text.")]
            [Browsable(true)]
            public virtual Color BackColorOnError
            {
                get { return _BackColorOnError; }
                set { _BackColorOnError = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the mouse hovers over it.")]
            [Browsable(true)]
            public virtual Color BackColorOnMouseHover
            {
                get { return _BackColorOnMouseHover; }
                set { _BackColorOnMouseHover = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the mouse moves over it.")]
            [Browsable(true)]
            public virtual Color BackColorOnMouseMove
            {
                get { return _BackColorOnMouseMove; }
                set { _BackColorOnMouseMove = value; }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets the ToolTip Text of the control.")]
            [Browsable(true)]
            public virtual String ToolTipText
            {
                get { return _ToolTipText; }
                set
                {
                    _ToolTipText = value.TrimStart(Environment.NewLine.ToCharArray()).TrimEnd(Environment.NewLine.ToCharArray());
                    SetToolTip();
                }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Gets or sets the background color of the control.")]
            [Browsable(true)]
            public override Color BackColor
            {
                get
                {
                    return base.BackColor;
                }
                set
                {
                    base.BackColor = value;
                    if (value == Control.DefaultBackColor)
                    {
                        UseVisualStyleBackColor = true;
                    }
                }
            }

            protected override void OnEnter(EventArgs e)
            {
                try
                {
                    BackColor = BackColorOnEnter;
                    LastUsedBackColor = BackColor;
                    base.OnEnter(e);
                }
                catch
                {
                }
            }

            protected override void OnLeave(EventArgs e)
            {
                try
                {
                    BackColor = BackColorOnLeave;
                    LastUsedBackColor = BackColor;
                    base.OnLeave(e);
                }
                catch
                {
                }
            }

            protected override void OnMouseHover(EventArgs e)
            {
                try
                {
                    BackColor = BackColorOnMouseHover;
                    base.OnMouseHover(e);
                }
                catch
                {
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                try
                {
                    BackColor = BackColorOnMouseMove;
                    base.OnMouseMove(e);
                }
                catch
                {
                }
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                try
                {
                    BackColor = LastUsedBackColor;
                    base.OnMouseLeave(e);
                }
                catch
                {
                }
            }

            public ButtonEx()
                : base()
            {
                ToolTipText = String.Empty;
                Size = Constants.Sizes.ControlSmall;
            }

            public virtual Boolean SetToolTip()
            {
                try
                {
                    FormControlsEx.objToolTip.SetToolTip(this, ToolTipText);
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual void SetBackColorAsError()
            {
                try
                {
                    BackColor = BackColorOnError;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
            }
        }

        public class ComboBoxEx : ComboBox
        {
            private Color _LastUsedBackColor = Color.White;
            private Boolean _IsSetFocusForError = false;

            private Color _BackColorOnEnter = Color.LightSteelBlue;
            private Color _BackColorOnLeave = ControlsColor.OnLeave;
            private Color _BackColorOnError = ControlsColor.OnError;
            private Color _BackColorOnMouseHover = ControlsColor.OnMouseHover;
            private Color _BackColorOnMouseMove = ControlsColor.OnMouseMove;
            private Enumerations.ControlFocusModes _ControlFocusMode = Enumerations.ControlFocusModes.Select;
            private String _ToolTipText = String.Empty;
            private Int32 _DefaultSelectedIndex = -1;
            private Int32 _MinSelectedIndex = -1;
            private Enumerations.ComboBoxValueModes _ValueMode = Enumerations.ComboBoxValueModes.ValueFollowedByDescription;
            private Char _ValueDelimiter = '\0';
            private ToolStripStatusLabelEx _StatusLabel = new ToolStripStatusLabelEx();
            private Label _LabelAssociated = new Label();
            private String _OnLeaveErrorMessage = String.Empty;
            private Boolean _OnLeaveErrorSetFocus = false;

            protected virtual Color LastUsedBackColor
            {
                get { return _LastUsedBackColor; }
                set { _LastUsedBackColor = value; }
            }

            protected virtual Boolean IsSetFocusForError
            {
                get { return _IsSetFocusForError; }
                set { _IsSetFocusForError = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the cursor enters it.")]
            [Browsable(true)]
            public virtual Color BackColorOnEnter
            {
                get { return _BackColorOnEnter; }
                set { _BackColorOnEnter = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the cursor leaves it.")]
            [Browsable(true)]
            public virtual Color BackColorOnLeave
            {
                get { return _BackColorOnLeave; }
                set
                {
                    _BackColorOnLeave = value;
                    LastUsedBackColor = value;
                }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the validation error occurs on the control's text.")]
            [Browsable(true)]
            public virtual Color BackColorOnError
            {
                get { return _BackColorOnError; }
                set { _BackColorOnError = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the mouse hovers over it.")]
            [Browsable(true)]
            public virtual Color BackColorOnMouseHover
            {
                get { return _BackColorOnMouseHover; }
                set { _BackColorOnMouseHover = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the mouse moves over it.")]
            [Browsable(true)]
            public virtual Color BackColorOnMouseMove
            {
                get { return _BackColorOnMouseMove; }
                set { _BackColorOnMouseMove = value; }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Gets or sets the mode to display the value.")]
            [Browsable(true)]
            public virtual Enumerations.ComboBoxValueModes ValueMode
            {
                get { return _ValueMode; }
                set { _ValueMode = value; }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Gets or sets the delimiter used for displaying value.")]
            [Browsable(true)]
            public virtual Char ValueDelimiter
            {
                get { return _ValueDelimiter; }
                set { _ValueDelimiter = value; }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [Description("Gets or sets the ToolStripStatusLabelEx control to be used for displaying error.")]
            [Browsable(true)]
            public virtual ToolStripStatusLabelEx StatusLabel
            {
                get { return _StatusLabel; }
                set { _StatusLabel = value; }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [Description("Gets or sets the Label control associated with it.")]
            [Browsable(true)]
            public virtual Label LabelAssociated
            {
                get { return _LabelAssociated; }
                set
                {
                    _LabelAssociated = value;
                    SetOnLeaveErrorMessage();
                }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Gets or sets the error message to be displayed on leaving the control.")]
            [Browsable(true)]
            public virtual String OnLeaveErrorMessage
            {
                get { return _OnLeaveErrorMessage; }
                set { _OnLeaveErrorMessage = value; }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Gets or sets the flag used to focusing the control if error occurs on leaving it.")]
            [Browsable(true)]
            public virtual Boolean OnLeaveErrorSetFocus
            {
                get { return _OnLeaveErrorSetFocus; }
                set { _OnLeaveErrorSetFocus = value; }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Gets or sets the focus mode for the control.")]
            [Browsable(true)]
            public virtual Enumerations.ControlFocusModes ControlFocusMode
            {
                get { return _ControlFocusMode; }
                set { _ControlFocusMode = value; }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Gets the value associated with selected item of this control.")]
            [Browsable(true)]
            public virtual String ItemValue
            {
                get
                {
                    if (SelectedIndex >= 0)
                    {
                        List<String> strListParts = Text.Split(ValueDelimiter).ToList();
                        switch (ValueMode)
                        {
                            case Enumerations.ComboBoxValueModes.OnlyValue:
                                return strListParts[0];
                            case Enumerations.ComboBoxValueModes.ValueFollowedByDescription:
                                if (strListParts.Count > 1)
                                {
                                    if (strListParts[0].Length > 1)
                                    {
                                        return strListParts[0].Substring(0, strListParts[0].Length - 1);
                                    }
                                }
                                return strListParts[0];
                            case Enumerations.ComboBoxValueModes.OnlyDescription:
                                break;
                            case Enumerations.ComboBoxValueModes.DescriptionFollowedByValue:
                                if (strListParts.Count > 1)
                                {
                                    if (strListParts[1].Length > 1)
                                    {
                                        return strListParts[1].Substring(1);
                                    }
                                }
                                break;
                            default:
                                break;
                        }
                    }
                    return String.Empty;
                }
                set
                {
                    Boolean IsValueSet = false;
                    foreach (String Item in Items)
                    {
                        String strCompareValue = String.Empty;
                        List<String> strListParts = Item.Split(ValueDelimiter).ToList();
                        switch (ValueMode)
                        {
                            case Enumerations.ComboBoxValueModes.OnlyValue:
                                strCompareValue = Item;
                                break;
                            case Enumerations.ComboBoxValueModes.OnlyDescription:
                                break;
                            case Enumerations.ComboBoxValueModes.ValueFollowedByDescription:
                                if (strListParts.Count > 1)
                                {
                                    if (strListParts[0].Length > 1)
                                    {
                                        strCompareValue = strListParts[0].Substring(0, strListParts[0].Length - 1);
                                    }
                                }
                                else
                                {
                                    strCompareValue = Item;
                                }
                                break;
                            case Enumerations.ComboBoxValueModes.DescriptionFollowedByValue:
                                if (strListParts.Count > 1)
                                {
                                    if (strListParts[1].Length > 1)
                                    {
                                        strCompareValue = strListParts[1].Substring(1);
                                    }
                                }
                                break;
                            default:
                                break;
                        }

                        if (strCompareValue == value)
                        {
                            Text = Item;
                            IsValueSet = true;
                            break;
                        }
                    }

                    if (!IsValueSet)
                    {
                        SetFromDefaultSelectedIndex();
                    }
                }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Gets the value associated with selected item of this control.")]
            [Browsable(true)]
            public virtual String ItemDescription
            {
                get
                {
                    if (SelectedIndex >= 0)
                    {
                        List<String> strListParts = Text.Split(ValueDelimiter).ToList();
                        switch (ValueMode)
                        {
                            case Enumerations.ComboBoxValueModes.OnlyValue:
                                break;
                            case Enumerations.ComboBoxValueModes.ValueFollowedByDescription:
                                if (strListParts.Count > 1)
                                {
                                    if (strListParts[1].Length > 1)
                                    {
                                        return strListParts[1].Substring(1);
                                    }
                                }
                                break;
                            case Enumerations.ComboBoxValueModes.OnlyDescription:
                                return strListParts[0];
                            case Enumerations.ComboBoxValueModes.DescriptionFollowedByValue:
                                if (strListParts.Count > 1)
                                {
                                    if (strListParts[0].Length > 1)
                                    {
                                        return strListParts[0].Substring(0, strListParts[0].Length - 1);
                                    }
                                }
                                return strListParts[0];
                            default:
                                break;
                        }
                    }
                    return String.Empty;
                }
                set
                {
                    Boolean IsValueSet = false;
                    foreach (String Item in Items)
                    {
                        String strCompareValue = String.Empty;
                        List<String> strListParts = Item.Split(ValueDelimiter).Select(obj => obj).ToList();
                        switch (ValueMode)
                        {
                            case Enumerations.ComboBoxValueModes.OnlyValue:
                                break;
                            case Enumerations.ComboBoxValueModes.OnlyDescription:
                                strCompareValue = Item;
                                break;
                            case Enumerations.ComboBoxValueModes.ValueFollowedByDescription:
                                if (strListParts.Count > 1)
                                {
                                    if (strListParts[1].Length > 1)
                                    {
                                        strCompareValue = strListParts[1].Substring(1);
                                    }
                                }
                                break;
                            case Enumerations.ComboBoxValueModes.DescriptionFollowedByValue:
                                if (strListParts.Count > 1)
                                {
                                    if (strListParts[0].Length > 1)
                                    {
                                        strCompareValue = strListParts[0].Substring(0, strListParts[0].Length - 1);
                                    }
                                }
                                else
                                {
                                    strCompareValue = Item;
                                }
                                break;
                            default:
                                break;
                        }

                        if (strCompareValue == value)
                        {
                            Text = Item;
                            IsValueSet = true;
                            break;
                        }
                    }

                    if (!IsValueSet)
                    {
                        SetFromDefaultSelectedIndex();
                    }
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets the ToolTip Text of the control.")]
            [Browsable(true)]
            public virtual String ToolTipText
            {
                get { return _ToolTipText; }
                set
                {
                    _ToolTipText = value.TrimStart(Environment.NewLine.ToCharArray()).TrimEnd(Environment.NewLine.ToCharArray());
                    SetToolTip();
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the index of the default item to be selected.")]
            [Browsable(true)]
            public virtual Int32 DefaultSelectedIndex
            {
                get
                { return _DefaultSelectedIndex; }
                set
                {
                    _DefaultSelectedIndex = value;
                    SetFromDefaultSelectedIndex();
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the minimum index of the item to be selected on leave.")]
            [Browsable(true)]
            public virtual Int32 MinSelectedIndex
            {
                get { return _MinSelectedIndex; }
                set
                {
                    _MinSelectedIndex = value;
                    SetOnLeaveErrorMessage();
                }
            }

            protected override void OnEnter(EventArgs e)
            {
                try
                {
                    if (!IsSetFocusForError)
                    {
                        BackColor = BackColorOnEnter;
                        LastUsedBackColor = BackColor;
                    }
                    else
                    {
                        IsSetFocusForError = false;
                    }
                    base.OnEnter(e);
                }
                catch
                {
                }
            }

            protected override void OnLeave(EventArgs e)
            {
                try
                {
                    BackColor = BackColorOnLeave;
                    LastUsedBackColor = BackColor;
                    if (IsValidOnLeave())
                    {
                        StatusLabel.SetForeColorAsValid();
                        base.OnLeave(e);
                    }
                }
                catch
                {
                }
            }

            protected override void OnMouseHover(EventArgs e)
            {
                try
                {
                    BackColor = BackColorOnMouseHover;
                    base.OnMouseHover(e);
                }
                catch
                {
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                try
                {
                    BackColor = BackColorOnMouseMove;
                    base.OnMouseMove(e);
                }
                catch
                {
                }
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                try
                {
                    BackColor = LastUsedBackColor;
                    base.OnMouseLeave(e);
                }
                catch
                {
                }
            }

            protected override void OnSelectedIndexChanged(EventArgs e)
            {
                base.OnSelectedIndexChanged(e);
                SelectedValue = Text;
            }

            public ComboBoxEx()
                : base()
            {
                Int32 MinWidth = MinimumSize.Width;
                MinimumSize = Constants.Sizes.ControlSmall;
                Size = Constants.Sizes.ControlSmall;
                MinimumSize = new System.Drawing.Size(MinWidth, Size.Height);
                DropDownStyle = ComboBoxStyle.DropDownList;
                DefaultSelectedIndex = -1;
                OnLeaveErrorMessage = String.Empty;
                MinSelectedIndex = -1;
                ToolTipText = String.Empty;
                ValueMode = Enumerations.ComboBoxValueModes.ValueFollowedByDescription;
                ValueDelimiter = '|';
                OnLeaveErrorSetFocus = true;
                Font = Constants.Fonts.CourierNew;
            }

            public virtual Boolean SetToolTip()
            {
                try
                {
                    FormControlsEx.objToolTip.SetToolTip(this, ToolTipText);
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            protected virtual Boolean SetOnLeaveErrorMessage()
            {
                try
                {
                    String Prefix = String.Empty;
                    if (LabelAssociated != null)
                    {
                        if (!String.IsNullOrEmpty(LabelAssociated.Text))
                        {
                            Prefix = String.Format("{0}: ", LabelAssociated.Text);
                        }
                    }

                    if (MinSelectedIndex >= 0)
                    {
                        OnLeaveErrorMessage = String.Format("{0}Selected Index must be >= {1}.", Prefix, MinSelectedIndex);
                    }
                    else
                    {
                        OnLeaveErrorMessage = String.Empty;
                    }
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean SetFocus()
            {
                try
                {
                    switch (ControlFocusMode)
                    {
                        case Enumerations.ControlFocusModes.None:
                            break;
                        case Enumerations.ControlFocusModes.Focus:
                            Focus();
                            break;
                        case Enumerations.ControlFocusModes.Select:
                            Select();
                            break;
                        default:
                            break;
                    }
                    return true;
                }
                catch
                {
                }
                return false;
            }

            public virtual void SetBackColorAsError(String StatusText = Constants.DefaultValues.StringNull, Boolean UseOnLeaveErrorMessageIfEmpty = true, Boolean IsFocus = false)
            {
                try
                {
                    if (StatusText == Constants.DefaultValues.StringNull && UseOnLeaveErrorMessageIfEmpty)
                    {
                        StatusText = OnLeaveErrorMessage;
                    }

                    if (OnLeaveErrorSetFocus || IsFocus)
                    {
                        IsSetFocusForError = true;
                        SetFocus();
                    }

                    if (StatusText != null)
                    {
                        StatusLabel.SetForeColorAsError(StatusText);
                    }
                    BackColor = BackColorOnError;
                    LastUsedBackColor = BackColor;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
            }

            public virtual Boolean SetFromDefaultSelectedIndex(Boolean OnlyIfNotSelectedAny = false)
            {
                try
                {
                    if (DefaultSelectedIndex > -1 && DefaultSelectedIndex < Items.Count)
                    {
                        if ((OnlyIfNotSelectedAny & SelectedIndex < 0) | (!OnlyIfNotSelectedAny))
                        {
                            SelectedIndex = DefaultSelectedIndex;
                            BackColor = LastUsedBackColor;
                        }
                    }
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean AddItem(String Item)
            {
                try
                {
                    Items.Add(Item);
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean AddItems(List<Object> Items)
            {
                try
                {
                    foreach (Object Item in Items)
                    {
                        this.Items.Add(Item);
                    }
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean LoadItems(List<Object> Items, String DefaultItem = "<Select>")
            {
                try
                {
                    DefaultItem = DefaultItem.Trim();
                    this.Items.Clear();
                    if (!String.IsNullOrEmpty(DefaultItem))
                    {
                        this.Items.Add(DefaultItem);
                        DefaultSelectedIndex = 0;
                    }

                    if (AddItems(Items))
                    {
                        if (MinSelectedIndex >= this.Items.Count)
                        {
                            MinSelectedIndex = this.Items.Count - 1;
                        }
                        return SetFromDefaultSelectedIndex();
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean LoadItemFromEnum(Enum Value, Boolean IsSkipItemIfDescriptionEmpty = true)
            {
                try
                {
                    String EnumName = Value.ToString();
                    String EnumDescription = Enumerations.GetDescriptionFromValue(Value);

                    if (ValueMode == Enumerations.ComboBoxValueModes.ValueFollowedByDescription || ValueMode == Enumerations.ComboBoxValueModes.DescriptionFollowedByValue)
                    {
                        if (!String.IsNullOrEmpty(EnumDescription))
                        {
                            if (ValueMode == Enumerations.ComboBoxValueModes.DescriptionFollowedByValue)
                            {
                                AddItem(String.Format("{0} {1} {2}", EnumDescription, ValueDelimiter, EnumName));
                            }
                            else if (ValueMode == Enumerations.ComboBoxValueModes.ValueFollowedByDescription)
                            {
                                AddItem(String.Format("{0} {1} {2}", EnumName, ValueDelimiter, EnumDescription));
                            }
                        }
                        else if (!IsSkipItemIfDescriptionEmpty)
                        {
                            AddItem(EnumName);
                        }
                    }
                    else
                    {
                        if (ValueMode == Enumerations.ComboBoxValueModes.OnlyValue || (String.IsNullOrEmpty(EnumDescription) && !IsSkipItemIfDescriptionEmpty))
                        {
                            AddItem(EnumName);
                        }
                        else
                        {
                            if (!(String.IsNullOrEmpty(EnumDescription) && IsSkipItemIfDescriptionEmpty))
                            {
                                AddItem(EnumDescription);
                            }
                        }
                    }
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean LoadItemsFromEnum<T>(String DefaultItem = "<Select>", Boolean IsSkipItemIfDescriptionEmpty = true)
            {
                try
                {
                    Items.Clear();
                    Type objType = typeof(T);
                    if (objType.IsEnum)
                    {
                        Enum DefaultValue = (Enum)Enum.ToObject(objType, 0);

                        if (DefaultItem != null)
                        {
                            AddItem(DefaultItem);
                        }

                        foreach (Enum Value in Enum.GetValues(objType))
                        {
                            if (!LoadItemFromEnum(Value, IsSkipItemIfDescriptionEmpty))
                            {
                                return false;
                            }
                        }
                    }
                    if (MinSelectedIndex >= Items.Count)
                    {
                        MinSelectedIndex = Items.Count - 1;
                    }
                    SetFromDefaultSelectedIndex();
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean IsValidOnLeave(Boolean IsSkipValidationIfNoneSelected = true, Boolean IsFocus = false)
            {
                try
                {
                    if (SelectedIndex >= MinSelectedIndex || IsSkipValidationIfNoneSelected)
                    {
                        IsSetFocusForError = false;
                        return true;
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                SetBackColorAsError(IsFocus: IsFocus);
                return false;
            }
        }

        public class DateTimePickerEx : DateTimePicker
        {
            private const Int32 WM_ERASEBKGND = 0x0014;

            private Color _LastUsedBackColor = Color.White;
            private Boolean _IsSetFocusForError = false;

            private Color _BackColor = Color.White;
            private Color _BackColorOnEnter = ControlsColor.OnEnter;
            private Color _BackColorOnLeave = ControlsColor.OnLeave;
            private Color _BackColorOnError = ControlsColor.OnError;
            private Color _BackColorOnMouseHover = ControlsColor.OnMouseHover;
            private Color _BackColorOnMouseMove = ControlsColor.OnMouseMove;
            private Enumerations.ControlFocusModes _ControlFocusMode = Enumerations.ControlFocusModes.Select;
            private String _ToolTipText = String.Empty;
            private DateTime _DefaultValue = Constants.DefaultValues.Date;
            private String _TextEx = String.Empty;
            private String _TextExCustomFormat = "MM/dd/yyyy hh:mm:ss.fff tt";
            private ToolStripStatusLabelEx _StatusLabel = new ToolStripStatusLabelEx();
            private Label _LabelAssociated = new Label();
            private String _OnLeaveErrorMessage = String.Empty;
            private Boolean _OnLeaveErrorSetFocus = false;

            protected virtual Color LastUsedBackColor
            {
                get { return _LastUsedBackColor; }
                set { _LastUsedBackColor = value; }
            }

            protected virtual Boolean IsSetFocusForError
            {
                get { return _IsSetFocusForError; }
                set { _IsSetFocusForError = value; }
            }

            protected virtual String ToolTipCaption
            {
                get
                {
                    StringBuilder objSB = new StringBuilder();
                    objSB.AppendLine("Use Arrows ⇄ to navigate between Date/Month/Year");
                    objSB.AppendLine("Use Arrows ⇅ or ± to increment/decrement values");
                    return objSB.ToString().TrimStart(Environment.NewLine.ToCharArray()).TrimEnd(Environment.NewLine.ToCharArray());
                }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [Browsable(true)]
            [DescriptionAttribute("Gets or sets the background color of the control.")]
            public override Color BackColor
            {
                get { return _BackColor; }
                set
                {
                    _BackColor = value;
                    Invalidate();
                }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the cursor enters it.")]
            [Browsable(true)]
            public virtual Color BackColorOnEnter
            {
                get { return _BackColorOnEnter; }
                set { _BackColorOnEnter = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the cursor leaves it.")]
            [Browsable(true)]
            public virtual Color BackColorOnLeave
            {
                get { return _BackColorOnLeave; }
                set
                {
                    _BackColorOnLeave = value;
                    LastUsedBackColor = value;
                }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the validation error occurs on the control's text.")]
            [Browsable(true)]
            public virtual Color BackColorOnError
            {
                get { return _BackColorOnError; }
                set { _BackColorOnError = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the mouse hovers over it.")]
            [Browsable(true)]
            public virtual Color BackColorOnMouseHover
            {
                get { return _BackColorOnMouseHover; }
                set { _BackColorOnMouseHover = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the mouse moves over it.")]
            [Browsable(true)]
            public virtual Color BackColorOnMouseMove
            {
                get { return _BackColorOnMouseMove; }
                set { _BackColorOnMouseMove = value; }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets the ToolTip Text of the control.")]
            [Browsable(true)]
            public virtual String ToolTipText
            {
                get { return _ToolTipText; }
                set
                {
                    _ToolTipText = value.TrimStart(Environment.NewLine.ToCharArray()).TrimEnd(Environment.NewLine.ToCharArray());
                    SetToolTip();
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the default value for the control.")]
            [Browsable(true)]
            public virtual DateTime DefaultValue
            {
                get { return _DefaultValue; }
                set { _DefaultValue = value; }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets the Date and Time values in the format specified in Text2CustomFormat.")]
            [Browsable(true)]
            public virtual String TextEx
            {
                get
                {
                    SetTextEx();
                    return _TextEx;
                }
            }

            public virtual String TextExCustomFormat
            {
                get { return _TextExCustomFormat; }
                set
                {
                    _TextExCustomFormat = value.Trim();
                    SetTextEx();
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the flag to determine whether Date and Time values are displayed using standard or custom formatting.")]
            [Browsable(true)]
            public new virtual DateTimePickerFormat Format
            {
                get
                {
                    return base.Format;
                }
                set
                {
                    base.Format = value;
                    SetTextEx();
                }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [Description("Gets or sets the ToolStripStatusLabelEx control to be used for displaying error.")]
            [Browsable(true)]
            public virtual ToolStripStatusLabelEx StatusLabel
            {
                get { return _StatusLabel; }
                set { _StatusLabel = value; }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [Description("Gets or sets the Label control associated with it.")]
            [Browsable(true)]
            public virtual Label LabelAssociated
            {
                get { return _LabelAssociated; }
                set { _LabelAssociated = value; }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Gets or sets the error message to be displayed on leaving the control.")]
            [Browsable(true)]
            public virtual String OnLeaveErrorMessage
            {
                get { return _OnLeaveErrorMessage; }
                set
                {
                    String Prefix = String.Empty;
                    if (LabelAssociated != null)
                    {
                        if (!String.IsNullOrEmpty(LabelAssociated.Text))
                        {
                            Prefix = String.Format("{0}: ", LabelAssociated.Text);
                        }

                        if (value.StartsWith(Prefix))
                        {
                            Prefix = String.Empty;
                        }
                    }

                    if (!String.IsNullOrEmpty(value))
                    {
                        _OnLeaveErrorMessage = String.Format("{0}{1}", Prefix, value);
                    }
                    else
                    {
                        _OnLeaveErrorMessage = String.Empty;
                    }
                }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Gets or sets the flag used to focusing the control if error occurs on leaving it.")]
            [Browsable(true)]
            public virtual Boolean OnLeaveErrorSetFocus
            {
                get { return _OnLeaveErrorSetFocus; }
                set { _OnLeaveErrorSetFocus = value; }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Gets or sets the focus mode for the control.")]
            [Browsable(true)]
            public virtual Enumerations.ControlFocusModes ControlFocusMode
            {
                get { return _ControlFocusMode; }
                set { _ControlFocusMode = value; }
            }

            protected override void OnEnter(EventArgs e)
            {
                try
                {
                    if (!IsSetFocusForError)
                    {
                        BackColor = BackColorOnEnter;
                        LastUsedBackColor = BackColor;
                    }
                    else
                    {
                        IsSetFocusForError = false;
                    }
                    base.OnEnter(e);
                }
                catch
                {
                }
            }

            protected override void OnLeave(EventArgs e)
            {
                try
                {
                    BackColor = BackColorOnLeave;
                    LastUsedBackColor = BackColor;
                    base.OnLeave(e);
                }
                catch
                {
                }
            }

            protected override void OnMouseHover(EventArgs e)
            {
                try
                {
                    BackColor = BackColorOnMouseHover;
                    base.OnMouseHover(e);
                }
                catch
                {
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                try
                {
                    BackColor = BackColorOnMouseMove;
                    base.OnMouseMove(e);
                }
                catch
                {
                }
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                try
                {
                    BackColor = LastUsedBackColor;
                    base.OnMouseLeave(e);
                }
                catch
                {
                }
            }

            protected override void WndProc(ref System.Windows.Forms.Message Message)
            {
                try
                {
                    if (Message.Msg == WM_ERASEBKGND)
                    {
                        Graphics graphics = Graphics.FromHdc(Message.WParam);
                        graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
                        graphics.Dispose();
                        return;
                    }
                    base.WndProc(ref Message);
                }
                catch
                {
                }
            }

            public DateTimePickerEx()
                : base()
            {
                Format = DateTimePickerFormat.Custom;
                CustomFormat = "MM/dd/yyyy";
                Int32 MinWidth = MinimumSize.Width;
                MinimumSize = Constants.Sizes.ControlSmall;
                Size = Constants.Sizes.ControlSmall;
                MinimumSize = new System.Drawing.Size(MinWidth, Size.Height);
                Value = DateTime.Now;
                DefaultValue = Constants.DefaultValues.Date;
                ToolTipText = ToolTipCaption;
                OnLeaveErrorMessage = String.Empty;
                OnLeaveErrorSetFocus = true;
            }

            public virtual Boolean SetToolTip()
            {
                try
                {
                    FormControlsEx.objToolTip.SetToolTip(this, ToolTipText);
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean SetFocus()
            {
                try
                {
                    switch (ControlFocusMode)
                    {
                        case Enumerations.ControlFocusModes.None:
                            break;
                        case Enumerations.ControlFocusModes.Focus:
                            Focus();
                            break;
                        case Enumerations.ControlFocusModes.Select:
                            Select();
                            break;
                        default:
                            break;
                    }
                    return true;
                }
                catch
                {
                }
                return false;
            }

            public virtual void SetBackColorAsError(String StatusText = Constants.DefaultValues.StringNull, Boolean UseOnLeaveErrorMessageIfEmpty = true, Boolean IsFocus = false)
            {
                try
                {
                    if (StatusText == Constants.DefaultValues.StringNull && UseOnLeaveErrorMessageIfEmpty)
                    {
                        StatusText = OnLeaveErrorMessage;
                    }

                    if (OnLeaveErrorSetFocus || IsFocus)
                    {
                        IsSetFocusForError = true;
                        SetFocus();
                    }

                    if (StatusText != null)
                    {
                        StatusLabel.SetForeColorAsError(StatusText);
                    }
                    BackColor = BackColorOnError;
                    LastUsedBackColor = BackColor;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
            }

            public virtual Boolean SetFromDefaultValue(Boolean IsSkipValidation = true)
            {
                try
                {
                    Text = Text.TrimEnd();
                    if ((!IsSkipValidation && Value == Constants.DefaultValues.Date) || (IsSkipValidation))
                    {
                        Value = DefaultValue;
                    }
                    BackColor = LastUsedBackColor;
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean SetTextEx()
            {
                try
                {
                    switch (base.Format)
                    {
                        case DateTimePickerFormat.Long:
                            _TextEx = base.Value.ToLongDateString();
                            break;
                        case DateTimePickerFormat.Short:
                            _TextEx = base.Value.ToShortDateString();
                            break;
                        case DateTimePickerFormat.Time:
                            _TextEx = base.Value.ToLongTimeString();
                            break;
                        case DateTimePickerFormat.Custom:
                            if (String.IsNullOrEmpty(TextExCustomFormat))
                            {
                                _TextEx = Text;
                            }
                            else
                            {
                                _TextEx = base.Value.ToString(TextExCustomFormat);
                            }
                            break;
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }
        }

        public class MaskedTextBoxEx : MaskedTextBox
        {
            private Color _LastUsedBackColor = Color.White;
            private Boolean _IsSetFocusForError = false;

            private Color _BackColorOnEnter = ControlsColor.OnEnter;
            private Color _BackColorOnLeave = ControlsColor.OnLeave;
            private Color _BackColorOnError = ControlsColor.OnError;
            private Color _BackColorOnMouseHover = ControlsColor.OnMouseHover;
            private Color _BackColorOnMouseMove = ControlsColor.OnMouseMove;
            private Enumerations.ControlFocusModes _ControlFocusMode = Enumerations.ControlFocusModes.Select;
            private ToolStripStatusLabelEx _StatusLabel = new ToolStripStatusLabelEx();
            private Label _LabelAssociated = new Label();
            private Regex _RegExpOnLeave = null;
            private Regex _RegExpForEmpty = null;
            private String _OnLeaveErrorMessage = String.Empty;
            private Boolean _OnLeaveErrorSetFocus = false;
            private Boolean _EnableDefaultToolTip = true;
            private String _ToolTipText = String.Empty;
            private String _DefaultValue = String.Empty;

            protected virtual Color LastUsedBackColor
            {
                get { return _LastUsedBackColor; }
                set { _LastUsedBackColor = value; }
            }

            protected virtual Boolean IsSetFocusForError
            {
                get { return _IsSetFocusForError; }
                set { _IsSetFocusForError = value; }
            }

            protected virtual String ToolTipCaption
            {
                get
                {
                    StringBuilder objSB = new StringBuilder();
                    objSB.AppendLine(String.Format("Mask: {0}", base.Mask));
                    return objSB.ToString().TrimStart(Environment.NewLine.ToCharArray()).TrimEnd(Environment.NewLine.ToCharArray());
                }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the cursor enters it.")]
            [Browsable(true)]
            public virtual Color BackColorOnEnter
            {
                get { return _BackColorOnEnter; }
                set { _BackColorOnEnter = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the cursor leaves it.")]
            [Browsable(true)]
            public virtual Color BackColorOnLeave
            {
                get { return _BackColorOnLeave; }
                set
                {
                    _BackColorOnLeave = value;
                    LastUsedBackColor = value;
                }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the validation error occurs on the control's text.")]
            [Browsable(true)]
            public virtual Color BackColorOnError
            {
                get { return _BackColorOnError; }
                set { _BackColorOnError = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the mouse hovers over it.")]
            [Browsable(true)]
            public virtual Color BackColorOnMouseHover
            {
                get { return _BackColorOnMouseHover; }
                set { _BackColorOnMouseHover = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the mouse moves over it.")]
            [Browsable(true)]
            public virtual Color BackColorOnMouseMove
            {
                get { return _BackColorOnMouseMove; }
                set { _BackColorOnMouseMove = value; }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [Description("Gets or sets the ToolStripStatusLabelEx control to be used for displaying error.")]
            [Browsable(true)]
            public virtual ToolStripStatusLabelEx StatusLabel
            {
                get { return _StatusLabel; }
                set { _StatusLabel = value; }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [Description("Gets or sets the Label control associated with it.")]
            [Browsable(true)]
            public virtual Label LabelAssociated
            {
                get { return _LabelAssociated; }
                set
                {
                    _LabelAssociated = value;
                    SetOnLeaveErrorMessage();
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the regular expression used to validate the control's text while leaving it.")]
            [Browsable(true)]
            public virtual String RegularExpressionOnLeave
            {
                get
                {
                    if (_RegExpOnLeave != null)
                    {
                        return _RegExpOnLeave.ToString();
                    }
                    return String.Empty;
                }
                set
                {
                    try
                    {
                        _RegExpOnLeave = new Regex(value);
                    }
                    catch
                    {
                    }
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the regular expression for empty text in the control.")]
            [Browsable(true)]
            public virtual String RegularExpressionForEmpty
            {
                get
                {
                    if (_RegExpForEmpty != null)
                    {
                        return _RegExpForEmpty.ToString();
                    }
                    return String.Empty;
                }
                set
                {
                    try
                    {
                        _RegExpForEmpty = new Regex(value);
                    }
                    catch
                    {
                    }
                }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Gets or sets the error message to be displayed on leaving the control.")]
            [Browsable(true)]
            public virtual String OnLeaveErrorMessage
            {
                get { return _OnLeaveErrorMessage; }
                set { _OnLeaveErrorMessage = value; }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Gets or sets the flag used to focusing the control if error occurs on leaving it.")]
            [Browsable(true)]
            public virtual Boolean OnLeaveErrorSetFocus
            {
                get { return _OnLeaveErrorSetFocus; }
                set { _OnLeaveErrorSetFocus = value; }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Gets or sets the focus mode for the control.")]
            [Browsable(true)]
            public virtual Enumerations.ControlFocusModes ControlFocusMode
            {
                get { return _ControlFocusMode; }
                set { _ControlFocusMode = value; }
            }

            protected virtual Regex RegExpOnLeave
            {
                get { return _RegExpOnLeave; }
                set { _RegExpOnLeave = value; }
            }

            protected virtual Regex RegExpForEmpty
            {
                get { return _RegExpForEmpty; }
                set { _RegExpForEmpty = value; }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [Description("Enables or disables the default ToolTip on the control.")]
            [Browsable(true)]
            public virtual Boolean EnableDefaultToolTip
            {
                get { return _EnableDefaultToolTip; }
                set { _EnableDefaultToolTip = value; }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [Description("Gets the ToolTip Text of the control.")]
            [Browsable(true)]
            public virtual String ToolTipText
            {
                get { return _ToolTipText; }
                set
                {
                    _ToolTipText = value.TrimStart(Environment.NewLine.ToCharArray()).TrimEnd(Environment.NewLine.ToCharArray());
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [Description("Gets or sets the default value for the control.")]
            [Browsable(true)]
            public virtual String DefaultValue
            {
                get
                {
                    return _DefaultValue;
                }
                set
                {
                    _DefaultValue = value;
                    SetToolTip();
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [Description("Gets or sets the input mask to use at run time.")]
            [Browsable(true)]
            public new virtual String Mask
            {
                get { return base.Mask; }
                set
                {
                    base.Mask = value;
                    SetOnLeaveErrorMessage();
                    _ToolTipText = ToolTipCaption;
                    SetToolTip();
                }
            }

            protected override void OnEnter(EventArgs e)
            {
                try
                {
                    if (!IsSetFocusForError)
                    {
                        BackColor = BackColorOnEnter;
                        LastUsedBackColor = BackColor;
                    }
                    else
                    {
                        IsSetFocusForError = false;
                    }
                    base.OnEnter(e);
                }
                catch
                {
                }
            }

            protected override void OnLeave(EventArgs e)
            {
                try
                {
                    BackColor = BackColorOnLeave;
                    LastUsedBackColor = BackColor;
                    if (IsValidTextOnLeave())
                    {
                        StatusLabel.SetForeColorAsValid();
                        base.OnLeave(e);
                    }
                }
                catch
                {
                }
            }

            protected override void OnMouseHover(EventArgs e)
            {
                try
                {
                    BackColor = BackColorOnMouseHover;
                    base.OnMouseHover(e);
                }
                catch
                {
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                try
                {
                    BackColor = BackColorOnMouseMove;
                    base.OnMouseMove(e);
                }
                catch
                {
                }
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                try
                {
                    BackColor = LastUsedBackColor;
                    base.OnMouseLeave(e);
                }
                catch
                {
                }
            }

            public MaskedTextBoxEx()
                : base()
            {
                Int32 MinWidth = MinimumSize.Width;
                MinimumSize = Constants.Sizes.ControlSmall;
                Size = Constants.Sizes.ControlSmall;
                MinimumSize = new System.Drawing.Size(MinWidth, Size.Height);
                EnableDefaultToolTip = true;
                ToolTipText = ToolTipCaption;
                ContextMenu = new ContextMenu();
                DefaultValue = String.Empty;
                OnLeaveErrorMessage = String.Empty;
                OnLeaveErrorSetFocus = true;
                SetToolTip();
                SetOnLeaveErrorMessage();
            }

            protected virtual Boolean SetOnLeaveErrorMessage()
            {
                try
                {
                    String Prefix = String.Empty;
                    if (LabelAssociated != null)
                    {
                        if (!String.IsNullOrEmpty(LabelAssociated.Text))
                        {
                            Prefix = String.Format("{0}: ", LabelAssociated.Text);
                        }
                    }
                    OnLeaveErrorMessage = String.Format("{0}Entered text must be in the format {1}.", Prefix, Mask);

                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean SetToolTip()
            {
                try
                {
                    if (EnableDefaultToolTip)
                    {
                        ToolTipText = ToolTipCaption;
                    }
                    FormControlsEx.objToolTip.SetToolTip(this, ToolTipText);
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean IsValidTextOnLeave(Boolean IsSkipValidationIfEmpty = true, Boolean IsFocus = false)
            {
                try
                {
                    if (RegExpOnLeave != null)
                    {
                        if (RegExpOnLeave.IsMatch(Text))
                        {
                            IsSetFocusForError = false;
                            return true;
                        }
                        else if (IsSkipValidationIfEmpty && RegExpForEmpty != null)
                        {
                            if (RegExpForEmpty.IsMatch(Text))
                            {
                                IsSetFocusForError = false;
                                return true;
                            }
                        }
                    }
                    else
                    {
                        IsSetFocusForError = false;
                        return true;
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                SetBackColorAsError(IsFocus: IsFocus);
                return false;
            }

            public virtual Boolean SetFocus()
            {
                try
                {
                    switch (ControlFocusMode)
                    {
                        case Enumerations.ControlFocusModes.None:
                            break;
                        case Enumerations.ControlFocusModes.Focus:
                            Focus();
                            break;
                        case Enumerations.ControlFocusModes.Select:
                            Select();
                            break;
                        default:
                            break;
                    }
                    return true;
                }
                catch
                {
                }
                return false;
            }

            public virtual void SetBackColorAsError(String StatusText = Constants.DefaultValues.StringNull, Boolean UseOnLeaveErrorMessageIfEmpty = true, Boolean IsFocus = false)
            {
                try
                {
                    if (StatusText == Constants.DefaultValues.StringNull && UseOnLeaveErrorMessageIfEmpty)
                    {
                        StatusText = OnLeaveErrorMessage;
                    }

                    if (OnLeaveErrorSetFocus || IsFocus)
                    {
                        IsSetFocusForError = true;
                        SetFocus();
                    }

                    if (StatusText != null)
                    {
                        StatusLabel.SetForeColorAsError(StatusText);
                    }
                    BackColor = BackColorOnError;
                    LastUsedBackColor = BackColor;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
            }

            public virtual Boolean SetFromDefaultValue(Boolean OnlyIfEmpty = false)
            {
                try
                {
                    if ((OnlyIfEmpty & String.IsNullOrEmpty(Text)) | (!OnlyIfEmpty))
                    {
                        base.Text = DefaultValue;
                        BackColor = LastUsedBackColor;
                    }
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }
        }

        public class ToolStripStatusLabelEx : ToolStripStatusLabel
        {
            private Color _ForeColorValid = ControlsColor.OnValid;
            private Color _ForeColorInfo = ControlsColor.OnInfo;
            private Color _ForeColorWarning = ControlsColor.OnWarning;
            private Color _ForeColorError = Color.Red;

            [Category(FormControlsEx.CategoryNameForeColor)]
            [DescriptionAttribute("Gets or sets the foreground color used for setting valid message.")]
            [Browsable(true)]
            public virtual Color ForeColorValid
            {
                get { return _ForeColorValid; }
                set { _ForeColorValid = value; }
            }

            [Category(FormControlsEx.CategoryNameForeColor)]
            [DescriptionAttribute("Gets or sets the foreground color used for setting information.")]
            [Browsable(true)]
            public virtual Color ForeColorInfo
            {
                get { return _ForeColorInfo; }
                set { _ForeColorInfo = value; }
            }

            [Category(FormControlsEx.CategoryNameForeColor)]
            [DescriptionAttribute("Gets or sets the foreground color used for setting warning message.")]
            [Browsable(true)]
            public Color ForeColorWarning
            {
                get { return _ForeColorWarning; }
                set { _ForeColorWarning = value; }
            }

            [Category(FormControlsEx.CategoryNameForeColor)]
            [DescriptionAttribute("Gets or sets the foreground color used for setting error message.")]
            [Browsable(true)]
            public virtual Color ForeColorError
            {
                get { return _ForeColorError; }
                set { _ForeColorError = value; }
            }

            public ToolStripStatusLabelEx()
                : base()
            {
                ForeColorValid = ControlsColor.OnValid;
                ForeColorInfo = ControlsColor.OnInfo;
                ForeColorError = ControlsColor.OnError;
                SetForeColorAsInfo();
                BorderSides = ToolStripStatusLabelBorderSides.All;
                BorderStyle = Border3DStyle.SunkenInner;
                TextAlign = ContentAlignment.MiddleLeft;
                Spring = false;
                Width = Constants.Sizes.ControlMedium.Width;
            }

            public virtual void SetForeColorAsValid(String Text = Constants.DefaultValues.String)
            {
                try
                {
                    if (Text != null)
                    {
                        this.Text = Text;
                    }
                    ForeColor = ForeColorValid;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);

                }
            }

            public virtual void SetForeColorAsInfo(String Text = Constants.DefaultValues.String)
            {
                try
                {
                    if (Text != null)
                    {
                        this.Text = Text;
                    }
                    ForeColor = ForeColorInfo;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
            }

            public virtual void SetForeColorAsWarning(String Text = Constants.DefaultValues.String)
            {
                try
                {
                    if (Text != null)
                    {
                        this.Text = Text;
                    }
                    ForeColor = ForeColorWarning;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
            }

            public virtual void SetForeColorAsError(String Text = Constants.DefaultValues.String)
            {
                try
                {
                    if (Text != null)
                    {
                        this.Text = Text;
                    }
                    ForeColor = ForeColorError;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
            }
        }

        public class RichTextBoxEx : RichTextBox
        {
            private Color _LastUsedBackColor = Color.White;

            private Color _BackColorOnEnter = ControlsColor.OnEnter;
            private Color _BackColorOnLeave = ControlsColor.OnLeave;
            private Color _BackColorOnError = ControlsColor.OnError;
            private Color _BackColorOnMouseHover = ControlsColor.OnMouseHover;
            private Color _BackColorOnMouseMove = ControlsColor.OnMouseMove;

            protected virtual Color LastUsedBackColor
            {
                get { return _LastUsedBackColor; }
                set { _LastUsedBackColor = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the cursor enters it.")]
            [Browsable(true)]
            public virtual Color BackColorOnEnter
            {
                get { return _BackColorOnEnter; }
                set { _BackColorOnEnter = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the cursor leaves it.")]
            [Browsable(true)]
            public virtual Color BackColorOnLeave
            {
                get { return _BackColorOnLeave; }
                set
                {
                    _BackColorOnLeave = value;
                    LastUsedBackColor = value;
                }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the validation error occurs on the control's text.")]
            [Browsable(true)]
            public virtual Color BackColorOnError
            {
                get { return _BackColorOnError; }
                set { _BackColorOnError = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the mouse hovers over it.")]
            [Browsable(true)]
            public virtual Color BackColorOnMouseHover
            {
                get { return _BackColorOnMouseHover; }
                set { _BackColorOnMouseHover = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the mouse moves over it.")]
            [Browsable(true)]
            public virtual Color BackColorOnMouseMove
            {
                get { return _BackColorOnMouseMove; }
                set { _BackColorOnMouseMove = value; }
            }

            protected override void OnEnter(EventArgs e)
            {
                try
                {
                    BackColor = BackColorOnEnter;
                    LastUsedBackColor = BackColor;
                    base.OnEnter(e);
                }
                catch
                {
                }
            }

            protected override void OnLeave(EventArgs e)
            {
                try
                {
                    BackColor = BackColorOnLeave;
                    LastUsedBackColor = BackColor;
                    base.OnLeave(e);
                }
                catch
                {
                }
            }

            protected override void OnMouseHover(EventArgs e)
            {
                try
                {
                    BackColor = BackColorOnMouseHover;
                    base.OnMouseHover(e);
                }
                catch
                {
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                try
                {
                    BackColor = BackColorOnMouseMove;
                    base.OnMouseMove(e);
                }
                catch
                {
                }
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                try
                {
                    BackColor = LastUsedBackColor;
                    base.OnMouseLeave(e);
                }
                catch
                {
                }
            }

            public RichTextBoxEx()
                : base()
            {
                Size = Constants.Sizes.ControlLarge;
                Font = Constants.Fonts.CourierNew;
                ContextMenu = new ContextMenu();
            }
        }

        public class TextBoxEx : TextBox
        {
            private Color _LastUsedBackColor = Color.White;
            private Boolean _IsSetFocusForError = false;
            private Boolean _IsKeyBackSpace = false;
            private Boolean _IsKeyDelete = false;
            private Boolean _IsKeyCopy = false;
            private Boolean _IsKeyCut = false;
            private Boolean _IsKeyPaste = false;

            private Color _BackColorOnEnter = ControlsColor.OnEnter;
            private Color _BackColorOnLeave = ControlsColor.OnLeave;
            private Color _BackColorOnError = ControlsColor.OnError;
            private Color _BackColorOnMouseHover = ControlsColor.OnMouseHover;
            private Color _BackColorOnMouseMove = ControlsColor.OnMouseMove;
            private Color _BackColorOnLeaveReadOnly = Control.DefaultBackColor;
            private Enumerations.ControlFocusModes _ControlFocusMode = Enumerations.ControlFocusModes.Select;

            private Enumerations.TrimTextOptions _TrimTextOnLeave = Enumerations.TrimTextOptions.None;
            private Enumerations.PadTextOptions _PadTextOption = Enumerations.PadTextOptions.None;
            private Char _PadChar = '\0';
            private Boolean _EnableDefaultToolTip = true;
            private String _ToolTipText = String.Empty;
            private Int32 _MinLength = 0;
            private String _DefaultValue = String.Empty;
            private ToolStripStatusLabelEx _StatusLabel = new ToolStripStatusLabelEx();
            private Label _LabelAssociated = new Label();
            private String _OnLeaveErrorMessage = String.Empty;
            private Boolean _OnLeaveErrorSetFocus = false;

            protected virtual Color LastUsedBackColor
            {
                get { return _LastUsedBackColor; }
                set { _LastUsedBackColor = value; }
            }

            protected virtual Boolean IsSetFocusForError
            {
                get { return _IsSetFocusForError; }
                set { _IsSetFocusForError = value; }
            }

            protected virtual Boolean IsKeyBackSpace
            {
                get { return _IsKeyBackSpace; }
                set { _IsKeyBackSpace = value; }
            }

            protected virtual Boolean IsKeyDelete
            {
                get { return _IsKeyDelete; }
                set { _IsKeyDelete = value; }
            }

            protected virtual Boolean IsKeyCopy
            {
                get { return _IsKeyCopy; }
                set { _IsKeyCopy = value; }
            }

            protected virtual Boolean IsKeyCut
            {
                get { return _IsKeyCut; }
                set { _IsKeyCut = value; }
            }

            protected virtual Boolean IsKeyPaste
            {
                get { return _IsKeyPaste; }
                set { _IsKeyPaste = value; }
            }

            protected virtual String ToolTipCaption
            {
                get
                {
                    StringBuilder objSB = new StringBuilder();
                    if (MinLength > 0)
                    {
                        objSB.AppendLine(String.Format("Minimum number of characters allowed to be typed or pasted: {0}", MinLength));
                    }
                    objSB.AppendLine(String.Format("Maximum number of characters allowed to be typed or pasted: {0}", MaxLength));

                    if (!String.IsNullOrEmpty(DefaultValue))
                    {
                        objSB.AppendLine(String.Format("Default value is: {0}", DefaultValue));
                    }
                    return objSB.ToString().TrimStart(Environment.NewLine.ToCharArray()).TrimEnd(Environment.NewLine.ToCharArray());
                }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the cursor enters it.")]
            [Browsable(true)]
            public virtual Color BackColorOnEnter
            {
                get { return _BackColorOnEnter; }
                set { _BackColorOnEnter = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the cursor leaves it.")]
            [Browsable(true)]
            public virtual Color BackColorOnLeave
            {
                get { return _BackColorOnLeave; }
                set
                {
                    _BackColorOnLeave = value;
                    LastUsedBackColor = value;
                }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the validation error occurs on the control's text.")]
            [Browsable(true)]
            public virtual Color BackColorOnError
            {
                get { return _BackColorOnError; }
                set { _BackColorOnError = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the mouse hovers over it.")]
            [Browsable(true)]
            public virtual Color BackColorOnMouseHover
            {
                get { return _BackColorOnMouseHover; }
                set { _BackColorOnMouseHover = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the mouse moves over it.")]
            [Browsable(true)]
            public virtual Color BackColorOnMouseMove
            {
                get { return _BackColorOnMouseMove; }
                set { _BackColorOnMouseMove = value; }
            }

            [Category(FormControlsEx.CategoryNameBackColor)]
            [DescriptionAttribute("Gets or sets the background color used for the control when the cursor leaves it and ReadOnly mode is enabled.")]
            [Browsable(true)]
            public virtual Color BackColorOnLeaveReadOnly
            {
                get { return _BackColorOnLeaveReadOnly; }
                set { _BackColorOnLeaveReadOnly = value; }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [Description("Gets or sets a value indicating whether the contents of the TextBox control can be changed.")]
            [Browsable(true)]
            public new virtual Boolean ReadOnly
            {
                get
                {
                    return base.ReadOnly;
                }
                set
                {
                    base.ReadOnly = value;
                    if (value)
                    {
                        BackColor = BackColorOnLeaveReadOnly;
                    }
                    else
                    {
                        BackColor = BackColorOnLeave;
                    }
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [Description("Gets or sets the Trim Text Option to be used when the cursor leaves the control.")]
            [Browsable(true)]
            public virtual Enumerations.TrimTextOptions TrimTextOnLeave
            {
                get { return _TrimTextOnLeave; }
                set { _TrimTextOnLeave = value; }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the Pad Text Option for Padding Characters to the Text of the control.")]
            [Browsable(true)]
            public virtual Enumerations.PadTextOptions PadTextOption
            {
                get { return _PadTextOption; }
                set
                {
                    _PadTextOption = value;
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the Padding Character for the control. Only one character is allowed.")]
            [Browsable(true)]
            public virtual Char PadChar
            {
                get { return _PadChar; }
                set
                {
                    _PadChar = value;
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [Description("Enables or disables the default ToolTip on the control.")]
            [Browsable(true)]
            public Boolean EnableDefaultToolTip
            {
                get { return _EnableDefaultToolTip; }
                set
                {
                    _EnableDefaultToolTip = value;
                    SetToolTip();
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [Description("Gets the ToolTip Text of the control.")]
            [Browsable(true)]
            public virtual String ToolTipText
            {
                get { return _ToolTipText; }
                set
                {
                    _ToolTipText = value.TrimStart(Environment.NewLine.ToCharArray()).TrimEnd(Environment.NewLine.ToCharArray());
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [Description("Gets or sets the minimum number of characters the user can type or paste into the text box control.")]
            [Browsable(true)]
            public virtual Int32 MinLength
            {
                get { return _MinLength; }
                set
                {
                    if (value < 0)
                    {
                        value = 0;
                    }
                    _MinLength = value;
                    if (value > base.MaxLength)
                    {
                        base.MaxLength = value;
                    }
                    SetToolTip();
                    SetOnLeaveErrorMessage();
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [Description("Gets or sets the maximum number of characters the user can type or paste into the text box control.")]
            [Browsable(true)]
            public override Int32 MaxLength
            {
                get
                {
                    return base.MaxLength;
                }
                set
                {
                    base.MaxLength = value;
                    if (value < _MinLength)
                    {
                        _MinLength = value;
                    }
                    SetToolTip();
                    SetOnLeaveErrorMessage();
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [Description("Gets or sets the default value for the control.")]
            [Browsable(true)]
            public virtual String DefaultValue
            {
                get
                {
                    return GetTrimmedText(_DefaultValue);
                }
                set
                {
                    _DefaultValue = GetTrimmedText(value);
                    SetToolTip();
                }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [Description("Gets or sets the ToolStripStatusLabelEx control to be used for displaying error.")]
            [Browsable(true)]
            public virtual ToolStripStatusLabelEx StatusLabel
            {
                get { return _StatusLabel; }
                set { _StatusLabel = value; }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [Description("Gets or sets the Label control associated with it.")]
            [Browsable(true)]
            public virtual Label LabelAssociated
            {
                get { return _LabelAssociated; }
                set
                {
                    _LabelAssociated = value;
                    SetOnLeaveErrorMessage();
                }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Gets or sets the error message to be displayed on leaving the control.")]
            [Browsable(true)]
            public virtual String OnLeaveErrorMessage
            {
                get { return _OnLeaveErrorMessage; }
                set { _OnLeaveErrorMessage = value; }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Gets or sets the flag used to focusing the control if error occurs on leaving it.")]
            [Browsable(true)]
            public virtual Boolean OnLeaveErrorSetFocus
            {
                get { return _OnLeaveErrorSetFocus; }
                set { _OnLeaveErrorSetFocus = value; }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Gets or sets the focus mode for the control.")]
            [Browsable(true)]
            public virtual Enumerations.ControlFocusModes ControlFocusMode
            {
                get { return _ControlFocusMode; }
                set { _ControlFocusMode = value; }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Gets or sets the text contents of the text box.")]
            [Browsable(true)]
            public override String Text
            {
                get
                {
                    return base.Text;
                }
                set
                {
                    if (value.Length > base.MaxLength && value.Length > 0)
                    {
                        value = value.Substring(0, base.MaxLength);
                    }
                    base.Text = value;
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets the trimmed text based on the option selected from the control.")]
            [Browsable(true)]
            public virtual String TrimmedText
            {
                get
                {
                    return GetTrimmedText();
                }
            }

            protected override void OnEnter(EventArgs e)
            {
                try
                {
                    if (!IsSetFocusForError)
                    {
                        BackColor = BackColorOnEnter;
                        LastUsedBackColor = BackColor;
                    }
                    else
                    {
                        IsSetFocusForError = false;
                    }
                    base.OnEnter(e);
                }
                catch
                {
                }
            }

            protected override void OnLeave(EventArgs e)
            {
                try
                {
                    ReadOnly = ReadOnly;
                    LastUsedBackColor = BackColor;
                    if (IsValidTextOnLeave(IsSkipPadIfEmpty: true))
                    {
                        StatusLabel.SetForeColorAsValid();
                        base.OnLeave(e);
                    }
                }
                catch
                {
                }
            }

            protected override void OnMouseCaptureChanged(EventArgs e)
            {
                base.OnMouseCaptureChanged(e);
            }

            protected override void OnMouseHover(EventArgs e)
            {
                try
                {
                    BackColor = BackColorOnMouseHover;
                    base.OnMouseHover(e);
                }
                catch
                {
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                try
                {
                    BackColor = BackColorOnMouseMove;
                    base.OnMouseMove(e);
                }
                catch
                {
                }
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                try
                {
                    IsKeyBackSpace = false;
                    IsKeyDelete = false;
                    IsKeyCopy = false;
                    IsKeyCut = false;
                    IsKeyPaste = false;

                    //To Handle Backspace and Delete Key
                    switch (e.KeyCode)
                    {
                        case Keys.Back:
                            IsKeyBackSpace = true;
                            break;
                        case Keys.Delete:
                            IsKeyDelete = true;
                            if (GetNewValue(((Char)127).ToString()).Length < MinLength && MinLength > 0)
                            {
                                //e.Handled = true;
                            }
                            break;
                    }

                    //To Handle Cut, Copy and Paste Keys
                    if (Control.ModifierKeys == Keys.Control)
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.C:
                                IsKeyCopy = true;
                                break;
                            case Keys.V:
                                IsKeyPaste = true;
                                break;
                            case Keys.X:
                                IsKeyCut = true;
                                break;
                        }
                    }

                    base.OnKeyDown(e);
                }
                catch
                {
                }
            }

            protected override void OnKeyPress(KeyPressEventArgs e)
            {
                try
                {
                    if (IsKeyCut)// | IsKeyBackSpace
                    {
                        if (GetNewValue(e.KeyChar.ToString()).Length < MinLength && MinLength > 0)
                        {
                            e.Handled = true;
                        }
                    }
                    else if (IsKeyPaste)
                    {
                        String ClipText = Clipboard.GetText();
                        if (GetNewValue(ClipText).Length > MaxLength)
                        {
                            e.Handled = true;
                        }
                    }
                    base.OnKeyPress(e);
                }
                catch
                {
                }
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                try
                {
                    BackColor = LastUsedBackColor;
                    base.OnMouseLeave(e);
                }
                catch
                {
                }
            }

            public TextBoxEx()
                : base()
            {
                Int32 MinWidth = MinimumSize.Width;
                MinimumSize = Constants.Sizes.ControlSmall;
                Size = Constants.Sizes.ControlSmall;
                MinimumSize = new System.Drawing.Size(MinWidth, Size.Height);
                MinLength = 0;
                DefaultValue = String.Empty;
                TrimTextOnLeave = Enumerations.TrimTextOptions.None;
                PadTextOption = Enumerations.PadTextOptions.None;
                PadChar = ' ';
                EnableDefaultToolTip = true;
                ToolTipText = ToolTipCaption;
                OnLeaveErrorMessage = String.Empty;
                OnLeaveErrorSetFocus = true;
                ContextMenu = new ContextMenu();
                SetToolTip();
            }

            public virtual Boolean SetToolTip(String ToolTipCaption = Constants.DefaultValues.StringNull)
            {
                try
                {
                    if (ToolTipCaption == null)
                    {
                        ToolTipCaption = this.ToolTipCaption;
                    }

                    if (EnableDefaultToolTip)
                    {
                        ToolTipText = ToolTipCaption;
                    }
                    FormControlsEx.objToolTip.SetToolTip(this, ToolTipText);
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            protected virtual Boolean SetOnLeaveErrorMessage()
            {
                try
                {
                    String Prefix = String.Empty;
                    if (LabelAssociated != null)
                    {
                        if (!String.IsNullOrEmpty(LabelAssociated.Text))
                        {
                            Prefix = String.Format("{0}: ", LabelAssociated.Text);
                        }
                    }

                    if (MinLength > 0 && MinLength == MaxLength)
                    {
                        OnLeaveErrorMessage = String.Format("{0}Length of the entered text must be equal to {1}.", Prefix, MinLength);
                    }
                    else if (MinLength > 0)
                    {
                        OnLeaveErrorMessage = String.Format("{0}Length of the entered text must be between {1} and {2}.", Prefix, MinLength, MaxLength);
                    }
                    else
                    {
                        OnLeaveErrorMessage = String.Empty;
                    }
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean IsValidTextOnLeave(Boolean IsSkipValidationIfEmpty = true, Boolean IsSkipPadIfEmpty = false, Boolean IsFocus = false)
            {
                try
                {
                    Text = GetNormalizedText(IsSkipPadIfEmpty: IsSkipPadIfEmpty);
                    if (Text.Length >= MinLength || (IsSkipValidationIfEmpty && String.IsNullOrEmpty(Text)))
                    {
                        IsSetFocusForError = false;
                        return true;
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                SetBackColorAsError(IsFocus: IsFocus);
                return false;
            }

            public virtual String GetTrimmedText(String Text = Constants.DefaultValues.String)
            {
                try
                {
                    if (Text == null)
                    {
                        Text = this.Text;
                    }

                    switch (TrimTextOnLeave)
                    {
                        case Enumerations.TrimTextOptions.None:
                            break;
                        case Enumerations.TrimTextOptions.BothEnds:
                            Text = Text.Trim();
                            break;
                        case Enumerations.TrimTextOptions.Start:
                            Text = Text.TrimStart();
                            break;
                        case Enumerations.TrimTextOptions.End:
                            Text = Text.TrimEnd();
                            break;
                        default:
                            break;
                    }
                    return Text;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return String.Empty;
            }

            public virtual String GetPaddedText(String Text, Boolean IsSkipPadIfEmpty = false, Enumerations.PadTextOptions? PadTextOption = null)
            {
                try
                {
                    if (PadTextOption == null)
                    {
                        PadTextOption = this.PadTextOption;
                    }
                    if (PadTextOption != Enumerations.PadTextOptions.None && !(IsSkipPadIfEmpty && String.IsNullOrEmpty(Text)))
                    {
                        if (String.IsNullOrEmpty(PadChar.ToString()))
                        {
                            PadChar = ' ';
                        }

                        Text = GetTrimmedText(Text);

                        if (PadTextOption == Enumerations.PadTextOptions.Both || PadTextOption == Enumerations.PadTextOptions.Left)
                        {
                            Text = Text.PadLeft(MaxLength, PadChar);
                        }

                        if (PadTextOption == Enumerations.PadTextOptions.Both || PadTextOption == Enumerations.PadTextOptions.Right)
                        {
                            Text = Text.PadRight(MaxLength, PadChar);
                        }
                    }
                    return Text;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return String.Empty;
            }

            public virtual Boolean PadText()
            {
                try
                {
                    Text = GetPaddedText(Text);
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual String GetNormalizedText(String Text = null, Boolean IsSkipPadIfEmpty = false)
            {
                try
                {
                    if (Text == null)
                    {
                        Text = this.Text;
                    }

                    Text = GetTrimmedText(Text);
                    if (!String.IsNullOrEmpty(DefaultValue) && String.IsNullOrEmpty(Text))
                    {
                        Text = DefaultValue;
                    }
                    return GetPaddedText(Text, IsSkipPadIfEmpty);
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return String.Empty;
            }

            public virtual String GetNewValue(String NewChars, String Text = null)
            {
                try
                {
                    if (Text == null)
                    {
                        Text = this.Text;
                    }
                    Text = GetTrimmedText(Text);

                    if (SelectionStart >= 0 & SelectionStart <= Text.Length)
                    {
                        if (SelectionLength < Text.Length)
                        {
                            if (IsKeyBackSpace | IsKeyCut)
                            {
                                if (SelectionStart == 0)
                                {
                                    Text = Text.Substring(SelectionLength);
                                }
                                else
                                {
                                    Text = Text.Substring(0, SelectionStart - 1) + Text.Substring(SelectionStart + SelectionLength);
                                }
                            }
                            else if (IsKeyDelete)
                            {
                                if (SelectionStart + SelectionLength < Text.Length)
                                {
                                    Text = Text.Substring(0, SelectionStart) + Text.Substring(SelectionStart + SelectionLength + 1);
                                }
                                else if (SelectionStart + SelectionLength == Text.Length)
                                {
                                    Text = Text.Substring(0, SelectionStart);
                                }
                            }
                            else if (!IsKeyCopy)
                            {
                                Text = Text.Substring(0, SelectionStart) + NewChars + Text.Substring(SelectionStart + SelectionLength);
                            }
                        }
                        else if (SelectionLength == Text.Length)
                        {
                            if (IsKeyBackSpace | IsKeyDelete | IsKeyCut)
                            {
                                Text = String.Empty;
                            }
                            else if (!IsKeyCopy)
                            {
                                Text = NewChars;
                            }
                        }
                    }
                    return Text;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return String.Empty;
            }

            public virtual Boolean SetFocus()
            {
                try
                {
                    switch (ControlFocusMode)
                    {
                        case Enumerations.ControlFocusModes.None:
                            break;
                        case Enumerations.ControlFocusModes.Focus:
                            Focus();
                            break;
                        case Enumerations.ControlFocusModes.Select:
                            Select();
                            break;
                        default:
                            break;
                    }
                    return true;
                }
                catch
                {
                }
                return false;
            }

            public virtual void SetBackColorAsError(String StatusText = Constants.DefaultValues.StringNull, Boolean UseOnLeaveErrorMessageIfEmpty = true, Boolean IsFocus = false)
            {
                try
                {
                    if (StatusText == Constants.DefaultValues.StringNull && UseOnLeaveErrorMessageIfEmpty)
                    {
                        StatusText = OnLeaveErrorMessage;
                    }

                    if (OnLeaveErrorSetFocus || IsFocus)
                    {
                        IsSetFocusForError = true;
                        SetFocus();
                    }

                    if (StatusText != null)
                    {
                        StatusLabel.SetForeColorAsError(StatusText);
                    }
                    BackColor = BackColorOnError;
                    LastUsedBackColor = BackColor;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
            }

            public virtual Boolean SetFromDefaultValue(Boolean OnlyIfEmpty = false)
            {
                try
                {
                    Text = TrimmedText;
                    if ((OnlyIfEmpty && String.IsNullOrEmpty(Text)) || (!OnlyIfEmpty))
                    {
                        Text = DefaultValue;
                        BackColor = LastUsedBackColor;
                    }
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }
        }

        public class SpecialTextBox : TextBoxEx
        {
            private Regex _RegExp = null;
            private Regex _RegExpOnLeave = null;
            private String _RegExpCaption = String.Empty;

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the regular expression used to validate the text entered into the control.")]
            [Browsable(true)]
            public virtual String RegularExpression
            {
                get
                {
                    if (_RegExp != null)
                    {
                        return _RegExp.ToString();
                    }
                    return String.Empty;
                }
                set
                {
                    try
                    {
                        _RegExp = new Regex(value);
                    }
                    catch
                    {
                    }
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the regular expression used to validate the control's text while leaving it.")]
            [Browsable(true)]
            public virtual String RegularExpressionOnLeave
            {
                get
                {
                    if (_RegExpOnLeave != null)
                    {
                        return _RegExpOnLeave.ToString();
                    }
                    return String.Empty;
                }
                set
                {
                    try
                    {
                        _RegExpOnLeave = new Regex(value);
                    }
                    catch
                    {
                    }
                }
            }

            protected virtual Regex RegExp
            {
                get { return _RegExp; }
                set { _RegExp = value; }
            }

            protected virtual Regex RegExpOnLeave
            {
                get { return _RegExpOnLeave; }
                set { _RegExpOnLeave = value; }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the caption for the regular expression used to validate the text entered into the control.")]
            [Browsable(true)]
            public virtual String RegExpCaption
            {
                get { return _RegExpCaption; }
                set
                {
                    _RegExpCaption = value;
                    SetToolTip();
                }
            }

            protected override String ToolTipCaption
            {
                get
                {
                    try
                    {
                        StringBuilder objSB = new StringBuilder();
                        objSB.AppendLine(_RegExpCaption);
                        objSB.AppendLine(base.ToolTipCaption);
                        return objSB.ToString().TrimStart(Environment.NewLine.ToCharArray()).TrimEnd(Environment.NewLine.ToCharArray());
                    }
                    catch
                    {
                    }
                    return String.Empty;
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the Text in the control.")]
            [Browsable(true)]
            public override String Text
            {
                get { return base.Text; }
                set
                {
                    if (IsValidText(value))
                    {
                        base.Text = value;
                    }
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the default value for the control.")]
            [Browsable(true)]
            public override String DefaultValue
            {
                get { return base.DefaultValue; }
                set
                {
                    if (IsValidText(DefaultValue))
                    {
                        base.DefaultValue = GetTrimmedText(value);
                    }
                    SetToolTip();
                }
            }

            protected override void OnKeyPress(KeyPressEventArgs e)
            {
                try
                {
                    if (!(IsKeyBackSpace | IsKeyDelete | IsKeyCopy | IsKeyCut | IsKeyPaste))
                    {
                        if (!IsValidText(GetNewValue(e.KeyChar.ToString())))
                        {
                            e.Handled = true;
                        }
                    }
                    else if (IsKeyPaste)
                    {
                        String ClipText = Clipboard.GetText();
                        if (!IsValidText(GetNewValue(ClipText)))
                        {
                            e.Handled = true;
                        }
                    }
                    base.OnKeyPress(e);
                }
                catch
                {
                }
            }

            protected override void OnLeave(EventArgs e)
            {
                try
                {
                    if (IsValidTextOnLeave(IsSkipPadIfEmpty: true))
                    {
                        StatusLabel.SetForeColorAsValid();
                        base.OnLeave(e);
                    }
                }
                catch
                {
                }
            }

            public SpecialTextBox()
                : base()
            {
                RegularExpression = "^[A-Za-z]*$";//Special - "^[A-Za-z0-9\\s\\,\\-']*$"
                RegExpCaption = "Only Alphabets (a to z, A to Z) are allowed.";
                RegularExpressionOnLeave = RegularExpression;//Special - "^[A-Za-z0-9\\s\\,\\-']*$"
                SetOnLeaveErrorMessage();
                PadTextOption = Enumerations.PadTextOptions.None;
                PadChar = '\0';
                SetToolTip();
            }

            protected override Boolean SetOnLeaveErrorMessage()
            {
                try
                {
                    String Prefix = String.Empty;
                    if (LabelAssociated != null)
                    {
                        if (!String.IsNullOrEmpty(LabelAssociated.Text))
                        {
                            Prefix = String.Format("{0}: ", LabelAssociated.Text);
                        }
                    }
                    OnLeaveErrorMessage = String.Format("{0}Must be Alphabetic characters of length {1}.", Prefix, MinLength);
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean IsValidText(String Text = null)
            {
                try
                {
                    if (String.IsNullOrEmpty(Text))
                    {
                        Text = this.Text;
                    }

                    if (RegExp != null)
                    {
                        return RegExp.IsMatch(Text);
                    }

                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public override Boolean IsValidTextOnLeave(Boolean IsSkipValidationIfEmpty = true, Boolean IsSkipPadIfEmpty = false, Boolean IsFocus = false)
            {
                try
                {
                    Text = GetNormalizedText(IsSkipPadIfEmpty: IsSkipPadIfEmpty);
                    if (RegExpOnLeave != null)
                    {
                        if (RegExpOnLeave.IsMatch(Text) || (IsSkipValidationIfEmpty && String.IsNullOrEmpty(Text)))
                        {
                            IsSetFocusForError = false;
                            return true;
                        }
                    }
                    else
                    {
                        IsSetFocusForError = false;
                        return true;
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                SetBackColorAsError(IsFocus: IsFocus);
                return false;
            }

            public override String GetNormalizedText(String Text = null, Boolean IsSkipPadIfEmpty = false)
            {
                try
                {
                    if (Text == null)
                    {
                        Text = this.Text;
                    }

                    Text = GetTrimmedText(Text);
                    if (!IsValidText(Text) & !String.IsNullOrEmpty(Text))
                    {
                        SetBackColorAsError("Invalid Text");
                    }

                    if (!String.IsNullOrEmpty(DefaultValue) & String.IsNullOrEmpty(Text))
                    {
                        Text = DefaultValue;
                    }

                    return GetPaddedText(Text, IsSkipPadIfEmpty);
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return String.Empty;
            }

            public virtual Boolean SetToolTip()
            {
                try
                {
                    return SetToolTip(ToolTipCaption);
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }
        }

        public class AmountTextBox : TextBoxEx
        {
            private Int32 _MaxLenRealPart = 0;
            private Boolean _HandleMaxLenRealPart = false;
            private Boolean _HandleValueRange = false;
            private Boolean _CheckRangeOnNewChar = false;
            private Decimal _MinValue = 0;
            private Decimal _MaxValue = 0;
            private String _SuffixSymbol = "$";

            protected override String ToolTipCaption
            {
                get
                {
                    StringBuilder objSB = new StringBuilder();
                    objSB.AppendFormat("Allowed characters are: 0 to 9{0}{1}{2}", ((MaxLenDecimalPart > 0) ? ", Dot (.)" : String.Empty), (!String.IsNullOrEmpty(SuffixSymbol) ? (", " + SuffixSymbol) : String.Empty), ((MinValue < 0) ? ", Plus(+)/Minus(-)" : String.Empty));

                    if (MinLength > 0)
                    {
                        objSB.AppendLine("Minimum number of characters allowed to be typed or pasted: " + MinLength.ToString());
                    }
                    objSB.AppendLine("Maximum number of characters allowed to be typed or pasted: " + MaxLength.ToString());

                    if (HandleMaxLenRealPart)
                    {
                        objSB.AppendLine("Maximum number of characters allowed to be typed or pasted for real part is   : " + MaxLenRealPart.ToString());
                        objSB.AppendLine("Maximum number of characters allowed to be typed or pasted for Decimal part is: " + MaxLenDecimalPart.ToString());
                    }

                    if (HandleValueRange)
                    {
                        objSB.AppendLine("Minimum value allowed to be typed or pasted: " + MinValue.ToString() + SuffixSymbol);
                        objSB.AppendLine("Maximum value allowed to be typed or pasted: " + MaxValue.ToString() + SuffixSymbol);
                    }
                    return objSB.ToString().TrimStart(Environment.NewLine.ToCharArray()).TrimEnd(Environment.NewLine.ToCharArray());
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the maximum number of characters (including the sign bit) the user can type or paste into the control for the Real Part.")]
            [Browsable(true)]
            public virtual Int32 MaxLenRealPart
            {
                get { return _MaxLenRealPart; }
                set
                {
                    if (value >= 0 & value < MaxLength)
                    {
                        _MaxLenRealPart = value;
                    }
                    else if (value >= MaxLength)
                    {
                        if (String.IsNullOrEmpty(SuffixSymbol))
                        {
                            _MaxLenRealPart = MaxLength;
                        }
                        else
                        {
                            _MaxLenRealPart = MaxLength - 1;
                        }
                    }
                    else if (value < 0)
                    {
                        _MaxLenRealPart = 0;
                    }
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Enables or disables the maximum length validation for Real Part and Decimal Part.")]
            [Browsable(true)]
            public virtual Boolean HandleMaxLenRealPart
            {
                get { return _HandleMaxLenRealPart; }
                set
                {
                    _HandleMaxLenRealPart = value;

                    if (!(!String.IsNullOrEmpty(DefaultValue) && IsValidText(DefaultValue)))
                    {
                        DefaultValue = String.Empty;
                    }

                    SetToolTip();
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Enables or disables the validation to check whether the Text value is in the given range.")]
            [Browsable(true)]
            public virtual Boolean HandleValueRange
            {
                get { return _HandleValueRange; }
                set
                {
                    _HandleValueRange = value;

                    if (!value)
                    {
                        _CheckRangeOnNewChar = value;
                    }

                    SetToolTip();
                }
            }


            public virtual Boolean CheckRangeOnNewChar
            {
                get { return _CheckRangeOnNewChar; }
                set { _CheckRangeOnNewChar = value; }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the minimum value the user can type or paste into the control.")]
            [Browsable(true)]
            public virtual Decimal MinValue
            {
                get { return _MinValue; }
                set
                {
                    _MinValue = value;
                    if (value > _MaxValue)
                    {
                        _MaxValue = value;
                    }
                    SetToolTip();
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the maximum value the user can type or paste into the control.")]
            [Browsable(true)]
            public virtual Decimal MaxValue
            {
                get { return _MaxValue; }
                set
                {
                    _MaxValue = value;
                    if (value < _MinValue)
                    {
                        _MinValue = value;
                    }
                    SetToolTip();
                }
            }

            [Category(FormControlsEx.CategoryNameAppearance)]
            [DescriptionAttribute("Suffix Symbol to be appended to the Text. Only one character is allowed.")]
            [Browsable(true)]
            public virtual String SuffixSymbol
            {
                get { return _SuffixSymbol; }
                set
                {
                    value = value.Trim();
                    if (value.Length > 0)
                    {
                        value = value.Substring(0, 1);
                    }
                    String OldSuffixSymbol = _SuffixSymbol;
                    String OldTextDecimal = TextDecimal;
                    _SuffixSymbol = value;
                    Text = GetNormalizedText(OldTextDecimal);
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the Text in the control.")]
            [Browsable(true)]
            public override String Text
            {
                get { return base.Text; }
                set
                {
                    if (IsValidText(value))
                    {
                        base.Text = value;
                    }
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets the Decimal value from the control.")]
            [Browsable(true)]
            public virtual String TextDecimal
            {
                get
                {
                    return GetTextDecimal();
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the Decimal value from the control without any special characters.")]
            [Browsable(true)]
            public virtual String TextWithoutSpecialChars
            {
                get
                {
                    Regex regex = new Regex("[^\\d]");
                    return regex.Replace(TextDecimal, "");
                }
                set
                {
                    Regex regex = new Regex("[^\\d]");
                    value = regex.Replace(value, "");
                    if (value.Length >= MinLength)
                    {
                        String strDecimalValue = String.Empty;
                        Decimal DecimalValue = default(Decimal);

                        if (value.Length <= MaxLenRealPart)
                        {
                            strDecimalValue = value;
                        }
                        else
                        {
                            String RealPart = String.Empty;
                            String DecimalPart = String.Empty;
                            RealPart = value.Substring(0, MaxLenRealPart);
                            DecimalPart = value.Substring(MaxLenRealPart);
                            strDecimalValue = RealPart + "." + DecimalPart;
                        }

                        if (!String.IsNullOrEmpty(value))
                        {
                            Decimal.TryParse(strDecimalValue, out DecimalValue);
                            DecimalValue = Math.Round(DecimalValue, MaxLenDecimalPart);
                            strDecimalValue = DecimalValue.ToString();
                            Text = strDecimalValue;
                        }
                    }
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets the Padding Character for the control. This property is not-editable.")]
            [Browsable(true)]
            public override Char PadChar
            {
                get { return base.PadChar; }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the maximum number of characters the user can type or paste into the control.")]
            [Browsable(true)]
            public override Int32 MaxLength
            {
                get { return base.MaxLength; }
                set
                {
                    base.MaxLength = value;
                    if (MaxLenRealPart >= MaxLength)
                    {
                        if (String.IsNullOrEmpty(SuffixSymbol))
                        {
                            _MaxLenRealPart = value;
                        }
                        else
                        {
                            _MaxLenRealPart = value - 1;
                        }
                    }
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the maximum number of characters the user can type or paste into the control for the Decimal Part.")]
            [Browsable(true)]
            public virtual Int32 MaxLenDecimalPart
            {
                get
                {
                    Int32 value = MaxLength - MaxLenRealPart;
                    Int32 bound = 1;
                    //To allocate one position for Dot (".")
                    if (!String.IsNullOrEmpty(_SuffixSymbol))
                    {
                        bound = 2;
                        //To allocate another position for SuffixSymbol if present
                    }
                    if (value <= bound)
                    {
                        return 0;
                    }
                    else
                    {
                        return (value - bound);
                    }
                }
            }

            [Category(FormControlsEx.CategoryNameBehavior)]
            [DescriptionAttribute("Gets or sets the default value for the control.")]
            [Browsable(true)]
            public override String DefaultValue
            {
                get { return base.DefaultValue; }
                set
                {
                    if (String.IsNullOrEmpty(value))
                    {
                        base.DefaultValue = value;
                    }
                    else
                    {
                        if (IsValidText(value))
                        {
                            base.DefaultValue = value;
                        }
                    }
                    SetToolTip();
                }
            }

            public virtual Char FirstCharacter
            {
                get { return GetFirstCharacter(); }
            }

            public virtual Char LastCharacter
            {
                get { return GetLastCharacter(); }

            }

            public virtual Boolean HasSignBit
            {
                get { return GetHasSignBit(); }

            }

            public virtual String RealPart
            {
                get { return GetRealPart(); }

            }

            public virtual String DecimalPart
            {
                get { return GetDecimalPart(); }

            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                try
                {
                    if (e.KeyCode == Keys.Delete)
                    {
                        IsKeyDelete = true;
                        if (!IsValidText(GetNewValue(((Char)127).ToString())))
                        {
                            //e.Handled = true;
                        }
                    }
                    base.OnKeyDown(e);
                }
                catch
                {
                }
            }

            protected override void OnKeyPress(KeyPressEventArgs e)
            {
                try
                {
                    String NewValue = String.Empty;

                    if (IsKeyPaste)
                    {
                        String ClipText = Clipboard.GetText();
                        NewValue = GetNewValue(ClipText);
                        if (!(GetHasSignBit(NewValue) && NewValue.Length == 1))
                        {
                            if (!IsValidText(NewValue))
                            {
                                e.Handled = true;
                            }
                        }
                    }
                    else
                    {
                        NewValue = GetNewValue(e.KeyChar.ToString());
                    }

                    if (!(IsKeyBackSpace | IsKeyDelete | IsKeyCopy | IsKeyCut | IsKeyPaste))
                    {
                        switch (e.KeyChar)
                        {
                            case '0': // TODO: to Strings.ChrW(Strings.Asc("9"))
                            case '1':
                            case '2':
                            case '3':
                            case '4':
                            case '5':
                            case '6':
                            case '7':
                            case '8':
                            case '9':
                                if (!IsValidText(NewValue))
                                {
                                    e.Handled = true;
                                }
                                break;
                            case '.':
                                if (Text.Contains(e.KeyChar))
                                {
                                    e.Handled = true;
                                }
                                else if (!(IsValidRealPart(NewValue) & IsValidDecimalPart(NewValue) & !TextDecimal.Contains(".") & MaxLenDecimalPart > 0))
                                {
                                    e.Handled = true;
                                }
                                break;
                            case '+':
                            case '-':
                                if (SelectionStart != 0 | (HandleValueRange & MinValue >= 0))
                                {
                                    e.Handled = true;
                                }
                                break;
                            default:
                                if (e.KeyChar == GetFirstCharacter(SuffixSymbol))
                                {
                                    if (Text.Contains(e.KeyChar))
                                    {
                                        e.Handled = true;
                                    }

                                    if (SelectionStart < (Text.Length))
                                    {
                                        e.Handled = true;
                                    }

                                    if (!(IsValidRealPart(NewValue) & IsValidDecimalPart(NewValue) & !Text.Contains(SuffixSymbol)))
                                    {
                                        e.Handled = true;
                                    }
                                }
                                else
                                {
                                    e.Handled = true;
                                }
                                break;
                        }
                    }

                    base.OnKeyPress(e);
                }
                catch
                {
                }
            }

            protected override void OnLeave(EventArgs e)
            {
                try
                {
                    Text = GetNormalizedText(IsSkipPadIfEmpty: true);
                    if (new Conversion().ToDecimal(TextDecimal) == -0)
                    {
                        Text = GetNormalizedText("0");
                    }
                    base.OnLeave(e);
                }
                catch
                {
                }
            }

            public AmountTextBox()
                : base()
            {
                MaxLength = 7;
                MaxLenRealPart = 3;
                HandleMaxLenRealPart = true;
                PadTextOption = Enumerations.PadTextOptions.Both;
                base.PadChar = '0';
                MinValue = 0;
                MaxValue = 100;
                SetToolTip();
            }

            public virtual Char GetFirstCharacter(String Text = Constants.DefaultValues.String)
            {
                try
                {
                    if (String.IsNullOrEmpty(Text))
                    {
                        Text = TextDecimal;
                    }

                    if (Text.Length > 0)
                    {
                        return Text.ToCharArray().First();
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return '\0';
            }

            public virtual Char GetLastCharacter(String Text = Constants.DefaultValues.String)
            {
                try
                {
                    if (String.IsNullOrEmpty(Text))
                    {
                        Text = TextDecimal;
                    }

                    if (Text.Length > 0)
                    {
                        return Text.ToCharArray().Last();
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return '\0';
            }

            public virtual Boolean GetHasSignBit(String Text = Constants.DefaultValues.String)
            {
                try
                {
                    if (String.IsNullOrEmpty(Text))
                    {
                        Text = TextDecimal;
                    }

                    if (GetFirstCharacter(Text) == '+' | GetFirstCharacter(Text) == '-')
                    {
                        return true;
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual String GetRealPart(String Text = Constants.DefaultValues.String)
            {
                try
                {
                    Text = GetTextDecimal(Text);

                    if (!String.IsNullOrEmpty(Text))
                    {
                        Int32 DecimalPointIndex = Text.IndexOf(".");
                        if (DecimalPointIndex < 0)
                        {
                            return Text;
                        }
                        else if ((DecimalPointIndex == 0 | (DecimalPointIndex == 1 & GetHasSignBit(Text))))
                        {
                            return String.Empty;
                        }
                        else if (DecimalPointIndex > 0)
                        {
                            return Text.Substring(0, DecimalPointIndex);
                        }
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return String.Empty;
            }

            public virtual String GetDecimalPart(String Text = Constants.DefaultValues.String)
            {
                try
                {
                    Text = GetTextDecimal(Text);

                    if (!String.IsNullOrEmpty(Text))
                    {
                        Int32 DecimalPointIndex = Text.IndexOf(".");
                        if (DecimalPointIndex < 0 | DecimalPointIndex >= Text.Length)
                        {
                            return String.Empty;
                        }
                        else
                        {
                            return Text.Substring(DecimalPointIndex + 1);
                        }
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return String.Empty;
            }

            public virtual String GetTextDecimal(String Text = Constants.DefaultValues.String)
            {
                try
                {
                    if (String.IsNullOrEmpty(Text))
                    {
                        Text = this.Text;
                    }
                    Text = GetTrimmedText(Text);

                    if (!String.IsNullOrEmpty(Text))
                    {
                        if (!String.IsNullOrEmpty(SuffixSymbol))
                        {
                            return Text.Replace(SuffixSymbol, String.Empty);
                        }
                        else
                        {
                            return Text;
                        }
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return String.Empty;
            }

            public virtual Boolean IsValidRealPart(String Text = null)
            {
                try
                {
                    Text = GetTextDecimal(Text);
                    if (HandleMaxLenRealPart)
                    {
                        if (GetRealPart(Text).Length <= MaxLenRealPart)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (Text.Length >= MinLength & Text.Length <= MaxLength)
                        {
                            return true;
                        }
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean IsValidDecimalPart(String Text = null)
            {
                try
                {
                    Text = GetTextDecimal(Text);
                    if (HandleMaxLenRealPart)
                    {
                        if (GetDecimalPart(Text).Length <= MaxLenDecimalPart)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean IsValidNewValue(String Text = null)
            {
                try
                {
                    Text = GetTextDecimal(Text);
                    if (!String.IsNullOrEmpty(Text))
                    {
                        Decimal NewValueDecimal;
                        return Decimal.TryParse(Text, out NewValueDecimal);
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean IsValidText(String Text = null)
            {
                try
                {
                    if ((IsValidRealPart(Text) && IsValidDecimalPart(Text) && IsValidNewValue(Text)))
                    {
                        return IsValueInRange(Text);
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual Boolean IsValueInRange(String Text = null)
            {
                try
                {
                    if (HandleValueRange)
                    {
                        Text = GetTextDecimal(Text);

                        Decimal DecimalValue;
                        if (Decimal.TryParse(Text, out DecimalValue))
                        {
                            if (DecimalValue >= MinValue & DecimalValue <= MaxValue)
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public virtual String GetNewValue(String NewChars)
            {
                try
                {
                    if (SelectionStart == 0 & !HasSignBit & SelectionLength == 0)
                    {
                        return NewChars + TextDecimal;
                    }
                    else
                    {
                        return base.GetNewValue(NewChars, Text);
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return String.Empty;
            }

            public override String GetPaddedText(String Text, Boolean IsSkipPadIfEmpty = false, Enumerations.PadTextOptions? PadTextOption = null)
            {
                try
                {
                    if (PadTextOption == null)
                    {
                        PadTextOption = this.PadTextOption;
                    }
                    if (!String.IsNullOrEmpty(PadChar.ToString()) && !(IsSkipPadIfEmpty && String.IsNullOrEmpty(Text)))
                    {
                        if (String.IsNullOrEmpty(Text) && !IsSkipPadIfEmpty)
                        {
                            return GetPaddedText(Text);
                        }
                        String RealPart = GetRealPart(Text);
                        String DecimalPart = GetDecimalPart(Text);
                        Boolean HasSignBit = GetHasSignBit(Text);
                        Char FirstCharacter = GetFirstCharacter(Text);
                        Decimal RealPartValue = default(Decimal);
                        Decimal DecimalPartValue = default(Decimal);
                        if (Decimal.TryParse(RealPart, out RealPartValue))
                        {
                            if (Decimal.TryParse(DecimalPart, out DecimalPartValue))
                            {
                                if (HandleMaxLenRealPart)
                                {
                                    if (PadTextOption == Enumerations.PadTextOptions.Both | PadTextOption == Enumerations.PadTextOptions.Left)
                                    {
                                        if (HasSignBit & RealPart.Length > 0)
                                        {
                                            RealPart = FirstCharacter + RealPart.Substring(1).PadLeft(MaxLenRealPart - 1, PadChar);
                                        }
                                        else
                                        {
                                            RealPart = RealPart.PadLeft(MaxLenRealPart, PadChar);
                                        }
                                    }
                                    else if (PadTextOption != Enumerations.PadTextOptions.Right)
                                    {
                                        if (!String.IsNullOrEmpty(RealPart) && RealPartValue == 0)
                                        {
                                            RealPart = "0";
                                        }
                                    }

                                    if (PadTextOption == Enumerations.PadTextOptions.Both | PadTextOption == Enumerations.PadTextOptions.Right)
                                    {
                                        DecimalPart = DecimalPart.PadRight(MaxLenDecimalPart, PadChar);
                                    }
                                    else if (PadTextOption != Enumerations.PadTextOptions.Left)
                                    {
                                        if (!String.IsNullOrEmpty(DecimalPart) && DecimalPartValue == 0)
                                        {
                                            DecimalPart = String.Empty;
                                        }
                                    }

                                    if (String.IsNullOrEmpty(DecimalPart))
                                    {
                                        Text = RealPart + SuffixSymbol;
                                    }
                                    else
                                    {
                                        Text = RealPart + "." + DecimalPart + SuffixSymbol;
                                    }
                                }
                                else if (!String.IsNullOrEmpty(PadChar.ToString()))
                                {
                                    if (PadTextOption == Enumerations.PadTextOptions.Both | PadTextOption == Enumerations.PadTextOptions.Left)
                                    {
                                        if (HasSignBit & Text.Length > 0)
                                        {
                                            Text = FirstCharacter + Text.Substring(1).PadLeft(MaxLength - 1, PadChar);
                                        }
                                        else
                                        {
                                            Text = Text.PadLeft(MaxLength, PadChar);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    return Text;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return String.Empty;
            }

            public override Boolean PadText()
            {
                try
                {
                    Text = GetPaddedText(Text);
                    return true;
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return false;
            }

            public override String GetNormalizedText(String Text = Constants.DefaultValues.String, Boolean IsSkipPadIfEmpty = false)
            {
                try
                {
                    if (String.IsNullOrEmpty(Text))
                    {
                        Text = this.Text;
                    }

                    Text = GetTrimmedText(Text);
                    if (!String.IsNullOrEmpty(DefaultValue) & String.IsNullOrEmpty(GetTextDecimal(Text)))
                    {
                        Text = DefaultValue;
                    }

                    if (!String.IsNullOrEmpty(Text))
                    {
                        Decimal DecimalValue;
                        if (Decimal.TryParse(GetTextDecimal(Text), out DecimalValue))
                        {
                            Text = Math.Round(DecimalValue, MaxLenDecimalPart).ToString() + SuffixSymbol;

                            if (GetLastCharacter(Text) == '.' & Text.Length > 1)
                            {
                                Text = Text.Substring(0, Text.Length - 2) + SuffixSymbol;
                            }

                            if (!IsValidText(Text))
                            {
                                SetBackColorAsError();
                            }
                            else
                            {
                                Text = GetPaddedText(Text, IsSkipPadIfEmpty);
                                StatusLabel.SetForeColorAsValid();
                            }
                            return Text;
                        }
                    }
                }
                catch (Exception objException)
                {
                    EventLogger.WriteException(objException);
                }
                return String.Empty;
            }
        }

        public class PercentTextBox : AmountTextBox
        {
            public PercentTextBox()
                : base()
            {
                SuffixSymbol = "%";
            }
        }

        public class MultiLineTextBox : SpecialTextBox
        {
            protected override void OnGotFocus(EventArgs e)
            {
                try
                {
                    base.OnGotFocus(e);
                    SelectionStart = 0;
                    SelectionLength = 0;
                }
                catch
                {
                }
            }

            protected override void OnMouseHover(EventArgs e)
            {
                try
                {
                    ScrollBars = ScrollBars.Both;
                    base.OnMouseHover(e);
                }
                catch
                {
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                try
                {
                    ScrollBars = ScrollBars.Both;
                    base.OnMouseMove(e);
                }
                catch
                {
                }
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                try
                {
                    ScrollBars = ScrollBars.None;
                    base.OnMouseLeave(e);
                }
                catch
                {
                }
            }

            public MultiLineTextBox()
                : base()
            {
                Multiline = true;
                Size = Constants.Sizes.ControlLarge;
                ScrollBars = ScrollBars.Both;
                AcceptsReturn = false;
                AcceptsTab = false;
                WordWrap = true;
                BorderStyle = BorderStyle.Fixed3D;
                this.ReadOnly = true;
            }
        }
    }
}
