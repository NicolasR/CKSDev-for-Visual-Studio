using Microsoft.VisualStudio.SharePoint;
using Microsoft.VisualStudio.SharePoint.Explorer;
using System.ComponentModel.Composition;
using System.Windows.Forms;
using CKSProperties = CKS.Dev.Core.Properties.Resources;
namespace CKS.Dev.VisualStudio.SharePoint.Explorer
{
    /// <summary>
    /// Represents an extension of SharePoint site nodes in Server Explorer for content types. 
    /// </summary>
    // Enables Visual Studio to discover and load this extension.
    [Export(typeof(IExplorerNodeTypeExtension))]
    // Indicates that this class extends SharePoint site nodes in Server Explorer.
    [ExplorerNodeType(ExplorerNodeTypes.GenericFolderNode)]
    internal class ContentTypeFolderNodeExtension : IExplorerNodeTypeExtension
    {
        #region Methods

        /// <summary>
        /// Initialise the node.
        /// </summary>
        /// <param name="nodeType">The node type.</param>
        public void Initialize(IExplorerNodeType nodeType)
        {
            nodeType.NodeMenuItemsRequested += ContentTypes_NodeMenuItemsRequested;
        }

        /// <summary>
        /// Register the menu items for the content types.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The ExplorerNodeMenuItemsRequestedEventArgs object.</param>
        void ContentTypes_NodeMenuItemsRequested(object sender, ExplorerNodeMenuItemsRequestedEventArgs e)
        {
            //Check this is the content types node
            if (e.Node.Text == CKSProperties.ContentTypesSiteNodeExtension_ContentTypesNode)
            {
                //Register the view in browser menu item
                e.MenuItems.Add(CKSProperties.ContentTypeFolderNodeExtension_ImportAllCustom).Click += ContentTypesGenericFolderNodeExtension_Click;
            }
        }

        /// <summary>
        /// Import all the custom content types.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">The MenuItemEventArgs object.</param>
        void ContentTypesGenericFolderNodeExtension_Click(object sender, MenuItemEventArgs e)
        {
            IExplorerNode owner = (IExplorerNode)e.Owner;

            if (MessageBox.Show(CKSProperties.ContentTypeFolderExtension_ImportAllConfirmationQuestion,
                    CKSProperties.ContentTypeFolderExtension_ImportAllConfirmationDialogTitle,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                ImportContentTypes(owner);
            }
        }

        private void ImportContentTypes(IExplorerNode owner)
        {
            //TODO: this import all needs to check each ct for OOTB and import it if not
        }

        #endregion
    }
}