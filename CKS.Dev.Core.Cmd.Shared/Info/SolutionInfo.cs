using System;

namespace CKS.Dev.VisualStudio.SharePoint.Commands.Info
{
    /// <summary>
    /// Contains basic data about a solution on the SharePoint site. This class is 
    /// serializable so that instances of it can be sent between the Visual Studio and 
    /// SharePoint command assemblies.
    /// </summary>
    [Serializable]
    public class SolutionInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Is Sandboxed Solution flag.
        /// </summary>
        public bool IsSandboxedSolution { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the local path.
        /// </summary>
        public string LocalPath { get; set; }

        #endregion
    }
}
