using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using VlastelinClient.Commands;
using Vlastelin.Data.Model;
using VlastelinClient.Util;
using System.Data;
using Vlastelin.Common;
using Reports.Classes;

namespace VlastelinClient.ViewModel.WindowsViewModel
{
	/// <summary>
	/// печать РКО
	/// </summary>
	public class ListRKOWindowVM : BaseViewModel
	{
		/// <summary>
		/// список отчетов
		/// </summary>
		public DataTable ReportTable { get; private set; }
		
		/// <summary>
		/// команда печати РКО
		/// </summary>
		public ICommand PrintCommand
		{
			get
			{
				return new RelayCommand(this.PrintExecute, this.PrintCanExecute);
			}
		}

		/// <summary>
		/// загрузка автобусов
		/// </summary>
		public void LoadData()
		{
			this.ReportTable = UtilManager.Instance.Client.ReportGet(ReportTypes.RKOReport, DateTime.MinValue, DateTime.MaxValue, null, null);
		}

		/// <summary>
		/// обработчик команды печати РКО
		/// </summary>
		/// <param name="param"></param>
		public void PrintExecute(object param)
		{
			DataRow dr = (param as DataRowView).Row;
			int id = (int)dr["id"];
			
			RKO[] list = UtilManager.Instance.Client.RKOGet(id);
			if (list.Length > 0)
			{
				RKO rko = list.First();
				ReportsExecutor.ReportCashOrder(rko);
			}
		}

		/// <summary>
		/// проверка команды печати РКО
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		private bool PrintCanExecute(object param)
		{
			return param != null;
		}
	}
}
