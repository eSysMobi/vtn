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
	public class OwnerMaterializator
		:IMaterializator<Owner>
	{

        public Owner Materialize(DataReaderAdapter dataReader)
        {
            throw new NotImplementedException();
        }

        public List<Owner> Materialize_List(DataReaderAdapter dataReader)
        {
            List<Owner> owners = new List<Owner>();

            while (dataReader.Read())
            {
                Owner o = dataReader.ReadObject<Owner>();
                o.authorities = DriverAuthorityDAO.Instance.DriverAuthorityGet(o);
                owners.Add(o);
            }

            dataReader.NextResult();
            while (dataReader.Read())
            {
                DirPosition dp = dataReader.ReadObject<DirPosition>();
                owners
                    .Where(o => o._dirPosition == dp.Id)
                    .ToList()
                    .ForEach(oo => oo.DirPosition = dp);
            }


            return owners;
        }
    }
		
	public class OwnerDAO
		:ItemDAO<Owner,OwnerMaterializator>
	{
		#region .ctor & instance
		protected OwnerDAO()
			: base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
		{
		}

		public static new OwnerDAO Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new OwnerDAO();
				}

				return (OwnerDAO)_instance;
			}
		}
		#endregion

		public List<Owner> OwnersGet(long? ID = null)
		{
			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();

			if (ID.HasValue)
				parameters.AddInputParameter("OwnerId", DbType.Int32, ID.Value);
			else
				parameters.AddInputParameter("OwnerId", DbType.Int32, null);

			return this.Execute_GetList(CommandType.StoredProcedure, "sp_owners_get", parameters);

		}

		public long OwnerAdd(Owner owner)
		{
			if (owner == null)
				throw new ArgumentNullException("owner");
			if (string.IsNullOrWhiteSpace(owner.Name))
				throw new ArgumentNullException("owner.Name");

			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();
			parameters.AddInputParameter("ownerName", DbType.String, owner.Name);
			parameters.AddInputParameter("numSv", DbType.String, owner.NumSv);
			parameters.AddInputParameter("ogrn", DbType.String, owner.OGRN);
            parameters.AddInputParameter("inn", DbType.String, owner.INN);
            parameters.AddInputParameter("addr", DbType.String, owner.Address);
			parameters.AddInputParameter("docNum", DbType.String, owner.DocNum);
			parameters.AddInputParameter("docDate", DbType.DateTime, owner.DocDate);
            parameters.AddInputParameter("docEndDate", DbType.DateTime, owner.DocEndDate);
            parameters.AddInputParameter("dirPos", DbType.Int32, owner._dirPosition);
			parameters.AddInputParameter("dirName", DbType.String, owner.DirName);
			parameters.AddInputParameter("dirSurname", DbType.String, owner.DirSurname);
			parameters.AddInputParameter("dirPatronymic", DbType.String, owner.DirPatronymic);
            parameters.AddInputParameter("feeType", DbType.Int32, owner.FeeType);
            parameters.AddInputParameter("feeVal", DbType.Double, owner.FeeAmount);
			long res = this.Execute_ScalarStoredProcedure<long>(CommandType.StoredProcedure, "sp_owners_add", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Owners);
            return res;
		}

		public void OwnerDelete(Owner owner)
		{
			if (owner == null)
				throw new ArgumentNullException("owner");
			if (owner.Id == 0)
				throw new ArgumentNullException("owner.Id");

			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();
			parameters.AddInputParameter("owner_Id", DbType.Int32, owner.Id);
			this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_owners_delete", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Owners);
		}

		public void OwnerEdit(Owner owner)
		{
			if (owner == null)
				throw new ArgumentNullException("owner");
			if (owner.Id == 0)
				throw new ArgumentNullException("owner.Id");

			List<MySqlParameter> parameters = new List<MySqlParameter>();
			parameters.AddErrorOutputParameter();
			parameters.AddInputParameter("ownerId", DbType.Int32, owner.Id);
			parameters.AddInputParameter("ownerName", DbType.String, owner.Name);
			parameters.AddInputParameter("numSv", DbType.String, owner.NumSv);
			parameters.AddInputParameter("ogrn", DbType.String, owner.OGRN);
            parameters.AddInputParameter("inn", DbType.String, owner.INN);
            parameters.AddInputParameter("addr", DbType.String, owner.Address);
			parameters.AddInputParameter("docNum", DbType.String, owner.DocNum);
			parameters.AddInputParameter("docDate", DbType.DateTime, owner.DocDate);
            parameters.AddInputParameter("docEndDate", DbType.DateTime, owner.DocEndDate);
            parameters.AddInputParameter("dirPos", DbType.Int32, owner._dirPosition);
			parameters.AddInputParameter("dirName", DbType.String, owner.DirName);
			parameters.AddInputParameter("dirSurname", DbType.String, owner.DirSurname);
			parameters.AddInputParameter("dirPatronymic", DbType.String, owner.DirPatronymic);
            parameters.AddInputParameter("feeType", DbType.Int32, owner.FeeType);
            parameters.AddInputParameter("feeVal", DbType.Double, owner.FeeAmount);
			this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_owners_edit", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Owners);
		}
	}
}
