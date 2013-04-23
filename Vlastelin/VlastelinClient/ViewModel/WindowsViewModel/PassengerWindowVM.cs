using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VlastelinClient.ViewModel.ObjectsViewModel;
using VlastelinClient.Util;
using System.Windows.Input;
using VlastelinClient.Commands;
using Vlastelin.Data.Model;
using System.Collections.ObjectModel;
using Vlastelin.Common;

namespace VlastelinClient.ViewModel.WindowsViewModel
{
    /// <summary>
    /// вьюмодель для окна ввода данных пассажира
    /// </summary>
    public class PassengerWindowVM : BaseViewModel
    {
        /// <summary>
        /// вспомогательные объекты
        /// </summary>
        private UtilManager utilite;

        /// <summary>
        /// таймаут блокировки места
        /// </summary>
        public TimeSpan EstimatedTime { get; set; }

        /// <summary>
        /// таймаут блокировки места (предельный)
        /// </summary>
        public TimeSpan LimitTime { get; private set; }

        /// <summary>
        /// выбранное место
        /// </summary>
        public SeatVM CurrentSeat { get; private set; }

        /// <summary>
        /// вспомогательное свойство для отображения параметров рейса
        /// </summary>
        public StationScheduleVM Shedule
        {
            get
            {
                return this.CurrentSeat != null ? this.CurrentSeat.SS : null;
            }
        }

        /// <summary>
        /// список городов прибытия и отправления, для комбобоксов
        /// </summary>
        public ObservableCollection<TownVM> DepartureTowns { get; set; }
        public ObservableCollection<TownVM> ArrivalTowns { get; set; }
        
        /// <summary>
        /// пассажир, используемый для биндинга
        /// </summary>
        public PassengerVM Passenger { get; set; }

        /// <summary>
        /// флаг определяет, кончилось ли время бронирования билета или нет
        /// </summary>
        public bool IsTimeExpired { get; private set; }

        public PassengerWindowVM()
        {
        }

        /// <summary>
        /// инициализация начальных данных
        /// </summary>
        public void Init(TimeSpan limit, SeatVM seat)
        {
            this.Passenger = new PassengerVM();
            this.EstimatedTime = limit;

            this.LimitTime = limit;
            this.CurrentSeat = seat;
            this.IsTimeExpired = true;
        }

        /// <summary>
        /// обработчик тика таймера
        /// </summary>
        /// <returns>истина - время блокировки не закончилось</returns>
        public bool TimerTick()
        {
            this.EstimatedTime = this.EstimatedTime.Add(new TimeSpan(0, 0, -1));
            this.OnPropertyChanged("EstimatedTime");

            bool istime = this.EstimatedTime == new TimeSpan();

            if (istime)
            {
                this.IsTimeExpired = false;
            }

            return !istime;
        }

        /// <summary>
        /// обработчик команды покупки билета
        /// </summary>
        /// <param name="param">цена места</param>
        public bool BuyTicket(double price, String anotherArrival)
        {
            if (this.IsTimeExpired)
            {
                try
                {
                    this.CurrentSeat.State = SeatState.Sold;
                    this.CurrentSeat.Price = price;
                    this.CurrentSeat.DesiredDestination = !String.IsNullOrWhiteSpace(anotherArrival) ? anotherArrival : String.Empty;
                    PassengerVM pass = new PassengerVM();
                    pass.CopyFrom(this.Passenger);
                    pass.DocType = 1;
                    this.CurrentSeat.Passenger = pass;

                    this.OnPropertyChanged("CurrentSeat");
                    UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Info, String.Format("Продажа билета : {0}", this.Passenger.ToString()));
                    return true;
                }
                catch (IncorrectDataException ex)
                {
                    UtilManager.Instance.MessageProvider.ShowInformationWindow(ex.Message);
                    return false;
                }
                catch (Exception ex)
                {
                    UtilManager.Instance.logger.LogMessage(Vlastelin.Common.LogEventType.Error, String.Format("Продажа билета : {0}", ex.ToString()));
                    return false;
                }
            }
            return false;
        }
    }
}
