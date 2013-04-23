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
    public class PKOMaterializator
        : IMaterializator<PKO>
    {

        #region IMaterializator<PKO> Members

        PKO IMaterializator<PKO>.Materialize(DataReaderAdapter dataReader)
        {
            throw new NotImplementedException();
        }

        List<PKO> IMaterializator<PKO>.Materialize_List(DataReaderAdapter dataReader)
        {
            List<PKO> pkos = new List<PKO>();

            while (dataReader.Read())
            {
                PKO pko = dataReader.ReadObject<PKO>();
                pkos.Add(pko);
            }


            // Operator
            dataReader.NextResult();
            while (dataReader.Read())
            {
                Operator o = dataReader.ReadObject<Operator>();
                pkos
                    .Where(p => p.OperatorId == p.Id)
                    .ToList()
                    .ForEach(pp => pp.Operator = o);
            }
            return pkos;
        }

        #endregion
    }

    public class PKODAO:
		ItemDAO<PKO,PKOMaterializator>
	{
		#region .ctor & instance
        protected PKODAO()
			: base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
		{
		}

        public static new PKODAO Instance
		{
			get
			{
				if (_instance == null)
				{
                    _instance = new PKODAO();
				}

                return (PKODAO)_instance;
			}
		}
		#endregion

        public List<PKO> PKOGet(long? ID = null)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();

            if (ID.HasValue)
                parameters.AddInputParameter("pkoId", DbType.Int32, ID.Value);
            else
                parameters.AddInputParameter("rkoId", DbType.Int32, null);

            return this.Execute_GetList(CommandType.StoredProcedure, "sp_pko_get", parameters);

        }

        public long PKOAdd(PKO pko)
        {
            if (pko == null)
                throw new ArgumentException("PKO");
            if (pko.OperatorId == 0)
                throw new ArgumentException("PKO.OperatorId");
            //if (rko.Sum == 0)
            //    throw new ArgumentException("RKO.sum");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("num", DbType.String, pko.DocNum);
            parameters.AddInputParameter("dDate", DbType.DateTime, pko.DocDate);
            parameters.AddInputParameter("opId", DbType.Int32, pko.OperatorId);
            parameters.AddInputParameter("sum", DbType.Double, pko.Sum);
            return this.Execute_ScalarStoredProcedure<long>(CommandType.StoredProcedure, "sp_pko_add", parameters);
        }
    }
}
