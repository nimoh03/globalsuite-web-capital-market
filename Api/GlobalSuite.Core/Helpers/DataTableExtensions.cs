using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace GlobalSuite.Core.Helpers
{
    public static class DataTableExtensions
    {
        public static List<T> ToList<T>(this DataTable table) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            List<T> result = new List<T>();

            foreach (var row in table.Rows)
            {
                var item = CreateItemFromRow<T>((DataRow)row, properties);
                result.Add(item);
            }

            return result;
        }

        private static T CreateItemFromRow<T>(DataRow row, IList<PropertyInfo> properties) where T : new()
        {
            T item = new T();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(System.DayOfWeek))
                {
                    DayOfWeek day = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), row[property.Name].ToString());
                    property.SetValue(item, day, null);
                }
                else
                {

                    try
                    {
                        if (row[property.Name] == DBNull.Value)
                            property.SetValue(item, null, null);
                        else
                        {
                            if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                            {
                                //nullable
                                object convertedValue = null;
                                try
                                {
                                    convertedValue = System.Convert.ChangeType(row[property.Name], Nullable.GetUnderlyingType(property.PropertyType));
                                }
                                catch (Exception ex)
                                {
                                }
                                property.SetValue(item, convertedValue, null);
                            }
                            else
                                property.SetValue(item, row[property.Name], null);

                        }
                    }
                    catch 
                    {

                        continue;
                    }
                       
                    
                }
            }
            return item;
        }

        public static T GetItem<T>(this DataRow dr) where T : new()
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

           return CreateItemFromRow<T>(dr, temp.GetProperties().ToList());
            // foreach (DataColumn column in dr.Table.Columns)
            // {
            //     foreach (PropertyInfo pro in temp.GetProperties().ToList())
            //     {
            //         if (pro.Name == column.ColumnName)
            //             pro.SetValue(obj, dr[column.ColumnName], null);
            //         else
            //             continue;
            //     }
            // }
            // return obj;
        }

         
    }
}
