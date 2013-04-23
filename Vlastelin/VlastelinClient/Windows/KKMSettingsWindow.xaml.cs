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
using VlastelinClient.Util;
using Vlastelin.KKM;
using Vlastelin.Common;
using VlastelinClient.ViewModel;

namespace VlastelinClient.Windows
{
    /// <summary>
    /// Interaction logic for WindowKKMSettings.xaml
    /// </summary>
    public partial class WindowKKMSettings : WindowBase
    {
        public WindowKKMSettings()
        {
            InitializeComponent();
        }

        public ICollection<String> CheckCutTypes
        {
            get
            {
                return Ct.GetEnumDescriptionValues(typeof(CheckCutType));
            }
        }

        public ICollection<String> TaxesPrintingFormats
        {
            get
            {
                return Ct.GetEnumDescriptionValues(typeof(TaxesPrintingFormat));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.DataContext = this;

			if (UtilManager.Instance.KKMManager.IsKKMEnabled && Properties.Settings.Default.FlagUseKKM)
            {
				this.ComboBoxCheckCutType.ItemsSource = this.CheckCutTypes;
				this.ComboBoxTaxesPrintingFormat.ItemsSource = this.TaxesPrintingFormats;

				this.CheckBoxZeroCashWhenCloseSession.IsChecked = UtilManager.Instance.KKMManager.KKM.ZeroCashWhenCloseSession;
                this.CheckBoxPrintFooterext.IsChecked = UtilManager.Instance.KKMManager.KKM.PrintFooterext;
                this.CheckBoxAutomaticDaylightShift.IsChecked = UtilManager.Instance.KKMManager.KKM.AutomaticDaylightShift;
                this.CheckBoxOpenCashBoxWhenFheckClosed.IsChecked = UtilManager.Instance.KKMManager.KKM.OpenCashBoxWhenFheckClosed;
                this.CheckBoxCheckScrollBeforeCut.IsChecked = UtilManager.Instance.KKMManager.KKM.CheckScrollBeforeCut;
				this.ComboBoxCheckCutType.SelectedItem = UtilManager.Instance.KKMManager.KKM.CheckCutType.GetDescription();
                this.ComboBoxTaxesPrintingFormat.SelectedItem = UtilManager.Instance.KKMManager.KKM.TaxesPrintingFormat.GetDescription();
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
			if (UtilManager.Instance.KKMManager.IsKKMEnabled && Properties.Settings.Default.FlagUseKKM)
            {
                FuncExec.Execute(() =>
				{
					UtilManager.Instance.KKMManager.KKM.ZeroCashWhenCloseSession = this.CheckBoxZeroCashWhenCloseSession.IsChecked.Value;
					UtilManager.Instance.KKMManager.KKM.PrintFooterext = this.CheckBoxPrintFooterext.IsChecked.Value;
					UtilManager.Instance.KKMManager.KKM.AutomaticDaylightShift = this.CheckBoxAutomaticDaylightShift.IsChecked.Value;
					UtilManager.Instance.KKMManager.KKM.OpenCashBoxWhenFheckClosed = this.CheckBoxOpenCashBoxWhenFheckClosed.IsChecked.Value;
					UtilManager.Instance.KKMManager.KKM.CheckScrollBeforeCut = this.CheckBoxCheckScrollBeforeCut.IsChecked.Value;
					UtilManager.Instance.KKMManager.KKM.CheckCutType = (CheckCutType)Ct.GetEnumFromDescription(typeof(CheckCutType), this.ComboBoxCheckCutType.SelectedItem.ToString());
					UtilManager.Instance.KKMManager.KKM.TaxesPrintingFormat = (TaxesPrintingFormat)Ct.GetEnumFromDescription(typeof(TaxesPrintingFormat), this.ComboBoxTaxesPrintingFormat.SelectedItem.ToString());

					UtilManager.Instance.MessageProvider.ShowInformationWindow("Настройки ККМ сохранены!");
				});
			}
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
