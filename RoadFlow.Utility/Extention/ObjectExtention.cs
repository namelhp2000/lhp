using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace RoadFlow.Utility
{
    /// <summary>
    /// 对象扩展
    /// </summary>
    public static class ObjectExtention
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        static ObjectExtention()
        {
            JsonSerializerSettings setting = new JsonSerializerSettings();
            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
            {
                //日期类型默认格式化处理
                setting.DateFormatHandling = DateFormatHandling.MicrosoftDateFormat;
                setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                return setting;
            });
        }

        /// <summary>
        /// 将一个object对象序列化，返回一个byte[]         
        /// </summary> 
        /// <param name="obj">能序列化的对象</param>
        /// <returns></returns> 
        public static byte[] ToBytes(this object obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                return ms.GetBuffer();
            }
        }

        /// <summary>
        /// 判断是否为Null或者空
        /// </summary>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this object obj)
        {
            if (obj == null)
                return true;
            else
            {
                string objStr = obj.ToString();
                return string.IsNullOrEmpty(objStr);
            }
        }

        /// <summary>
        /// 将对象序列化成Json字符串
        /// </summary>
        /// <param name="obj">需要序列化的对象</param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 实体类转json数据，速度快
        /// </summary>
        /// <param name="t">实体类</param>
        /// <returns></returns>
        public static string EntityToJson(this object t)
        {
            if (t == null)
                return null;
            string jsonStr = "";
            jsonStr += "{";
            PropertyInfo[] infos = t.GetType().GetProperties();
            for (int i = 0; i < infos.Length; i++)
            {
                jsonStr = jsonStr + "\"" + infos[i].Name + "\":\"" + infos[i].GetValue(t).ToString() + "\"";
                if (i != infos.Length - 1)
                    jsonStr += ",";
            }
            jsonStr += "}";
            return jsonStr;
        }

        /// <summary>
        /// 深复制
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static T DeepClone<T>(this T obj) where T : class
        {
            if (obj == null)
                return null;

            return obj.ToJson().ToObject<T>();
        }

        /// <summary>
        /// 将对象序列化为XML字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string ToXmlStr<T>(this T obj)
        {
            var jsonStr = obj.ToJson();
            var xmlDoc = JsonConvert.DeserializeXmlNode(jsonStr);
            string xmlDocStr = xmlDoc.InnerXml;

            return xmlDocStr;
        }

        /// <summary>
        /// 将对象序列化为XML字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象</param>
        /// <param name="rootNodeName">根节点名(建议设为xml)</param>
        /// <returns></returns>
        public static string ToXmlStr<T>(this T obj, string rootNodeName)
        {
            var jsonStr = obj.ToJson();
            var xmlDoc = JsonConvert.DeserializeXmlNode(jsonStr, rootNodeName);
            string xmlDocStr = xmlDoc.InnerXml;

            return xmlDocStr;
        }

        /// <summary>
        /// 获取某属性值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <returns></returns>
        public static object GetPropertyValue(this object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).GetValue(obj);
        }

        /// <summary>
        /// 获取某属性值
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="propertyName">属性名</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static void SetPropertyValue(this object obj, string propertyName, object value)
        {
            obj.GetType().GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static).SetValue(obj, value);
        }

        /// <summary>
        /// 改变类型
        /// </summary>
        /// <param name="obj">原对象</param>
        /// <param name="targetType">目标类型</param>
        /// <returns></returns>
        public static object ChangeType(this object obj, Type targetType)
        {
            object resObj = null;
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                NullableConverter newNullableConverter = new NullableConverter(targetType);
                resObj = newNullableConverter.ConvertFrom(obj);
            }
            else
            {
                resObj = Convert.ChangeType(obj, targetType);
            }

            return resObj;
        }


        /// <summary>
        /// 将Json字符串反序列化为对象
        /// </summary>
        /// <param name="jsonStr">json字符串</param>
        /// <param name="type">对象类型</param>
        /// <returns></returns>
        public static object ToObject(this string jsonStr, Type type)
        {
            return JsonConvert.DeserializeObject(jsonStr, type);
        }



        /// <summary>
        ///     取得对象指定属性的值
        /// </summary>
        /// <param name="predicate">要取值的属性</param>
        /// <returns></returns>
        public static object GetPropertyValue<T, TProperty>(this T obj, Expression<Func<T, TProperty>> predicate)
        {
            var propertyName = predicate.GetPropertyName(); //属性名称

            return obj.GetPropertyValue(propertyName);
        }

        /// <summary>
        ///     取对象属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName">支持“.”分隔的多级属性取值。</param>
        /// <returns></returns>
        public static object GetPropertyValue<T>(this T obj, string propertyName)
        {
            var strs = propertyName.Split('.');

            PropertyInfo property = null;
            object value = obj;

            for (var i = 0; i < strs.Length; i++)
            {
                property = value.GetType().GetProperty(strs[i]);
                value = property.GetValue(value, null);
            }
            return value;
        }




        #region 反射设置属性与取值
        public static string GetPropertyName<T, TProperty>(this Expression<Func<T, TProperty>> predicate)
        {
            var expression = predicate.Body as MemberExpression;
            //return expression.Member.Name; //该属性只返回最后一个属性，因此采用下面方法返回。
            return expression.ToString().Substring(2);
        }



        /// <summary>
        ///     设置对象指定属性的值
        /// </summary>
        /// <param name="predicate">要设置值的属性</param>
        /// <param name="value">设置值</param>
        /// <returns>是否设置成功</returns>
        public static bool SetPropertyValue<T, TProperty>(this T obj, Expression<Func<T, TProperty>> predicate,
            object value)
        {
            var propertyName = predicate.GetPropertyName(); //属性名称

            return obj.SetPropertyValue(propertyName, value);
        }

        /// <summary>
        ///     设置对象属性值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName">propertyName1.propertyName2.propertyName3</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetPropertyValue<T>(this T obj, string propertyName, object value)
        {
            var strs = propertyName.Split('.');

            PropertyInfo property = null;
            object target = obj;

            for (var i = 0; i < strs.Length; i++)
            {
                property = target.GetType().GetProperty(strs[i]);
                if (i < strs.Length - 1)
                    target = property.GetValue(target, null);
            }

            var flag = false; //设置成功标记
            if (property != null && property.CanWrite)
            {
                if (false == property.PropertyType.IsGenericType) //非泛型
                {
                    if (property.PropertyType.IsEnum)
                    {
                        property.SetValue(target, Convert.ChangeType(value, typeof(int)));
                        flag = true;
                    }
                    else if (value.ToString() != property.PropertyType.ToString())
                    {
                        //property.SetValue(target, string.IsNullOrEmpty(value) ? null : Convert.ChangeType(value, property.PropertyType), null);
                        property.SetValue(target,
                            value == null ? null : Convert.ChangeType(value, property.PropertyType),
                            null);
                        flag = true;
                    }
                }
                else //泛型Nullable<>
                {
                    var genericTypeDefinition = property.PropertyType.GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof(Nullable<>))
                    {
                        //property.SetValue(target, string.IsNullOrEmpty(value) ? null : Convert.ChangeType(value, Nullable.GetUnderlyingType(property.PropertyType)), null);
                        property.SetValue(target,
                            value == null
                                ? null
                                : Convert.ChangeType(value, Nullable.GetUnderlyingType(property.PropertyType)),
                            null);
                        flag = true;
                    }
                }
            }

            return flag;
        }


        #endregion



    }
}
