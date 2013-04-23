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
//using VlastelinClient.ServiceReference1;
using System.ComponentModel;
using System.Windows.Controls;
using Vlastelin.Common;

namespace VlastelinClient.ViewModel
{
    /// <summary>
    /// вьюмодель для окна списка автобусов
    /// </summary>
    public class BusWindowVM : BaseWindowVM
    {
        /// <summary>
        /// вьюмодель окна автобусов для работы с списком автобусов
        /// </summary>
        private OwnerWindowVM ownerWindowVM;

        /// <summary>
        /// список автобусов
        /// </summary>
        public ObservableCollection<BusVM> Buses { get; private set; }

        /// <summary>
        /// список фирм-владельцев, используется для редактирования автобуса
        /// </summary>
        public IEnumerable<OwnerVM> Owners
        {
            get
            {
                return this.ownerWindowVM.Owners.OrderBy(ow => ow.Name);
            }
        }
        
        /// <summary>
        /// используется для биндинга к свойствам добавляемого автобуса
        /// </summary>

        public BusWindowVM(OwnerWindowVM ownerVM)
        {
            this.ownerWindowVM = ownerVM;
        }

        /// <summary>
        /// инициализация начальных данных
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.ModifiedObj = ModifiedObjects.Buses;
        }

        /// <summary>
        /// загрузка данных из базы
        /// </summary>
        public override void LoadDataExecute()
        {
            base.LoadDataExecute();
            this.Buses = new ObservableCollection<BusVM>(UtilManager.Instance.Client.BusesGet(null).Select(bus => new BusVM(bus)));           
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
            bus.PassengersCount = !String.IsNullOrWhiteSpace(fields[4].ToString()) ? int.Parse(fields[4].ToString()) : 0;
            bus.Owner = fields[5] as OwnerVM;
            bus.bus.Id = oldBus == null ? 0 : oldBus.bus.Id;

            this.flagTypeEdit = oldBus != null;

            return bus;
        }

        /// <summary>
        /// обработчик команды удаления автобуса
        /// </summary>
        /// <param name="param">автобус</param>
        protected override void DeleteItem(object param)
        {
            BusVM bus = param as BusVM;
            if (bus != null)
            {
                UtilManager.Instance.Client.BusDelete(bus.bus); 
                this.Buses.Remove(bus);
                this.OnChangeItem(this.Buses.FirstOrDefault());
            }
        }

        /// <summary>
        /// обработчик команды добавления автобуса
        /// </summary>
        /// <param name="param">не используется</param>
        protected override void AddItem(object param)
        {
            ListBox listBox = param as ListBox;
            listBox.SelectedIndex = -1;
        }

        /// <summary>
        /// обработчик команды для изменения автобуса
        /// </summary>
        /// <param name="param"></param>
        protected override void EditItem(object param)
        {
            BusVM bus = this.GetBusFromList(param as IList<object>);
            
            if (bus == null)
            {
                throw new ArgumentException("Не удалось прочитать данные автобуса из формы");
            }

            if (this.flagTypeEdit)
            {
                UtilManager.Instance.Client.BusEdit(bus.bus);

                base.EditItem(param);

                foreach (BusVM bs in this.Buses)
                {
                    if (bs.EqualsById(bus))
                    {
                        bs.CopyFrom(bus);
                    }
                }
            }
            else
            {
                bus.bus.Id = UtilManager.Instance.Client.BusAdd(bus.bus);
                this.Buses.Add(bus);
            }
            this.OnChangeItem(bus);
        }

        /// <summary>
        /// обработчик команды для обновления списка объектов
        /// </summary>
        /// <param name="param"></param>
        protected override void UpdateData(object param)
        {
            this.LoadData();
            this.ownerWindowVM.LoadData();
        }

        /// <summary>
        /// обновление интерфейса
        /// </summary>
        protected override void RefreshItems()
        {
            base.RefreshItems();
            this.OnPropertyChanged("Buses");
            this.OnPropertyChanged("Owners");
        }
    }
}
