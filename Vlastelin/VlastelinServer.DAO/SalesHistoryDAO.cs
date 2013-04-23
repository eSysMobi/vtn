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
    public class SalesHistoryMaterializator
        : IMaterializator<SalesHistory>
    {
        public SalesHistory Materialize(DataReaderAdapter dataReader)
        {
            throw new NotImplementedException();
        }

        public List<SalesHistory> Materialize_List(DataReaderAdapter dataReader)
        {
            List<SalesHistory> sHistory = new List<SalesHistory>();

            while (dataReader.Read())
            {
                SalesHistory sh = dataReader.ReadObject<SalesHistory>();
                sHistory.Add(sh);
            }

            // operator
            dataReader.NextResult();
            while (dataReader.Read())
            {
                Operator op = dataReader.ReadObject<Operator>();
                sHistory
                    .Where(sh => sh.operatorId == op.Id)
                    .ToList()
                    .ForEach(shh => shh.Operator = op);
            }

            // branch
            dataReader.NextResult();
            while (dataReader.Read())
            {
                Branch br = dataReader.ReadObject<Branch>();
                sHistory
                    .Where(sh => sh.Operator.branchId == br.Id)
                    .ToList()
                    .ForEach(shh => shh.Operator.Branch = br);
            }

            // town
            dataReader.NextResult();
            while (dataReader.Read())
            {
                Town t = dataReader.ReadObject<Town>();
                sHistory
                    .Where(sh => sh.Operator.Branch.townId == t.Id)
                    .ToList()
                    .ForEach(shh => shh.Operator.Branch.Town = t);
            }

            return sHistory;

        }
    }

    public class SalesHistoryDAO:
        ItemDAO<SalesHistory, SalesHistoryMaterializator>
	{
		#region .ctor & instance
        protected SalesHistoryDAO()
			: base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
		{
		}

        public static new SalesHistoryDAO Instance
		{
			get
			{
				if (_instance == null)
				{
                    _instance = new SalesHistoryDAO();
				}

                return (SalesHistoryDAO)_instance;
			}
		}
		#endregion

        public List<SalesHistory> SalesHistoryGet(DateTime? from, DateTime? to)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("dtFrom", DbType.DateTime, from);
            parameters.AddInputParameter("dtTo", DbType.DateTime, to);

            return this.Execute_GetList(CommandType.StoredProcedure, "sp_sales_get", parameters);
        }

        public List<SalesHistory> SalesHistoryGet(Seat seat)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("sId", DbType.Int32, seat.Id);

            return this.Execute_GetList(CommandType.StoredProcedure, "sp_sales_get_byseat", parameters);
        }

        public long SellItem(Operator op, double sum, SalesKind kind, IItemForSale item,int checkNumber)
        {
            if (op == null)
                throw new ArgumentException("operator");
            
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("opId", DbType.Int32, op.Id);
            parameters.AddInputParameter("money", DbType.Double, sum);
            parameters.AddInputParameter("sellTime", DbType.DateTime, DateTime.Now);
            parameters.AddInputParameter("kindId", DbType.Int32, kind.Id);
            parameters.AddInputParameter("itemId", DbType.Int32, item.Id);
            parameters.AddInputParameter("checkNum", DbType.Int32, checkNumber);
            long res = this.Execute_ScalarStoredProcedure<long>(CommandType.StoredProcedure, "sp_sell_item", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.SalesHistory);
            return res;
        }

        public void ReturnTicket(Operator op, Seat seat, int returnCheckNumber, int commissionCheckNumber)
        {
            if (op == null)
                throw new ArgumentException("operator");
            if (seat == null)
                throw new ArgumentException("seat");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("opId", DbType.Int32, op.Id);
            parameters.AddInputParameter("sId", DbType.Int32, seat.Id);
            parameters.AddInputParameter("retCheckNum", DbType.Int32, returnCheckNumber);
            parameters.AddInputParameter("comissionCheckNum", DbType.Int32, commissionCheckNumber);
            parameters.AddInputParameter("sellTime", DbType.DateTime, DateTime.Now);
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_ticket_return", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.SalesHistory);
        }
    }
}
