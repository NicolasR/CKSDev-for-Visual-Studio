using Microsoft.VisualStudio.SharePoint.Explorer.Extensions;
using System;

namespace CKS.Dev.VisualStudio.SharePoint.Commands.Info
{
    /// <summary>
    /// The field node info.
    /// </summary>
    [Serializable]
    public class FieldNodeInfo : IFieldNodeInfo
    {
        /// <summary>
        /// Gets or sets the name of the content type.
        /// </summary>
        /// <value>The name of the content type.</value>
        public string ContentTypeName { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hidden.
        /// </summary>
        /// <value><c>true</c> if this instance is hidden; otherwise, <c>false</c>.</value>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets or sets the list id.
        /// </summary>
        /// <value>The list id.</value>
        public Guid ListId { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

    }
}
