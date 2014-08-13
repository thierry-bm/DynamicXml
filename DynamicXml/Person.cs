using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace DynamicXml
{
    public class Person
    {
        [XmlElement("ageGoesHere")]
        public double Age { get; set; }

        [XmlElement("heightGoesThere")]
        [XmlToString("N2")]
        public double Height { get; set; }

        [XmlAttribute]
        public string Name { get; set; }

        [XmlToString("yyyy-MM")]
        public DateTime Date { get; set; }

        public SubObject MySubObject { get; set; }

        public List<SubObject> MySubObjects { get; set; }

        public Person()
        {
            Age = 23 + Math.PI;
            Height = 5 + 10.0 / 12.0;
            Name = "Thierry";
            Date = DateTime.Now;
            MySubObject = new SubObject();
            MySubObjects = new List<SubObject>();

            foreach (var _ in Enumerable.Range(0, 3))
            {
                MySubObjects.Add(new SubObject());
            }
        }
    }

    public class SubObject
    {
        [XmlToString("N4")]
        public double A { get; set; }

        public int B { get; set; }

        [XmlToString("yyyy-MM")]
        public DateTime C { get; set; }

        public SubObject()
        {
            A = 56.156465451;
            B = 8;
            C = new DateTime(2001, 09, 11);
        }
    }
}

