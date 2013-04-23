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
    public class BranchMaterializator
        : IMaterializator<Branch>
    {
        public Branch Materialize(DataReaderAdapter dataReader)
        {
            throw new NotImplementedException();
        }

        public List<Branch> Materialize_List(DataReaderAdapter dataReader)
        {
            List<Branch> branches = new List<Branch>();

            while (dataReader.Read())
            {
                Branch br = dataReader.ReadObject<Branch>();
                branches.Add(br);
            }
            dataReader.NextResult();

            while (dataReader.Read())
            {
                Town t = dataReader.ReadObject<Town>();
                branches
                    .Where(b => b.townId == t.Id)
                    .ToList()
                    .ForEach(bb => bb.Town = t);
            }

            return branches;

        }
    }

    public class BranchDAO:
        ItemDAO<Branch,BranchMaterializator>
	{
		#region .ctor & instance
		protected BranchDAO()
			: base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
		{
		}

		public static new BranchDAO Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new BranchDAO();
				}

				return (BranchDAO)_instance;
			}
		}
		#endregion

        public List<Branch> BranchesGet(long? ID = null)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();

            if (ID.HasValue)
                parameters.AddInputParameter("brId", DbType.Int32, ID.Value);
            else
                parameters.AddInputParameter("brId", DbType.Int32, null);

            return this.Execute_GetList(CommandType.StoredProcedure, "sp_branches_get", parameters);

        }

        public long BranchAdd(Branch branch)
        {
            if(string.IsNullOrWhiteSpace(branch.Name))
                throw new ArgumentNullException("branch.Name");
            if (string.IsNullOrWhiteSpace(branch.Address))
                throw new ArgumentNullException("branch.Address");
            if (branch.townId==0)
                throw new ArgumentNullException("branch.townId");
            

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();

            parameters.AddInputParameter("nm", DbType.String, branch.Name);
            parameters.AddInputParameter("addr", DbType.String, branch.Address);
            parameters.AddInputParameter("ph", DbType.String, branch.Phone);
            parameters.AddInputParameter("tId", DbType.Int32, branch.townId);

            long res = this.Execute_ScalarStoredProcedure<long>(CommandType.StoredProcedure, "sp_branch_add", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Branches);
            return res;
        }

        public void BranchDelete(Branch branch)
        {
            if(branch.Id==0)
                throw new ArgumentNullException("branch.Id");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("br_Id", DbType.Int32, branch.Id);

            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_branch_delete", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Branches);
        }

        public void BranchEdit(Branch branch)
        {
            if (branch.Id == 0)
                throw new ArgumentNullException("branch.Id");
            if (string.IsNullOrWhiteSpace(branch.Name))
                throw new ArgumentNullException("branch.Name");
            if (string.IsNullOrWhiteSpace(branch.Address))
                throw new ArgumentNullException("branch.Address");
            if (branch.townId == 0)
                throw new ArgumentNullException("branch.townId");


            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("br_Id", DbType.Int32, branch.Id);
            parameters.AddInputParameter("nm", DbType.String, branch.Name);
            parameters.AddInputParameter("addr", DbType.String, branch.Address);
            parameters.AddInputParameter("ph", DbType.String, branch.Phone);
            parameters.AddInputParameter("tId", DbType.Int32, branch.townId);

            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_branch_edit", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Branches);
        }
    }
}
