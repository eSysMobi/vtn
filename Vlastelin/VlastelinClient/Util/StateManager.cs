using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vlastelin.Common;
using System.ComponentModel;

namespace VlastelinClient.Util
{
	public enum ServerStates
	{
		[DescriptionAttribute("нет данных")]Unknown = 0,
		[DescriptionAttribute("есть соединение")]Connected = 1,
		[DescriptionAttribute("нет соединения")]Disconnected = 2,
		[DescriptionAttribute("ошибка сервера")]Faulted = 3,
		[DescriptionAttribute("обновление данных")]Updating = 4,
        [DescriptionAttribute("проверка соединения")]CheckState = 5,
		[DescriptionAttribute("загрузка данных")]DataLoading = 6
	}

	public enum KKMStates
	{
		[DescriptionAttribute("нет данных")]Unknown = 0,
		[DescriptionAttribute("подключен")]Enabled = 1,
		[DescriptionAttribute("отключен")]Disabled = 2,
		[DescriptionAttribute("незакрытая смена")]UnClosedSession = 3,
		[DescriptionAttribute("ошибка ККМ")]Faulted = 4,
		[DescriptionAttribute("не используется")]NotUsed = 5
	}
	
	public class StateManager
	{
		/// <summary>
		/// строка состояния для серверса и ККМ
		/// </summary>
		public event PropertyChangedEventHandler StateChanged;

		protected virtual void OnStateChanged(string propertyName)
		{
			if (this.StateChanged != null)
			{
				this.StateChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		private ServerStates _serverState = ServerStates.Unknown;
		private KKMStates _kkmState = KKMStates.Unknown;

		/// <summary>
		/// состояние сервера
		/// </summary>
		public ServerStates ServerState
		{
			get
			{
				return this._serverState;
			}
			set
			{
				this._serverState = value;
				this.OnStateChanged("ServerState");
				this.OnStateChanged("IsWindowEnabled");
				this.OnStateChanged("IsServerEnabled");
			}
		}

		/// <summary>
		/// состояние ККМ
		/// </summary>
		public KKMStates KKMState
		{
			get
			{
				return this._kkmState;
			}
			set
			{
				this._kkmState = value;
				this.OnStateChanged("KKMState");
			}
		}

		/// <summary>
		/// строкое представление состояния сервера
		/// </summary>
		public String ServerStateString
		{
			get
			{
				return this.ServerState.GetDescription();
			}
		}

		/// <summary>
		/// строкове представление состояния ККМ
		/// </summary>
		public String KKMStateString
		{
			get
			{
				return this.KKMState.GetDescription();
			}
		}

		/// <summary>
		/// флаг, определяющий дисейбл окна при загрузке данных
		/// </summary>
		public bool IsWindowEnabled
		{
			get
            {
                return ServerState != ServerStates.DataLoading;
            }
		}

		/// <summary>
		/// флаг, определяющий дисейбл окна при загрузке данных
		/// </summary>
		public bool IsServerEnabled
		{
			get
			{
				return ServerState != ServerStates.Disconnected && ServerState != ServerStates.Unknown;
			}
		}
	}
}
