using System.Collections.Generic;
using System.IO;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards
{
    /// <summary>
    /// Helper to assist with strong key
    /// </summary>
    internal class StrongNameProjectManager
    {
        #region Fields

        StrongNameKey key;
        const string KEY_FILENAME = "key.snk";
        const string PUBLIC_KEY_TOKEN_REPLACEMENT_KEY = "$publickeytoken$";

        #endregion

        #region Methods

        /// <summary>
        /// Adds the key file to project.
        /// </summary>
        /// <param name="project">The project.</param>
        internal void AddKeyFileToProject(EnvDTE.Project project)
        {
            if (this.key != null)
            {
                string destinationFileName = Path.Combine(Path.GetDirectoryName(project.FullName), "key.snk");
                this.key.SaveTo(destinationFileName);
                project.ProjectItems.AddFromFile(destinationFileName);
                EnvDTE.Properties properties = project.Properties;
                properties.Item("SignAssembly").Value = true;
                properties.Item("AssemblyOriginatorKeyFile").Value = "key.snk";
            }
        }

        /// <summary>
        /// Adds the key to dictionary.
        /// </summary>
        /// <param name="replacementsDictionary">The replacements dictionary.</param>
        internal void AddKeyToDictionary(Dictionary<string, string> replacementsDictionary)
        {
            string publicKeyToken = this.key.GetPublicKeyToken();
            replacementsDictionary.Add("$publickeytoken$", publicKeyToken);
        }

        /// <summary>
        /// Generates the key.
        /// </summary>
        internal void GenerateKey()
        {
            this.key = StrongNameKey.CreateNewKeyPair();
        }

        /// <summary>
        /// Gets the public key.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        internal bool GetPublicKey(EnvDTE.Project project)
        {
            EnvDTE.Properties projectProps = project.Properties;
            if (!HasPublicKey(projectProps))
            {
                return false;
            }
            string str = (string)projectProps.Item("AssemblyOriginatorKeyFile").Value;
            if (str != null)
            {
                string fullPath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(project.FileName), str));
                this.key = StrongNameKey.Load(fullPath);
            }
            else
            {
                string keyContainer = (string)projectProps.Item("AssemblyKeyContainerName").Value;
                this.key = StrongNameKey.LoadContainer(keyContainer);
            }
            return true;
        }

        /// <summary>
        /// Determines whether [has public key] [the specified project props].
        /// </summary>
        /// <param name="projectProps">The project props.</param>
        /// <returns>
        ///   <c>true</c> if [has public key] [the specified project props]; otherwise, <c>false</c>.
        /// </returns>
        private static bool HasPublicKey(EnvDTE.Properties projectProps)
        {
            if (projectProps.Item("AssemblyOriginatorKeyFile").Value == null)
            {
                return (projectProps.Item("AssemblyKeyContainerName").Value != null);
            }
            return true;
        }

        #endregion
    }
}


