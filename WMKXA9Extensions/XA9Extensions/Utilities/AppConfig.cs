/*************************************************************************************************************************************************
 *  Class               : AppConfig
 *  Description         : This static class will provide access to the business settings in app.config file
 *  Used by             : 
 *  Author              : TriZetto Inc (viswan jayaraman). 
 *************************************************************************************************************************************************
 * Ver                  Date                    Modification Description
 * 1.0                  04/25/2016              Initial Version
 *************************************************************************************************************************************************
 */
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using XA9Extensions.BusinessLayer;

namespace XA9Extensions.Utilities
{
    public static class AppConfig
    {
        private static SettingElementCollection _colElements;

        #region constructor
        /// <summary>
        /// Static constructor - use CongigurationManager to read the contents of the config file
        /// </summary>
        static AppConfig()
        {
           
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
                ConfigurationSectionGroup group = config.SectionGroups["applicationSettings"];
                ClientSettingsSection section = (ClientSettingsSection)group.Sections["XA9Extensions.Properties.Settings"];
                _colElements = section.Settings;
            }
            catch
            {
                //e.g. missing configuration file
                _colElements = null;
            }
        }//constructor
        #endregion constructor

        #region Properties (i.e. configuration settings)
        //These read-only properties must match the contents of Settings.settings

        /// <summary>
        /// LogFileLocation value from app.config 
        /// </summary>
        public static string LogFileLocation
        {
            get
            {
                if (_colElements == null) return string.Empty;

                return _colElements.Get("LogFileLocation").Value.ValueXml.InnerText;
                
            }
        }

        /// <summary>
        /// LoggingLevel value from app.config 
        /// </summary>
        public static LoggingLevelEnum LoggingLevel
        {
            get
            {
                if (_colElements == null) return LoggingLevelEnum.ErrorsOnly;

                return (LoggingLevelEnum)int.Parse(_colElements.Get("LoggingLevel").Value.ValueXml.InnerText);
            }
        }

        /// <summary>
        /// LoggingLevel value from app.config 
        /// </summary>
        public static string[] InpatientHospType
        {
            get
            {
                //if (_colElements == null) return string.Empty;

                string strInpHospTypes = _colElements.Get("InpatientHospType").Value.ValueXml.InnerText;
                return strInpHospTypes.Split(XA9Constants.CHR_COMMA);

            }
        }

        /// <summary>
        /// LoggingLevel value from app.config 
        /// </summary>
        public static string SerlQualifier
        {
            get
            {
                if (_colElements == null) return string.Empty;

                return _colElements.Get("SerlQualifier").Value.ValueXml.InnerText;
            }
        }
        /// <summary>
        /// List of claims status to exclude from app.config 
        /// </summary>
        public static string[] ExcludeClaimStatus
        {
            get
            {
                //if (_colElements == null) return string.Empty;

                string strExcludeClmSts = _colElements.Get("ExcludeClaimStatus").Value.ValueXml.InnerText;
                return strExcludeClmSts.Split(XA9Constants.CHR_COMMA);
            }
        }

        /// <summary>
        /// Medicare Suppliment Trading Partners 
        /// </summary>
        public static string[] MedSupTradingPartners
        {
            get
            {
                //if (_colElements == null) return string.Empty;

                string strMedSupTradingPartners = _colElements.Get("MedSupTradingPartners").Value.ValueXml.InnerText;
                return strMedSupTradingPartners.Split(XA9Constants.CHR_COMMA);

            }
        }

