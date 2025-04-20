using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using BGICompiler.Compiler.Logger;

namespace MOD.Scripts.Core.MODXMLWrapper
{
	internal class MODXMLWrapper
	{
		static public void Serialize<T>(string outputPath, T objectToSerialize)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			TextWriter writer = new StreamWriter(outputPath);
			serializer.Serialize(writer, objectToSerialize);
			writer.Close();
		}

		static private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
		{
			Debug.Log("MODXMLWrapper - Unknown Node:" + e.Name + "\t" + e.Text);
		}

		static private void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
		{
			Debug.Log("MODXMLWrapper - Unknown attribute " + e.Attr.Name + "='" + e.Attr.Value + "'");
		}

		static public T Deserialize<T>(string inputPath)
		{
			XmlSerializer serializer = new XmlSerializer(typeof(T));

			serializer.UnknownNode += new
			XmlNodeEventHandler(serializer_UnknownNode);
			serializer.UnknownAttribute += new
			XmlAttributeEventHandler(serializer_UnknownAttribute);

			FileStream fs = new FileStream(inputPath, FileMode.Open);

			return (T)serializer.Deserialize(fs);
		}
	}
}
