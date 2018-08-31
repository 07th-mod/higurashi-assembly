using System.Collections.Generic;
using System.Linq;

namespace Newtonsoft.Json.Schema
{
	internal class JsonSchemaModelBuilder
	{
		private JsonSchemaNodeCollection _nodes = new JsonSchemaNodeCollection();

		private Dictionary<JsonSchemaNode, JsonSchemaModel> _nodeModels = new Dictionary<JsonSchemaNode, JsonSchemaModel>();

		private JsonSchemaNode _node;

		public JsonSchemaModel Build(JsonSchema schema)
		{
			_nodes = new JsonSchemaNodeCollection();
			_node = AddSchema(null, schema);
			_nodeModels = new Dictionary<JsonSchemaNode, JsonSchemaModel>();
			return BuildNodeModel(_node);
		}

		public JsonSchemaNode AddSchema(JsonSchemaNode existingNode, JsonSchema schema)
		{
			string id;
			if (existingNode != null)
			{
				if (existingNode.Schemas.Contains(schema))
				{
					return existingNode;
				}
				id = JsonSchemaNode.GetId(existingNode.Schemas.Union(new JsonSchema[1]
				{
					schema
				}));
			}
			else
			{
				id = JsonSchemaNode.GetId(new JsonSchema[1]
				{
					schema
				});
			}
			if (_nodes.Contains(id))
			{
				return _nodes[id];
			}
			JsonSchemaNode jsonSchemaNode = (existingNode == null) ? new JsonSchemaNode(schema) : existingNode.Combine(schema);
			_nodes.Add(jsonSchemaNode);
			AddProperties(schema.Properties, jsonSchemaNode.Properties);
			AddProperties(schema.PatternProperties, jsonSchemaNode.PatternProperties);
			if (schema.Items != null)
			{
				for (int i = 0; i < schema.Items.Count; i++)
				{
					AddItem(jsonSchemaNode, i, schema.Items[i]);
				}
			}
			if (schema.AdditionalProperties != null)
			{
				AddAdditionalProperties(jsonSchemaNode, schema.AdditionalProperties);
			}
			if (schema.Extends != null)
			{
				jsonSchemaNode = AddSchema(jsonSchemaNode, schema.Extends);
			}
			return jsonSchemaNode;
		}

		public void AddProperties(IDictionary<string, JsonSchema> source, IDictionary<string, JsonSchemaNode> target)
		{
			if (source != null)
			{
				foreach (KeyValuePair<string, JsonSchema> item in source)
				{
					AddProperty(target, item.Key, item.Value);
				}
			}
		}

		public void AddProperty(IDictionary<string, JsonSchemaNode> target, string propertyName, JsonSchema schema)
		{
			target.TryGetValue(propertyName, out JsonSchemaNode value);
			target[propertyName] = AddSchema(value, schema);
		}

		public void AddItem(JsonSchemaNode parentNode, int index, JsonSchema schema)
		{
			JsonSchemaNode existingNode = (parentNode.Items.Count <= index) ? null : parentNode.Items[index];
			JsonSchemaNode jsonSchemaNode = AddSchema(existingNode, schema);
			if (parentNode.Items.Count <= index)
			{
				parentNode.Items.Add(jsonSchemaNode);
			}
			else
			{
				parentNode.Items[index] = jsonSchemaNode;
			}
		}

		public void AddAdditionalProperties(JsonSchemaNode parentNode, JsonSchema schema)
		{
			parentNode.AdditionalProperties = AddSchema(parentNode.AdditionalProperties, schema);
		}

		private JsonSchemaModel BuildNodeModel(JsonSchemaNode node)
		{
			if (_nodeModels.TryGetValue(node, out JsonSchemaModel value))
			{
				return value;
			}
			value = JsonSchemaModel.Create(node.Schemas);
			_nodeModels[node] = value;
			foreach (KeyValuePair<string, JsonSchemaNode> property in node.Properties)
			{
				if (value.Properties == null)
				{
					value.Properties = new Dictionary<string, JsonSchemaModel>();
				}
				value.Properties[property.Key] = BuildNodeModel(property.Value);
			}
			foreach (KeyValuePair<string, JsonSchemaNode> patternProperty in node.PatternProperties)
			{
				if (value.PatternProperties == null)
				{
					value.PatternProperties = new Dictionary<string, JsonSchemaModel>();
				}
				value.PatternProperties[patternProperty.Key] = BuildNodeModel(patternProperty.Value);
			}
			for (int i = 0; i < node.Items.Count; i++)
			{
				if (value.Items == null)
				{
					value.Items = new List<JsonSchemaModel>();
				}
				value.Items.Add(BuildNodeModel(node.Items[i]));
			}
			if (node.AdditionalProperties != null)
			{
				value.AdditionalProperties = BuildNodeModel(node.AdditionalProperties);
			}
			return value;
		}
	}
}
