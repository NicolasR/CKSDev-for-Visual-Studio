using Microsoft.VisualStudio.SharePoint;
using System;

namespace CKS.Dev.VisualStudio.SharePoint.Environment.Dialogs
{
    /// <summary>
    /// Helper class to store basic information about a Project Feature List Item.
    /// </summary>
    public class SharePointProjectFeatureListItem
    {
        /// <summary>
        /// Gets the feature.
        /// </summary>
        /// <value>
        /// The feature.
        /// </value>
        public ISharePointProjectFeature Feature { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePointProjectFeatureListItem" /> class.
        /// </summary>
        /// <param name="feature">The feature.</param>
        /// <exception cref="System.ArgumentNullException">feature</exception>
        public SharePointProjectFeatureListItem(ISharePointProjectFeature feature)
        {
            if (feature == null)
            {
                throw new ArgumentNullException("feature");
            }

            Feature = feature;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return String.Format("{0} ({1}; {2})", Feature.Model.Title, Feature.Model.Scope, Feature.Project.Name);
        }
    }
}