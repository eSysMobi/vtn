using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VlastelinClient.Util;
using System.Collections.ObjectModel;
using VlastelinClient.ViewModel.ObjectsViewModel;
using Vlastelin.Data.Model;
using VlastelinClient.ViewModel.WindowsViewModel;
using VlastelinClient.Windows;
using Vlastelin.Common;
using System.ServiceModel;
using VlastelinClient.ServiceReference1;

namespace VlastelinClient.ViewModel
{
    /// <summary>
    /// вьюмодель для списка мест
    /// </summary>
    public class SeatTableVM : BaseViewModel
    {
        /// <summary>
        /// вьюмодель окна пассажира
        /// </summary>
        public PassengerWindowVM passengerVM { get; private set; }

        /// <summary>
        /// список загруженных из базы мест
        /// </summary>
        public ObservableCollection<SeatVM> LoadedSeats { get; set; }

        /// <summary>
        /// список мест, показываемых для выделенного расписания
        /// </summary>
        public ObservableCollection<SeatVM> ListSeats { get; set; }

		/// <summary>
		/// сумма проданных мест
		/// </summary>
		public decimal SeatsSoldSum { get; set; }

        public SeatTableVM()
        {
            this.Init();
        }

        /// <summary>
        /// инициализация начальных данных
        /// </summary>
        private void Init()
        {
            this.passengerVM = new PassengerWindowVM();
        }

        /// <summary>
        /// загрузка списка уже занятых или блокированных мест из базы данынх
        /// </summary>
        /// <param name="shedule"></param>
        public bool LoadData(StationScheduleVM shedule, TripPriceVM price, DateTime date)
        {
			// проверяем, возможна ли загрузка мест и необходима ли
			if (shedule == null || price == null || !UtilManager.Instance.NeedLoad(ModifiedObjects.Seats))
			{
				return false;
			}

			// загружаем места из бд (включаю сумму и оператора)
			List<Tuple<Seat, double, String>> result = UtilManager.Instance.Client.SeatsGet(shedule.ss, price.tripPrice, date.SetTime(shedule.DepartureTime)).ToList();
			
			// формируем места с необходимыми данными
			this.LoadedSeats = new ObservableCollection<SeatVM>(
				result.Select(s => new SeatVM(s.Item1)
				{
					Price = s.Item2,
					SellingOperator = s.Item3
				}));

			// рассчитываем сумму билетов
			this.SeatsSoldSum = (decimal)this.LoadedSeats.Sum(s => s.Price);

			return true;
        }

        /// <summary>
        /// обновление списка мест
        /// </summary>
        /// <param name="shedule">выделенное расписание</param>
        public void UpdateSeatsList(StationScheduleVM shedule,  TripPriceVM price, DateTime date)
        {
            if (shedule != null && price != null)
            {
                this.ListSeats = new ObservableCollection<SeatVM>();
                this.OnPropertyChanged("ListSeats");
                SeatVM seat = null;

                // заполняем места пустыми значениями, но если найдено соответствие номера места и маршрута в загруженных из базы, это место добавляется в лист
                for (int i = 1; i <= shedule.TS.Bus.PassengersCount; i++)
                {
                    if (this.LoadedSeats != null)
                    {
                        // так как у нас уже загружены места для конкретного подмаршрута и времени, то ищем только по номеру места
                        seat = this.LoadedSeats.FirstOrDefault(s => s.SeatNumber == i);
                    }

                    // если место не найдено, то создаем пустое
                    if (seat == null)
                    {
                        seat = new SeatVM() 
						{ 
							SeatNumber = i, 
							State = SeatState.Free, 
							Passenger = null, 
							SS = shedule, 
							TripPrice = price, 
							TripDate = date.Date.SetTime(shedule.DepartureTime)
						};
						//seat.seat.SSid = shedule.ss.Id;
                    }
                    this.ListSeats.Add(seat);
                }
            }
        }

