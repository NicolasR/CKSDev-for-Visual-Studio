using CKS.Dev.VisualStudio.SharePoint.Environment.Options;
using EnvDTE;
using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows.Forms;
using CKSProperties = CKS.Dev.Core.Properties.Resources;

namespace CKS.Dev.VisualStudio.SharePoint.Environment
{
    /// <summary>
    /// Project extension.
    /// </summary>
    [Export(typeof(ISharePointProjectExtension))]
    public class ProjectExtension : ISharePointProjectExtension
    {
        /// <summary>
        /// Initializes the SharePoint project extension.
        /// </summary>
        /// <param name="projectService">An instance of SharePoint project service.</param>
        public void Initialize(ISharePointProjectService projectService)
        {
            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.CopyAssemblyName, true))
            {
                projectService.ProjectMenuItemsRequested += new EventHandler<SharePointProjectMenuItemsRequestedEventArgs>(projectService_ProjectMenuItemsRequested);
            }
        }

        /// <summary>
        /// Handles the ProjectMenuItemsRequested event of the projectService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.SharePointProjectMenuItemsRequestedEventArgs"/> instance containing the event data.</param>
        void projectService_ProjectMenuItemsRequested(object sender, SharePointProjectMenuItemsRequestedEventArgs e)
        {
            //Some error in the VS code which leaves the first menu item as 'Menu Command'
            IMenuItem dummy = e.ActionMenuItems.Add("");
            dummy.IsEnabled = false;

            IMenuItem copyAssemblyNameItem = e.ActionMenuItems.Add(CKSProperties.ProjectExtension_CopyAssemblyName);
            copyAssemblyNameItem.Click += new EventHandler<MenuItemEventArgs>(CopyAssemblyNameItem_Click);
        }

        /// <summary>
        /// Handles the Click event of the copyAssemblyNameItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.MenuItemEventArgs"/> instance containing the event data.</param>
        void CopyAssemblyNameItem_Click(object sender, MenuItemEventArgs e)
        {
            System.Threading.Tasks.Task.Run(async () => await CopAssemblyNameAsync(e));
        }

        async System.Threading.Tasks.Task CopAssemblyNameAsync(MenuItemEventArgs e)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            ISharePointProject project = e.Owner as ISharePointProject;

            Project dteProject = DTEManager.ProjectService.Convert<ISharePointProject, Project>(project);

            if (dteProject.DTE.Solution.SolutionBuild.BuildState != vsBuildState.vsBuildStateInProgress)
            {
                dteProject.DTE.Solution.SolutionBuild.BuildProject(dteProject.DTE.Solution.SolutionBuild.ActiveConfiguration.Name, dteProject.UniqueName, true);
                Clipboard.SetText(ProjectUtilities.GetAssemblyName(project));
            }
        }
    }
}
