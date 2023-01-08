using System;

namespace CKS.Dev.VisualStudio.SharePoint.Commands.Info
{
    /// <summary>
    /// Contains basic data the developer dashboard seeting on the SharePoint site. This class is 
    /// serializable so that instances of it can be sent between the Visual Studio and 
    /// SharePoint command assemblies.
    /// </summary>
    [Serializable]
    public class DeveloperDashboardNodeInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the SPDeveloperDashboardLevel.
        /// </summary>
        public string SPDeveloperDashboardLevel { get; set; }

        #endregion
    }
}
