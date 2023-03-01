#region [ Using ]
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace InovoCIM.Data.Repository
{
    public class ModelRepository<T>
    {
        public async virtual Task<List<T>> ConvertToList(SqlDataReader reader)
        {
            List<T> Model = new List<T>();
            while (await reader.ReadAsync())
            {
                T Item = Activator.CreateInstance<T>();
                foreach (PropertyInfo Property in typeof(T).GetProperties())
                {
                    if (!reader.IsDBNull(reader.GetOrdinal(Property.Name)))
                    {
                        Type convertTo = Nullable.GetUnderlyingType(Property.PropertyType) ?? Property.PropertyType;
                        Property.SetValue(Item, Convert.ChangeType(reader[Property.Name], convertTo), null);
                    }
                }
                Model.Add(Item);
            }
            return Model;
        }
    }
}
