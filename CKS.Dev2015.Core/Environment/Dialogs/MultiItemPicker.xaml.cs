using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CKS.Dev2015.Core.VisualStudio.SharePoint.Environment.Dialogs
{
    /// <summary>
    /// Interaction logic for MultiItemPicker.xaml
    /// </summary>
    public partial class MultiItemPicker : UserControl
    {
        /// <summary>
        /// Gets or sets the available items.
        /// </summary>
        /// <value>The available items.</value>
        public IEnumerable<object> AvailableItems
        {
            get
            {
                List<object> items = new List<object>(availableItems.Items.Count);
                foreach (object item in availableItems.Items)
                {
                    items.Add(item);
                }

                return items;
            }
            set
            {
                availableItems.Items.Clear();

                if (value != null)
                {
                    foreach (object item in value)
                    {
                        availableItems.Items.Add(item);
                    }
                }

                ToggleControls();
            }
        }

        /// <summary>
        /// Gets or sets the selected items.
        /// </summary>
        /// <value>The selected items.</value>
        public IEnumerable<object> SelectedItems
        {
            get
            {
                List<object> items = new List<object>(selectedItems.Items.Count);
                foreach (object item in selectedItems.Items)
                {
                    items.Add(item);
                }

                return items;
            }
            set
            {
                selectedItems.Items.Clear();

                if (value != null)
                {
                    foreach (object item in value)
                    {
                        selectedItems.Items.Add(item);
                    }
                }

                ToggleControls();
            }
        }

        /// <summary>
        /// Gets or sets the available items label.
        /// </summary>
        /// <value>The available items label.</value>
        public string AvailableItemsLabel
        {
            get
            {
                return availableItemsLabel.Content as string;
            }
            set
            {
                availableItemsLabel.Content = value;
            }
        }

        /// <summary>
        /// Gets or sets the selected items label.
        /// </summary>
        /// <value>The selected items label.</value>
        public string SelectedItemsLabel
        {
            get
            {
                return selectedItemsLabel.Content as string;
            }
            set
            {
                selectedItemsLabel.Content = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiItemPicker"/> class.
        /// </summary>
        public MultiItemPicker()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the availableItems control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void availableItems_SelectionChanged(object sender,
            SelectionChangedEventArgs e)
        {
            ToggleControls();
        }

        /// <summary>
        /// Toggles the controls.
        /// </summary>
        private void ToggleControls()
        {
            AddAllButton.IsEnabled = availableItems.Items.Count > 0;
            AddButton.IsEnabled = availableItems.Items.Count > 0;
            RemoveAllButton.IsEnabled = selectedItems.Items.Count > 0;
            RemoveButton.IsEnabled = selectedItems.Items.Count > 0;
            MoveUpButton.IsEnabled = selectedItems.SelectedItem != null && selectedItems.SelectedIndex > 0;
            MoveDownButton.IsEnabled = selectedItems.SelectedItem != null && selectedItems.SelectedIndex < selectedItems.Items.Count - 1;
        }

        /// <summary>
        /// Handles the SelectionChanged event of the selectedItems control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void selectedItems_SelectionChanged(object sender,
            SelectionChangedEventArgs e)
        {
            ToggleControls();
        }

        /// <summary>
        /// Handles the Click event of the AddAllButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddAllButton_Click(object sender,
            RoutedEventArgs e)
        {
            while (availableItems.Items.Count > 0)
            {
                selectedItems.Items.Add(availableItems.Items[0]);
                availableItems.Items.RemoveAt(0);
            }

            ToggleControls();
        }

        /// <summary>
        /// Handles the Click event of the AddButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddButton_Click(object sender,
            RoutedEventArgs e)
        {
            selectedItems.Items.Add(availableItems.SelectedItem);
            availableItems.Items.Remove(availableItems.SelectedItem);

            ToggleControls();
        }

        /// <summary>
        /// Handles the Click event of the RemoveButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveButton_Click(object sender,
            RoutedEventArgs e)
        {
            availableItems.Items.Add(selectedItems.SelectedItem);
            selectedItems.Items.Remove(selectedItems.SelectedItem);

            ToggleControls();
        }

        /// <summary>
        /// Handles the Click event of the RemoveAllButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveAllButton_Click(object sender, RoutedEventArgs e)
        {
            while (selectedItems.Items.Count > 0)
            {
                availableItems.Items.Add(selectedItems.Items[0]);
                selectedItems.Items.RemoveAt(0);
            }

            ToggleControls();
        }

        /// <summary>
        /// Handles the Click event of the MoveUpButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MoveUpButton_Click(object sender,
            RoutedEventArgs e)
        {
            int currentSelectedIndex = selectedItems.SelectedIndex;

            if (currentSelectedIndex > 0)
            {
                selectedItems.Items.Insert(currentSelectedIndex - 1, selectedItems.SelectedItem);
                selectedItems.Items.RemoveAt(currentSelectedIndex + 1);
                selectedItems.SelectedIndex = currentSelectedIndex - 1;
            }
        }

        /// <summary>
        /// Handles the Click event of the MoveDownButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void MoveDownButton_Click(object sender,
            RoutedEventArgs e)
        {
            int currentSelectedIndex = selectedItems.SelectedIndex;

            if (currentSelectedIndex < selectedItems.Items.Count - 1)
            {
                selectedItems.Items.Insert(currentSelectedIndex + 2, selectedItems.SelectedItem);
                selectedItems.Items.RemoveAt(currentSelectedIndex);
                selectedItems.SelectedIndex = currentSelectedIndex + 1;
            }
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the availableItems control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void availableItems_MouseDoubleClick(object sender,
            MouseButtonEventArgs e)
        {
            if (availableItems.SelectedItem != null)
            {
                AddButton_Click(sender, null);
            }
        }

        /// <summary>
        /// Handles the MouseDoubleClick event of the selectedItems control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void selectedItems_MouseDoubleClick(object sender,
            MouseButtonEventArgs e)
        {
            if (selectedItems.SelectedItem != null)
            {
                RemoveButton_Click(sender, null);
            }
        }
    }
}

