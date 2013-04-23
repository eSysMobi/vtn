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
    public class MembershipUserMaterializator
        : MaterializatorBase<VlastelinMembershipUser>
    {
        public override VlastelinMembershipUser ReadSingleObject(DataReaderAdapter dataReader)
        {
            return dataReader.ReadObject<VlastelinMembershipUser>();
        }
    }

    public class MembershipUserDAO
        : ItemDAO<VlastelinMembershipUser, MembershipUserMaterializator>
    {
        #region .ctor & instance
        protected MembershipUserDAO()
            : base(DatabaseConstants.DatabaseAlias, new DatabaseExceptionHanlder())
        {
        }

        public static new MembershipUserDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MembershipUserDAO();
                }

                return (MembershipUserDAO)_instance;
            }
        }
        #endregion

        public bool SetPassword(string userName, string appName, string newPasswordValue)
        {
            string query="UPDATE MembershipUsers " +
                    " SET Password = @Pwd, LastPasswordChangedDate = @LastPwdChangedDate " +
                    " WHERE Username = @Uname AND appName = @AppName";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("Pwd", DbType.String, newPasswordValue);
            parameters.AddInputParameter("LastPwdChangedDate", DbType.DateTime, DateTime.Now);
            parameters.AddInputParameter("Uname", DbType.String, userName);
            parameters.AddInputParameter("AppName", DbType.String, appName);

            this.Execute_StoredProcedure(System.Data.CommandType.Text, query, parameters);

            return true;
        }

        public bool SetQuestionAndAnswer(string userName, string appName, string question, string answer)
        {
            string query = "UPDATE MembershipUsers " +
                    " SET PasswordQuestion = question, PasswordAnswer = answer " +
                    " WHERE Username = Uname AND appName = AppName";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("question", DbType.String, question);
            parameters.AddInputParameter("answer", DbType.String, answer);
            parameters.AddInputParameter("Uname", DbType.String, userName);
            parameters.AddInputParameter("AppName", DbType.String, appName);

            int rowsAffected = this.Execute_ScalarStoredProcedure<int>(System.Data.CommandType.Text, query, parameters);

            return (rowsAffected > 0);
        }

        public bool CreateUser(VlastelinMembershipUser newUser)
        {
            string query = "INSERT INTO MembershipUsers " +
                      " (isOnline, id, Username, Password, Email, PasswordQuestion, " +
                      " PasswordAnswer, IsApproved," +
                      " Comment, CreationDate, LastPasswordChangedDate, LastActivityDate," +
                      " appName, IsLocked, LastLockedOutDate," +
                      " FailedPasswordAttemptCount, FailedPasswordAttemptWindowStart, " +
                      " FailedPasswordAnswerAttemptCount, FailedPasswordAnswerAttemptWindowStart)" +
                      " Values(False, @pkId, @Uname, @Pwd, @email, @PwdQuestion, @PwdAnswer, @approved, @cmnt, @CreationDt," +
                      " @LastPwdChangedDate, @LastActDate, @AppName, @Locked, @LastLockedDate, " +
                      " @FailedPwdAttemptCount, @FailedPwdAttemptWindowStart," +
                      " @FailedPwdAnswerAttemptCount, @FailedPwdAnswerAttemptWindowStart )";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("pkId", DbType.String, newUser.id);
            parameters.AddInputParameter("Uname", DbType.String, newUser.Username);
            parameters.AddInputParameter("Pwd", DbType.String, newUser.Password);
            parameters.AddInputParameter("email", DbType.String, newUser.Email);
            parameters.AddInputParameter("PwdQuestion", DbType.String, newUser.PasswordQuestion);
            parameters.AddInputParameter("PwdAnswer", DbType.String, newUser.PasswordAnswer);
            parameters.AddInputParameter("approved", DbType.Boolean, newUser.isApproved);
            parameters.AddInputParameter("cmnt", DbType.String, newUser.Comment);
            parameters.AddInputParameter("CreationDt", DbType.DateTime, newUser.CreationDate);
            parameters.AddInputParameter("LastPwdChangedDate", DbType.DateTime, newUser.LastPasswordChangedDate);
            parameters.AddInputParameter("LastActDate", DbType.DateTime, newUser.LastActivityDate);
            parameters.AddInputParameter("AppName", DbType.String, newUser.ApplicationName);
            parameters.AddInputParameter("Locked", DbType.Boolean, newUser.isLocked);
            parameters.AddInputParameter("LastLockedDate", DbType.DateTime, newUser.LastLockedOutDate);
            parameters.AddInputParameter("FailedPwdAttemptCount", DbType.Int32, newUser.FailedPasswordAnswerAttemptCount);
            parameters.AddInputParameter("FailedPwdAttemptWindowStart", DbType.DateTime, newUser.FailedPasswordAnswerAttemptWindowStart);
            parameters.AddInputParameter("FailedPwdAnswerAttemptCount", DbType.Int32, newUser.FailedPasswordAttemptCount);
            parameters.AddInputParameter("FailedPwdAnswerAttemptWindowStart", DbType.DateTime, newUser.FailedPasswordAttemptWindowStart);

            int rowsAffected = this.Execute_StoredProcedure(System.Data.CommandType.Text, query, parameters);

            return (rowsAffected > 0);
        }

        public bool CreateUser(string id, string login, string password)
        {
            VlastelinMembershipUser newUser = new VlastelinMembershipUser();
            newUser.id = id;
            newUser.Username = login;
            newUser.Password = password;
            newUser.Email = "";
            newUser.PasswordQuestion = "";
            newUser.PasswordAnswer = "";
            newUser.isApproved = true;
            newUser.Comment = "";
            newUser.CreationDate = DateTime.Now;
            newUser.LastPasswordChangedDate = DateTime.Now;
            newUser.LastActivityDate = DateTime.Now;
            newUser.ApplicationName = "VlastelinServer";
            newUser.isLocked = false;
            newUser.LastLockedOutDate = DateTime.Now;
            newUser.FailedPasswordAnswerAttemptCount = 0;
            newUser.FailedPasswordAnswerAttemptWindowStart = DateTime.Now;
            newUser.FailedPasswordAttemptCount = 0;
            newUser.FailedPasswordAttemptWindowStart = DateTime.Now;

            return CreateUser(newUser);
        }

        public bool DeleteUser(string userName, string appName, bool deleteAllRelatedData)
        {
            string query = "DELETE FROM MembershipUsers " +
                    " WHERE Username = Uname AND appName = AppName";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("Uname", DbType.String, userName);
            parameters.AddInputParameter("AppName", DbType.String, appName);

            int rowsAffected = this.Execute_StoredProcedure(System.Data.CommandType.Text, query, parameters);

            if (deleteAllRelatedData)
            {
                // some additional operations here
            }

            return (rowsAffected > 0);
        }

        public bool EditUser(VlastelinMembershipUser user)
        {

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("Uname", DbType.String, user.Username);
            parameters.AddInputParameter("Pwd", DbType.String, user.Password);
            parameters.AddInputParameter("email", DbType.String, user.Email);
            parameters.AddInputParameter("PwdQuestion", DbType.String, user.PasswordQuestion);
            parameters.AddInputParameter("PwdAnswer", DbType.String, user.PasswordAnswer);
            parameters.AddInputParameter("approved", DbType.Boolean, user.isApproved);
            parameters.AddInputParameter("cmnt", DbType.String, user.Comment);
            parameters.AddInputParameter("CreationDt", DbType.DateTime, user.CreationDate);
            parameters.AddInputParameter("LastPwdChangedDate", DbType.DateTime, user.LastPasswordChangedDate);
            parameters.AddInputParameter("LastActDate", DbType.DateTime, user.LastActivityDate);
            parameters.AddInputParameter("AppName", DbType.String, user.ApplicationName);
            parameters.AddInputParameter("Locked", DbType.Boolean, user.isLocked);
            parameters.AddInputParameter("LastLockedDate", DbType.DateTime, user.LastLockedOutDate);
            parameters.AddInputParameter("LastLoginDt", DbType.DateTime, user.LastLoginDate);
            parameters.AddInputParameter("LastLockOutDt", DbType.DateTime, user.LastLockOutDate);
            parameters.AddInputParameter("FailedPwdAttemptCount", DbType.Int32, user.FailedPasswordAnswerAttemptCount);
            parameters.AddInputParameter("FailedPwdAttemptWindowStart", DbType.DateTime, user.FailedPasswordAnswerAttemptWindowStart);
            parameters.AddInputParameter("FailedPwdAnswerAttemptCount", DbType.Int32, user.FailedPasswordAttemptCount);
            parameters.AddInputParameter("FailedPwdAnswerAttemptWindowStart", DbType.DateTime, user.FailedPasswordAttemptWindowStart);
            parameters.AddInputParameter("UserId", DbType.String, user.id);

            int rowsAffected = this.Execute_StoredProcedure(System.Data.CommandType.StoredProcedure, "sp_musers_edit", parameters);

            return (rowsAffected > 0);
        }

        public List<VlastelinMembershipUser> GetUsers(string appName, Guid? pkId, string userName, string email)
        {
            StringBuilder query = new StringBuilder("SELECT * FROM MembershipUsers WHERE appName = AppName");

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("AppName", DbType.String, appName);

            if (pkId.HasValue)
                query.Append(string.Format(" AND id='{0}'", pkId.Value.ToString()));
            if(!string.IsNullOrWhiteSpace(userName))
                query.Append(string.Format(" AND UserName='{0}'", userName));
            if (!string.IsNullOrWhiteSpace(email))
                query.Append(string.Format(" AND Email='{0}'", email));

            return this.Execute_GetList(CommandType.Text, query.ToString(), parameters);
        }

        public List<VlastelinMembershipUser> GetUsersOnline(string appName, TimeSpan onlineSpan)
        {
            /* Второй вариант проверки:
             *  TimeSpan onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
                DateTime compareTime = DateTime.Now.Subtract(onlineSpan);
                SqlCommand cmd = new SqlCommand("SELECT Count(*) FROM membershipUsers " +
                        " WHERE LastActivityDate > @CompareDate AND appName = @ApplicationName", conn);
             */
            //TimeSpan onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
            DateTime compareTime = DateTime.Now.Subtract(onlineSpan);

            string query = "SELECT * FROM MembershipUsers " +
                    " WHERE appName = AppName" +
                    " AND isOnline = True" +
                    " AND LastActivityDate > compareTime";

            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            parameters.AddInputParameter("AppName", DbType.String, appName);
            parameters.AddInputParameter("compareTime", DbType.DateTime, compareTime);
            
            return this.Execute_GetList(CommandType.Text, query.ToString(), parameters);
        }

        internal VlastelinMembershipUser GetUser(string guid)
        {
            string sql = "select * from MembershipUsers where id='" + guid + "'";
            List<MySqlParameter> parameters = new List<MySqlParameter>();
            parameters.AddErrorOutputParameter();
            return this.Execute_Get(CommandType.Text, sql, parameters);
        }
    }
}
