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
    public class DriverMaterializator
        : MaterializatorBase<Driver>
    {
        public override Driver ReadSingleObject(DataReaderAdapter dataReader)
        {
            Driver ret = dataReader.ReadObject<Driver>();

            ret.authorities = DriverAuthorityDAO.Instance.DriverAuthorityGet(ret);

            return ret;
        }
    }

    public class DriverDAO
        : ItemDAO<Driver, DriverMaterializator>
    {
        #region .ctor & instance
        protected DriverDAO()
            : base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
        {
        }

        public static new DriverDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DriverDAO();
                }

                return (DriverDAO)_instance;
            }
        }
        #endregion

        public List<Driver> DriversGet(long? ID = null)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();

            if (ID.HasValue)
                parameters.AddInputParameter("driverId", DbType.Int32, ID.Value);
            else
                parameters.AddInputParameter("driverId", DbType.Int32, null);

            return this.Execute_GetList(CommandType.StoredProcedure, "sp_drivers_get", parameters);

        }

        public List<Driver> DriversGetByBus(Bus bus)
        {
            if (bus == null)
                throw new ArgumentNullException("bus");
            if (bus.Id == 0)
                throw new ArgumentNullException("bus.ID");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("bus_Id", DbType.Int32, bus.Id);

            return this.Execute_GetList(CommandType.StoredProcedure, "sp_drivers_get_by_bus", parameters);
        }

        public long DriverAdd(Driver driver)
        {
            if (driver == null)
                throw new ArgumentNullException("driver");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("Name", DbType.String, driver.Name);
            parameters.AddInputParameter("Surname", DbType.String, driver.Surname);
            parameters.AddInputParameter("Patronymic", DbType.String, driver.Patronymic);
            parameters.AddInputParameter("pSer", DbType.String, driver.PassportSer);
            parameters.AddInputParameter("pNum", DbType.String, driver.PassportNum);
            parameters.AddInputParameter("pDate", DbType.DateTime, driver.PassportDate);
            parameters.AddInputParameter("pIssuer", DbType.String, driver.PassportIssuer);
            long res = this.Execute_ScalarStoredProcedure<long>(CommandType.StoredProcedure, "sp_driver_add", parameters);

            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Drivers);
            return res;
        }

        public void DriverDelete(Driver driver)
        {
            if (driver == null)
                throw new ArgumentNullException("driver");
            if (driver.Id == 0)
                throw new ArgumentNullException("driver.Id");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("driver_Id", DbType.Int32, driver.Id);
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_driver_delete", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Drivers);
        }

        public void DriverEdit(Driver driver)
        {
            if (driver == null)
                throw new ArgumentNullException("driver");
            if (driver.Id == 0)
                throw new ArgumentNullException("driver.Id");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("driverId", DbType.Int32, driver.Id);
            parameters.AddInputParameter("Name", DbType.String, driver.Name);
            parameters.AddInputParameter("Surname", DbType.String, driver.Surname);
            parameters.AddInputParameter("Patronymic", DbType.String, driver.Patronymic);
            parameters.AddInputParameter("pSer", DbType.String, driver.PassportSer);
            parameters.AddInputParameter("pNum", DbType.String, driver.PassportNum);
            parameters.AddInputParameter("pDate", DbType.DateTime, driver.PassportDate);
            parameters.AddInputParameter("pIssuer", DbType.String, driver.PassportIssuer);
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_driver_edit", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Drivers);
        }
    }
}
