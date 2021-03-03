using CKS.Dev2015.VisualStudio.SharePoint.Environment.Options;
using Microsoft.VisualStudio.SharePoint;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace CKS.Dev2015.VisualStudio.SharePoint.Environment
{
    /// <summary>
    /// Cancel Adding SPIs extension
    /// </summary>
    [Export(typeof(ISharePointProjectExtension))]
    public class CancelAddingSPIProjectExtension
        : ISharePointProjectExtension
    {
        private Guid recentlyAddedItem = Guid.Empty;

        /// <summary>
        /// Initializes the SharePoint project extension.
        /// </summary>
        /// <param name="projectService">Instance of SharePoint project service.</param>
        public void Initialize(ISharePointProjectService projectService)
        {
            if (EnabledExtensionsOptionsPage.GetSetting<bool>(EnabledExtensionsOptions.CancelAddingSPIs, true))
            {
                foreach (ISharePointProjectItemType type in projectService.ProjectItemTypes.Values)
                {
                    type.ProjectItemAdded += new EventHandler<SharePointProjectItemEventArgs>(spiType_ProjectItemAdded);
                    type.ProjectItemInitialized += new EventHandler<SharePointProjectItemEventArgs>(spiType_ProjectItemInitialized);
                }
            }
        }

        /// <summary>
        /// Handles the ProjectItemAdded event of the spiType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SharePointProjectItemEventArgs" /> instance containing the event data.</param>
        private void spiType_ProjectItemAdded(object sender, SharePointProjectItemEventArgs e)
        {
            recentlyAddedItem = e.ProjectItem.Id;
        }

        /// <summary>
        /// Handles the ProjectItemInitialized event of the spiType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="SharePointProjectItemEventArgs" /> instance containing the event data.</param>
        private void spiType_ProjectItemInitialized(object sender, SharePointProjectItemEventArgs e)
        {
            if (recentlyAddedItem == e.ProjectItem.Id)
            {
                recentlyAddedItem = Guid.Empty;
                ISharePointProjectItem spi = e.ProjectItem;
                IEnumerable<ISharePointProjectFeature> source = from ISharePointProjectFeature feature
                                                                in spi.Project.Features
                                                                where feature.ProjectItems.Contains(spi)
                                                                select feature;

                if ((source != null) && (source.Count() > 0))
                {
                    foreach (ISharePointProjectFeature feature in source)
                    {
                        feature.ProjectItems.Remove(spi);
                    }
                }
            }
        }
    }
}