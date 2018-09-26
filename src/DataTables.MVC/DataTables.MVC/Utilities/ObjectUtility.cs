
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Web.Script.Serialization;
using System.Collections;
using DataTables.MVC.Attributes;
using DataTables.MVC.Interfaces;

namespace DataTables.MVC.Utilities
{
    public static class ObjectUtility
    {
        /// <summary>
        /// To copy all property from source object to destination object
        /// Applicable for 
        /// - Exact same object type
        /// - Different object type, will copy property with same name
        /// - DIfferent object type, with copy property based on DataTables.MVC.Attributes.PropertyMapAttribute specified
        /// </summary>
        /// <param name="source">source object</param>
        /// <param name="target">target object</param>
        /// <param name="skipNullProperty"> [Optional] Set to TRUE to not copy null property from source entity, defaule : FALSE</param>
        public static void CopyProperties(Object source, Object target, bool skipNullProperty = false)
        {
            if (source == target)
            {
                return;
            }

            foreach (PropertyInfo sourceProp in source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                //if (sourceProp.PropertyType != typeof(byte[]) && sourceProp.PropertyType != typeof(string) &&
                //    (sourceProp.PropertyType.IsClass || sourceProp.PropertyType.IsInterface))
                //{
                //    continue;
                //}

                if (!sourceProp.CanWrite)
                {
                    continue;
                }

                if (sourceProp.GetCustomAttributes<KeyAttribute>(true).Any())
                {
                    continue;
                }

                //Set the value if object contain PropertyMapAttribute
                List<PropertyMapAttribute> mapAttrs = sourceProp.GetCustomAttributes<PropertyMapAttribute>().ToList();
                foreach (PropertyMapAttribute mapAttr in mapAttrs)
                {
                    if (target.GetType() == mapAttr.TargetPropertyType)
                    {
                        PropertyInfo targetProp = target.GetType().GetProperty(mapAttr.TargetPropertyName);
                        SetValue(targetProp, sourceProp, target, source, skipNullProperty);
                    }
                }

                //set the value if property have the same name
                PropertyInfo targetSameProp = target.GetType().GetProperty(sourceProp.Name);
                SetValue(targetSameProp, sourceProp, target, source, skipNullProperty);

            }

            foreach (FieldInfo f in source.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance))
            {
                if (f.FieldType != typeof(byte[]) && f.FieldType != typeof(string) &&
                    (f.FieldType.IsClass || f.FieldType.IsInterface))
                {
                    continue;
                }

                f.SetValue(target, f.GetValue(source));
            }
        }

        public static void SetCompoundValue(string compoundProperty, object target, object value)
        {
            string[] bits = compoundProperty.Split('.');
            for (int i = 0; i < bits.Length - 1; i++)
            {
                PropertyInfo propertyToGet = target.GetType().GetProperty(bits[i]);
                object tempTarget = propertyToGet.GetValue(target, null);
                if (tempTarget == null)
                {
                    tempTarget = Activator.CreateInstance(propertyToGet.PropertyType);
                    propertyToGet.SetValue(target, tempTarget);
                }
                target = tempTarget;
            }
            if (target != null)
            {
                PropertyInfo propertyToSet = target.GetType().GetProperty(bits.Last());
                if(propertyToSet != null)
                {
                    if(value != null)
                        value = FormatValue(value, propertyToSet.PropertyType, value.GetType());
                    propertyToSet.SetValue(target, value, null);
                }
            }
        }

        public static PropertyInfo GetCompoundProperty(string compoundProperty, object target, out object value)
        {
            string[] bits = compoundProperty.Split('.');
            PropertyInfo propertyToGet = null;
            value = null;
            for (int i = 0; i < bits.Length; i++)
            {
                propertyToGet = target.GetType().GetProperty(bits[i]);
                if (propertyToGet == null)
                {
                    return null;
                }
                object tempTarget = propertyToGet.GetValue(target, null);
                if (tempTarget == null)
                {
                    value = tempTarget;
                    return null;
                }
                target = tempTarget;
                value = tempTarget;
            }
            return propertyToGet;
        }

