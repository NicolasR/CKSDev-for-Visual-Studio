using CKS.Dev2015.VisualStudio.SharePoint.Commands.Info;
using CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards.Models;
using System;
using System.Windows.Forms;
using CKSProperties = CKS.Dev2015.Core.Properties.Resources;
namespace CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards.Pages
{
    /// <summary>
    /// The first page of the Web Template wizard.
    /// </summary>
    class WebTemplatePage : BaseWizardPage
    {
        #region Controls

        private System.Windows.Forms.ErrorProvider errMessages;
        private TableLayoutPanel tableLayoutPanel1;
        private Label lblIntroduction;
        private LinkLabel lnkMSDNArticle;
        private Label lblRequired;
        private Label lblTitle;
        private TextBox txtTitle;
        private Label lblWebTemplate;
        private System.ComponentModel.IContainer components;

        #endregion

        private ComboBox cboWebTemplates;

        #region Fields

        /// <summary>
        /// Field to hold the presentation model
        /// </summary>
        WebTemplatePresentationModel _model;

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="WebTemplatePage"/> class.
        /// </summary>
        /// <param name="wizard">The wizard.</param>
        /// <param name="model">The model.</param>
        public WebTemplatePage(ArtifactWizardForm wizard, WebTemplatePresentationModel model)
            : base(wizard)
        {
            _model = model;
            InitializeComponent();
            base.LoadSettings();
            base.Headline = CKSProperties.WebTemplatePage1_StepTitle;
            base.HelpKeyword = "VS.SharePointTools.SPE.WebTemplate";
        }

        /// <summary>
        /// Load the settings from the presentation model.
        /// </summary>
        protected override void LoadSettingsFromPresentationModel()
        {
            base.Skippable = _model.IsOptional;

            txtTitle.Text = _model.Title;
            if (_model.SelectedWebTemplate == null)
            {
                //cboWebTemplates.SelectedIndex = 0;
            }
            else
            {
                string val = _model.SelectedWebTemplate.ToString();
                cboWebTemplates.SelectedIndex = cboWebTemplates.FindStringExact(val);
            }

        }

        /// <summary>
        /// Do the databinding from the model to the form components.
        /// </summary>
        /// <returns>True</returns>
        public override bool OnActivate()
        {
            //Do any databinding required here


            //Web Templates
            cboWebTemplates.DataSource = this._model.AvailableWebTemplates;
            //cboRegistrationType.DisplayMember = "DisplayMember";
            //cboRegistrationType.ValueMember = "ValueMember";

            txtTitle.Text = _model.Title;
            if (_model.SelectedWebTemplate == null)
            {
                //cboWebTemplates.SelectedIndex = 0;
            }
            else
            {
                string val = _model.SelectedWebTemplate.ToString();
                cboWebTemplates.SelectedIndex = cboWebTemplates.FindStringExact(val);
            }

            //if (String.IsNullOrEmpty(_model.RegistrationType))
            //{
            //    cboRegistrationType.SelectedIndex = 0;
            //}
            //else
            //{
            //    cboRegistrationType.SelectedIndex = cboRegistrationType.FindStringExact(_model.RegistrationType);
            //}

            return base.OnActivate();
        }

        /// <summary>
        /// Apply the changes to the model and invoke its save changes.
        /// </summary>
        protected override void ApplyChangesToPresentationModel()
        {
            _model.Title = txtTitle.Text;
            _model.SelectedWebTemplate = cboWebTemplates.SelectedItem as WebTemplateInfo;
            //_model.Description = txtDescription.Text;
            //_model.GroupId = txtGroupId.Text;
            //_model.Id = txtId.Text;
            //_model.ImageUrl = txtImageUrl.Text;
            //_model.Location = txtLocation.Text;
            //if (!String.IsNullOrEmpty(txtSequence.Text))
            //{
            //    _model.Sequence = Convert.ToInt32(txtSequence.Text);
            //}
            //_model.Title = txtTitle.Text;
            _model.SaveChanges();
        }

        /// <summary>
        /// Onload to configure the form.
        /// </summary>
        /// <param name="e">The EventArgs object.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //Set the label text
            lblIntroduction.Text = CKSProperties.WebTemplatePage1_Introduction;
            lblRequired.Text = CKSProperties.WebTemplatePage1_Required;
            lnkMSDNArticle.Text = CKSProperties.WebTemplatePage1_MSDNLinkText;

            LinkLabel.Link link = new LinkLabel.Link();
            link.Description = CKSProperties.WebTemplatePage1_MSDNLinkText;
            link.LinkData = CKSProperties.WebTemplatePage1_MSDNLinkUrl;
            link.Enabled = true;
            link.Name = "lnkMSDN";
            lnkMSDNArticle.Links.Add(link);

            lblTitle.Text = CKSProperties.WebTemplatePage1_Title;
            lblWebTemplate.Text = CKSProperties.WebTemplatePage1_WebTemplate;
        }

        /// <summary>
        /// Is the page valid?
        /// </summary>
        /// <returns>The result of the validation checks.</returns>
        protected override bool IsCompletelyValid()
        {
            return base.IsCompletelyValid()
                    && ValidateTitle()
                    && ValidateSelectedWebTemplate();
        }

        /// <summary>
        /// Update the model on deactivate.
        /// </summary>
        /// <returns>The result.</returns>
        public override bool OnDeactivate()
        {
            this.ValidateChildren();
            if (!base.Wizard.MovingPrevious)
            {
                return base.OnDeactivate();
            }
            if (!IsCompletelyValid())
            {
                base.Visited = false;
                base.Skippable = false;
            }
            return true;
        }

