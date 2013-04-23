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
    /// <summary>
    /// вьюмодель для расписания
    /// </summary>
    public class TripSheduleWindowVM : BaseWindowVM
    {
        /// <summary>
        /// расписания
        /// </summary>
        public ObservableCollection<TripSheduleVM> Shedules { get; private set; }

        /// <summary>
        /// список автобусов, используется для добавления и редактирования расписания
        /// </summary>
        public ObservableCollection<BusVM> Buses { get; private set; }

        /// <summary>
        /// список расписаниеов, используется для редактирования расписания
        /// </summary>
        public ObservableCollection<TripVM> Trips { get; private set; }

        public TripSheduleWindowVM(Utilities utls, ObservableCollection<TripVM> trips, ObservableCollection<BusVM> buses) :
            base(utls)
        {
            this.Trips = trips;
            this.Buses = buses;

            //this.Shedules = SampleData.Shedules;
        }

        /// <summary>
        /// инициализация начальных данных
        /// </summary>
        protected override void Init()
        {
            base.Init();

        }

        /// <summary>
        /// загрузка данных из базы
        /// </summary>
        public override object LoadData()
        {
            base.LoadData();
            this.Shedules = new ObservableCollection<TripSheduleVM>(this.utilite.Client.TripScheduleGet(null).Select(shedule => new TripSheduleVM(shedule)));

            return this.Shedules.Count;
        }

        /// <summary>
        /// получение объекта расписание из данных формы
        /// </summary>
        /// <param name="fields">список полей</param>
        /// <returns>получаемый расписание</returns>
        private BaseItemVM GetItemFromList(IList<object> fields)
        {
            // 4 - количество полей в классе расписание + изменяемый расписание
            if (fields == null || fields.Count != 4)
            {
                throw new ArgumentException("Не удалось получить расписание из данных формы");
            }

            TripSheduleVM item = new TripSheduleVM();
            TripSheduleVM oldItem;

            // формирование объекта автобус из данных формы
            oldItem = fields[0] as TripSheduleVM;
            item.Trip = fields[1] as TripVM;
            item.Bus = fields[2] as BusVM;
            item.DepartureTime = DateTime.Parse(fields[3].ToString());
            item.shedule.Id = oldItem == null ? 0 : oldItem.shedule.Id;

            this.flagTypeEdit = oldItem != null;

            return item;
        }

        /// <summary>
        /// обработчик команды для удаления расписания
        /// </summary>
        /// <param name="param">объект расписания</param>
        protected override object DeleteItem(object param)
        {
            TripSheduleVM item = param as TripSheduleVM;
            if (item != null)
            {
                this.Shedules.Remove(item);
                this.utilite.Client.TripScheduleDelete(item.shedule);
            }

            return item;
        }

        /// <summary>
        /// обработчик команды для добавления расписания
        /// </summary>
        /// <param name="param">называние расписания</param>
        protected override void AddItem(object param)
        {
            ListBox listBox = param as ListBox;
            if (listBox == null)
            {
                throw new ArgumentException("Ошибка при добавлении расписания");
            }

            listBox.SelectedIndex = -1;
        }

        /// <summary>
        /// обработчик команды для изменения расписания
        /// </summary>
        /// <param name="param">объект расписания</param>
        protected override object EditItem(object param)
        {
            TripSheduleVM item = this.GetItemFromList(param as IList<object>) as TripSheduleVM;

            if (item == null)
            {
                throw new ArgumentException("Не удалось прочитать данные расписания из формы");
            }

            if (this.flagTypeEdit)
            {
                base.EditItem(param);

                foreach (TripSheduleVM im in this.Shedules)
                {
                    if (im.Equals(item))
                    {
                        im.Copy(item);
                    }
                }

                this.utilite.Client.TripScheduleEdit(item.shedule);
            }
            else
            {
                long id = this.utilite.Client.TripScheduleAdd(item.shedule);
                item.shedule.Id = id;
                this.Shedules.Add(item);
            }
            return item;
        }
    }
}