        public static Object GetCompoundPropertyValue(string compoundProperty, object target)
        {
            string[] bits = compoundProperty.Split('.');
            PropertyInfo propertyToGet = null;
            for (int i = 0; i < bits.Length - 1; i++)
            {
                propertyToGet = target.GetType().GetProperty(bits[i]);
                object tempTarget = propertyToGet.GetValue(target, null);
                if (tempTarget == null)
                {
                    return null;
                }
                target = tempTarget;
            }
            return target;
        }


        public static string GetPropertyName<T>(Expression<Func<T, object>> expression)
        {
            var body = expression.Body as MemberExpression;

            if (body == null)
            {
                body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }

            return body.Member.Name;
        }

        public static string GetPropertyName<T, TProp>(Expression<Func<T, TProp>> expression)
        {
            var body = expression.Body as MemberExpression;

            if (body == null)
            {
                body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }

            return body.Member.Name;
        }
        //public static string GetPropertyName<T>(Expression<Func<T>> expression)
        //{
        //    return (((MemberExpression)(expression.Body)).Member).Name;
        //}

        public static string GetFullPropertyName<T>(Expression<Func<T, object>> expression)
        {
            var body = expression.Body as MemberExpression;

            if (body == null)
            {
                body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }

            var parameter = expression.Parameters.FirstOrDefault();
            if(parameter != null)
            {
                if (parameter.Type != body.Member.DeclaringType)
                {
                    string remove = string.Format("{0}.", parameter.Name);
                    return body.ToString().TrimStart(remove.ToCharArray());

                    //return string.Format("{0}.{1}", body.Member.DeclaringType.Name, body.Member.Name);
                }
            }
            return body.Member.Name;
        }

        public static string GetClassPropertyName<T>(Expression<Func<T, object>> expression)
        {
            return typeof(T).Name + "." + GetPropertyName<T>(expression);
        }

        public static string GetJsonPropertyName(PropertyInfo prop)
        {
            var attribute = prop.GetCustomAttribute<Newtonsoft.Json.JsonPropertyAttribute>(false);
            if (attribute != null && !string.IsNullOrEmpty(attribute.PropertyName))
                return attribute.PropertyName;
            return prop.Name;
        }

        public static PropertyInfo GetProperty<T>(Expression<Func<T, object>> expression)
        {
            string propName = GetPropertyName<T>(expression);
            PropertyInfo prop = typeof(T).GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
            {
                var body = expression.Body as MemberExpression;
                if (body == null)
                    body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
                if (body.Expression != null)
                    prop = body.Expression.Type.GetProperty(body.Member.Name);
                else
                    return null;
            }
            return prop;
        }

        public static PropertyInfo GetProperty<T, TProp>(Expression<Func<T, TProp>> expression)
        {
            string propName = GetPropertyName<T, TProp>(expression);
            PropertyInfo prop = typeof(T).GetProperty(propName, BindingFlags.Public | BindingFlags.Instance);
            if (prop == null)
            {
                var body = expression.Body as MemberExpression;
                if (body == null)
                    body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
                if (body.Expression != null)
                    prop = body.Expression.Type.GetProperty(body.Member.Name);
                else
                    return null;
            }
            return prop;
        }

        public static MemberInfo GetMemberInfo<T>(Expression<Func<T, object>> expression)
        {
            var body = expression.Body as MemberExpression;

            if (body == null)
            {
                body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }

            return body.Member;
        }

        public static T ToEnum<T>(String val)
        {
            if (!typeof(T).IsEnum)
                throw new Exception("Not enum type");
            return (T)Enum.Parse(typeof(T), val);
        }

        public static string GetEnumDescription<T>(T value)
        {
            Type type = typeof(T);
            return GetEnumDescription(type, value);
        }

