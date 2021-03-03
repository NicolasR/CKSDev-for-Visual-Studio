using CKS.Dev2015.VisualStudio.SharePoint.Environment.Dialogs;
using Microsoft.VisualStudio.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CKS.Dev2015.Core.VisualStudio.SharePoint.Environment.Dialogs
{
    /// <summary>
    /// The feature picker.
    /// </summary>
    public partial class FeaturesPickerDialog : Form
    {
        /// <summary>
        /// Gets the selected features.
        /// </summary>
        /// <value>The selected features.</value>
        public IEnumerable<ISharePointProjectFeature> SelectedFeatures
        {
            get
            {
                List<ISharePointProjectFeature> selectedFeatures = new List<ISharePointProjectFeature>();

                foreach (object item in featuresPicker.SelectedItems)
                {
                    if (item is SharePointProjectFeatureListItem)
                    {
                        selectedFeatures.Add(((SharePointProjectFeatureListItem)item).Feature);
                    }
                }

                return selectedFeatures;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeaturesPickerDialog"/> class.
        /// </summary>
        /// <param name="featuresFromPackage">The features from package.</param>
        /// <param name="selectedFeaturesIds">The selected features ids.</param>
        public FeaturesPickerDialog(IEnumerable<ISharePointProjectFeature> featuresFromPackage,
            IEnumerable<Guid> selectedFeaturesIds)
        {
            InitializeComponent();

            List<SharePointProjectFeatureListItem> features = new List<SharePointProjectFeatureListItem>(featuresFromPackage.Count());
            foreach (ISharePointProjectFeature f in featuresFromPackage)
            {
                features.Add(new SharePointProjectFeatureListItem(f));
            }

            FillFeaturesPicker(features, selectedFeaturesIds);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeaturesPickerDialog"/> class.
        /// </summary>
        /// <param name="featuresFromPackage">The features from package.</param>
        /// <param name="selectedFeaturesIds">The selected features ids.</param>
        public FeaturesPickerDialog(IEnumerable<SharePointProjectFeatureListItem> featuresFromPackage,
            IEnumerable<Guid> selectedFeaturesIds)
        {
            InitializeComponent();

            FillFeaturesPicker(featuresFromPackage, selectedFeaturesIds);
        }

        /// <summary>
        /// Fills the features picker.
        /// </summary>
        /// <param name="featuresFromPackage">The features from package.</param>
        /// <param name="selectedFeaturesIds">The selected features ids.</param>
        private void FillFeaturesPicker(IEnumerable<SharePointProjectFeatureListItem> featuresFromPackage,
            IEnumerable<Guid> selectedFeaturesIds)
        {
            List<SharePointProjectFeatureListItem> availableFeatures = new List<SharePointProjectFeatureListItem>(featuresFromPackage);

            if (selectedFeaturesIds != null)
            {
                List<SharePointProjectFeatureListItem> selectedFeatures = new List<SharePointProjectFeatureListItem>(selectedFeaturesIds.Count());

                foreach (Guid featureId in selectedFeaturesIds)
                {
                    SharePointProjectFeatureListItem feature = (from SharePointProjectFeatureListItem f
                                                        in availableFeatures
                                                                where f.Feature.Id.Equals(featureId)
                                                                select f).FirstOrDefault();

                    if (feature != null)
                    {
                        selectedFeatures.Add(feature);
                        availableFeatures.Remove(feature);
                    }
                }

                featuresPicker.SelectedItems = selectedFeatures;
            }

            featuresPicker.AvailableItems = availableFeatures;
            featuresPicker.SelectedItemsLabel = "Features to activate:";
            featuresPicker.AvailableItemsLabel = "Features in the package:";
        }
    }
}
