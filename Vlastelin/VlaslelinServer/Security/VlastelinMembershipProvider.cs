using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web.Configuration;
using System.Collections.Specialized;
using System.Configuration.Provider;
using System.Configuration;
using Vlastelin.Common;
using VlastelinServer.DAO;
using Vlastelin.Data.Model;
using System.Security.Cryptography;

namespace VlastelinServer.Security
{
    public class VlastelinMembershipProvider 
       :MembershipProvider
    {

        public VlastelinMembershipProvider()
        {
        }

        private Logger logger = new Logger();
        //
        // Global connection string, generated password length, generic exception message, event log info.
        //

        private int newPasswordLength = 8;
        private string eventSource = "VlastelinMembershipProvider";
        private string eventLog = "Application";
        private string exceptionMessage = "An exception occurred. Please check the Log.";
        private string connectionString;

        //
        // Used when determining encryption key values.
        //

        private MachineKeySection machineKey;

        //
        // If false, exceptions are thrown to the caller. If true,
        // exceptions are written to the event log.
        //

        public bool WriteExceptionsToEventLog {get;set;}

        //
        // System.Configuration.Provider.ProviderBase.Initialize Method
        //

        public override void Initialize(string name, NameValueCollection config)
        {
            //
            // Initialize values from web.config.
            //

            if (config == null)
                throw new ArgumentNullException("VlastelinMembershipProvider: missing config!");

            if (name == null || name.Length == 0)
                name = "SqlMembershipProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Vlastelin Membership provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            pApplicationName = GetConfigValue(config["applicationName"],
                                            System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
            pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            pPasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
            WriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"));

            string temp_format = config["passwordFormat"];
            if (temp_format == null)
            {
                temp_format = "Hashed";
            }

            switch (temp_format)
            {
                case "Hashed":
                    pPasswordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "Encrypted":
                    pPasswordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Clear":
                    pPasswordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format not supported.");
            }

            //
            // Initialize Connection.
            //

            ConnectionStringSettings ConnectionStringSettings =
              ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

            if (ConnectionStringSettings == null || ConnectionStringSettings.ConnectionString.Trim() == "")
            {
                throw new ProviderException("Connection string cannot be blank.");
            }

            connectionString = ConnectionStringSettings.ConnectionString;


            // Get encryption and decryption key information from the configuration.
            Configuration cfg =
              WebConfigurationManager.OpenWebConfiguration(System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

            if (machineKey.ValidationKey.Contains("AutoGenerate"))
                if (PasswordFormat != MembershipPasswordFormat.Clear)
                    throw new ProviderException("Hashed or Encrypted passwords " +
                                                "are not supported with auto-generated keys.");
        }


        //
        // A helper function to retrieve config values from the configuration file.
        //

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        //
        // System.Web.Security.MembershipProvider properties.
        //


        private string pApplicationName;
        private bool pEnablePasswordReset;
        private bool pEnablePasswordRetrieval;
        private bool pRequiresQuestionAndAnswer;
        private bool pRequiresUniqueEmail;
        private int pMaxInvalidPasswordAttempts;
        private int pPasswordAttemptWindow;
        private MembershipPasswordFormat pPasswordFormat;

        public override string ApplicationName
        {
            get { return pApplicationName; }
            set { pApplicationName = value; }
        }

        public override bool EnablePasswordReset
        {
            get { return pEnablePasswordReset; }
        }


        public override bool EnablePasswordRetrieval
        {
            get { return pEnablePasswordRetrieval; }
        }


        public override bool RequiresQuestionAndAnswer
        {
            get { return pRequiresQuestionAndAnswer; }
        }


        public override bool RequiresUniqueEmail
        {
            get { return pRequiresUniqueEmail; }
        }


        public override int MaxInvalidPasswordAttempts
        {
            get { return pMaxInvalidPasswordAttempts; }
        }


        public override int PasswordAttemptWindow
        {
            get { return pPasswordAttemptWindow; }
        }


        public override MembershipPasswordFormat PasswordFormat
        {
            get { return pPasswordFormat; }
        }

        private int pMinRequiredNonAlphanumericCharacters;

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return pMinRequiredNonAlphanumericCharacters; }
        }

