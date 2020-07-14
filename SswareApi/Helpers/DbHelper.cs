using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace Helper
{
    /// <summary>
    /// Class helps handling database
    /// </summary>
    public sealed class DbHelper
    {
        public readonly string ConnectionString;
        public bool ThrowError = true;

        private readonly List<SqlParameter> Parameters;
        private readonly LogHelper LogHelper;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connectionstring</param>
        public DbHelper(string connectionString)
        {
            //Connect database          
            ConnectionString = connectionString;
            Parameters = new List<SqlParameter>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connectionstring</param>
        /// <param name="logHelper">logHelper object</param>
        public DbHelper(string connectionString, LogHelper logHelper) : this(connectionString)
        {
            //Set LogHelper
            LogHelper = logHelper;
            logHelper.DbHelper = this;
        }

        /// <summary>
        /// Set parameter for call query
        /// </summary>
        /// <param name="parameter">Parameter name</param>
        /// <param name="value">Parameter value</param>
        /// <param name="type">Parameter type</param>
        /// <param name="encrypt">Encrypt sha1 before insert ( type must be binary )</param>
        /// <param name="typeName">Typename for udt, Structured</param>
        public void AddInput(string parameter, object value, SqlDbType type
            , bool encrypt = false, string typeName = "")
        {             
            SqlParameter s = new SqlParameter(parameter, type);

            if (type == SqlDbType.Udt)
            {
                s.UdtTypeName = typeName;
            }
            else if (type == SqlDbType.Structured)
            {
                s.TypeName = typeName;
            }

            if (encrypt)
            {
                using (SHA1 sha1 = new SHA1CryptoServiceProvider())
                {
                    s.Value = sha1.ComputeHash(Encoding.UTF8.GetBytes((string)value));
                }
            }
            else
            {
                s.Value = value;
            }                 
            
            Parameters.Add(s);
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
            Parameters.Add(s);
        }

        /// <summary>
        /// Call stored procedure
        /// </summary>
        /// <param name="query">Stored procedure name</param>
        /// <param name="ds">Return dataset</param>
        /// <param name="output">Return output array</param>
        /// <returns>Customized stored procedure return value or count of affected rows</returns>
        public int StoredProcedure(string query, DataSet ds, string[] output)
        {
            try
            {
                int i, j = 0, retval;
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand comm = new SqlCommand(query, conn))
                using (SqlDataAdapter adap = new SqlDataAdapter(comm))
                {
                    comm.CommandType = CommandType.StoredProcedure;
                    for (i = 0; i < Parameters.Count; i++)
                    {
                        comm.Parameters.Add(Parameters[i]);
                    }
                    conn.Open();
                    retval = adap.Fill(ds);

                    for (i = 0; i < comm.Parameters.Count; i++)
                    {
                        if (comm.Parameters[i].Direction == ParameterDirection.Output)
                        {
                            output[j] = comm.Parameters[i].Value.ToString();
                            j++;
                        }
                    }
                    Parameters.Clear();
                }

                return retval;
            }
            catch (Exception e)
            {
                if (LogHelper != null)
                {
                    LogHelper.WriteText("DbHelper", e.ToString());
                }

                if (ThrowError)
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
        public int StoredProcedure(string query, DataSet ds)
        {
            string[] str = new string[0];
            return StoredProcedure(query, ds, str);
        }

        /// <summary>
        /// Call stored procedure
        /// </summary>
        /// <param name="query">Stored procedure name</param>
        /// <param name="output">Return output array</param>
        /// <returns>Customized stored procedure return value or count of affected rows</returns>
        public int StoredProcedure(string query, string[] output)
        {
            DataSet ds = new DataSet();
            return StoredProcedure(query, ds, output);
        }

        /// <summary>
        /// Call stored procedure
        /// </summary>
        /// <param name="query">Stored procedure name</param>
        /// <returns>Customized stored procedure return value or count of affected rows</returns>
        public int StoredProcedure(string query)
        {
            DataSet ds = new DataSet();
            string[] str = new string[0];
            return StoredProcedure(query, ds, str);
        }

        /// <summary>
        /// Call scolar-valued function
        /// </summary>
        /// <param name="query">Function name</param>
        /// <returns>Function return value</returns>
        public string ScalarValuedFunction(string query)
        {
            try
            {
                string result = "";
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    query = "SELECT dbo." + query;
                    comm.CommandType = CommandType.Text;
                    conn.Open();
                    result = comm.ExecuteScalar().ToString();
                }
                return result;
            }
            catch (Exception e)
            {
                if (LogHelper != null)
                {
                    LogHelper.WriteText("DbHelper", e.ToString());
                }

                if (ThrowError)
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
        public DataTable TableValuedFunction(string query)
        {
            DataTable t = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand comm = new SqlCommand(query, conn))
                using (SqlDataAdapter adap = new SqlDataAdapter(comm))
                {
                    query = "SELECT * FROM dbo." + query;
                    comm.CommandType = CommandType.Text;
                    conn.Open();
                    adap.Fill(t);
                }
                return t;
            }
            catch (Exception e)
            {
                if (LogHelper != null)
                {
                    LogHelper.WriteText("DbHelper", e.ToString());
                }

                if (ThrowError)
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
        public int Query(string query, DataSet ds)
        {
            try
            {
                int result = 0;
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand comm = new SqlCommand(query, conn))
                using (SqlDataAdapter adap = new SqlDataAdapter(comm))
                {
                    conn.Open();
                    result = adap.Fill(ds);
                }
                return result;
            }
            catch (Exception e)
            {
                if (LogHelper != null)
                {
                    LogHelper.WriteText("DbHelper", e.ToString());
                }

                if (ThrowError)
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
        public int Query(string query)
        {
            try
            {
                int result = 0;
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                using (SqlCommand comm = new SqlCommand(query, conn))
                {
                    conn.Open();
                    result = comm.ExecuteNonQuery();
                }
                return result;
            }
            catch (Exception e)
            {
                if (LogHelper != null)
                {
                    LogHelper.WriteText("DbHelper", e.ToString());
                }

                if (ThrowError)
                {
                    throw (e);
                }
                else
                {
                    return -1;
                }
            }
        }
    }
}
