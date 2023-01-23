using System;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.XPath;
using FacetsControlLibrary;
using XA9Extensions.BusinessLayer;
using XA9Extensions.Utilities;

namespace XA9Extensions
{
    public class frmStopLoss : FacetsBaseControl
    {
        private Label lblClaimIDValue;
        private Label lblClaimID;
        private Label lblStopLoss;
        private Label lblCreatedOnValue;
        private TextBox txtStopLoss;
        private Button btnSave;
        private bool FormSetUpComplete = false;
        string strClaimID = string.Empty;
        string strClaimStatus = string.Empty;
        
        private LinkLabel lnkRefresh;
        string strAccessLevel = string.Empty;
        string strPzapAppID = string.Empty;

        /* defect */
        string strPzpzID = string.Empty;


        /// <summary>
        /// Set up the state of the form 
        /// </summary>
        private void FirstTimeFormSetUp()
        {
            if (string.IsNullOrEmpty(ContextData.ContextInstance.UserId))
            {
                Initiate();
            }

            this.SetFacetsControlColors();
            this.SetFacetsFormFont();
            this.SetFacetsControlFont(this);
            this.FormSetUpComplete = true;


        }

        private void GetPzapAppID()
        {
            XDocument xdocContextData = null;
            string strContextData = string.Empty;
            try
            {
                GetData("$CTXT_SYIN", ref strContextData);
                //Logger.LoggerInstance.ReportMessage("strContextData is : ", strContextData);
                if (!string.IsNullOrEmpty(strContextData))
                {
                    xdocContextData = XDocument.Parse(strContextData);
                    strPzapAppID = xdocContextData.XPathSelectElement(string.Format(XA9Constants.XML_COLLUMN_XPATH, "PZAP_APP_ID")).Value;
                    strPzpzID = xdocContextData.XPathSelectElement(string.Format(XA9Constants.XML_COLLUMN_XPATH, "PZPZ_ID")).Value; // DEFECT
                }
            }
            catch (Exception ex)
            {
                lblClaimIDValue.Text = string.Empty;
            }
        }
        /// <summary>
        /// Get Claim ID of currently opened claim in Facets online
        /// </summary>
        /// <returns></returns>
        //private string GetClaimId()
        private void GetClaimId()
        {
            XDocument xdocClaimData = null;
            string strClaimData = string.Empty;
            

            try
            {
                GetData("CLCL", ref strClaimData);
                if (!strClaimData.Equals(string.Empty))
                {
                    xdocClaimData = XDocument.Parse(strClaimData);
                    strClaimID = xdocClaimData.XPathSelectElement(string.Format(XA9Constants.XML_COLLUMN_XPATH, "CLCL_ID")).Value;
                    strClaimStatus = xdocClaimData.XPathSelectElement(string.Format(XA9Constants.XML_COLLUMN_XPATH, "CLCL_CUR_STS")).Value;
                }
                else
                {
                    strClaimID = string.Empty;
                }

                lblClaimIDValue.Text = strClaimID;
            }
            catch (Exception ex)
            {
                lblClaimIDValue.Text = string.Empty;
            }
            
            //return strClaimID;
        }

        // <summary>
        /// Set up Facets context object
        /// </summary>
        
        private void Initiate()
        {
            // Sets the Facets connection ifnormation for establishing the Data connection
            string strSignOnData = string.Empty;
            GetData(XA9Constants.GETDATASGN0, ref strSignOnData);
            //Logger.LoggerInstance.ReportMessage("strSignOnData", strSignOnData);
            ContextData.ContextInstance.Initialize(strSignOnData);

        }

