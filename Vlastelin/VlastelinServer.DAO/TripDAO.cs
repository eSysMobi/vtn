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
	public class TripMaterializator
		   : IMaterializator<Trip>
	{
		public Trip Materialize(DataReaderAdapter dataReader)
		{
			throw new NotImplementedException();
		}

		public List<Trip> Materialize_List(DataReaderAdapter dataReader)
		{
			List<Trip> trips = new List<Trip>();

			while (dataReader.Read())
			{
				Trip trip = dataReader.ReadObject<Trip>();
				trips.Add(trip);
			}
            dataReader.NextResult();
            while (dataReader.Read())
            {
                Town t = dataReader.ReadObject<Town>();
                trips
                    .Where(tr => tr._arrivalId == t.Id)
                    .ToList()
                    .ForEach(trp => trp.Arrival = t);
                trips
                    .Where(tr => tr._departureId == t.Id)
                    .ToList()
                    .ForEach(trp => trp.Departure = t);
            }
			return trips;

		}
	}

	public class TripDAO:
		ItemDAO<Trip,TripMaterializator>
	{
		#region .ctor & instance
		protected TripDAO()
			: base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
		{
		}

		public static new TripDAO Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new TripDAO();
				}

				return (TripDAO)_instance;
			}
		}
		#endregion

		public List<Trip> TripsGet(long? ID = null)
		{
			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();

			if (ID.HasValue)
				parameters.AddInputParameter("tripId", DbType.Int32, ID.Value);
			else
				parameters.AddInputParameter("tripId", DbType.Int32, null);

			return this.Execute_GetList(CommandType.StoredProcedure, "sp_trips_get", parameters);

		}

		public long TripAdd(Trip trip)
		{
			if (trip == null)
				throw new ArgumentNullException("trip");
			if (trip.Departure == null)
				throw new ArgumentNullException("trip.DepartureTown");
			if (trip.Arrival == null)
				throw new ArgumentNullException("trip.ArrivalTown");

			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();
			parameters.AddInputParameter("name", DbType.String, trip.Name);
			parameters.AddInputParameter("dep", DbType.Int32, trip._departureId);
			parameters.AddInputParameter("arr", DbType.Int32, trip._arrivalId);
			parameters.AddInputParameter("description", DbType.String, trip.Description);
			long res = this.Execute_ScalarStoredProcedure<long>(CommandType.StoredProcedure, "sp_trip_add", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Trips);

            List<Town> stations = new List<Town>();
            stations.Add(trip.Departure);
            stations.Add(trip.Arrival);
            trip.Id = res;
            StationOrderDAO.Instance.StationOrderEdit(trip, stations);
            return res;
		}

		public void TripDelete(Trip trip)
		{
			if (trip == null)
				throw new ArgumentNullException("trip");
			if (trip.Id == 0)
				throw new ArgumentNullException("trip.id");

			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("trip_id", DbType.Int32, trip.Id);
			this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_trip_delete", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Trips);
		}

		public void TripEdit(Trip trip)
		{
			if (trip == null)
				throw new ArgumentNullException("trip");
			if (trip.Id == 0)
				throw new ArgumentNullException("trip.id");

			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();
			parameters.AddInputParameter("tripId", DbType.Int32, trip.Id);
			parameters.AddInputParameter("name", DbType.String, trip.Name);
			parameters.AddInputParameter("dep", DbType.Int32, trip._departureId);
			parameters.AddInputParameter("arr", DbType.Int32, trip._arrivalId);
           //parameters.AddInputParameter("sType", DbType.Int32, trip.ScheduleType);
			parameters.AddInputParameter("description", DbType.String, trip.Description);
			this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_trip_edit", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Trips);
		}
	}
}
