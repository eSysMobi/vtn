using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Data;
using Vlastelin.Common;

namespace VlastelinServer.DAO.DBAccess
{
    /// <summary>
    /// DBHelper class
    /// </summary>
    public static class DBHelper
    {
        private static Dictionary<string,MySqlConnection> _connectionPool;
        private static Logger logger;
        static DBHelper()
        {
            logger = new Logger();
            _connectionPool = new Dictionary<string, MySqlConnection>();
        }

        #region Public constants

        private const string ERROR_ID = "ERROR_ID";

        #endregion

        #region Connection management

        /// <summary>
        /// Получение строки подключения по имени подключения из конфиг.файла
        /// </summary>
        /// <param name="connectionName">connection name</param>
        /// <returns>connection string</returns>
        public static string GetConnectionStringByName(string connectionName)
        {
            ConnectionStringSettings css = ConfigurationManager.ConnectionStrings[connectionName];
            if (css == null)
            {
                throw new Exception("Строка подключения к базе данных отсутствует");
            }
            return css.ConnectionString;
        }

        /// <summary>
        /// Get SQL connection  by connection name
        /// </summary>
        /// <param name="connectionName">connection name</param>
        /// <returns>SQL connection</returns>
        public static MySqlConnection GetConnectionByName(string connectionName)
        {
            return GetConnection(GetConnectionStringByName(connectionName));            
        }

