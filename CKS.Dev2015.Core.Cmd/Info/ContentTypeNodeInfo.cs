using Microsoft.VisualStudio.SharePoint.Explorer.Extensions;
using System;

namespace CKS.Dev2015.VisualStudio.SharePoint.Commands.Info
{
    /// <summary>
    /// The content type node information.
    /// </summary>
    [Serializable]
    public class ContentTypeNodeInfo : IContentTypeNodeInfo
    {
        /// <summary>
        /// Gets the name of the content type.
        /// </summary>
        /// <value></value>
        /// <returns>The name of the content type.</returns>
        public string Name { get; set; }

    }
}
