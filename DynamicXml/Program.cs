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
            var dSerializer = new DynamicXmlSerializer<Person>();

//            var thierry = new Person();
//            Type t = ProperyBuilder<Person>.BuildDynamicTypeWithProperties();
//
//            var tSerializer = new XmlSerializer(t);
        }
    }
}