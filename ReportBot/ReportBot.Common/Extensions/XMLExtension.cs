using McgTgBotNet.Attributes;
using System;
using System.Xml.Linq;

namespace McgTgBotNet.Extensions
{
    public static class XMLExtension
    {
        public static T ParseXML<T>(this XElement element)
            where T : new()
        {
            T obj = new T();

            Type type = typeof(T);
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var attribute = (XMLPropertyAttribute)Attribute.GetCustomAttribute(property, typeof(XMLPropertyAttribute));
                if (attribute != null)
                {
                    var propertyElement = element.Element(attribute.XMLName);
                    if (propertyElement != null)
                    {
                        var value = Convert.ChangeType(propertyElement.Value, property.PropertyType);
                        property.SetValue(obj, value);
                    }
                }
            }

            return obj;
        }
    }
}
