using System;
using System.ComponentModel;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards
{
    /// <summary>
    /// The interface to define the Source Url Source
    /// </summary>
    public interface ISourceUrlSource : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the source URL.
        /// </summary>
        /// <value>
        /// The source URL.
        /// </value>
        Uri SourceUrl
        {
            get;
        }
    }
}
