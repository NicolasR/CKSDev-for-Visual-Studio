using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CKS.Dev2015.VisualStudio.SharePoint.Deployment.ProjectProperties
{
    /// <summary>
    /// Project CKSProperties.
    /// </summary>
    public class ProjectProperties
    {
        const string PropertyScriptName = "CKS.Dev.SharePoint.DevTools.ProjectExtension.Script";
        const string PropertySiteDefinition = "CKS.Dev.SharePoint.DevTools.ProjectExtension.SiteDefinition";
        const string PropertyWarmUpSiteAsync = "CKS.Dev.SharePoint.DevTools.ProjectExtension.PropertyWarmUpSiteAsync";
        IDictionary<string, string> _properties;

        /// <summary>
        /// Gets or sets the script name.
        /// </summary>
        [DefaultValue((string)null)]
        [Category(PropertyCategory.DevTools)]
        public string ScriptName
        {
            get
            {
                string script = null;
                _properties.TryGetValue(PropertyScriptName, out script);
                return script;
            }
            set { _properties[PropertyScriptName] = value; }
        }

        /// <summary>
        /// Gets or sets the site definition.
        /// </summary>
        [DefaultValue((string)null)]
        [Category(PropertyCategory.DevTools)]
        public string SiteDefinition
        {
            get
            {
                string siteDefinition = null;
                _properties.TryGetValue(PropertySiteDefinition, out siteDefinition);
                return siteDefinition;
            }
            set { _properties[PropertySiteDefinition] = value; }
        }

        /// <summary>
        /// Gets or sets the warm up site async flag.
        /// </summary>
        [DefaultValue(true)]
        [Category(PropertyCategory.DevTools)]
        public bool WarmUpSiteAsync
        {
            get
            {
                string warmUpSite;
                _properties.TryGetValue(PropertyWarmUpSiteAsync, out warmUpSite);
                return warmUpSite != null ? Boolean.Parse(warmUpSite) : true;
            }
            set { _properties[PropertyWarmUpSiteAsync] = value.ToString(); }
        }

        /// <summary>
        /// Create an instance of the ProjectProperties object.
        /// </summary>
        /// <param name="properties">The CKSProperties.</param>
        public ProjectProperties(IDictionary<string, string> properties)
        {
            _properties = properties;
        }
    }
}
