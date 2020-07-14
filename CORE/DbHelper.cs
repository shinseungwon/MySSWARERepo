using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace CORE
{
    /// <summary>
    /// Class helps connecting database
    /// </summary>
    public sealed class DbHelper
    {
        private SqlConnection conn;
        private SqlDataAdapter adap;
        private List<SqlParameter> parameters;

        public Logger logger;

        public bool throwError = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connectionstring</param>
        public DbHelper(string connectionString)
        {
            //Connect database          
            conn = new SqlConnection(connectionString);
            adap = new SqlDataAdapter();
            parameters = new List<SqlParameter>();
        }

        /// <summary>
        /// Set logger instance for print log
        /// </summary>
        /// <param name="logger">Logger instance</param>
        public void SetLogger(Logger logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// Connect to database
        /// </summary>
        public void Connect()
        {
            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                if (logger != null)
                {
                    logger.WriteText(e.ToString());
                }

                if (throwError)
                {
                    throw (e);
                }
            }
        }

        /// <summary>
        /// Set parameter for call query
        /// </summary>
        /// <param name="parameter">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <param name="type">Parameter type</param>
        /// <param name="encrypt">Encrypt sha1 before insert ( type must be binary )</param>
        public void AddInput(string parameter, object value, SqlDbType type, bool encrypt = false, string typeName = "")
        {
            SqlParameter s;
            byte[] byteStr = null;
            string str;

            if (encrypt)
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();
                byteStr = sha1.ComputeHash(Encoding.UTF8.GetBytes((string)value));
            }

            if (type == SqlDbType.VarChar)
            {
                str = (string)value;

                if (str.Length > 8000)
                    s = new SqlParameter(parameter, type, -1);
                else
                    s = new SqlParameter(parameter, type, str.Length);
            }
            else if (type == SqlDbType.NVarChar)
            {
                str = (string)value;

                if (str.Length > 4000)
                    s = new SqlParameter(parameter, type, -1);
                else
                    s = new SqlParameter(parameter, type, str.Length);
            }
            else if (type == SqlDbType.Udt)
            {
                s = new SqlParameter(parameter, type, -1);
                s.UdtTypeName = typeName;
            }
            else if (type == SqlDbType.Structured)
            {
                s = new SqlParameter(parameter, type, -1);
                s.TypeName = typeName;
            }
            else
            {
                s = new SqlParameter(parameter, type, -1);
            }

            if (encrypt) s.Value = byteStr;
            else s.Value = value;
            parameters.Add(s);
        }

        /// <summary>
        /// Set output parameter for call query
        /// </summary>
        /// <param name="parameter">Parameter name</param>
        /// <param name="type">Parameter type</param>
        /// <param name="size">Parameter size</param>
        public void AddOutput(string parameter, SqlDbType type, int size = -1)
        {
            SqlParameter s = new SqlParameter(parameter, type, size);
            s.Direction = ParameterDirection.Output;
            parameters.Add(s);
        }

        /// <summary>
        /// Call stored procedure
        /// </summary>
        /// <param name="query">Stored procedure name</param>
        /// <param name="ds">Return dataset</param>
        /// <param name="output">Return output array</param>
        /// <returns>Customized stored procedure return value or count of affected rows</returns>
        public int CallSP(string query, ref DataSet ds, ref string[] output)
        {
            try
            {
                int i, j, retval;

                SqlCommand comm = new SqlCommand(query, conn);
                comm.CommandType = CommandType.StoredProcedure;

                for (i = 0; i < parameters.Count; i++)
                {
                    comm.Parameters.Add(parameters[i]);
                }

                adap.SelectCommand = comm;
                Connect();
                retval = adap.Fill(ds);
                Close();

                j = 0;

                for (i = 0; i < comm.Parameters.Count; i++)
                {
                    if (comm.Parameters[i].Direction == ParameterDirection.Output)
                    {
                        output[j] = comm.Parameters[i].Value.ToString();
                        j++;
                    }
                }

                parameters.Clear();
                return retval;
            }
            catch (Exception e)
            {
                if (logger != null)
                {
                    logger.WriteText(e.ToString());
                }

                if (throwError)
                {
                    throw (e);
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// Call stored procedure
        /// </summary>
        /// <param name="query">Stored procedure name</param>
        /// <param name="ds">Return dataset</param>
        /// <returns>Customized stored procedure return value or count of affected rows</returns>
        public int CallSP(string query, ref DataSet ds)
        {
            string[] str = new string[0];
            return CallSP(query, ref ds, ref str);
        }

        /// <summary>
        /// Call stored procedure
        /// </summary>
        /// <param name="query">Stored procedure name</param>
        /// <param name="output">Return output array</param>
        /// <returns>Customized stored procedure return value or count of affected rows</returns>
        public int CallSP(string query, ref string[] output)
        {
            DataSet ds = new DataSet();
            return CallSP(query, ref ds, ref output);
        }

        /// <summary>
        /// Call stored procedure
        /// </summary>
        /// <param name="query">Stored procedure name</param>
        /// <returns>Customized stored procedure return value or count of affected rows</returns>
        public int CallSP(string query)
        {
            DataSet ds = new DataSet();
            string[] str = new string[0];
            return CallSP(query, ref ds, ref str);
        }

        /// <summary>
        /// Call scolar-valued function
        /// </summary>
        /// <param name="query">Function name</param>
        /// <returns>Function return value</returns>
        public string CallFS(string query)
        {
            try
            {
                query = "SELECT dbo." + query;
                SqlCommand comm = new SqlCommand(query, conn);
                comm.CommandType = CommandType.Text;
                Connect();
                string result = comm.ExecuteScalar().ToString();
                Close();
                return result;
            }
            catch (Exception e)
            {
                if (logger != null)
                {
                    logger.WriteText(e.ToString());
                }

                if (throwError)
                {
                    throw (e);
                }
                else
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Call table-valued function
        /// </summary>
        /// <param name="query">function name</param>
        /// <returns>Function return datatable</returns>
        public DataTable CallFT(string query)
        {
            DataTable t = new DataTable();
            try
            {
                query = "SELECT * FROM dbo." + query;
                SqlCommand comm = new SqlCommand(query, conn);
                comm.CommandType = CommandType.Text;
                adap.SelectCommand = comm;
                Connect();
                adap.Fill(t);
                Close();
                return t;
            }
            catch (Exception e)
            {
                if (logger != null)
                {
                    logger.WriteText(e.ToString());
                }

                if (throwError)
                {
                    throw (e);
                }
                else
                {
                    return t;
                }
            }
        }

        /// <summary>
        /// Call query
        /// </summary>
        /// <param name="query">Query string</param>
        /// <param name="ds">Return dataset</param>
        /// <returns>Count of affected rows</returns>
        public int CallQuery(string query, ref DataSet ds)
        {
            try
            {
                SqlCommand comm = new SqlCommand(query, conn);

                foreach (SqlParameter param in parameters)
                {
                    comm.Parameters.Add(param);
                }

                if (parameters.Count > 0)
                    parameters.Clear();

                adap.SelectCommand = comm;
                Connect();
                int result = adap.Fill(ds);
                Close();
                return result;
            }
            catch (Exception e)
            {
                if (logger != null)
                {
                    logger.WriteText(e.ToString());
                }

                if (throwError)
                {
                    throw (e);
                }
                else
                {
                    return -1;
                }
            }
        }

        /// <summary>
        /// Call query
        /// </summary>
        /// <param name="query">Query string</param>
        /// <returns>Count of affected rows</returns>
        public int CallQuery(string query)
        {
            try
            {
                SqlCommand comm = new SqlCommand(query, conn);

                foreach (SqlParameter param in parameters)
                {
                    comm.Parameters.Add(param);
                }

                if (parameters.Count > 0)
                    parameters.Clear();

                Connect();
                int result = comm.ExecuteNonQuery();
                Close();
                return result;
            }
            catch (Exception e)
            {
                if (logger != null)
                {
                    logger.WriteText(e.ToString());
                }

                if (throwError)
                {
                    throw (e);
                }
                else
                {
                    return -1;
                }
            }
        }

        //소멸자
        private void Close()
        {
            conn.Close();
        }
    }
}