        /// <summary>
        /// Medicare Suppliment User Data
        /// </summary>
        public static string[] MedSupUserData
        {
            get
            {
                //if (_colElements == null) return string.Empty;

                string strMedSupUserData = _colElements.Get("MedSupUserData").Value.ValueXml.InnerText;
                return strMedSupUserData.Split(XA9Constants.CHR_COMMA);

            }
        }
        /// <summary>
        /// LoggingLevel value from app.config 
        /// </summary>
        public static string BDCOverrideExcdID
        {
            get
            {
                if (_colElements == null) return string.Empty;

                return _colElements.Get("BDCEXCDID").Value.ValueXml.InnerText;
            }
        }
        /// <summary>
        /// LoggingLevel value from app.config 
        /// </summary>
        public static string CopayOverrideExcdID
        {
            get
            {
                if (_colElements == null) return string.Empty;

                return _colElements.Get("OCPDEXCDID").Value.ValueXml.InnerText;
            }
        }
        /// <summary>
        /// FacetsOnlineApps values from app.config 
        /// </summary>
        public static string[] FacetsOnlineApps
        {
            get
            {
                //if (_colElements == null) return string.Empty;

                string strFacetsOnlineApps = _colElements.Get("FacetsOnlineApps").Value.ValueXml.InnerText;
                return strFacetsOnlineApps.Split(XA9Constants.CHR_COMMA);

            }
        }
        /// <summary>
        /// FacetsBatchApps values from app.config 
        /// </summary>
        public static string[] FacetsBatchApps
        {
            get
            {
                //if (_colElements == null) return string.Empty;

                string strFacetsBatchApps = _colElements.Get("FacetsBatchApps").Value.ValueXml.InnerText;
                return strFacetsBatchApps.Split(XA9Constants.CHR_COMMA);

            }
        }

        /// <summary>
        /// ITS Non Par EXCD_ID value from app.config 
        /// </summary>
        public static string[] ITSNPEXCDID
        {
            get
            {
                /*if (_colElements == null) return string.Empty;

                return _colElements.Get("ITSNPEXCDID").Value.ValueXml.InnerText;*/
                string arrITSNPEXCDID = _colElements.Get("ITSNPEXCDID").Value.ValueXml.InnerText;
                return arrITSNPEXCDID.Split(XA9Constants.CHR_COMMA);
            }
        }

        /// <summary>
        /// Non Par Provider Classifications - PricingMethChg values from app.config 
        /// </summary>
        public static string[] NonParProvClass
        {
            get
            {

                string strNonParProvClass = _colElements.Get("NonParProvClass").Value.ValueXml.InnerText;
                return strNonParProvClass.Split(XA9Constants.CHR_COMMA);

            }
        }

        /// <summary>
        /// Non Par Provider Hold Harmless SF - ITSHoldHarmlessSF values from app.config 
        /// </summary>
        public static string[] ITSHoldHarmlessSF
        {
            get
            {

                string strITSHoldHarmlessSF = _colElements.Get("ITSHoldHarmlessSF").Value.ValueXml.InnerText;
                return strITSHoldHarmlessSF.Split(XA9Constants.CHR_COMMA);

            }
        }

        /// <summary>
        /// Non Par Provider Force Apply WMK Price SF - WMKForcePricingSF values from app.config 
        /// </summary>
        public static string[] WMKForcePricingSF
        {
            get
            {

                string strITSHoldHarmlessSF = _colElements.Get("WMKForcePricingSF").Value.ValueXml.InnerText;
                return strITSHoldHarmlessSF.Split(XA9Constants.CHR_COMMA);

            }
        }

        /// <summary>
        /// ITS Non Par Facility Percent value from app.config 
        /// </summary>
        public static string ITSNonParFacilityPercent
        {
            get
            {
                if (_colElements == null) return string.Empty;

                return _colElements.Get("ITSNonParFacPCT").Value.ValueXml.InnerText;
            }
        }

        /// <summary>
        /// ITS Proxy Proivider ID to pay at 100 % from app.config 
        /// </summary>
        public static string ITSProxyPrprIDPayAt100Pct
        {
            get
            {
                if (_colElements == null) return string.Empty;

                return _colElements.Get("ITSProxyPrprIDPayAt100Pct").Value.ValueXml.InnerText;
            }
        }

        /// <summary>
        /// ITS Non Par EXCD_ID value from app.config 
        /// </summary>
        public static string ITSILExcdID
        {
            get
            {
                if (_colElements == null) return string.Empty;

                return _colElements.Get("ITSILEXCDID").Value.ValueXml.InnerText;
            }
        }

        /// <summary>
        /// ITSIKEXCDID value from app.config 
        /// </summary>
        public static string ITSIKEXCDID
        {
            get
            {
                if (_colElements == null) return string.Empty;

                return _colElements.Get("ITSIKEXCDID").Value.ValueXml.InnerText;
            }
        }

