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
using Vlastelin.Data.Model;

namespace VlastelinClient.Windows
{
	/// <summary>
	/// Interaction logic for WindowDepositMoney.xaml
	/// </summary>
	public partial class WindowDepositMoney : WindowBase
	{
		public double Sum
		{
			get
			{
				return (double)this.doubleUpDownSum.Value;
			}
		}

		
		public WindowDepositMoney()
		{
			InitializeComponent();
		}

		private void ButtonApply_Click(object sender, RoutedEventArgs e)
		{
			if (this.doubleUpDownSum.Value == null || this.doubleUpDownSum.Value <= 0)
			{
				UtilManager.Instance.MessageProvider.ShowInformationWindow("Сумма должна быть больше нуля");
			}
			else
			{
				this.DialogResult = true;
				this.Close();
			}
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		private void WindowBase_Loaded(object sender, RoutedEventArgs e)
		{
			this.doubleUpDownSum.Focus();
		}
	}
}
