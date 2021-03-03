using CKS.Dev2015.Core.VisualStudio.SharePoint.Environment.Dialogs;
using CKS.Dev2015.VisualStudio.SharePoint.Environment.Options;
using Microsoft.VisualStudio.SharePoint;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;

namespace CKS.Dev2015.VisualStudio.SharePoint.Environment
{
    /// <summary>
    /// Activate Selected Features Project Extension
    /// </summary>
    [Export(typeof(ISharePointProjectExtension))]
    public class ActivateSelectedFeaturesProjectExtension : ISharePointProjectExtension
    {
        /// <summary>
        /// CKSDEV_FeaturesSelectedForActivation
        /// </summary>
        public const string ProjectPropertyName = "CKSDEV_FeaturesSelectedForActivation";

        /// <summary>
        /// Initializes the SharePoint project extension.
        /// </summary>
        /// <param name="projectService">An instance of SharePoint project service.</param>
        public void Initialize(ISharePointProjectService projectService)
        {
            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.ActivateSelectedFeatures, true))
            {
                projectService.ProjectMenuItemsRequested += new EventHandler<SharePointProjectMenuItemsRequestedEventArgs>(projectService_ProjectMenuItemsRequested);
            }
        }

        /// <summary>
        /// Handles the ProjectMenuItemsRequested event of the projectService control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.SharePointProjectMenuItemsRequestedEventArgs"/> instance containing the event data.</param>
        void projectService_ProjectMenuItemsRequested(object sender,
            SharePointProjectMenuItemsRequestedEventArgs e)
        {
            IMenuItem configureFeaturesActivationMenuItem = e.ActionMenuItems.Add("Select Features to activate...");
            configureFeaturesActivationMenuItem.Click += new EventHandler<MenuItemEventArgs>(configureFeaturesActivationMenuItem_Click);
            configureFeaturesActivationMenuItem.IsEnabled = e.Project.Package.Model.Features.Count > 0;
        }

        /// <summary>
        /// Handles the Click event of the configureFeaturesActivationMenuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.MenuItemEventArgs"/> instance containing the event data.</param>
        void configureFeaturesActivationMenuItem_Click(object sender,
            MenuItemEventArgs e)
        {
            ISharePointProject project = e.Owner as ISharePointProject;

            if (project != null)
            {
                IEnumerable<ISharePointProjectFeature> featuresFromPackage = ProjectUtilities.GetFeaturesFromFeatureRefs(project, project.Package.Model.Features);

                IEnumerable<Guid> currentlySelectedFeaturesIds = null;

                List<string> selectedFeaturesIds = ProjectUtilities.GetValueFromCurrentProject(project, ProjectPropertyName);

                if (selectedFeaturesIds != null && selectedFeaturesIds.Count > 0)
                {
                    currentlySelectedFeaturesIds = from string featureId
                                                   in selectedFeaturesIds
                                                   select new Guid(featureId);
                }

                FeaturesPickerDialog picker = new FeaturesPickerDialog(featuresFromPackage, currentlySelectedFeaturesIds);

                if (picker.ShowDialog() == DialogResult.OK)
                {
                    IEnumerable<ISharePointProjectFeature> featuresToBeActivated = picker.SelectedFeatures;

                    if (featuresToBeActivated != null && featuresToBeActivated.Count() > 0)
                    {
                        selectedFeaturesIds = new List<string>(featuresToBeActivated.Count());

                        foreach (ISharePointProjectFeature selectedFeature in featuresToBeActivated)
                        {
                            selectedFeaturesIds.Add(selectedFeature.Id.ToString());
                        }

                        ProjectUtilities.StoreValueInCurrentProject(selectedFeaturesIds, project, ProjectPropertyName);
                    }
                }
            }
        }
    }
}
