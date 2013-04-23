using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VlastelinClient.Util;
using Vlastelin.Data.Model;
using System.Collections.ObjectModel;
using VlastelinClient.ViewModel.ObjectsViewModel;
using System.Windows.Input;
using VlastelinClient.Commands;
using VlastelinClient.ServiceReference1;
using System.ComponentModel;
using System.Windows.Controls;

namespace VlastelinClient.ViewModel
{
    /// <summary>
    /// вьюмодель для окна списка автобусов
    /// </summary>
    public class BusWindowVM : BaseWindowVM
    {        
        /// <summary>
        /// список автобусов
        /// </summary>
        public ObservableCollection<BusVM> Buses { get; private set; }

        /// <summary>
        /// список фирм-владельцев, используется для редактирования автобуса
        /// </summary>
        public ObservableCollection<OwnerVM> Owners { get; set; }
        
        /// <summary>
        /// используется для биндинга к свойствам добавляемого автобуса
        /// </summary>

        public BusWindowVM(Utilities utls, ObservableCollection<OwnerVM> owners) :
            base(utls)
        {
            this.Owners = owners;
            //this.Buses = SampleData.Buses;
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
            this.Buses = new ObservableCollection<BusVM>(this.utilite.Client.BusesGet(null).Select(bus => new BusVM(bus)));

            return this.Buses.Count;
        }

        /// <summary>
        /// получение объекта автобус из данных формы
        /// </summary>
        /// <param name="fields">список полей</param>
        /// <returns>получаемый автобус</returns>
        private BusVM GetBusFromList(IList<object> fields)
        {
            // 6 - количество полей в классе автобус + изменяемый автобус
            if (fields == null || fields.Count != 6)
            {
                return null;
            }

            BusVM bus = new BusVM();
            BusVM oldBus;

            // формирование объекта автобус из данных формы
            oldBus = fields[0] as BusVM;
            bus.Manufacter = fields[1].ToString();
            bus.Model = fields[2].ToString();
            bus.RegNumber = fields[3].ToString();
            bus.PassengersCount = int.Parse(fields[4].ToString());
            bus.Owner = fields[5] as OwnerVM;
            bus.bus.Id = oldBus == null ? 0 : oldBus.bus.Id;

            this.flagTypeEdit = oldBus != null;

            return bus;
        }

        /// <summary>
        /// обработчик команды удаления автобуса
        /// </summary>
        /// <param name="param">автобус</param>
        protected override object DeleteItem(object param)
        {
            BusVM bus = param as BusVM;
            if (bus != null)
            {
                this.Buses.Remove(bus);
                this.utilite.Client.BusDelete(bus.bus);
            }
            return bus;
        }

        /// <summary>
        /// обработчик команды добавления автобуса
        /// </summary>
        /// <param name="param">не используется</param>
        protected override void AddItem(object param)
        {
            ListBox listBox = param as ListBox;
            if (listBox == null)
            {
                throw new ArgumentException("Ошибка при добавлении автобуса");
            }

            listBox.SelectedIndex = -1;
        }

        /// <summary>
        /// обработчик команды для изменения автобуса
        /// </summary>
        /// <param name="param"></param>
        protected override object EditItem(object param)
        {
            BusVM bus = this.GetBusFromList(param as IList<object>);
            
            if (bus == null)
            {
                throw new ArgumentException("Не удалось прочитать данные автобуса из формы");
            }

            if (this.flagTypeEdit)
            {
                base.EditItem(param);

                foreach (BusVM bs in this.Buses)
                {
                    if (bs.Equals(bus))
                    {
                        bs.Copy(bus);
                    }
                }

                this.utilite.Client.BusEdit(bus.bus);
            }
            else
            {
                long id = this.utilite.Client.BusAdd(bus.bus);
                bus.bus.Id = id;
                this.Buses.Add(bus);
            }
            return bus;
        }

        /// <summary>
        /// обработчик команды для обновления списка объектов
        /// </summary>
        /// <param name="param"></param>
        protected override void UpdateExecute(object param)
        {
            base.UpdateExecute(param);
            this.OnPropertyChanged("Buses");
        }
    }
}