        /// <summary>
        /// ITS Non Par Line Level EXCD_ID value from app.config 
        /// </summary>
        public static string ITSNonParNPHostLineLevelOverrideExcdID
        {
            get
            {
                if (_colElements == null) return string.Empty;

                return _colElements.Get("ITSNPLineOvrHostEXCDID").Value.ValueXml.InnerText;
            }
        }

        /// <summary>
        /// ITS Non Par Claim Level EXCD_ID value from app.config 
        /// </summary>
        public static string ITSNonParNPClaimLevelOverrideExcdID
        {
            get
            {
                if (_colElements == null) return string.Empty;

                return _colElements.Get("ITSNPClaimOvrEXCDID").Value.ValueXml.InnerText;
            }
        }

       /// <summary>
        /// ITSNPProfEmergencyProcCodes value from app.config 
        /// </summary>
        public static string[] ITSNPProfEmergencyProcCodes
        {
            get
            {
                //if (_colElements == null) return string.Empty;

                string arrITSNPProfEmergencyProcCodes = _colElements.Get("ITSNPProfEmergencyProcCodes").Value.ValueXml.InnerText;
                return arrITSNPProfEmergencyProcCodes.Split(XA9Constants.CHR_COMMA);

            }
        }

        /// <summary>
        /// ITSNPHospEmergencyRevCodes value from app.config 
        /// </summary>
        public static string[] ITSNPHospEmergencyRevCodes
        {
            get
            {
                //if (_colElements == null) return string.Empty;

                string arrITSNPHospEmergencyRevCodes = _colElements.Get("ITSNPHospEmergencyRevCodes").Value.ValueXml.InnerText;
                return arrITSNPHospEmergencyRevCodes.Split(XA9Constants.CHR_COMMA);

            }
        }

        /// <summary>
        /// ITSNPProfEmergencyPOS value from app.config 
        /// </summary>
        public static string[] ITSNPProfEmergencyPOS
        {
            get
            {
                //if (_colElements == null) return string.Empty;

                string arrITSNPProfEmergencyPOS = _colElements.Get("ITSNPProfEmergencyPOS").Value.ValueXml.InnerText;
                return arrITSNPProfEmergencyPOS.Split(XA9Constants.CHR_COMMA);

            }
        }

        /// <summary>
        /// ITSNPManualEPOverride value from app.config 
        /// </summary>
        public static string[] ITSNPManualEPOverride
        {
            get
            {
                //if (_colElements == null) return string.Empty;

                string arrITSNPManualEPOverride = _colElements.Get("ITSNPManualEPOverride").Value.ValueXml.InnerText;
                return arrITSNPManualEPOverride.Split(XA9Constants.CHR_COMMA);

            }
        }

        /// <summary>
        /// ITSInpHospNPPRRevertToChg value from app.config 
        /// </summary>
        public static string[] ITSNPInpHospNPPRRevertToChg
        {
            get
            {
                //if (_colElements == null) return string.Empty;

                string arrITSInpHospNPPRRevertToChg = _colElements.Get("ITSNPInpHospNPPRRevertToChg").Value.ValueXml.InnerText;
                return arrITSInpHospNPPRRevertToChg.Split(XA9Constants.CHR_COMMA);

            }
        }

        /// <summary>
        /// ITSProfNPPRRevertToChg value from app.config 
        /// </summary>
        public static string[] ITSNPProfNPPRRevertToChg
        {
            get
            {
                //if (_colElements == null) return string.Empty;

                string arrITSProfNPPRRevertToChg = _colElements.Get("ITSNPProfNPPRRevertToChg").Value.ValueXml.InnerText;
                return arrITSProfNPPRRevertToChg.Split(XA9Constants.CHR_COMMA);

            }
        }

        /// <summary>
        /// ITSNPProfMCTRRevertToChg value from app.config 
        /// </summary>
        public static string[] ITSNPProfMCTRRevertToChg
        {
            get
            {
                //if (_colElements == null) return string.Empty;

                string arrITSNPProfMCTRRevertToChg = _colElements.Get("ITSNPProfMCTRRevertToChg").Value.ValueXml.InnerText;
                return arrITSNPProfMCTRRevertToChg.Split(XA9Constants.CHR_COMMA);

            }
        }
        /// <summary>
        /// ITSNonParMedSupDeliveryMtd value from app.config 
        /// </summary>
        public static string[] ITSNonParMedSupDeliveryMtd
        {
            get
            {
                //if (_colElements == null) return string.Empty;

                string arrITSNonParMedSupDeliveryMtd = _colElements.Get("ITSNonParMedSupDeliveryMtd").Value.ValueXml.InnerText;
                return arrITSNonParMedSupDeliveryMtd.Split(XA9Constants.CHR_COMMA);

            }
        }

