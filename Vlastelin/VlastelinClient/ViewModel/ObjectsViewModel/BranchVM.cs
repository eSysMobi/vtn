using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;

namespace VlastelinClient.ViewModel.ObjectsViewModel
{
    /// <summary>
    /// вьюмодель для класса филиал
    /// </summary>
    public class BranchVM : BaseItemVM
    {
        /// <summary>
        /// объект класса модели Филиал
        /// </summary>
        public Branch branch
        {
            get
            {
                return this.item as Branch;
            }
            set
            {
                this.item = value;
            }
        }

        /// <summary>
        /// название филиала
        /// </summary>
        public string Name 
        {
            get
            {
                return this.branch.Name;
            }
            set
            {
                this.branch.Name = value;
                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// город филиала
        /// </summary>
        public TownVM Town
        {
            get
            {
                return new TownVM(this.branch.Town);
            }
            set
            {
                this.branch.Town = value != null ? value.town : null;
                this.OnPropertyChanged("Town");
            }
        }

        /// <summary>
        /// адрес филиала
        /// </summary>
        public string Address 
        {
            get
            {
                return this.branch.Address;
            }
            set
            {
                this.branch.Address = value;
                this.OnPropertyChanged("Address");
            }
        }

        /// <summary>
        /// телефон филиала
        /// </summary>
        public string Phone 
        {
            get
            {
                return this.branch.Phone;
            }
            set
            {
                this.branch.Phone = value;
                this.OnPropertyChanged("Phone ");
            }
        }

        public BranchVM()
        {
            this.branch = new Branch();
        }

        public BranchVM(Branch br)
        {
            if (br == null)
            {
                br = new Branch();
            }
            this.branch = br;
        }

        public override void CopyFrom(BaseItemVM itm)
        {
            BranchVM item = itm as BranchVM;
            if (item !=null)
            {
                base.CopyFrom(itm);
                this.Town.CopyFrom(item.Town);
            }      
        }

        public override string ToString()
        {
            return this.branch.ToString();
        }

        public bool FilterCondition(String name, String town)
        {
            return this.Name.ToUpper().Contains(name.ToUpper()) && this.Town.Name.ToUpper().Contains(town.ToUpper());
        }
    }
}
