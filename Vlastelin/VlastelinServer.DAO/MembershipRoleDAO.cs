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
    public class RoleMaterializator
        : MaterializatorBase<VlastelinRole>
    {
        public override VlastelinRole ReadSingleObject(DataReaderAdapter dataReader)
        {
            return dataReader.ReadObject<VlastelinRole>();
        }
    }

    public class MembershipRoleDAO
        : ItemDAO<VlastelinRole, RoleMaterializator>
    {
        #region .ctor & instance
        protected MembershipRoleDAO()
			: base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
		{
		}

        public static new MembershipRoleDAO Instance
		{
			get
			{
				if (_instance == null)
				{
                    _instance = new MembershipRoleDAO();
				}

                return (MembershipRoleDAO)_instance;
			}
		}
		#endregion

        private const string vlastelinAppName = "VlastelinServer";

        public void AddUsersToRoles(List<string> users, List<string> roles, string AppName = vlastelinAppName)
        {
            /*
             * SqlCommand cmd = new SqlCommand("INSERT INTO UsersInRoles " +
                    " (Username, Rolename, appName) " +
                    " Values(@Username, @Rolename, @ApplicationName)", conn);
             */
        }

        public void RemoveUsersFromRoles(List<string> users, List<string> roles, string AppName = vlastelinAppName)
        {
            /*
             *SqlCommand cmd = new SqlCommand("DELETE FROM UsersInRoles " +
                    " WHERE Username = @Username AND Rolename = @Rolename AND appName = @ApplicationName", conn);
             */
        }

        public void CreateRole(string role, string AppName = vlastelinAppName)
        {
            string query = "INSERT INTO MembershipRoles " +
                    " (Rolename, ApplicationName) " +
                    " Values(@rName, @AppName)";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("AppName", DbType.String, AppName);
            parameters.AddInputParameter("rName", DbType.String, role);

            this.Execute_StoredProcedure(CommandType.Text, query, parameters);
        }

        public bool DeleteRole(string role, string AppName = vlastelinAppName)
        {
            string query = "DELETE FROM MembershipRoles WHERE" +
                    " Rolename = rName AND ApplicationName = @AppName ";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("AppName", DbType.String, AppName);
            parameters.AddInputParameter("rName", DbType.String, role);

            this.Execute_StoredProcedure(CommandType.Text, query, parameters);

            query = "DELETE FROM MembershipUsersInRoles " +
                    " WHERE Rolename = rName AND ApplicationName = AppName";
            this.Execute_StoredProcedure(CommandType.Text, query, parameters);

            return true;
        }

        public List<VlastelinRole> GetRoles(string AppName = vlastelinAppName)
        {
            string query = "SELECT * FROM MembershipRoles WHERE ApplicationName = @AppName";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("AppName", DbType.String, AppName);

            return this.Execute_GetList(CommandType.Text, query, parameters);
        }

        public List<VlastelinRole> GetRolesForUser(string userName, bool exactUsernameMatch = false)
        {
            StringBuilder query = new StringBuilder("SELECT * FROM MembershipUsersInRoles WHERE ApplicationName = @AppName");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("AppName", DbType.String, vlastelinAppName);

            if (!string.IsNullOrWhiteSpace(userName))
            {
                if (exactUsernameMatch)
                    query.Append(string.Format(" AND UserName='{0}'", userName));
                else
                    query.Append(string.Format(" AND UserName LIKE '%{0}%'", userName));
            }

            return this.Execute_GetList(CommandType.Text, query.ToString(), parameters);
        }

        public List<string> GetUsersInRole(string AppName, string rolename, string userNamePattern = "")
        {
            StringBuilder query = new StringBuilder("SELECT userName FROM MembershipUsersInRoles WHERE ApplicationName = @AppName");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("AppName", DbType.String, AppName);
            
            query.Append(string.Format(" AND RoleName='{0}'", rolename));
            if(!string.IsNullOrWhiteSpace(userNamePattern))
                query.Append(string.Format(" AND Username LIKE '%{0}%'",userNamePattern));

            // trick
            MySqlDataReader dr = DBHelper.ExecuteReader(
                DBHelper.GetConnectionByName(DatabaseConstants.DatabaseAlias),
                CommandType.Text,
                query.ToString(),
                parameters);

            if (dr.HasRows)
            {
                List<string> ret = new List<string>();
                while (dr.Read())
                    ret.Add(dr.GetString(0));

                return ret;
            }

            return null;
        }

        public bool IsUserInRole(string useName, string roleName, string AppName = vlastelinAppName)
        {
            StringBuilder query = new StringBuilder("SELECT COUNT(*) FROM MembershipUsersInRoles ");

            query.Append(string.Format("WHERE ApplicationName='{0}' ", AppName));
            query.Append(string.Format("AND Username ='{0}' ", useName));
            query.Append(string.Format("AND Rolename='{0}' ", roleName));
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();

            long cnt = this.Execute_ScalarStoredProcedure<long>(CommandType.Text, query.ToString(), parameters);

            return cnt > 0;
        }

        public bool IsRoleExists(string roleName, string AppName = vlastelinAppName)
        {
            string query = "SELECT COUNT(*) FROM MembershipRoles " +
                    " WHERE Rolename = rName AND ApplicationName = @AppName";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("AppName", DbType.String, AppName);
            parameters.AddInputParameter("rName", DbType.String, roleName);

            int cnt = this.Execute_StoredProcedure(CommandType.Text, query, parameters);

            return cnt > 0;
        }

        private void RemoveUserFromRole(string login, string role)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("AppName", DbType.String, vlastelinAppName);
            parameters.AddInputParameter("login", DbType.String, login);
            parameters.AddInputParameter("role", DbType.String, role);            
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_muser_remove_role", parameters);
        }
        private void RemoveAllRolesForUser(string login)
        {
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("AppName", DbType.String, vlastelinAppName);
            parameters.AddInputParameter("login", DbType.String, login);
            parameters.AddInputParameter("role", DbType.String, null);
            this.Execute_StoredProcedure(CommandType.StoredProcedure, "sp_muser_remove_role", parameters);
        }
        private void AddUserToRole(string login, string role)
        {
            StringBuilder query = new StringBuilder("INSERT INTO MembershipUsersInRoles (RoleName,Username,ApplicationName) ");
            query.Append(string.Format("VALUES ('{0}', '{1}', '{2}')", role, login, vlastelinAppName));
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();

            this.Execute_StoredProcedure(CommandType.Text, query.ToString(), parameters);
        }

        public void EnsureUserInRole(string login, Roles role)
        {
            string roleString = role.ToString();
            if (!IsUserInRole(login, roleString))
            {
                RemoveAllRolesForUser(login);
                AddUserToRole(login, roleString);
            }
        }
    }
}
