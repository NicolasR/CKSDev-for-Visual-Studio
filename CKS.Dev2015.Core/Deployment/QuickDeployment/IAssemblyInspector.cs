namespace CKS.Dev2015.VisualStudio.SharePoint.Deployment.QuickDeployment
{
    /// <summary>
    /// The AssemblyInspector interface.
    /// </summary>
    public interface IAssemblyInspector
    {
        /// <summary>
        /// Gets the replaceable GUID tokens.
        /// </summary>
        /// <param name="assemblyPath">The assembly path.</param>
        /// <returns>The AssemblyInspectorResult</returns>
        AssemblyInspectorResult GetReplaceableGuidTokens(string assemblyPath);
    }
}

