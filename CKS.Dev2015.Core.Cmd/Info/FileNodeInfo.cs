using System;

namespace CKS.Dev2015.VisualStudio.SharePoint.Commands.Info
{
    /// <summary>
    /// Contains basic data about a file on the SharePoint site. This class is 
    /// serializable so that instances of it can be sent between the Visual Studio and 
    /// SharePoint command assemblies.
    /// </summary>
    [Serializable]
    public class FileNodeInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique Id.
        /// </summary>
        public Guid UniqueId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the file type.
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// Gets or sets the server relative url.
        /// </summary>
        public string ServerRelativeUrl { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the is checked out flag.
        /// </summary>
        public bool IsCheckedOut { get; set; }

        /// <summary>
        /// Gets or sets the file contents which is used to save the contents to SharePoint.
        /// </summary>
        public string Contents { get; set; }

        #endregion
    }
}
