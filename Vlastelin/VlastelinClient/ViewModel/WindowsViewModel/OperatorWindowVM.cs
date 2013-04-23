using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using VlastelinClient.ViewModel.ObjectsViewModel;
using VlastelinClient.Util;
using Vlastelin.Common;
using Vlastelin.Data.Model;
using System.Windows.Controls;
using System.ComponentModel;

namespace VlastelinClient.ViewModel.WindowsViewModel
{
    public class OperatorWindowVM : BaseWindowVM
    {
        /// <summary>
        /// вьюмодель окна операторов для работы с списком операторов
        /// </summary>
        protected BranchWindowVM branchWindowVM;

        /// <summary>
        /// список операторов
        /// </summary>
        public ObservableCollection<OperatorVM> Operators { get; private set; }

        /// <summary>
        /// список фирм-владельцев, используется для редактирования оператора
        /// </summary>
        public IEnumerable<BranchVM> Branches
        {
            get
            {
                return this.branchWindowVM.Branches.OrderBy(br => br.Name);
            }
        }

		/// <summary>
		/// пароль из пассвордбокс
		/// </summary>
		public String Password { get; set; }

		/// <summary>
		/// список ролей
		/// </summary>
		public IEnumerable<String> RolesList
		{
			get
			{
				return Ct.GetEnumDescriptionValues(typeof(Roles));
			}
		}
        
        /// <summary>
        /// используется для биндинга к свойствам добавляемого оператора
        /// </summary>

        public OperatorWindowVM(BranchWindowVM branchVM)
        {
            this.branchWindowVM = branchVM;
        }

        /// <summary>
        /// инициализация начальных данных
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.ModifiedObj = ModifiedObjects.Operators;
        }

        /// <summary>
        /// загрузка данных из базы
        /// </summary>
        public override void LoadDataExecute()
        {
            base.LoadDataExecute();
            this.Operators = new ObservableCollection<OperatorVM>(UtilManager.Instance.Client.OperatorsGet(null).Select(op => new OperatorVM(op)));
            this.RefreshItems();
        }

        /// <summary>
        /// получение объекта оператор из данных формы
        /// </summary>
        /// <param name="fields">список полей</param>
        /// <returns>получаемый оператор</returns>
        private Tuple<OperatorVM, String> GetOperatorFromList(IList<object> fields)
        {
            // 7 - количество полей в классе оператор + изменяемый оператор
            if (fields == null || fields.Count != 7)
            {
                throw new ArgumentException("Не удалось получить данные оператора из формы");
            }

            OperatorVM item = new OperatorVM();
            OperatorVM oldItem;

            // формирование объекта оператор из данных формы
            oldItem = fields[0] as OperatorVM;
            item.Name = fields[1].ToString();
            item.Surname = fields[2].ToString();
            item.Patronymic = fields[3].ToString();
            item.Branch = fields[4] != null ? fields[4] as BranchVM : null;
            item.operatr.Id = oldItem == null ? 0 : oldItem.operatr.Id;

			String login = fields[5].ToString();
			if (String.IsNullOrWhiteSpace(login))
			{
				throw new IncorrectDataException("Логин не может быть пустым");
			}

			item.RoleString = fields[6] != null ? fields[6].ToString() : null;

            this.flagTypeEdit = oldItem != null;

			return new Tuple<OperatorVM, string>(item, login);
        }

        /// <summary>
        /// обработчик команды удаления оператора
        /// </summary>
        /// <param name="param">оператор</param>
        protected override void DeleteItem(object param)
        {
            OperatorVM item = param as OperatorVM;
            if (item != null)
            {
                UtilManager.Instance.Client.OperatorDelete(item.operatr);
                this.Operators.Remove(item);
                this.OnChangeItem(this.Operators.FirstOrDefault());
            }          
        }

        /// <summary>
        /// обработчик команды для изменения оператора
        /// </summary>
        /// <param name="param"></param>
        protected override void EditItem(object param)
        {
			Tuple<OperatorVM, string> result = this.GetOperatorFromList(param as IList<object>);
			OperatorVM item = result.Item1;
            
            if (item == null)
            {
                throw new ArgumentException("Не удалось прочитать данные оператора из формы");
            }

            OperatorVM current = this.Operators.FirstOrDefault(op => op.EqualsById(item));

            if (this.flagTypeEdit)
            {
                base.EditItem(param);
				current.CopyFrom(item);

				UtilManager.Instance.Client.OperatorEdit(current.operatr, result.Item2, this.Password);
            }
            else
            {
				if (String.IsNullOrWhiteSpace(this.Password))
				{
					throw new IncorrectDataException("Пароль должен быть задан");
				}
				item.operatr.Id = UtilManager.Instance.Client.OperatorAdd(item.operatr, result.Item2, this.Password);
            }
            this.UpdateData(null);
            this.OnChangeItem(this.Operators.FirstOrDefault(op => op.Equals(item)));
        }

        /// <summary>
        /// обработчик команды для обновления списка объектов
        /// </summary>
        /// <param name="param"></param>
        protected override void UpdateData(object param)
        {            
            this.LoadData();
            this.branchWindowVM.LoadData();          
        }

        /// <summary>
        /// обновление интерфейса
        /// </summary>
        protected override void RefreshItems()
        {
            base.RefreshItems();
            this.OnPropertyChanged("Operators");
            this.OnPropertyChanged("Branches");
        }
    }
}
