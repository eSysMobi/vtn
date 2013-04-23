using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;
using System.Collections.ObjectModel;
using VlastelinClient.ViewModel.ObjectsViewModel;
using System.Windows.Input;
using VlastelinClient.Commands;
using VlastelinClient.Util;
using VlastelinClient.ServiceReference1;
using System.ComponentModel;
using System.Windows.Controls;

namespace VlastelinClient.ViewModel
{
    /// <summary>
    /// вью модель для списка городов
    /// </summary>
    public class TownWindowVM : BaseWindowVM
    {
        /// <summary>
        /// список городов
        /// </summary>
        public ObservableCollection<TownVM> Towns { get; private set; }

        public TownWindowVM(Utilities utls): 
            base(utls)
        {
        }

        /// <summary>
        /// получение списка городов из базы данных и приведение его к типу вьюмодели
        /// </summary>
        protected override void Init()
        {
            base.Init();

            //this.Towns = SampleData.Towns;
        }

        /// <summary>
        /// загрузка данных из базы
        /// </summary>
        /// <returns></returns>
        public override object LoadData()
        {
            var lst = this.utilite.Client.TownsGet(null);
            this.Towns = new ObservableCollection<TownVM>(lst.Select(town => new TownVM(town)));

            return this.Towns.Count;
        }

        /// <summary>
        /// получение объекта город из данных формы
        /// </summary>
        /// <param name="fields">список полей</param>
        /// <returns>получаемый город</returns>
        private TownVM GetItemFromList(IList<object> fields)
        {
            // 2 - количество полей в классе город + изменяемый город
            if (fields == null || fields.Count != 2)
            {
                throw new ArgumentException("Town");
            }

            TownVM town = new TownVM();
            TownVM oldTown;

            // формирование объекта автобус из данных формы
            oldTown = fields[0] as TownVM;
            town.Name = fields[1].ToString();
           
            town.town.Id = oldTown == null ? 0 : oldTown.town.Id;

            this.flagTypeEdit = oldTown != null;

            return town;
        }

        /// <summary>
        /// обработчик команды для удаления города
        /// </summary>
        /// <param name="param">объект города</param>
        protected override object DeleteItem(object param)
        {
            TownVM town = param as TownVM;
            if (town != null)
            {
                //if (this.utilite.MessageProvider.ShowYesNoDialogWindow("Вы уверены?"))
                //{
                    this.Towns.Remove(town);
                    this.utilite.Client.TownDelete(town.town);
                //}
            }
            return town;
        }

        /// <summary>
        /// обработчик команды для добавления города
        /// </summary>
        /// <param name="param">называние города</param>
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
        /// обработчик команды для изменения города
        /// </summary>
        /// <param name="param">объект города</param>
        protected override object EditItem(object param)
        {
            TownVM town = this.GetItemFromList(param as IList<object>);

            if (town == null)
            {
                throw new ArgumentException("Не удалось прочитать данные города из формы");
            }

            if (this.flagTypeEdit)
            {
                base.EditItem(param);

                foreach (TownVM twn in this.Towns)
                {
                    if (twn.Equals(town))
                    {
                        twn.Copy(town);
                    }
                }

                this.utilite.Client.TownEdit(town.town);
            }
            else
            {
                long id = this.utilite.Client.TownAdd(town.town);
                town.town.Id = id;
                this.Towns.Add(town);
            }
            return town;
        }
    }
}
