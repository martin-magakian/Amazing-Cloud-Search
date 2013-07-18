using System.Collections.Generic;
using System.Reflection;

namespace AmazingCloudSearch.Helper
{
    internal class ListProperties<T>
    {
        List<PropertyInfo> _properties;

        public static object _propertyLock = new object();

        public List<PropertyInfo> GetProperties()
        {
            if (_properties != null)
            {
                return _properties;
            }

            lock (_propertyLock)
            {
                if (_properties != null)
                {
                    return _properties;
                }

                _properties = List();

                return _properties;
            }
        }

        public List<PropertyInfo> List()
        {
            _properties = new List<PropertyInfo>();

            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in properties)
            {
                if (!p.CanWrite || !p.CanRead)
                {
                    continue;
                }

                if (!AreGetterAndSetterBothPublic(p))
                {
                    continue;
                }

                _properties.Add(p);
            }
            return _properties;
        }

        bool AreGetterAndSetterBothPublic(PropertyInfo prop)
        {
            if (prop.GetGetMethod(false) == null || prop.GetSetMethod(false) == null)
            {
                return false;
            }

            return true;
        }
    }
}
