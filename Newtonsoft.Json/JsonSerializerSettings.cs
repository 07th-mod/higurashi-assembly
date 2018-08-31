using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;

namespace Newtonsoft.Json
{
	public class JsonSerializerSettings
	{
		internal const ReferenceLoopHandling DefaultReferenceLoopHandling = ReferenceLoopHandling.Error;

		internal const MissingMemberHandling DefaultMissingMemberHandling = MissingMemberHandling.Ignore;

		internal const NullValueHandling DefaultNullValueHandling = NullValueHandling.Include;

		internal const DefaultValueHandling DefaultDefaultValueHandling = DefaultValueHandling.Include;

		internal const ObjectCreationHandling DefaultObjectCreationHandling = ObjectCreationHandling.Auto;

		internal const PreserveReferencesHandling DefaultPreserveReferencesHandling = PreserveReferencesHandling.None;

		internal const ConstructorHandling DefaultConstructorHandling = ConstructorHandling.Default;

		internal const TypeNameHandling DefaultTypeNameHandling = TypeNameHandling.None;

		internal const FormatterAssemblyStyle DefaultTypeNameAssemblyFormat = FormatterAssemblyStyle.Simple;

		internal static readonly StreamingContext DefaultContext = default(StreamingContext);

		public ReferenceLoopHandling ReferenceLoopHandling
		{
			get;
			set;
		}

		public MissingMemberHandling MissingMemberHandling
		{
			get;
			set;
		}

		public ObjectCreationHandling ObjectCreationHandling
		{
			get;
			set;
		}

		public NullValueHandling NullValueHandling
		{
			get;
			set;
		}

		public DefaultValueHandling DefaultValueHandling
		{
			get;
			set;
		}

		public IList<JsonConverter> Converters
		{
			get;
			set;
		}

		public PreserveReferencesHandling PreserveReferencesHandling
		{
			get;
			set;
		}

		public TypeNameHandling TypeNameHandling
		{
			get;
			set;
		}

		public FormatterAssemblyStyle TypeNameAssemblyFormat
		{
			get;
			set;
		}

		public ConstructorHandling ConstructorHandling
		{
			get;
			set;
		}

		public IContractResolver ContractResolver
		{
			get;
			set;
		}

		public IReferenceResolver ReferenceResolver
		{
			get;
			set;
		}

		public SerializationBinder Binder
		{
			get;
			set;
		}

		public EventHandler<ErrorEventArgs> Error
		{
			get;
			set;
		}

		public StreamingContext Context
		{
			get;
			set;
		}

		public JsonSerializerSettings()
		{
			ReferenceLoopHandling = ReferenceLoopHandling.Error;
			MissingMemberHandling = MissingMemberHandling.Ignore;
			ObjectCreationHandling = ObjectCreationHandling.Auto;
			NullValueHandling = NullValueHandling.Include;
			DefaultValueHandling = DefaultValueHandling.Include;
			PreserveReferencesHandling = PreserveReferencesHandling.None;
			TypeNameHandling = TypeNameHandling.None;
			TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple;
			Context = DefaultContext;
			Converters = new List<JsonConverter>();
		}
	}
}
