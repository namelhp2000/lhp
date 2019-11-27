using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace RoadFlow.Mapper
{



    #region 新的方法2.8.3
    /// <summary>
    /// 映射通用方法
    /// </summary>
    public class Common
    {
        #region 设置只读数据类型映射
        // 字段类型映射
        public static readonly Type _Bool = typeof(bool);
        public static readonly Type _DateTime = typeof(DateTime);
        public static readonly Type _DateTimeNull = typeof(DateTime?);
        public static readonly Type _Decimal = typeof(decimal);
        public static readonly Type _DecimalNull = typeof(decimal?);
        public static readonly Type _Double = typeof(double);
        public static readonly Type _DoubleNull = typeof(double?);
        public static readonly Type _Float = typeof(float);
        public static readonly Type _FloatNull = typeof(float?);
        public static readonly Type _Guid = typeof(Guid);
        public static readonly Type _GuidNull = typeof(Guid?);
        public static readonly Type _Int = typeof(int);
        public static readonly Type _IntNull = typeof(int?);
        public static readonly Type _Long = typeof(long);
        public static readonly Type _LongNull = typeof(long?);
        public static readonly Type _Short = typeof(short);
        public static readonly Type _ShortNull = typeof(short?);
        public static readonly Type _String = typeof(string);
        public static readonly Dictionary<Type, Type> NullTypes;

        #endregion




        // Methods
        static Common()
        {
            Dictionary<Type, Type> dictionary1 = new Dictionary<Type, Type>();
            dictionary1.Add(_GuidNull, _Guid);
            dictionary1.Add(_DateTimeNull, _DateTime);
            dictionary1.Add(_ShortNull, _Short);
            dictionary1.Add(_LongNull, _Long);
            dictionary1.Add(_DoubleNull, _Double);
            dictionary1.Add(_FloatNull, _Float);
            dictionary1.Add(_DecimalNull, _Decimal);
            NullTypes = dictionary1;
        }



        /// <summary>
        /// 获取数据字段类型， 针对数据类型含空处理
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public static Type GetDataColumnType(Type propertyType)
        {
            if (NullTypes.ContainsKey(propertyType))
            {
                return NullTypes[propertyType];
            }
            return propertyType;
        }

        /// <summary>
        /// 获取参数值 ,针对Guid与日期类型特殊处理
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object GetParameterValue(object value)
        {

            if ((value == null) || (value == DBNull.Value))
            {
                return DBNull.Value;
            }
            Type type = value.GetType();
            if (_Guid == type) //类型是否是标识符
            {
                return value.ToString().ToUpper();
            }
            if (_DateTime == type)//类型是否是日期
            {
                return DateTime.Parse(value.ToString());
            }
            return value.ToString();
        }





        /// <summary>
        /// 获取主键和类型 ，针对Orcle数据进行处理
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static List<ValueTuple<string, Type>> GetPrimaryKeyAndTypes(PropertyInfo[] properties)
        {
            List<ValueTuple<string, Type>> list = new List<ValueTuple<string, Type>>();
            foreach (PropertyInfo info in properties)
            {
                if (info.GetCustomAttributes((Type)typeof(KeyAttribute), false).Length != 0) //判断是否是自定义属性
                {
                    list.Add(new ValueTuple<string, Type>(info.Name, info.PropertyType));
                }
            }

            if (!Enumerable.Any<ValueTuple<string, Type>>((IEnumerable<ValueTuple<string, Type>>)list))//都不满足
            {
                foreach (PropertyInfo info2 in properties)
                {
                    if (info2.Name.Equals("id", (StringComparison)StringComparison.CurrentCultureIgnoreCase))
                    {
                        list.Add(new ValueTuple<string, Type>("Id", info2.PropertyType));
                        return list;
                    }
                }
            }
            return list;
        }


        /// <summary>
        /// 获取主键
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static List<string> GetPrimaryKeys(PropertyInfo[] properties)
        {
            List<string> list = new List<string>();
            foreach (PropertyInfo info in properties)
            {
                if (info.GetCustomAttributes((Type)typeof(KeyAttribute), false).Length != 0)
                {
                    list.Add(info.Name);
                }
            }
            if (!Enumerable.Any<string>((IEnumerable<string>)list) && Enumerable.Any<PropertyInfo>(properties,key=>key.Name.Equals("id", (StringComparison)StringComparison.CurrentCultureIgnoreCase)))

            {
                list.Add("Id");
            }
            return list;
        }


        /// <summary>
        /// 取读对应类型的值 针对Oracle
        /// </summary>
        /// <param name="propertyType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object GetReaderValue(Type propertyType, object value)
        {
            if ((value == null) || (DBNull.Value == value))
            {
                return DBNull.Value;
            }
            if (propertyType == _String)
            {
                return value.ToString();
            }
            if (propertyType == _Guid)
            {
                return Guid.Parse(value.ToString());
            }
            if (propertyType == _DateTime)
            {
                return DateTime.Parse(value.ToString());
            }
            if (propertyType == _Short)
            {
                return (short)short.Parse(value.ToString());
            }
            if (propertyType == _Int)
            {
                return (int)int.Parse(value.ToString());
            }
            if (propertyType == _Long)
            {
                return (long)long.Parse(value.ToString());
            }
            if (propertyType == _Double)
            {
                return (double)double.Parse(value.ToString());
            }
            if (propertyType == _Float)
            {
                return (float)float.Parse(value.ToString());
            }
            if (propertyType == _Decimal)
            {
                return decimal.Parse(value.ToString());
            }
            if (propertyType == _Bool)
            {
                return (bool)bool.Parse(value.ToString());
            }
            if (propertyType == _GuidNull)
            {
                Guid guid;
                return (Guid.TryParse(value.ToString(), out guid) ? new Guid?(guid) : null);
            }
            if (propertyType == _DateTimeNull)
            {
                DateTime time;
                return (DateTime.TryParse(value.ToString(), out time) ? new DateTime?(time) : null);
            }
            if (propertyType == _ShortNull)
            {
                short num;
                return (short.TryParse(value.ToString(), out num) ? new short?(num) : null);
            }
            if (propertyType == _IntNull)
            {
                int num2;
                return (int.TryParse(value.ToString(), out num2) ? new int?(num2) : null);
            }
            if (propertyType == _LongNull)
            {
                long num3;
                return (long.TryParse(value.ToString(), out num3) ? new long?(num3) : null);
            }
            if (propertyType == _DoubleNull)
            {
                double num4;
                return (double.TryParse(value.ToString(), out num4) ? new double?(num4) : null);
            }
            if (propertyType == _FloatNull)
            {
                float num5;
                return (float.TryParse(value.ToString(), out num5) ? new float?(num5) : null);
            }
            if (propertyType == _DecimalNull)
            {
                decimal num6;
                return (decimal.TryParse(value.ToString(), out num6) ? new decimal?(num6) : null);
            }
            return value;
        }

        /// <summary>
        /// 获取表名
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetTableName(Type type)
        {
            object[] customAttributes = type.GetCustomAttributes((Type)typeof(TableAttribute), false);
            if (customAttributes.Length == 0)
            {
                return type.Name;
            }
            return (customAttributes[0] as TableAttribute).Name;
        }




        /// <summary>
        /// List列表转DataTable表的方式输出  针对SQLServer批量复制
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ts"></param>
        /// <returns></returns>
        public static DataTable ListToDataTable<T>(IEnumerable<T> ts)
        {
            DataTable table = new DataTable();
            if (Enumerable.Any<T>(ts))
            {
                Type type = typeof(T);
                PropertyInfo[] properties = type.GetProperties();
                string tableName = GetTableName(type);
                if (string.IsNullOrWhiteSpace(tableName))
                {
                    return table;
                }
                table.TableName = tableName;
                foreach (PropertyInfo info in properties)
                {
                    DataColumn column1 = new DataColumn(info.Name);
                    //处理Nullable报错
                    column1.DataType = Nullable.GetUnderlyingType(GetDataColumnType(info.PropertyType)) ?? GetDataColumnType(info.PropertyType);
                    DataColumn column = column1;
                    table.Columns.Add(column);
                }
                foreach (T local in ts)
                {
                    new List<object>();
                    DataRow row = table.NewRow();
                    foreach (PropertyInfo info2 in properties)
                    {
                        object obj2 = info2.GetValue(local);
                        row[info2.Name] = obj2 ?? DBNull.Value;
                    }
                    table.Rows.Add(row);
                }
            }
            return table;
        }



     
    }






    #endregion

}


