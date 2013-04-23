using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

namespace VlastelinClient.ViewModel
{
    /// <summary>
    /// базовый класс вьюмоделей, используется чтобы не наследовать каждый раз интерфейс нотификации об изменении свойства
    /// </summary>
    public class BaseViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }

        public virtual void Refresh()
        {
            foreach (PropertyInfo prop in this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                this.OnPropertyChanged(prop.Name);
            }
        }

		/// <summary>
		/// уведомления об изменениях во вьюмодели
		/// </summary>
		public event PropertyChangedEventHandler ViewModelNotify;

		protected virtual void OnNotify(string param)
		{
			if (this.ViewModelNotify != null)
			{
				this.ViewModelNotify(this, new System.ComponentModel.PropertyChangedEventArgs(param));
			}
		}
    }
}
