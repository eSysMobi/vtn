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
//using VlastelinClient.ServiceReference1;
using System.ComponentModel;
using System.Windows.Controls;
using Vlastelin.Common;

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

        public ICommand ResetLastNumberCommand
        {
            get
            {
                return new RelayCommand(this.ResetLastNumberExecute);
            }
        }

        public TownWindowVM()
        {
        }

        /// <summary>
        /// получение списка городов из базы данных и приведение его к типу вьюмодели
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.ModifiedObj = ModifiedObjects.Towns;
        }

        /// <summary>
        /// загрузка данных из базы
        /// </summary>
        /// <returns></returns>
        public override void LoadDataExecute()
        {
            base.LoadDataExecute();
            this.Towns = new ObservableCollection<TownVM>(UtilManager.Instance.Client.TownsGet(null).Select(town => new TownVM(town)));
        }

        /// <summary>
        /// получение объекта город из данных формы
        /// </summary>
        /// <param name="fields">список полей</param>
        /// <returns>получаемый город</returns>
        private TownVM GetItemFromList(IList<object> fields)
        {
            // 3 - количество полей в классе город + изменяемый город
            if (fields == null || fields.Count != 3)
            {
                throw new ArgumentException("Town");
            }

            TownVM town = new TownVM();
            TownVM oldTown;

            // формирование объекта автобус из данных формы
            oldTown = fields[0] as TownVM;
            town.Name = fields[1].ToString();
            town.Prefix = fields[2].ToString();
           
            town.town.Id = oldTown == null ? 0 : oldTown.town.Id;

            this.flagTypeEdit = oldTown != null;

            return town;
        }

        /// <summary>
        /// обработчик команды для удаления города
        /// </summary>
        /// <param name="param">объект города</param>
        protected override void DeleteItem(object param)
        {
            TownVM town = param as TownVM;
            if (town != null)
            {
                UtilManager.Instance.Client.TownDelete(town.town);
                this.Towns.Remove(town);
                this.OnChangeItem(this.Towns.FirstOrDefault());
            }
        }

        /// <summary>
        /// обновляет изменяемый город в коллекции (для отражения изменений)
        /// </summary>
        /// <param name="town">измененный город</param>
        private void ModifySelectedTown(TownVM town)
        {
            foreach (TownVM twn in this.Towns)
            {
                if (twn.EqualsById(town))
                {
                    twn.CopyFrom(town);
                }
            }
        }

        /// <summary>
        /// обработчик команды для изменения города
        /// </summary>
        /// <param name="param">объект города</param>
        protected override void EditItem(object param)
        {
            TownVM town = this.GetItemFromList(param as IList<object>);

            if (town == null)
            {
                throw new ArgumentException("Не удалось прочитать данные города из формы");
            }

            if (this.flagTypeEdit)
            {
                UtilManager.Instance.Client.TownEdit(town.town);

                base.EditItem(param);
                ModifySelectedTown(town);
            }
            else
            {
                town.town.Id = UtilManager.Instance.Client.TownAdd(town.town);
                this.Towns.Add(town);
            }
            this.OnChangeItem(town);
        }

        /// <summary>
        /// обработчик команды для обновления списка объектов
        /// </summary>
        /// <param name="param"></param>
        protected override void UpdateData(object param)
        {
            this.LoadData();
        }

        /// <summary>
        /// обновление городов
        /// </summary>
        protected override void RefreshItems()
        {
            base.RefreshItems();
            this.OnPropertyChanged("Towns");
        }

        /// <summary>
        /// сбрасывание последнего номера документов
        /// </summary>
        /// <param name="param">город</param>
        private void ResetLastNumberExecute(object param)
        {
            TownVM town = param as TownVM;
            if (town != null)
            {
                if (UtilManager.Instance.MessageProvider.ShowYesNoDialogWindow("Сбросить счетчик нумерации документов для данного города?"))
                {
                    FuncExec.Execute(() => 
                        {
                            UtilManager.Instance.Client.TownEdit(town.town);
                            town.LastNumber = 0;
                            ModifySelectedTown(town);
                            this.OnChangeItem(town);
                        });
                }
            }
        }
    }
}
