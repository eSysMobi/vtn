using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VlastelinServer.DAO.DBAccess;
using Vlastelin.Data.Model;
using Vlastelin.Common.Constants;
using Vlastelin.Common;
using MySql.Data.MySqlClient;
using System.Data;

namespace VlastelinServer.DAO
{
    public class PassengerMaterializator :
        MaterializatorBase<Passenger>
    {
        public override Passenger ReadSingleObject(DataReaderAdapter dataReader)
        {
            return dataReader.ReadObject<Passenger>();
        }
    }
    
    public class PassengerDAO :
        ItemDAO<Passenger,PassengerMaterializator>
    {

        #region .ctor & instance
        protected PassengerDAO()
			: base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
		{
		}

        public static new PassengerDAO Instance
		{
			get
			{
				if (_instance == null)
				{
                    _instance = new PassengerDAO();
				}

                return (PassengerDAO)_instance;
			}
		}
		#endregion

        public List<Passenger> PassengersGet(long? ID = null)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();

            if (ID.HasValue)
                parameters.AddInputParameter("pass_id", DbType.Int32, ID.Value);
            else
                parameters.AddInputParameter("pass_id", DbType.Int32, null);

            return this.Execute_GetList(CommandType.StoredProcedure, "sp_passengers_get", parameters);

        }

        public long PassengerAdd(Passenger Passenger)
        {
            if (Passenger == null)
                throw new ArgumentNullException("Passenger");
            if (string.IsNullOrWhiteSpace(Passenger.Name))
                throw new ArgumentNullException("Passenger.Name");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("nm", DbType.String, Passenger.Name);
            parameters.AddInputParameter("snm", DbType.String, Passenger.Surname);
            parameters.AddInputParameter("patr", DbType.String, Passenger.Patronymic);
            parameters.AddInputParameter("dType", DbType.Int32, Passenger.DocType);
            parameters.AddInputParameter("dSer", DbType.String, Passenger.DocSer);
            parameters.AddInputParameter("dNum", DbType.Int32, Passenger.DocNum);
            parameters.AddInputParameter("dDate", DbType.DateTime, Passenger.DocDate);
            long res = this.Execute_ScalarStoredProcedure<long>(CommandType.StoredProcedure, "sp_passengers_add", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Passengers);
            return res;
        }

    }
}
