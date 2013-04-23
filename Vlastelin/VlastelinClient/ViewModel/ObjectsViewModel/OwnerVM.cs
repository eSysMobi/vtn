using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;
using VlastelinClient.Util;
using System.Collections.ObjectModel;
using Vlastelin.Common;

namespace VlastelinClient.ViewModel.ObjectsViewModel
{
    /// <summary>
    /// вью модель для владельца
    /// </summary>
    public class OwnerVM : BaseItemVM
    {
        /// <summary>
        /// объект класса модели для владельца
        /// </summary>
        public Owner owner 
        {
            get
            {
                return this.item as Owner ;
            }
            set
            {
                this.item = value;
            }
        }

  
        /// <summary>
        /// Название организации
        /// </summary>
        public string Name 
        {
            get
            {
                return this.owner.Name;
            }
            set
            {
                this.owner.Name = value;
                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Номер свидетельства
        /// </summary>
        public string NumSv 
        {
            get
            {
                return this.owner.NumSv;
            }
            set
            {
                this.owner.NumSv = value;
                this.OnPropertyChanged("NumSv");
            } 
        }

        /// <summary>
        /// ОГРН
        /// </summary>
        public string OGRN 
        {
            get
            {
                return this.owner.OGRN;
            }
            set
            {
                this.owner.OGRN = value;
                this.OnPropertyChanged("OGRN");
            }
        }

        /// <summary>
        /// Номер и дата договора коммиссии
        /// </summary>
        public string DocNum 
        {
            get
            {
                return this.owner.DocNum;
            }
            set
            {
                this.owner.DocNum = value;
                this.OnPropertyChanged("DocNum");
            } 
        }
        public DateTime DocDate 
        {
            get
            {
                return this.owner.DocDate;
            }
            set
            {
                this.owner.DocDate = value;
                this.OnPropertyChanged("DocDate");
            } 
        }

        /// <summary>
        /// ФИО директора
        /// </summary>
        public string DirName 
        {
            get
            {
                return this.owner.DirName;
            }
            set
            {
                this.owner.DirName = value;
                this.OnPropertyChanged("DirName");
            } 
        }
        public string DirSurname 
        {
            get
            {
                return this.owner.DirSurname;
            }
            set
            {
                this.owner.DirSurname = value;
                this.OnPropertyChanged("DirSurname");
            } 
        }
        public string DirPatronymic 
        {
            get
            {
                return this.owner.DirPatronymic;
            }
            set
            {
                this.owner.DirPatronymic = value;
                this.OnPropertyChanged("DirPatronymic");
            } 
        }

        /// <summary>
        /// тип комиссии
        /// </summary>
        public FeeTypes FeeType
        {
            get 
            {
                return this.owner.FeeType;
            }
            set
            {
                this.owner.FeeType = value;
                this.OnPropertyChanged("FeeType");
            }
        }

        /// <summary>
        /// строковое представление типа комиссии
        /// </summary>
        public String FeeTypeStr
        {
            get
            {
                return this.owner.FeeType.GetDescription();
            }
        }

        /// <summary>
        /// размер комиссии
        /// </summary>
        public double FeeAmount
        {
            get
            {
                return this.owner.FeeAmount;
            }
            set
            {
                this.owner.FeeAmount = value;
                this.OnPropertyChanged("FeeAmount");
            }
        }

        /// <summary>
        /// ИНН
        /// </summary>
        public String INN
        {
            get
            {
                return this.owner.INN;
            }
            set
            {
                this.owner.INN = value;
                this.OnPropertyChanged("INN");
            }
        }

        /// <summary>
        /// юридический адрес
        /// </summary>
        public String Address
        {
            get
            {
                return this.owner.Address;
            }
            set
            {
                this.owner.Address = value;
                this.OnPropertyChanged("Address");
            }
        }

        /// <summary>
        /// дата окончания действиия договора
        /// </summary>
        public DateTime DocEndDate
        {
            get
            {
                return this.owner.DocEndDate;
            }
            set
            {
                this.owner.DocEndDate = value;
                this.OnPropertyChanged("DocEndDate");
            }
        }

        /// <summary>
        /// тип позиции управляющего
        /// </summary>
        public DirPosition DirPosition
        {
            get
            {
                return this.owner.DirPosition;
            }
            set
            {
                this.owner.DirPosition = value;
                this.OnPropertyChanged("DirPosition");
            }
        }

        private ObservableCollection<DriverAuthority> _authorities;

        public ObservableCollection<DriverAuthority> Authorities
        {
          get 
          { 
              return _authorities; 
          }
          set 
          { 
              _authorities = value;
              this.OnPropertyChanged("Authorities");
          }
        }

        public OwnerVM()
        {
            this.owner = new Owner();
        }

        public OwnerVM(Owner ow)
        {
            if (ow == null)
            {
                ow = new Owner();
            }
            this.owner = ow;
            this._authorities = new ObservableCollection<DriverAuthority>(this.owner.authorities);
        }

        /// <summary>
        /// добавление доверенности
        /// </summary>
        /// <param name="auth">добавляемая доверенность</param>
        public void AddAuthority(DriverAuthority auth)
        {
            this._authorities.Add(auth);
            this.owner.authorities.Add(auth);
        }

        /// <summary>
        /// удаление доверенности
        /// </summary>
        /// <param name="auth">удаляемая доверенность</param>
        public void RemoveAuthority(DriverAuthority auth)
        {
            this._authorities.Remove(auth);
            this.owner.authorities.Remove(auth);
        }

        public bool FilterCondition(String name, String doc)
        {
            return this.Name.ToUpper().Contains(name.ToUpper()) && this.DocNum.ToUpper().Contains(doc.ToUpper());
        }

        #region переопределенные методы

        public override void CopyFrom(BaseItemVM itm)
        {
            OwnerVM item = itm as OwnerVM;
            if (item != null)
            {
                base.CopyFrom(item);
                this.DirPosition.CopyFrom(item.DirPosition);
            }
        }

        public override string ToString()
        {
            return this.owner.ToString();
        }

        #endregion
    }
}
