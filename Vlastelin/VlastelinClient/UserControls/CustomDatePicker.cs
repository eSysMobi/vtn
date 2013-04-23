using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;

namespace VlastelinClient.UserControls
{
	public class CustomDatePicker : DatePicker
	{
		public override void OnApplyTemplate()
		{
			DependencyObject d = GetTemplateChild("ButtonSetToday");
			if (d != null)
			{
				(d as Button).Click += new RoutedEventHandler(ButtonToday_Click);
			}

			base.OnApplyTemplate();
		}

		public void ButtonToday_Click(object sender, RoutedEventArgs e)
		{
			//this.SelectedDate = DateTime.Today;
			DependencyObject d = GetTemplateChild("PART_TextBox");
			if (d != null)
			{
				TextBox text = d as TextBox;
				text.Text = DateTime.Today.ToString("dd.MM.yyyy");
				text.Focus();
				this.Focus();
			}
		}
	}
}
