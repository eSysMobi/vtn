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
    public class DriverAuthorityMaterializator
        :IMaterializator<DriverAuthority>
    {
        public DriverAuthority Materialize(DataReaderAdapter dataReader)
        {
            throw new NotImplementedException();
        }

        public List<DriverAuthority> Materialize_List(DataReaderAdapter dataReader)
        {
            List<DriverAuthority> ret = new List<DriverAuthority>();

            
            while (dataReader.Read())
            {
                DriverAuthority da = dataReader.ReadObject<DriverAuthority>();
                ret.Add(da);
            }
            dataReader.NextResult();

            while (dataReader.Read())
            {
                Driver d = dataReader.ReadObject<Driver>();
                ret
                    .Where(v => v._driverId == d.Id)
                    .ToList()
                    .ForEach(vv => vv.Driver=d);
            }

            dataReader.NextResult();

            while (dataReader.Read())
            {
                Owner o = dataReader.ReadObject<Owner>();
                ret
                    .Where(v => v._ownerId == o.Id)
                    .ToList()
                    .ForEach(vv => vv.Owner = o);
            }

            dataReader.NextResult();
            while (dataReader.Read())
            {
                DirPosition dp = dataReader.ReadObject<DirPosition>();
                ret
                    .Where(v => v.Owner._dirPosition == dp.Id)
                    .ToList()
                    .ForEach(vv => vv.Owner.DirPosition = dp);
            }

            return ret;
        }
    }

    public class DriverAuthorityDAO
         : ItemDAO<DriverAuthority, DriverAuthorityMaterializator>
    {
        #region .ctor & instance
        protected DriverAuthorityDAO()
            : base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
        {
        }

        public static new DriverAuthorityDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DriverAuthorityDAO();
                }

                return (DriverAuthorityDAO)_instance;
            }
        }
        #endregion

        public List<DriverAuthority> DriverAuthorityGet(long? ID = null)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();

            parameters.AddInputParameter("drvId", DbType.Int32, null);
            parameters.AddInputParameter("ownId", DbType.Int32, null);

            if (ID.HasValue)
                parameters.AddInputParameter("daId", DbType.Int32, ID.Value);
            else
                parameters.AddInputParameter("daId", DbType.Int32, null);

            return this.Execute_GetList(CommandType.StoredProcedure, "sp_da_get", parameters);
        }

        public List<DriverAuthority> DriverAuthorityGet(Driver driver)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();

            if (driver.Id == 0)
                throw new ArgumentException("Incorrect driver");

            parameters.AddInputParameter("daId", DbType.Int32, null);
            parameters.AddInputParameter("drvId", DbType.Int32, driver.Id);
            parameters.AddInputParameter("ownId", DbType.Int32, null);

            return this.Execute_GetList(CommandType.StoredProcedure, "sp_da_get", parameters);
        }

        public List<DriverAuthority> DriverAuthorityGet(Owner owner)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();

            if (owner.Id == 0)
                throw new ArgumentException("Incorrect owner");

            parameters.AddInputParameter("daId", DbType.Int32, null);
            parameters.AddInputParameter("drvId", DbType.Int32, null);
            parameters.AddInputParameter("ownId", DbType.Int32, owner.Id);

            return this.Execute_GetList(CommandType.StoredProcedure, "sp_da_get", parameters);
        }

        public long DriverAuthorityAdd(int driverId, int ownerId, string num, DateTime date)
        {
            if (driverId == 0)
                throw new ArgumentNullException("driver");
            if (ownerId == 0)
                throw new ArgumentNullException("owner");
            if (string.IsNullOrWhiteSpace(num))
                throw new ArgumentNullException("number");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("drvId", DbType.Int32, driverId);
            parameters.AddInputParameter("ownId", DbType.Int32, ownerId);
            parameters.AddInputParameter("num", DbType.String, num);
            parameters.AddInputParameter("dt", DbType.DateTime, date);

            long res = this.Execute_ScalarStoredProcedure<long>(CommandType.StoredProcedure, "sp_da_add", parameters);

            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.DriverAuthorities);
            return res;
        }

        public void DriverAuthorityDelete(int daId)
        {
            if (daId == 0)
                throw new ArgumentNullException("driver authority ID");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("daId", DbType.Int32, daId);
            
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_da_delete", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.DriverAuthorities);
        }

    }
}
