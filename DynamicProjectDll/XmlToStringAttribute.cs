using System;

namespace DynamicXml
{
    // TODO pass parameters as array (Culture can be declared)
    public class XmlToStringAttribute : Attribute
    {
        public string Argument { get; set; }

        public XmlToStringAttribute(string argument)
        {
            Argument = argument;
        }
    }
}