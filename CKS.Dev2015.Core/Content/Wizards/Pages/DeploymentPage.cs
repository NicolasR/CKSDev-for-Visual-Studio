namespace CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards.Pages
{
    using CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards;
    using CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards.Models;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    class DeploymentPage : BaseWizardPage
    {
        private CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards.Models.DeploymentPresentationModel _model;

        private TableLayoutPanel allTextLayoutPanel;
        private IContainer components;
        private Label labelFullTrust;
        private LinkLabel labelHelp;
        private Label labelUserSolution;
        private Label localSiteQuestionLabel;
        private RadioButton radioFullTrust;
        private RadioButton radioUserSolution;
        private TableLayoutPanel siteUrlTable;
        private Label trustLevelQuestionLabel;
        private ComboBox urlComboBox;
        private Button validateButton;

        public DeploymentPage(ArtifactWizardForm wiz, DeploymentPresentationModel model) : base(wiz)
        {
            _model = model;
            InitializeComponent();
            base.LoadSettings();
            base.Headline = "WizardResources.DeploymentPageHeadline";
            urlComboBox.AutoCompleteMode = AutoCompleteMode.Suggest;
            urlComboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
            base.HelpKeyword = "VS.SharePointTools.SPE.FirstWizardPage";
        }

        private void allTextLayoutPanel_FontChanged(object sender, EventArgs e)
        {
            try
            {
                Font font = new Font(allTextLayoutPanel.Font, FontStyle.Bold);
                localSiteQuestionLabel.Font = font;
                trustLevelQuestionLabel.Font = font;
                radioFullTrust.Font = font;
                radioUserSolution.Font = font;
            }
            catch
            {
            }
        }

        protected override void ApplyChangesToPresentationModel()
        {
            _model.SaveChanges();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            ComponentResourceManager manager = new ComponentResourceManager(typeof(DeploymentPage));
            allTextLayoutPanel = new TableLayoutPanel();
            localSiteQuestionLabel = new Label();
            labelFullTrust = new Label();
            radioFullTrust = new RadioButton();
            labelUserSolution = new Label();
            radioUserSolution = new RadioButton();
            trustLevelQuestionLabel = new Label();
            siteUrlTable = new TableLayoutPanel();
            validateButton = new Button();
            urlComboBox = new ComboBox();
            labelHelp = new LinkLabel();
            allTextLayoutPanel.SuspendLayout();
            siteUrlTable.SuspendLayout();
            base.SuspendLayout();
            manager.ApplyResources(allTextLayoutPanel, "allTextLayoutPanel");
            allTextLayoutPanel.Controls.Add(localSiteQuestionLabel, 0, 0);
            allTextLayoutPanel.Controls.Add(labelFullTrust, 0, 6);
            allTextLayoutPanel.Controls.Add(radioFullTrust, 0, 5);
            allTextLayoutPanel.Controls.Add(labelUserSolution, 0, 4);
            allTextLayoutPanel.Controls.Add(radioUserSolution, 0, 3);
            allTextLayoutPanel.Controls.Add(trustLevelQuestionLabel, 0, 2);
            allTextLayoutPanel.Controls.Add(siteUrlTable, 0, 1);
            allTextLayoutPanel.Controls.Add(labelHelp, 0, 7);
            allTextLayoutPanel.Name = "allTextLayoutPanel";
            allTextLayoutPanel.FontChanged += new EventHandler(allTextLayoutPanel_FontChanged);
            manager.ApplyResources(localSiteQuestionLabel, "localSiteQuestionLabel");
            localSiteQuestionLabel.Name = "localSiteQuestionLabel";
            manager.ApplyResources(labelFullTrust, "labelFullTrust");
            labelFullTrust.Name = "labelFullTrust";
            manager.ApplyResources(radioFullTrust, "radioFullTrust");
            radioFullTrust.Name = "radioFullTrust";
            radioFullTrust.TabStop = true;
            radioFullTrust.UseVisualStyleBackColor = true;
            manager.ApplyResources(labelUserSolution, "labelUserSolution");
            labelUserSolution.Name = "labelUserSolution";
            manager.ApplyResources(radioUserSolution, "radioUserSolution");
            radioUserSolution.Name = "radioUserSolution";
            radioUserSolution.TabStop = true;
            radioUserSolution.UseVisualStyleBackColor = true;
            radioUserSolution.CheckedChanged += new EventHandler(radioUserSolution_CheckedChanged);
            manager.ApplyResources(trustLevelQuestionLabel, "trustLevelQuestionLabel");
            trustLevelQuestionLabel.Name = "trustLevelQuestionLabel";
            manager.ApplyResources(siteUrlTable, "siteUrlTable");
            siteUrlTable.Controls.Add(validateButton, 1, 0);
            siteUrlTable.Controls.Add(urlComboBox, 0, 0);
            siteUrlTable.Name = "siteUrlTable";
            manager.ApplyResources(validateButton, "validateButton");
            validateButton.Name = "validateButton";
            validateButton.UseVisualStyleBackColor = true;
            validateButton.Click += new EventHandler(validateButton_Click);
            manager.ApplyResources(urlComboBox, "urlComboBox");
            urlComboBox.FormattingEnabled = true;
            urlComboBox.Name = "urlComboBox";
            urlComboBox.SelectedIndexChanged += new EventHandler(urlComboBox_SelectedIndexChanged);
            urlComboBox.TextUpdate += new EventHandler(urlComboBox_TextUpdate);
            manager.ApplyResources(labelHelp, "labelHelp");
            labelHelp.Name = "labelHelp";
            labelHelp.TabStop = true;
            labelHelp.LinkClicked += new LinkLabelLinkClickedEventHandler(labelHelp_LinkClicked);
            manager.ApplyResources(this, "$this");
            base.AutoScaleMode = AutoScaleMode.Font;
            base.Controls.Add(allTextLayoutPanel);
            base.Name = "DeploymentPage";
            allTextLayoutPanel.ResumeLayout(false);
            allTextLayoutPanel.PerformLayout();
            siteUrlTable.ResumeLayout(false);
            siteUrlTable.PerformLayout();
            base.ResumeLayout(false);
        }

        protected override bool IsCompletelyValid()
        {
            bool flag = true;
            if (_model.ValidateUrlWithSharePoint)
            {
                string str;
                flag = _model.IsUrlValid(out str);
                if (!flag)
                {
                    //RtlAwareMessageBox.ShowError(this, str, WizardResources.SharePointConnectionErrorCaption);
                }
            }
            return flag;
        }

        private void labelHelp_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //WizardHelpers.DisplayTopicFromF1Keyword("VS.SharePointTools.Project.SandboxedSolutions");
        }

        protected override void LoadSettingsFromPresentationModel()
        {
            if (!_model.EnableUserSolutionInput)
            {
                radioUserSolution.Enabled = false;
                radioFullTrust.Checked = true;
            }
            else
            {
                radioUserSolution.Checked = _model.IsSandboxedSolution;
                radioFullTrust.Checked = !_model.IsSandboxedSolution;
            }
            urlComboBox.DataSource = _model.MruUrlList;
            urlComboBox.Text = _model.Url;
            base.Skippable = _model.IsOptional;
        }

        public override bool OnDeactivate()
        {
            if (_model.IsValidatingWithSharePoint)
            {
                return false;
            }
            return base.OnDeactivate();
        }

        private void radioUserSolution_CheckedChanged(object sender, EventArgs e)
        {
            _model.IsSandboxedSolution = radioUserSolution.Checked;
        }

        protected virtual void SiteUrlChanged()
        {
            if ((_model != null) && _model.IsUrlModified)
            {
                base.Wizard.InvalidateRest();
            }
        }

        private void urlComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            _model.Url = urlComboBox.Text;
            SiteUrlChanged();
            base.Wizard.OnValidationStateChanged(this);
        }

        private void urlComboBox_TextUpdate(object sender, EventArgs e)
        {
            _model.Url = urlComboBox.Text;
            SiteUrlChanged();
            base.Wizard.OnValidationStateChanged(this);
        }

        private void validateButton_Click(object sender, EventArgs e)
        {
            string str;
            if (!_model.IsUrlValid(out str))
            {
                //RtlAwareMessageBox.ShowError(this, str, WizardResources.SharePointConnectionErrorCaption);
            }
            else
            {
                //string text = string.Format(CultureInfo.CurrentCulture, WizardResources.ValidSiteUrlMessage, new object[] { _model.Url });
                //RtlAwareMessageBox.ShowMessage(this, text, WizardResources.ValidSiteUrlCaption);
            }
        }

        public override bool IsDataValid
        {
            get
            {
                return _model.IsUrlFormatValid;
            }
        }
    }
}

