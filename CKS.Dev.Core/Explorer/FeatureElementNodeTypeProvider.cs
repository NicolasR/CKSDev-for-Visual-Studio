using CKS.Dev.VisualStudio.SharePoint.Commands;
using CKS.Dev.VisualStudio.SharePoint.Commands.Info;
using Microsoft.VisualStudio.SharePoint.Explorer;
using Microsoft.VisualStudio.SharePoint.Explorer.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace CKS.Dev.VisualStudio.SharePoint.Explorer
{
    /// <summary>
    /// The feature element group node type provider.
    /// </summary>
    // Enables Visual Studio to discover and load this extension.
    [Export(typeof(IExplorerNodeTypeProvider))]
    // Indicates that this class extends SharePoint nodes in Server Explorer.
    [ExplorerNodeType(ExplorerNodeIds.FeatureElementNode)]
    public class FeatureElementNodeTypeProvider : IExplorerNodeTypeProvider
    {
        #region Methods

        /// <summary>
        /// Creates the feature element nodes.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        internal static void CreateFeatureElementNodes(IExplorerNode parentNode)
        {
            IFeatureNodeInfo info = parentNode.ParentNode.Annotations.GetValue<IFeatureNodeInfo>();
            FeatureInfo featureDetails = new FeatureInfo()
            {
                FeatureID = info.Id
            };
            FeatureElementInfo[] elements =
                parentNode.Context.SharePointConnection.ExecuteCommand<FeatureInfo, FeatureElementInfo[]>(FeatureSharePointCommandIds.GetFeatureElements, featureDetails);
            foreach (FeatureElementInfo element in elements)
            {
                IExplorerNode elementNode = CreateNode(parentNode, element);
                elementNode.DoubleClick += ElementNode_DoubleClick;
            }
        }

        /// <summary>
        /// Handles the DoubleClick event of the ElementNode control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.VisualStudio.SharePoint.Explorer.ExplorerNodeEventArgs"/> instance containing the event data.</param>
        static void ElementNode_DoubleClick(object sender, ExplorerNodeEventArgs e)
        {
            FeatureElementInfo elementInfo = e.Node.Annotations.GetValue<FeatureElementInfo>();

            XDocument document = XDocument.Parse(e.Node.Context.SharePointConnection.ExecuteCommand<FeatureElementInfo, string>(FeatureSharePointCommandIds.GetElementDefinition, elementInfo));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.NewLineOnAttributes = true;
            settings.Indent = true;
            StringBuilder text = new StringBuilder();
            using (StringWriter stringWriter = new StringWriter(text))
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
            {
                document.WriteTo(xmlWriter);
            }
            e.Node.Context.ShowMessageBox(text.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Creates the node.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        internal static IExplorerNode CreateNode(IExplorerNode parentNode, FeatureElementInfo element)
        {
            Dictionary<object, object> annotations = new Dictionary<object, object>();
            annotations.Add(typeof(FeatureElementInfo), element);
            return parentNode.ChildNodes.Add(ExplorerNodeIds.FeatureElementNode,
                String.Format("{0} ({1})", element.Name, element.ElementType),
                annotations);
        }

        /// <summary>
        /// Initializes the new node type.
        /// </summary>
        /// <param name="typeDefinition">The definition of the new node type.</param>
        public void InitializeType(IExplorerNodeTypeDefinition typeDefinition)
        {
            typeDefinition.IsAlwaysLeaf = true;
        }

        #endregion
    }
}
