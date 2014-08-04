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

            //List<FieldBuilder> fieldBuilders = new List<FieldBuilder>();
            //List<PropertyBuilder> propertyBuilders = new List<PropertyBuilder>();
            Dictionary<string, string> toStringArguments = new Dictionary<string,string>();
            //foreach (var targetField in targetType.GetFields())
            //{
                
            //    fieldBuilders.Add(fieldBuilder);
            //}

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

        public static object CreateInstance<U>(Type t, U from)
        {
            var instance = Activator.CreateInstance(t);

            return instance;
        }
    }
}

//PropertyBuilder propBuilder = typeBuilder.DefineProperty(targetProperty.Name, targetProperty.Attributes, newPropType, null);

//MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

//MethodBuilder getAccessorBuilder = typeBuilder.DefineMethod("get_" + targetProperty.Name, getSetAttr, newPropType, Type.EmptyTypes);

//ILGenerator getILGenerator = getAccessorBuilder.GetILGenerator();
//getILGenerator.Emit(OpCodes.Ldarg_0);
//getILGenerator.Emit(OpCodes.Ldfld, fieldBuilder);
//getILGenerator.Emit(OpCodes.Ret);

//MethodBuilder setAccessorBuilder = typeBuilder.DefineMethod("set_" + targetProperty.Name, getSetAttr, null, new Type[] { newPropType });

//ILGenerator setILGenerator = setAccessorBuilder.GetILGenerator();
//setILGenerator.Emit(OpCodes.Ldarg_0);
//setILGenerator.Emit(OpCodes.Ldarg_1);
//setILGenerator.Emit(OpCodes.Stfld, fieldBuilder);
//setILGenerator.Emit(OpCodes.Ret);

//propBuilder.SetGetMethod(getAccessorBuilder);
//propBuilder.SetSetMethod(setAccessorBuilder);
