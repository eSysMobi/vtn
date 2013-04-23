using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using VlastelinClient.ViewModel.ObjectsViewModel;
using Vlastelin.Data.Model;
//using VlastelinClient.ServiceReference1;
using VlastelinClient.Util;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;
using VlastelinClient.Commands;
using System.Windows.Data;
using System.Data;
using System.Windows;

namespace VlastelinClient.ViewModel
{
    public class TripWindowVM : BaseWindowVM
    {
        private TownWindowVM townWindowVM;

        /// <summary>
        /// список маршрутов
        /// </summary>
        public ObservableCollection<TripVM> Trips { get; private set; }

        /// <summary>
        /// полный список городов
        /// </summary>
		public IEnumerable<TownVM> TownsList
        {
            get
            {
				return this.townWindowVM != null ? this.townWindowVM.Towns.OrderBy(t => t.Name) : null;
            }
        }

        /// <summary>
        /// список подмаршрутов
        /// </summary>
        public IEnumerable<TripPriceVM> TripPrices { get; set; }

        /// <summary>
        /// список порядка городов
        /// </summary>
        public IEnumerable<StationOrder> StationOrders { get; set; }

        /// <summary>
        /// список городов, используется для выбора пункта прибытия и отъезда в промежуточных пунктах
        /// фильтруется в зависимости от пунктов
        /// </summary>
        private IEnumerable<TownVM> _towns;
		public IEnumerable<TownVM> Towns 
        {
            get
            {
                return this._towns.OrderBy(t => t.Name);
            }
            set
            {
                this._towns = value;
                this.OnPropertyChanged("Towns");
            }
        }

        public ICommand AddSubTripCommand
        {
            get
            {
                return new RelayCommand(this.AddSubTripExecute, this.SubTripCanExecute);
            }
        }

        public ICommand DeleteSubTripCommand
        {
            get
            {
                return new RelayCommand(this.DeleteSubTripExecute, this.SubTripCanExecute);
            }
        }

        public ICommand SaveSubTripCommand
        {
            get
            {
                return new RelayCommand(this.SaveSubTripExecute, this.SaveSubTripCanExecute);
            }
        }

        public ICommand MoveUpRouteTownCommand
        {
            get
            {
                return new RelayCommand(this.MoveUpRouteTownExecute, this.MoveUpSubTripCanExecute);
            }
        }

        public ICommand MoveDownRouteTownCommand
        {
            get
            {
                return new RelayCommand(this.MoveDownRouteTownExecute, this.MoveDownSubTripCanExecute);
            }
        }

        public TripWindowVM(TownWindowVM townVM)
        {
            this.townWindowVM = townVM;
        }

