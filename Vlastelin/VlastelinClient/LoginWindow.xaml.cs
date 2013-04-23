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
using System.ServiceModel;
using Vlastelin.Common;
using System.ServiceModel.Security;
using System.ServiceModel.Description;
using System.IO;
using System.Net;
using VlastelinClient.Windows;

namespace VlastelinClient
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : WindowBase
    {
		/// <summary>
		/// логин из поля ввода
		/// </summary>
		public String Login
		{
			get
			{
				return this.textBoxLogin.Text;
			}
		}

		/// <summary>
		/// пароль из окна ввода пароля
		/// </summary>
		public String Password
		{
			get
			{
				return this.passwordBoxPassword.Password;
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		public LoginWindow()
        {
            InitializeComponent();
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void AttemptLogin()
		{
			if (LoginManager.AttemptLogin(this.Login, this.Password))
			{
				this.DialogResult = true;
				this.Close();
			}
			else
			{
				this.passwordBoxPassword.Focus();
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Button_Click(object sender, RoutedEventArgs e)
        {
			this.AttemptLogin();
        }

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void WindowBase_Loaded(object sender, RoutedEventArgs e)
		{
			this.textBoxLogin.Text = Properties.Settings.Default.LastSuccessfulLogin;
			if (String.IsNullOrEmpty(this.textBoxLogin.Text))
			{
				this.textBoxLogin.Focus();
			}
			else
			{
				this.passwordBoxPassword.Focus();
			}
			
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////

		private void Grid_KeyDown(object sender, KeyEventArgs e)
		{			
			if (e.Key == Key.Enter)
			{
				e.Handled = true;
				this.AttemptLogin();
			}
			if (e.Key == Key.Escape)
			{
				e.Handled = true;
				this.DialogResult = false;
				this.Close();
			}
		}

		/// <summary>
		/// обработка события нажатия кнопки отправления отчета об ошибке
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Button_Click_1(object sender, RoutedEventArgs e)
		{
			try
			{
				WindowEnterMessage window = new WindowEnterMessage();
				if (window.ShowDialog() == true)
				{
					UtilManager.Instance.SendErrorNotification(window.Description);
					UtilManager.Instance.MessageProvider.ShowInformationWindow("Отчет об ошибке успешно отправлен");
				}
			}
			catch (IOException ex)
			{
				UtilManager.Instance.MessageProvider.ShowErrorWindow("Возникла ошибка работы с файлами логов");
				UtilManager.Instance.logger.LogMessage(LogEventType.Error, ex.ToString());
			}
			catch (WebException ex)
			{
				UtilManager.Instance.MessageProvider.ShowErrorWindow("Не удалось выполнить отправку отчета об ошибке. Проблемы при соединении с сервером");
				UtilManager.Instance.logger.LogMessage(LogEventType.Error, ex.ToString());
			}
			catch (Exception ex)
			{
				UtilManager.Instance.MessageProvider.ShowErrorWindow(ClientMsg.ErrorUnknown);
				UtilManager.Instance.logger.LogMessage(LogEventType.Error, ex.ToString());
			}
		}

		////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
