namespace Newtonsoft.Json.Converters
{
	internal interface IXmlElement : IXmlNode
	{
		void SetAttributeNode(IXmlNode attribute);

		string GetPrefixOfNamespace(string namespaceURI);
	}
}
