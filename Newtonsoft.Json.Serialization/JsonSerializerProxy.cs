using Newtonsoft.Json.Utilities;
using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace Newtonsoft.Json.Serialization
{
	internal class JsonSerializerProxy : JsonSerializer
	{
		private readonly JsonSerializerInternalReader _serializerReader;

		private readonly JsonSerializerInternalWriter _serializerWriter;

		private readonly JsonSerializer _serializer;

		public override IReferenceResolver ReferenceResolver
		{
			get
			{
				return _serializer.ReferenceResolver;
			}
			set
			{
				_serializer.ReferenceResolver = value;
			}
		}

		public override JsonConverterCollection Converters => _serializer.Converters;

		public override DefaultValueHandling DefaultValueHandling
		{
			get
			{
				return _serializer.DefaultValueHandling;
			}
			set
			{
				_serializer.DefaultValueHandling = value;
			}
		}

		public override IContractResolver ContractResolver
		{
			get
			{
				return _serializer.ContractResolver;
			}
			set
			{
				_serializer.ContractResolver = value;
			}
		}

		public override MissingMemberHandling MissingMemberHandling
		{
			get
			{
				return _serializer.MissingMemberHandling;
			}
			set
			{
				_serializer.MissingMemberHandling = value;
			}
		}

		public override NullValueHandling NullValueHandling
		{
			get
			{
				return _serializer.NullValueHandling;
			}
			set
			{
				_serializer.NullValueHandling = value;
			}
		}

		public override ObjectCreationHandling ObjectCreationHandling
		{
			get
			{
				return _serializer.ObjectCreationHandling;
			}
			set
			{
				_serializer.ObjectCreationHandling = value;
			}
		}

		public override ReferenceLoopHandling ReferenceLoopHandling
		{
			get
			{
				return _serializer.ReferenceLoopHandling;
			}
			set
			{
				_serializer.ReferenceLoopHandling = value;
			}
		}

		public override PreserveReferencesHandling PreserveReferencesHandling
		{
			get
			{
				return _serializer.PreserveReferencesHandling;
			}
			set
			{
				_serializer.PreserveReferencesHandling = value;
			}
		}

		public override TypeNameHandling TypeNameHandling
		{
			get
			{
				return _serializer.TypeNameHandling;
			}
			set
			{
				_serializer.TypeNameHandling = value;
			}
		}

		public override FormatterAssemblyStyle TypeNameAssemblyFormat
		{
			get
			{
				return _serializer.TypeNameAssemblyFormat;
			}
			set
			{
				_serializer.TypeNameAssemblyFormat = value;
			}
		}

		public override ConstructorHandling ConstructorHandling
		{
			get
			{
				return _serializer.ConstructorHandling;
			}
			set
			{
				_serializer.ConstructorHandling = value;
			}
		}

		public override SerializationBinder Binder
		{
			get
			{
				return _serializer.Binder;
			}
			set
			{
				_serializer.Binder = value;
			}
		}

		public override StreamingContext Context
		{
			get
			{
				return _serializer.Context;
			}
			set
			{
				_serializer.Context = value;
			}
		}

		public override event EventHandler<ErrorEventArgs> Error
		{
			add
			{
				_serializer.Error += value;
			}
			remove
			{
				_serializer.Error -= value;
			}
		}

		public JsonSerializerProxy(JsonSerializerInternalReader serializerReader)
		{
			ValidationUtils.ArgumentNotNull(serializerReader, "serializerReader");
			_serializerReader = serializerReader;
			_serializer = serializerReader.Serializer;
		}

		public JsonSerializerProxy(JsonSerializerInternalWriter serializerWriter)
		{
			ValidationUtils.ArgumentNotNull(serializerWriter, "serializerWriter");
			_serializerWriter = serializerWriter;
			_serializer = serializerWriter.Serializer;
		}

		internal JsonSerializerInternalBase GetInternalSerializer()
		{
			if (_serializerReader != null)
			{
				return _serializerReader;
			}
			return _serializerWriter;
		}

		internal override object DeserializeInternal(JsonReader reader, Type objectType)
		{
			if (_serializerReader != null)
			{
				return _serializerReader.Deserialize(reader, objectType);
			}
			return _serializer.Deserialize(reader, objectType);
		}

		internal override void PopulateInternal(JsonReader reader, object target)
		{
			if (_serializerReader != null)
			{
				_serializerReader.Populate(reader, target);
			}
			else
			{
				_serializer.Populate(reader, target);
			}
		}

		internal override void SerializeInternal(JsonWriter jsonWriter, object value)
		{
			if (_serializerWriter != null)
			{
				_serializerWriter.Serialize(jsonWriter, value);
			}
			else
			{
				_serializer.Serialize(jsonWriter, value);
			}
		}
	}
}
