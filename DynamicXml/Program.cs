using System;
using System.Globalization;
using System.Threading;

namespace DynamicXml
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            
            var dSerializer = new DynamicXmlSerializer<Person>();
            var dSerializer2 = new DynamicXmlSerializer<Person>();

            var person = new Person();

            dSerializer.Serialize(Console.Out, person);
            dSerializer2.Serialize(Console.Out, person);
        }
    }
}