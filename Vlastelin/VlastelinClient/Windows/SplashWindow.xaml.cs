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
using System.Windows.Shapes;

namespace VlastelinClient.Windows
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        protected const string DefaultDialogWindowStyle = "Window_Splash_Style";

        public SplashWindow()
        {
            InitializeComponent();
            this.SetResourceReference(StyleProperty, DefaultDialogWindowStyle);
        }
    }
}
