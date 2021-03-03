using System;

namespace CKS.Dev2015.VisualStudio.SharePoint.Commands.Info
{
    /// <summary>
    /// The tag info
    /// </summary>
    class TagInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name of the assembly.
        /// </summary>
        /// <value>
        /// The name of the assembly.
        /// </value>
        public string AssemblyName { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the namespace.
        /// </summary>
        /// <value>
        /// The name of the namespace.
        /// </value>
        public string NamespaceName { get; set; }

        /// <summary>
        /// Gets the name of the tag.
        /// </summary>
        /// <value>
        /// The name of the tag.
        /// </value>
        public string TagName
        {
            get
            {
                string tagName = Name;
                if (ContentTypeSharePointCommands.CustomTagPrefixMappings.ContainsKey(NamespaceName))
                {
                    tagName = ContentTypeSharePointCommands.CustomTagPrefixMappings[NamespaceName];
                }
                else if (ContentTypeSharePointCommands.TagPrefixMappings.ContainsKey(NamespaceName))
                {
                    tagName = ContentTypeSharePointCommands.TagPrefixMappings[NamespaceName];
                }

                return tagName;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="TagInfo" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <param name="namespaceName">Name of the namespace.</param>
        public TagInfo(string name, string assemblyName, string namespaceName)
        {
            Name = name;
            AssemblyName = assemblyName;
            NamespaceName = namespaceName;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("<%@ Register Tagprefix=\"{0}\" Namespace=\"{1}\" Assembly=\"{2}\"  %>", TagName, NamespaceName, AssemblyName);
        }

        #endregion
    }
}