using System;

namespace McgTgBotNet.Attributes
{

    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class XMLPropertyAttribute : Attribute
    {
        public string XMLName { get; set; }
        public XMLPropertyAttribute(string xmlName) => XMLName = xmlName;
    }
}
