using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Xml.Serialization;

namespace DynamicXml
{
    public class DynamicXmlSerializer<T> where T : new()
    {
        private static AssemblyBuilder AssemblyBuilder { get; set; }
        private static ModuleBuilder ModuleBuilder { get; set; }

        private Dictionary<string, string> _toStringArguments { get; set; }
        private Type _derivedType { get; set; }
        private XmlSerializer _staticSerializer { get; set; }

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
            _toStringArguments = new Dictionary<string, string>();

            var targetType = typeof(T);
            TypeBuilder typeBuilder = ModuleBuilder.DefineType(targetType.Name, TypeAttributes.Public); // FIXME Ici si on instancie plusieurs fois avec le meme type on est foutu.

            foreach (var targetProperty in targetType.GetProperties())
            {
                Type newFieldType = targetProperty.PropertyType;
                string newFieldName = targetProperty.Name;

                var attributesList = targetProperty.GetCustomAttributes(true).ToList();
                if (attributesList.Exists(attribute => attribute as XmlToStringAttribute != null))
                {
                    var xmlToStringAttribute = (XmlToStringAttribute)attributesList.First(attribute => attribute as XmlToStringAttribute != null);
                    _toStringArguments[newFieldName] = xmlToStringAttribute.Argument;
                    newFieldType = typeof(string);
                }

                typeBuilder.DefineField(newFieldName, newFieldType, FieldAttributes.Public);
            }

            _derivedType = typeBuilder.CreateType();
            _staticSerializer = new XmlSerializer(_derivedType);
        }

        public void Serialize(TextWriter textWriter, T t)
        {
            var derivedInstance = Activator.CreateInstance(_derivedType); //Hence the need of where T: new()

            foreach (var derivedField in _derivedType.GetFields())
            {
                var fieldName = derivedField.Name;
                dynamic primaryValue = typeof(T).GetProperty(fieldName).GetValue(t, null);

                if (!_toStringArguments.ContainsKey(fieldName))
                {
                    derivedField.SetValue(derivedInstance, primaryValue);
                }
                else
                {
                    var toStringArgument = _toStringArguments[fieldName];
                    derivedField.SetValue(derivedInstance, primaryValue.ToString(toStringArgument));
                }
            }

            _staticSerializer.Serialize(textWriter, derivedInstance);
        }

        private static bool IsBuiltinType(Type t)
        {
            return (t == typeof(object) || Type.GetTypeCode(t) != TypeCode.Object);
        }
    }
}