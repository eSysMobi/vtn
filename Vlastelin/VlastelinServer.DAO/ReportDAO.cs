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
    public class ReportDAO:
        SimpleItemDAO
    {
        #region .ctor & instance
        protected ReportDAO()
			: base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
		{
		}
        protected static new ReportDAO _instance = null;
        public static new ReportDAO Instance
		{
			get
			{
				if (_instance == null)
				{
                    _instance = new ReportDAO();
				}

                return (ReportDAO)_instance;
			}
		}
		#endregion

        public DataTable ReportGet(ReportTypes reportType, DateTime from, DateTime to, BaseItem param1, BaseItem param2)
        {
            DataTable ret = null;
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("dtFrom", DbType.DateTime, from);
            parameters.AddInputParameter("dtTo", DbType.DateTime, to);

            switch (reportType)
            {
                case ReportTypes.SalesReportFull:
                    ret = this.Execute_GetDataTable(CommandType.Text, "select * from v_sales_report where dt between @dtFrom and @dtTo", parameters); 
                    ret.Columns.RemoveAt(0); // salesKindId
                    break;
                case ReportTypes.SalesReportTickets:
                    ret = this.Execute_GetDataTable(CommandType.Text, "select * from v_sales_report where dt between @dtFrom and @dtTo and SoldItemKind=1", parameters);
                    ret.Columns.RemoveAt(0); // salesKindId
                    break;
                case ReportTypes.SalesReportNonTickets:
                    ret = this.Execute_GetDataTable(CommandType.Text, "select * from v_sales_report where dt between @dtFrom and @dtTo and SoldItemKind<>1", parameters);
                    ret.Columns.RemoveAt(0); // salesKindId
                    break;
                case ReportTypes.SalesReportByBus:
                    if (!(param1 is Bus)) throw new ArgumentException("Param1 must be Bus");
                    parameters.AddInputParameter("bus_Id", DbType.Int32, param1.Id);
                    ret = this.Execute_GetDataTable(CommandType.Text, "select * from v_sales_report_bus where dt between @dtFrom and @dtTo and bId=@bus_Id", parameters);
                    ret.Columns.RemoveAt(0); // busId
                    ret.Columns.RemoveAt(0); // busName
                    break;
                case ReportTypes.SalesReportByTrip:
                    if (!(param1 is Trip)) throw new ArgumentException("Param1 must be Trip");
                    parameters.AddInputParameter("trip_Id", DbType.Int32, param1.Id);
                    ret = this.Execute_GetDataTable(CommandType.Text, "select * from v_sales_report_trip where dt between @dtFrom and @dtTo and tId=@trip_Id", parameters);
                    ret.Columns.RemoveAt(0); // tId
                    break;
                case ReportTypes.RKOReport:
                    ret = this.Execute_GetDataTable(CommandType.Text, "select * from v_rko_report where dt between @dtFrom and @dtTo", parameters);
                    ret.Columns.RemoveAt(0); // dt
                    break;
                case ReportTypes.PKOReport:
                    ret = this.Execute_GetDataTable(CommandType.Text, "select * from v_pko_report where dt between @dtFrom and @dtTo order by dt desc", parameters);
                    ret.Columns.RemoveAt(0); // dt
                    break;
                case ReportTypes.ReturnedTickets:
                    ret = this.Execute_GetDataTable(CommandType.Text, "select * from v_returned_tickets where dt between @dtFrom and @dtTo", parameters);
                    ret.Columns.RemoveAt(0); // dt
                    break;
            }

            return ret;
        }

        public DataTable ReportStatement(StationSchedule ss, DateTime depTime)
        {
            DataTable ret = null;
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("ss_id", DbType.Int32, ss.Id);
            parameters.AddInputParameter("depTime", DbType.DateTime, depTime);

            ret = this.Execute_GetDataTable(CommandType.Text, "select * from v_report_stmt where ssId=@ss_id and tripDate=@depTime", parameters);
            ret.Columns.RemoveAt(0); // ssId
            ret.Columns.RemoveAt(0); // dt
            
            return ret;
        }

        public DataTable ReportPassengers(DateTime? from, DateTime? to, string surname, string name, string patronymic)
        {
            DataTable ret = null;
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("dtFrom", DbType.DateTime, from);
            parameters.AddInputParameter("dtTo", DbType.DateTime, to);

            string dtCondition = "";
            if (from.HasValue && to.HasValue)
                dtCondition = "and dt between @dtFrom and @dtTo";
            else
                if (from.HasValue)
                    dtCondition = "and dt>=@dtFrom";
                else if (to.HasValue)
                    dtCondition = "and dt<=@dtTo";

            string sql = string.Format("select * from v_passengers where s like '%{0}%' and n like '%{1}%' and p like '%{2}%' {3}", surname, name, patronymic, dtCondition);

            ret = this.Execute_GetDataTable(CommandType.Text, sql, parameters);
            ret.Columns.RemoveAt(0); // pId
            ret.Columns.RemoveAt(0); // surname
            ret.Columns.RemoveAt(0); // name
            ret.Columns.RemoveAt(0); // patr

            return ret;
        }
    }
}
