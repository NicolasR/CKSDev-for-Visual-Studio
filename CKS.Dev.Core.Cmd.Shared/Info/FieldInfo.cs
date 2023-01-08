using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

    namespace CKS.Dev.VisualStudio.SharePoint.Commands.Info
{
    /// <summary>The field info
    /// 
    /// </summary>
    class FieldInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the template.
        /// </summary>
        /// <value>
        /// The template.
        /// </value>
        public string Template { get; set; }

        /// <summary>
        /// Gets or sets the tag.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public TagInfo Tag { get; set; }

        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        /// <value>
        /// The field.
        /// </value>
        public SPField Field { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldInfo" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="template">The template.</param>
        /// <param name="tag">The tag.</param>
        /// <param name="field">The field.</param>
        public FieldInfo(string name, string title, string description, string template, TagInfo tag, SPField field)
        {
            Name = name;
            Title = title;
            Description = description;
            Template = template;
            Tag = tag;
            Field = field;
        }

        #endregion
    }
}