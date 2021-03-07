using System;

namespace CKS.Dev.VisualStudio.SharePoint.Commands.Info
{
    /// <summary>
    /// Contains basic data about a feature element on the SharePoint site. This class is 
    /// serializable so that instances of it can be sent between the Visual Studio and 
    /// SharePoint command assemblies.
    /// </summary>
    [Serializable]
    public class FeatureElementInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the feature Id.
        /// </summary>
        public Guid FeatureID { get; set; }

        /// <summary>
        /// Gets or sets the element Id.
        /// </summary>
        public string ElementID { get; set; }

        /// <summary>
        /// Gets or sets the UI version.
        /// </summary>
        public string UIVersion { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the element type.
        /// </summary>
        public string ElementType { get; set; }

        #endregion
    }
}
