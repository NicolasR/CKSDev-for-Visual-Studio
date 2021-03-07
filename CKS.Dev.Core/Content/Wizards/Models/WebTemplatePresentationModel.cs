using CKS.Dev2015.VisualStudio.SharePoint.Commands;
using CKS.Dev2015.VisualStudio.SharePoint.Commands.Info;
using CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards.WizardProperties;
using EnvDTE;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards.Models
{
    class WebTemplatePresentationModel : BasePresentationModel
    {
        #region Fields

        private WebTemplateProperties _webTemplateProperties;

        private DTE _designTimeEnvironment;

        private Uri _sourceurl;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the result of validating the Title
        /// </summary>
        public bool IsTitleValid
        {
            get { return ValidateTitle(); }
        }

        /// <summary>
        /// Gets or sets the selected web template.
        /// </summary>
        /// <value>The selected web template.</value>
        public WebTemplateInfo SelectedWebTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is selected web template valid.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is selected web template valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelectedWebTemplateValid
        {
            get { return ValidateSelectedWebTemplate(); }
        }

        /// <summary>
        /// Gets the available web templates.
        /// </summary>
        /// <value>The available web templates.</value>
        public WebTemplateInfo[] AvailableWebTemplates
        {
            get
            {
                return GetAvailableWebTemplates();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="WebTemplatePresentationModel"/> class.
        /// </summary>
        /// <param name="webTemplateProperties">The web template CKSProperties.</param>
        /// <param name="isOptional">if set to <c>true</c> [is optional].</param>
        /// <param name="designTimeEnvironment">The design time environment.</param>
        public WebTemplatePresentationModel(WebTemplateProperties webTemplateProperties,
            bool isOptional,
            DTE designTimeEnvironment)
            : base(isOptional)
        {
            if (webTemplateProperties == null)
            {
                throw new ArgumentNullException("webTemplateProperties");
            }
            _webTemplateProperties = webTemplateProperties;
            _designTimeEnvironment = designTimeEnvironment;
            _sourceurl = _webTemplateProperties.SourceUrl;
            _webTemplateProperties.PropertyChanged += new PropertyChangedEventHandler(_webTemplateProperties_PropertyChanged);
        }

        /// <summary>
        /// Handles the PropertyChanged event of the _webTemplateProperties control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        void _webTemplateProperties_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if ((e.PropertyName == "SourceUrl") && !this._sourceurl.Equals(((WebTemplateProperties)sender).SourceUrl))
            {
                this._sourceurl = ((WebTemplateProperties)sender).SourceUrl;
            }
        }

        /// <summary>
        /// Gets the available web templates.
        /// </summary>
        /// <returns></returns>
        protected virtual WebTemplateInfo[] GetAvailableWebTemplates()
        {
            Microsoft.VisualStudio.OLE.Interop.IServiceProvider serviceProvider = _designTimeEnvironment as Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
            ISharePointProjectService projectService = new ServiceProvider(serviceProvider).GetService(typeof(ISharePointProjectService)) as ISharePointProjectService;

            return projectService.SharePointConnection.ExecuteCommand<WebTemplateInfo[]>(WebTemplateCollectionSharePointCommandIds.GetWebTemplates);
        }


        /// <summary>
        /// Save the changes to the properties
        /// </summary>
        public override void SaveChanges()
        {
            _webTemplateProperties.Title = Title;
            _webTemplateProperties.WebTemplateInfo = SelectedWebTemplate;
        }

        /// <summary>
        /// Validate the Title
        /// </summary>
        /// <returns>True if the Title is valid</returns>
        protected virtual bool ValidateTitle()
        {
            return !String.IsNullOrEmpty(Title);
        }

        /// <summary>
        /// Validate the Description
        /// </summary>
        /// <returns>True if the Description is valid</returns>
        protected virtual bool ValidateSelectedWebTemplate()
        {
            return true;
        }

        #endregion
    }
}
