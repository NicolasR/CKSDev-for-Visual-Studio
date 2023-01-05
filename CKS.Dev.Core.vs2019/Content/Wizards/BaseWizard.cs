using EnvDTE;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards
{
    /// <summary>
    /// Base wizard class mirrors the code from MS
    /// </summary>
    abstract class BaseWizard : IWizard
    {
        #region Fields

        /// <summary>
        /// Field to back hold a ICertificateGenerator
        /// </summary>
        private ICertificateGenerator _certificateGenerator;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the design time environment
        /// </summary>
        internal DTE DesignTimeEnvironment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the is solution closing flag
        /// </summary>
        public bool IsSolutionClosing
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a flag indicating whether a SharePoint connection is required
        /// </summary>
        protected abstract bool IsSharePointConnectionRequired { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Before opening file
        /// </summary>
        /// <param name="projectItem">The project item</param>
        public void BeforeOpeningFile(ProjectItem projectItem)
        {
        }

        /// <summary>
        /// Project finished generating
        /// </summary>
        /// <param name="project">The project item</param>
        public void ProjectFinishedGenerating(EnvDTE.Project project)
        {
            try
            {
                RunProjectFinishedGenerating(project);
            }
            catch (Exception exception)
            {
                DTEManager.ProjectService.Logger.ActivateOutputWindow();
                DTEManager.ProjectService.Logger.WriteLine(exception.Message, LogCategory.Error);
                throw;
            }
        }

        /// <summary>
        /// Run the project finished generating
        /// </summary>
        /// <param name="project">The project</param>
        public virtual void RunProjectFinishedGenerating(EnvDTE.Project project)
        {
            SetProjectProperties(project);
            //_certificateGenerator.AddKeyFile(project);
        }

        /// <summary>
        /// Project item finsihed generating
        /// </summary>
        /// <param name="projectItem">The project item</param>
        public void ProjectItemFinishedGenerating(EnvDTE.ProjectItem projectItem)
        {
            try
            {
                RunProjectItemFinishedGenerating(projectItem);
            }
            catch (Exception exception)
            {
                DTEManager.ProjectService.Logger.ActivateOutputWindow();
                DTEManager.ProjectService.Logger.WriteLine(exception.Message, LogCategory.Error);
                throw;
            }
        }

        /// <summary>
        /// Run project item finished generating
        /// </summary>
        /// <param name="projectItem">The project item</param>
        public virtual void RunProjectItemFinishedGenerating(ProjectItem projectItem)
        {
        }

        /// <summary>
        /// Run finished
        /// </summary>
        public void RunFinished()
        {
            try
            {
                RunWizardFinished();
            }
            catch (Exception exception)
            {
                DTEManager.ProjectService.Logger.ActivateOutputWindow();
                DTEManager.ProjectService.Logger.WriteLine(exception.Message, LogCategory.Error);
                throw;
            }
        }

        /// <summary>
        /// Run wizard finsihed
        /// </summary>
        public virtual void RunWizardFinished()
        {

        }

        /// <summary>
        /// Run started
        /// </summary>
        /// <param name="automationObject">The automation oject</param>
        /// <param name="replacementsDictionary">The replacements dictionary</param>
        /// <param name="runKind">The wizard run kind</param>
        /// <param name="customParams">The custom parameter</param>
        public void RunStarted(object automationObject, Dictionary<string, string> replacementsDictionary, WizardRunKind runKind, object[] customParams)
        {
            DesignTimeEnvironment = automationObject as DTE;

            if (DesignTimeEnvironment == null)
            {
                throw new ArgumentNullException("automationObject");
            }

            if ((runKind == WizardRunKind.AsNewItem) && IsSharePointConnectionRequired)
            {
                ConnectItemWizardToSharePoint();
            }

            SetIsSolutionClosing(runKind, replacementsDictionary);
            InitializeFromWizardData(replacementsDictionary);
            DisplayWizard(automationObject, runKind);
            PopulateReplacementDictionary(replacementsDictionary);
            if (runKind == WizardRunKind.AsNewProject)
            {
                _certificateGenerator.GenerateKeyFile(replacementsDictionary);
            }
        }

        /// <summary>
        /// Populate the replacement dictionary
        /// </summary>
        /// <param name="replacementsDictionary">The replacements dictionary</param>
        public virtual void PopulateReplacementDictionary(Dictionary<string, string> replacementsDictionary)
        {
        }

        /// <summary>
        /// Display the wizard form
        /// </summary>
        /// <param name="automationObject">The automation object</param>
        /// <param name="runKind">The wizard run kind</param>
        /// <returns>The </returns>
        private IWizardFormExtension DisplayWizard(object automationObject, WizardRunKind runKind)
        {
            IWizardFormExtension extension = CreateWizardForm((DTE)automationObject, runKind);
            if (extension != null)
            {
                extension.Launch();
                if (extension.Cancelled)
                {
                    throw new WizardCancelledException();
                }
            }
            return extension;
        }

        /// <summary>
        /// Intialise the form wizard data
        /// </summary>
        /// <param name="replacementsDictionary">The replacements dictionary</param>
        public virtual void InitializeFromWizardData(Dictionary<string, string> replacementsDictionary)
        {
        }

        /// <summary>
        /// Should add project item
        /// </summary>
        /// <param name="filePath">The file path</param>
        /// <returns>Returns true</returns>
        public virtual bool ShouldAddProjectItem(string filePath)
        {
            return true;
        }

        /// <summary>
        /// Set the is solution closing flag
        /// </summary>
        /// <param name="runKind">The wizard run kind</param>
        /// <param name="replacementsDictionary">The replacement dictionary</param>
        private void SetIsSolutionClosing(WizardRunKind runKind, Dictionary<string, string> replacementsDictionary)
        {
            IsSolutionClosing = (runKind == WizardRunKind.AsNewProject) && bool.Parse(replacementsDictionary["$exclusiveproject$"]);
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Set the project properties
        /// </summary>
        /// <param name="project">The project</param>
        public abstract void SetProjectProperties(EnvDTE.Project project);

        /// <summary>
        /// Create the wizard form
        /// </summary>
        /// <param name="designTimeEnvironment">The design time environment</param>
        /// <param name="runKind">The wizard run kind</param>
        /// <returns>The IWizardFormExtension</returns>
        public abstract IWizardFormExtension CreateWizardForm(DTE designTimeEnvironment, WizardRunKind runKind);

        #endregion

        #region Static Methods

        /// <summary>
        /// Connect the item wizard to SharePoint
        /// </summary>
        public static void ConnectItemWizardToSharePoint()
        {
            Uri defaultProjectUrl = WizardHelpers.GetDefaultProjectUrl();
            WizardHelpers.CheckMissingSiteUrl(defaultProjectUrl);
            //try
            //{
            //    SharePointWeb.ValidateSiteWhileShowingProgress(defaultProjectUrl);
            //}
            //catch (Exception exception)
            //{
            //    string sharePointConnectionErrorMessage = SharePointWeb.GetSharePointConnectionErrorMessage(exception, defaultProjectUrl.AbsoluteUri);
            //    //RtlAwareMessageBox.ShowError(null, sharePointConnectionErrorMessage, WizardResources.SharePointConnectionErrorCaption);
            //    //WizardHelpers.Logger.LogException(exception);
            //    throw new WizardCancelledException();
            //}
        }
        #endregion
    }
}