        private int pMinRequiredPasswordLength;

        public override int MinRequiredPasswordLength
        {
            get { return pMinRequiredPasswordLength; }
        }

        private string pPasswordStrengthRegularExpression;

        public override string PasswordStrengthRegularExpression
        {
            get { return pPasswordStrengthRegularExpression; }
        }



        //
        // System.Web.Security.MembershipProvider methods.
        //

        //
        // MembershipProvider.ChangePassword
        //

        public override bool ChangePassword(string username, string oldPwd, string newPwd)
        {
            if (!ValidateUser(username, oldPwd))
                return false;


            ValidatePasswordEventArgs args =
              new ValidatePasswordEventArgs(username, newPwd, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");

            string newPasswordValue=EncodePassword(newPwd);
            return MembershipUserDAO.Instance.SetPassword(username, pApplicationName, newPasswordValue);
        }

        /// <summary>
        /// Меняем пароль из интерфейса админа, т.е. без запроса старого пароля
        /// </summary>
        /// <param name="pkid"></param>
        /// <param name="newPwd"></param>
        /// <returns></returns>
        public bool AdminChangePassword(string username, string newPwd)
        {
            ValidatePasswordEventArgs args =
              new ValidatePasswordEventArgs(username, newPwd, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Change password canceled due to new password validation failure.");

            string newPasswordValue = EncodePassword(newPwd);
            return MembershipUserDAO.Instance.SetPassword(username, pApplicationName, newPasswordValue);
        }


        //
        // MembershipProvider.ChangePasswordQuestionAndAnswer
        //

        public override bool ChangePasswordQuestionAndAnswer(string username,
                      string password,
                      string newPwdQuestion,
                      string newPwdAnswer)
        {
            if (!ValidateUser(username, password))
                return false;

            string newAnswer= EncodePassword(newPwdAnswer);
            return MembershipUserDAO.Instance.SetQuestionAndAnswer(username, pApplicationName, newPwdQuestion, newAnswer);
        }



        //
        // MembershipProvider.CreateUser
        //

        public override MembershipUser CreateUser(string username,
                 string password,
                 string email,
                 string passwordQuestion,
                 string passwordAnswer,
                 bool isApproved,
                 object providerUserKey,
                 out MembershipCreateStatus status)
        {
            ValidatePasswordEventArgs args =
              new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }



            if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            MembershipUser u = GetUser(username, false);

            if (u == null)
            {
                DateTime createDate = DateTime.Now;

                if (providerUserKey == null)
                {
                    providerUserKey = Guid.NewGuid();
                }
                else
                {
                    if (!(providerUserKey is Guid))
                    {
                        status = MembershipCreateStatus.InvalidProviderUserKey;
                        return null;
                    }
                }


                VlastelinMembershipUser newUser = new VlastelinMembershipUser();
                newUser.userId = (Guid)providerUserKey;
                newUser.Username = username;
                newUser.Password = EncodePassword(password);
                newUser.Email = email;
                newUser.PasswordQuestion = passwordQuestion != null ? passwordQuestion : "";
                newUser.PasswordAnswer = passwordAnswer == null ? "" : EncodePassword(passwordAnswer); ;
                newUser.isApproved = isApproved;
                newUser.Comment = "";
                newUser.CreationDate = createDate;
                newUser.LastPasswordChangedDate = createDate;
                newUser.ApplicationName = pApplicationName;
                newUser.isLocked = false;
                newUser.LastLockedOutDate = createDate;
                newUser.FailedPasswordAnswerAttemptCount = 0;
                newUser.FailedPasswordAnswerAttemptWindowStart = createDate;
                newUser.FailedPasswordAttemptCount = 0;
                newUser.FailedPasswordAttemptWindowStart = createDate;

                bool isSuccessed = MembershipUserDAO.Instance.CreateUser(newUser);
                status = isSuccessed ? status = MembershipCreateStatus.Success : MembershipCreateStatus.ProviderError;

                return GetUser(username, false);
            }
            else
            {
                status = MembershipCreateStatus.DuplicateUserName;
            }


            return null;
        }



        //
        // MembershipProvider.DeleteUser
        //

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            return MembershipUserDAO.Instance.DeleteUser(username, pApplicationName, deleteAllRelatedData);
        }



