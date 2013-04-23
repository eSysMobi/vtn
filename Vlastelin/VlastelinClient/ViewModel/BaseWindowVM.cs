using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VlastelinClient.Util;
using Vlastelin.Data.Model;
using System.Windows.Input;
using VlastelinClient.Commands;
using VlastelinClient.ServiceReference1;
using Vlastelin.Common;

namespace VlastelinClient.ViewModel
{
    /// <summary>
    /// базовая вьюмодель для работы с окнами
    /// </summary>
    public class BaseWindowVM: BaseViewModel
    {
        protected ClientLogger clientLogger;

        /// <summary>
        /// определяет тип изменения объекта - редактирование или добавление, true - редактирование false добавление
        /// </summary>
        protected bool flagTypeEdit;

        /// <summary>
        /// содержит сервисные данные и вспомогательные модули
        /// </summary>
        protected Utilities utilite;

        /// <summary>
        /// команды для действий с объектами
        /// </summary>
        #region commands

        public ICommand DeleteItemCommand
        {
            get
            {
                return new RelayCommand(this.DeleteItemExecute);
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
                return new RelayCommand(this.EditItemExecute);
            }
        }
        public ICommand UpdateCommand
        {
            get
            {
                return new RelayCommand(this.UpdateExecute);
            }
        }
        #endregion

        /////////////////////////////////////////////////////////////////////////////

        public BaseWindowVM(Utilities utls)
        {
            this.utilite = utls;

            this.Init();
            //this.LoadData();
        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// инициализация
        /// </summary>
        protected virtual void Init()
        {
            this.clientLogger = new ClientLogger(this.utilite.logger, this.GetType().Name);
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// загрузка данных из базы
        /// </summary>
        public virtual object LoadData()
        {
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// загрузка данных из базы
        /// </summary>
        public virtual void LoadDataWithLog()
        {
            this.clientLogger.LogStart("Data loading");

            try
            {
                object info  = this.LoadData();
                this.clientLogger.LogFinish(String.Format("Items count: {0}", info));
            }
            catch (Exception ex)
            {
                this.clientLogger.LogException("Data loading", ex);
            }
        }

        ////////////////////////////////////////////////////////////////////////////

        protected virtual object DeleteItem(object param)
        {
            return null;
        }

        ////////////////////////////////////////////////////////////////////////////

        protected virtual void AddItem(object param)
        {
        }

        ////////////////////////////////////////////////////////////////////////////

        protected virtual object EditItem(object param)
        {
            return null;
        }

        /// <summary>
        /// обработчик команды удаления объекта
        /// </summary>
        /// <param name="param">удаляемый объект</param>
        protected virtual void DeleteItemExecute(object param)
        {
            try
            {
                object item = this.DeleteItem(param);
                this.clientLogger.LogDeleteItem(item);
            }
            catch (Exception ex)
            {
                this.clientLogger.LogException("Delete item", ex);
            }
        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик команды добавления объекта
        /// </summary>
        /// <param name="param">добавляемый объект</param>
        protected virtual void AddItemExecute(object param)
        {
            try
            {
                this.AddItem(param);
            }
            catch (Exception ex)
            {
                this.clientLogger.LogException("Add item", ex);
            }
        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// обработчик редактирования объекта
        /// </summary>
        /// <param name="param">редактируемый объект</param>
        protected virtual void EditItemExecute(object param)
        {
            try
            {
                object item = this.EditItem(param);
                this.clientLogger.LogSaveItem(item);
            }
            catch (Exception ex)
            {
                this.clientLogger.LogException("Edit item", ex);
            }
        }

        /////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// команда обновления списка объектов
        /// </summary>
        /// <param name="param"></param>
        protected virtual void UpdateExecute(object param)
        {
            this.LoadDataWithLog();
        }
    }
}
