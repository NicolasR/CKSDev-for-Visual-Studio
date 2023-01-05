using System;

namespace CKS.Dev.VisualStudio.SharePoint.Commands.Info
{
    /// <summary>
    /// Contains basic data about a feature dependancy on the SharePoint site. This class is 
    /// serializable so that instances of it can be sent between the Visual Studio and 
    /// SharePoint command assemblies.
    /// </summary>
    [Serializable]
    public class FeatureDependencyInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the minimum version.
        /// </summary>
        public string MinimumVersion { get; set; }

        /// <summary>
        /// Gets or sets the feature Id.
        /// </summary>
        public Guid FeatureID { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        public FeatureScope Scope { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        #endregion
    }
}
