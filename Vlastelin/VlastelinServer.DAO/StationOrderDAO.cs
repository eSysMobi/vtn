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
    public class SOMaterializator
           : IMaterializator<StationOrder>
    {
        public StationOrder Materialize(DataReaderAdapter dataReader)
        {
            throw new NotImplementedException();
        }

        public List<StationOrder> Materialize_List(DataReaderAdapter dataReader)
        {
            List<StationOrder> sorder = new List<StationOrder>();

            while (dataReader.Read())
            {
                StationOrder so = dataReader.ReadObject<StationOrder>();
                sorder.Add(so);
            }

            dataReader.NextResult();
            while (dataReader.Read())
            {
                Trip t = dataReader.ReadObject<Trip>();
                sorder
                    .Where(so => so.tripId == t.Id)
                    .ToList()
                    .ForEach(soo => soo.Trip = t);
            }

            dataReader.NextResult();
            while (dataReader.Read())
            {
                Town t = dataReader.ReadObject<Town>();
                sorder
                    .Where(so => so._tId == t.Id)
                    .ToList()
                    .ForEach(soo => soo.Town = t);

                sorder
                    .Where(so => so.Trip._departureId == t.Id)
                    .ToList()
                    .ForEach(soo => soo.Trip.Departure = t);
                sorder
                    .Where(so => so.Trip._arrivalId == t.Id)
                    .ToList()
                    .ForEach(soo => soo.Trip.Arrival = t);
            }

            return sorder;

        }
    }

    public class StationOrderDAO:
		ItemDAO<StationOrder,SOMaterializator>
	{
		#region .ctor & instance
		protected StationOrderDAO()
			: base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
		{
		}

		public static new StationOrderDAO Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new StationOrderDAO();
				}

				return (StationOrderDAO)_instance;
			}
		}
		#endregion

        public List<StationOrder> StationOrderGet(Trip trip)
		{
			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();
			parameters.AddInputParameter("tId", DbType.Int32, trip==null? -1 : trip.Id);
			return this.Execute_GetList(CommandType.StoredProcedure, "sp_so_get", parameters);
		}

        public void StationOrderEdit(Trip trip, List<Town> towns)
        {
            if (trip == null)
                throw new ArgumentNullException("trip");
            if (trip.Id == 0)
                throw new ArgumentNullException("trip.ID");
            if (towns == null)
                throw new ArgumentNullException("towns");
            if (towns.Count == 0)
                throw new ArgumentNullException("towns.Count=0");

            StringBuilder townsSB = new StringBuilder();
            towns.ForEach(t => townsSB.AppendFormat("{0}|", t.Id));

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("tId", DbType.Int32, trip.Id);
            parameters.AddInputParameter("tList", DbType.String, townsSB.ToString());
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_so_edit", parameters);

            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Trips);
        }
    }
}
