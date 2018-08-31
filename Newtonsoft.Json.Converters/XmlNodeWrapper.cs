using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Newtonsoft.Json.Converters
{
	internal class XmlNodeWrapper : IXmlNode
	{
		private readonly XmlNode _node;

		public object WrappedNode => _node;

		public XmlNodeType NodeType => _node.NodeType;

		public string Name => _node.Name;

		public string LocalName => _node.LocalName;

		public IList<IXmlNode> ChildNodes => (from XmlNode n in _node.ChildNodes
		select WrapNode(n)).ToList();

		public IList<IXmlNode> Attributes
		{
			get
			{
				if (_node.Attributes == null)
				{
					return null;
				}
				return (from XmlAttribute a in _node.Attributes
				select WrapNode(a)).ToList();
			}
		}

		public IXmlNode ParentNode
		{
			get
			{
				XmlNode xmlNode = (!(_node is XmlAttribute)) ? _node.ParentNode : ((XmlAttribute)_node).OwnerElement;
				if (xmlNode == null)
				{
					return null;
				}
				return WrapNode(xmlNode);
			}
		}

		public string Value
		{
			get
			{
				return _node.Value;
			}
			set
			{
				_node.Value = value;
			}
		}

		public string Prefix => _node.Prefix;

		public string NamespaceURI => _node.NamespaceURI;

		public XmlNodeWrapper(XmlNode node)
		{
			_node = node;
		}

		private IXmlNode WrapNode(XmlNode node)
		{
			switch (node.NodeType)
			{
			case XmlNodeType.Element:
				return new XmlElementWrapper((XmlElement)node);
			case XmlNodeType.XmlDeclaration:
				return new XmlDeclarationWrapper((XmlDeclaration)node);
			default:
				return new XmlNodeWrapper(node);
			}
		}

		public IXmlNode AppendChild(IXmlNode newChild)
		{
			XmlNodeWrapper xmlNodeWrapper = (XmlNodeWrapper)newChild;
			_node.AppendChild(xmlNodeWrapper._node);
			return newChild;
		}
	}
}
