namespace CommonUtilities
{
    partial class frmMessage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMessage));
            this.pnlMain = new System.Windows.Forms.Panel();
            this.rtbMessage = new CommonUtilities.UserControls.RichTextBoxEx();
            this.pbMessageIcon = new System.Windows.Forms.PictureBox();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnYes = new CommonUtilities.UserControls.ButtonEx();
            this.btnOK = new CommonUtilities.UserControls.ButtonEx();
            this.btnIgnore = new CommonUtilities.UserControls.ButtonEx();
            this.btnRetry = new CommonUtilities.UserControls.ButtonEx();
            this.btnAbort = new CommonUtilities.UserControls.ButtonEx();
            this.btnCancel = new CommonUtilities.UserControls.ButtonEx();
            this.btnNo = new CommonUtilities.UserControls.ButtonEx();
            this.msTop = new System.Windows.Forms.MenuStrip();
            this.tsmiView = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiWordWrap = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiScrollBars = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSB_None = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSB_Horizontal = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSB_Vertical = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSB_Both = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTextAlignment = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTA_Left = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTA_Right = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiTA_Centered = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbMessageIcon)).BeginInit();
            this.pnlButtons.SuspendLayout();
            this.msTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.rtbMessage);
            this.pnlMain.Controls.Add(this.pbMessageIcon);
            this.pnlMain.Controls.Add(this.pnlButtons);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 24);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(389, 115);
            this.pnlMain.TabIndex = 0;
            // 
            // rtbMessage
            // 
            this.rtbMessage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.rtbMessage.BackColorOnEnter = System.Drawing.Color.WhiteSmoke;
            this.rtbMessage.BackColorOnError = System.Drawing.Color.Orange;
            this.rtbMessage.BackColorOnLeave = System.Drawing.Color.WhiteSmoke;
            this.rtbMessage.BackColorOnMouseHover = System.Drawing.Color.LightSteelBlue;
            this.rtbMessage.BackColorOnMouseMove = System.Drawing.Color.LightSteelBlue;
            this.rtbMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbMessage.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbMessage.Location = new System.Drawing.Point(54, 12);
            this.rtbMessage.Name = "rtbMessage";
            this.rtbMessage.Size = new System.Drawing.Size(225, 90);
            this.rtbMessage.TabIndex = 1;
            this.rtbMessage.Text = "";
            // 
            // pbMessageIcon
            // 
            this.pbMessageIcon.Location = new System.Drawing.Point(12, 12);
            this.pbMessageIcon.Name = "pbMessageIcon";
            this.pbMessageIcon.Size = new System.Drawing.Size(36, 36);
            this.pbMessageIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbMessageIcon.TabIndex = 2;
            this.pbMessageIcon.TabStop = false;
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnYes);
            this.pnlButtons.Controls.Add(this.btnOK);
            this.pnlButtons.Controls.Add(this.btnIgnore);
            this.pnlButtons.Controls.Add(this.btnRetry);
            this.pnlButtons.Controls.Add(this.btnAbort);
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Controls.Add(this.btnNo);
            this.pnlButtons.Location = new System.Drawing.Point(285, 12);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(81, 87);
            this.pnlButtons.TabIndex = 0;
            // 
            // btnYes
            // 
            this.btnYes.BackColorOnEnter = System.Drawing.Color.SkyBlue;
            this.btnYes.BackColorOnError = System.Drawing.Color.OrangeRed;
            this.btnYes.BackColorOnLeave = System.Drawing.SystemColors.Control;
            this.btnYes.BackColorOnMouseHover = System.Drawing.Color.LightSteelBlue;
            this.btnYes.BackColorOnMouseMove = System.Drawing.Color.LightSteelBlue;
            this.btnYes.Location = new System.Drawing.Point(3, 3);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(75, 23);
            this.btnYes.TabIndex = 0;
            this.btnYes.Text = "&Yes";
            this.btnYes.ToolTipText = "";
            this.btnYes.UseVisualStyleBackColor = true;
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // btnOK
            // 
            this.btnOK.BackColorOnEnter = System.Drawing.Color.SkyBlue;
            this.btnOK.BackColorOnError = System.Drawing.Color.OrangeRed;
            this.btnOK.BackColorOnLeave = System.Drawing.SystemColors.Control;
            this.btnOK.BackColorOnMouseHover = System.Drawing.Color.LightSteelBlue;
            this.btnOK.BackColorOnMouseMove = System.Drawing.Color.LightSteelBlue;
            this.btnOK.Location = new System.Drawing.Point(4, 178);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "&OK";
            this.btnOK.ToolTipText = "";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnIgnore
            // 
            this.btnIgnore.BackColorOnEnter = System.Drawing.Color.SkyBlue;
            this.btnIgnore.BackColorOnError = System.Drawing.Color.OrangeRed;
            this.btnIgnore.BackColorOnLeave = System.Drawing.SystemColors.Control;
            this.btnIgnore.BackColorOnMouseHover = System.Drawing.Color.LightSteelBlue;
            this.btnIgnore.BackColorOnMouseMove = System.Drawing.Color.LightSteelBlue;
            this.btnIgnore.Location = new System.Drawing.Point(3, 148);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(75, 23);
            this.btnIgnore.TabIndex = 5;
            this.btnIgnore.Text = "&Ignore";
            this.btnIgnore.ToolTipText = "";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // btnRetry
            // 
            this.btnRetry.BackColorOnEnter = System.Drawing.Color.SkyBlue;
            this.btnRetry.BackColorOnError = System.Drawing.Color.OrangeRed;
            this.btnRetry.BackColorOnLeave = System.Drawing.SystemColors.Control;
            this.btnRetry.BackColorOnMouseHover = System.Drawing.Color.LightSteelBlue;
            this.btnRetry.BackColorOnMouseMove = System.Drawing.Color.LightSteelBlue;
            this.btnRetry.Location = new System.Drawing.Point(3, 119);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new System.Drawing.Size(75, 23);
            this.btnRetry.TabIndex = 4;
            this.btnRetry.Text = "&Retry";
            this.btnRetry.ToolTipText = "";
            this.btnRetry.UseVisualStyleBackColor = true;
            this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.BackColorOnEnter = System.Drawing.Color.SkyBlue;
            this.btnAbort.BackColorOnError = System.Drawing.Color.OrangeRed;
            this.btnAbort.BackColorOnLeave = System.Drawing.SystemColors.Control;
            this.btnAbort.BackColorOnMouseHover = System.Drawing.Color.LightSteelBlue;
            this.btnAbort.BackColorOnMouseMove = System.Drawing.Color.LightSteelBlue;
            this.btnAbort.Location = new System.Drawing.Point(3, 90);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(75, 23);
            this.btnAbort.TabIndex = 3;
            this.btnAbort.Text = "&Abort";
            this.btnAbort.ToolTipText = "";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColorOnEnter = System.Drawing.Color.SkyBlue;
            this.btnCancel.BackColorOnError = System.Drawing.Color.OrangeRed;
            this.btnCancel.BackColorOnLeave = System.Drawing.SystemColors.Control;
            this.btnCancel.BackColorOnMouseHover = System.Drawing.Color.LightSteelBlue;
            this.btnCancel.BackColorOnMouseMove = System.Drawing.Color.LightSteelBlue;
            this.btnCancel.Location = new System.Drawing.Point(3, 61);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.ToolTipText = "";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnNo
            // 
            this.btnNo.BackColorOnEnter = System.Drawing.Color.SkyBlue;
            this.btnNo.BackColorOnError = System.Drawing.Color.OrangeRed;
            this.btnNo.BackColorOnLeave = System.Drawing.SystemColors.Control;
            this.btnNo.BackColorOnMouseHover = System.Drawing.Color.LightSteelBlue;
            this.btnNo.BackColorOnMouseMove = System.Drawing.Color.LightSteelBlue;
            this.btnNo.Location = new System.Drawing.Point(3, 32);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(75, 23);
            this.btnNo.TabIndex = 1;
            this.btnNo.Text = "&No";
            this.btnNo.ToolTipText = "";
            this.btnNo.UseVisualStyleBackColor = true;
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // msTop
            // 
            this.msTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiView});
            this.msTop.Location = new System.Drawing.Point(0, 0);
            this.msTop.Name = "msTop";
            this.msTop.Size = new System.Drawing.Size(389, 24);
            this.msTop.TabIndex = 1;
            this.msTop.Text = "menuStrip1";
            // 
            // tsmiView
            // 
            this.tsmiView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiWordWrap,
            this.tsmiScrollBars,
            this.tsmiTextAlignment});
            this.tsmiView.Name = "tsmiView";
            this.tsmiView.Size = new System.Drawing.Size(44, 20);
            this.tsmiView.Text = "&View";
            // 
            // tsmiWordWrap
            // 
            this.tsmiWordWrap.Checked = true;
            this.tsmiWordWrap.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiWordWrap.Name = "tsmiWordWrap";
            this.tsmiWordWrap.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.W)));
            this.tsmiWordWrap.Size = new System.Drawing.Size(202, 22);
            this.tsmiWordWrap.Text = "Word W&rap";
            this.tsmiWordWrap.Click += new System.EventHandler(this.tsmiWordWrap_Click);
            // 
            // tsmiScrollBars
            // 
            this.tsmiScrollBars.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiSB_None,
            this.tsmiSB_Horizontal,
            this.tsmiSB_Vertical,
            this.tsmiSB_Both});
            this.tsmiScrollBars.Name = "tsmiScrollBars";
            this.tsmiScrollBars.Size = new System.Drawing.Size(202, 22);
            this.tsmiScrollBars.Text = "&Scroll Bars";
            // 
            // tsmiSB_None
            // 
            this.tsmiSB_None.Checked = true;
            this.tsmiSB_None.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiSB_None.Name = "tsmiSB_None";
            this.tsmiSB_None.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.N)));
            this.tsmiSB_None.Size = new System.Drawing.Size(195, 22);
            this.tsmiSB_None.Text = "&None";
            this.tsmiSB_None.Click += new System.EventHandler(this.tsmiSB_None_Click);
            // 
            // tsmiSB_Horizontal
            // 
            this.tsmiSB_Horizontal.Name = "tsmiSB_Horizontal";
            this.tsmiSB_Horizontal.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.H)));
            this.tsmiSB_Horizontal.Size = new System.Drawing.Size(195, 22);
            this.tsmiSB_Horizontal.Text = "&Horizontal";
            this.tsmiSB_Horizontal.Click += new System.EventHandler(this.tsmiSB_Horizontal_Click);
            // 
            // tsmiSB_Vertical
            // 
            this.tsmiSB_Vertical.Name = "tsmiSB_Vertical";
            this.tsmiSB_Vertical.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.V)));
            this.tsmiSB_Vertical.Size = new System.Drawing.Size(195, 22);
            this.tsmiSB_Vertical.Text = "&Vertical";
            this.tsmiSB_Vertical.Click += new System.EventHandler(this.tsmiSB_Vertical_Click);
            // 
            // tsmiSB_Both
            // 
            this.tsmiSB_Both.Name = "tsmiSB_Both";
            this.tsmiSB_Both.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt) 
            | System.Windows.Forms.Keys.B)));
            this.tsmiSB_Both.Size = new System.Drawing.Size(195, 22);
            this.tsmiSB_Both.Text = "&Both";
            this.tsmiSB_Both.Click += new System.EventHandler(this.tsmiSB_Both_Click);
            // 
            // tsmiTextAlignment
            // 
            this.tsmiTextAlignment.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiTA_Left,
            this.tsmiTA_Right,
            this.tsmiTA_Centered});
            this.tsmiTextAlignment.Name = "tsmiTextAlignment";
            this.tsmiTextAlignment.Size = new System.Drawing.Size(202, 22);
            this.tsmiTextAlignment.Text = "&Text Alignment";
            // 
            // tsmiTA_Left
            // 
            this.tsmiTA_Left.Name = "tsmiTA_Left";
            this.tsmiTA_Left.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.L)));
            this.tsmiTA_Left.Size = new System.Drawing.Size(196, 22);
            this.tsmiTA_Left.Text = "&Left";
            this.tsmiTA_Left.Click += new System.EventHandler(this.tsmiTA_Left_Click);
            // 
            // tsmiTA_Right
            // 
            this.tsmiTA_Right.Name = "tsmiTA_Right";
            this.tsmiTA_Right.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.R)));
            this.tsmiTA_Right.Size = new System.Drawing.Size(196, 22);
            this.tsmiTA_Right.Text = "&Right";
            this.tsmiTA_Right.Click += new System.EventHandler(this.tsmiTA_Right_Click);
            // 
            // tsmiTA_Centered
            // 
            this.tsmiTA_Centered.Checked = true;
            this.tsmiTA_Centered.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsmiTA_Centered.Name = "tsmiTA_Centered";
            this.tsmiTA_Centered.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.C)));
            this.tsmiTA_Centered.Size = new System.Drawing.Size(196, 22);
            this.tsmiTA_Centered.Text = "&Centered";
            this.tsmiTA_Centered.Click += new System.EventHandler(this.tsmiTA_Centered_Click);
            // 
            // frmMessage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(389, 139);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.msTop);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMessage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Message";
            this.SizeChanged += new System.EventHandler(this.frmMessage_SizeChanged);
            this.pnlMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbMessageIcon)).EndInit();
            this.pnlButtons.ResumeLayout(false);
            this.msTop.ResumeLayout(false);
            this.msTop.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlButtons;
        private UserControls.ButtonEx btnCancel;
        private UserControls.ButtonEx btnNo;
        private UserControls.ButtonEx btnYes;
        private UserControls.ButtonEx btnOK;
        private UserControls.ButtonEx btnIgnore;
        private UserControls.ButtonEx btnRetry;
        private UserControls.ButtonEx btnAbort;
        private System.Windows.Forms.PictureBox pbMessageIcon;
        private System.Windows.Forms.MenuStrip msTop;
        private System.Windows.Forms.ToolStripMenuItem tsmiView;
        private System.Windows.Forms.ToolStripMenuItem tsmiWordWrap;
        private System.Windows.Forms.ToolStripMenuItem tsmiScrollBars;
        private System.Windows.Forms.ToolStripMenuItem tsmiSB_None;
        private System.Windows.Forms.ToolStripMenuItem tsmiSB_Horizontal;
        private System.Windows.Forms.ToolStripMenuItem tsmiSB_Vertical;
        private System.Windows.Forms.ToolStripMenuItem tsmiSB_Both;
        private UserControls.RichTextBoxEx rtbMessage;
        private System.Windows.Forms.ToolStripMenuItem tsmiTextAlignment;
        private System.Windows.Forms.ToolStripMenuItem tsmiTA_Left;
        private System.Windows.Forms.ToolStripMenuItem tsmiTA_Right;
        private System.Windows.Forms.ToolStripMenuItem tsmiTA_Centered;

    }
}
