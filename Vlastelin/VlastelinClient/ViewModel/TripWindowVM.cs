using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using VlastelinClient.ViewModel.ObjectsViewModel;
using Vlastelin.Data.Model;
using VlastelinClient.ServiceReference1;
using VlastelinClient.Util;
using System.ComponentModel;
using System.Windows.Controls;

namespace VlastelinClient.ViewModel
{
    public class TripWindowVM : BaseWindowVM
    {
        /// <summary>
        /// список маршрутов
        /// </summary>
        public ObservableCollection<TripVM> Trips { get; private set; }

        /// <summary>
        /// список городов, используется для выбора пункта прибытия и отъезда
        /// </summary>
        public ObservableCollection<TownVM> Towns { get; private set; }

        public TripWindowVM(Utilities utls, ObservableCollection<TownVM> towns) : 
            base(utls)
        {
            this.Towns = towns;
            //this.Trips = SampleData.Trips;
        }

        /// <summary>
        /// получение списка маршрутов из базы данных и приведение его к типу вьюмодели
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.LoadDataWithLog();
        }

        /// <summary>
        /// загрузка данных из базы
        /// </summary>
        public override object LoadData()
        {
            var lst = this.utilite.Client.TripsGet(null);
            this.Trips = new ObservableCollection<TripVM>(lst.Select(item => new TripVM(item)));

            return this.Trips.Count;
        }

        /// <summary>
        /// получение объекта маршрут из данных формы
        /// </summary>
        /// <param name="fields">список полей</param>
        /// <returns>получаемый маршрут</returns>
        private BaseItemVM GetItemFromList(IList<object> fields)
        {
            // 6 - количество полей в классе маршрут + изменяемый маршрут
            if (fields == null || fields.Count != 6)
            {
                throw new ArgumentException("Не удалось получить маршрут из данных формы");
            }

            TripVM item = new TripVM();
            TripVM oldItem;

            // формирование объекта автобус из данных формы
            oldItem = fields[0] as TripVM;
            item.Name = fields[1].ToString();
            item.DepartureTown = fields[2] as TownVM;
            item.ArrivalTown = fields[3] as TownVM;
            item.Price = double.Parse(fields[4].ToString());
            item.Description = fields[5].ToString();
            item.trip.Id = oldItem == null ? 0 : oldItem.trip.Id;

            this.flagTypeEdit = oldItem != null;

            return item;
        }

        /// <summary>
        /// обработчик команды для удаления маршрута
        /// </summary>
        /// <param name="param">объект маршрута</param>
        protected override object DeleteItem(object param)
        {
            TripVM item = param as TripVM;
            if (item != null)
            {
                this.Trips.Remove(item);
                this.utilite.Client.TripDelete(item.trip);
            }

            return item;
        }

        /// <summary>
        /// обработчик команды для добавления маршрута
        /// </summary>
        /// <param name="param">называние маршрута</param>
        protected override void AddItem(object param)
        {
            ListBox listBox = param as ListBox;
            if (listBox == null)
            {
                throw new ArgumentException("Ошибка при добавлении маршрута");
            }

            listBox.SelectedIndex = -1;
        }

        /// <summary>
        /// обработчик команды для изменения маршрута
        /// </summary>
        /// <param name="param">объект маршрута</param>
        protected override object EditItem(object param)
        {
            TripVM item = this.GetItemFromList(param as IList<object>) as TripVM;

            if (item == null)
            {
                throw new ArgumentException("Не удалось прочитать данные маршрута из формы");
            }

            if (this.flagTypeEdit)
            {
                base.EditItem(param);

                foreach (TripVM im in this.Trips)
                {
                    if (im.Equals(item))
                    {
                        im.Copy(item);
                    }
                }

                this.utilite.Client.TripEdit(item.trip);
            }
            else
            {
                long id = this.utilite.Client.TripAdd(item.trip);
                item.trip.Id = id;
                this.Trips.Add(item);
            }
            return item;
        }
    }
}
