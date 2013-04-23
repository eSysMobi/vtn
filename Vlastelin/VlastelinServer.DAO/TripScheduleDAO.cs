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
    public class TSMaterializator
        : IMaterializator<TripSchedule>
    {
        public TripSchedule Materialize(DataReaderAdapter dataReader)
        {
            throw new NotImplementedException();
        }

        public List<TripSchedule> Materialize_List(DataReaderAdapter dataReader)
        {
            List<TripSchedule> tripSchedule = new List<TripSchedule>();

            while (dataReader.Read())
            {
                TripSchedule ts = dataReader.ReadObject<TripSchedule>();
                tripSchedule.Add(ts);
            }

            dataReader.NextResult();
            while (dataReader.Read())
            {
                Trip t = dataReader.ReadObject<Trip>();
                tripSchedule
                    .Where(ts => ts.tripId == t.Id)
                    .ToList()
                    .ForEach(tss => tss.Trip = t);
            }

            dataReader.NextResult();
            while (dataReader.Read())
            {
                Town t = dataReader.ReadObject<Town>();
                tripSchedule
                    .Where(ts => ts.Trip._arrivalId == t.Id)
                    .ToList()
                    .ForEach(tss => tss.Trip.Arrival = t);
                tripSchedule
                    .Where(ts => ts.Trip._departureId == t.Id)
                    .ToList()
                    .ForEach(tss => tss.Trip.Departure = t);
            }

            dataReader.NextResult();
            while (dataReader.Read())
            {
                Bus b = dataReader.ReadObject<Bus>();
                tripSchedule
                    .Where(ts => ts.BusId == b.Id)
                    .ToList()
                    .ForEach(tss => tss.Bus = b);
            }

            dataReader.NextResult();
            while (dataReader.Read())
            {
                Owner o = dataReader.ReadObject<Owner>();
                tripSchedule
                    .Where(ts => ts.Bus.OwnerId == o.Id)
                    .ToList()
                    .ForEach(tss => tss.Bus.Owner = o);
            }
            dataReader.NextResult();
            while (dataReader.Read())
            {
                DirPosition dp = dataReader.ReadObject<DirPosition>();
                tripSchedule
                    .Where(ts => ts.Bus.Owner._dirPosition == dp.Id)
                    .ToList()
                    .ForEach(tss => tss.Bus.Owner.DirPosition = dp);
            }
            return tripSchedule;

        }
    }

    public class TripScheduleDAO :
        ItemDAO<TripSchedule, TSMaterializator>
    {
        #region .ctor & instance
        protected TripScheduleDAO()
            : base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
        {
        }

        public static new TripScheduleDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TripScheduleDAO();
                }

                return (TripScheduleDAO)_instance;
            }
        }
        #endregion

        public List<TripSchedule> TripSchedulesGet(long? ID = null)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();

            if (ID.HasValue)
                parameters.AddInputParameter("tsId", DbType.Int32, ID.Value);
            else
                parameters.AddInputParameter("tsId", DbType.Int32, null);

            return this.Execute_GetList(CommandType.StoredProcedure, "sp_ts_get", parameters);

        }

        public long TripScheduleAdd(TripSchedule ts)
        {
            if (ts == null)
                throw new ArgumentNullException("ts");
            if (ts.tripId == 0)
                throw new ArgumentNullException("ts.TripId");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("tId", DbType.Int32, ts.tripId);
            parameters.AddInputParameter("sType", DbType.Int32, ts.ScheduleType);
            parameters.AddInputParameter("stTime", DbType.DateTime, ts.StartTime);
            parameters.AddInputParameter("enTime", DbType.DateTime, ts.EndTime);
            parameters.AddInputParameter("bId", DbType.Int32, ts.BusId);
            long res = this.Execute_ScalarStoredProcedure<long>(CommandType.StoredProcedure, "sp_ts_add", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.TripsSchedule);
            return res;
        }

        public void TripScheduleDelete(TripSchedule ts)
        {
            if (ts == null)
                throw new ArgumentNullException("ts");
            if (ts.Id == 0)
                throw new ArgumentNullException("ts.Id");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("ts_Id", DbType.Int32, ts.Id);
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_ts_delete", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.TripsSchedule);
        }

        public void TripScheduleEdit(TripSchedule ts)
        {
            if (ts == null)
                throw new ArgumentNullException("ts");
            if (ts.tripId == 0)
                throw new ArgumentNullException("ts.TripId");
            /*if (ts._departureId == 0)
                throw new ArgumentNullException("ts.DepId");*/

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("tsId", DbType.Int32, ts.Id);
            parameters.AddInputParameter("tId", DbType.Int32, ts.tripId);
            parameters.AddInputParameter("sType", DbType.Int32, ts.ScheduleType);
            parameters.AddInputParameter("stTime", DbType.DateTime, ts.StartTime);
            parameters.AddInputParameter("enTime", DbType.DateTime, ts.EndTime);
            parameters.AddInputParameter("bId", DbType.Int32, ts.BusId);
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_ts_edit", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.TripsSchedule);
        }

        public void OperatorChangeBus(TripSchedule ts, Bus newBus, DateTime tripDate)
        {
            if (ts == null)
                throw new ArgumentNullException("ts");
            if (newBus == null)
                throw new ArgumentNullException("new bus");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("ts_Id", DbType.Int32, ts.Id);
            parameters.AddInputParameter("bId", DbType.Int32, newBus.Id);
            parameters.AddInputParameter("tDate", DbType.DateTime, tripDate);
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_ts_changebus", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.TripsSchedule);
        }

        public void TripScheduleSave(TripSchedule ts,List<StationSchedule> ss)
        {
            if (ts == null)
                throw new ArgumentNullException("ts");
            if (ts.tripId == 0)
                throw new ArgumentNullException("ts.TripId");
            if (ss == null)
                throw new ArgumentNullException("ss");
            if (ss.Count == 0)
                throw new ArgumentNullException("ss.Count=0");

            StringBuilder ssSB = new StringBuilder();
            ss.ForEach(t => ssSB.AppendFormat("{0}_{1}_{2}_|", t.Id, t._tId, t.DepartureTime.ToString("yyyy-MM-dd HH:mm:ss")));


            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("ts_Id", DbType.Int32, ts.Id);
            parameters.AddInputParameter("tId", DbType.Int32, ts.tripId);
            parameters.AddInputParameter("bId", DbType.Int32, ts.BusId);
            parameters.AddInputParameter("sType", DbType.Int32, ts.ScheduleType);
            parameters.AddInputParameter("stTime", DbType.DateTime, ts.StartTime);
            parameters.AddInputParameter("enTime", DbType.DateTime, ts.EndTime);
            parameters.AddInputParameter("ssList", DbType.String, ssSB.ToString());
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_ts_save", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.TripsSchedule);
        }
    }
}
