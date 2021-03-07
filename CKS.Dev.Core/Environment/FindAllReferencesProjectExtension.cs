using CKS.Dev.Core.VisualStudio.SharePoint.Environment.Dialogs;
using CKS.Dev.VisualStudio.SharePoint.Environment.Options;
using Microsoft.VisualStudio.SharePoint;
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;

namespace CKS.Dev.VisualStudio.SharePoint.Environment
{
    /// <summary>
    /// Find All References Project Extension
    /// </summary>
    [Export(typeof(ISharePointProjectExtension))]
    public class FindAllReferencesProjectExtension : ISharePointProjectExtension
    {
        /// <summary>
        /// Initializes the SharePoint project extension.
        /// </summary>
        /// <param name="projectService">An instance of SharePoint project service.</param>
        public void Initialize(ISharePointProjectService projectService)
        {
            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.SPIReferences, true))
            {
                foreach (ISharePointProjectItemType spit in projectService.ProjectItemTypes.Values)
                {
                    spit.ProjectItemMenuItemsRequested += new EventHandler<SharePointProjectItemMenuItemsRequestedEventArgs>(spit_ProjectItemMenuItemsRequested);
                }
            }
        }

        /// <summary>
        /// Handles the ProjectItemMenuItemsRequested event of the spit control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.SharePointProjectItemMenuItemsRequestedEventArgs"/> instance containing the event data.</param>
        void spit_ProjectItemMenuItemsRequested(object sender,
            SharePointProjectItemMenuItemsRequestedEventArgs e)
        {
            IMenuItem findAllReferencesMenuItem = e.ViewMenuItems.Add("Find all references");
            findAllReferencesMenuItem.Click += new EventHandler<MenuItemEventArgs>(findAllReferencesMenuItem_Click);

            IMenuItem removeFromAllReferencesMenuItem = e.ViewMenuItems.Add("Remove from all Features and Packages...");
            removeFromAllReferencesMenuItem.Click += new EventHandler<MenuItemEventArgs>(removeFromAllReferencesMenuItem_Click);

            if (CanBeDeployedUsingFeature(e.ProjectItem.ProjectItemType.SupportedDeploymentScopes))
            {
                IMenuItem addToSpecificFeature = e.AddMenuItems.Add("Add to specific Feature...");
                addToSpecificFeature.Click += new EventHandler<MenuItemEventArgs>(addToSpecificFeature_Click);
            }

            if (SupportsDeploymentScope(e.ProjectItem.ProjectItemType.SupportedDeploymentScopes, SupportedDeploymentScopes.Package))
            {
                IMenuItem addToSpecificPackage = e.AddMenuItems.Add("Add to specific Package...");
                addToSpecificPackage.Click += new EventHandler<MenuItemEventArgs>(addToSpecificPackage_Click);
            }
        }

        /// <summary>
        /// Handles the Click event of the addToSpecificPackage control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.MenuItemEventArgs"/> instance containing the event data.</param>
        void addToSpecificPackage_Click(object sender, MenuItemEventArgs e)
        {
            ISharePointProjectItem spi = e.Owner as ISharePointProjectItem;

            if (spi != null)
            {
                using (PackagesViewerForm form = new PackagesViewerForm(spi.Project))
                {
                    if (form.ShowDialog() == DialogResult.OK && form.SelectedPackage != null)
                    {
                        form.SelectedPackage.ProjectItems.Add(spi);
                        ISharePointProjectLogger logger = spi.Project.ProjectService.Logger;
                        logger.ClearErrorList();
                        logger.ActivateOutputWindow();
                        logger.WriteLine(String.Format("SharePoint Project Item '{0}' successfully added to Package '{1}'", spi.Name, form.SelectedPackage.Model.Name), LogCategory.Message, form.SelectedPackage.PackageFile.FullPath, 1, 1);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the addToSpecificFeature control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.MenuItemEventArgs"/> instance containing the event data.</param>
        void addToSpecificFeature_Click(object sender, MenuItemEventArgs e)
        {
            ISharePointProjectItem spi = e.Owner as ISharePointProjectItem;

            if (spi != null)
            {
                using (FeaturesViewerForm form = new FeaturesViewerForm(spi))
                {
                    if (form.ShowDialog() == DialogResult.OK && form.SelectedFeature != null)
                    {
                        form.SelectedFeature.ProjectItems.Add(spi);
                        ISharePointProjectLogger logger = spi.Project.ProjectService.Logger;
                        logger.ClearErrorList();
                        logger.ActivateOutputWindow();
                        logger.WriteLine(String.Format("SharePoint Project Item '{0}' successfully added to Feature '{1}'", spi.Name, form.SelectedFeature.Model.Title), LogCategory.Message, form.SelectedFeature.FullPath, 1, 1);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the removeFromAllReferencesMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.MenuItemEventArgs"/> instance containing the event data.</param>
        void removeFromAllReferencesMenuItem_Click(object sender,
            MenuItemEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to remove this project item from all Features and Packages?", "Remove from all Features and Packages", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.OK)
            {

                ISharePointProjectItem spi = e.Owner as ISharePointProjectItem;

                if (spi != null)
                {
                    ISharePointProjectLogger logger = spi.Project.ProjectService.Logger;
                    logger.ClearErrorList();
                    logger.ActivateOutputWindow();
                    logger.WriteLine(String.Format("Removing SharePoint Project Item '{0}' from Packages and Features...", spi.Name), LogCategory.Status);

                    var projects = spi.Project.ProjectService.Projects;

                    foreach (var project in projects)
                    {
                        ISharePointProjectItem spiInPackage = null;
                        foreach (ISharePointProjectItem s in project.Package.ProjectItems)
                        {
                            if (s.Id == spi.Id)
                            {
                                spiInPackage = s;
                                break;
                            }
                        }

                        if (spiInPackage != null)
                        {
                            project.Package.ProjectItems.Remove(spiInPackage);
                            logger.WriteLine(String.Format("SharePoint Project Item '{0}' removed from Package '{1}'", spi.Name, project.Package.Model.Name), LogCategory.Message, project.Package.PackageFile.FullPath, 1, 1);
                        }

                        foreach (var feature in project.Features)
                        {
                            ISharePointProjectItem spiInFeature = null;
                            foreach (var s in feature.ProjectItems)
                            {
                                if (s.Id == spi.Id)
                                {
                                    spiInFeature = s;
                                    break;
                                }
                            }

                            if (spiInFeature != null)
                            {
                                feature.ProjectItems.Remove(spiInFeature);
                                logger.WriteLine(String.Format("SharePoint Project Item '{0}' removed from Feature '{1}'", spi.Name, feature.Model.Title), LogCategory.Message, feature.FeatureFile.FullPath, 1, 1);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Handles the Click event of the findAllReferencesMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.MenuItemEventArgs"/> instance containing the event data.</param>
        void findAllReferencesMenuItem_Click(object sender,
            MenuItemEventArgs e)
        {
            ISharePointProjectItem spi = e.Owner as ISharePointProjectItem;
            if (spi != null)
            {
                ISharePointProjectLogger logger = spi.Project.ProjectService.Logger;
                logger.ClearErrorList();
                logger.ActivateOutputWindow();
                logger.WriteLine(String.Format("Searching for references to SharePoint Project Item '{0}'...", spi.Name), LogCategory.Status);

                bool referencesFound = false;

                foreach (var project in spi.Project.ProjectService.Projects)
                {
                    if (SupportsDeploymentScope(spi.ProjectItemType.SupportedDeploymentScopes, SupportedDeploymentScopes.Package))
                    {
                        if ((from ISharePointProjectItem s
                             in project.Package.ProjectItems
                             where s.Id == spi.Id
                             select s).Any())
                        {
                            referencesFound = true;
                            spi.Project.ProjectService.Logger.WriteLine(String.Format("Found reference to SharePoint Project Item '{0}' in Package '{1}' ({2})", spi.Name, project.Package.Model.Name, project.Name), LogCategory.Message, project.Package.PackageFile.FullPath, 1, 1);
                        }
                    }

                    if (CanBeDeployedUsingFeature(spi.ProjectItemType.SupportedDeploymentScopes))
                    {
                        var spiInFeatures = from ISharePointProjectFeature f
                                            in project.Features
                                            where (from ISharePointProjectItem s
                                                   in f.ProjectItems
                                                   where s.Id == spi.Id
                                                   select s).Any()
                                            select f;

                        foreach (var feature in spiInFeatures)
                        {
                            referencesFound = true;
                            spi.Project.ProjectService.Logger.WriteLine(String.Format("Found reference to SharePoint Project Item '{0}' in Feature '{1}' ({2})", spi.Name, feature.Model.Title, feature.Project.Name), LogCategory.Message, feature.FeatureFile.FullPath, 1, 1);
                        }
                    }
                }

                if (!referencesFound)
                {
                    logger.WriteLine(String.Format("No references to SharePoint Project Item '{0}' found", spi.Name), LogCategory.Status);
                }
            }
        }

        /// <summary>
        /// Supportses the deployment scope.
        /// </summary>
        /// <param name="supportedScopes">The supported scopes.</param>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
        private static bool SupportsDeploymentScope(SupportedDeploymentScopes supportedScopes,
            SupportedDeploymentScopes scope)
        {
            return (supportedScopes & scope) == scope;
        }

        /// <summary>
        /// Determines whether this instance [can be deployed using feature] the specified supported scopes.
        /// </summary>
        /// <param name="supportedScopes">The supported scopes.</param>
        /// <returns>
        /// 	<c>true</c> if this instance [can be deployed using feature] the specified supported scopes; otherwise, <c>false</c>.
        /// </returns>
        private static bool CanBeDeployedUsingFeature(SupportedDeploymentScopes supportedScopes)
        {
            return (supportedScopes & SupportedDeploymentScopes.Farm) == SupportedDeploymentScopes.Farm ||
                (supportedScopes & SupportedDeploymentScopes.Site) == SupportedDeploymentScopes.Site ||
                (supportedScopes & SupportedDeploymentScopes.Web) == SupportedDeploymentScopes.Web ||
                (supportedScopes & SupportedDeploymentScopes.WebApplication) == SupportedDeploymentScopes.WebApplication;
        }
    }
}
