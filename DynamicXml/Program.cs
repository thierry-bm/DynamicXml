using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace DynamicXml
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var dSerializer = new DynamicXmlSerializer<Person>();
			var person = new Person();

			dSerializer.Serialize(Console.Out, person);

//            var thierry = new Person();
//            Type t = ProperyBuilder<Person>.BuildDynamicTypeWithProperties();
//
//            var tSerializer = new XmlSerializer(t);
        }
    }
}