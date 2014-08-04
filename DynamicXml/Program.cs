using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Xml.Serialization;

namespace DynamicXml
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var thierry = new Person();
            Type t = ProperyBuilder<Person>.BuildDynamicTypeWithProperties();

            var tSerializer = new XmlSerializer(t);
        }
    }

    public class Person
    {
        [XmlElement("ageGoesHere")]
        public double Age { get; set; }

        [XmlElement("heightGoesThere")]
        [XmlToString("N2")]
        public double Height { get; set; }

        public string Name { get; set; }

        [XmlToString("yyyy-MM")]
        public DateTime Date { get; set; }

        public Person()
        {
            Age = 23 + Math.PI;
            Height = 5 + 10.0 / 12.0;
            Name = "Thierry";
        }
    }

    public class XmlToStringAttribute : Attribute
    {
        public string Argument { get; set; }

        public XmlToStringAttribute(string argument)
        {
            Argument = argument;
        }
    }
}