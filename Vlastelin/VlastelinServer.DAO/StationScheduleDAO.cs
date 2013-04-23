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
    public class SSMaterializator
           : IMaterializator<StationSchedule>
    {
        StationSchedule IMaterializator<StationSchedule>.Materialize(DataReaderAdapter dataReader)
        {
            throw new NotImplementedException();
        }

        List<StationSchedule> IMaterializator<StationSchedule>.Materialize_List(DataReaderAdapter dataReader)
        {
            List<StationSchedule> sschedule = new List<StationSchedule>();

            while (dataReader.Read())
            {
                StationSchedule ss = dataReader.ReadObject<StationSchedule>();
                sschedule.Add(ss);
            }

            dataReader.NextResult();
            while (dataReader.Read())
            {
                TripSchedule ts = dataReader.ReadObject<TripSchedule>();
                sschedule
                    .Where(ss => ss._tsId == ts.Id)
                    .ToList()
                    .ForEach(sss => sss.TS = ts);
            }
            dataReader.NextResult();
            while (dataReader.Read())
            {
                Trip t = dataReader.ReadObject<Trip>();
                sschedule
                    .Where(ss => ss.TS.tripId == t.Id)
                    .ToList()
                    .ForEach(sss => sss.TS.Trip = t);
            }

            dataReader.NextResult();
            while (dataReader.Read())
            {
                Town t = dataReader.ReadObject<Town>();
                sschedule
                    .Where(ss => ss._tId == t.Id)
                    .ToList()
                    .ForEach(sss => sss.Town = t);

                sschedule
                    .Where(ss => ss.TS.Trip._departureId == t.Id)
                    .ToList()
                    .ForEach(sss => sss.TS.Trip.Departure = t);
                sschedule
                    .Where(ss => ss.TS.Trip._arrivalId == t.Id)
                    .ToList()
                    .ForEach(sss => sss.TS.Trip.Arrival = t);
            }

            dataReader.NextResult();
            while (dataReader.Read())
            {
                Bus b = dataReader.ReadObject<Bus>();
                sschedule
                    .Where(ss => ss.TS.BusId == b.Id)
                    .ToList()
                    .ForEach(sss => sss.TS.Bus = b);
            }

            dataReader.NextResult();
            while (dataReader.Read())
            {
                Owner o = dataReader.ReadObject<Owner>();
                sschedule
                    .Where(ss => ss.TS.Bus.OwnerId == o.Id)
                    .ToList()
                    .ForEach(sss => sss.TS.Bus.Owner = o);
            }
            dataReader.NextResult();
            while (dataReader.Read())
            {
                DirPosition dp = dataReader.ReadObject<DirPosition>();
                sschedule
                    .Where(ss => ss.TS.Bus.Owner._dirPosition == dp.Id)
                    .ToList()
                    .ForEach(sss => sss.TS.Bus.Owner.DirPosition = dp);
            }
            return sschedule;
        }
    }
    public class StationScheduleDAO:
		ItemDAO<StationSchedule,SSMaterializator>
	{
		#region .ctor & instance
        protected StationScheduleDAO()
			: base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
		{
		}

        public static new StationScheduleDAO Instance
		{
			get
			{
				if (_instance == null)
				{
                    _instance = new StationScheduleDAO();
				}

                return (StationScheduleDAO)_instance;
			}
		}
		#endregion

        public List<StationSchedule> StationScheduleGet(TripSchedule ts)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("ts_Id", DbType.Int32, ts == null ? -1 : ts.Id);
            return this.Execute_GetList(CommandType.StoredProcedure, "sp_ss_get", parameters);
        }

        public void StationScheduleEdit(StationSchedule ss)
        {
            if (ss == null)
                throw new ArgumentNullException("SS");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("ssId", DbType.Int32, ss.Id);
            parameters.AddInputParameter("tsId", DbType.Int32, ss._tsId);
            parameters.AddInputParameter("tId", DbType.Int32, ss._tId);
            parameters.AddInputParameter("depTime", DbType.DateTime, ss.DepartureTime);
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_ss_edit", parameters);
        }

        /*public void StationSchedulesSave(List<StationSchedule> stationsList)
        {
            foreach (StationSchedule ss in stationsList)
                StationScheduleEdit(ss);
        }*/

        public long StationScheduleAdd(StationSchedule ss)
        {
            if (ss == null)
                throw new ArgumentNullException("town");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("ts_Id", DbType.Int32, ss._tsId);
            parameters.AddInputParameter("town_Id", DbType.Int32, ss._tId);
            parameters.AddInputParameter("depTime", DbType.DateTime, ss.DepartureTime);
            long res = this.Execute_ScalarStoredProcedure<long>(CommandType.StoredProcedure, "sp_ss_add", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Towns);
            return res;
        }

        public void StationScheduleDelete(StationSchedule ss)
        {
            if (ss == null)
                throw new ArgumentNullException("SS");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("ss_Id", DbType.Int32, ss.Id);
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_ss_delete", parameters);
        }
    }
}
