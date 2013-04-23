using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VlastelinServer.DAO.DBAccess
{
    /// <summary>
    /// Base interface for data objects materialization
    /// </summary>
    /// <typeparam name="T">Data object type</typeparam>
    public interface IMaterializator<T> where T : class
    {
        /// <summary>
        /// Materialize object from DataReader
        /// Note: All result sets should be read in the this method
        /// </summary>
        /// <param name="dataReader">DataReader packed to the helper class DataReaderAdapter</param>
        /// <returns>Instance of T</returns>
        T Materialize(DataReaderAdapter dataReader);

        /// <summary>
        /// Materialize list of objects from DataReader
        /// Note: All result sets should be read in the this method
        /// </summary>
        /// <param name="dataReader">DataReader packed to the helper class DataReaderAdapter</param>
        /// <returns>List of T instances</returns>
        List<T> Materialize_List(DataReaderAdapter dataReader);
    }

    /// <summary>
    /// Base interface for data objects materialization
    /// </summary>
    /// <typeparam name="T">Data object type</typeparam>
    public abstract class MaterializatorBase<T> : IMaterializator<T> where T : class
    {
        //в методе ReadSingleObject не требуется вызывать метод Read(), 
        //чтобы переместить указатель на запись, 
        //т.к. он вызывается в базовом классе материализатора
        /// <summary>
        /// Read Single Object from DataReaderAdapter
        /// </summary>
        /// <param name="dataReader">DataReaderAdapter</param>
        /// <returns>result of type T</returns>
        public abstract T ReadSingleObject(DataReaderAdapter dataReader);

        /// <summary>
        /// Materialize object from DataReader
        /// Note: All result sets should be read in the this method
        /// </summary>
        /// <param name="dataReader">DataReader packed to the helper class DataReaderAdapter</param>
        /// <returns>Instance of T</returns>
        public virtual T Materialize(DataReaderAdapter dataReader)
        {
            if (dataReader == null)
            {
                throw new ArgumentNullException("output ");
            }

            T output = null;
            if (dataReader.Read())
            {
                output = ReadSingleObject(dataReader);
            }

            return output;
        }

        /// <summary>
        /// Materialize list of objects from DataReader
        /// Note: All result sets should be read in the this method
        /// </summary>
        /// <param name="dataReader">DataReader packed to the helper class DataReaderAdapter</param>
        /// <returns>List of T instances</returns>
        public virtual List<T> Materialize_List(DataReaderAdapter dataReader)
        {
            if (dataReader == null)
            {
                throw new ArgumentNullException("output ");
            }

            List<T> output = new List<T>();
            while (dataReader.Read())
            {
                output.Add(ReadSingleObject(dataReader));
            }

            return output;
        }
    }

}