        //
        // MembershipProvider.GetAllUsers
        //

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();

            List<VlastelinMembershipUser> _users = MembershipUserDAO.Instance.GetUsers(pApplicationName, null, null, null);

            _users
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .Select(u => new MembershipUser(
                    this.Name,
                    u.Username,
                    u.userId,
                    u.Email,
                    u.PasswordQuestion,
                    u.Comment,
                    u.isApproved,
                    u.isLocked,
                    u.CreationDate,
                    u.LastLoginDate,
                    u.LastActivityDate,
                    u.LastPasswordChangedDate,
                    u.LastLockOutDate))
                .ToList()
                .ForEach(u => users.Add(u));

            totalRecords = _users.Count;
            
            return users;
        }


        //
        // MembershipProvider.GetNumberOfUsersOnline
        //

        public override int GetNumberOfUsersOnline()
        {
            TimeSpan onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);

            int numOnline = MembershipUserDAO.Instance.GetUsersOnline(pApplicationName, onlineSpan).Count;
            return numOnline;
        }



        //
        // MembershipProvider.GetPassword
        //

        public override string GetPassword(string username, string answer)
        {
            if (!EnablePasswordRetrieval)
                throw new ProviderException("Password Retrieval Not Enabled.");

            if (PasswordFormat == MembershipPasswordFormat.Hashed)
                throw new ProviderException("Cannot retrieve Hashed passwords.");

            List<VlastelinMembershipUser> users = MembershipUserDAO.Instance.GetUsers(pApplicationName, null, username, null);

            if (users.Count < 1)
                throw new MembershipPasswordException(string.Format("No users found for given username: {0}", username));
            if (users.Count > 1)
                throw new MembershipPasswordException(string.Format("More than one user found for given username: {0}", username));

            VlastelinMembershipUser user = users[0];

            if(user.isLocked)
                throw new MembershipPasswordException("The supplied user is locked out.");

            string password = user.Password;
            string passwordAnswer = user.PasswordAnswer;

            if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
            {
                UpdateFailureCount(username, "passwordAnswer");

                throw new MembershipPasswordException("Incorrect password answer.");
            }


            if (PasswordFormat == MembershipPasswordFormat.Encrypted)
            {
                password = UnEncodePassword(password);
            }

            return password;
        }



        //
        // MembershipProvider.GetUser(string, bool)
        //

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            List<VlastelinMembershipUser> users = MembershipUserDAO.Instance.GetUsers(pApplicationName, null, username, null);

            if (users.Count < 1)
                throw new ProviderException(string.Format("No users found for given username: {0}", username));
            if (users.Count > 1)
                throw new ProviderException(string.Format("More than one user found for given username: {0}", username));

            VlastelinMembershipUser user = users[0];

            if (userIsOnline)
            {
                user.LastActivityDate = DateTime.Now;
                MembershipUserDAO.Instance.EditUser(user);
            }

            MembershipUser mu = new MembershipUser(
                       this.Name,
                       user.Username,
                       user.userId,
                       user.Email,
                       user.PasswordQuestion,
                       user.Comment,
                       user.isApproved,
                       user.isLocked,
                       user.CreationDate,
                       user.LastLoginDate,
                       user.LastActivityDate,
                       user.LastPasswordChangedDate,
                       user.LastLockOutDate);

            return mu;
        }


        //
        // MembershipProvider.GetUser(object, bool)
        //

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (!(providerUserKey is Guid))
                throw new ProviderException("providerUserKey is not Guid");

            List<VlastelinMembershipUser> users = MembershipUserDAO.Instance.GetUsers(pApplicationName, (Guid)providerUserKey, null, null);

            if (users.Count < 1)
                throw new ProviderException(string.Format("No users found for given ID: {0}", providerUserKey.ToString()));
            if (users.Count > 1)
                throw new ProviderException(string.Format("More than one user found for given ID: {0}", providerUserKey.ToString()));

            VlastelinMembershipUser user = users[0];

            if (userIsOnline)
            {
                user.LastActivityDate = DateTime.Now;
                MembershipUserDAO.Instance.EditUser(user);
            }

            MembershipUser mu = new MembershipUser(
                       this.Name,
                       user.Username,
                       user.userId,
                       user.Email,
                       user.PasswordQuestion,
                       user.Comment,
                       user.isApproved,
                       user.isLocked,
                       user.CreationDate,
                       user.LastLoginDate,
                       user.LastActivityDate,
                       user.LastPasswordChangedDate,
                       user.LastLockOutDate);


            return mu;
        }

        
        // функция будет блокировать пользователя по запросу.

        public bool LockUser(string username)
        {
            List<VlastelinMembershipUser> users = MembershipUserDAO.Instance.GetUsers(pApplicationName, null, username, null);

            if (users.Count < 1)
                throw new ProviderException(string.Format("No users found for given username: {0}", username));
            if (users.Count > 1)
                throw new ProviderException(string.Format("More than one user found for given username: {0}", username));

            VlastelinMembershipUser user = users[0];

            user.isLocked = true;
            user.LastLockedOutDate = DateTime.Now;

            bool isSuccess = MembershipUserDAO.Instance.EditUser(user);

            return isSuccess;
        }

        //
        // MembershipProvider.UnlockUser
        //

        public override bool UnlockUser(string username)
        {
            List<VlastelinMembershipUser> users = MembershipUserDAO.Instance.GetUsers(pApplicationName, null, username, null);

            if (users.Count < 1)
                throw new ProviderException(string.Format("No users found for given username: {0}", username));
            if (users.Count > 1)
                throw new ProviderException(string.Format("More than one user found for given username: {0}", username));

            VlastelinMembershipUser user = users[0];

            user.isLocked = true;
            user.LastLockOutDate = DateTime.Now;

            bool isSuccess = MembershipUserDAO.Instance.EditUser(user);

            return isSuccess;
        }


        //
        // MembershipProvider.GetUserNameByEmail
        //

        public override string GetUserNameByEmail(string email)
        {
            List<VlastelinMembershipUser> users = MembershipUserDAO.Instance.GetUsers(pApplicationName, null, null, email);

            if (users.Count < 1)
                throw new ProviderException(string.Format("No users found for given email: {0}", email));
            if (users.Count > 1)
                throw new ProviderException(string.Format("More than one user found for email email: {0}", email));

            VlastelinMembershipUser user = users[0];

            return user.Username;
        }




        //
        // MembershipProvider.ResetPassword
        //

        public override string ResetPassword(string username, string answer)
        {
            if (!EnablePasswordReset)
            {
                throw new NotSupportedException("Password reset is not enabled.");
            }

            if (answer == null && RequiresQuestionAndAnswer)
            {
                UpdateFailureCount(username, "passwordAnswer");

                throw new ProviderException("Password answer required for password reset.");
            }
            
            List<VlastelinMembershipUser> users = MembershipUserDAO.Instance.GetUsers(pApplicationName, null, username, null);

            if (users.Count < 1)
                throw new ProviderException(string.Format("No users found for given username: {0}", username));
            if (users.Count > 1)
                throw new ProviderException(string.Format("More than one user found for given username: {0}", username));

            VlastelinMembershipUser user = users[0];

            if(user.isLocked)
                throw new MembershipPasswordException("The supplied user is locked out.");

            string newPassword =
              System.Web.Security.Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);

            ValidatePasswordEventArgs args =
              new ValidatePasswordEventArgs(username, newPassword, true);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Reset password canceled due to password validation failure.");

            string passwordAnswer = user.PasswordAnswer;
            

            if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
            {
                UpdateFailureCount(username, "passwordAnswer");

                throw new MembershipPasswordException("Incorrect password answer.");
            }

            user.Password=EncodePassword(newPassword);
            user.LastPasswordChangedDate=DateTime.Now;

            MembershipUserDAO.Instance.EditUser(user);

            return newPassword;
        }


        //
        // MembershipProvider.UpdateUser
        //

        public override void UpdateUser(MembershipUser user)
        {
            List<VlastelinMembershipUser> users = MembershipUserDAO.Instance.GetUsers(pApplicationName, null, user.UserName, null);

            if (users.Count < 1)
                throw new ProviderException(string.Format("No users found for given username: {0}", user.UserName));
            if (users.Count > 1)
                throw new ProviderException(string.Format("More than one user found for given username: {0}", user.UserName));

            VlastelinMembershipUser _user = users[0];

            _user.Email = user.Email;
            _user.Comment = user.Comment;
            _user.isApproved = user.IsApproved;

            MembershipUserDAO.Instance.EditUser(_user);
        }


        //
        // MembershipProvider.ValidateUser
        //

        public override bool ValidateUser(string username, string password)
        {
            bool isValid = false;

            List<VlastelinMembershipUser> users = MembershipUserDAO.Instance.GetUsers(pApplicationName, null, username, null);

            if (users.Count < 1)
                throw new ProviderException(string.Format("No users found for given username: {0}", username));
            if (users.Count > 1)
                throw new ProviderException(string.Format("More than one user found for given username: {0}", username));

            VlastelinMembershipUser user = users[0];

            bool isApproved = user.isApproved;
            string pwd =user.Password;


            if (CheckPassword(password, pwd))
            {
                if (isApproved)
                {
                    isValid = true;
                    user.LastLoginDate=DateTime.Now;
                    MembershipUserDAO.Instance.EditUser(user);
                }
            }
            else
            {
                UpdateFailureCount(username, "password");
            }
            
            return isValid;
        }


        //
        // UpdateFailureCount
        //   A helper method that performs the checks and updates associated with
        // password failure tracking.
        //

        public void UpdateFailureCount(string username, string failureType)
        {

            List<VlastelinMembershipUser> users = MembershipUserDAO.Instance.GetUsers(pApplicationName, null, username, null);

            if (users.Count < 1)
                throw new ProviderException(string.Format("No users found for given username: {0}", username));
            if (users.Count > 1)
                throw new ProviderException(string.Format("More than one user found for given username: {0}", username));

            VlastelinMembershipUser user = users[0];

            int failureCount=0;
            DateTime windowStart=new DateTime();

            if (failureType == "password")
            {
                failureCount = user.FailedPasswordAttemptCount;
                windowStart = user.FailedPasswordAttemptWindowStart ;
            }

            if (failureType == "passwordAnswer")
            {
                failureCount = user.FailedPasswordAnswerAttemptCount;
                windowStart = user.FailedPasswordAnswerAttemptWindowStart;
            }
            

            DateTime windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);

            if (failureCount == 0 || DateTime.Now > windowEnd)
            {
                // First password failure or outside of PasswordAttemptWindow. 
                // Start a new password failure count from 1 and a new window starting now.

                if (failureType == "password")
                {
                    user.FailedPasswordAttemptCount=1;
                    user.FailedPasswordAttemptWindowStart=DateTime.Now;
                }
                if (failureType == "passwordAnswer")
                {
                    user.FailedPasswordAnswerAttemptCount=1;
                    user.FailedPasswordAnswerAttemptWindowStart=DateTime.Now;
                }
                bool isSuccess=MembershipUserDAO.Instance.EditUser(user);
                if(!isSuccess)
                    throw new ProviderException("Unable to update failure count and window start.");
            }
            else
            {
                if (failureCount++ >= MaxInvalidPasswordAttempts)
                {
                    // Password attempts have exceeded the failure threshold. Lock out
                    // the user.

                    bool isSuccess=LockUser(user.Username);
                    if (!isSuccess)
                        throw new ProviderException("Unable to lock out user.");
                }
                else
                {
                    // Password attempts have not exceeded the failure threshold. Update
                    // the failure counts. Leave the window the same.

                    if (failureType == "password")
                        user.FailedPasswordAttemptCount=failureCount;

                    if (failureType == "passwordAnswer")
                        user.FailedPasswordAnswerAttemptCount=failureCount;

                    bool isSuccess=MembershipUserDAO.Instance.EditUser(user);
                    if (!isSuccess)
                        throw new ProviderException("Unable to update failure count.");
                }
            }
        }


        //
        // CheckPassword
        //   Compares password values based on the MembershipPasswordFormat.
        //

        private bool CheckPassword(string password, string dbpassword)
        {
            string pass1 = password;
            string pass2 = dbpassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = EncodePassword(password);
                    break;
                default:
                    break;
            }

            if (pass1 == pass2)
            {
                return true;
            }

            return false;
        }


        //
        // EncodePassword
        //   Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
        //

        private string EncodePassword(string password)
        {
            string encodedPassword = password;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword =
                      Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    HMACSHA1 hash = new HMACSHA1();
                    hash.Key = HexToByte(machineKey.ValidationKey);
                    encodedPassword =
                      Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return encodedPassword;
        }


        //
        // UnEncodePassword
        //   Decrypts or leaves the password clear based on the PasswordFormat.
        //

        private string UnEncodePassword(string encodedPassword)
        {
            string password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password =
                      Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot unencode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return password;
        }

        //
        // HexToByte
        //   Converts a hexadecimal string to a byte array. Used to convert encryption
        // key values from the configuration.
        //

        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }


        //
        // MembershipProvider.FindUsersByName
        //

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();

            List<VlastelinMembershipUser> _users = MembershipUserDAO.Instance.GetUsers(pApplicationName, null, usernameToMatch, null);

            _users
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .Select(u => new MembershipUser(
                    this.Name,
                    u.Username,
                    u.userId,
                    u.Email,
                    u.PasswordQuestion,
                    u.Comment,
                    u.isApproved,
                    u.isLocked,
                    u.CreationDate,
                    u.LastLoginDate,
                    u.LastActivityDate,
                    u.LastPasswordChangedDate,
                    u.LastLockOutDate))
                .ToList()
                .ForEach(u => users.Add(u));

            totalRecords = _users.Count;

            return users;
        }

        //
        // MembershipProvider.FindUsersByEmail
        //

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection users = new MembershipUserCollection();

            List<VlastelinMembershipUser> _users = MembershipUserDAO.Instance.GetUsers(pApplicationName, null, null, emailToMatch);

            _users
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .Select(u => new MembershipUser(
                    this.Name,
                    u.Username,
                    u.userId,
                    u.Email,
                    u.PasswordQuestion,
                    u.Comment,
                    u.isApproved,
                    u.isLocked,
                    u.CreationDate,
                    u.LastLoginDate,
                    u.LastActivityDate,
                    u.LastPasswordChangedDate,
                    u.LastLockOutDate))
                .ToList()
                .ForEach(u => users.Add(u));

            totalRecords = _users.Count;

            return users;
        }


        //
        // WriteToEventLog
        //   A helper function that writes exception detail to the event log. Exceptions
        // are written to the event log as a security measure to avoid private database
        // details from being returned to the browser. If a method does not return a status
        // or boolean indicating the action succeeded or failed, a generic exception is also 
        // thrown by the caller.
        //

        private void WriteToEventLog(Exception e, string action)
        {
            /*EventLog log = new EventLog();
            log.Source = eventSource;
            log.Log = eventLog;*/

            string message = "An exception occurred communicating with the data source.\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + e.ToString();

            //log.WriteEntry(message);
            logger.LogMessage(LogEventType.Error, message);
        }

    }
}
