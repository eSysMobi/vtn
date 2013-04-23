using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VlastelinClient.ViewModel.ObjectsViewModel;
using System.Collections.ObjectModel;
using Vlastelin.Data.Model;
//using VlastelinClient.ServiceReference1;
using VlastelinClient.Util;
using System.ComponentModel;
using Vlastelin.Common;
using System.Windows.Controls;
using System.Windows.Input;
using VlastelinClient.Commands;

namespace VlastelinClient.ViewModel
{
    public class DriverWindowVM : BaseWindowVM
    {
        /// <summary>
        /// вьюмодель окна автобусов для работы с списком автобусов
        /// </summary>
        private OwnerWindowVM ownerWindowVM;

        /// <summary>
        /// список водителей
        /// </summary>
        public ObservableCollection<DriverVM> Drivers { get; private set; }

        /// <summary>
        /// список фирм-владельцев, используется для редактирования доверенностей
        /// </summary>
        public IEnumerable<OwnerVM> Owners
        {
            get
            {
				return this.ownerWindowVM.Owners.OrderBy(ow => ow.Name);
            }
        }

        public ICommand AddAuthorityCommand
        {
            get
            {
                return new RelayCommand(this.AddAuthorityExecute, this.AddAuthorityCanExecute);
            }
        }

        public ICommand DeleteAuthorityCommand
        {
            get
            {
                return new RelayCommand(this.DeleteAuthorityExecute, this.DeleteAuthorityCanExecute);
            }
        }
       
        public DriverWindowVM(OwnerWindowVM ownerVM)
        {
            this.ownerWindowVM = ownerVM;
        }

        /// <summary>
        /// инициализация начальных параметров
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.ModifiedObj = ModifiedObjects.Drivers;
        }

		/// <summary>
		/// загрузка данных из базы
		/// </summary>
		public override void LoadData()
		{
			// если изменились водители или доверенности, то загружаем
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
            this.Drivers = new ObservableCollection<DriverVM>(UtilManager.Instance.Client.DriversGet(null).Select(driver => new DriverVM(driver)));
        }

        /// <summary>
        /// получение объекта водитель из данных формы
        /// </summary>
        /// <param name="fields">список полей</param>
        /// <returns>получаемый водитель</returns>
        private BaseItemVM GetItemFromList(IList<object> fields)
        {
            // 8 - количество полей в классе водитель + изменяемый водитель
            if (fields == null || fields.Count != 8)
            {
                throw new ArgumentException("Не удалось получить водителя из данных формы");
            }

            DriverVM item = new DriverVM();
            DriverVM oldItem;

            // формирование объекта автобус из данных формы
            oldItem = fields[0] as DriverVM;
            item.Name = fields[1].ToString();
            item.Surname = fields[2].ToString();
            item.Patronymic= fields[3].ToString();
            item.PassportSer = fields[4].ToString();
            item.PassportNum = fields[5].ToString();
            item.PassportIssuer = fields[6].ToString();
            if (fields[7] != null) item.PassportDate = DateTime.Parse(fields[7].ToString());
            item.driver.Id = oldItem == null ? 0 : oldItem.driver.Id;

            this.flagTypeEdit = oldItem != null;

            return item;
        }

        /// <summary>
        /// обработчик команды для удаления водитель
        /// </summary>
        /// <param name="param">объект водитель</param>
        protected override void DeleteItem(object param)
        {
            DriverVM item = param as DriverVM;
            if (item != null)
            {
                this.Drivers.Remove(item);
                UtilManager.Instance.Client.DriverDelete(item.driver);
                this.OnChangeItem(this.Drivers.FirstOrDefault());
            }
        }

        /// <summary>
        /// обработчик команды для изменения водитель
        /// </summary>
        /// <param name="param">объект водитель</param>
        protected override void EditItem(object param)
        {
            DriverVM item = this.GetItemFromList(param as IList<object>) as DriverVM;

            if (item == null)
            {
                throw new ArgumentException("Не удалось прочитать данные водитель из формы");
            }

            if (this.flagTypeEdit)
            {
                UtilManager.Instance.Client.DriverEdit(item.driver);

                base.EditItem(param);

                foreach (DriverVM im in this.Drivers)
                {
                    if (im.EqualsById(item))
                    {
                        im.CopyFrom(item);
                    }
                }               
            }
            else
            {
                item.driver.authorities = new List<DriverAuthority>();
                item.Authorities = new ObservableCollection<DriverAuthority>();

                item.driver.Id = UtilManager.Instance.Client.DriverAdd(item.driver);
                this.Drivers.Add(item);
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

			DriverVM driver = lst[0] as DriverVM;
			DriverAuthority auth = lst[1] as DriverAuthority;

			return 
                driver != null && // водитель выбран
                auth != null && //доверенность выбаран
                UtilManager.Instance.StateManager.IsServerEnabled; // сервер доступен
		}

        /// <summary>
        /// обработчик команды удаления доверенности у водителя
        /// </summary>
        /// <param name="param">удаляемая доверенность и изменяемый водитель</param>
        private void DeleteAuthorityExecute(object param)
        {
            List<object> lst = param as List<object>;

            DriverVM driver = lst[0] as DriverVM;
            DriverAuthority auth = lst[1] as DriverAuthority;

			FuncExec.Execute(() =>
				{
					UtilManager.Instance.Client.DriverAuthorityDelete((int)auth.Id);
					driver.Authorities.Remove(auth);

					UtilManager.Instance.UpdateModifiedObject(ModifiedObjects.DriverAuthorities);
					UtilManager.Instance.ModifiedTables[ModifiedObjects.Owners] = DateTime.MinValue;
				});
        }

        /// <summary>
        /// обработчик команды добавления доверенности у водителя
        /// </summary>
        /// <param name="param">добавляемая доверенность</param>
		private void AddAuthorityExecute(object param)
		{
			List<object> lst = param as List<object>;

			DriverAuthority auth = new DriverAuthority();
			DriverVM driver = lst[0] as DriverVM;

			FuncExec.Execute(() =>
				{
					auth.Driver = driver != null ? driver.driver : null;
					auth.Owner = lst[1] != null ? (lst[1] as OwnerVM).owner : null;
					auth.Number = lst[2].ToString();
					auth.Date = DateTime.Parse(lst[3].ToString());

					auth.Id = UtilManager.Instance.Client.DriverAuthorityAdd((int)auth.Driver.Id, (int)auth.Owner.Id, auth.Number, auth.Date);
					driver.Authorities.Add(auth);

					UtilManager.Instance.UpdateModifiedObject(ModifiedObjects.DriverAuthorities);
					UtilManager.Instance.ModifiedTables[ModifiedObjects.Owners] = DateTime.MinValue;
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
            this.ownerWindowVM.LoadData();
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
