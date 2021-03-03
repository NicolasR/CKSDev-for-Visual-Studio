using Microsoft.VisualStudio.SharePoint;
using System.ComponentModel;

namespace CKS.Dev2015.VisualStudio.SharePoint.Deployment.ProjectProperties
{
    /// <summary>
    /// Auto copy to SharePoint root property.
    /// </summary>
    public class AutoCopyToSharePointRootProperty : DeploymentPropertyBase
    {
        // Values for the project property.
        private const string customPropertyId = "CKSDEV.DeploymentExtensions.AutoCopyToSharePointRootProperty";
        private const bool customPropertyDefaultValue = false;
        private const string propertyDescription = "Specifies whether or not CKSDEV should automatically \"Copy to SharePoint Root\" whenever a deployable file is saved, for example an ASCX or ASPX file.";
        private const string propertyName = "Auto Copy to SharePoint Root";

        /// <summary>
        /// Create a new instance of the AutoCopyToSharePointRootProperty object.
        /// </summary>
        /// <param name="project">The SharePoint project.</param>
        public AutoCopyToSharePointRootProperty(ISharePointProject project)
            : base(project)
        {
        }

        /// <summary>
        /// Gets or sets the auto copy to SharePoint root flag.
        /// </summary>
        [DisplayName(propertyName)]
        [DescriptionAttribute(propertyDescription)]
        [DefaultValue(customPropertyDefaultValue)]
        [CategoryAttribute(propertyCategory)]
        public bool AutoCopyToSharePointRoot
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
        /// Get from project.
        /// </summary>
        /// <param name="project">The SharePoint project.</param>
        /// <returns>The result.</returns>
        public static bool GetFromProject(ISharePointProject project)
        {
            return new AutoCopyToSharePointRootProperty(project).AutoCopyToSharePointRoot;
        }
    }

}

