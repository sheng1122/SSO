using DA.DataAccesses.Common;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace DA.DataAccesses
{
    public class ConfigDA : BaseDA
    {
        public ConfigDA(string connstr) : base(connstr)
        {
        }

        public T GetConfig<T>(string appName)
        {
            T obj = (T)Activator.CreateInstance(typeof(T));

            using (MySqlConnection conn = new MySqlConnection(_connStr))
            {
                conn.Open();
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand("usp_get_config", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("appName", appName);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Dictionary<string, PropertyInfo> propertyInfoes = new Dictionary<string, PropertyInfo>();

                            foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
                            {
                                propertyInfoes[propertyInfo.Name] = propertyInfo;
                            }

                            while (reader.Read())
                            {
                                string name = reader["name"].ToString();
                                var value = reader["value"];
                                string[] names = name.Split('.');

                                if (propertyInfoes.ContainsKey(names.First()))
                                {
                                    PropertyInfo propertyInfo = propertyInfoes[names.First()];

                                    if (propertyInfo.PropertyType.IsPrimitive
                                        || propertyInfo.PropertyType == typeof(string))
                                    {
                                        Type t = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                                        propertyInfo.SetValue(obj, Convert.ChangeType(value, t), null);
                                    }
                                    else
                                    {
                                        SetProperty(name, obj, value);
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

            return obj;
        }

        private void SetProperty(string compoundProperty, object target, object value)
        {
            string[] bits = compoundProperty.Split('.');
            object propertyTarget = target;


            PropertyInfo propertyObj = target.GetType().GetProperty(bits[0]);

            object obj = propertyObj.GetValue(propertyTarget, null);

            if (obj == null)
            {
                obj = Activator.CreateInstance(propertyObj.GetType());
                propertyObj.SetValue(target, obj, null);
            }

            for (int i = 0; i < bits.Length - 1; i++)
            {
                PropertyInfo propertyToGet = propertyTarget.GetType().GetProperty(bits[i]);
                propertyTarget = propertyToGet.GetValue(propertyTarget, null);
            }

            PropertyInfo propertyToSet = propertyTarget.GetType().GetProperty(bits.Last());

            if (value != null)
            {
                switch (propertyToSet.PropertyType.ToString().ToLower())
                {
                    case "system.guid":
                        propertyToSet.SetValue(propertyTarget, Guid.Parse(value.ToString()), null);
                        break;
                    case "system.decimal":
                        propertyToSet.SetValue(propertyTarget, decimal.Parse(value.ToString()), null);
                        break;
                    default:
                        propertyToSet.SetValue(propertyTarget, value, null);
                        break;
                }
            }

            if (propertyTarget != null)
            {
                switch (propertyObj.PropertyType.ToString().ToLower())
                {
                    case "system.guid":
                        propertyObj.SetValue(target, Guid.Parse(value.ToString()), null);
                        break;
                    case "system.decimal":
                        propertyObj.SetValue(target, decimal.Parse(value.ToString()), null);
                        break;
                    default:
                        propertyObj.SetValue(target, propertyTarget, null);
                        break;
                }
            }
        }
    }
}
