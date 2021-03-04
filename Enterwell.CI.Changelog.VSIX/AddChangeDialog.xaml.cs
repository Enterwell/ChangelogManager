using System;
using System.Windows;
using System.Windows.Controls;
using Enterwell.CI.Changelog.Shared;
using TextBox = System.Windows.Controls.TextBox;

namespace Enterwell.CI.Changelog.VSIX
{
    /// <summary>
    /// Interaction logic for AddChangeDialog.xaml
    /// </summary>
    public partial class AddChangeDialog : Window
    {
        private readonly string solutionPath;

        private readonly string[] changeTypes = {"Added", "Changed", "Deprecated", "Removed", "Fixed", "Security"};

        /// <summary>
        /// Gets the text from the Change Type dropdown.
        /// </summary>
        public string ChangeType => TypeComboBox.Text;

        /// <summary>
        /// Gets the text from the Change Category dropdown or from the Change Category text box if the changelog configuration
        /// does not exist and therefore the dropdown is not visible.
        /// </summary>
        public string ChangeCategory => CategoryComboBox.Text != string.Empty ? CategoryComboBox.Text : CategoryTextBox.Text;

        /// <summary>
        /// Gets the text from the Change Description text box.
        /// </summary>
        public string ChangeDescription => DescriptionTextBox.Text;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddChangeDialog"/> class and therefore initializes the component.
        /// It also initializes the dropdowns with corresponding data.
        /// </summary>
        /// <param name="solutionPath">Path to the solution which is used to find the changelog configuration in its root, not null.</param>
        /// <exception cref="ArgumentNullException">Thrown if argument is null.</exception>
        public AddChangeDialog(string solutionPath)
        {
            this.solutionPath = solutionPath ?? throw new ArgumentNullException(nameof(solutionPath));

            InitializeComponent();
            InitializeChangeTypes();
            InitializeChangeCategories();

            DescriptionTextBox.Focus();
        }

        /// <summary>
        /// Initializes the Change Type dropdown with data and makes the first element selected by default.
        /// </summary>
        private void InitializeChangeTypes()
        {
            TypeComboBox.ItemsSource = changeTypes;
            TypeComboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Initializes and displays the Change Category dropdown if the changelog configuration exists, or in case it doesn't,
        /// displays the Change Category text box that accepts any text from the user.
        /// </summary>
        private void InitializeChangeCategories()
        {
            var configuration = Configuration.LoadConfiguration(this.solutionPath);

            if (configuration == null || configuration.IsEmpty())
            {
                CategoryComboBox.Visibility = Visibility.Hidden;
                CategoryTextBox.Visibility = Visibility.Visible;
            }
            else
            {
                CategoryComboBox.ItemsSource = configuration.Categories;
                CategoryComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Callback for when the Add Change button is pressed.
        /// Sets the <see cref="Window.DialogResult"/> to <see langword="true"/> and closes the dialog.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void OnAddChangeBtnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Callback for when the Cancel Change button is pressed.
        /// Sets the <see cref= "Window.DialogResult" /> to < see langword= "false" /> and closes the dialog.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void OnCancelChangeBtnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Callback for when the text is entered in the <see cref="DescriptionTextBox"/>.
        /// Used to enable <see cref="AddChangeBtn"/> if the text box is not empty and disable it otherwise.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void DescriptionTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var descriptionTextBox = sender as TextBox;

            if (string.IsNullOrWhiteSpace(descriptionTextBox?.Text))
            {
                AddChangeBtn.IsEnabled = false;
            }
            else
            {
                AddChangeBtn.IsEnabled = true;
            }
        }
    }
}