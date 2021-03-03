namespace CKS.Dev2015.VisualStudio.SharePoint.Content.Wizards
{
    /// <summary>
    /// Interface to define the form
    /// </summary>
    public interface IWizardFormExtension
    {
        /// <summary>
        /// Gets a value indicating whether this <see cref="IWizardFormExtension" /> is cancelled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if cancelled; otherwise, <c>false</c>.
        /// </value>
        bool Cancelled
        {
            get;
        }

        /// <summary>
        /// Launches this instance.
        /// </summary>
        void Launch();
    }
}
