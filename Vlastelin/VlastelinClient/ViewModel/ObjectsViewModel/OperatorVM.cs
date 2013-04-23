using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;
using Vlastelin.Common;

namespace VlastelinClient.ViewModel.ObjectsViewModel
{
    public class OperatorVM : FIOItemVM
    {
        public Operator operatr
        {
            get
            {
                return this.item as Operator;
            }
            set
            {
                this.item = value;
                this.OnPropertyChanged("Operator");
            }
        }

        /// <summary>
        /// филиал
        /// </summary>
        public BranchVM Branch
        {
            get
            {
                return new BranchVM(this.operatr.Branch);
            }
            set
            {
                this.operatr.Branch = value != null ? value.branch : null;
                this.OnPropertyChanged("Branch");
            }
        }

        /// <summary>
        /// роль
        /// </summary>
        public Roles Role
        {
            get
            {
                return this.operatr.Role;
            }
            set
            {
                this.operatr.Role = value;
				this.OnPropertyChanged("Role");
				this.OnPropertyChanged("RoleString");
            }
        }

		/// <summary>
        /// роль (строкое представление)
        /// </summary>
		public String RoleString
		{
			get
			{
				return this.Role.GetDescription();
			}
			set
			{
				if (String.IsNullOrEmpty(value))
				{
					throw new IncorrectDataException("Необходимо выбрать роль оператора");
				}
				this.Role = (Roles)Ct.GetEnumFromDescription(typeof(Roles), value);
				this.OnPropertyChanged("Role");
				this.OnPropertyChanged("RoleString");
			}
		}

		/// <summary>
		/// данные о профиле и безопасности пользователя
		/// </summary>
		public string Login { get { return this.operatr.Login; } }
		public DateTime LastPasswordChangeDate { get { return this.operatr.LastPasswordChangeDate; } }
		public DateTime LastActivityDate { get { return this.operatr.LastActivityDate; } }
		public DateTime LastLoginDate { get { return this.operatr.LastLoginDate; } }
		public DateTime CreationDate { get { return this.operatr.CreationDate; } }

        public OperatorVM()
        {
            this.operatr = new Operator();
        }

        public OperatorVM(Operator op)
        {
            this.operatr = op != null ? op : new Operator();
        }

        public override void CopyFrom(BaseItemVM itm)
        {
            base.CopyFrom(itm);
            this.Branch.CopyFrom((itm as OperatorVM).Branch);
			this.Role = (itm as OperatorVM).Role;
            this.Refresh();
        }

        public bool FilterCondition(String name, String surname)
        {
            return this.Name.ToUpper().Contains(name.ToUpper()) && this.Surname.ToUpper().Contains(surname.ToUpper());
        }

    }
}
