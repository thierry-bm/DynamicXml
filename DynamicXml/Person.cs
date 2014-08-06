using System;
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
}

