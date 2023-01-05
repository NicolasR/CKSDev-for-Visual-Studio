using Microsoft.VisualStudio.SharePoint;
using System.ComponentModel;

namespace CKS.Dev.VisualStudio.SharePoint.Deployment.ProjectProperties
{
    /// <summary>
    /// Auto copy assemblies property.
    /// </summary>
    [DesignerCategory(propertyCategory)]
    public class AutoCopyAssembliesProperty : DeploymentPropertyBase
    {
        // Values for the project property.
        private const string customPropertyId = "CKSDEV.DeploymentExtensions.AutoCopyAssembliesProperty";
        private const bool customPropertyDefaultValue = false;
        private const string propertyDescription = "Specifies whether or not CKSDEV should automatically \"Copy to GAC/BIN\" whenever a SharePoint project is built.";
        private const string propertyName = "Auto Copy to GAC/BIN";

        /// <summary>
        /// Create a new instance of the AutoCopyAssembliesProperty object.
        /// </summary>
        /// <param name="project">The SharePoint project.</param>
        public AutoCopyAssembliesProperty(ISharePointProject project)
            : base(project)
        {
        }

        /// <summary>
        /// Gets or sets the auto copy assemblies flag.
        /// </summary>
        [DisplayName(propertyName)]
        [DescriptionAttribute(propertyDescription)]
        [DefaultValue(customPropertyDefaultValue)]
        [CategoryAttribute(propertyCategory)]
        public bool AutoCopyAssemblies
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
        /// <param name="project">The SharePoint project.</param>
        /// <returns>The flag value.</returns>
        public static bool GetFromProject(ISharePointProject project)
        {
            return new AutoCopyAssembliesProperty(project).AutoCopyAssemblies;
        }
    }

}