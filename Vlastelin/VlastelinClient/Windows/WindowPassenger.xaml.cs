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
using System.Windows.Threading;
using VlastelinClient.ViewModel.WindowsViewModel;
using VlastelinClient.ViewModel.ObjectsViewModel;
using VlastelinClient.Util;

namespace VlastelinClient.Windows
{
    /// <summary>
    /// Interaction logic for WindowPassengerк.xaml
    /// </summary>
    public partial class WindowPassenger : WindowBase
    {
        /// <summary>
        /// таймер для отсчета времени на оформление билета
        /// </summary>
        DispatcherTimer timer = new DispatcherTimer();

        /// <summary>
        /// вьюмодель окна пассажира
        /// </summary>
        public PassengerWindowVM passengerVM
        {
            get
            {
                return this.DataContext as PassengerWindowVM;
            }
        }
        
        public WindowPassenger()
        {
            InitializeComponent();

            // устанавливаем обработчик тика таймера и время срабатывания раз в секунду
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 1, 0);
        }

        /// <summary>
        /// при старте окна запускаем таймер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowBase_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Start();
            this.textBoxSurname.Focus();
        }

        /// <summary>
        /// обработчик тика таймера
        /// если время на оформление вышло, то останавливаем таймер и выводим надпись
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Tick(object sender, EventArgs e)
        {
            if (!this.passengerVM.TimerTick())
            {
                this.StopTimer();
                this.textBlockTime.Text = "Время на оформление билета закончилось. Данные не будут сохранены!";
                this.textBlockTime.FontSize = 14;
                this.textBlockTime.Foreground = Brushes.Red;
            }
        }

        /// <summary>
        /// остановка таймера
        /// </summary>
        private void StopTimer()
        {
            this.timer.Stop();
        }

        /// <summary>
        /// обработчик события нажатия кнопки Закрыть
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = null;
            this.timer.Stop();

        }

		/// <summary>
		/// проверка на ошибки при вводе форм
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		private bool HasErrors(DependencyObject obj)
		{
			foreach (object child in LogicalTreeHelper.GetChildren(obj))
			{
				TextBox element = child as TextBox;
				if (element == null) continue;
				if (Validation.GetHasError(element) || (element.Text.Length == 0))
				{
					return true;
				}
				HasErrors(element);
			}
			return false;
		}

        /// <summary>
        /// обработчик нажатия кнопки Купить
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Buy_Click(object sender, RoutedEventArgs e)
        {
			if (this.HasErrors(this.gridDataForms))
			{
				UtilManager.Instance.MessageProvider.ShowInformationWindow("Неправильно заполнены некоторые поля");
			}else
			
			if (passengerVM.BuyTicket(this.doubleUpDownPrice.Value.Value, this.textBoxAnotherArrival.Text))
            {
                this.StopTimer();
                this.DialogResult = true;
            }
        }

        /// <summary>
        /// обработчик закрытия окна. останавливаем таймер
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowBase_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.timer.Stop();
        }

        private void checkBoxAnotherArrival_Checked(object sender, RoutedEventArgs e)
        {
            this.textBoxAnotherArrival.Text = String.Empty;
        }

		/// <summary>
		/// переход по текстбоксам по энтеру
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void gridDataForms_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key != Key.Return)
			{
				return;
			}
			List<Control> list = new List<Control>();

			// проходим по текстбоксам и дообавляем их в лист
			foreach (object child in LogicalTreeHelper.GetChildren(sender as Grid))
			{
				if (child is Control)
				{
					list.Add(child as Control);
				}
			}

			// упорядочиваем по табиндексу
			list = list.OrderBy(tx => tx.TabIndex).ToList();
			
			// ищем тот текстбокс, на котором фокус
			Control ctrl = list.FirstOrDefault(tx => tx.IsFocused);
			if (ctrl == null)
			{
				// если фокуса нет, то ставим
				list.First().Focus();
			}
			else
			{
				// иначе ищем индекс фокусированного текстбокса и берем следующий либо первый, если выбран последний
				int index = list.IndexOf(ctrl);
				if (index == list.Count - 1) index = 0; else index++;
				list[index].Focus();
			}
		}

		/// <summary>
		/// обработка нажатия клавиши для датепикера
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void datePickerDocDate_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			this.buttonSell.Focus();
			e.Handled = true;
		}
    }
}
