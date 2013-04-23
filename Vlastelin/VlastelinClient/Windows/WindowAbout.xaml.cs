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
using System.Reflection;
using VlastelinClient.Util;

namespace VlastelinClient.Windows
{
	/// <summary>
	/// Interaction logic for WindowAbout.xaml
	/// </summary>
	public partial class WindowAbout : WindowBase
	{
		public WindowAbout()
		{
			InitializeComponent();
		}

		private void WindowBase_Loaded(object sender, RoutedEventArgs e)
		{
			this.textBlockName.Text = String.Format("{0} Version {1}", Assembly.GetExecutingAssembly().GetName().Name, UtilManager.Instance.CurrentAppVersion);
			this.buttonOk.Focus();
		}
	}
}