        /// <summary>
        /// получение списка маршрутов из базы данных и приведение его к типу вьюмодели
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.ModifiedObj = Vlastelin.Common.ModifiedObjects.Trips;
        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// загужает основные списки данных для маршрутов
        /// список маршрутов
        /// список цен на маршруты
        /// порядок городов
        /// </summary>
        public void LoadMainLists()
        {
            if (UtilManager.Instance.NeedLoad(Vlastelin.Common.ModifiedObjects.Trips))
			{
				this.Trips = new ObservableCollection<TripVM>(UtilManager.Instance.Client.TripsGet(null).OrderBy(t => t.Departure.Name).Select(item => new TripVM(item)));
				this.TripPrices = UtilManager.Instance.Client.TripPricesGet(null).Select(item => new TripPriceVM(item));
				this.StationOrders = UtilManager.Instance.Client.StationOrderGet(null);
			}
        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обновляет основные массивы для одного маршрута
        /// </summary>
        /// <param name="trip">маршрут</param>
		public void UpdateData()
        {
			this.LoadMainLists();

			// устанавливаем подмаршруты для каждого полученного маршрута
			// генерируем матрицу цен
            foreach (var trip in this.Trips)
            {
                trip.SetRouteTowns(this.StationOrders);
                trip.SetTripPrices(this.TripPrices);
                trip.GeneratePriceMatrix();
            }
			this.OnPropertyChanged("Trips");
        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// загрузка данных из базы
        /// </summary>
        public override void LoadDataExecute()
        {
            base.LoadDataExecute();
			this.UpdateData();

            if (this.townWindowVM.Towns != null)
            {
                this.Towns = new ObservableCollection<TownVM>(this.townWindowVM.Towns.OrderBy(t => t.Name));
            }
        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// получение объекта маршрут из данных формы
        /// </summary>
        /// <param name="fields">список полей</param>
        /// <returns>получаемый маршрут</returns>
        private BaseItemVM GetItemFromList(IList<object> fields)
        {
            // 5 - количество полей в классе маршрут + изменяемый маршрут
            if (fields == null || fields.Count != 5)
            {
                throw new ArgumentException("Не удалось получить маршрут из данных формы");
            }

            TripVM item = new TripVM();
            TripVM oldItem;

            // формирование объекта автобус из данных формы
            oldItem = fields[0] as TripVM;
			item.Name = fields[1].ToString();
            item.Departure = fields[2] != null ? fields[2] as TownVM : null;
            item.Arrival = fields[3] != null ? fields[3] as TownVM : null;
            item.Description = fields[4].ToString();
            item.trip.Id = oldItem == null ? 0 : oldItem.trip.Id;

            this.flagTypeEdit = oldItem != null;

            return item;
        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик команды для удаления маршрута
        /// </summary>
        /// <param name="param">объект маршрута</param>
        protected override void DeleteItem(object param)
        {
            TripVM item = param as TripVM;
            if (item != null)
            {
                UtilManager.Instance.Client.TripDelete(item.trip);
                this.Trips.Remove(item);
                this.OnChangeItem(this.Trips.FirstOrDefault());
            } 
        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик команды для изменения маршрута
        /// </summary>
        /// <param name="param">объект маршрута</param>
        protected override void EditItem(object param)
        {
            TripVM item = this.GetItemFromList(param as IList<object>) as TripVM;

            if (item == null)
            {
                throw new ArgumentException("Не удалось прочитать данные маршрута из формы");
            }

            if (this.flagTypeEdit)
            {
                // если объект изменяюм, то находим этот объект в общем списке, копируем в него текущие значения полей и изменяем в клиенте
                UtilManager.Instance.Client.TripEdit(item.trip);
            }
            else
            {
                // если объект дообавляем, то вызываем метод клиента для маршрутов
                item.trip.Id = UtilManager.Instance.Client.TripAdd(item.trip);
            }

            this.UpdateData();
            this.OnChangeItem(null);
        }

        /////////////////////////////////////////////////////////////////////////////

        #region Работа с подмаршрутами

        /// <summary>
        /// обработчик команды добавления промежуточного пункта
        /// </summary>
        /// <param name="param">маршрут и добавляемый город</param>
        private void AddSubTripExecute(object param)
        {
            List<object> lst = param as List<object>;

            if (lst != null)
            {
                TripVM trip = lst[0] as TripVM;
                TownVM town = lst[1] as TownVM;
                
                // имеется выделенный маршрут, в который добавляем пункты
                // новый город не имеется в списоке промежуточных пунктов
                // новый город не является пунктом отправления или прибытия

                if (trip != null && town != null)
                {
                    trip.AddRouteTown(town);
                    trip.GeneratePriceMatrix();
                }
            }
        }

        /// <summary>
        /// проверка на возможность добавления и удаления промежуточных пунктов
        /// </summary>
        /// <param name="param"></param>
        private bool SubTripCanExecute(object param)
        {
            List<object> lst = param as List<object>;

            if (lst == null) return false;

            TripVM trip = lst[0] as TripVM;
            TownVM town = lst[1] as TownVM;

            if (trip == null || town == null) return false;

            return true;
        }

        /// <summary>
        /// обработчик команды удаления промежуточного пункта
        /// </summary>
        /// <param name="param"></param>
        private void DeleteSubTripExecute(object param)
        {
            List<object> lst = param as List<object>;

            if (lst != null)
            {
                TripVM trip = lst[0] as TripVM;
                TownVM town = lst[1] as TownVM;

                trip.DeleteRouteTown(town);
                trip.GeneratePriceMatrix();
            }
        }

        /// <summary>
        /// обработчик команды сохранения списка подмаршрутов
        /// </summary>
        /// <param name="param"></param>
        private void SaveSubTripExecute(object param)
        {
             List<object> lst = param as List<object>;

             if (lst != null)
             {
                 TripVM trip = lst[0] as TripVM;
                 DataView view = lst[1] as DataView;

                 if (trip != null && view != null)
                 {
                     FuncExec.Execute(() =>
                         {
                             List<TripPrice> prices = trip.GetPricesFromMatrix(view.Table);
                             UtilManager.Instance.Client.StationOrderEdit(trip.trip, trip.RouteTowns.Select(t => t.town).ToArray());
                             UtilManager.Instance.Client.TripPriceSave(prices.ToArray());
                             this.UpdateData();
							 UtilManager.Instance.UpdateModifiedObject(this.ModifiedObj);
                             UtilManager.Instance.MessageProvider.ShowInformationWindow("Изменения были сохранены");
                         });
                 }
             }
        }

        /// <summary>
        /// проверка на возможность сохранения промежуточных пунктов
        /// </summary>
        /// <param name="param"></param>
        private bool SaveSubTripCanExecute(object param)
        {
            List<object> lst = param as List<object>;

            if (lst == null) return false;

            TripVM trip = lst[0] as TripVM;
            DataView view = lst[1] as DataView;

            if (trip == null || view == null) return false;

            return true;
        }

        /// <summary>
        /// сдвигает промежуточный пункт на одну позицию вверх (к городу отправления)
        /// </summary>
        /// <param name="param">изменяемый маршрут и выделенный город</param>
        private void MoveUpRouteTownExecute(object param)
        {
            List<object> lst = param as List<object>;

            TripVM trip = lst[0] as TripVM;
            DataGrid grid = lst[1] as DataGrid;
            TownVM town = grid.SelectedItem as TownVM;
            int nextIndex = trip.RouteTowns.IndexOf(town) - 1;

            // меняем местами текущий город и город выше
            TownVM temp = trip.RouteTowns[nextIndex];
            trip.RouteTowns[nextIndex] = town;
            trip.RouteTowns[nextIndex + 1] = temp;

            trip.GeneratePriceMatrix();
            grid.SelectedIndex = nextIndex + 1;
        }

        /// <summary>
        /// проверка на возможность менять порядок промежуточных пунктов
        /// </summary>
        /// <param name="param"></param>
        private bool MoveUpSubTripCanExecute(object param)
        {
            List<object> lst = param as List<object>;

            if (lst == null) return false;

            TripVM trip = lst[0] as TripVM;
            DataGrid grid = lst[1] as DataGrid;
            TownVM town = grid.SelectedItem as TownVM;

            if (trip == null || town == null) return false;

            // получаем индекс города, который находится выше текущего
            // например, Саратов - Балаково - Балашов - Москва, для Балашова индекс 3, а выше - 2
            int nextIndex = trip.RouteTowns.IndexOf(town) - 1;

            // промежуточный пункт не может быть городом отправления главного маршрута, как и городом прибытия
            // то есть для вышеизложенного примера Балашов не может иметь индекс выше 1 и ниже 3
            if (nextIndex <= 0 || nextIndex >= trip.RouteTowns.Count - 2) return false;

            return true;
        }

        /// <summary>
        /// проверка на возможность менять порядок промежуточных пунктов
        /// </summary>
        /// <param name="param"></param>
        private bool MoveDownSubTripCanExecute(object param)
        {
            List<object> lst = param as List<object>;

            if (lst == null) return false;

            TripVM trip = lst[0] as TripVM;
            DataGrid grid = lst[1] as DataGrid;
            TownVM town = grid.SelectedItem as TownVM;

            if (trip == null || town == null) return false;

            // получаем индекс города, который находится выше текущего
            // например, Саратов - Балаково - Балашов - Москва, для Балашова индекс 3, а выше - 4
            int nextIndex = trip.RouteTowns.IndexOf(town) + 1;

            // промежуточный пункт не может быть городом отправления главного маршрута, как и городом прибытия
            // то есть для вышеизложенного примера Балашов не может иметь индекс выше 1 и ниже 3
            if (nextIndex <= 1 || nextIndex >= trip.RouteTowns.Count - 1) return false;

            return true;
        }

            /// <summary>
        /// сдвигает промежуточный пункт на одну позицию вверх (к городу отправления)
        /// </summary>
        /// <param name="param">изменяемый маршрут и выделенный город</param>
        private void MoveDownRouteTownExecute(object param)
        {
            List<object> lst = param as List<object>;

            TripVM trip = lst[0] as TripVM;
            DataGrid grid = lst[1] as DataGrid;
            TownVM town = grid.SelectedItem as TownVM;


            int nextIndex = trip.RouteTowns.IndexOf(town) + 1;

            // меняем местами текущий город и город выше
            TownVM temp = trip.RouteTowns[nextIndex];
            trip.RouteTowns[nextIndex] = town;
            trip.RouteTowns[nextIndex - 1] = temp;

            trip.GeneratePriceMatrix();
            grid.SelectedIndex = nextIndex - 1;
        }

        #endregion

        /// <summary>
        /// обработчик команды для обновления списка объектов
        /// </summary>
        /// <param name="param"></param>
        protected override void UpdateData(object param)
        {
            this.UpdateData();
        }

        /// <summary>
        /// обновление маршрутов
        /// </summary>
        protected override void RefreshItems()
        {
            base.RefreshItems();
            this.OnPropertyChanged("Trips");
        }
    }
}
