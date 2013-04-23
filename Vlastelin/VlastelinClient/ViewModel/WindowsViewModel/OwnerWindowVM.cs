using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VlastelinClient.ViewModel.ObjectsViewModel;
using Vlastelin.Data.Model;
using System.ComponentModel;
using System.Collections.ObjectModel;
//using VlastelinClient.ServiceReference1;
using VlastelinClient.Util;
using Vlastelin.Common;
using System.Windows.Controls;
using System.Windows.Input;
using VlastelinClient.Commands;

namespace VlastelinClient.ViewModel
{
    public class OwnerWindowVM : BaseWindowVM
    {
        /// <summary>
        /// список владельцев
        /// </summary>
        public ObservableCollection<OwnerVM> Owners { get; private set; }

        /// <summary>
        /// список типов комиссии (для выпдающего списка)
        /// </summary>
        public ICollection<String> FeeTypesList
        {
            get
            {
                return Ct.GetEnumDescriptionValues(typeof(FeeTypes));
            }
        }

        /// <summary>
        /// список позиций управляющего (для выпадающего списка)
        /// </summary>
        public IList<DirPosition> DirPositionsList { get; private set; }

        /// <summary>
        /// вьюмодель водителей (для списка доверенностей)
        /// </summary>
        public DriverWindowVM driverVM { get; set; }

        public IEnumerable<DriverVM> Drivers
        {
            get 
            {
                return driverVM != null ? driverVM.Drivers.OrderBy(d => d.Surname) : null ;
            }
        }

        public ICommand AddAuthorityCommand
        {
            get
            {
                return new RelayCommand(this.AddAuthorityExecute, AddAuthorityCanExecute);
            }
        }

        public ICommand DeleteAuthorityCommand
        {
            get
            {
				return new RelayCommand(this.DeleteAuthorityExecute, this.DeleteAuthorityCanExecute);
            }
        }

        public OwnerWindowVM()
        {
        }

        /// <summary>
        /// получение списка владельцев из базы данных и приведение его к типу вьюмодели
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.DirPositionsList = new List<DirPosition>() 
            { 
                new DirPosition() { Id = 1, Name = Ct.DirPositionIP },
                new DirPosition() { Id = 2, Name = Ct.DirPositionDirector },
            };
            this.ModifiedObj = ModifiedObjects.Owners;
        }

		/// <summary>
		/// загрузка данных из базы
		/// </summary>
		public override void LoadData()
		{
			// если изменились перевозчики или доверенности, то загружаем
			if (UtilManager.Instance.NeedLoad(this.ModifiedObj) || UtilManager.Instance.NeedLoad(ModifiedObjects.DriverAuthorities))
			{
				this.LoadDataExecute();
			}
		}

        /// <summary>
        /// загрузка данных из базы
        /// </summary>
        public override void LoadDataExecute()
        {
            base.LoadDataExecute();
            this.Owners = new ObservableCollection<OwnerVM>(UtilManager.Instance.Client.OwnersGet(null).Select(owner => new OwnerVM(owner)));
        }

        /// <summary>
        /// получение объекта владелец из данных формы
        /// </summary>
        /// <param name="fields">список полей</param>
        /// <returns>получаемый владелец</returns>
        private BaseItemVM GetItemFromList(IList<object> fields)
        {
            // 15 - количество полей в классе владелец + изменяемый владелец
            if (fields == null || fields.Count != 15)
            {
                throw new ArgumentException("Не удалось получить владельца из данных формы");
            }

            OwnerVM item = new OwnerVM();
            OwnerVM oldItem;

            // формирование объекта автобус из данных формы
            oldItem = fields[0] as OwnerVM;
            item.Name = fields[1].ToString();
            item.NumSv = fields[2].ToString();
            item.OGRN = fields[3].ToString();
            item.DocNum = fields[4].ToString();
            if (fields[5] != null) item.DocDate = (DateTime)fields[5];
            if (fields[6] != null) item.DocEndDate = (DateTime)fields[6];
            item.INN = fields[7].ToString();
            item.Address = fields[8].ToString();
            item.FeeAmount = !String.IsNullOrWhiteSpace(fields[9].ToString()) ? double.Parse(fields[9].ToString()) : 0;
            item.FeeType = fields[10] != null ? (FeeTypes)Ct.GetEnumFromDescription(typeof(FeeTypes), fields[10].ToString()) : FeeTypes.FixedAmount;
            item.DirName = fields[11].ToString();
            item.DirSurname = fields[12].ToString();
            item.DirPatronymic = fields[13].ToString();
            item.DirPosition = fields[14] as DirPosition;
            
            item.owner.Id = oldItem == null ? 0 : oldItem.owner.Id;

            this.flagTypeEdit = oldItem != null;

            return item;
        }

