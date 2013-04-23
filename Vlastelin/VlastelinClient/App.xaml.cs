using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using VlastelinClient.Util;
using VlastelinClient.Windows;
using Vlastelin.Common;
using Vlastelin.Data.Model;
using VlastelinClient.ViewModel;
//using VlastelinClient.ServiceReference2;
using System.ComponentModel;
using VlastelinClient.ServiceReference1;
using System.Windows.Markup;
using System.Globalization;
using System.Threading;
//using VlastelinClient.ServiceReference3;

namespace VlastelinClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindowVM mainViewModel;
        private bool DoHandle = true;

        /// <summary>
        /// задаем основные параметры программы
        /// сервис, сплэш-скрин, оператора, првайдера для работы с окнами сообщений итд
        /// </summary>
        private void PreInitialize()
        {
            UtilManager.Instance.Client = new VlastelinSrvClient();
            this.mainViewModel = new MainWindowVM();

            // обработчик неотловленных исключений
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            // режим закрытия приложения
            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

			// устанавливаем русскую культуру для всего приложения
			FrameworkElement.LanguageProperty.OverrideMetadata(
				typeof(FrameworkElement),
				new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(Ct.RussianCulture.IetfLanguageTag)));
        }

        /// <summary>
        /// загрузка данных и запуск основного окна
        /// </summary>
        private void LoadInitData()
        {
            // создаем окно и активируем класс для работы с фоновым потоком
            var mainWindow = new MainWindow();
            BackgroundWorker worker = new BackgroundWorker();

            // процесс, работающий в фоновом режиме
            worker.DoWork += (o, ea) =>
            {
				// показываем сплэш-скрин в потоке UI-интерфейса
				DispatchService.Invoke(() =>
				{
					UtilManager.Instance.StateManager.ServerState = ServerStates.Connected;
					
					UtilManager.Instance.ShowSplash();
					UtilManager.Instance.ChangeSplashString("Проверка версии...");
				});

				DispatchService.Invoke(() =>
				{
					UtilManager.Instance.ChangeSplashString("Загрузка настроек...");
				});
				// загружаем данные
				mainViewModel.LoadInitDataSettings();

				// проверяем версию приложения
				mainViewModel.CheckApplicationVersion();

				DispatchService.Invoke(() =>
				{
					UtilManager.Instance.ChangeSplashString("Поиск ККМ...");
				});
				mainViewModel.LoadInitDataSearchKKM();

				DispatchService.Invoke(() =>
				{
					UtilManager.Instance.ChangeSplashString("Загрузка маршрутов...");
				});
				mainViewModel.LoadInitDataTrips();

				DispatchService.Invoke(() =>
				{
					UtilManager.Instance.ChangeSplashString("Загрузка расписания...");
				});
				mainViewModel.LoadInitDataTripSchedules();
              
                // устанавливаем главное окно программы
                DispatchService.Invoke(() =>
                {
                    Current.MainWindow = mainWindow;
                });

                mainWindow.mainViewModel = this.mainViewModel;
            };

            // обработчик по окончанию фонового процесса
            worker.RunWorkerCompleted += (o, ea) =>
            {
                // закрывает сплэш-скрин и показывает диалоговое окно
                UtilManager.Instance.CloseSplash();

                if (ea.Error == null)
                {
                    mainWindow.Focus();
                    mainWindow.Show();
                }
                else
                {
                    FuncExec.ProcessException(ea.Error);
                    Application.Current.Shutdown();
                }
            };

            // стартуем фоновый процесс
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// обработчик события старта приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            this.PreInitialize();
            LoginWindow loginWnd = new LoginWindow();

			// если сделана попытка логина
			if (loginWnd.ShowDialog() == true)
			{
				Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
                this.LoadInitData();
			}
			else Application.Current.Shutdown();
        }

        /// <summary>
        /// обработчик неотловленных исключений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
			Exception ex = e.ExceptionObject as Exception;
            FuncExec.ProcessException(ex);
			// Application.Current.Shutdown();
        }

		/// <summary>
		/// обработчик события поимки неотловленных исключений в потоке интерфейса
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
		{
			FuncExec.ProcessException(e.Exception);
		}
    }
}
