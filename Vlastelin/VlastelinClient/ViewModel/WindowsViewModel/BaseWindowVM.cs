using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VlastelinClient.Util;
using Vlastelin.Data.Model;
using System.Windows.Input;
using VlastelinClient.Commands;
//using VlastelinClient.ServiceReference1;
using Vlastelin.Common;
using System.Windows;
using System.Threading;
using System.ComponentModel;
using System.ServiceModel;
using VlastelinClient.ViewModel.ObjectsViewModel;

namespace VlastelinClient.ViewModel
{
    /// <summary>
    /// аргументы для события изменения объекта каталога
    /// </summary>
    public class ChangeItemEventArgs
    {
        public ChangeItemEventArgs(BaseItemVM item) 
        {
            this.ChangedItem = item;
        }
        public BaseItemVM ChangedItem { get; private set; }
    }

    public delegate void ChangeItemEventHandler(object sender, ChangeItemEventArgs e);

    /// <summary>
    /// базовая вьюмодель для работы с окнами
    /// </summary>
    public class BaseWindowVM: BaseViewModel
    {
        protected bool isBusy;
        protected ModifiedObjects ModifiedObj;

        public bool IsBusy
        {
            get 
            {
                return this.isBusy;
            }
            set 
            {
                this.isBusy = value;
                this.OnPropertyChanged("IsBusy");
            }
        }

        // объявляем событие изменения объекта каталога
        public event ChangeItemEventHandler ChangeItem;

        /// <summary>
        /// обертка для метода события
        /// </summary>
        /// <param name="item">изменяемый элемент</param>
        protected virtual void OnChangeItem(BaseItemVM item)
        {
            if (ChangeItem != null)
                ChangeItem(this, new ChangeItemEventArgs(item));
        }

        /// <summary>
        /// определяет тип изменения объекта - редактирование или добавление, true - редактирование false добавление
        /// </summary>
		public bool flagTypeEdit { get; set; }

        /// <summary>
        /// команды для действий с объектами
        /// </summary>
        #region Команды

        public ICommand DeleteItemCommand
        {
            get
            {
                return new RelayCommand(this.DeleteItemExecute, this.DeleteItemCanExecute);
            }
        }

        public ICommand AddItemCommand
        {
            get
            {
                return new RelayCommand(this.AddItemExecute);
            }
        }
        public ICommand EditItemCommand
        {
            get
            {
                return new RelayCommand(this.EditItemExecute, this.EditItemCanExecute);
            }
        }
        public ICommand UpdateCommand
        {
            get
            {
                return new RelayCommand(this.UpdateDataExecute, this.UpdateDataCanExecute);
            }
        }
        #endregion

        /////////////////////////////////////////////////////////////////////////////

        public BaseWindowVM()
        {
            this.Init();
            //this.LoadData();
        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// инициализация
        /// </summary>
        protected virtual void Init()
        {
            this.IsBusy = false;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// загрузка данных из базы
        /// </summary>
        public virtual void LoadDataExecute()
        {
            
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// загрузка данных из базы
        /// </summary>
        public virtual void LoadData()
        {
            if (UtilManager.Instance.NeedLoad(this.ModifiedObj))
            {
                this.LoadDataExecute();
            }   
        }

        ////////////////////////////////////////////////////////////////////////////

        protected virtual void DeleteItem(object param)
        {
        }

        ////////////////////////////////////////////////////////////////////////////

        protected virtual void AddItem(object param)
        {
        }

        ////////////////////////////////////////////////////////////////////////////

        protected virtual void EditItem(object param)
        {
        }

        /// <summary>
        /// обработчик команды удаления объекта
        /// </summary>
        /// <param name="param">удаляемый объект</param>
        protected virtual void DeleteItemExecute(object param)
        {
            if (UtilManager.Instance.MessageProvider.ShowYesNoDialogWindow("Вы действительно хотите удалить объект?"))
            {
                FuncExec.Execute(() =>
					{
						this.DeleteItem(param);
						UtilManager.Instance.UpdateModifiedObject(this.ModifiedObj);
                    });
            }
        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик для команды удаления объекта - проверка возможности удаления
        /// </summary>
        /// <param name="param"></param>
        protected virtual bool DeleteItemCanExecute(object param)
        {
            return 
                param != null && // объект выбран
                param is BaseItemVM &&// объект нужного типа
                UtilManager.Instance.StateManager.IsServerEnabled; // сервер доступен
        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик команды добавления объекта
        /// </summary>
        /// <param name="param">добавляемый объект</param>
        protected virtual void AddItemExecute(object param)
        {
            FuncExec.Execute(() =>
				{
					this.AddItem(param);
                    this.OnChangeItem(null);
				});
        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик редактирования объекта
        /// </summary>
        /// <param name="param">редактируемый объект</param>
        protected virtual void EditItemExecute(object param)
        {          
            FuncExec.Execute(() =>
            {
                this.EditItem(param);
				UtilManager.Instance.UpdateModifiedObject(this.ModifiedObj);
                UtilManager.Instance.MessageProvider.ShowInformationWindow("Изменения были сохранены");
            });
        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// проверка редактирования объекта
        /// </summary>
        /// <param name="param">редактируемый объект</param>
        protected virtual bool EditItemCanExecute(object param)
        {
            return UtilManager.Instance.StateManager.IsServerEnabled;
        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обновление данных в интерфейсе
        /// </summary>
        protected virtual void RefreshItems()
        {

        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// команда обновления списка объектов
        /// </summary>
        /// <param name="param"></param>
        protected virtual void UpdateData(object param)
        {

        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// команда обновления списка объектов
        /// </summary>
        /// <param name="param"></param>
        protected virtual void UpdateDataExecute(object param)
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (o, ea) =>
            {
                //long-running-process code
                this.UpdateData(param);
            };

            worker.RunWorkerCompleted += (o, ea) =>
            {
                this.IsBusy = false;

                if (ea.Error == null)
                {
                    DispatchService.Invoke((Action)(() =>
                    {
                        this.RefreshItems();
                        this.OnChangeItem(null);
                    }));
                }
                else
                {
                    DispatchService.Invoke((Action)(() =>
                    {
                        FuncExec.ProcessException(ea.Error);
                    }));
                }
                
            };
            this.IsBusy = true; //here the BusyIndicator.IsBusy is set to TRUE
            worker.RunWorkerAsync();
        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// проверка обновления данных
        /// </summary>
        /// <param name="param">редактируемый объект</param>
        protected virtual bool UpdateDataCanExecute(object param)
        {
            return UtilManager.Instance.StateManager.IsServerEnabled;
        }
    }
}
