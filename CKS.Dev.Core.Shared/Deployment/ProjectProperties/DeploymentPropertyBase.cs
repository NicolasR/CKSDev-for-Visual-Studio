using Microsoft.VisualStudio.SharePoint;

namespace CKS.Dev.VisualStudio.SharePoint.Deployment.ProjectProperties
{
    /// <summary>
    /// Deployment property helper.
    /// </summary>
    public abstract class DeploymentPropertyBase
    {
        /// <summary>
        /// The category.
        /// </summary>
        protected const string propertyCategory = "Quick Deploy (CKSDEV)";

        /// <summary>
        /// The sharepoint project.
        /// </summary>
        protected ISharePointProject sharePointProject = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentPropertyBase"/> class.
        /// </summary>
        /// <param name="myProject">My project.</param>
        protected DeploymentPropertyBase(ISharePointProject myProject)
        {
            sharePointProject = myProject;
        }

        /// <summary>
        /// Gets the string value.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns></returns>
        internal string GetStringValue(string id, string defaultValue)
        {
            string propvalue = null;

            // Try to get the value from the .user file. The value of the property is 
            // stored in the variable "propvalue".
            if (!sharePointProject.ProjectUserFileData.TryGetValue(id, out propvalue))
            {
                // If we cannot get the value, then return the default value.
                propvalue = defaultValue;
            }

            return propvalue;
        }

        /// <summary>
        /// Gets the bool value.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="defaultValue">if set to <c>true</c> [default value].</param>
        /// <returns></returns>
        internal bool GetBoolValue(string id, bool defaultValue)
        {
            return bool.Parse(GetStringValue(id, defaultValue.ToString()));
        }

        /// <summary>
        /// Sets the string value.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="value">The value.</param>
        internal void SetStringValue(string id, string value)
        {
            // If a key is not already in the .user file, add one.
            if (!sharePointProject.ProjectUserFileData.ContainsKey(id))
            {
                // Add a new key/value pair to the .user file.
                sharePointProject.ProjectUserFileData.Add(id, value.ToString());
            }
            else
            {
                // Set the property value.
                sharePointProject.ProjectUserFileData[id] = value.ToString();
            }
        }

        /// <summary>
        /// Sets the bool value.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        internal void SetBoolValue(string id, bool value)
        {
            SetStringValue(id, value.ToString());
        }
    }
}
