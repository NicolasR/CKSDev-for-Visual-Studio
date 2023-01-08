using System;

namespace CKS.Dev.VisualStudio.SharePoint.Commands.Info
{
    /// <summary>
    /// Contains information about an SPBasePermission. This class is 
    /// serializable so that instances of it can be sent between the Visual Studio and 
    /// SharePoint command assemblies.
    /// </summary>
    [Serializable]
    public class SPBasePermissionInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        #endregion
    }
}
