using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using UnityEngine;

public static class DocGen
{
	public const BindingFlags Flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

	public static List<string> TypeBlacklist = new List<string> { "<PrivateImplementationDetails>", "<PrivateImplementationDetails>+__StaticArrayInitTypeSize=20" };

	public static XDocument Doc = new XDocument(new XElement("Root"));

	public static void Go()
	{
		Type[] types = typeof(CardData).Assembly.GetTypes();
		foreach (Type type in types)
		{
			if (!TypeBlacklist.Contains(type.ToString()) && type.Namespace == null)
			{
				if (type.IsEnum)
				{
					DoEnum(type);
				}
				else if ((type.IsClass || type.IsValueType || type.IsInterface) && !type.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false))
				{
					DoClass(type);
				}
			}
		}
		File.WriteAllText("F:/doctest/doc.xml", Doc.ToString());
		Debug.Log("Done!");
	}

	public static void DoEnum(Type type)
	{
		XElement xElement = new XElement("Enum", new XAttribute("Name", type));
		foreach (int value in Enum.GetValues(type))
		{
			xElement.Add(new XElement("Value", new XAttribute("Name", Enum.GetName(type, value)), new XAttribute("Value", value), new XElement("Description", "")));
		}
		xElement.Add(new XElement("Description", ""));
		Doc.Root.Add(xElement);
		File.WriteAllText("F:/doctest/source/_doc/enums/" + type.Name + ".xml", xElement.ToString());
	}

	public static void DoClass(Type type)
	{
		Type type2 = type;
		List<Type> list = new List<Type>();
		while (type2 != typeof(MonoBehaviour) && type2.BaseType != null)
		{
			type2 = type2.BaseType;
			if (type2 != typeof(object))
			{
				list.Add(type2);
			}
		}
		XElement xElement = new XElement("Class", new XAttribute("Name", type));
		if (type.IsValueType)
		{
			xElement.Add(new XAttribute("IsStruct", true));
		}
		if (type.IsInterface)
		{
			xElement.Add(new XAttribute("IsInterface", true));
		}
		if (type.IsAbstract)
		{
			xElement.Add(new XAttribute("IsAbstract", true));
		}
		if (type.IsSealed)
		{
			xElement.Add(new XAttribute("IsSealed", true));
		}
		foreach (Type item in list)
		{
			xElement.Add(new XElement("Inherits", new XAttribute("Name", item)));
		}
		foreach (Type implementedInterface in type.GetTypeInfo().ImplementedInterfaces)
		{
			xElement.Add(new XElement("Implements", new XAttribute("Name", implementedInterface)));
		}
		List<MethodInfo> list2 = new List<MethodInfo>();
		XElement xElement2 = new XElement("Properties");
		PropertyInfo[] properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (PropertyInfo propertyInfo in properties)
		{
			list2.AddRange(propertyInfo.GetAccessors(nonPublic: true));
			XElement xElement3 = new XElement("Property", new XAttribute("Name", propertyInfo.Name), new XAttribute("Type", propertyInfo.PropertyType));
			MethodInfo getMethod = propertyInfo.GetGetMethod(nonPublic: true);
			MethodInfo setMethod = propertyInfo.GetSetMethod(nonPublic: true);
			if (getMethod != null)
			{
				if (getMethod.IsAbstract && !getMethod.IsFinal)
				{
					xElement3.Add(new XAttribute("IsAbstract", true));
				}
				if (getMethod.IsVirtual && !getMethod.IsFinal)
				{
					xElement3.Add(new XAttribute("IsVirtual", true));
				}
				if (getMethod.IsStatic)
				{
					xElement3.Add(new XAttribute("IsStatic", true));
				}
				if (getMethod.GetBaseDefinition() != getMethod)
				{
					xElement3.Add(new XAttribute("Override", getMethod.GetBaseDefinition().DeclaringType));
				}
				xElement3.Add(new XAttribute("Access", GetAccess(getMethod)));
				xElement3.Add(new XAttribute("Accessors", (setMethod != null) ? "get; set" : "get"));
			}
			else
			{
				if (setMethod.IsAbstract && !setMethod.IsFinal)
				{
					xElement3.Add(new XAttribute("IsAbstract", true));
				}
				if (setMethod.IsVirtual && !setMethod.IsFinal)
				{
					xElement3.Add(new XAttribute("IsVirtual", true));
				}
				if (setMethod.IsStatic)
				{
					xElement3.Add(new XAttribute("IsStatic", true));
				}
				if (setMethod.GetBaseDefinition() != setMethod)
				{
					xElement3.Add(new XAttribute("Override", setMethod.GetBaseDefinition().DeclaringType));
				}
				xElement3.Add(new XAttribute("Access", GetAccess(setMethod)));
				xElement3.Add(new XAttribute("Accessors", "set"));
			}
			xElement3.Add(new XElement("Description", ""));
			xElement2.Add(xElement3);
		}
		xElement.Add(xElement2);
		XElement xElement4 = new XElement("Fields");
		FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!fieldInfo.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false))
			{
				XElement xElement5 = new XElement("Field", new XAttribute("Name", fieldInfo.Name), new XAttribute("Type", fieldInfo.FieldType), new XAttribute("Access", GetAccess(fieldInfo.FieldType)));
				if (fieldInfo.IsStatic)
				{
					xElement5.Add(new XAttribute("IsStatic", true));
				}
				xElement5.Add(new XElement("Description", ""));
				xElement4.Add(xElement5);
			}
		}
		xElement.Add(xElement4);
		XElement xElement6 = new XElement("Methods");
		MethodInfo[] methods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (MethodInfo methodInfo in methods)
		{
			if (methodInfo.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false) || list2.Contains(methodInfo))
			{
				continue;
			}
			XElement xElement7 = new XElement("Method", new XAttribute("Name", methodInfo.Name), new XAttribute("Type", methodInfo.ReturnType), new XAttribute("Access", GetAccess(methodInfo)));
			if (methodInfo.IsAbstract && !methodInfo.IsFinal)
			{
				xElement7.Add(new XAttribute("IsAbstract", true));
			}
			if (methodInfo.IsVirtual && !methodInfo.IsFinal)
			{
				xElement7.Add(new XAttribute("IsVirtual", true));
			}
			if (methodInfo.IsStatic)
			{
				xElement7.Add(new XAttribute("IsStatic", true));
			}
			if (methodInfo.GetBaseDefinition() != methodInfo)
			{
				xElement7.Add(new XAttribute("Override", methodInfo.GetBaseDefinition().DeclaringType));
			}
			if (methodInfo.IsGenericMethod)
			{
				xElement7.Add(new XAttribute("IsGeneric", true));
				XElement xElement8 = new XElement("GenericArguments");
				Type[] genericArguments = methodInfo.GetGenericArguments();
				foreach (Type value in genericArguments)
				{
					xElement8.Add(new XElement("Argument", new XAttribute("Type", value)));
				}
				xElement7.Add(xElement8);
			}
			XElement xElement9 = new XElement("Parameters");
			ParameterInfo[] parameters = methodInfo.GetParameters();
			foreach (ParameterInfo parameterInfo in parameters)
			{
				XElement xElement10 = new XElement("Parameter", new XAttribute("Name", parameterInfo.Name), new XAttribute("Type", parameterInfo.ParameterType));
				if (parameterInfo.HasDefaultValue)
				{
					xElement10.Add(new XAttribute("DefaultValue", (parameterInfo.DefaultValue == null) ? "null" : parameterInfo.DefaultValue.ToString()));
				}
				if (parameterInfo.IsOptional)
				{
					xElement10.Add(new XAttribute("IsOptional", true));
				}
				xElement9.Add(xElement10);
			}
			xElement7.Add(xElement9);
			xElement7.Add(new XElement("Description", ""));
			xElement6.Add(xElement7);
		}
		xElement.Add(xElement6);
		XElement xElement11 = new XElement("Events");
		EventInfo[] events = type.GetEvents(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (EventInfo eventInfo in events)
		{
			if (!eventInfo.IsDefined(typeof(CompilerGeneratedAttribute), inherit: false))
			{
				XElement xElement12 = new XElement("Event", new XAttribute("Name", eventInfo.Name), new XAttribute("Type", eventInfo.EventHandlerType), new XAttribute("Access", GetAccess(eventInfo.EventHandlerType)));
				xElement12.Add(new XElement("Description", ""));
				xElement11.Add(xElement12);
			}
		}
		xElement.Add(xElement11);
		xElement.Add(new XElement("Description", ""));
		Doc.Root.Add(xElement);
		File.WriteAllText("F:/doctest/source/_doc/classes/" + type.Name.Replace('[', '(').Replace(']', ')') + ".xml", xElement.ToString());
	}

	public static string GetAccess(MethodInfo method)
	{
		if (method.IsAssembly)
		{
			if (!method.IsFamily)
			{
				return "internal";
			}
			return "internal protected";
		}
		if (method.IsPublic)
		{
			return "public";
		}
		if (method.IsPrivate)
		{
			return "private";
		}
		if (method.IsFamily)
		{
			return "protected";
		}
		return "public";
	}

	public static string GetAccess(Type type)
	{
		if (type.IsPublic || type.IsNestedPublic)
		{
			return "public";
		}
		if (type.IsNestedFamORAssem)
		{
			return "protected internal";
		}
		if (type.IsNestedAssembly)
		{
			return "internal";
		}
		if (type.IsNestedFamily)
		{
			return "protected";
		}
		if (type.IsNestedFamANDAssem)
		{
			return "private protected";
		}
		return "private";
	}
}
