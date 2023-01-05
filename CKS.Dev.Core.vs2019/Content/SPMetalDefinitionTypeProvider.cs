using EnvDTE;
using Microsoft.VisualStudio.SharePoint;
using System;
using System.ComponentModel.Composition;
using VSLangProj;
using CKSProperties = CKS.Dev2015.Core.Properties.Resources;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content
{
    /// <summary>
    /// SPMetalDefinition item provider.
    /// </summary>
    // Enables Visual Studio to discover and load this extension.
    [Export(typeof(ISharePointProjectItemTypeProvider))]
    // Specifies the ID for this new project item type. This string must match the value of the 
    // Type attribute of the ProjectItem element in the .spdata file for the project item.
    [SharePointProjectItemType(ProjectItemIds.SPMetalDefinition)]
    // Specifies the icon to display with this project item in Solution Explorer.
    [SharePointProjectItemIcon("CKS.Dev11.VisualStudio.SharePoint.Resources.SolutionExplorerIcons.SPMetalDefinition_SolutionExplorer.ico")]
    class SPMetalDefinitionTypeProvider : ISharePointProjectItemTypeProvider
    {
        IServiceProvider _serviceProvider;

        /// <summary>
        /// Called by projects to initialize an instance of a SharePoint project item type.
        /// </summary>
        /// <param name="typeDefinition">A project item type definition to initialize.</param>
        public void InitializeType(ISharePointProjectItemTypeDefinition typeDefinition)
        {
            _serviceProvider = typeDefinition.ProjectService.ServiceProvider;
            typeDefinition.Name = CKSProperties.SPMetalDefinitionTypeProvider_TypeDefinitionName;
            typeDefinition.SupportedDeploymentScopes = SupportedDeploymentScopes.Package;
            typeDefinition.SupportedTrustLevels = SupportedTrustLevels.All;
            typeDefinition.ProjectItemPropertiesRequested += TypeDefinition_ProjectItemPropertiesRequested;
        }

        /// <summary>
        /// Forces the regenerate the SPMetal designer file.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="properties">The properties for the SPMetal definition.</param>
        internal void ForceRegenerate(ISharePointProjectItem owner, SPMetalDefinitionProperties properties)
        {
            owner.Annotations.Remove<SPMetalDefinitionProperties>();
            owner.Annotations.Add<SPMetalDefinitionProperties>(properties);

            ProjectItem dteItem = owner.Project.ProjectService.Convert<ISharePointProjectItemFile, ProjectItem>(owner.DefaultFile);
            VSProjectItem vp = (VSProjectItem)dteItem.Object;

            vp.RunCustomTool();
        }

        /// <summary>
        /// Handles the ProjectItemPropertiesRequested event of the TypeDefinition control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.SharePointProjectItemPropertiesRequestedEventArgs"/> instance containing the event data.</param>
        void TypeDefinition_ProjectItemPropertiesRequested(object sender, SharePointProjectItemPropertiesRequestedEventArgs e)
        {
            SPMetalDefinitionProperties properties = e.ProjectItem.Annotations.GetValue<SPMetalDefinitionProperties>();
            if (properties == null)
            {
                properties = new SPMetalDefinitionProperties(this, e.ProjectItem, e.ProjectItem.ExtensionData);
                e.ProjectItem.Annotations.Add<SPMetalDefinitionProperties>(properties);
            }
            e.PropertySources.Add(properties);
        }
    }
}
