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
	public class BusMaterializator
		: IMaterializator<Bus>
	{
		public Bus Materialize(DataReaderAdapter dataReader)
		{
			throw new NotImplementedException();
		}

		public List<Bus> Materialize_List(DataReaderAdapter dataReader)
		{
			List<Bus> buses = new List<Bus>();

			while (dataReader.Read())
			{
				Bus bus = dataReader.ReadObject<Bus>();
				buses.Add(bus);
			}
			dataReader.NextResult();

			while (dataReader.Read())
			{
				Owner o = dataReader.ReadObject<Owner>();
				buses
					.Where(b => b.OwnerId == o.Id)
					.ToList()
					.ForEach(bb => bb.Owner = o);
			}
            dataReader.NextResult();
            while (dataReader.Read())
            {
                DirPosition dp = dataReader.ReadObject<DirPosition>();
                buses
                    .Where(b => b.Owner._dirPosition == dp.Id)
                    .ToList()
                    .ForEach(bb => bb.Owner.DirPosition = dp);
            }
			return buses;

		}
	}

	public class BusDAO:
		ItemDAO<Bus,BusMaterializator>
	{
		#region .ctor & instance
		protected BusDAO()
			: base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
		{
		}

		public static new BusDAO Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new BusDAO();
				}

				return (BusDAO)_instance;
			}
		}
		#endregion

		public List<Bus> BusesGet(long? ID = null)
		{
			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();

			if (ID.HasValue)
				parameters.AddInputParameter("busId", DbType.Int32, ID.Value);
			else
				parameters.AddInputParameter("busId", DbType.Int32, null);

			return this.Execute_GetList(CommandType.StoredProcedure, "sp_buses_get", parameters);

		}

		public long BusAdd(Bus bus)
		{
			if (bus == null)
				throw new ArgumentNullException("bus");
			if (bus.Owner == null)
				throw new ArgumentNullException("bus.owner");

			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();
			parameters.AddInputParameter("manufacter", DbType.String, bus.Manufacter);
			parameters.AddInputParameter("model", DbType.String, bus.Model);
			parameters.AddInputParameter("regNumber", DbType.String, bus.RegNumber);
			parameters.AddInputParameter("passengersCount", DbType.Int32, bus.PassengersCount);
			parameters.AddInputParameter("owner_Id", DbType.Int32, bus.OwnerId);
			long res=this.Execute_ScalarStoredProcedure<long>(CommandType.StoredProcedure, "sp_bus_add", parameters);

            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Buses);

            return res;
		}

		public void BusDelete(Bus bus)
		{
			if (bus == null)
				throw new ArgumentNullException("bus");
			if (bus.Id == 0)
				throw new ArgumentNullException("bus.Id");

			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();
			parameters.AddInputParameter("bus_Id", DbType.Int32, bus.Id);
			this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_bus_delete", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Buses);
		}

		public void BusEdit(Bus bus)
		{
			if (bus == null)
				throw new ArgumentNullException("bus");
			if (bus.Id == 0)
				throw new ArgumentNullException("bus.Id");

			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();
			parameters.AddInputParameter("busId", DbType.Int32, bus.Id);
			parameters.AddInputParameter("manufacter", DbType.String, bus.Manufacter);
			parameters.AddInputParameter("model", DbType.String, bus.Model);
			parameters.AddInputParameter("regNumber", DbType.String, bus.RegNumber);
			parameters.AddInputParameter("passengersCount", DbType.Int32, bus.PassengersCount);
			parameters.AddInputParameter("owner_Id", DbType.Int32, bus.OwnerId);
			this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_bus_edit", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Buses);
		}

        public List<Bus> GetAvailableBuses(StationSchedule ss, DateTime tripdate, int minSeatsCount)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("ss_Id", DbType.Int32, ss.Id);
            parameters.AddInputParameter("tDate", DbType.DateTime, tripdate);
            parameters.AddInputParameter("minSeats", DbType.Int32, minSeatsCount);

            return this.Execute_GetList(CommandType.StoredProcedure, "sp_buses_get_available", parameters);

        }
	}
}