        /// <summary>
        /// Validate the Title.
        /// </summary>
        /// <returns>True if the Title is valid.</returns>
        protected virtual bool ValidateTitle()
        {
            return !String.IsNullOrEmpty(txtTitle.Text);
        }

        /// <summary>
        /// Validates the selected web template.
        /// </summary>
        /// <returns></returns>
        protected virtual bool ValidateSelectedWebTemplate()
        {
            return true;
        }

        /// <summary>
        /// Launch the MSDN link.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The LinkLabelLinkClickedEventArgs object.</param>
        private void lnkMSDNArticle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Display the appropriate link based on the value of the 
            // LinkData property of the Link object.
            string target = e.Link.LinkData as string;

            // If the value looks like a URL, navigate to it.
            if (target != null && target.StartsWith("http"))
            {
                System.Diagnostics.Process.Start(target);
            }
        }

        /// <summary>
        /// Validate the title textbox entry.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The EventArgs object.</param>
        private void txtTitle_Validated(object sender, EventArgs e)
        {
            if (ValidateTitle())
            {
                // Clear the error, if any, in the error provider.
                errMessages.SetError(txtTitle, "");
            }
            else
            {
                // Set the error if the name is not valid.
                errMessages.SetError(txtTitle, CKSProperties.WebTemplatePage1_Title_Error);
            }
        }

        #endregion

        #region Designer Generated

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.errMessages = new System.Windows.Forms.ErrorProvider(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblIntroduction = new System.Windows.Forms.Label();
            this.lnkMSDNArticle = new System.Windows.Forms.LinkLabel();
            this.lblRequired = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.lblWebTemplate = new System.Windows.Forms.Label();
            this.cboWebTemplates = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.errMessages)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // infoPanel
            // 
            this.infoPanel.BackColor = System.Drawing.SystemColors.Control;
            this.infoPanel.Dock = System.Windows.Forms.DockStyle.None;
            this.infoPanel.Location = new System.Drawing.Point(0, 180);
            this.infoPanel.Size = new System.Drawing.Size(505, 304);
            // 
            // errMessages
            // 
            this.errMessages.ContainerControl = this;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.Controls.Add(this.lblIntroduction, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.lnkMSDNArticle, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblRequired, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblTitle, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.txtTitle, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.lblWebTemplate, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.cboWebTemplates, 0, 9);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 26;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 4F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 4F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 4F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 4F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 4F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 4F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 4F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(508, 389);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // lblIntroduction
            // 
            this.lblIntroduction.AutoSize = true;
            this.lblIntroduction.Location = new System.Drawing.Point(3, 10);
            this.lblIntroduction.MaximumSize = new System.Drawing.Size(400, 0);
            this.lblIntroduction.Name = "lblIntroduction";
            this.lblIntroduction.Size = new System.Drawing.Size(63, 13);
            this.lblIntroduction.TabIndex = 24;
            this.lblIntroduction.Text = "Introduction";
            // 
            // lnkMSDNArticle
            // 
            this.lnkMSDNArticle.AutoSize = true;
            this.lnkMSDNArticle.Location = new System.Drawing.Point(358, 10);
            this.lnkMSDNArticle.Name = "lnkMSDNArticle";
            this.lnkMSDNArticle.Size = new System.Drawing.Size(39, 13);
            this.lnkMSDNArticle.TabIndex = 25;
            this.lnkMSDNArticle.TabStop = true;
            this.lnkMSDNArticle.Text = "MSDN";
            this.lnkMSDNArticle.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkMSDNArticle_LinkClicked);
            // 
            // lblRequired
            // 
            this.lblRequired.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblRequired.AutoSize = true;
            this.lblRequired.Location = new System.Drawing.Point(3, 27);
            this.lblRequired.MaximumSize = new System.Drawing.Size(400, 0);
            this.lblRequired.Name = "lblRequired";
            this.lblRequired.Size = new System.Drawing.Size(50, 13);
            this.lblRequired.TabIndex = 26;
            this.lblRequired.Text = "Required";
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblTitle, 2);
            this.lblTitle.Location = new System.Drawing.Point(3, 50);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(30, 13);
            this.lblTitle.TabIndex = 70;
            this.lblTitle.Text = "Title:";
            // 
            // txtTitle
            // 
            this.txtTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.txtTitle, 2);
            this.txtTitle.Location = new System.Drawing.Point(3, 66);
            this.txtTitle.Margin = new System.Windows.Forms.Padding(3, 3, 30, 3);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(475, 20);
            this.txtTitle.TabIndex = 0;
            this.txtTitle.Validated += new System.EventHandler(this.txtTitle_Validated);
            // 
            // lblWebTemplate
            // 
            this.lblWebTemplate.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblWebTemplate, 2);
            this.lblWebTemplate.Location = new System.Drawing.Point(3, 93);
            this.lblWebTemplate.Name = "lblWebTemplate";
            this.lblWebTemplate.Size = new System.Drawing.Size(80, 13);
            this.lblWebTemplate.TabIndex = 72;
            this.lblWebTemplate.Text = "Web Template:";
            // 
            // cboWebTemplates
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.cboWebTemplates, 2);
            this.cboWebTemplates.FormattingEnabled = true;
            this.cboWebTemplates.Location = new System.Drawing.Point(3, 109);
            this.cboWebTemplates.Name = "cboWebTemplates";
            this.cboWebTemplates.Size = new System.Drawing.Size(475, 21);
            this.cboWebTemplates.TabIndex = 83;
            // 
            // WebTemplatePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "WebTemplatePage";
            this.Size = new System.Drawing.Size(508, 487);
            this.Controls.SetChildIndex(this.infoPanel, 0);
            this.Controls.SetChildIndex(this.tableLayoutPanel1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.errMessages)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
