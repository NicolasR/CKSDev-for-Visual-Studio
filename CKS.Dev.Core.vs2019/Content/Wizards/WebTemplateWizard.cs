using CKS.Dev2015.VisualStudio.SharePoint.Commands;
using CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards.Models;
using CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards.Pages;
using CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards.WizardProperties;
using EnvDTE;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.TemplateWizard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CKSProperties = CKS.Dev2015.Core.Properties.Resources;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards
{
    class WebTemplateWizard : BaseWizard
    {
        #region Fields

        /// <summary>
        /// Field to hold the web template properties
        /// </summary>
        private WebTemplateProperties _webTemplateProperties;

        /// <summary>
        /// Field to hold the deployment properties
        /// </summary>
        private DeploymentProperties _deploymentProperties;

        /// <summary>
        /// Field to hold the project name
        /// </summary>
        private string _projectName;

        /// <summary>
        /// Field to hold the root name
        /// </summary>
        private string _rootName;

        string projectItemName;

        WelcomePageType welcomePageType;

        #endregion

        /// <summary>
        /// Gets the flag to indicate whether a SharePoint connection is required
        /// </summary>
        protected override bool IsSharePointConnectionRequired
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Set the project properties
        /// </summary>
        /// <param name="project">The project</param>
        public override void SetProjectProperties(EnvDTE.Project project)
        {
            ISharePointProject spProject = DTEManager.ProjectService.Convert<EnvDTE.Project, Microsoft.VisualStudio.SharePoint.ISharePointProject>(project);

            spProject.SiteUrl = _deploymentProperties.Url;
            spProject.IsSandboxedSolution = _deploymentProperties.IsSandboxedSolution;
            spProject.StartupItem = Enumerable.FirstOrDefault<ISharePointProjectItem>(ProjectUtilities.GetItemsOfType(spProject, ProjectItemIds.WebTemplate));
        }

        /// <summary>
        /// Create the wizard forms
        /// </summary>
        /// <param name="designTimeEnvironment">The design time environment</param>
        /// <param name="runKind">The wizard run kind</param>
        /// <returns>The IWizardFormExtension</returns>
        public override IWizardFormExtension CreateWizardForm(DTE designTimeEnvironment, WizardRunKind runKind)
        {
            //TODO: put this back
            _deploymentProperties = new DeploymentProperties();

            ArtifactWizardForm wiz = new ArtifactWizardForm(designTimeEnvironment, CKSProperties.WebTemplate_WizardTitle);

            if (runKind == WizardRunKind.AsNewProject)
            {
                _webTemplateProperties = new WebTemplateProperties(Guid.NewGuid(), _deploymentProperties);
                DeploymentPresentationModel model = new DeploymentPresentationModel(_deploymentProperties, false, IsSharePointConnectionRequired);
                WebTemplatePresentationModel model2 = new WebTemplatePresentationModel(_webTemplateProperties, false, designTimeEnvironment);
                DeploymentPage page = new DeploymentPage(wiz, model);
                WebTemplatePage page2 = new WebTemplatePage(wiz, model2);
                wiz.AddPage(page);
                wiz.AddPage(page2);
                return wiz;
            }

            ISharePointProject spProject = DTEManager.ActiveSharePointProject;
            _deploymentProperties.IsSandboxedSolution = spProject.IsSandboxedSolution;
            _deploymentProperties.Url = spProject.SiteUrl;
            _projectName = spProject.Name;
            _webTemplateProperties = new WebTemplateProperties(Guid.NewGuid(), _deploymentProperties);
            _webTemplateProperties.Title = _rootName;
            WebTemplatePresentationModel model3 = new WebTemplatePresentationModel(_webTemplateProperties, false, designTimeEnvironment);
            WebTemplatePage page5 = new WebTemplatePage(wiz, model3);
            wiz.AddPage(page5);
            return wiz;
        }

        /// <summary>
        /// Initialise from the the wizard data
        /// </summary>
        /// <param name="replacementsDictionary">The replacements dictionary</param>
        public override void InitializeFromWizardData(Dictionary<string, string> replacementsDictionary)
        {
            base.InitializeFromWizardData(replacementsDictionary);
            if (replacementsDictionary.ContainsKey("$rootname$"))
            {
                _rootName = replacementsDictionary["$rootname$"];
            }
            if (replacementsDictionary.ContainsKey("$projectname$"))
            {
                _projectName = replacementsDictionary["$projectname$"];
            }
        }

        /// <summary>
        /// Populate the replacement dictionary
        /// </summary>
        /// <param name="replacementsDictionary">The replacements dictionary</param>
        public override void PopulateReplacementDictionary(Dictionary<string, string> replacementsDictionary)
        {
            base.PopulateReplacementDictionary(replacementsDictionary);

            welcomePageType = WelcomePageType.WebPartPage;

            string selectedWebTemplateId = _webTemplateProperties.WebTemplateInfo.Name;
            string siteDefinitionName = selectedWebTemplateId.Substring(0, selectedWebTemplateId.IndexOf('#'));
            string siteDefinitionConfig = selectedWebTemplateId.Substring(selectedWebTemplateId.IndexOf('#') + 1);
            projectItemName = replacementsDictionary["$rootname$"];

            replacementsDictionary["$onet$"] = GetOnetContents(DTEManager.ProjectService,
                siteDefinitionName,
                siteDefinitionConfig,
                _webTemplateProperties.WebTemplateInfo.SetupPath,
                projectItemName,
                _webTemplateProperties.Title,
                replacementsDictionary,
                out welcomePageType);

            replacementsDictionary["$siteDefinitionName$"] = siteDefinitionName;
            replacementsDictionary["$siteDefinitionConfig$"] = siteDefinitionConfig;
            replacementsDictionary["$siteDefinitionId$"] = _webTemplateProperties.WebTemplateInfo.Id.ToString();
            replacementsDictionary["$siteDefinitionCategory$"] = _webTemplateProperties.WebTemplateInfo.DisplayCategory;
            replacementsDictionary["$webTempTitle$"] = _webTemplateProperties.Title;

        }

        public override void RunWizardFinished()
        {
            base.RunWizardFinished();

            // required to push changes from DTE.Project to ISharePointProject
            ISharePointProject spProject = DTEManager.ActiveSharePointProject;
            spProject.Synchronize();

            string projectPath = Path.GetDirectoryName(spProject.FullPath);

            ISharePointProjectItem webTemplate = DTEManager.FindSPIItemByName(projectItemName, spProject);
            ISharePointProjectItem webTemplateStamp = DTEManager.FindSPIItemByName(String.Format("{0}Stamp", projectItemName), spProject);
            ISharePointProjectItem publishingPage = DTEManager.FindSPIItemByName(String.Format("{0}PublishingPage", projectItemName), spProject);
            ISharePointProjectItem wikiPage = DTEManager.FindSPIItemByName(String.Format("{0}WikiPage", projectItemName), spProject);
            ISharePointProjectItem webPartPage = DTEManager.FindSPIItemByName(String.Format("{0}WebPartPage", projectItemName), spProject);

            ISharePointProjectFeature webTemplateFeature = spProject.Features[projectItemName];
            ISharePointProjectFeature webTemplateStampFeature = spProject.Features[String.Format("{0}Stamp", projectItemName)];
            ISharePointProjectFeature webTemplateWelcomePageFeature = spProject.Features[String.Format("{0}WelcomePage", projectItemName)];

            // remove automatically added feature associations
            foreach (ISharePointProjectFeature feature in spProject.Features)
            {
                feature.ProjectItems.Remove(webTemplate);
                feature.ProjectItems.Remove(webTemplateStamp);
                feature.ProjectItems.Remove(publishingPage);
                feature.ProjectItems.Remove(wikiPage);
                feature.ProjectItems.Remove(webPartPage);
            }

            // add standard spis to own features
            webTemplateFeature.ProjectItems.Add(webTemplate);
            webTemplateStampFeature.ProjectItems.Add(webTemplateStamp);
            webTemplateWelcomePageFeature.ProjectItems.Add(publishingPage);
            webTemplateWelcomePageFeature.ProjectItems.Add(wikiPage);
            webTemplateWelcomePageFeature.ProjectItems.Add(webPartPage);

            ProjectItem pagesFolder = DTEManager.FindItemByName(DTEManager.ActiveProject.ProjectItems, "Pages", true);
            ProjectItem defaultPageProjectItem = null;

            switch (welcomePageType)
            {
                case WelcomePageType.WebPartPage:
                    DTEManager.SafeDeleteProjectItem(DTEManager.FindItemByName(pagesFolder.ProjectItems, String.Format("{0}WikiPage", projectItemName), true));
                    DTEManager.SafeDeleteProjectItem(DTEManager.FindItemByName(pagesFolder.ProjectItems, String.Format("{0}PublishingPage", projectItemName), true));
                    defaultPageProjectItem = DTEManager.FindItemByName(pagesFolder.ProjectItems, String.Format("{0}WebPartPage", projectItemName), true);
                    break;
                case WelcomePageType.WikiPage:
                    DTEManager.SafeDeleteProjectItem(DTEManager.FindItemByName(pagesFolder.ProjectItems, String.Format("{0}WebPartPage", projectItemName), true));
                    DTEManager.SafeDeleteProjectItem(DTEManager.FindItemByName(pagesFolder.ProjectItems, String.Format("{0}PublishingPage", projectItemName), true));
                    defaultPageProjectItem = DTEManager.FindItemByName(pagesFolder.ProjectItems, String.Format("{0}WikiPage", projectItemName), true);
                    break;
                case WelcomePageType.PublishingPage:
                    DTEManager.SafeDeleteProjectItem(DTEManager.FindItemByName(pagesFolder.ProjectItems, String.Format("{0}WikiPage", projectItemName), true));
                    DTEManager.SafeDeleteProjectItem(DTEManager.FindItemByName(pagesFolder.ProjectItems, String.Format("{0}WebPartPage", projectItemName), true));
                    defaultPageProjectItem = DTEManager.FindItemByName(pagesFolder.ProjectItems, String.Format("{0}PublishingPage", projectItemName), true);
                    break;
            }

            if (defaultPageProjectItem != null)
            {
                defaultPageProjectItem.Name = String.Format("{0}Default", projectItemName);
            }
        }

        /// <summary>
        /// Gets the onet contents.
        /// </summary>
        /// <param name="projectService">The project service.</param>
        /// <param name="siteDefinitionName">Name of the site definition.</param>
        /// <param name="siteDefinitionConfig">The site definition config.</param>
        /// <param name="setupPath">The setup path.</param>
        /// <param name="webTemplateName">Name of the web template.</param>
        /// <param name="webTemplateTitle">The web template title.</param>
        /// <param name="replacements">The replacements.</param>
        /// <param name="welcomePageType">Type of the welcome page.</param>
        /// <returns></returns>
        private string GetOnetContents(ISharePointProjectService projectService,
            string siteDefinitionName,
            string siteDefinitionConfig,
            string setupPath,
            string webTemplateName,
            string webTemplateTitle,
            IDictionary<string, string> replacements,
            out WelcomePageType welcomePageType)
        {
            string onetContents = String.Empty;
            welcomePageType = WelcomePageType.WebPartPage;

            string templatesFolderPath = projectService.SharePointConnection.ExecuteCommand<string, string>(ObjectModelSharePointCommandIds.GetFullSPRootFolderPath, @"TEMPLATE");

            string siteDefinitionOnetPath = Path.Combine(templatesFolderPath, String.IsNullOrEmpty(setupPath) ? Path.Combine("SiteTemplates", siteDefinitionName) : setupPath, @"xml\onet.xml");

            XElement xProject = XElement.Parse(File.ReadAllText(siteDefinitionOnetPath));
            xProject.Descendants("ListTemplates").Remove();
            xProject.Descendants("DocumentTemplates").Remove();
            xProject.Descendants("ServerEmailFooter").Remove();
            xProject.Descendants("Configurations").Descendants("Configuration").Where(xConfig => xConfig.Attribute("ID").Value != siteDefinitionConfig).Remove();
            xProject.Descendants("Configurations").Descendants("Configuration").Descendants("Modules").Remove();
            xProject.Descendants("Modules").Remove();

            xProject.Attribute("Title").Value = webTemplateTitle;
            XElement xConfiguration = xProject.Descendants("Configurations").Descendants("Configuration").First();
            xConfiguration.Attribute("ID").Value = "0";
            xConfiguration.Attribute("Name").Value = webTemplateName;

            XElement xWebFeatures = xConfiguration.Descendants("WebFeatures").First();
            xWebFeatures.Add(
                new XComment(String.Format("{0} Welcome Page", webTemplateTitle)),
                new XElement("Feature",
                    new XAttribute("ID", replacements["$guid3$"])),
                new XComment(String.Format("{0} Stamp", webTemplateTitle)),
                new XElement("Feature",
                    new XAttribute("ID", replacements["$guid2$"])));

            onetContents = xProject.ToString();

            if (onetContents.IndexOf("00BFEA71-D8FE-4FEC-8DAD-01C19A6E4053", StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                welcomePageType = WelcomePageType.WikiPage;
            }
            else if (onetContents.IndexOf("94C94CA6-B32F-4da9-A9E3-1F3D343D7ECB", StringComparison.InvariantCultureIgnoreCase) > 0)
            {
                welcomePageType = WelcomePageType.PublishingPage;
            }

            return onetContents;
        }

    }
}

