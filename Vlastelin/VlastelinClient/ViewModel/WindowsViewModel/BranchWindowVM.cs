using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using VlastelinClient.ViewModel.ObjectsViewModel;
using Vlastelin.Data.Model;
using System.Collections.ObjectModel;
//using VlastelinClient.ServiceReference1;
using VlastelinClient.Util;
using Vlastelin.Common;
using System.Windows.Controls;

namespace VlastelinClient.ViewModel
{
    public class BranchWindowVM : BaseWindowVM
    {
        /// <summary>
        /// вьюмодель городов, используется для филиалов
        /// </summary>
        protected TownWindowVM townWindowVM;

        /// <summary>
        /// список филиалов
        /// </summary>
        public ObservableCollection<BranchVM> Branches { get; set; }

        /// <summary>
        /// список городов для комбобокса
        /// </summary>
        public IEnumerable<TownVM> Towns
        {
            get
            {
                return this.townWindowVM.Towns.OrderBy(t => t.Name);
            }
        }

        public BranchWindowVM(TownWindowVM townVM): 
            base()
        {
            this.townWindowVM = townVM;
        }

        /// <summary>
        /// инициализация начальных данных
        /// </summary>
        protected override void Init()
        {
            base.Init();
            this.ModifiedObj = ModifiedObjects.Branches;
        }

        /// <summary>
        /// загрузка данных из базы
        /// </summary>
        public override void LoadDataExecute()
        {
            base.LoadDataExecute();
            this.Branches = new ObservableCollection<BranchVM>(UtilManager.Instance.Client.BranchesGet(null).Select(item => new BranchVM(item)));
        }

        /// <summary>
        /// получение объекта филиал из данных формы
        /// </summary>
        /// <param name="fields">список полей</param>
        /// <returns>получаемый филиал</returns>
        private BranchVM GetItemFromList(IList<object> fields)
        {
            // 5 - количество полей в классе филиал + изменяемый филиал
            if (fields == null || fields.Count != 5)
            {
                throw new ArgumentException("Не удалось получить филиал из данных формы");
            }

            BranchVM item = new BranchVM();
            BranchVM oldItem;

            // формирование объекта филиал из данных формы
            oldItem = fields[0] as BranchVM;
            item.Name = fields[1].ToString();
            item.Town = fields[2] != null ? fields[2] as TownVM : null;
            item.Address = fields[3].ToString();
            item.Phone = fields[4].ToString();
            item.branch.Id = oldItem == null ? 0 : oldItem.branch.Id;

            this.flagTypeEdit = oldItem != null;

            return item;
        }

        /// <summary>
        /// обработчик команды удаления филиала
        /// </summary>
        /// <param name="param">филиал</param>
        protected override void DeleteItem(object param)
        {
            BranchVM item = param as BranchVM;
            if (item != null)
            {
                UtilManager.Instance.Client.BranchDelete(item.branch);
                this.Branches.Remove(item);
                this.OnChangeItem(this.Branches.FirstOrDefault());
            }
        }

        /// <summary>
        /// обработчик команды для изменения филиала
        /// </summary>
        /// <param name="param"></param>
        protected override void EditItem(object param)
        {
            BranchVM item = this.GetItemFromList(param as IList<object>);

            if (item == null)
            {
                throw new ArgumentException("Не удалось прочитать данные филиала из формы");
            }

            if (this.flagTypeEdit)
            {
                UtilManager.Instance.Client.BranchEdit(item.branch);

                base.EditItem(param);

                foreach (BranchVM bs in this.Branches)
                {
                    if (bs.EqualsById(item))
                    {
                        bs.CopyFrom(item);
                    }
                }
            }
            else
            {
                item.branch.Id = UtilManager.Instance.Client.BranchAdd(item.branch);
                this.Branches.Add(item);
            }
            this.OnChangeItem(item);
        }

        /// <summary>
        /// обработчик команды для обновления списка объектов
        /// </summary>
        /// <param name="param"></param>
        protected override void UpdateData(object param)
        {
            this.LoadData();
            this.townWindowVM.LoadDataExecute();
        }

        /// <summary>
        /// обновление интерфейса
        /// </summary>
        protected override void RefreshItems()
        {
            base.RefreshItems();
            this.OnPropertyChanged("Branches");
            this.OnPropertyChanged("Towns");
        }
    }
}
