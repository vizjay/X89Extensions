using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text;
using FacetsControlLibrary;
using System.Windows.Forms;
using XA9Extensions.BusinessLayer;
using XA9Extensions.DataAccessLayer;
using XA9Extensions.Utilities;

namespace XA9Extensions
{
    public class frmXA9BDCMain : FacetsBaseControl
    {
        private Label lblBDCValue;
        private Label lblBDCText;
        private Label lblCreateDT;
        private Label lblCreatedOnValue;
        private LinkLabel lnkRefresh;
        private bool FormSetUpComplete = false;
        
       

        


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

        /// <summary>
        /// Get Claim ID of currently opened claim in Facets online
        /// </summary>
        /// <returns></returns>
        private string GetClaimId()
        {
            XDocument xdocClaimData = null;
            string strClaimData = string.Empty;
            string strClaimID = string.Empty;

            try
            {
                GetData("CLCL", ref strClaimData);
                if (!strClaimData.Equals(string.Empty))
                {
                    xdocClaimData = XDocument.Parse(strClaimData);
                    strClaimID = xdocClaimData.XPathSelectElement(string.Format(XA9Constants.XML_COLLUMN_XPATH, "CLCL_ID")).Value;
                }
                else
                {
                    strClaimID = string.Empty;
                }

                lblBDCValue.Text = strClaimID;
            }
            catch (Exception ex)
            {
                lblBDCValue.Text = string.Empty;
            }
            
            return strClaimID;
        }

        // <summary>
        /// Set up Facets context object
        /// </summary>
        
