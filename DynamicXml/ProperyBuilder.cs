using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace DynamicXml
{
    class ProperyBuilder<T>
    {
        public static Type BuildDynamicTypeWithProperties()
        {
            var targetType = typeof(T);

            AppDomain domain = Thread.GetDomain();
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "DynamicAssembly";

            AssemblyBuilder assemblyBuilder = domain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".dll");

            TypeBuilder typeBuilder = moduleBuilder.DefineType("_" + targetType.Name, TypeAttributes.Public);

            Dictionary<string, string> toStringArguments = new Dictionary<string,string>();


            // STRATEGY TO GET IT GOING FROM HERE :
            // 1. We make a new type from T. This new type (let call it τ) will effectively extract all public fields/properties of T and
            // add them upon τ. Because the XmlSerializer already considers only public fields and properties with Getter and Setter, no
            // information is lost in the process.
            // 2. All members aforementioned are kept as is, except with those decorated by the ToXmlString Attribute. This attribute is here
            // to cast the field to a string. For a example if we have :
            //  [XmlToString("yyyy-MM")]
            //  DateTime MyCustomeDate { get; set; }
            // we want to make sure that <MyCustomDate>...</MyCustomDate> will be outputted using the attribute arguments.
            // So τ shall redefine the type of members decorated as string.
            // 3. This means that when a T instance is cast to τ, then we must check members with XmlToStringAttribute and cast them 
            // at this moment. 
            // (4.) We  would also like to keep all other attributes defined in the class. For what I can tell, this can only be done 
            // using something like : 
            // fieldBuilder.SetCustomAttribute(attributeData.Constructor, new byte[] { 0x01, 0x00, 0x07, ...
            // and define the correct byte array that matches the existing attribute.. Easy enough, but for now, let's concentrate
            // on the points described above.
            // 5. Also, all of this should be wrapped in a neat API hiding the custom serializer, custom classes, custom everything. 
            // All of it should stay seamless.

            foreach (var targetProperty in targetType.GetProperties())
            {
                Type newPropType = targetProperty.PropertyType;
                var propAttributes = targetProperty.Attributes;
                //var toStringAttribute = targetProperty.GetCustomAttributes(typeof(XmlToStringAttribute), true);
                var attributes = targetProperty.GetCustomAttributes(true);

                foreach (var attribute in attributes.Where(attr => attr as XmlToStringAttribute != null))
                {
                    if (attribute as XmlToStringAttribute != null)
                    {
                        var toStringArgument = ((XmlToStringAttribute)attribute).Argument;
                        toStringArguments.Add(targetProperty.Name, toStringArgument);
                        newPropType = typeof(string);
                        continue;
                    }

                    //targetProperty.GetCustomAttributesData()
                    ////attributeData = new CustomAttributeData()
                }

                //List<CustomAttributeData> attributeDatas = new List<CustomAttributeData>();
                //foreach (var attributeData in targetProperty.GetCustomAttributesData())
                //{
                //    attributeData.
                //} 
                //TBC..


                FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + targetProperty.Name, newPropType, FieldAttributes.Public);

                foreach (var attributeData in targetProperty.GetCustomAttributesData())
                {
                    //attributeData.
                    fieldBuilder.SetCustomAttribute(attributeData.Constructor, new byte[] { 0x01, 0x00, 0x07, 0x42, 0x6F, 0x6E, 0x6A, 0x6F, 0x75, 0x72, 0x00, 0x00 });
                }
            }
            

            Type returnType = typeBuilder.CreateType();
            return returnType;
        }
    }
}