using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace VlastelinServer.DAO.DBAccess
{
    /// <summary>
    /// Extensions class
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Add new SQL parameter to list
        /// </summary>
        /// <param name="parameters">list to add to</param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterType">parameter type</param>
        /// <param name="parameterValue">parameter value</param>
        /// <param name="parameterDirection">parameter direction</param>
        public static void AddParameter(this List<MySqlParameter> parameters, string parameterName, DbType parameterType, object parameterValue, ParameterDirection parameterDirection)
        {
            parameters.Add(DBHelper.CreateParameter(parameterName, parameterType, parameterValue, parameterDirection));
        }

        /// <summary>
        /// Add new input SQL parameter to list
        /// </summary>
        /// <param name="parameters">list to add to</param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterType">parameter type</param>
        /// <param name="parameterValue">parameter value</param>
        public static void AddInputParameter(this List<MySqlParameter> parameters, string parameterName, DbType parameterType, object parameterValue)
        {
            parameters.Add(DBHelper.CreateInputParameter(parameterName, parameterType, parameterValue));
        }

        /// <summary>
        /// Add new output SQL parameter to list
        /// </summary>
        /// <param name="parameters">list to add to</param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterType">parameter type</param>
        /// <param name="parameterValue">parameter value</param>
        public static void AddOutputParameter(this List<MySqlParameter> parameters, string parameterName, DbType parameterType, object parameterValue)
        {
            parameters.Add(DBHelper.CreateOutputParameter(parameterName, parameterType, parameterValue));
        }
        /// <summary>
        /// Add new output SQL parameter to list
        /// </summary>
        /// <param name="parameters">list to add to</param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterType">parameter type</param>
        /// <param name="parameterValue">parameter value</param>
        /// <param name="size">parameter size</param>
        public static void AddOutputParameter(this List<MySqlParameter> parameters, string parameterName, DbType parameterType, object parameterValue, int size)
        {
            parameters.Add(DBHelper.CreateOutputParameter(parameterName, parameterType, parameterValue, size));
        }

        /// <summary>
        /// Add new input-output SQL parameter to list
        /// </summary>
        /// <param name="parameters">list to add to</param>
        /// <param name="parameterName">parameter name</param>
        /// <param name="parameterType">parameter type</param>
        /// <param name="parameterValue">parameter value</param>
        public static void AddInputOutputParameter(this List<MySqlParameter> parameters, string parameterName, DbType parameterType, object parameterValue)
        {
            parameters.Add(DBHelper.CreateInputOutputParameter(parameterName, parameterType, parameterValue));
        }
        public static void AddErrorOutputParameter(this List<MySqlParameter> parameters)
        {
            parameters.Add(DBHelper.CreateErrorParameter());
        }
    }

}
