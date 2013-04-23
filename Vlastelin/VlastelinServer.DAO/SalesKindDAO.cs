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
    public class SalesKindMaterializator
        : MaterializatorBase<SalesKind>
    {
        public override SalesKind ReadSingleObject(DataReaderAdapter dataReader)
        {
            SalesKind ret = dataReader.ReadObject<SalesKind>();

            return ret;
        }
    }
    public class SalesKindDAO : ItemDAO<SalesKind, SalesKindMaterializator>
    {
        #region .ctor & instance
        protected SalesKindDAO()
            : base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
        {
        }

        public static new SalesKindDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SalesKindDAO();
                }

                return (SalesKindDAO)_instance;
            }
        }
        #endregion

        public List<SalesKind> SalesKindsGet(long? id)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();

            if (id.HasValue)
                parameters.AddInputParameter("skId", DbType.Int32, id.Value);
            else
                parameters.AddInputParameter("skId", DbType.Int32, null);

            return this.Execute_GetList(CommandType.StoredProcedure, "sp_saleskind_get", parameters);
        }
    }
}
