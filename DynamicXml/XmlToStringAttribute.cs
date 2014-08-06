using System;

namespace DynamicXml
{
	public class XmlToStringAttribute : Attribute
	{
		public string Argument { get; set; }

		public XmlToStringAttribute(string argument)
		{
			Argument = argument;
		}
	}
}

