using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace inTouch
{
    public static class DataManager
    {

        public static SqlConnection GetConnection()
        {
            Properties.Settings settings = new Properties.Settings();
            SqlConnectionStringBuilder cs = new SqlConnectionStringBuilder();
            cs.DataSource = settings.Server;
            cs.InitialCatalog = settings.Database;
            cs.IntegratedSecurity = true;
            string connectionString = cs.ToString();
            return new SqlConnection(connectionString);
        }

        public static bool TestConnection()
        {
            bool result = true;
            Properties.Settings settings = new Properties.Settings();
            SqlConnection cn = GetConnection();
            try
            {
                cn.Open();
            }
            catch (Exception ex)
            {
                result = false;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                cn.Close();
            }
            return result;
        }

        public static string GetCurrentUser()
        {
            SqlConnection cn = GetConnection();
            cn.Open();
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = "SELECT SUSER_NAME()";
            string result = cmd.ExecuteScalar().ToString();
            cn.Close();
            return result;
        }

        /// <summary>
        /// Adds current user to the table of online users
        /// </summary>
        /// <param name="register">If false, deletes current user from the table</param>
        public static void RegisterUser(bool register)
        {
            SqlConnection cn = GetConnection();
            try
            {
                cn.Open();
                SqlCommand cmd = cn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SetUserStatus";
                cmd.Parameters.AddWithValue("@add_user", (register) ? 1 : 0);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public static DataTable UsersList()
        {
            DataTable result = new DataTable();
            SqlConnection cn = GetConnection();
            cn.Open();
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.UserList()";
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            try
            {
                da.Fill(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cn.Close();
            }
            return result;
        }

        public static DataTable FetchMessages(DateTime after)
        {
            DataTable result = new DataTable();
            Properties.Settings settings = new Properties.Settings();
            int depth = settings.LogDepth;
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("SELECT TOP {0} * FROM Messages ", depth);
            sql.AppendFormat("WHERE Stamp>'{0}' ", after.ToString("yyyyMMdd HH:mm:ss"));
            sql.Append("ORDER BY Stamp DESC");
            SqlConnection cn = GetConnection();
            cn.Open();
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = sql.ToString();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            try
            {
                da.Fill(result);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cn.Close();
            }
            return result;
        }

        public static void PostMessage(string userName, string messageText, string address="")
        {
            SqlConnection cn = GetConnection();
            cn.Open();
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "PostMessage";
            cmd.Parameters.AddWithValue("@UserName", userName);
            cmd.Parameters.AddWithValue("@MessageText", messageText);
            if (address != "")
                cmd.Parameters.AddWithValue("@Address", address);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cn.Close();
            }
        }

        public static DateTime GetLastStamp()
        {
            object result;
            SqlConnection cn = GetConnection();
            cn.Open();
            SqlCommand cmd = cn.CreateCommand();
            cmd.CommandText = "SELECT MAX(Stamp) FROM Messages";
            try
            {
                result = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                cn.Close();
            }
            if (System.Convert.IsDBNull(result))
                return DateTime.MinValue;
            else
                return DateTime.Parse(result.ToString());
        }
    }
}
