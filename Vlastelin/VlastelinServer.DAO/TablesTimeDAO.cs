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
    public class TablesTimeMaterializator
        : MaterializatorBase<TablesTime>
    {
        public override TablesTime ReadSingleObject(DataReaderAdapter dataReader)
        {
            return dataReader.ReadObject<TablesTime>();
        }
    }

    public class TablesTimeDAO
        :ItemDAO<TablesTime,TablesTimeMaterializator>
	{
		#region .ctor & instance
        protected TablesTimeDAO()
			: base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
		{
		}

        public static new TablesTimeDAO Instance
		{
			get
			{
				if (_instance == null)
				{
                    _instance = new TablesTimeDAO();
				}

                return (TablesTimeDAO)_instance;
			}
		}
		#endregion

        public DateTime GetLastModifiedTime(ModifiedObjects obj)
        {
            string sql = string.Format("select lastModifiedTime from TablesTime where lower(tName)='{0}'", obj.ToString().ToLower());
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            return this.Execute_ScalarStoredProcedure<DateTime>(CommandType.Text, sql, parameters);
        }

        public void SetLastModifiedTime(ModifiedObjects obj)
        {
            string sql = string.Format("update TablesTime set lastModifiedTime=@lmt where lower(tName)='{0}'", obj.ToString().ToLower());
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("lmt", DbType.DateTime, DateTime.Now);
            this.Execute_StoredProcedure(CommandType.Text, sql, parameters);
        }
    }
}
