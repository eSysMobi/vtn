using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using VlastelinClient.ViewModel.ObjectsViewModel;
using Vlastelin.Data.Model;
using System.Collections.ObjectModel;
using VlastelinClient.ServiceReference1;
using VlastelinClient.Util;
using Vlastelin.Common;

namespace VlastelinClient.ViewModel
{
    public class BranchWindowVM : BaseWindowVM
    {
        /// <summary>
        /// список филиалов
        /// </summary>
        public ObservableCollection<BranchVM> Branchs { get; private set; }

        public BranchWindowVM(Utilities utls): 
            base(utls)
        {
        }

        /// <summary>
        /// получение списка филиалов из базы данных и приведение его к типу вьюмодели
        /// </summary>
        protected override void Init()
        {
            //var lst = this.utilite.Client.BranchsGet(null);
            /*this.Branchs = new ObservableCollection<BranchVM>(lst.Select(branch => new BranchVM(branch)));
            foreach (BranchVM branch in this.Branchs)
            {
                branch.PropertyChanged += new PropertyChangedEventHandler(this.BranchChanged);
            }*/
        }

        /// <summary>
        /// обработчик команды для удаления филиала
        /// </summary>
        /// <param name="param">объект филиала</param>
        protected override void DeleteItemExecute(object param)
        {
            BranchVM branch = param as BranchVM;
            if (branch != null)
            {
                if (this.utilite.MessageProvider.ShowYesNoDialogWindow("Вы уверены?"))
                {
                    this.Branchs.Remove(branch);
                    //this.utilite.Client.BranchDelete(branch.branch);
                }
            }
        }

        /// <summary>
        /// обработчик команды для добавления филиала
        /// </summary>
        /// <param name="param">называние филиала</param>
        protected override void AddItemExecute(object param)
        {
            String name = param as String;
            if (!String.IsNullOrEmpty(name))
            {
                /*Branch branch = new Branch(name);
                long id = this.utilite.Client.BranchAdd(branch);
                branch.Id = id;*/

                //BranchVM tvm = new BranchVM(branch);
                /*tvm.PropertyChanged += new PropertyChangedEventHandler(BranchChanged);
                this.Branchs.Add(tvm);*/
            }
        }

        /// <summary>
        /// обработчик команды для изменения филиала
        /// </summary>
        /// <param name="param">объект филиала</param>
        protected override void EditItemExecute(object param)
        {
            BranchVM branch = param as BranchVM;

            if (branch != null)
            { 
                //this.utilite.Client.BranchEdit(branch.branch);
            }
        }

        /// <summary>
        /// обработчик события изменения филиала
        /// </summary>
        /// <param name="sender">изменяемый филиал</param>
        /// <param name="e">параметры</param>
        private void BranchChanged(object sender, PropertyChangedEventArgs e)
        {
            this.EditItemExecute(sender);
        }
    }
}
