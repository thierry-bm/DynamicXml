using System;

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