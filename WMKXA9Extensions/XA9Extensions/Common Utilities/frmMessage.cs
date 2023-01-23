using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CommonUtilities
{
    public partial class frmMessage : Form
    {
        private class PanelSize
        {
            public static Size OneButton = new Size(81, 29);
            public static Size TwoButtons = new Size(81, 58);
            public static Size ThreeButtons = new Size(81, 87);
        }

        private class ButtonPosition
        {
            public static Point Button1 = new Point(3, 3);
            public static Point Button2 = new Point(3, 32);
            public static Point Button3 = new Point(3, 61);
        }

        protected MessageBoxButtons _MessageBoxButton = MessageBoxButtons.OK;
        protected DialogResult _DialogResult = DialogResult.OK;

        public frmMessage()
        {
            InitializeComponent();
            Text = MessageBoxEx.ApplicationName;
            rtbMessage.Font = Constants.Fonts.CourierNew;
            rtbMessage.SelectionAlignment = HorizontalAlignment.Center;
            ResizeControls();
        }

        private void frmMessage_SizeChanged(object sender, EventArgs e)
        {
            ResizeControls();
        }

        private Boolean ResizeControls()
        {
            try
            {
                Int32 Height = 0;
                Int32 Width = 0;
                Int32 X = 0;
                Int32 Y = 0;

                Height = pnlMain.Height - 24;
                rtbMessage.Height = Height;

                Width = pnlMain.Width - (rtbMessage.Location.X + pnlButtons.Width + 18);
                rtbMessage.Width = Width;

                switch (_MessageBoxButton)
                {
                    case MessageBoxButtons.AbortRetryIgnore:
                    case MessageBoxButtons.YesNoCancel:
                        Height = PanelSize.ThreeButtons.Height;
                        break;
                    case MessageBoxButtons.OKCancel:
                    case MessageBoxButtons.RetryCancel:
                    case MessageBoxButtons.YesNo:
                        Height = PanelSize.TwoButtons.Height;
                        break;
                    case MessageBoxButtons.OK:
                        Height = PanelSize.OneButton.Height;
                        break;
                    default:
                        break;
                }
                pnlButtons.Height = Height;

                X = rtbMessage.Location.X + rtbMessage.Width + 6;
                Y = (pnlMain.Height - pnlButtons.Height) / 2;
                pnlButtons.Location = new Point(X, Y);

                btnYes.Visible = false;
                btnNo.Visible = false;
                btnCancel.Visible = false;
                btnAbort.Visible = false;
                btnRetry.Visible = false;
                btnIgnore.Visible = false;
                btnOK.Visible = false;

                switch (_MessageBoxButton)
                {
                    case MessageBoxButtons.AbortRetryIgnore:
                        btnAbort.Visible = true;
                        btnAbort.Location = ButtonPosition.Button1;

                        btnRetry.Visible = true;
                        btnRetry.Location = ButtonPosition.Button2;

                        btnIgnore.Visible = true;
                        btnIgnore.Location = ButtonPosition.Button3;

                        AcceptButton = btnAbort;
                        btnAbort.Focus();
                        break;
                    case MessageBoxButtons.OK:
                        btnOK.Visible = true;
                        btnOK.Location = ButtonPosition.Button1;

                        AcceptButton = btnOK;
                        btnOK.Focus();
                        break;
                    case MessageBoxButtons.OKCancel:
                        btnOK.Visible = true;
                        btnOK.Location = ButtonPosition.Button1;

                        btnCancel.Visible = true;
                        btnCancel.Location = ButtonPosition.Button2;

                        AcceptButton = btnOK;
                        btnOK.Focus();
                        break;
                    case MessageBoxButtons.RetryCancel:
                        btnRetry.Visible = true;
                        btnRetry.Location = ButtonPosition.Button1;

                        btnCancel.Visible = true;
                        btnCancel.Location = ButtonPosition.Button2;

                        AcceptButton = btnRetry;
                        btnRetry.Focus();
                        break;
                    case MessageBoxButtons.YesNo:
                        btnYes.Visible = true;
                        btnYes.Location = ButtonPosition.Button1;

                        btnNo.Visible = true;
                        btnNo.Location = ButtonPosition.Button2;

                        AcceptButton = btnYes;
                        btnYes.Focus();
                        break;
                    case MessageBoxButtons.YesNoCancel:
                        btnYes.Visible = true;
                        btnYes.Location = ButtonPosition.Button1;

                        btnNo.Visible = true;
                        btnNo.Location = ButtonPosition.Button2;

                        btnCancel.Visible = true;
                        btnCancel.Location = ButtonPosition.Button3;

                        AcceptButton = btnYes;
                        btnYes.Focus();
                        break;
                    default:
                        break;
                }
                return true;
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return false;
        }

        public Boolean SetMessage(String Message = Constants.DefaultValues.String, MessageBoxButtons MessageBoxButton = MessageBoxButtons.OK, Icon SystemIcon = null, Boolean IsWordWrap = true, RichTextBoxScrollBars ScrollBar = RichTextBoxScrollBars.None, HorizontalAlignment Alignment = HorizontalAlignment.Center)
        {
            try
            {
                rtbMessage.Text = Message;
                rtbMessage.SelectionStart = 0;
                if (SystemIcon == null)
                {
                    SystemIcon = SystemIcons.Information;
                }
                pbMessageIcon.Image = Bitmap.FromHicon(SystemIcon.Handle);
                _MessageBoxButton = MessageBoxButton;
                SetScrollBars(ScrollBar);
                SetTextAlignment(Alignment);
                SetWordWrap(IsWordWrap);
                ResizeControls();
                return true;
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return false;
        }

        public static DialogResult Show(String Message = Constants.DefaultValues.String, MessageBoxButtons MessageBoxButton = MessageBoxButtons.OK, Icon SystemIcon = null, Boolean IsWordWrap = true, RichTextBoxScrollBars ScrollBar = RichTextBoxScrollBars.None, HorizontalAlignment Alignment = HorizontalAlignment.Center, Boolean ShowMessageOnlyIfNotEmpty = true)
        {
            try
            {
                if (ShowMessageOnlyIfNotEmpty && String.IsNullOrEmpty(Message))
                {
                    return DialogResult.OK;
                }
                frmMessage frm = new frmMessage();
                frm.SetMessage(Message, MessageBoxButton, SystemIcon, IsWordWrap, ScrollBar, Alignment);
                frm.ShowDialog();
                return frm._DialogResult;
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return DialogResult.None;
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            _DialogResult = DialogResult.Yes;
            Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            _DialogResult = DialogResult.No;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btnAbort_Click(object sender, EventArgs e)
        {
            _DialogResult = DialogResult.Abort;
            Close();
        }

        private void btnRetry_Click(object sender, EventArgs e)
        {
            _DialogResult = DialogResult.Retry;
            Close();
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            _DialogResult = DialogResult.Ignore;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            _DialogResult = DialogResult.OK;
            Close();
        }

        private void tsmiWordWrap_Click(object sender, EventArgs e)
        {
            SetWordWrap(!tsmiWordWrap.Checked);
        }

        private Boolean SetWordWrap(Boolean IsWordWrap = true)
        {
            try
            {
                tsmiWordWrap.Checked = IsWordWrap;
                rtbMessage.WordWrap = IsWordWrap;
                tsmiSB_Horizontal.Enabled = !IsWordWrap;
                return true;
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return false;
        }

        private void tsmiSB_None_Click(object sender, EventArgs e)
        {
            SetScrollBars(RichTextBoxScrollBars.None);
        }

        private void tsmiSB_Horizontal_Click(object sender, EventArgs e)
        {
            SetScrollBars(RichTextBoxScrollBars.Horizontal);
        }

        private void tsmiSB_Vertical_Click(object sender, EventArgs e)
        {
            SetScrollBars(RichTextBoxScrollBars.Vertical);
        }

        private void tsmiSB_Both_Click(object sender, EventArgs e)
        {
            SetScrollBars(RichTextBoxScrollBars.Both);
        }

        private Boolean SetScrollBars(RichTextBoxScrollBars ScrollBar = RichTextBoxScrollBars.None)
        {
            try
            {
                tsmiSB_None.Checked = false;
                tsmiSB_Horizontal.Checked = false;
                tsmiSB_Vertical.Checked = false;
                tsmiSB_Both.Checked = false;
                rtbMessage.BorderStyle = BorderStyle.FixedSingle;
                switch (ScrollBar)
                {
                    case RichTextBoxScrollBars.Both:
                        tsmiSB_Both.Checked = true;
                        break;
                    case RichTextBoxScrollBars.Horizontal:
                        tsmiSB_Horizontal.Checked = true;
                        break;
                    case RichTextBoxScrollBars.None:
                        tsmiSB_None.Checked = true;
                        rtbMessage.BorderStyle = BorderStyle.None;
                        break;
                    case RichTextBoxScrollBars.Vertical:
                        tsmiSB_Vertical.Checked = true;
                        break;
                    default:
                        break;
                }
                rtbMessage.ScrollBars = ScrollBar;
                return true;
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return false;
        }

        private void tsmiTA_Left_Click(object sender, EventArgs e)
        {
            SetTextAlignment(HorizontalAlignment.Left);
        }

        private void tsmiTA_Right_Click(object sender, EventArgs e)
        {
            SetTextAlignment(HorizontalAlignment.Right);
        }

        private void tsmiTA_Centered_Click(object sender, EventArgs e)
        {
            SetTextAlignment(HorizontalAlignment.Center);
        }

        private Boolean SetTextAlignment(HorizontalAlignment Alignment = HorizontalAlignment.Center)
        {
            try
            {
                tsmiTA_Left.Checked = false;
                tsmiTA_Right.Checked = false;
                tsmiTA_Centered.Checked = false;
                switch (Alignment)
                {
                    case HorizontalAlignment.Center:
                        tsmiTA_Centered.Checked = true;
                        break;
                    case HorizontalAlignment.Left:
                        tsmiTA_Left.Checked = true;
                        break;
                    case HorizontalAlignment.Right:
                        tsmiTA_Right.Checked = true;
                        break;
                    default:
                        break;
                }
                rtbMessage.SelectAll();
                rtbMessage.SelectionAlignment = Alignment;
                rtbMessage.SelectionStart = 0;
                return true;
            }
            catch (Exception objException)
            {
                EventLogger.WriteException(objException);
            }
            return false;
        }
    }
}