        private void Initiate()
        {
            // Sets the Facets connection ifnormation for establishing the Data connection
            string strSignOnData = string.Empty;
            GetData(XA9Constants.GETDATASGN0, ref strSignOnData);
            ContextData.ContextInstance.Initialize(strSignOnData);
        }
        public frmXA9BDCMain()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.lblBDCText = new System.Windows.Forms.Label();
            this.lblBDCValue = new System.Windows.Forms.Label();
            this.lblCreateDT = new System.Windows.Forms.Label();
            this.lblCreatedOnValue = new System.Windows.Forms.Label();
            this.lnkRefresh = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // lblBDCText
            // 
            this.lblBDCText.AutoSize = true;
            this.lblBDCText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBDCText.Location = new System.Drawing.Point(17, 18);
            this.lblBDCText.Name = "lblBDCText";
            this.lblBDCText.Size = new System.Drawing.Size(36, 13);
            this.lblBDCText.TabIndex = 2;
            this.lblBDCText.Text = "BDC:";
            // 
            // lblBDCValue
            // 
            this.lblBDCValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBDCValue.Location = new System.Drawing.Point(57, 18);
            this.lblBDCValue.Name = "lblBDCValue";
            this.lblBDCValue.Size = new System.Drawing.Size(107, 13);
            this.lblBDCValue.TabIndex = 3;
            this.lblBDCValue.Text = "0000000001NN";
            // 
            // lblCreateDT
            // 
            this.lblCreateDT.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCreateDT.Location = new System.Drawing.Point(170, 18);
            this.lblCreateDT.Name = "lblCreateDT";
            this.lblCreateDT.Size = new System.Drawing.Size(89, 13);
            this.lblCreateDT.TabIndex = 4;
            this.lblCreateDT.Text = "Created On:";
            // 
            // lblCreatedOnValue
            // 
            this.lblCreatedOnValue.AutoSize = true;
            this.lblCreatedOnValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCreatedOnValue.Location = new System.Drawing.Point(253, 18);
            this.lblCreatedOnValue.Name = "lblCreatedOnValue";
            this.lblCreatedOnValue.Size = new System.Drawing.Size(58, 13);
            this.lblCreatedOnValue.TabIndex = 5;
            this.lblCreatedOnValue.Text = "NODATE";
            // 
            // lnkRefresh
            // 
            this.lnkRefresh.AutoSize = true;
            this.lnkRefresh.Location = new System.Drawing.Point(431, 18);
            this.lnkRefresh.Name = "lnkRefresh";
            this.lnkRefresh.Size = new System.Drawing.Size(44, 13);
            this.lnkRefresh.TabIndex = 9;
            this.lnkRefresh.TabStop = true;
            this.lnkRefresh.Text = "Refresh";
            this.lnkRefresh.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lnkRefresh_MouseClick);
            // 
            // frmXA9BDCMain
            // 
            this.AutoSize = true;
            this.Controls.Add(this.lnkRefresh);
            this.Controls.Add(this.lblCreatedOnValue);
            this.Controls.Add(this.lblCreateDT);
            this.Controls.Add(this.lblBDCValue);
            this.Controls.Add(this.lblBDCText);
            this.Name = "frmXA9BDCMain";
            this.Size = new System.Drawing.Size(1021, 45);
            this.FacetsWinPostOpen += new FacetsControlLibrary.FacetsEventHandler(this.frmXA9BDCMain_FacetsWinPostOpen);
            this.FacetsSwitchToThisForm += new FacetsControlLibrary.FacetsEventHandler(this.frmXA9BDCMain_FacetsSwitchToThisForm);
            this.Load += new System.EventHandler(this.frmXA9BDCMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void frmXA9BDCMain_Load(object sender, EventArgs e)
        {

        }

        private int frmXA9BDCMain_FacetsSwitchToThisForm(object sender, FacetsEventArgs e)
        {
            
            lblBDCValue.Text = string.Empty;

            XA9BusinessLayer objXA9BL = new XA9BusinessLayer();
            string strClaimID = string.Empty;
            //string strBdcForClaimXml = string.Empty;
            string strQuery = string.Empty;
            string strQueryResult = string.Empty;
            //XDocument xdocBdcCode = null;
            string strBdcForClaim = string.Empty;

            string strBdcPgmID = string.Empty;
            string strBdcSubPgmID = string.Empty;
            string strBdcFlag = string.Empty;
            string strBdcName = string.Empty;
            string strCreateDate = string.Empty;
            bool recordsReturned = false;
            string strCustom = string.Empty;
            bool isCustomDataFound = false;
            try
            {
                if (FormSetUpComplete == false)
                {
                    FirstTimeFormSetUp();
                }


                /* Check whether Custom property has BDC Code */

                this.GetData("CUSTOM", ref strCustom);


                if (!string.IsNullOrEmpty(strCustom))
                {
                    //Logger.LoggerInstance.ReportMessage("Custom data for BDC is : ", strCustom);
                    XDocument xdocBDCValues = XDocument.Parse(strCustom);
                    //string strBDCSave = xdocBDCValues.Elements().FirstOrDefault(f => f.Attribute("name").Value.Equals("POSTSAVEBDC")).Value;
                    string strBDCSave = FacetsData.FacetsInstance.GetDbSingleDataItem(strCustom, "", "POSTSAVEBDC", false);
                    if (!string.IsNullOrEmpty(strBDCSave))
                    {
                        isCustomDataFound = true;
                        //Logger.LoggerInstance.ReportMessage("Custom Data Found : ", isCustomDataFound.ToString());
                        if (strBDCSave.Equals("Y"))
                        {
                            /*strBdcPgmID = xdocBDCValues.Elements().FirstOrDefault(elmPgmID => elmPgmID.Attribute("name").Value.Equals("BDCPGMID")).Value;
                            strBdcSubPgmID = xdocBDCValues.Elements().FirstOrDefault(elmSubPgmID => elmSubPgmID.Attribute("name").Value.Equals("BDCSUBPGMID")).Value;
                            strBdcFlag = xdocBDCValues.Elements().FirstOrDefault(elmBdcFlag => elmBdcFlag.Attribute("name").Value.Equals("BDCFLAG")).Value;
                            strBdcName = xdocBDCValues.Elements().FirstOrDefault(elmPgmName => elmPgmName.Attribute("name").Value.Equals("BDCPGMNM")).Value;
                            //strCreateDate = string.Empty;
                            strCreateDate = DateTime.Parse(strCreateDate).ToShortDateString() + "  " + DateTime.Parse(strCreateDate).ToShortTimeString();*/

                            strBdcPgmID = FacetsData.FacetsInstance.GetDbSingleDataItem(strCustom, "", "BDCPGMID", false);
                            strBdcSubPgmID = FacetsData.FacetsInstance.GetDbSingleDataItem(strCustom, "", "BDCSUBPGMID", false);
                            strBdcName = FacetsData.FacetsInstance.GetDbSingleDataItem(strCustom, "", "BDCPGMNM", false);
                            strBdcFlag = FacetsData.FacetsInstance.GetDbSingleDataItem(strCustom, "", "BDCFLAG", false);
                            if (!string.IsNullOrEmpty(strBdcPgmID) && !string.IsNullOrEmpty(strBdcSubPgmID))
                                strCreateDate = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString(); //DateTime.Parse(Datetim).ToShortDateString() + "  " + DateTime.Parse(strCreateDate).ToShortTimeString();
                            else
                                strCreateDate = "";
                        }
                    }

                }
                //else
                if(isCustomDataFound == false)
                {
                    strClaimID = GetClaimId();

                    strQuery = objXA9BL.GetBdcCodeForClaim(strClaimID);
                    GetDbRequest(strQuery, ref strQueryResult);

                    recordsReturned = FacetsData.FacetsInstance.IsDbDataAvailable(strQueryResult);

                    if (recordsReturned == true)
                    {
                        //xdocBdcCode = XDocument.Parse(strQueryResult);
                        strBdcPgmID = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "BDC_PGM_ID", false);
                        strBdcSubPgmID = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "BDC_SUB_PGM_ID", false);
                        strBdcFlag = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "BDC_FLAG", false);
                        strBdcName = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "BDC_PGM_NM", false);
                        //strBdcName = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "BDC_PGM_NM", false);
                        strCreateDate = FacetsData.FacetsInstance.GetDbSingleDataItem(strQueryResult, "DATA", "CREATE_DT", false);
                        strCreateDate = DateTime.Parse(strCreateDate).ToShortDateString() + "  " + DateTime.Parse(strCreateDate).ToShortTimeString();
                    }
                }

                lblBDCValue.Text = strBdcPgmID + strBdcSubPgmID + strBdcFlag;
                //lblCreatedOnValue.Text = DateTime.Parse(strCreateDate).ToShortDateString() + "  " + DateTime.Parse(strCreateDate).ToShortTimeString();
                lblCreatedOnValue.Text = strCreateDate;
                        //strBdcForClaim = strBdcPgmID + strBdcSubPgmID + strBdcFlag + "    " + strBdcName;
                 
                
                    /*
                if true
                {
                    lblBDCValue.Text = string.Empty;
                    lblCreatedOnValue.Text = string.Empty;
                }*/

                

            }
            catch (Exception ex)
            {
                //Logger.LoggerInstance.ReportMessage("EXCEPTION : ", ex.Message);
                lblBDCValue.Text = string.Empty;
                lblCreatedOnValue.Text = string.Empty;
            }
            
            return default(int);
        }

        private int frmXA9BDCMain_FacetsWinPostOpen(object sender, FacetsEventArgs e)
        {
            
            if (((FacetsControlLibrary.FacetsBaseControl)sender).FacetsIsCurrentForm == true)
            {

                frmXA9BDCMain_FacetsSwitchToThisForm(sender, e);
            }
            return 0;
        }

        private void lnkRefresh_MouseClick(object sender, MouseEventArgs e)
        {
            frmXA9BDCMain_FacetsSwitchToThisForm(sender, null);
        }
    }
}
