using System;

namespace CKS.Dev.VisualStudio.SharePoint.Commands.Info
{
    /// <summary>
    /// Contains basic data about a web template on the SharePoint site. This class is 
    /// serializable so that instances of it can be sent between the Visual Studio and 
    /// SharePoint command assemblies.
    /// </summary>
    [Serializable]
    public class WebTemplateInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display category.
        /// </summary>
        public string DisplayCategory { get; set; }

        /// <summary>
        /// Gets or sets the is custom template flag.
        /// </summary>
        public bool IsCustomTemplate { get; set; }

        /// <summary>
        /// Gets or sets the is hidden flag.
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Gets or sets the is root web only flag.
        /// </summary>
        public bool IsRootWebOnly { get; set; }

        /// <summary>
        /// Gets or sets the is sub web web only flag.
        /// </summary>
        public bool IsSubWebOnly { get; set; }

        /// <summary>
        /// Gets or sets the Lcid.
        /// </summary>
        public uint Lcid { get; set; }

        /// <summary>
        /// Gets or sets the image url.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the setup path.
        /// </summary>
        /// <value>The setup path.</value>
        public string SetupPath { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0} ({1})", Title, Name);
        }

        #endregion
    }
}

