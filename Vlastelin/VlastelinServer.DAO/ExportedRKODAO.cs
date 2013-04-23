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

using Vlastelin.Data.Model.ExportedData;

namespace VlastelinServer.DAO
{
    public class ExportedRKOMaterializator
        : MaterializatorBase<ExportedRKO>
    {
        public override ExportedRKO ReadSingleObject(DataReaderAdapter dataReader)
        {
            ExportedRKO ret = dataReader.ReadObject<ExportedRKO>();
            
            return ret;
        }
    }

    public class ExportedRKODAO
    : ItemDAO<ExportedRKO, ExportedRKOMaterializator>
    {
        #region .ctor & instance
        protected ExportedRKODAO()
            : base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
        {
        }

        public static new ExportedRKODAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ExportedRKODAO();
                }

                return (ExportedRKODAO)_instance;
            }
        }
        #endregion

        public List<ExportedRKO> ExportRKO(DateTime? from, DateTime? to)
        {
            string query = "select * from v_rko_export where DocDate between @from and @to";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("from", DbType.DateTime, from);
            parameters.AddInputParameter("to", DbType.DateTime, to);
            
            return this.Execute_GetList(System.Data.CommandType.Text, query, parameters);
        }
    }
}