        public static string GetEnumDescription(Type type, object value)
        {
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (attr != null)
                    {
                        name = attr.Description;
                    }
                }
            }
            return name;
        }

        public static bool GetEnumBindable(Type type, object value)
        {
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    var attr =
                           Attribute.GetCustomAttribute(field, typeof(ListBindableAttribute)) as ListBindableAttribute;
                    if (attr != null)
                    {
                        return attr.ListBindable;
                    }
                }
            }
            return true;
        }

        public static int GetEnumDisplayOrder(Type type, object value)
        {
            string name = Enum.GetName(type, value);
            int result = 0;
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    var attr =
                           Attribute.GetCustomAttribute(field, typeof(DisplayOrderAttribute)) as DisplayOrderAttribute;
                    if (attr != null)
                    {
                        result = attr.Order;
                    }
                }
            }
            return result;
        }       

        public static List<TProp> GetAllStaticFields<TClass, TProp>()
        {
            return GetAllStaticFields<TProp>(typeof(TClass));
        }

        public static List<TProp> GetAllStaticFields<TProp>(Type type)
        {
            List<TProp> allProp = type
                            .GetFields(BindingFlags.Public | BindingFlags.Static)
                            .Where(f => f.FieldType == typeof(TProp))
                            .Select(f => (TProp)f.GetValue(null))
                            .ToList();
            return allProp;
        }

        public static object CreateInstance(string className)
        {
            if (className.Contains(","))
            {
                Type type = Type.GetType(className);
                return type.Assembly.CreateInstance(type.FullName);
            }
            else
            {
                /*var frame = new StackFrame(1);
                var method = frame.GetMethod();
                var callerAssembly = method.DeclaringType.Assembly;*/
                var callerAssembly = Assembly.GetEntryAssembly();

                if (className.Contains(callerAssembly.GetType().Namespace))
                {
                    return callerAssembly.CreateInstance(className);
                }
                else
                {
                    foreach (Type type in callerAssembly.GetTypes())
                    {
                        if (type.Name == className)
                        {
                            return callerAssembly.CreateInstance(type.FullName);
                        }
                    }
                    return null;
                }
            }
        }

        public static string ToJson(Object obj)
        {
            return new JavaScriptSerializer().Serialize(obj);
        }

        public static void SetDirty<T>(T ori, T mod)
            where T : IChangeMonitored
        {
            //return if modified object is null
            if (mod == null)
            {
                return;
            }
            //set dirty = true and return if original is null and modified is not null
            else if (ori == null)
            {
                mod.IsDirty = true;
                return;
            }

            bool dirty = false;
            Type modelType = mod.GetType();
            var checkProps = modelType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly)
                .Where(x => !x.GetCustomAttributes<SkipDirtyCheckAttribute>().Any());
            var singleProps = checkProps.Where(x => !(x.GetValue(mod) is IList && x.PropertyType.IsGenericType));
            var listProps = checkProps.Where(x => (x.GetValue(mod) is IList && x.PropertyType.IsGenericType));

            foreach (var prop in singleProps)
            {
                if (!prop.CanWrite)
                    continue;
                if (prop.GetCustomAttributes<KeyAttribute>(true).Any())
                    continue;

                var oriValue = prop.GetValue(ori);
                var modValue = prop.GetValue(mod);
                bool propDirty = IsPropDirty(prop, oriValue, modValue);
                if (propDirty)
                {
                    dirty = propDirty;
                    mod.ChangedProperties.Add(new ChangedProperty(prop, oriValue, modValue));
                    //break;
                }
            }
            mod.IsDirty = dirty;
            foreach (var prop in listProps)
            {
                object childOri = prop.GetValue(ori);
                object childMod = prop.GetValue(mod);
                if (childMod != null)
                {
                    if (childMod is IEnumerable)
                    {
                        var modList = childMod as IEnumerable;
                        var oriList = childOri as IEnumerable;
                        int i = 0;
                        foreach (object objMod in modList)
                        {
                            if (objMod == null)
                                continue;
                            object objOri = GetMatch(oriList, objMod, i);
                            SetDirty<IChangeMonitored>((IChangeMonitored)objOri, (IChangeMonitored)objMod);
                            if (((IChangeMonitored)objMod).IsDirty)
                                mod.IsDirty = true;
                            i++;
                        }
                    }
                    else
                    {
                        SetDirty<IChangeMonitored>((IChangeMonitored)childOri, (IChangeMonitored)childMod);
                        if (((IChangeMonitored)childMod).IsDirty)
                            mod.IsDirty = true;
                    }
                }
            }
        }

        private static bool IsPropDirty(PropertyInfo prop, object ori, object mod)
        {
            bool dirty = false;
            if (typeof(IChangeMonitored).IsAssignableFrom(prop.PropertyType))
            {
                if (mod != null)
                {
                    SetDirty<IChangeMonitored>((IChangeMonitored)ori, (IChangeMonitored)mod);
                    dirty = ((IChangeMonitored)mod).IsDirty;
                }
            }
            else
            {
                if (mod != null)
                {
                    dirty = !mod.Equals(ori);
                }
                else
                {
                    dirty = mod != ori;
                }
            }
            return dirty;
        }


        private static object GetMatch(IEnumerable oriList, object objMod, int index)
        {
            if (oriList != null)
            {
                var objectList = oriList.Cast<object>().ToList();
                if (objectList.Count > index)
                    return objectList[index];
            }
            return null;
        }


        #region Test Transform 

        public static List<TModel> MergeBy<TModel, TKey>(this List<TModel> list
            , Func<TModel, TKey> keySelector
            , Func<TModel, IEnumerable<TModel>, TModel> mergeFunction)
        {
            var grouped = list.GroupBy<TModel, TKey>(keySelector).ToList();
            return grouped.Select(g => mergeFunction(g.First(), g)).ToList();
        }

        public static List<TModel> MergeAndOrderBy<TModel, TKey, TOrder>(this List<TModel> list
            , Func<TModel, TKey> keySelector
            , Func<TModel, TOrder> orderSelector
            , Func<TModel, IEnumerable<TModel>, TModel> mergeFunction)
        {
            var grouped = list.GroupBy<TModel, TKey>(keySelector).ToList();
            return grouped.Select(g => mergeFunction(g.OrderBy(orderSelector).First(), g)).ToList();
        }

        public static List<TModel> MergeBy<TModel, TKey, TMerge>(this List<TModel> list
            , Func<TModel, TKey> keySelector, Expression<Func<TModel, TMerge>> mergedColumnSelector
            , Func<IEnumerable<TModel>, TMerge> getMergedValueFunc)
        {
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(typeof(TModel));
            List<TModel> mergedList = (List<TModel>)Activator.CreateInstance(constructedListType);
            var grouped = list.GroupBy<TModel, TKey>(keySelector).ToList();
            grouped.ForEach(g =>
            {
                var obj = g.First();
                PropertyInfo prop = ObjectUtility.GetProperty<TModel, TMerge>(mergedColumnSelector);
                prop.SetValue(obj, getMergedValueFunc(g));
                mergedList.Add(obj);
            });
            return mergedList;
        }

        private static void SetValue(PropertyInfo targetProp, PropertyInfo sourceProp, Object target, object source, bool skipNullProperty)
        {
            if (targetProp != null)
            {
                object sourceValue = sourceProp.GetValue(source, null);

                if (sourceValue != null || !skipNullProperty)
                {
                    sourceValue = FormatValue(sourceValue, targetProp, sourceProp);
                    targetProp.SetValue(target, sourceValue, null);
                }
            }
        }

        private static void SetValue(PropertyInfo targetProp, PropertyInfo sourceProp, Object target, object source, object sourceValue
            , bool skipNullProperty)
        {
            if (targetProp != null)
            {
                if (sourceValue != null || !skipNullProperty)
                {
                    sourceValue = FormatValue(sourceValue, targetProp, sourceProp);
                    targetProp.SetValue(target, sourceValue, null);
                }
            }
        }


        private static object FormatValue(object value, PropertyInfo targetProp, PropertyInfo sourceProp)
        {
            if (sourceProp != null && targetProp != null)
            {
                return FormatValue(value, targetProp.PropertyType, sourceProp.PropertyType);
            }
            else
            {
                return null;
            }
        }

        private static object FormatValue(object value, Type targetPropType, Type sourcePropType)
        {
            if (value == null)
                return null; 
            
            //add custom value

            return value;
        }




        #endregion

    }
}
