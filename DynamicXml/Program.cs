using System;

namespace DynamicXml
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var dSerializer = new DynamicXmlSerializer<Person>();
            var dSerializer2 = new DynamicXmlSerializer<Person>();

            var person = new Person();

            dSerializer.Serialize(Console.Out, person);
            dSerializer2.Serialize(Console.Out, person);
        }
    }
}