using System;

namespace CKS.Dev.VisualStudio.SharePoint.Commands.Info
{
    /// <summary>
    /// Contains basic data about a single content type on the SharePoint site. This class is 
    /// serializable so that instances of it can be sent between the Visual Studio and 
    /// SharePoint command assemblies.
    /// </summary>
    [Serializable]
    public class ContentTypeInfo
    {
        #region Properties

        /// <summary>
        /// Gets or set the  content type ID of the content type.
        /// </summary>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content type name.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content type group to which the content type is assigned. 
        /// Content type groups are user-defined groups that help you organize your columns into logical categories.
        /// </summary>
        public string Group
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the read only value, TRUE to define the content type as read-only. 
        /// If you do not include this attribute, SharePoint Foundation treats 
        /// the content type as if this attributes was set to FALSE.
        /// </summary>
        public bool? ReadOnly
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the hidden value, TRUE to define the content type as hidden. 
        /// If you define a content type as hidden, SharePoint Foundation does not display 
        /// that content type on the New button in list views. 
        /// If you do not include this attribute, SharePoint Foundation treats the content 
        /// type as if this attributes was set to FALSE.
        /// </summary>
        public bool? Hidden
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the description for the content type, to display in the SharePoint Foundation user interface.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sealed value, TRUE to define the content type as sealed. You must have site collection 
        /// administrator rights to unseal and edit a content type defined as sealed. 
        /// If you do not include this attribute, SharePoint Foundation treats the content type as if this attributes
        /// was set to FALSE.
        /// </summary>
        public bool? Sealed
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the inherits.
        /// </summary>
        /// <value>The inherits.</value>
        public bool? Inherits
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the document template.
        /// </summary>
        /// <value>The document template.</value>
        public string DocumentTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the field refs.
        /// </summary>
        public FieldRef[] FieldRefs
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the XML documents.
        /// </summary>
        /// <value>The XML documents.</value>
        public string[] XmlDocuments
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentTypeInfo"/> class.
        /// </summary>
        public ContentTypeInfo()
        {
            Inherits = true;
        }

        #endregion

        /// <summary>
        /// Contains basic data about a field ref on the SharePoint site. This class is 
        /// serializable so that instances of it can be sent between the Visual Studio and 
        /// SharePoint command assemblies.
        /// </summary>
        [Serializable]
        public class FieldRef
        {
            #region Properties

            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            /// <value>The id.</value>
            public string Id
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>The name.</value>
            public string Name
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the display name.
            /// </summary>
            /// <value>The display name.</value>
            public string DisplayName
            {
                get;
                set;
            }

            #endregion
        }
    }
}
