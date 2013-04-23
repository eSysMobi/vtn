using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using Vlastelin.Common;

namespace VlastelinServer.DAO.DBAccess
{
    /// <summary>
    /// ItemDAO class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="M"></typeparam>
    public class ItemDAO<T, M> : SimpleItemDAO
        where T : class
        where M : class, IMaterializator<T>, new()
    {
        #region Properties and variables

        /// <summary>
        /// materializer field
        /// </summary>
        protected M _materializer;

        /// <summary>
        /// Class instance field
        /// </summary>
        protected static new ItemDAO<T, M> _instance = null;

        /// <summary>
        /// Materializer property
        /// </summary>
        public M Materializer
        {
            get
            {
                return _materializer;
            }
        }

        #endregion //Properties and variables

        Logger logger;

        /// <summary>
        /// Create new class instance
        /// </summary>
        /// <param name="databaseAlias">database alias</param>
        public ItemDAO(string databaseAlias)
            : base(databaseAlias)
        {
            _materializer = new M();
            _errorHandler = null;
            logger = new Logger();
        }

        /// <summary>
        /// Create new class instance
        /// </summary>
        /// <param name="databaseAlias">database alias</param>
        /// <param name="errorHandler">error handler</param>
        public ItemDAO(string databaseAlias, IDatabaseErrorHandler errorHandler)
            : base(databaseAlias, errorHandler)
        {
            _materializer = new M();
            logger = new Logger();
        }

        /// <summary>
        /// Initialize singletone instance of specific ItemDAO class
        /// </summary>
        /// <param name="databaseAlias">database alias (configuration name)</param>
        /// <returns>Instance of ItemDAO class</returns>
        public static new ItemDAO<T, M> Instance(string databaseAlias)
        {
            if (_instance == null)
            {
                _instance = new ItemDAO<T, M>(databaseAlias);
            }

            return _instance;
        }

        /// <summary>
        /// Initialize singletone instance of specific ItemDAO class
        /// </summary>
        /// <param name="databaseAlias">database alias (configuration name)</param>
        /// <param name="errorHandler">specific database's error handler</param>
        /// <returns>Instance of ItemDAO class</returns>
        public static new ItemDAO<T, M> Instance(string databaseAlias, IDatabaseErrorHandler errorHandler)
        {
            if (_instance == null)
            {
                _instance = new ItemDAO<T, M>(databaseAlias, errorHandler);
            }

            return _instance;
        }

        /// <summary>
        /// Create and Read single object from database
        /// </summary>
        /// <param name="cmdtype">SQL command type</param>
        /// <param name="commandText">SQL command text</param>
        /// <param name="commandParameters">Sql parameters</param>
        /// <returns>Instance of object T</returns>
        protected T Execute_Get(CommandType cmdtype, string commandText, List<MySqlParameter> commandParameters)
        {
            if (commandText == null)
            {
                throw new ArgumentNullException("commandText");
            }

            T result = null;
            try
            {
                logger.LogMessage(LogEventType.Debug,string.Format("ExecuteGet({0}, {1})",
                    cmdtype.ToString(),
                    commandText));
                using (DataReaderAdapter reader = DBHelper.ExecuteReaderEx(Connection, cmdtype, commandText, commandParameters))
                {
                    result = Materializer.Materialize(reader);
                    logger.LogMessage(LogEventType.Debug, string.Format("ExecuteGet returned {0}",
                    typeof(T).ToString()));
                }
            }
            catch (MySqlException exc)
            {
                HandleSqlException(exc, cmdtype, commandText, commandParameters);
            }

            if (_errorHandler != null)
            {
                _errorHandler.RaiseException(DBHelper.GetErrorCode(commandParameters), string.Empty);
            }

            return result;
        }

        /// <summary>
        /// Get list of objects from database
        /// </summary>
        /// <param name="cmdtype">SQL command type</param>
        /// <param name="commandText">SQL command text</param>
        /// <param name="commandParameters">Sql parameters</param>
        /// <returns>List of objects read</returns>
        protected List<T> Execute_GetList(CommandType cmdtype, string commandText, List<MySqlParameter> commandParameters)
        {
            if (commandText == null)
            {
                throw new ArgumentNullException("commandText");
            }

            List<T> result = new List<T>();
            try
            {
                logger.LogMessage(LogEventType.Info, string.Format("Execute_GetList({0}, {1})",
                    cmdtype.ToString(),
                    commandText));
                using (DataReaderAdapter reader = DBHelper.ExecuteReaderEx(Connection, cmdtype, commandText, commandParameters))
                {
                    result = Materializer.Materialize_List(reader);
                    logger.LogMessage(LogEventType.Debug, string.Format("Execute_GetList returned {1}[{0}]",
                    result.Count, typeof(T).ToString()));
                }
            }
            catch (MySqlException exc)
            {
                HandleSqlException(exc, cmdtype, commandText, commandParameters);
            }

            if (_errorHandler != null)
            {
                _errorHandler.RaiseException(DBHelper.GetErrorCode(commandParameters), null);
            }

            return result;
        }

        /// <summary>
        /// Handle and re-raise sql exceptions
        /// </summary>
        /// <param name="exc">SQL exception</param>
        /// <param name="cmdType">SQL command type</param>
        /// <param name="cmdText">SQL command text</param>
        /// <param name="commandParameters">Sql parameters</param>
        protected override void HandleSqlException(MySqlException exc, CommandType cmdType, string cmdText, List<MySqlParameter> commandParameters)
        {
            string msg = base.GetErrorMessage(exc, cmdType, cmdText, commandParameters);
            logger.LogMessage(LogEventType.Error, msg);
            if (_errorHandler != null)
            {
                _errorHandler.RaiseException(exc.Number, msg);
            }
            else
            {
                throw new Exception(msg, exc);
            }
        }
    }

}
