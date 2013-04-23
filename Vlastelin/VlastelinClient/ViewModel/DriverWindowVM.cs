using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VlastelinClient.ViewModel.ObjectsViewModel;
using System.Collections.ObjectModel;
using Vlastelin.Data.Model;
using VlastelinClient.ServiceReference1;
using VlastelinClient.Util;
using System.ComponentModel;
using Vlastelin.Common;
using System.Windows.Controls;

namespace VlastelinClient.ViewModel
{
    public class DriverWindowVM : BaseWindowVM
    {
        /// <summary>
        /// список водителей
        /// </summary>
        public ObservableCollection<DriverVM> Drivers { get; private set; }
        
        /// <summary>
        /// используется для биндинга к свойствам добавляемого водителя
        /// </summary>

        public DriverWindowVM(Utilities utls) :
            base(utls)
        {
        }

        /// <summary>
        /// инициализация начальных параметров
        /// </summary>
        protected override void Init()
        {
            base.Init();
            //this.Drivers = SampleData.Drivers;
        }

        /// <summary>
        /// загрузка данных из базы
        /// </summary>
        public override object LoadData()
        {
            base.LoadData();

            this.Drivers = new ObservableCollection<DriverVM>(this.utilite.Client.DriversGet(null).Select(driver => new DriverVM(driver)));

            return this.Drivers.Count;
        }

        /// <summary>
        /// получение объекта водитель из данных формы
        /// </summary>
        /// <param name="fields">список полей</param>
        /// <returns>получаемый водитель</returns>
        private BaseItemVM GetItemFromList(IList<object> fields)
        {
            // 6 - количество полей в классе водитель + изменяемый водитель
            if (fields == null || fields.Count != 6)
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
            item.driver.Id = oldItem == null ? 0 : oldItem.driver.Id;

            this.flagTypeEdit = oldItem != null;

            return item;
        }

        /// <summary>
        /// обработчик команды для удаления водитель
        /// </summary>
        /// <param name="param">объект водитель</param>
        protected override object DeleteItem(object param)
        {
            DriverVM item = param as DriverVM;
            if (item != null)
            {
                this.Drivers.Remove(item);
                this.utilite.Client.DriverDelete(item.driver);
            }

            return item;
        }

        /// <summary>
        /// обработчик команды для добавления водитель
        /// </summary>
        /// <param name="param">называние водитель</param>
        protected override void AddItem(object param)
        {
            ListBox listBox = param as ListBox;
            if (listBox == null)
            {
                throw new ArgumentException("Ошибка при добавлении водитель");
            }

            listBox.SelectedIndex = -1;
        }

        /// <summary>
        /// обработчик команды для изменения водитель
        /// </summary>
        /// <param name="param">объект водитель</param>
        protected override object EditItem(object param)
        {
            DriverVM item = this.GetItemFromList(param as IList<object>) as DriverVM;

            if (item == null)
            {
                throw new ArgumentException("Не удалось прочитать данные водитель из формы");
            }

            if (this.flagTypeEdit)
            {
                base.EditItem(param);

                foreach (DriverVM im in this.Drivers)
                {
                    if (im.Equals(item))
                    {
                        im.Copy(item);
                    }
                }

                this.utilite.Client.DriverEdit(item.driver);
            }
            else
            {
                long id = this.utilite.Client.DriverAdd(item.driver);
                item.driver.Id = id;
                this.Drivers.Add(item);
            }
            return item;
        }
    }
}