        /// <summary>
        /// обработчик команды для удаления владельца
        /// </summary>
        /// <param name="param">объект владельца</param>
        protected override void DeleteItem(object param)
        {
            OwnerVM item = param as OwnerVM;
            if (item != null)
            {
                UtilManager.Instance.Client.OwnerDelete(item.owner);
                this.Owners.Remove(item);
                this.OnChangeItem(this.Owners.FirstOrDefault());
            }
        }

        /// <summary>
        /// обработчик команды для изменения владельца
        /// </summary>
        /// <param name="param">объект владельца</param>
        protected override void EditItem(object param)
        {
            OwnerVM item = this.GetItemFromList(param as IList<object>) as OwnerVM;

            if (item == null)
            {
                throw new ArgumentException("Не удалось прочитать данные владельца из формы");
            }

            if (this.flagTypeEdit)
            {
                UtilManager.Instance.Client.OwnerEdit(item.owner);

                base.EditItem(param);

                foreach (OwnerVM im in this.Owners)
                {
                    if (im.EqualsById(item))
                    {
                        im.CopyFrom(item);
                    }
                }
            }
            else
            {
                item.owner.authorities = new List<DriverAuthority>();
                item.Authorities = new ObservableCollection<DriverAuthority>();
                
                item.owner.Id = UtilManager.Instance.Client.OwnerAdd(item.owner);
                this.Owners.Add(item);
            }
            this.OnChangeItem(item);
        }

		/// <summary>
		/// проверка удаления доверенности у водителя
		/// </summary>
		/// <param name="param">удаляемая доверенность и изменяемый водитель</param>
		private bool DeleteAuthorityCanExecute(object param)
		{
			List<object> lst = param as List<object>;
			if (lst == null) return false;

			OwnerVM owner = lst[0] as OwnerVM;
			DriverAuthority auth = lst[1] as DriverAuthority;

			return 
                owner != null && // перевозчик задан
                auth != null && // выбрана доверенность
                UtilManager.Instance.StateManager.IsServerEnabled; // сервер доступен
		}

        /// <summary>
        /// обработчик команды удаления доверенности у владельца
        /// </summary>
        /// <param name="param">удаляемая доверенность и изменяемый владелец</param>
        private void DeleteAuthorityExecute(object param)
        {
            List<object> lst = param as List<object>;

            OwnerVM owner = lst[0] as OwnerVM;
            DriverAuthority auth = lst[1] as DriverAuthority;

			FuncExec.Execute(() =>
			{
				UtilManager.Instance.Client.DriverAuthorityDelete((int)auth.Id);
				owner.RemoveAuthority(auth);

				UtilManager.Instance.UpdateModifiedObject(ModifiedObjects.DriverAuthorities);
				UtilManager.Instance.ModifiedTables[ModifiedObjects.Drivers] = DateTime.MinValue;
			});
        }

        /// <summary>
        /// обработчик команды добавления доверенности у владельца
        /// </summary>
        /// <param name="param">добавляемая доверенность</param>
        private void AddAuthorityExecute(object param)
        {
            List<object> lst = param as List<object>;

            DriverAuthority auth = new DriverAuthority();
            OwnerVM owner = lst[0] as OwnerVM;

			FuncExec.Execute(() =>
			{
				auth.Owner = owner != null ? owner.owner : null;
				auth.Driver = lst[1] != null ? (lst[1] as DriverVM).driver : null;
				auth.Number = lst[2].ToString();
				auth.Date = DateTime.Parse(lst[3].ToString());

				auth.Id = UtilManager.Instance.Client.DriverAuthorityAdd((int)auth.Driver.Id, (int)auth.Owner.Id, auth.Number, auth.Date);
				owner.AddAuthority(auth);

				UtilManager.Instance.UpdateModifiedObject(ModifiedObjects.DriverAuthorities);
				UtilManager.Instance.ModifiedTables[ModifiedObjects.Drivers] = DateTime.MinValue;
			});
        }

        /// <summary>
        /// проверка удаления доверенности у водителя
        /// </summary>
        /// <param name="param">удаляемая доверенность и изменяемый водитель</param>
        private bool AddAuthorityCanExecute(object param)
        {
            return UtilManager.Instance.StateManager.IsServerEnabled; // сервер доступен
        }

        /// <summary>
        /// обработчик команды для обновления списка объектов
        /// </summary>
        /// <param name="param"></param>
        protected override void UpdateData(object param)
        {
            this.LoadData();
            this.driverVM.LoadData();
        }

        /// <summary>
        /// обновление интерфейса
        /// </summary>
        protected override void RefreshItems()
        {
            base.RefreshItems();
            this.OnPropertyChanged("Drivers");
            this.OnPropertyChanged("Owners");
        }
    }
}