        /// <summary>
        /// DME DMEStartDate value from app.config 
        /// </summary>
        public static string DMEStartDate
        {
            get
            {
                if (_colElements == null) return string.Empty;

                return _colElements.Get("DMEStartDate").Value.ValueXml.InnerText;
            }
        }

        /// <summary>
        ///DME DMERentalCount value from app.config 
        /// </summary>
        public static string DMERentalCount
        {
            get
            {
                if (_colElements == null) return string.Empty;

                return _colElements.Get("DMERentalCount").Value.ValueXml.InnerText;
            }
        }

        /// <summary>
        /// List of claims status to exclude from app.config specifically for One Copay Per Day
        /// </summary>
        public static string[] ExcludeClaimStatusForOCPD
        {
            get
            {
                //if (_colElements == null) return string.Empty;

                string strExcludeClmSts = _colElements.Get("ExcludeClaimStatusForOCPD").Value.ValueXml.InnerText;
                return strExcludeClmSts.Split(XA9Constants.CHR_COMMA);
            }
        }

        /// <summary>
        /// Non Par Provider Classifications for Once In A Life Time - NonParProvClassOLFTM values from app.config 
        /// </summary>
        public static string[] NonParProvClassForOLFTM
        {
            get
            {

                string strNonParProvClass = _colElements.Get("NonParProvClassOLFTM").Value.ValueXml.InnerText;
                return strNonParProvClass.Split(XA9Constants.CHR_COMMA);

            }
        }

        /// <summary>
        ///Once in a Lifetime EXCD_ID value from app.config 
        /// </summary>
        public static string OnceInALfTmExcdID
        {
            get
            {
                if (_colElements == null) return string.Empty;

                return _colElements.Get("OnceInALFExcdID").Value.ValueXml.InnerText;
            }
        }

        /// <summary>
        ///Once in a Lifetime Cdor value from app.config 
        /// </summary>
        public static string OnceInALfTmCdorVal
        {
            get
            {
                if (_colElements == null) return string.Empty;

                return _colElements.Get("OnceInALFSRCdorVal").Value.ValueXml.InnerText;
            }
        }

        /// <summary>
        /// ITS Non Par Line Level EXCD_ID value from app.config 
        /// </summary>
        public static string ITSNonParNPWMKLineLevelOverrideExcdID
        {
            get
            {
                if (_colElements == null) return string.Empty;

                return _colElements.Get("ITSNPLineOvrWMKEXCDID").Value.ValueXml.InnerText;
            }
        }

        /// <summary>
        /// PR Deny Clcb Type values from app.config 
        /// </summary>
        public static string[] PRDenyClcbType
        {
            get
            {

                string strNonParProvClass = _colElements.Get("PRDenyClcbType").Value.ValueXml.InnerText;
                return strNonParProvClass.Split(XA9Constants.CHR_COMMA);

            }
        }

        /// <summary>
        /// ITS Non Par Line Level manual price EXCD_ID value from (ITSEPManualPriceExcdID) app.config 
        /// </summary>
        public static string ITSEPManualPriceExcdID
        {
            get
            {
                if (_colElements == null) return string.Empty;

                return _colElements.Get("ITSEPManualPriceExcdID").Value.ValueXml.InnerText;
            }
        }

        /// <summary>
        /// PR Deny Clcb Type values from app.config 
        /// </summary>
        public static string[] PRDenyInStates
        {
            get
            {

                string strNonParProvClass = _colElements.Get("PRDenyInStates").Value.ValueXml.InnerText;
                return strNonParProvClass.Split(XA9Constants.CHR_COMMA);

            }
        }

        #endregion Properties (i.e. configuration settings)

    }//End of class Utilities

    public enum LoggingLevelEnum
    {
        None = 0,
        ErrorsOnly = 1,
        Short = 2,  //summary line + errors
        Full = 3,
        Verbose = 4  //XML passed between Facets and iCES (both ways)
    }    

}//End of namespace ClmDMEProc.Utilities
