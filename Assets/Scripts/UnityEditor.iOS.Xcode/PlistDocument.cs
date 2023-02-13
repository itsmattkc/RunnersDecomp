using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace UnityEditor.iOS.Xcode
{
	public class PlistDocument
	{
		public PlistElementDict root;

		public string version;

		public PlistDocument()
		{
			root = new PlistElementDict();
			version = "1.0";
		}

		internal static XDocument ParseXmlNoDtd(string text)
		{
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.ProhibitDtd = false;
			xmlReaderSettings.XmlResolver = null;
			XmlReader reader = XmlReader.Create(new StringReader(text), xmlReaderSettings);
			return XDocument.Load(reader);
		}

		internal static string CleanDtdToString(XDocument doc)
		{
			if (doc.DocumentType != null)
			{
				XDocument xDocument = new XDocument(new XDeclaration("1.0", "utf-8", null), new XDocumentType(doc.DocumentType.Name, doc.DocumentType.PublicId, doc.DocumentType.SystemId, null), new XElement(doc.Root.Name));
				return string.Concat(string.Empty, xDocument.Declaration, Environment.NewLine, xDocument.DocumentType, Environment.NewLine, doc.Root);
			}
			XDocument xDocument2 = new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement(doc.Root.Name));
			return string.Concat(string.Empty, xDocument2.Declaration, Environment.NewLine, doc.Root);
		}

		private static string GetText(XElement xml)
		{
			return string.Join(string.Empty, (from x in xml.Nodes().OfType<XText>()
				select x.Value).ToArray());
		}

		private static PlistElement ReadElement(XElement xml)
		{
			switch (xml.Name.LocalName)
			{
			case "dict":
			{
				List<XElement> list2 = xml.Elements().ToList();
				PlistElementDict plistElementDict = new PlistElementDict();
				if (list2.Count % 2 == 1)
				{
					throw new Exception("Malformed plist file");
				}
				for (int i = 0; i < list2.Count - 1; i++)
				{
					if (list2[i].Name != "key")
					{
						throw new Exception("Malformed plist file");
					}
					string key = GetText(list2[i]).Trim();
					PlistElement plistElement2 = ReadElement(list2[i + 1]);
					if (plistElement2 != null)
					{
						i++;
						plistElementDict[key] = plistElement2;
					}
				}
				return plistElementDict;
			}
			case "array":
			{
				List<XElement> list = xml.Elements().ToList();
				PlistElementArray plistElementArray = new PlistElementArray();
				{
					foreach (XElement item in list)
					{
						PlistElement plistElement = ReadElement(item);
						if (plistElement != null)
						{
							plistElementArray.values.Add(plistElement);
						}
					}
					return plistElementArray;
				}
			}
			case "string":
				return new PlistElementString(GetText(xml));
			case "integer":
			{
				int result;
				if (int.TryParse(GetText(xml), out result))
				{
					return new PlistElementInteger(result);
				}
				return null;
			}
			case "true":
				return new PlistElementBoolean(true);
			case "false":
				return new PlistElementBoolean(false);
			default:
				return null;
			}
		}

		public void ReadFromFile(string path)
		{
			ReadFromString(File.ReadAllText(path));
		}

		public void ReadFromStream(TextReader tr)
		{
			ReadFromString(tr.ReadToEnd());
		}

		public void ReadFromString(string text)
		{
			XDocument xDocument = ParseXmlNoDtd(text);
			version = (string)xDocument.Root.Attribute("version");
			XElement xml = xDocument.XPathSelectElement("plist/dict");
			PlistElement plistElement = ReadElement(xml);
			if (plistElement == null)
			{
				throw new Exception("Error parsing plist file");
			}
			root = (plistElement as PlistElementDict);
			if (root == null)
			{
				throw new Exception("Malformed plist file");
			}
		}

		private static XElement WriteElement(PlistElement el)
		{
			if (el is PlistElementBoolean)
			{
				PlistElementBoolean plistElementBoolean = el as PlistElementBoolean;
				return new XElement((!plistElementBoolean.value) ? "false" : "true");
			}
			if (el is PlistElementInteger)
			{
				PlistElementInteger plistElementInteger = el as PlistElementInteger;
				return new XElement("integer", plistElementInteger.value.ToString());
			}
			if (el is PlistElementString)
			{
				PlistElementString plistElementString = el as PlistElementString;
				return new XElement("string", plistElementString.value);
			}
			if (el is PlistElementDict)
			{
				PlistElementDict plistElementDict = el as PlistElementDict;
				XElement xElement = new XElement("dict");
				{
					foreach (KeyValuePair<string, PlistElement> value in plistElementDict.values)
					{
						XElement content = new XElement("key", value.Key);
						XElement xElement2 = WriteElement(value.Value);
						if (xElement2 != null)
						{
							xElement.Add(content);
							xElement.Add(xElement2);
						}
					}
					return xElement;
				}
			}
			if (el is PlistElementArray)
			{
				PlistElementArray plistElementArray = el as PlistElementArray;
				XElement xElement3 = new XElement("array");
				{
					foreach (PlistElement value2 in plistElementArray.values)
					{
						XElement xElement4 = WriteElement(value2);
						if (xElement4 != null)
						{
							xElement3.Add(xElement4);
						}
					}
					return xElement3;
				}
			}
			return null;
		}

		public void WriteToFile(string path)
		{
			File.WriteAllText(path, WriteToString());
		}

		public void WriteToStream(TextWriter tw)
		{
			tw.Write(WriteToString());
		}

		public string WriteToString()
		{
			XElement content = WriteElement(root);
			XElement xElement = new XElement("plist");
			xElement.Add(new XAttribute("version", version));
			xElement.Add(content);
			XDocument xDocument = new XDocument();
			xDocument.Add(xElement);
			return CleanDtdToString(xDocument);
		}
	}
}