        /// <summary>
        /// Get SQL connection  by connection string
        /// </summary>
        /// <param name="connectionString">connection string</param>
        /// <returns>SQL connection</returns>
        public static MySqlConnection GetConnection(string connectionString)
        {
            /* пока что не работает из-за открытых ридеров
            if (!_connectionPool.ContainsKey(connectionString))
            {
                lock (_connectionPool)
                {
                    logger.LogMessage(LogEventType.Info, "Adding connection to connection pool.");
                    _connectionPool.Add(connectionString, new MySqlConnection(connectionString));
                }
            }

            logger.LogMessage(LogEventType.Info, "Retrieving connection from comnnection pool.");
            return _connectionPool[connectionString];
             */
            return new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Open the connection
        /// </summary>
        /// <param name="connection">Connection</param>
        public static void OpenConnection(MySqlConnection connection)
        {
            if (connection.State != ConnectionState.Open)
            {
                logger.LogMessage(LogEventType.Info, "Opening database connection...");
                connection.Open();
                logger.LogMessage(LogEventType.Info, "Database connection opened.");
            }
        }

        /// <summary>
        /// Close connection
        /// </summary>
        /// <param name="connection">Connection</param>
        public static void CloseConnection(MySqlConnection connection)
        {
            if (connection != null && connection.State != ConnectionState.Closed)
            {
                logger.LogMessage(LogEventType.Info, "Closing database connection...");
                connection.Close();
                logger.LogMessage(LogEventType.Info, "Database connection closed.");
            }
        }

        #endregion //Connection management

        #region Вспомогательные методы для командных объекты
        /// <summary>
        /// Create sql command
        /// </summary>
        /// <param name="connection">SQL connection</param>
        /// <param name="cmdtype">Type of </param>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">List of parameters</param>
        /// <returns>SQL command</returns>
        public static MySqlCommand CreateSqlCommand(MySqlConnection connection, CommandType cmdtype, string commandText, List<MySqlParameter> parameters)
        {
            MySqlCommand cmd = new MySqlCommand()
            {
                CommandText = commandText,
                Connection = connection,
                CommandType = cmdtype,
                CommandTimeout = connection.ConnectionTimeout
            };

            ApplyParameters(cmd, parameters);

            return cmd;
        }

        /// <summary>
        /// Execute SQL command
        /// </summary>
        /// <param name="connection">Sql Connection</param>
        /// <param name="cmdtype">Command Type</param>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">Sql Parameters</param>
        /// <returns>Rows affected</returns>
        public static int ExecuteProcedure(MySqlConnection connection, CommandType cmdtype, string commandText, List<MySqlParameter> parameters)
        {
            int returnValue = 0;
            try
            {
                MySqlCommand dbCommand = CreateSqlCommand(connection, cmdtype, commandText, parameters);
                OpenConnection(connection);
                returnValue = dbCommand.ExecuteNonQuery();
            }
            finally
            {
                CloseConnection(connection);
            }
            return returnValue;
        }

        /// <summary>
        /// Execute SQL command and return scalar result
        /// </summary>
        /// <typeparam name="T">scalar type</typeparam>
        /// <param name="connection">Sql Connection</param>
        /// <param name="cmdType">Command Type</param>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">Sql Parameters</param>
        /// <returns>scalar result of type T</returns>
        public static T ExecuteScalarProcedure<T>(MySqlConnection connection, CommandType cmdType, string commandText, List<MySqlParameter> parameters)
        {
            T result = default(T);
            try
            {
                MySqlCommand dbCommand = CreateSqlCommand(connection, cmdType, commandText, parameters);
                OpenConnection(connection);
                object o = dbCommand.ExecuteScalar();
                result = (T) o;
            }
            finally
            {
                CloseConnection(connection);
            }

            return result;
        }

        /// <summary>
        /// Execute SQL command and return SqlDataReader
        /// </summary>
        /// <param name="connection">Sql Connection</param>
        /// <param name="cmdType">Command Type</param>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">Sql Parameters</param>
        /// <returns>SqlDataReader</returns>
        public static MySqlDataReader ExecuteReader(MySqlConnection connection, CommandType cmdType, string commandText, List<MySqlParameter> parameters)
        {
            try
            {
                MySqlCommand dbCommand = CreateSqlCommand(connection, cmdType, commandText, parameters);
                OpenConnection(connection);
                return dbCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception)
            {
                CloseConnection(connection);
                throw;
            }
        }

        /// <summary>
        /// Execute SQL command and return SqlDataReader
        /// </summary>
        /// <param name="connection">Sql Connection</param>
        /// <param name="cmdType">Command Type</param>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">Sql Parameters</param>
        /// <returns>SqlDataReader</returns>
        public static DataTable ExecuteTable(MySqlConnection connection, CommandType cmdType, string commandText, List<MySqlParameter> parameters)
        {
            DataTable result = null;
            try
            {
                result = new DataTable();
                result.TableName = "Table";
                MySqlCommand dbCommand = CreateSqlCommand(connection, cmdType, commandText, parameters);
                OpenConnection(connection);
                result.Load(dbCommand.ExecuteReader(CommandBehavior.CloseConnection));
            }
            catch (Exception)
            {
                CloseConnection(connection);
                throw;
            }
            return result;
        }

        /// <summary>
        /// Execute SQL command and return DataReaderAdapter
        /// </summary>
        /// <param name="connection">Sql Connection</param>
        /// <param name="cmdType">Command Type</param>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">Sql Parameters</param>
        /// <returns>DataReaderAdapter</returns>
        public static DataReaderAdapter ExecuteReaderEx(MySqlConnection connection, CommandType cmdType, string commandText, List<MySqlParameter> parameters)
        {
            return new DataReaderAdapter(ExecuteReader(connection, cmdType, commandText, parameters));
        }
        #endregion

        #region Helper methods for sql parameters creation

        /// <summary>
        /// Create input parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterType">Parameter type</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public static MySqlParameter CreateInputParameter(string parameterName, DbType parameterType, object parameterValue)
        {
            return CreateParameter(parameterName, parameterType, parameterValue, ParameterDirection.Input);
        }

        /// <summary>
        /// Create output parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterType">Parameter type</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter</returns>
        public static MySqlParameter CreateOutputParameter(string parameterName, DbType parameterType, object parameterValue)
        {
            return CreateParameter(parameterName, parameterType, parameterValue, ParameterDirection.Output);
        }
        /// <summary>
        /// Create output parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterType">Parameter type</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <param name="size">Parameter size</param>
        /// <returns>Parameter</returns>
        public static MySqlParameter CreateOutputParameter(string parameterName, DbType parameterType, object parameterValue, int size)
        {
            return CreateParameter(parameterName, parameterType, parameterValue, ParameterDirection.Output, size);
        }

        /// <summary>
        /// Create input/output parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterType">Parameter type</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <returns>Parameter </returns>
        public static MySqlParameter CreateInputOutputParameter(string parameterName, DbType parameterType, object parameterValue)
        {
            return CreateParameter(parameterName, parameterType, parameterValue, ParameterDirection.InputOutput);
        }

        /// <summary>
        /// Create parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterType">Parameter type</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <param name="parameterDirection">Parameter direction</param>
        /// <returns>Parameter</returns>
        public static MySqlParameter CreateParameter(string parameterName, DbType parameterType, object parameterValue, ParameterDirection parameterDirection)
        {
            return new MySqlParameter()
            {
                ParameterName = parameterName,
                DbType = parameterType,
                Direction = parameterDirection,
                Value = parameterValue
            };
        }


        /// <summary>
        /// Create parameter
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="parameterType">Parameter type</param>
        /// <param name="parameterValue">Parameter value</param>
        /// <param name="parameterDirection">Parameter direction</param>
        /// <param name="size">Parameter size</param>
        /// <returns>Parameter</returns>
        public static MySqlParameter CreateParameter(string parameterName, DbType parameterType, object parameterValue, ParameterDirection parameterDirection, int size)
        {
            return new MySqlParameter()
            {
                ParameterName = parameterName,
                DbType = parameterType,
                Direction = parameterDirection,
                Value = parameterValue,
                Size = size
            };
        }

        private static void ApplyParameters(MySqlCommand dbCommand, List<MySqlParameter> parameters)
        {
            if (parameters != null)
            {
                foreach (MySqlParameter param in parameters)
                {
                    if (param.Direction != ParameterDirection.Output && param.Value == null)
                    {
                        param.Value = DBNull.Value;
                    }

                    if (param.Direction != ParameterDirection.Output && param.Value.Equals(string.Empty) &&
                        (param.DbType == DbType.AnsiStringFixedLength || param.DbType == DbType.StringFixedLength || param.DbType == DbType.String || param.DbType == DbType.AnsiString))
                    {
                        param.Value = DBNull.Value;
                    }

                    dbCommand.Parameters.Add(param);
                }
            }
        }
        #endregion //Helper methods for sql parameters creation

        #region Helper methods for sql parameters reading

        /// <summary>
        /// Get error code from output parameters
        /// </summary>
        /// <param name="dbCommand">SQL Command</param>
        /// <returns>Error code</returns>
        public static int GetErrorCode(MySqlCommand dbCommand)
        {
            return GetParameterValue<int>(dbCommand, ERROR_ID);
        }

        /// <summary>
        /// Get error code from output parameters
        /// </summary>
        /// <param name="parameters">SQL parameters collection</param>
        /// <returns>Error code</returns>
        public static int GetErrorCode(List<MySqlParameter> parameters)
        {
            return GetParameterValue<int>(parameters, ERROR_ID);
        }

        /// <summary>
        /// Extract value from parameters collection
        /// </summary>
        /// <typeparam name="T">Parameter type</typeparam>
        /// <param name="parameters">Parameters collection</param>
        /// <param name="parameterName">Parameter Name</param>
        /// <returns>Value</returns>
        public static T GetParameterValue<T>(List<MySqlParameter> parameters, string parameterName)
        {
            MySqlParameter param = parameters.SingleOrDefault(a => a.ParameterName.Equals(parameterName));
            return GetParameterValue<T>(param);
        }

        /// <summary>
        /// Extract value from command
        /// </summary>
        /// <typeparam name="T">Parameter type</typeparam>
        /// <param name="cmd">SQL Command</param>
        /// <param name="parameterName">Parameter Name</param>
        /// <returns>Value</returns>
        public static T GetParameterValue<T>(MySqlCommand cmd, string parameterName)
        {
            MySqlParameter param = cmd.Parameters[parameterName];
            return GetParameterValue<T>(param);
        }

        private static T GetParameterValue<T>(MySqlParameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }
            if (parameter.Value == DBNull.Value)
            {
                return default(T);
            }

            return (T)parameter.Value;
        }

        #endregion //Helper methods for sql parameters reading

        /// <summary>
        /// Create output parameter 'ERROR_ID'
        /// </summary>
        /// <returns>Output parameter ERROR_ID</returns>
        public static MySqlParameter CreateErrorParameter()
        {
            return CreateOutputParameter(ERROR_ID, DbType.Int32, 0);
        }
    }

}
