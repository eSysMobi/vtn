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
    public class OperatorMaterializator
        : MaterializatorBase<Operator>
    {
        public override Operator ReadSingleObject(DataReaderAdapter dataReader)
        {
            Operator ret = dataReader.ReadObject<Operator>();
            ret.Branch = BranchDAO.Instance.BranchesGet(ret.branchId)[0];
            ret.User = MembershipUserDAO.Instance.GetUser(ret._userId);
            string role = MembershipRoleDAO.Instance.GetRolesForUser(ret.User.Username).First().Name;
            ret.Role = (Roles)Enum.Parse(typeof(Roles), role);
            return ret;
        }
    }

    public class OperatorDAO
        : ItemDAO<Operator, OperatorMaterializator>
    {
        #region .ctor & instance
        protected OperatorDAO()
            : base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
        {
        }

        public static new OperatorDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new OperatorDAO();
                }

                return (OperatorDAO)_instance;
            }
        }
        #endregion

        public List<Operator> OperatorsGet(long? ID = null)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();

            if (ID.HasValue)
                parameters.AddInputParameter("opId", DbType.Int32, ID.Value);
            else
                parameters.AddInputParameter("opId", DbType.Int32, null);

            return this.Execute_GetList(CommandType.StoredProcedure, "sp_operators_get", parameters);

        }

        public long OperatorAdd(Operator op, string login, string password)
        {
            if (op == null)
                throw new ArgumentNullException("Operator");

            if (string.IsNullOrWhiteSpace(op._userId))
                op._userId = Guid.NewGuid().ToString();

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("nm", DbType.String, op.Name);
            parameters.AddInputParameter("surNm", DbType.String, op.Surname);
            parameters.AddInputParameter("patr", DbType.String, op.Patronymic);
            parameters.AddInputParameter("brId", DbType.Int32, op.branchId);
            parameters.AddInputParameter("user_Id", DbType.String, op._userId);
            long res = this.Execute_ScalarStoredProcedure<long>(CommandType.StoredProcedure, "sp_operator_add", parameters);
            MembershipUserDAO.Instance.CreateUser(op._userId, login,password);
            MembershipRoleDAO.Instance.EnsureUserInRole(login, op.Role);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Operators);
            return res;
        }

        public void OperatorDelete(Operator op)
        {
            if (op == null)
                throw new ArgumentNullException("Operator");
            if (op.Id == 0)
                throw new ArgumentNullException("Operator.Id");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("opId", DbType.Int32, op.Id);
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_operator_delete", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Operators);
        }

        public void OperatorEdit(Operator op)
        {
            if (op == null)
                throw new ArgumentNullException("Operator");
            if (op.Id == 0)
                throw new ArgumentNullException("Operator.Id");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("opId", DbType.Int32, op.Id);
            parameters.AddInputParameter("nm", DbType.String, op.Name);
            parameters.AddInputParameter("surNm", DbType.String, op.Surname);
            parameters.AddInputParameter("patr", DbType.String, op.Patronymic);
            parameters.AddInputParameter("brId", DbType.Int32, op.branchId);
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_operator_edit", parameters);
            TablesTimeDAO.Instance.SetLastModifiedTime(ModifiedObjects.Operators);
        }

        public Operator OperatorGetByLogin(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentNullException("login");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("l", DbType.String, login);
            return this.Execute_Get(CommandType.Text, "select o.* from Operators o join MembershipUsers mu on o.UserId=mu.id where mu.UserName=@l", parameters);
        }
    }
}
