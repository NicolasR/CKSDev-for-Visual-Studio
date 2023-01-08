using CKS.Dev.VisualStudio.SharePoint.Deployment.ProjectProperties;
using Microsoft.VisualStudio.SharePoint;
using System;
using System.ComponentModel.Composition;


namespace CKS.Dev.VisualStudio.SharePoint.Deployment
{
    /// <summary>
    /// Extends the SharePoint projects with menu items.
    /// </summary>
    // Export attribute: Enables Visual Studio to discover and load this extension.
    [Export(typeof(ISharePointProjectExtension))]
    public class DeploymentProjectExtension : ISharePointProjectExtension
    {
        #region Methods

        /// <summary>
        /// Implements ISharePointProjectService.Initialize, which determines the behavior of the new property.
        /// </summary>
        /// <param name="projectService"></param>
        public void Initialize(ISharePointProjectService projectService)
        {
            // Handle events for when a project property is changed.
            projectService.ProjectPropertiesRequested += new EventHandler<SharePointProjectPropertiesRequestedEventArgs>(projectService_ProjectPropertiesRequested);
        }

        /// <summary>
        /// Handles the ProjectPropertiesRequested event of the projectService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SharePointProjectPropertiesRequestedEventArgs" /> instance containing the event data.</param>
        void projectService_ProjectPropertiesRequested(object sender, SharePointProjectPropertiesRequestedEventArgs e)
        {
            if (!e.Project.IsSandboxedSolution)
            {
                // Add new properties to the SharePoint project.
                e.PropertySources.Add((object)new AutoCopyToSharePointRootProperty(e.Project));
                e.PropertySources.Add((object)new AutoCopyAssembliesProperty(e.Project));
                e.PropertySources.Add((object)new BuildOnCopyAssembliesProperty(e.Project));
            }
        }

        #endregion
    }
}
