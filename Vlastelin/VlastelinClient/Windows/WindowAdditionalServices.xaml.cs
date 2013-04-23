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
using Vlastelin.Data.Model;
using VlastelinClient.Util;

namespace VlastelinClient.Windows
{
    /// <summary>
    /// Interaction logic for WindowAdditionalServices.xaml
    /// </summary>
    public partial class WindowAdditionalServices : WindowBase
    {
        /// <summary>
        /// список видов дополнительных услуг
        /// </summary>
        public IEnumerable<SalesKind> SalesKinds { get; set; }

		/// <summary>
		/// текущая дополнительная услуга
		/// </summary>
		public SalesKind CurrentKind { get; set; }

        public WindowAdditionalServices()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        /// <summary>
        /// продажа дополнительной услуги
        /// </summary>
        /// <param name="kind">вид услуги</param>
        /// <param name="price">цена</param>
        private void SellService(SalesKind kind, double price)
        {
            if (kind == null)
            {
                UtilManager.Instance.MessageProvider.ShowErrorWindow("Не выбран тип услуги");
                return;
            }

            if (price <= 0)
            {
                UtilManager.Instance.MessageProvider.ShowErrorWindow("Цена введена неверно");
                return;
            }

            FuncExec.Execute(() =>
                {
                    UtilManager.Instance.KKMManager.KKM_SellItem(kind, price);
                    UtilManager.Instance.Client.SalesKindSell(UtilManager.Instance.CurrentOperator, kind, price, UtilManager.Instance.KKMManager.LastSellCheckNum);
                    // UtilManager.Instance.MessageProvider.ShowInformationWindow(String.Format("Услуга \"{0}\" продана", kind.Name));
					this.Close();
                });
        }

        private void ButtonSell_Click(object sender, RoutedEventArgs e)
        {
            SalesKind kind = this.comboBoxServices.SelectedItem as SalesKind;
            double price = !String.IsNullOrWhiteSpace(this.textBoxPrice.Text) ? double.Parse(this.textBoxPrice.Text) : 0;

            this.SellService(kind, price);
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
