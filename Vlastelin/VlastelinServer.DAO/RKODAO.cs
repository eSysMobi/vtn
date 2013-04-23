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
	public class RKOMaterializator
		: IMaterializator<RKO>
	{
		public RKO Materialize(DataReaderAdapter dataReader)
		{
			throw new NotImplementedException();
		}

		public List<RKO> Materialize_List(DataReaderAdapter dataReader)
		{
			List<RKO> rkos = new List<RKO>();

			while (dataReader.Read())
			{
				RKO rko = dataReader.ReadObject<RKO>();
                rko.TSF = TripScheduleFactDAO.Instance.TripScheduleFactGet(rko.tsfId)[0];
				rkos.Add(rko);
			}

			
			// Operator
			dataReader.NextResult();
			while (dataReader.Read())
			{
				Operator o = dataReader.ReadObject<Operator>();
                o.Branch = BranchDAO.Instance.BranchesGet(o.branchId)[0];
				rkos
					.Where(r => r.OperatorId == o.Id)
					.ToList()
					.ForEach(rr => rr.Operator = o);
			}

			rkos.ForEach(r => r.Bus.Owner = OwnerDAO.Instance.OwnersGet(r.Bus.OwnerId)[0]);

			return rkos;
		}
	}

	public class RKODAO:
		ItemDAO<RKO,RKOMaterializator>
	{
		#region .ctor & instance
		protected RKODAO()
			: base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
		{
		}

		public static new RKODAO Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new RKODAO();
				}

				return (RKODAO)_instance;
			}
		}
		#endregion

		public List<RKO> RKOGet(long? ID = null)
		{
			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();

			if (ID.HasValue)
				parameters.AddInputParameter("rkoId", DbType.Int32, ID.Value);
			else
				parameters.AddInputParameter("rkoId", DbType.Int32, null);

			return this.Execute_GetList(CommandType.StoredProcedure, "sp_rko_get", parameters);

		}

		public List<RKO> RKOReport(DateTime from, DateTime to)
		{
			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();
			parameters.AddInputParameter("date_from", DbType.DateTime, from);
			parameters.AddInputParameter("date_to", DbType.DateTime, to);
			return this.Execute_GetList(CommandType.StoredProcedure, "sp_rko_report", parameters);
		}

		public long RKOAdd(RKO rko)
		{
			if (rko == null)
				throw new ArgumentException("RKO");
			if (rko.tsfId == 0)
				throw new ArgumentException("RKO.tsfId");
			if (rko.OperatorId == 0)
				throw new ArgumentException("RKO.OperatorId");
            //if (rko.Sum == 0)
            //    throw new ArgumentException("RKO.sum");

			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();
			parameters.AddInputParameter("num", DbType.String, rko.Number);
			parameters.AddInputParameter("dDate", DbType.DateTime, rko.DocDate);
			parameters.AddInputParameter("tsf_id", DbType.Int32, rko.tsfId);
			parameters.AddInputParameter("opId", DbType.Int32, rko.OperatorId);
			parameters.AddInputParameter("sum", DbType.Double, rko.Sum);
			return this.Execute_ScalarStoredProcedure<long>(CommandType.StoredProcedure, "sp_rko_add", parameters);
		}
	}
}
