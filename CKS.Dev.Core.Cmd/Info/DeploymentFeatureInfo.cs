using System;

namespace CKS.Dev.VisualStudio.SharePoint.Commands.Info
{
    /// <summary>
    /// Contains basic data about a feature on the SharePoint site. This class is 
    /// serializable so that instances of it can be sent between the Visual Studio and 
    /// SharePoint command assemblies.
    /// </summary>
    [Serializable]
    public class DeploymentFeatureInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the feature Id.
        /// </summary>
        public Guid FeatureID { get; set; }

        /// <summary>
        /// Gets or sets the name of the feature.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the scope.
        /// </summary>
        public DeploymentFeatureScope Scope { get; set; }

        #endregion
    }
}
