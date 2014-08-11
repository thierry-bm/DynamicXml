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
        private static Dictionary<Type, Dictionary<string, string>> _arguments { get; set; }
        private static List<Type> ExistingTypes
        {
            get { return ModuleBuilder.GetTypes().ToList(); }
        }

        private Type _derivedType { get; set; }
        private XmlSerializer _staticSerializer { get; set; }

        static DynamicXmlSerializer()
        {
            AppDomain domain = Thread.GetDomain();
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "DynamicAssembly";

            AssemblyBuilder = domain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder = AssemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName + ".dll");
            _arguments = new Dictionary<Type, Dictionary<string, string>>();
        }

        public DynamicXmlSerializer()
        {
            _derivedType = DerivedType(typeof(T));
            _staticSerializer = new XmlSerializer(_derivedType);
        }

        public Type DerivedType(Type t)
        {
            if (IsBuiltinType(t))
                return t;

            var hypotheticallyExistingType = ExistingTypes.FirstOrDefault(type => type.Name == t.Name);
            if (hypotheticallyExistingType != null)
                return hypotheticallyExistingType;

            var iEnumerableInterface = t.GetInterfaces().FirstOrDefault(i => i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (iEnumerableInterface != null)
            {
                var listedT = iEnumerableInterface.GetGenericArguments()[0];
                var derivedListedT = DerivedType(listedT);
                var listResult = typeof(List<>).MakeGenericType(new[] { derivedListedT }); // Just like properties are turned to fields, any IEnumerable will be cast to List<>
                return listResult;
            }

            TypeBuilder typeBuilder = ModuleBuilder.DefineType(t.Name, TypeAttributes.Public);
            var toStringArguments = new Dictionary<string, string>();

            foreach (var targetProperty in t.GetProperties())
            {
                Type newFieldType = targetProperty.PropertyType;
                string newFieldName = targetProperty.Name;

                var attributesList = targetProperty.GetCustomAttributes(true).ToList();
                if (attributesList.Exists(attribute => attribute as XmlToStringAttribute != null))
                {
                    var xmlToStringAttribute = (XmlToStringAttribute)attributesList.First(attribute => attribute as XmlToStringAttribute != null);
                    toStringArguments[newFieldName] = xmlToStringAttribute.Argument;
                    newFieldType = typeof(string);
                }

                typeBuilder.DefineField(newFieldName, DerivedType(newFieldType), FieldAttributes.Public);
            }

            Type result = typeBuilder.CreateType();
            _arguments[result] = toStringArguments;

            return result;
        }

        public void Serialize(TextWriter textWriter, T instance)
        {
            object derivedInstance = CreateDerivedInstance(instance, _derivedType, _arguments[_derivedType]);
            _staticSerializer.Serialize(textWriter, derivedInstance);
        }

        // TODO Please cleanup a bit. This is a serious mess.
        private object CreateDerivedInstance(object instance, Type derivedType, Dictionary<string, string> toStringArguments)
        {
            if (IsBuiltinType(derivedType))
                return instance;

            var result = Activator.CreateInstance(derivedType);
            
            foreach (var derivedField in derivedType.GetFields())
            {
                var fieldName = derivedField.Name;
                var fieldType = derivedField.FieldType;
                dynamic subInstance = instance.GetType().GetProperty(fieldName).GetValue(instance, null);

                if (!toStringArguments.ContainsKey(fieldName))
                {
                    if (IsBuiltinType(fieldType))
                    {
                        derivedField.SetValue(result, subInstance);
                    }
                    else
                    {
                        object derivedSubInstance = CreateDerivedInstance(subInstance, fieldType, _arguments[fieldType]);
                        derivedField.SetValue(result, derivedSubInstance);
                    }
                }
                else
                {
                    var toStringArgument = toStringArguments[fieldName];
                    derivedField.SetValue(result, subInstance.ToString(toStringArgument));
                }                
            }

            return result;
        }

        private static bool IsBuiltinType(Type t)
        {
            return (t == typeof(object) || Type.GetTypeCode(t) != TypeCode.Object);
        }
    }
}