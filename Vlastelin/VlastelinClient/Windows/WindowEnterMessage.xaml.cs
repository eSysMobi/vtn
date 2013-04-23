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
	/// Interaction logic for WindowEnterMessage.xaml
	/// </summary>
	public partial class WindowEnterMessage : WindowBase
	{
		public String Description
		{
			get
			{
				return this.textBoxTitleReport.Text;
			}
		}
		
		public WindowEnterMessage()
		{
			InitializeComponent();
		}

		private void buttonLogin_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
		}

		private void WindowBase_Loaded(object sender, RoutedEventArgs e)
		{
			this.textBoxTitleReport.Focus();
		}
	}
}
