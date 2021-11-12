using Bing.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Heavens.Core.Extentions;
public static class Extention
{
    /// <summary>
    /// 给现有对象属性赋值
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="resource"></param>
    /// <param name="includes"></param>
    /// <param name="excludes"></param>
    /// <param name="override"></param>
    public static void SetPropValue(object obj, object resource, IEnumerable<string> includes = null, IEnumerable<string> excludes = null, bool @override = false)
    {
        PropertyInfo[] objProps = obj.GetType().GetProperties();
        PropertyInfo[] resProps = resource.GetType().GetProperties();

        if (!includes.IsEmpty())
        {
            objProps = objProps.Where(o => includes.Contains(o.Name)).ToArray();
        }

        if (!excludes.IsEmpty())
        {
            objProps.RemoveAll(o => excludes.Contains(o.Name)).ToArray();
        }

        foreach (PropertyInfo reProp in resProps)
        {

            if (!@override)
            {
                Type type = reProp.GetType();
                object defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;
                if (reProp.GetValue(obj) != defaultValue)
                {
                    continue;
                }
            }

            PropertyInfo setProp = objProps.FirstOrDefault(o => o.Name == reProp.Name && o.PropertyType == reProp.PropertyType);
            if (!setProp.IsNull())
            {
                setProp.SetValue(obj, reProp.GetValue(resource));
            }
        }
    }
}
