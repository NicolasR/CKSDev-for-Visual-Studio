using Microsoft.VisualStudio.SharePoint;
using System;

namespace CKS.Dev2015.VisualStudio.SharePoint.Environment.Dialogs
{
    /// <summary>
    /// Helper class to store basic information about a Project List Item.
    /// </summary>
    public class SharePointProjectListItem
    {
        /// <summary>
        /// Gets the project.
        /// </summary>
        /// <value>
        /// The project.
        /// </value>
        public ISharePointProject Project { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharePointProjectListItem" /> class.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <exception cref="System.ArgumentNullException">project</exception>
        public SharePointProjectListItem(ISharePointProject project)
        {
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }

            Project = project;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return Project.Name;
        }
    }
}
