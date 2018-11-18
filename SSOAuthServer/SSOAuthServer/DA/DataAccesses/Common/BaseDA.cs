using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;

namespace DA.DataAccesses.Common
{
    public class BaseDA
    {
        protected string _connStr;

        public BaseDA(string connStr)
        {
            _connStr = connStr;
        }

        protected void ArgumentMapping(MySqlCommand cmd, Object arg)
        {
            string jsonArg = JsonConvert.SerializeObject(arg);

            Dictionary<string, object> dicArg = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonArg);

            foreach (string key in dicArg.Keys)
            {
                cmd.Parameters.AddWithValue(key, dicArg[key]);
            }
        }

        protected List<T> GetList<T>(string spName, Object arg = null)
        {
            List<T> list = new List<T>();

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(spName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (arg != null)
                        {
                            ArgumentMapping(cmd, arg);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            list = GetList<T>(reader);
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    conn.Close();
                    MySqlConnection.ClearPool(conn);
                }
            }

            return list;
        }

        protected List<T> GetList<T>(MySqlDataReader reader)
        {
            List<T> list = new List<T>();

            Dictionary<string, PropertyInfo> propertyInfoes = new Dictionary<string, PropertyInfo>();
            Dictionary<string, string> colNames = new Dictionary<string, string>();

            if (reader.Read())
            {
                //get colname from result set
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string colName = reader.GetName(i);
                    colNames[colName.Replace("_", "").ToLower()] = colName;
                }

                if (typeof(T).IsPrimitive || typeof(T) == typeof(string))
                {
                    T obj = default(T);

                    do
                    {
                        foreach (string colName in colNames.Values)
                        {
                            var value = reader[colName];

                            Type t = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
                            obj = (T)Convert.ChangeType(value, t);
                        }

                        list.Add(obj);
                    } while (reader.Read());
                }
                else
                {
                    T obj = (T)Activator.CreateInstance(typeof(T));

                    foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
                    {
                        if (colNames.ContainsKey(propertyInfo.Name.ToLower()))
                        {
                            string colName = colNames[propertyInfo.Name.ToLower()];

                            propertyInfoes[colName] = propertyInfo;
                        }
                    }

                    colNames = null;

                    do
                    {
                        obj = (T)Activator.CreateInstance(typeof(T));
                        foreach (string colName in propertyInfoes.Keys)
                        {
                            var value = reader[colName];

                            if (value != DBNull.Value)
                            {
                                PropertyInfo propertyInfo = propertyInfoes[colName];
                                Type t = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                                propertyInfo.SetValue(obj, Convert.ChangeType(value, t), null);
                            }
                        }

                        list.Add(obj);
                    } while (reader.Read());
                }
            }

            return list;
        }

        protected int ExecuteNonQuery(string spName, Object arg = null)
        {
            var status = 0;

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(spName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (arg != null)
                        {
                            ArgumentMapping(cmd, arg);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            status = reader.RecordsAffected;
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    conn.Close();
                    MySqlConnection.ClearPool(conn);
                }
            }

            return status;
        }

        protected int GetCount(string spName, Object arg = null)
        {
            var count = 0;

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(spName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (arg != null)
                        {
                            ArgumentMapping(cmd, arg);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            count = Convert.ToInt32(GetSingleData(reader, "count"));
                        }
                    }
                }
                catch
                {
                    throw;
                }
                finally
                {
                    conn.Close();
                    MySqlConnection.ClearPool(conn);
                }
            }

            return count;
        }

        protected object GetSingleData(MySqlDataReader reader, string colName)
        {
            object value = null;

            if (reader.Read())
            {
                value = reader[colName];
            }

            return value;
        }

        protected List<T> GetResultSet<T>(string spName, out int totalCount, out int filteredCount, Object arg = null)
        {
            List<T> list = new List<T>();
            totalCount = 0;
            filteredCount = 0;

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(spName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (arg != null)
                        {
                            ArgumentMapping(cmd, arg);
                        }

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            list = GetResultSet<T>(reader, out totalCount, out filteredCount);
                        }
                    }

                }
                catch
                {
                    throw;
                }
                finally
                {
                    conn.Close();
                    MySqlConnection.ClearPool(conn);
                }
            }

            return list;
        }
        
        protected List<T> GetResultSet<T>(MySqlDataReader reader, out int totalCount, out int filteredCount)
        {
            List<T> list = new List<T>();
            totalCount = 0;
            filteredCount = 0;
            bool isFirstResultSet = true;
            bool valid = true;
            var loopCount = 3; //this resultSet contain 3 result

            for (int i = 0; i < loopCount; i++)
            {
                if (isFirstResultSet)
                {
                    isFirstResultSet = false;
                }
                else
                {
                    valid = reader.NextResult();
                }

                if (valid)
                {
                    if (i == 0)
                    {
                        totalCount = Convert.ToInt32(GetSingleData(reader, "totalCount"));
                    }
                    else if (i == 1)
                    {
                        filteredCount = Convert.ToInt32(GetSingleData(reader, "filteredCount"));

                    }
                    else
                    {
                        list = GetList<T>(reader);
                    }
                }
            }

            return list;
        }

        protected int ExecuteSingleQuery<T>(string spName, out T singleRecord, Object arg = null)
        {
            singleRecord = default(T);
            var status = 0;

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(spName, conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        if (arg != null)
                        {
                            ArgumentMapping(cmd, arg);
                        }

                        Dictionary<string, PropertyInfo> propertyInfoes = new Dictionary<string, PropertyInfo>();
                        Dictionary<string, string> colNames = new Dictionary<string, string>();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                status++;

                                singleRecord = (T)Activator.CreateInstance(typeof(T));

                                //get colname from result set
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string colName = reader.GetName(i);
                                    colNames[colName.Replace("_", "").ToLower()] = colName;
                                }

                                foreach (PropertyInfo propertyInfo in singleRecord.GetType().GetProperties())
                                {
                                    if (colNames.ContainsKey(propertyInfo.Name.ToLower()))
                                    {
                                        string colName = colNames[propertyInfo.Name.ToLower()];

                                        propertyInfoes[colName] = propertyInfo;
                                    }
                                }

                                colNames = null;

                                foreach (string colName in propertyInfoes.Keys)
                                {
                                    var value = reader[colName];

                                    if (value != DBNull.Value)
                                    {
                                        PropertyInfo propertyInfo = propertyInfoes[colName];
                                        Type t = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                                        propertyInfo.SetValue(singleRecord, Convert.ChangeType(value, t), null);
                                    }
                                }
                            }

                        }
                    }

                }
                catch
                {
                    throw;
                }
                finally
                {
                    conn.Close();
                    MySqlConnection.ClearPool(conn);
                }
            }

            return status;
        }
    }
}
