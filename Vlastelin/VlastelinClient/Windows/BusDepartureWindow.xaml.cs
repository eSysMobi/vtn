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
using VlastelinClient.ViewModel.ObjectsViewModel;
using VlastelinClient.ViewModel.WindowsViewModel;
using System.ComponentModel;

namespace VlastelinClient.Windows
{
    /// <summary>
    /// Interaction logic for BusDepartureWindow.xaml
    /// </summary>
    public partial class BusDepartureWindow : WindowBase
    {
        public BusDepWindowVM viewModel
        {
            get
            {
                return this.DataContext as BusDepWindowVM;
            }
        }
        
        public BusDepartureWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// событие загрузки окна
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowBase_Loaded(object sender, RoutedEventArgs e)
        {
			this.viewModel.ViewModelNotify += new System.ComponentModel.PropertyChangedEventHandler(ViewModel_ViewModelNotify);
			this.viewModel.UpdateDriverList();
            this.comboBoxDrivers1.SelectedIndex = 0;
			this.viewModel.UpdateDriversComboList(this.comboBoxDrivers1.SelectedItem as DriverVM);
			this.comboBoxDrivers2.SelectedIndex = 0;
        }

        /// <summary>
        /// обработчик события изменения первого комбобокса водителей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxDrivers1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.viewModel.UpdateDriversComboList(this.comboBoxDrivers1.SelectedItem as DriverVM);
            this.comboBoxDrivers2.SelectedIndex = 0;
        }

		private void ViewModel_ViewModelNotify(object sender, PropertyChangedEventArgs e)
		{
			// this.DialogResult = true;
			this.Close();
		}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
