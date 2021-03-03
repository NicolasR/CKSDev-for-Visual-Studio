using Microsoft.VisualStudio.SharePoint;
using System.ComponentModel;

namespace CKS.Dev2015.VisualStudio.SharePoint.Deployment.ProjectProperties
{
    /// <summary>
    /// The build on copy assemblies property.
    /// </summary>
    [DesignerCategory(propertyCategory)]
    public class BuildOnCopyAssembliesProperty : DeploymentPropertyBase
    {
        // Values for the project property.
        private const string customPropertyId = "CKSDEV.DeploymentExtensions.BuildOnCopyAssembliesProperty";
        private const bool customPropertyDefaultValue = false;
        private const string propertyDescription = "Specifies whether or not CKSDEV should build the project before copying assemblies with \"Copy to GAC/BIN\".";
        private const string propertyName = "Build on Copy to GAC/BIN";

        /// <summary>
        /// Create a new instance of the BuildOnCopyAssembliesProperty object.
        /// </summary>
        /// <param name="project">The SharePoint project.</param>
        public BuildOnCopyAssembliesProperty(ISharePointProject project)
            : base(project)
        {
        }

        /// <summary>
        /// Gets or sets the build on copy assemblies flag.
        /// </summary>
        [DisplayName(propertyName)]
        [DescriptionAttribute(propertyDescription)]
        [DefaultValue(customPropertyDefaultValue)]
        [CategoryAttribute(propertyCategory)]
        public bool BuildOnCopyAssemblies
        {
            get
            {
                return this.GetBoolValue(customPropertyId, customPropertyDefaultValue);
            }

            set
            {
                this.SetBoolValue(customPropertyId, value);
            }
        }

        /// <summary>
        /// Get from the project.
        /// </summary>
        /// <param name="project">The SharePoint project</param>
        /// <returns>The flag.</returns>
        public static bool GetFromProject(ISharePointProject project)
        {
            return new BuildOnCopyAssembliesProperty(project).BuildOnCopyAssemblies;
        }

    }

}
