using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Data.Model;

namespace VlastelinClient.Util
{
    /// <summary>
    /// класс для работы с номерами документов, получаемых через инкремент
    /// </summary>
    public class NumbersManager
    {
        private String _number = "Не установлен";

        /// <summary>
        /// последний номер для текущего города
        /// </summary>
        public String Number
        {
            get
            {
                return this._number;
            }
            set
            {
                this._number = value;
            }
        }

        /// <summary>
        /// получение последнего номера
        /// </summary>
        public void SetLastNumber()
        {
            this.Number = UtilManager.Instance.Client.DocNumGetNextNumber(UtilManager.Instance.CurrentOperator.Branch.Town);
        }
    }
}
