using Microsoft.VisualStudio.SharePoint;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content
{
    /// <summary>
    /// FullTrustProxy Properties
    /// </summary>
    class FullTrustProxyProperties
    {
        #region Constants

        const string ExtensionDataKey = "CKS.Dev2015.VisualStudio.SharePoint.Project.FullTrustProxyProperties";

        #endregion

        #region Fields

        ObservableCollection<FullTrustProxyOperationDefinition> _operations;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the proxy.
        /// </summary>
        /// <value>The proxy.</value>
        public FullTrustProxyTypeProvider Proxy { get; private set; }

        /// <summary>
        /// Gets or sets the project item.
        /// </summary>
        /// <value>The project item.</value>
        public ISharePointProjectItem ProjectItem { get; private set; }

        /// <summary>
        /// Gets the operations.
        /// </summary>
        /// <value>The operations.</value>
        public ObservableCollection<FullTrustProxyOperationDefinition> Operations
        {
            get
            {
                if (_operations == null)
                {
                    _operations = new ObservableCollection<FullTrustProxyOperationDefinition>();
                    if (ProjectItem.ExtensionData.ContainsKey(ExtensionDataKey))
                    {
                        LoadCollection(_operations, ProjectItem.ExtensionData[ExtensionDataKey]);
                    }
                    StartMonitoring(_operations);
                }
                return _operations;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="FullTrustProxyProperties"/> class.
        /// </summary>
        /// <param name="proxy">The proxy.</param>
        /// <param name="projectItem">The project item.</param>
        public FullTrustProxyProperties(FullTrustProxyTypeProvider proxy, ISharePointProjectItem projectItem)
        {
            Proxy = proxy;
            ProjectItem = projectItem;
        }

        /// <summary>
        /// Loads the collection.
        /// </summary>
        /// <param name="operations">The operations.</param>
        /// <param name="data">The data.</param>
        void LoadCollection(ObservableCollection<FullTrustProxyOperationDefinition> operations, string data)
        {
            if (String.IsNullOrEmpty(data) == false)
            {
                string[] items = data.Split('|');
                foreach (string item in items)
                {
                    operations.Add(new FullTrustProxyOperationDefinition { FullClassName = item });
                }
            }
        }

        /// <summary>
        /// Saves the operations.
        /// </summary>
        /// <param name="operations">The operations.</param>
        void SaveOperations(ObservableCollection<FullTrustProxyOperationDefinition> operations)
        {
            StringBuilder builder = new StringBuilder();
            if (operations.Count > 0)
            {
                builder.Append(operations[0].FullClassName);
                for (int i = 1; i < operations.Count; i++)
                {
                    builder.Append("|");
                    builder.Append(operations[i].FullClassName);
                }
            }
            ProjectItem.ExtensionData[ExtensionDataKey] = builder.ToString();
        }

        /// <summary>
        /// Starts the monitoring.
        /// </summary>
        /// <param name="operations">The operations.</param>
        void StartMonitoring(ObservableCollection<FullTrustProxyOperationDefinition> operations)
        {
            operations.CollectionChanged +=
                delegate (object sender, NotifyCollectionChangedEventArgs e)
                {
                    SaveOperations((ObservableCollection<FullTrustProxyOperationDefinition>)sender);
                };
        }

        #endregion
    }
}
