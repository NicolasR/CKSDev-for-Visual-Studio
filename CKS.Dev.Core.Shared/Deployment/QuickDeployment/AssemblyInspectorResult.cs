using System;
using System.Collections.Generic;

namespace CKS.Dev.VisualStudio.SharePoint.Deployment.QuickDeployment
{
    /// <summary>
    /// A result container to allow multiple things to cross the marshaling boundary of the reflection.
    /// </summary>
    [Serializable]
    public class AssemblyInspectorResult
    {
        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        /// <value>The messages.</value>
        public List<String> Messages { get; set; }

        /// <summary>
        /// Gets or sets the tokens.
        /// </summary>
        /// <value>The tokens.</value>
        public Dictionary<string, string> Tokens { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyInspectorResult"/> class.
        /// </summary>
        public AssemblyInspectorResult()
        {
            Tokens = new Dictionary<string, string>();
            Messages = new List<string>();
        }

    }
}
