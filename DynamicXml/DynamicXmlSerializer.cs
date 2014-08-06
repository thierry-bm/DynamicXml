using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using System.IO;

namespace DynamicXml
{
	public class DynamicXmlSerializer<T> where T : new()
    {
        private static AssemblyBuilder AssemblyBuilder { get; set; }
        private static ModuleBuilder ModuleBuilder { get; set; }

        private Dictionary<string, string> ToStringArguments { get; set; }
        private Type DerivedType { get; set; }
        public XmlSerializer StaticSerializer { get; set; }

        static DynamicXmlSerializer()
        {
            AppDomain domain = Thread.GetDomain();
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "DynamicAssembly";

            AssemblyBuilder = domain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder = AssemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName + ".dll");
        }

        public DynamicXmlSerializer()
        {
            ToStringArguments = new Dictionary<string, string>();

            var targetType = typeof(T);
            TypeBuilder typeBuilder = ModuleBuilder.DefineType("_" + targetType.Name, TypeAttributes.Public); // FIXME Ici si on instancie plusieurs fois avec le meme type on est foutu.

            foreach (var targetProperty in targetType.GetProperties())
            {
                Type newFieldType = targetProperty.PropertyType;
                string newFieldName = targetProperty.Name;
                
                var attributesList = targetProperty.GetCustomAttributes(true).ToList();
                if (attributesList.Exists(attribute => attribute as XmlToStringAttribute != null))
                {
                    var xmlToStringAttribute = (XmlToStringAttribute)attributesList.First(attribute => attribute as XmlToStringAttribute != null);
                    ToStringArguments[newFieldName] = xmlToStringAttribute.Argument;
                    newFieldType = typeof(string);
                }

                typeBuilder.DefineField(newFieldName, newFieldType, FieldAttributes.Public);
            }

            DerivedType = typeBuilder.CreateType();
            StaticSerializer = new XmlSerializer(DerivedType);
        }

		public void Serialize(TextWriter textWriter, T t)
		{
			var derivedInstance = Activator.CreateInstance(DerivedType); //Hence the need of where T: new()
		}
    }
}
