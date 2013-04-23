using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Vlastelin.Data.Model;
using VlastelinServer.DAO.DBAccess;
using Vlastelin.Common.Constants;
using Vlastelin.Common;
using MySql.Data.MySqlClient;
using System.Data;

namespace VlastelinServer.DAO
{
	public class TownMaterializator:
		MaterializatorBase<Town>
	{
		public override Town ReadSingleObject(DataReaderAdapter dataReader)
		{
			return dataReader.ReadObject<Town>();
		}
	}

	public class TownDAO:
		ItemDAO<Town,TownMaterializator>
	{
		#region .ctor & instance
		protected TownDAO()
			: base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
		{
		}

		public static new TownDAO Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new TownDAO();
				}

				return (TownDAO)_instance;
			}
		}
		#endregion

		public List<Town> TownsGet(long? ID = null)
		{
			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();

			if (ID.HasValue)
				parameters.AddInputParameter("TownId", DbType.Int32, ID.Value);
			else
				parameters.AddInputParameter("TownId", DbType.Int32, null);

			return this.Execute_GetList(CommandType.StoredProcedure, "sp_towns_get", parameters);

		}

		public long TownsAdd(Town town)
		{
			if (town == null)
				throw new ArgumentNullException("town");
			if(string.IsNullOrWhiteSpace(town.Name))
				throw new ArgumentNullException("town.Name");

			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();
			parameters.AddInputParameter("TownName", DbType.String, town.Name);
            parameters.AddInputParameter("prx", DbType.String, town.Prefix ?? "");
            parameters.AddInputParameter("lastNum", DbType.Int32, town.LastNumber);
			long res = this.Execute_ScalarStoredProcedure<long>(CommandType.StoredProcedure, "sp_towns_add", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Towns);
            return res;
		}

		public void TownsDelete(Town town)
		{
			if (town == null)
				throw new ArgumentNullException("town");
			if (town.Id==0)
				throw new ArgumentNullException("town.Id");

			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();
			parameters.AddInputParameter("tId", DbType.Int32, town.Id);
			this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_towns_delete", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Towns);
		}

		public void TownsEdit(Town town)
		{
			if (town == null)
				throw new ArgumentNullException("town");
			if (town.Id == 0)
				throw new ArgumentNullException("town.Id");
			if (string.IsNullOrWhiteSpace(town.Name))
				throw new ArgumentNullException("town.Name");

			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();
			parameters.AddInputParameter("TownId", DbType.Int32, town.Id);
			parameters.AddInputParameter("TownName", DbType.String, town.Name);
            parameters.AddInputParameter("prx", DbType.String, town.Prefix);
            parameters.AddInputParameter("lastNum", DbType.String, town.LastNumber);

            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_towns_edit", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Towns);
		}

        public string GetNextNumber(Town town)
        {
            if (town == null)
                throw new ArgumentException("Town");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("tId", DbType.Int32, town.Id);

            return this.Execute_ScalarStoredProcedure<string>(CommandType.StoredProcedure, "sp_docnum_getnum", parameters);
        }
	}
}
