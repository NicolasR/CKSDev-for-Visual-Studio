using System;
using System.Windows.Forms;

namespace CKS.Dev2015.VisualStudio.SharePoint.Explorer.Dialogs
{
    /// <summary>
    /// The DeveloperDashboardSettingsDialog used to show options for the setting.
    /// </summary>
    public partial class DeveloperDashboardSettingsDialog : Form
    {
        #region Properties

        /// <summary>
        /// Gets or sets the selected level.
        /// </summary>
        public string SelectedLevel
        {
            get;
            set;
        }

        #endregion


        /// <summary>
        /// Create a new instance of the DeveloperDashboardSettingsDialog object.
        /// </summary>
        public DeveloperDashboardSettingsDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// On load to set the current value.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The EventArgs.</param>
        private void DeveloperDashboardSettingsDialog_Load(object sender, EventArgs e)
        {
            if (SelectedLevel.ToLower() == "off")
            {
                radDisplayLevelOff.Checked = true;
            }
            if (SelectedLevel.ToLower() == "ondemand")
            {
                radDisplayLevelOnDemand.Checked = true;
            }

            if (SelectedLevel.ToLower() == "on")
            {
                radDisplayLevelOn.Checked = true;
            }
        }

        /// <summary>
        /// Handler to set the new value based on user selection.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The EventArgs.</param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (radDisplayLevelOff.Checked)
            {
                SelectedLevel = "Off";
            }

            if (radDisplayLevelOnDemand.Checked)
            {
                SelectedLevel = "OnDemand";
            }

            if (radDisplayLevelOn.Checked)
            {
                SelectedLevel = "On";
            }
        }
    }
}

