using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace VlastelinServer.DAO.DBAccess
{
    /// <summary>
    /// DataReaderAdapter class
    /// </summary>
    public sealed class DataReaderAdapter : IDisposable
    {
        private IDataReader _dataReader;
        private Dictionary<string, int> _rowSchema;

        /// <summary>
        /// Create new class instance from IDataReader object
        /// </summary>
        /// <param name="dataReader">IDataReader object</param>
        public DataReaderAdapter(IDataReader dataReader)
        {
            if (dataReader == null)
                throw new ArgumentNullException("dataReader");

            _dataReader = dataReader;
            _rowSchema = new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Get value of field with given name and convert it to type T
        /// </summary>
        /// <typeparam name="T">rezult type</typeparam>
        /// <param name="field">field name</param>
        /// <returns>field value</returns>
        public T GetValue<T>(string field)
        {
            int index = GetOrdinal(field);
            return _dataReader.IsDBNull(index) ? default(T) : (T)_dataReader.GetValue(index);
        }

        /// <summary>
        /// Get value of field with given position and convert it to type T
        /// </summary>
        /// <typeparam name="T">rezult type</typeparam>
        /// <param name="index">field position</param>
        /// <returns>field value</returns>
        public T GetValue<T>(int index)
        {
            return _dataReader.IsDBNull(index) ? default(T) : (T)_dataReader.GetValue(index);
        }

        /// <summary>
        /// Get value of field with given position as object
        /// </summary>
        /// <param name="index">field position</param>
        /// <returns>field value</returns>
        public object GetValue(int index)
        {
            return _dataReader.IsDBNull(index) ? null : _dataReader.GetValue(index);
        }

        /// <summary>
        /// Get field name by position
        /// </summary>
        /// <param name="index">field position</param>
        /// <returns>field name</returns>
        public string GetName(int index)
        {
            return _dataReader.GetName(index);
        }

        /// <summary>
        /// Get number of fields in dataset row
        /// </summary>
        /// <returns>number of fields</returns>
        public int GetFieldCount()
        {
            return _dataReader.FieldCount;
        }

        /// <summary>
        /// Check if field value is DBNull
        /// </summary>
        /// <param name="field">field name to check</param>
        /// <returns>true if value is DBNull</returns>
        public bool IsNull(string field)
        {
            return _dataReader.IsDBNull(GetOrdinal(field));
        }

        /// <summary>
        /// Check if field value is DBNull
        /// </summary>
        /// <param name="field">field position to check</param>
        /// <returns>true if value is DBNull</returns>
        public bool IsNull(int field)
        {
            return _dataReader.IsDBNull(field);
        }

        /// <summary>
        /// Check if field value is not DBNull
        /// </summary>
        /// <param name="field">field name to check</param>
        /// <returns>true if value is not DBNull</returns>
        public bool IsNotNull(string field)
        {
            return !IsNull(field);
        }

        /// <summary>
        /// Check if field value is not DBNull
        /// </summary>
        /// <param name="field">field position to check</param>
        /// <returns>true if value is not DBNull</returns>
        public bool IsNotNull(int field)
        {
            return !IsNull(field);
        }

        /// <summary>
        /// Get field value as boolean
        /// </summary>
        /// <param name="field">field name to get</param>
        /// <returns>boolean value</returns>
        public bool GetBoolean(string field)
        {
            int index = GetOrdinal(field);
            return _dataReader.IsDBNull(index) ? default(bool) : _dataReader.GetBoolean(index);
        }

        /// <summary>
        /// Get field value as byte
        /// </summary>
        /// <param name="field">field name to get</param>
        /// <returns>byte value</returns>
        public byte GetByte(string field)
        {
            int index = GetOrdinal(field);
            return _dataReader.IsDBNull(index) ? default(byte) : _dataReader.GetByte(index);
        }

        /// <summary>
        /// Get field value as byte
        /// </summary>
        /// <param name="field">field name to get</param>
        /// <param name="fieldOffset">start position in field</param>
        /// <param name="buffer">buffer</param>
        /// <param name="bufferOffset">start position in buffer</param>
        /// <param name="length">bytes to read</param>
        /// <returns>actual bytes read</returns>
        public long GetBytes(string field, long fieldOffset, byte[] buffer, int bufferOffset, int length)
        {
            return _dataReader.GetBytes(GetOrdinal(field), fieldOffset, buffer, bufferOffset, length);
        }

        /// <summary>
        /// Get field value as char
        /// </summary>
        /// <param name="field">field name to get</param>
        /// <returns>char value</returns>
        public char GetChar(string field)
        {
            int index = GetOrdinal(field);
            return _dataReader.IsDBNull(index) ? default(char) : _dataReader.GetChar(index);
        }

        /// <summary>
        /// Get field value as char[]
        /// </summary>
        /// <param name="field">field name to get</param>
        /// <param name="fieldOffset">start position in field</param>
        /// <param name="buffer">buffer</param>
        /// <param name="bufferOffset">start position in buffer</param>
        /// <param name="length">chars to read</param>
        /// <returns>actual chars read</returns>
        public long GetChars(string field, long fieldOffset, char[] buffer, int bufferOffset, int length)
        {
            return _dataReader.GetChars(GetOrdinal(field), fieldOffset, buffer, bufferOffset, length);
        }

        /// <summary>
        /// Get field value as DateTime
        /// </summary>
        /// <param name="field">field name to get</param>
        /// <returns>DateTime value</returns>
        public DateTime GetDateTime(string field)
        {
            int index = GetOrdinal(field);
            return _dataReader.IsDBNull(index) ? default(DateTime) : _dataReader.GetDateTime(index);
        }

        /// <summary>
        /// Get field value as decimal
        /// </summary>
        /// <param name="field">field name to get</param>
        /// <returns>decimal value</returns>
        public decimal GetDecimal(string field)
        {
            int index = GetOrdinal(field);
            return _dataReader.IsDBNull(index) ? default(decimal) : _dataReader.GetDecimal(index);
        }

        /// <summary>
        /// Get field value as double
        /// </summary>
        /// <param name="field">field name to get</param>
        /// <returns>double value</returns>
        public double GetDouble(string field)
        {
            int index = GetOrdinal(field);
            return _dataReader.IsDBNull(index) ? default(double) : _dataReader.GetDouble(index);
        }

        /// <summary>
        /// Get field value as Float
        /// </summary>
        /// <param name="field">field name to get</param>
        /// <returns>float value</returns>
        public float GetFloat(string field)
        {
            int index = GetOrdinal(field);
            return _dataReader.IsDBNull(index) ? default(float) : _dataReader.GetFloat(index);
        }

        /// <summary>
        /// Get field value as Guid
        /// </summary>
        /// <param name="field">field name to get</param>
        /// <returns>Guid value</returns>
        public Guid GetGuid(string field)
        {
            int index = GetOrdinal(field);
            return _dataReader.IsDBNull(index) ? default(Guid) : _dataReader.GetGuid(index);
        }

        /// <summary>
        /// Get field value as Int16
        /// </summary>
        /// <param name="field">field name to get</param>
        /// <returns>Int16 value</returns>
        public short GetInt16(string field)
        {
            int index = GetOrdinal(field);
            return _dataReader.IsDBNull(index) ? default(short) : _dataReader.GetInt16(index);
        }

        /// <summary>
        /// Get field value as Int32
        /// </summary>
        /// <param name="field">field name to get</param>
        /// <returns>Int32 value</returns>
        public int GetInt32(string field)
        {
            int index = GetOrdinal(field);
            return _dataReader.IsDBNull(index) ? default(int) : _dataReader.GetInt32(index);
        }

        /// <summary>
        /// Get field value as Int64
        /// </summary>
        /// <param name="field">field name to get</param>
        /// <returns>Int64 value</returns>
        public long GetInt64(string field)
        {
            int index = GetOrdinal(field);
            return _dataReader.IsDBNull(index) ? default(long) : _dataReader.GetInt64(index);
        }

        /// <summary>
        /// Get field value as string
        /// </summary>
        /// <param name="field">field name to get</param>
        /// <returns>string value</returns>
        public string GetString(string field)
        {
            int index = GetOrdinal(field);
            return _dataReader.IsDBNull(index) ? default(string) : _dataReader.GetString(index);
        }

        /// <summary>
        /// Skip to next resultset
        /// </summary>
        /// <returns>true if successful</returns>
        public bool NextResult()
        {
            _rowSchema.Clear();
            return _dataReader.NextResult();
        }

        /// <summary>
        /// Read next dataset row
        /// </summary>
        /// <returns></returns>
        public bool Read()
        {
            return _dataReader.Read();
        }

        /// <summary>
        /// Get object of type T from current dataset row
        /// </summary>
        /// <typeparam name="T">result type</typeparam>
        /// <returns>object of type T</returns>
        public T ReadObject<T>() where T : class, new()
        {
            T objectDTO = new T();

            object o;
            ItemDTOInfo itemInfo = ItemDTOInfoHelper.GetItemDTOInfo(typeof(T));
            foreach (DTOProperty property in itemInfo.Properties)
            {
                o = null;
                int index = _dataReader.GetOrdinal(property.FieldNameAttribute.FieldName);
                if (!_dataReader.IsDBNull(index))
                {
                    o = _dataReader[index];
                    Type ourType = property.PropertyInfo.PropertyType;
                    o = Convert.ChangeType(o, ourType);
                }
                
                property.SetValue(objectDTO, o);
            }

            return objectDTO;
        }

        #region Helper methods

        private int GetOrdinal(string field)
        {
            int index;
            if (_rowSchema.TryGetValue(field, out index))
            {
                return index;
            }
            else
            {
                try
                {
                    index = _dataReader.GetOrdinal(field);
                }
                catch (IndexOutOfRangeException e)
                {
                    throw new ArgumentException(string.Format("Invalid column name '{0}'", field), "field", e);
                }
                _rowSchema.Add(field, index);
                return index;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// IDisposable implementation
        /// </summary>
        public void Dispose()
        {
            _dataReader.Dispose();
        }

        #endregion
    }

}
