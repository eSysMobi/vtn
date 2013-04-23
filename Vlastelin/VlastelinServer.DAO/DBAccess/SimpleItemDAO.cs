using System;
using System.Collections.Generic;
using MySql.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using Vlastelin.Common;

namespace VlastelinServer.DAO.DBAccess
{
    /// <summary>
    /// SimpleItemDAO class
    /// </summary>
    public class SimpleItemDAO
    {
        /// <summary>
        /// Database alias field
        /// </summary>
        protected string _databaseAlias;
        /// <summary>
        /// Error handler field
        /// </summary>
        protected IDatabaseErrorHandler _errorHandler;

        /// <summary>
        /// Database alias
        /// </summary>
        public string DatabaseAlias
        {
            get
            {
                return _databaseAlias;
            }
        }

        /// <summary>
        /// SQL connection
        /// </summary>
        public MySqlConnection Connection
        {
            get
            {
                return DBHelper.GetConnectionByName(_databaseAlias);
            }
        }

        #region .ctor
        /// <summary>
        /// Create new class instance
        /// </summary>
        /// <param name="databaseAlias">database alias</param>
        public SimpleItemDAO(string databaseAlias)
        {
            _databaseAlias = databaseAlias;
        }
        /// <summary>
        /// Create new class instance
        /// </summary>
        /// <param name="databaseAlias">database alias</param>
        /// <param name="errorHandler">error handler</param>
        public SimpleItemDAO(string databaseAlias, IDatabaseErrorHandler errorHandler)
        {
            _databaseAlias = databaseAlias;
            _errorHandler = errorHandler;
        }
        #endregion

        /// <summary>
        /// Execute stored procedure
        /// </summary>
        /// <param name="cmdType">command type</param>
        /// <param name="cmdText">command</param>
        /// <param name="commandParameters">parameters</param>
        /// <returns>rows affected</returns>
        protected int Execute_StoredProcedure(CommandType cmdType, string cmdText, List<MySqlParameter> commandParameters)
        {
            int result = 0;

            try
            {
                result = DBHelper.ExecuteProcedure(Connection, cmdType, cmdText, commandParameters);
            }
            catch (MySqlException exc)
            {
                HandleSqlException(exc, cmdType, cmdText, commandParameters);
            }
            if (_errorHandler != null)
            {
                _errorHandler.RaiseException(DBHelper.GetErrorCode(commandParameters), string.Empty);
            }

            return result;
        }

        /// <summary>
        /// Execute stored procedure with DataTable result
        /// </summary>
        /// <param name="cmdType">command type</param>
        /// <param name="cmdText">command</param>
        /// <param name="commandParameters">parameters</param>
        /// <returns>rows affected</returns>
        protected DataTable Execute_GetDataTable(CommandType cmdType, string cmdText, List<MySqlParameter> commandParameters)
        {
            DataTable result = null;

            try
            {
                result = DBHelper.ExecuteTable(Connection, cmdType, cmdText, commandParameters);
            }
            catch (MySqlException exc)
            {
                HandleSqlException(exc, cmdType, cmdText, commandParameters);
            }
            if (_errorHandler != null)
            {
                _errorHandler.RaiseException(DBHelper.GetErrorCode(commandParameters), string.Empty);
            }

            return result;
        }

        /// <summary>
        /// Execute scalar stored procedure
        /// </summary>
        /// <typeparam name="V">scalar type</typeparam>
        /// <param name="cmdType">command type</param>
        /// <param name="cmdText">command</param>
        /// <param name="commandParameters">parameters</param>
        /// <returns>result value</returns>
        protected V Execute_ScalarStoredProcedure<V>(CommandType cmdType, string cmdText, List<MySqlParameter> commandParameters)
        {
            V result = default(V);

            try
            {
                result = DBHelper.ExecuteScalarProcedure<V>(Connection, cmdType, cmdText, commandParameters);
            }
            catch (MySqlException exc)
            {
                HandleSqlException(exc, cmdType, cmdText, commandParameters);
            }
            if (_errorHandler != null)
            {
                _errorHandler.RaiseException(DBHelper.GetErrorCode(commandParameters), string.Empty);
            }

            return result;
        }

        /// <summary>
        /// Convert SQL exception to custom exception
        /// </summary>
        /// <param name="exc">SQL exception</param>
        /// <param name="cmdType">command type</param>
        /// <param name="cmdText">command</param>
        /// <param name="commandParameters">parameters</param>
        protected virtual void HandleSqlException(MySqlException exc, CommandType cmdType, string cmdText, List<MySqlParameter> commandParameters)
        {
            throw new Exception(GetErrorMessage(exc, cmdType, cmdText, commandParameters), exc);
        }

        /// <summary>
        /// Create error message
        /// </summary>
        /// <param name="exc">SQL exception</param>
        /// <param name="cmdType">command type</param>
        /// <param name="cmdText">command</param>
        /// <param name="commandParameters">parameters</param>
        /// <returns>error message</returns>
        protected string GetErrorMessage(MySqlException exc, CommandType cmdType, string cmdText, List<MySqlParameter> commandParameters)
        {
            StringBuilder extendedMessage = new StringBuilder(exc.Message);
            extendedMessage.AppendFormat("Ошибка при выполнении sql-команды.{0}Тип: {1}{0}Команда: {2}{0}", Environment.NewLine, cmdType.ToString(), cmdText);
            if (commandParameters == null)
            {
                extendedMessage.Append(" Параметры: ");
                foreach (MySqlParameter param in commandParameters)
                {
                    extendedMessage.AppendFormat(" {0}={1}", param.ParameterName, param.Value);
                }
            }
            return extendedMessage.ToString();
        }

    }

}