        public frmStopLoss()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.lblClaimID = new System.Windows.Forms.Label();
            this.lblClaimIDValue = new System.Windows.Forms.Label();
            this.lblStopLoss = new System.Windows.Forms.Label();
            this.lblCreatedOnValue = new System.Windows.Forms.Label();
            this.txtStopLoss = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.lnkRefresh = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // lblClaimID
            // 
            this.lblClaimID.AutoSize = true;
            this.lblClaimID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClaimID.Location = new System.Drawing.Point(17, 18);
            this.lblClaimID.Name = "lblClaimID";
            this.lblClaimID.Size = new System.Drawing.Size(54, 13);
            this.lblClaimID.TabIndex = 2;
            this.lblClaimID.Text = "Claim ID";
            // 
            // lblClaimIDValue
            // 
            this.lblClaimIDValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClaimIDValue.Location = new System.Drawing.Point(75, 18);
            this.lblClaimIDValue.Name = "lblClaimIDValue";
            this.lblClaimIDValue.Size = new System.Drawing.Size(107, 13);
            this.lblClaimIDValue.TabIndex = 3;
            // 
            // lblStopLoss
            // 
            this.lblStopLoss.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStopLoss.Location = new System.Drawing.Point(199, 18);
            this.lblStopLoss.Name = "lblStopLoss";
            this.lblStopLoss.Size = new System.Drawing.Size(72, 13);
            this.lblStopLoss.TabIndex = 4;
            this.lblStopLoss.Text = "Stop Loss";
            // 
            // lblCreatedOnValue
            // 
            this.lblCreatedOnValue.AutoSize = true;
            this.lblCreatedOnValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCreatedOnValue.Location = new System.Drawing.Point(253, 18);
            this.lblCreatedOnValue.Name = "lblCreatedOnValue";
            this.lblCreatedOnValue.Size = new System.Drawing.Size(0, 13);
            this.lblCreatedOnValue.TabIndex = 5;
            // 
            // txtStopLoss
            // 
            this.txtStopLoss.Location = new System.Drawing.Point(278, 13);
            this.txtStopLoss.MaxLength = 2;
            this.txtStopLoss.Name = "txtStopLoss";
            this.txtStopLoss.Size = new System.Drawing.Size(46, 20);
            this.txtStopLoss.TabIndex = 6;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(342, 13);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(57, 23);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lnkRefresh
            // 
            this.lnkRefresh.AutoSize = true;
            this.lnkRefresh.Location = new System.Drawing.Point(412, 19);
            this.lnkRefresh.Name = "lnkRefresh";
            this.lnkRefresh.Size = new System.Drawing.Size(44, 13);
            this.lnkRefresh.TabIndex = 8;
            this.lnkRefresh.TabStop = true;
            this.lnkRefresh.Text = "Refresh";
            this.lnkRefresh.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lnkRefresh_MouseClick);
            // 
            // frmStopLoss
            // 
            this.AutoSize = true;
            this.Controls.Add(this.lnkRefresh);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtStopLoss);
            this.Controls.Add(this.lblCreatedOnValue);
            this.Controls.Add(this.lblStopLoss);
            this.Controls.Add(this.lblClaimIDValue);
            this.Controls.Add(this.lblClaimID);
            this.Name = "frmStopLoss";
            this.Size = new System.Drawing.Size(1021, 45);
            this.FacetsWinPostOpen += new FacetsControlLibrary.FacetsEventHandler(this.frmStopLoss_FacetsWinPostOpen);
            this.FacetsSwitchToThisForm += new FacetsControlLibrary.FacetsEventHandler(this.frmStopLoss_FacetsSwitchToThisForm);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

       
        private void btnSave_Click(object sender, EventArgs e)
        {
            string strQueryResult = string.Empty;
            string strQuery = string.Empty;
            XA9BusinessLayer businesslayer = null;
            try
            {
                businesslayer = new XA9BusinessLayer();
                strQuery = businesslayer.SetStopLossForClaim(strClaimID, txtStopLoss.Text.Trim());
                GetDbRequest(strQuery, ref strQueryResult);
                MessageBox.Show("Stop Loss Value Saved", "STOP LOSS", MessageBoxButtons.OK, MessageBoxIcon.Information);

                //}
                /* Defect 3877 - Remove this validation - Start 
                if (txtStopLoss.Text.Trim().Equals(string.Empty))
                {
                    MessageBox.Show("Please enter a stop loss value for the claim", "STOP LOSS", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtStopLoss.Focus();
                }
                else
                {
                 Defect 3877 - Remove this validation - End */

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Occured - " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int frmStopLoss_FacetsWinPostOpen(object sender, FacetsEventArgs e)
        {
            if (((FacetsControlLibrary.FacetsBaseControl)sender).FacetsIsCurrentForm == true)
            {
                frmStopLoss_FacetsSwitchToThisForm(sender, e);
            }
            return 0;
        }

        private int frmStopLoss_FacetsSwitchToThisForm(object sender, FacetsEventArgs e)
        {
            string strQuery = string.Empty;
            string strQueryResult = string.Empty;
            
            XA9BusinessLayer businessLayer = null;
            bool recordsReturned;
            try
            {
                businessLayer = new XA9BusinessLayer();
                //strClaimID = GetClaimId();
                GetClaimId();
                GetPzapAppID(); // new
                lblClaimIDValue.Text = strClaimID;
                if (FormSetUpComplete == false)
                {
                    
                    FirstTimeFormSetUp();
                    //strQuery = businessLayer.GetUserAccessLvl(strPzapAppID, ContextData.ContextInstance.PZPZ_ID, ContextData.ContextInstance.UserId);
                    strQuery = businessLayer.GetUserAccessLvl(strPzapAppID, strPzpzID, ContextData.ContextInstance.UserId);

                    //Logger.LoggerInstance.ReportMessage("frmStopLoss_FacetsSwitchToThisForm", "strQuery is " + strQuery);

                    GetDbRequest(strQuery, ref strQueryResult);
                    recordsReturned = FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult);
                    //Logger.LoggerInstance.ReportMessage("frmStopLoss_FacetsSwitchToThisForm", "strQueryResult is " + strQueryResult);
                    if (recordsReturned == true)
                    {
                        strAccessLevel = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "ACSS_LVL", false);
                        strPzapAppID = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "PZAP_APP_ID", false);

                    }
                    else
                    {
                        strAccessLevel = string.Empty;
                    }
                    if(strAccessLevel.Equals(XA9Constants.ACSS_LVL_V))
                    {
                        txtStopLoss.ReadOnly = true;
                        txtStopLoss.Enabled = false;
                        //btnSave.Hide();
                        btnSave.Enabled = false;
                    }
                }

                strQuery = businessLayer.GetStopLossValueForClaim(strClaimID);
                GetDbRequest(strQuery, ref strQueryResult);

                recordsReturned = FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult);

                if (recordsReturned)
                {
                    //lblClaimIDValue.Text = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "STOP_LOSS", false);
                    txtStopLoss.Text = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "STOP_LOSS", false);
                }
                else
                {
                    //lblClaimIDValue.Text = string.Empty;
                    txtStopLoss.Text = string.Empty;
                }

                //if (string.IsNullOrEmpty(strClaimID) )

                //Logger.LoggerInstance.ReportMessage("UserId is : ", ContextData.ContextInstance.UserId);
                //Logger.LoggerInstance.ReportMessage("DatabaseId is : ", ContextData.ContextInstance.DatabaseId);
                //Logger.LoggerInstance.ReportMessage("DatabaseSourceId is : ", ContextData.ContextInstance.DatabaseSourceId);
                //Logger.LoggerInstance.ReportMessage("PZPZ_ID is : ", ContextData.ContextInstance.PZPZ_ID);
                //Logger.LoggerInstance.ReportMessage("ExitApplicationId is : ", ContextData.ContextInstance.ExitApplicationId);

                //Logger.LoggerInstance.ReportMessage("strPzapAppID is : ", strPzapAppID);
                //Logger.LoggerInstance.ReportMessage("strClaimID is : ", strClaimID);
                //Logger.LoggerInstance.ReportMessage("strClaimStatus is : ", strClaimStatus);
                //Logger.LoggerInstance.ReportMessage("strAccessLevel is : ", strAccessLevel);

                if (strPzapAppID.Equals("ICLQ") || string.IsNullOrEmpty(strClaimID) || strClaimStatus.Equals("02"))
                //if (string.IsNullOrEmpty(strClaimID))
                {
                    txtStopLoss.ReadOnly = true;
                    txtStopLoss.Enabled = false;
                    btnSave.Enabled = false;
                }
                else if (!strAccessLevel.Equals(XA9Constants.ACSS_LVL_V))
                {
                    txtStopLoss.ReadOnly = false;
                    txtStopLoss.Enabled = true;
                    btnSave.Enabled = true;
                }
            }
            catch (Exception ex)
            {

            }
            return 0;
        }

        private void lnkRefresh_MouseClick(object sender, MouseEventArgs e)
        {
            frmStopLoss_FacetsSwitchToThisForm(sender, null);
        }
    }
}

