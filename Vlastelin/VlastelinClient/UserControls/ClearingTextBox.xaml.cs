using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VlastelinClient.UserControls
{
    /// <summary>
    /// Interaction logic for ClearingTextBox.xaml
    /// </summary>
    public partial class ClearingTextBox : UserControl
    {
        public event EventHandler<TextChangedEventArgs> FilterTextChanged;
        
        public String Text
        {
            get
            {
                return this.textBoxFilter.Text;
            }
            set
            {
                this.textBoxFilter.Text = value;
            }
        }

        public ClearingTextBox()
        {
            InitializeComponent();
            this.SetButtonVisibility();
        }

        private void SetButtonVisibility()
        {
            this.imageDeleteItemButton.Visibility = String.IsNullOrEmpty(this.textBoxFilter.Text) ? Visibility.Collapsed : Visibility.Visible;
        }

        private void textBoxFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            e.Handled = true;
            if (FilterTextChanged != null)
            {
                FilterTextChanged(this, e);
                this.SetButtonVisibility();
            }
        }

        private void imageDeleteItemButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.textBoxFilter.Clear();
        }
    }
}
