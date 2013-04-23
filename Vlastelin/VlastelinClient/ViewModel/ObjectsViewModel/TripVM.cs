using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;
using System.Collections.ObjectModel;
using System.Data;
using VlastelinClient.Util;
using Vlastelin.Common;

namespace VlastelinClient.ViewModel.ObjectsViewModel
{
    /// <summary>
    /// вью модель для класса поездка
    /// </summary>
    public class TripVM : BaseItemVM
    {
        public static Dictionary<TripScheduleType, String> TripSheduleTypesMapping;
        
        /// <summary>
        /// объект класса поездка из модели
        /// </summary>
        public Trip trip
        {
            get
            {
                return this.item as Trip;
            }
            set
            {
                this.item = value;
            }
        }

        /// <summary>
        /// название маршрута
        /// </summary>
        public String Name
        {
            get
            {
                return this.trip.Name;
            }
            set
            {
                this.trip.Name = value;
                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// навзание маршрута
        /// </summary>
        public String NameString
        {
            get 
            {
                return this.trip.NameString;
            }
        }

		/// <summary>
		/// для сортировки
		/// </summary>
		public String SortField
		{
			get
			{
				return this.Departure.Name;
			}
		}

        /// <summary>
        /// список цен на маршруты (включает город отправления, прибытия)
        /// </summary>
        private ObservableCollection<TripPriceVM> _prices;
        public ObservableCollection<TripPriceVM> Prices 
        {
            get
            {
                return this._prices;
            }
            set
            {
                this._prices = value;
                this.OnPropertyChanged("Prices");
            }
        }

        /// <summary>
        /// Город отправления
        /// </summary>
        public TownVM Departure
        {
            get { return new TownVM(this.trip.Departure); }
            set
            {
                this.trip.Departure = value != null ? value.town : null;
				this.OnPropertyChanged("Departure");
				this.OnPropertyChanged("NameString");
            }
        }

        /// <summary>
        /// Город прибытия
        /// </summary>
        public TownVM Arrival
        {
            get { return new TownVM(this.trip.Arrival); }
            set
            {
                this.trip.Arrival = value != null ? value.town : null;
				this.OnPropertyChanged("Arrival");
				this.OnPropertyChanged("NameString");
            }
        }

        /// <summary>
        /// Описание
        /// </summary>
        public string Description 
        {
            get
            {
                return this.trip.Description;
            }
            set
            {
                this.trip.Description = value;
				this.OnPropertyChanged("Description");
            } 
        }

		private bool _hasSchedule = false;
		public bool HasSchedule
		{
			get
			{
				return this._hasSchedule;
			}
			set
			{
				this._hasSchedule = value;
				this.OnPropertyChanged("HasSchedule");
			}
		}

        // используется для матрицы цен - список городов (промежуточных пунктов) и список цен
        public ObservableCollection<TownVM> RouteTowns { get; set; }

        // матрица цен
        public DataTable PriceMatrix { get; set; }

        static TripVM()
        {
        }

        public TripVM()
        {
            this.trip = new Trip();
        }

        public TripVM(Trip trp)
        {
            this.trip = trp;
        }

        public override string ToString()
        {
            return this.trip.ToString();
        }

		/// <summary>
		/// устанавливает флаг - есть ли расписание у маршрута или нет
		/// </summary>
		/// <param name="schedules"></param>
		public void SetHasSchedule(IEnumerable<StationScheduleVM> schedules)
		{
			if (schedules == null) this.HasSchedule = false;
			this.HasSchedule = schedules.Count(sh => sh.TS.Trip.trip.Equals(this.trip)) > 0;
		}

        /// <summary>
        /// полное клонирование маршрута
        /// </summary>
        /// <param name="itm">присваиваемый маршрут</param>
        public override void CopyFrom(BaseItemVM itm)
        {
            TripVM item = itm as TripVM;

            if (item != null)
            {
                base.CopyFrom(itm);
                this.Departure.CopyFrom(item.Departure);
                this.Arrival.CopyFrom(item.Arrival);
                this.Refresh();
            }
        }

        /// <summary>
        /// условия фильтрации по маршруту
        /// </summary>
        /// <param name="dep">пункт отправления</param>
        /// <param name="arr">пункт прибытия</param>
        /// <returns>тру - услвоие соблюдено</returns>
        public bool FilterCondition(String dep, String arr)
        {
            return this.Departure != null && this.Arrival != null ? this.Departure.Name.ToUpper().Contains(dep.ToUpper()) && this.Arrival.Name.ToUpper().Contains(arr.ToUpper()) : false;
        }

        #region работа с подмаршрутами

        /// <summary>
        /// получаем список с ценами на подмаршруты и устанавливаем его для каждого маршрута
        /// </summary>
        /// <param name="prices"></param>
        public void SetTripPrices(IEnumerable<TripPriceVM> prices)
        {
            this.Prices = new ObservableCollection<TripPriceVM>(prices.Where(pr => this.RouteTowns.Contains(pr.Arrival) || this.RouteTowns.Contains(pr.Departure)));
            this.OnPropertyChanged("Prices");
        }

        /// <summary>
        /// получает список промежуточных пунктов из списка подмаршрутов
        /// </summary>
        /// <param name="trip">маршнут</param>
        /// <returns>список городов</returns>
        public void SetRouteTowns(IEnumerable<StationOrder> stationOrderList)
        {
            this.RouteTowns = new ObservableCollection<TownVM>(stationOrderList.Where(st => st.Trip.Equals(this.trip)).OrderBy(s => s.Order).Select(stt => new TownVM(stt.Town)));
			this.OnPropertyChanged("RouteTowns");
        }

        /// <summary>
        /// добавляет промежуточный город и соответствующие ему подмаршруты
        /// </summary>
        /// <param name="trip">маршрут</param>
        /// <param name="town">город</param>
        public void AddRouteTown(TownVM town)
        {
            // если новый пункт не содержиться в текущем маршруте, вставляем его в конец маршрута, но до пункта прибытия
            if (this.RouteTowns != null && !this.RouteTowns.Contains(town))
            {
                this.RouteTowns.Insert(this.RouteTowns.Count - 1, town);
            }
        }

        /// <summary>
        /// удаляет промежуточный пнукт
        /// </summary>
        /// <param name="town">удаляемый город</param>
        public void DeleteRouteTown(TownVM town)
        {
            this.RouteTowns.Remove(town);
        }

        /// <summary>
        /// получает цену подмрашрута по городу прибытия и отправления
        /// </summary>
        /// <param name="arr">город прибытия</param>
        /// <param name="dep">город отправления</param>
        /// <returns>маршрут или нулл, если маршрут не найден</returns>
        private TripPriceVM GetTripPrice(TownVM dep, TownVM arr)
        {
            // поулчаем подмаршрут только при совпадении ИД городов прибытия и отправления
            return this.Prices.FirstOrDefault(sb => sb.Arrival.Equals(arr) && sb.Departure.Equals(dep));
        }

        #endregion

        #region Работа с матрицей цен

        /// <summary>
        /// генерирует матрицу цен из текущего списка маршрутов
        /// </summary> 
        public void GeneratePriceMatrix()
        {
            DataTable matrix = null;
            
            // проверяем на количество пунктов в маршруте
            int c = this.RouteTowns.Count;
            if (c < 2) throw new ArgumentException("У маршрута должен быть минимум один подмаршрут");

            // формируем столбцы матрицы из названия промежуточных пунктов
            matrix = new DataTable();
            foreach (String town in this.RouteTowns.Where(t => !t.Equals(this.Departure)).Select(t => t.Name))
            {
                matrix.Columns.Add(town);
            }

            // формируем матрицу и значения цен в ней
            // матрица имеет следующий вид:
            // последняя строка не нужна, поскольку пункт прибытия не может быть промежуточным пунктом, из которого начинается другой подмаршрут
            // пример: Москва - Питер - Саратов, нет маршрутов из Саратова
            // первый столбец также исключается, так как нет маршрутов, где конечный пункт - пункт отправления главного маршрута
            // по предыдущему примеру: нет подмаршрутов типа Город- Москва
            // заголовки строк формируются в отдельном свойстве (они используются только для интерфейса, но не для логики матрицы)

            //  Москва  |  Питер  |  Саратов  | 
            //------------------------------------------
            //   -          500       2000
            //   -           -        1500

            TripPriceVM pr = new TripPriceVM();
            DataRow datarow;

            for (int i = 0; i < c - 1; i++)
            {
                datarow = matrix.NewRow();
                for (int j = 1; j < c; j++)
                {
                    // элементы под главной диагональю должны быть исключены, так как не может быть обратных маршрутов или маршрутов типа Москва - Москва
                    if (i >= j) datarow[j - 1] = "-";
                    else
                    {
                        // ищем подмаршрут, соответствующий текущей ячейке матрицы
                        // строка - город отправления, столбец - город прибытия
                        pr = this.GetTripPrice(this.RouteTowns[i], this.RouteTowns[j]);
                        
                        // если субтрип не найден, записываем ноль, иначе записываем цену этого подмаршрута
                        if (pr == null) datarow[j - 1] = String.Empty; else datarow[j - 1] = pr.Price;
                    }
                }
                datarow.RowError = this.RouteTowns[i].Name;
                matrix.Rows.Add(datarow);
            }
            this.PriceMatrix = matrix;
            this.OnPropertyChanged("PriceMatrix");
        }

        /// <summary>
        /// обновляет список маршрутов из матрицы цен, полученной из таблицы
        /// </summary>
        /// <param name="matrix">матрица цен</param>
        public List<TripPrice> GetPricesFromMatrix(DataTable matrix)
        {
            this.PriceMatrix = matrix;
          
            // количество строк равно количеству городов минус один (пункт назначения)
            // индексы столбцов считаем на один больше и берем только элементы справа от главной диагонали

            int c = this.RouteTowns.Count;
            decimal price = 0;
            TripPriceVM pr = new TripPriceVM();
            List<TripPrice> prices = new List<TripPrice>();
            String cell;

            for (int i = 0; i < c - 1; i++)
            {
                for (int j = i; j < c - 1; j++)
                {
                    // читаем цену из ячейки матрицы и получаем соответствующий субтрип
                    cell = matrix.Rows[i][j].ToString();
                    pr = this.GetTripPrice(this.RouteTowns[i], this.RouteTowns[j + 1]);
                    if (!String.IsNullOrWhiteSpace(cell))
                    {
                        if (!decimal.TryParse(cell, out price))
                        {
                            throw new IncorrectDataException(String.Format("Цена должна быть числом. Неверное значение: {0}", cell));
                        }
                    }
                        else price = 0;

                    // если субтрип найден, то присваиваем цену и помечаем его как измененный
                    if (pr != null)
                    {
                        pr.Price = price;
                        prices.Add(pr.tripPrice);
                    }
                    // иначе субтип является добавленным, помечаем его тип как добавленный
                    else
                        if (price > 0)
                        {
                            // формируем новый подмаршрут исходя из ячейки матрицы и добавляем в субтрипы
                            prices.Add(
                                new TripPrice()
                                {
                                    Arrival = this.RouteTowns[j + 1].town,
                                    Departure = this.RouteTowns[i].town,
                                    Price = price
                                });
                        }
                    }
                }
            return prices;
        }

        /// <summary>
        /// удаляет из матрицы цен город
        /// </summary>
        /// <param name="town">удаляемый город</param>
        public void DeleteMatrixTown(TownVM town)
        {
            if (town != null)
            {
                DataTable matrix = new DataTable();
                // удалить можно только промежуточные пункты, но не город отправления или прибытия
                if (this.RouteTowns.Count > 2)
                {
                    matrix = this.PriceMatrix.Copy();
                    
                    // удаляем столбец и строку с выбранным городом
                    matrix.Columns.Remove(town.Name);
                    foreach (DataRow row in matrix.Rows)
                    {
                        if (row.RowError == town.Name)
                        {
                            matrix.Rows.Remove(row);
                            break;
                        }
                    }
                }
                this.PriceMatrix = matrix;
                this.OnPropertyChanged("PriceMatrix");
            }
        }

        #endregion
    }
}
