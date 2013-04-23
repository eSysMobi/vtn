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
    public class TripScheduleFactMaterializator
        : IMaterializator<TripScheduleFact>
    {
        public TripScheduleFact Materialize(DataReaderAdapter dataReader)
        {
            throw new NotImplementedException();
        }

        public List<TripScheduleFact> Materialize_List(DataReaderAdapter dataReader)
        {
            List<TripScheduleFact> tsf = new List<TripScheduleFact>();

            while (dataReader.Read())
            {
                TripScheduleFact _tsf = dataReader.ReadObject<TripScheduleFact>();

                _tsf.TS = TripScheduleDAO.Instance.TripSchedulesGet(_tsf.TSId)[0];
                _tsf.FactBus = BusDAO.Instance.BusesGet(_tsf.FactBusId)[0];
                _tsf.FactDriver1 = DriverDAO.Instance.DriversGet(_tsf.FactDriverId_1)[0];
				_tsf.FactDriver2 = DriverDAO.Instance.DriversGet(_tsf.FactDriverId_2).FirstOrDefault();
                _tsf.Operator = OperatorDAO.Instance.OperatorsGet(_tsf.OperatorId)[0];
                _tsf.DepartureTown = TownDAO.Instance.TownsGet(_tsf.DepartureTownId)[0];
			
                tsf.Add(_tsf);
            }

             /*
             TODO: ускорить как все остальные DAO
            dataReader.NextResult();
           
            while (dataReader.Read())
            {
                Owner o = dataReader.ReadObject<Owner>();
                buses
                    .Where(b => b.OwnerId == o.Id)
                    .ToList()
                    .ForEach(bb => bb.Owner = o);
            }*/

            return tsf;

        }
    }

    public class TripScheduleFactDAO
        :ItemDAO<TripScheduleFact,TripScheduleFactMaterializator>
	{
		#region .ctor & instance
        protected TripScheduleFactDAO()
			: base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
		{
		}

        public static new TripScheduleFactDAO Instance
		{
			get
			{
				if (_instance == null)
				{
                    _instance = new TripScheduleFactDAO();
				}

                return (TripScheduleFactDAO)_instance;
			}
		}
		#endregion

        public List<TripScheduleFact> TripScheduleFactGet(long? ID = null)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();

            if (ID.HasValue)
                parameters.AddInputParameter("tsfId", DbType.Int32, ID.Value);
            else
                parameters.AddInputParameter("tsfId", DbType.Int32, null);

            return this.Execute_GetList(CommandType.StoredProcedure, "sp_tsf_get", parameters);
        }

        public List<TripScheduleFact> TripScheduleFactGet(DateTime dt)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("dt", DbType.DateTime, dt);

            return this.Execute_GetList(CommandType.StoredProcedure, "sp_tsf_get_dt", parameters);
        }

        public long TripScheduleFactAdd(TripScheduleFact tsf)
        {
            if (tsf == null)
                throw new ArgumentNullException("tsf");
            if (tsf.FactBus == null)
                throw new ArgumentNullException("tsf.FactBus");
            if (tsf.FactDriver1 == null && tsf.FactDriver2 == null)
                throw new ArgumentNullException("no fact drivers");
            if (tsf.Operator == null)
                throw new ArgumentNullException("tsf.Operator");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("ts_id", DbType.Int32, tsf.TSId);
            parameters.AddInputParameter("busId", DbType.Int32, tsf.FactBusId);
            parameters.AddInputParameter("drvId_1", DbType.Int32, tsf.FactDriver1 == null ? 0 : tsf.FactDriverId_1);
            parameters.AddInputParameter("drvId_2", DbType.Int32, tsf.FactDriver2 == null ? 0 : tsf.FactDriverId_2);
            parameters.AddInputParameter("deptownId", DbType.Int32, tsf.DepartureTownId);
            parameters.AddInputParameter("depTime", DbType.DateTime, tsf.FactDepartureTime);
            parameters.AddInputParameter("opId", DbType.Int32, tsf.OperatorId);
            parameters.AddInputParameter("opTime", DbType.DateTime, tsf.OperationTime);

            return this.Execute_ScalarStoredProcedure<long>(CommandType.StoredProcedure, "sp_tsf_add", parameters);
        }

        public void TripScheduleFactDelete(TripScheduleFact tsf)
        {
            if (tsf == null)
                throw new ArgumentNullException("tsf");
            if (tsf.Id == 0)
                throw new ArgumentNullException("tsf.ID");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("tsf_Id", DbType.Int32, tsf.Id);
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_tsf_delete", parameters);

        }
    }
}
