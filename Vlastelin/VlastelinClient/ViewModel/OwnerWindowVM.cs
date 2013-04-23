using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VlastelinClient.ViewModel.ObjectsViewModel;
using Vlastelin.Data.Model;
using System.ComponentModel;
using System.Collections.ObjectModel;
using VlastelinClient.ServiceReference1;
using VlastelinClient.Util;
using Vlastelin.Common;
using System.Windows.Controls;

namespace VlastelinClient.ViewModel
{
    public class OwnerWindowVM : BaseWindowVM
    {
        /// <summary>
        /// список владельцев
        /// </summary>
        public ObservableCollection<OwnerVM> Owners { get; private set; }

        public OwnerWindowVM(Utilities utls) : 
            base(utls)
        {
            //this.Owners = SampleData.Owners;
        }

        /// <summary>
        /// получение списка владельцев из базы данных и приведение его к типу вьюмодели
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
            var lst = this.utilite.Client.OwnersGet(null);
            this.Owners = new ObservableCollection<OwnerVM>(lst.Select(owner => new OwnerVM(owner)));

            return this.Owners.Count;
        }

        /// <summary>
        /// получение объекта владелец из данных формы
        /// </summary>
        /// <param name="fields">список полей</param>
        /// <returns>получаемый владелец</returns>
        private BaseItemVM GetItemFromList(IList<object> fields)
        {
            // 9 - количество полей в классе владелец + изменяемый владелец
            if (fields == null || fields.Count != 9)
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
            item.DocDate = DateTime.Parse(fields[5].ToString());
            item.DirName = fields[6].ToString();
            item.DirSurname = fields[7].ToString();
            item.DirPatronymic = fields[8].ToString();
            item.owner.Id = oldItem == null ? 0 : oldItem.owner.Id;

            this.flagTypeEdit = oldItem != null;

            return item;
        }

        /// <summary>
        /// обработчик команды для удаления владельца
        /// </summary>
        /// <param name="param">объект владельца</param>
        protected override object DeleteItem(object param)
        {
            OwnerVM item = param as OwnerVM;
            if (item != null)
            {
                this.Owners.Remove(item);
                this.utilite.Client.OwnerDelete(item.owner);
            }

            return item;
        }

        /// <summary>
        /// обработчик команды для добавления владельца
        /// </summary>
        /// <param name="param">называние владельца</param>
        protected override void AddItem(object param)
        {
            ListBox listBox = param as ListBox;
            if (listBox == null)
            {
                throw new ArgumentException("Ошибка при добавлении владельца");
            }

            listBox.SelectedIndex = -1;
        }

        /// <summary>
        /// обработчик команды для изменения владельца
        /// </summary>
        /// <param name="param">объект владельца</param>
        protected override object EditItem(object param)
        {
            OwnerVM item = this.GetItemFromList(param as IList<object>) as OwnerVM;

            if (item == null)
            {
                throw new ArgumentException("Не удалось прочитать данные владельца из формы");
            }

            if (this.flagTypeEdit)
            {
                base.EditItem(param);

                foreach (OwnerVM im in this.Owners)
                {
                    if (im.Equals(item))
                    {
                        im.Copy(item);
                    }
                }

                this.utilite.Client.OwnerEdit(item.owner);
            }
            else
            {
                long id = this.utilite.Client.OwnerAdd(item.owner);
                item.owner.Id = id;
                this.Owners.Add(item);
            }
            return item;
        }

    }
}
