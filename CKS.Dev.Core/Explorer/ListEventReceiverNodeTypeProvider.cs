using CKS.Dev.VisualStudio.SharePoint.Commands;
using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using Microsoft.VisualStudio.SharePoint.Explorer;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using CKSProperties = CKS.Dev.Core.Properties.Resources;

namespace CKS.Dev.VisualStudio.SharePoint.Explorer
{
    /// <summary>
    /// List event receiver extension.
    /// </summary>
    [Export(typeof(IExplorerNodeTypeProvider))]
    [ExplorerNodeType(ExplorerNodeIds.ListEventReceiverNode)]
    public class ListEventReceiverNodeTypeProvider : IExplorerNodeTypeProvider
    {
        #region Methods

        /// <summary>
        /// Initializes the new node type.
        /// </summary>
        /// <param name="typeDefinition">The definition of the new node type.</param>
        public void InitializeType(IExplorerNodeTypeDefinition typeDefinition)
        {
            typeDefinition.IsAlwaysLeaf = true;
            typeDefinition.DefaultIcon = CKSProperties.EventReceiver.ToBitmap();
            typeDefinition.NodePropertiesRequested += new EventHandler<ExplorerNodePropertiesRequestedEventArgs>(typeDefinition_NodePropertiesRequested);
        }

        /// <summary>
        /// Handles the NodePropertiesRequested event of the typeDefinition control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.Explorer.ExplorerNodePropertiesRequestedEventArgs"/> instance containing the event data.</param>
        void typeDefinition_NodePropertiesRequested(object sender, ExplorerNodePropertiesRequestedEventArgs e)
        {
            IExplorerNode listEventReceiverNode = e.Node;
            EventReceiverInfo eventReceiverInfo = listEventReceiverNode.Annotations.GetValue<EventReceiverInfo>();
            if (eventReceiverInfo != null)
            {
                Dictionary<string, string> listEventReceiverProperties = listEventReceiverNode.Context.SharePointConnection.ExecuteCommand<EventReceiverInfo, Dictionary<string, string>>(ListEventReceiversCommandIds.GetListEventReceiverProperties, eventReceiverInfo);
                object propertySource = listEventReceiverNode.Context.CreatePropertySourceObject(listEventReceiverProperties);
                e.PropertySources.Add(propertySource);
            }
        }

        #endregion
    }
}