        /// <summary>
        /// обработка места (продажа, блокрование, бронирование)
        /// </summary>
        /// <param name="seat">выбранное место</param>
        public void ProcessSeat(SeatVM fixedSeat)
        {
			// состояние выбранного места
			// необходимо чтобы сохранять бронь если продается бронированное место
			SeatState currentState = fixedSeat.State;
			
			// ищем выбранное место в списке мест и работаем с найденным, сбрасываем его на свободное(нужно для бронированных)
            SeatVM seat = this.ListSeats.FirstOrDefault(s => s.SeatNumber == fixedSeat.SeatNumber && s.SS.Equals(fixedSeat.SS));

			// устанавливаем цену и оператора
			seat.Price = (double)seat.TripPrice.Price;
			seat.SellingOperator = UtilManager.Instance.CurrentOperator.InitialName;
            seat.State = SeatState.Free;

            // блокируем место, получая его идентификатор и таймаут на оформление билета
            Tuple<TimeSpan, long> res = UtilManager.Instance.Client.SeatLock((int)seat.SeatNumber, (int)seat.SS.ss.Id, (int)seat.TripPrice.tripPrice.Id, seat.TripDate);
			UtilManager.Instance.UpdateModifiedObject(ModifiedObjects.Seats);
            TimeSpan timeout = res.Item1;
            seat.seat.Id = res.Item2;
            seat.State = SeatState.Locked;

            // если место не найдено или его не удалось заблокировать, то оно не может быть обработано
            if (seat == null || seat.State != SeatState.Locked)
            {
                UtilManager.Instance.MessageProvider.ShowInformationWindow("Возникла ошибка при блокировании места");
                return;
            }
           
            // создаем окно для ввода данных пассажира, устанавливаем датаконтекст для вьюмодели
            WindowPassenger window = new WindowPassenger();
            this.passengerVM.Init(timeout, seat);
            window.DataContext = this.passengerVM;

            // вызываем окно данных пассажира, и после его закрытия разлочиваем место и делаем его свободным для последующей обработки
            bool? result = window.ShowDialog();
            seat.State = SeatState.Free;

            // если время для ввода данных не вышло и в форме нажаты кнопки Бронировать либо Заказать, то проводим с местом соответствуующие процедуры
			if (result.HasValue && result.Value && this.passengerVM.IsTimeExpired)
			{
				this.OnPropertyChanged("ListSeats");
				// если место для продажы, то устанавливаем его как проданное

				// продаем место
				UtilManager.Instance.Client.SeatSell(UtilManager.Instance.CurrentOperator, seat.seat, seat.Passenger.passenger, seat.Price, UtilManager.Instance.KKMManager.LastSellCheckNum+1);
				seat.State = SeatState.Sold;

				this.SeatsSoldSum += (decimal)seat.Price;

				// создаем чек и печатаем его на ККМ
				UtilManager.Instance.KKMManager.KKM_SellSeat(seat.seat, seat.Price);
			}
			else
			{
				// разлочиваем место при отмене
				seatLockKey key = new seatLockKey()
				{
					seatNum = seat.SeatNumber,
					ssId = seat.SS.ss.Id,
					tpId = seat.TripPrice.tripPrice.Id,
					dt = seat.TripDate
				};
				UtilManager.Instance.Client.SeatUnlock(key);
                seat.State = SeatState.Free;

				// если место было забронировано, то снова бронируем его
				if (currentState == SeatState.Reserved)
				{
					UtilManager.Instance.Client.SeatReserve(seat.seat);
					seat.State = SeatState.Reserved;
				}
			};
        }

		/// <summary>
		/// возврат билета
		/// </summary>
		/// <param name="returnedSeat">возвращаемое место</param>
		public void SeatReturn(SeatVM returnedSeat, double commission)
		{
			SeatVM seat = this.ListSeats.FirstOrDefault(s => s.Equals(returnedSeat));
            
			SalesHistory sh = UtilManager.Instance.Client.SalesHistoryBySeat(seat.seat).FirstOrDefault();
			double sum = sh.FactPrice;
			UtilManager.Instance.KKMManager.KKM_ReturnTicket(seat.seat, sum, commission);
			
			seat.Passenger = null;
            UtilManager.Instance.Client.SeatSellReturn(UtilManager.Instance.CurrentOperator, seat.seat, UtilManager.Instance.KKMManager.LastSellReturnCheckNum, UtilManager.Instance.KKMManager.LastSellCheckNum);

            //seat.Passenger = new PassengerVM();
			seat.State = SeatState.Free;
			this.SeatsSoldSum -= (decimal)seat.Price;
		}
    }
}