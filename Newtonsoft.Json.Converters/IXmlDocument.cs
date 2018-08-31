namespace Newtonsoft.Json.Converters
{
	internal interface IXmlDocument : IXmlNode
	{
		IXmlElement DocumentElement
		{
			get;
		}

		IXmlNode CreateComment(string text);

		IXmlNode CreateTextNode(string text);

		IXmlNode CreateCDataSection(string data);

		IXmlNode CreateWhitespace(string text);

		IXmlNode CreateSignificantWhitespace(string text);

		IXmlNode CreateXmlDeclaration(string version, string encoding, string standalone);

		IXmlNode CreateProcessingInstruction(string target, string data);

		IXmlElement CreateElement(string elementName);

		IXmlElement CreateElement(string qualifiedName, string namespaceURI);

		IXmlNode CreateAttribute(string name, string value);

		IXmlNode CreateAttribute(string qualifiedName, string namespaceURI, string value);
	}
}
