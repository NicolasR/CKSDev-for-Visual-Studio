using CKS.Dev2015.VisualStudio.SharePoint.Commands.Info;
using System;
using System.ComponentModel;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards.WizardProperties
{
    class WebTemplateProperties
    {
        #region Fields

        private Uri _sourceUrl;
        private readonly string _uniqueId;

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the web template info.
        /// </summary>
        /// <value>The web template info.</value>
        public WebTemplateInfo WebTemplateInfo { get; set; }

        /// <summary>
        /// SourceUrl
        /// </summary>
        public Uri SourceUrl
        {
            get
            {
                return _sourceUrl;
            }
            private set
            {
                _sourceUrl = value;
                PropertyChangedEventHandler propertyChanged = PropertyChanged;
                if (propertyChanged != null)
                {
                    propertyChanged(this, new PropertyChangedEventArgs("SourceUrl"));
                }
            }
        }

        #endregion

        #region Methods

        public WebTemplateProperties()
        {

        }

        public WebTemplateProperties(Guid uniqueId, ISourceUrlSource urlSource)
        {
            _uniqueId = ConvertToId(uniqueId);
            _sourceUrl = urlSource.SourceUrl;
            urlSource.PropertyChanged += new PropertyChangedEventHandler(source_PropertyChanged);
        }

        private static string ConvertToId(Guid value)
        {
            return value.ToString().Replace("-", "");
        }

        private void source_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SourceUrl")
            {
                SourceUrl = ((ISourceUrlSource)sender).SourceUrl;
            }
        }

        #endregion
    }
}

