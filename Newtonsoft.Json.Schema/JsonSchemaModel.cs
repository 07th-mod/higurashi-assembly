using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Utilities;
using System.Collections.Generic;

namespace Newtonsoft.Json.Schema
{
	internal class JsonSchemaModel
	{
		public bool Required
		{
			get;
			set;
		}

		public JsonSchemaType Type
		{
			get;
			set;
		}

		public int? MinimumLength
		{
			get;
			set;
		}

		public int? MaximumLength
		{
			get;
			set;
		}

		public double? DivisibleBy
		{
			get;
			set;
		}

		public double? Minimum
		{
			get;
			set;
		}

		public double? Maximum
		{
			get;
			set;
		}

		public bool ExclusiveMinimum
		{
			get;
			set;
		}

		public bool ExclusiveMaximum
		{
			get;
			set;
		}

		public int? MinimumItems
		{
			get;
			set;
		}

		public int? MaximumItems
		{
			get;
			set;
		}

		public IList<string> Patterns
		{
			get;
			set;
		}

		public IList<JsonSchemaModel> Items
		{
			get;
			set;
		}

		public IDictionary<string, JsonSchemaModel> Properties
		{
			get;
			set;
		}

		public IDictionary<string, JsonSchemaModel> PatternProperties
		{
			get;
			set;
		}

		public JsonSchemaModel AdditionalProperties
		{
			get;
			set;
		}

		public bool AllowAdditionalProperties
		{
			get;
			set;
		}

		public IList<JToken> Enum
		{
			get;
			set;
		}

		public JsonSchemaType Disallow
		{
			get;
			set;
		}

		public JsonSchemaModel()
		{
			Type = JsonSchemaType.Any;
			AllowAdditionalProperties = true;
			Required = false;
		}

		public static JsonSchemaModel Create(IList<JsonSchema> schemata)
		{
			JsonSchemaModel jsonSchemaModel = new JsonSchemaModel();
			foreach (JsonSchema schematum in schemata)
			{
				Combine(jsonSchemaModel, schematum);
			}
			return jsonSchemaModel;
		}

		private static void Combine(JsonSchemaModel model, JsonSchema schema)
		{
			int required2;
			if (!model.Required)
			{
				bool? required = schema.Required;
				required2 = ((required.HasValue && required.Value) ? 1 : 0);
			}
			else
			{
				required2 = 1;
			}
			model.Required = ((byte)required2 != 0);
			JsonSchemaType type = model.Type;
			JsonSchemaType? type2 = schema.Type;
			model.Type = (type & ((!type2.HasValue) ? JsonSchemaType.Any : type2.Value));
			model.MinimumLength = MathUtils.Max(model.MinimumLength, schema.MinimumLength);
			model.MaximumLength = MathUtils.Min(model.MaximumLength, schema.MaximumLength);
			model.DivisibleBy = MathUtils.Max(model.DivisibleBy, schema.DivisibleBy);
			model.Minimum = MathUtils.Max(model.Minimum, schema.Minimum);
			model.Maximum = MathUtils.Max(model.Maximum, schema.Maximum);
			int exclusiveMinimum2;
			if (!model.ExclusiveMinimum)
			{
				bool? exclusiveMinimum = schema.ExclusiveMinimum;
				exclusiveMinimum2 = ((exclusiveMinimum.HasValue && exclusiveMinimum.Value) ? 1 : 0);
			}
			else
			{
				exclusiveMinimum2 = 1;
			}
			model.ExclusiveMinimum = ((byte)exclusiveMinimum2 != 0);
			int exclusiveMaximum2;
			if (!model.ExclusiveMaximum)
			{
				bool? exclusiveMaximum = schema.ExclusiveMaximum;
				exclusiveMaximum2 = ((exclusiveMaximum.HasValue && exclusiveMaximum.Value) ? 1 : 0);
			}
			else
			{
				exclusiveMaximum2 = 1;
			}
			model.ExclusiveMaximum = ((byte)exclusiveMaximum2 != 0);
			model.MinimumItems = MathUtils.Max(model.MinimumItems, schema.MinimumItems);
			model.MaximumItems = MathUtils.Min(model.MaximumItems, schema.MaximumItems);
			model.AllowAdditionalProperties = (model.AllowAdditionalProperties && schema.AllowAdditionalProperties);
			if (schema.Enum != null)
			{
				if (model.Enum == null)
				{
					model.Enum = new List<JToken>();
				}
				model.Enum.AddRangeDistinct(schema.Enum, new JTokenEqualityComparer());
			}
			JsonSchemaType disallow = model.Disallow;
			JsonSchemaType? disallow2 = schema.Disallow;
			model.Disallow = (disallow | (disallow2.HasValue ? disallow2.Value : JsonSchemaType.None));
			if (schema.Pattern != null)
			{
				if (model.Patterns == null)
				{
					model.Patterns = new List<string>();
				}
				model.Patterns.AddDistinct(schema.Pattern);
			}
		}
	}
}
