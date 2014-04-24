using System;
using Eto.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Reflection;

namespace Eto.Json
{
	public class DynamicLayoutConverter : JsonConverter
	{
		public override bool CanWrite
		{
			get { return false; }
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(DynamicItem).IsAssignableFrom(objectType) || typeof(DynamicRow).IsAssignableFrom(objectType);
		}

		public override object ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			object instance;
			JContainer container;
			if (reader.TokenType == JsonToken.Null)
			{
				return null;
			}
			if (reader.TokenType == JsonToken.StartArray)
			{
				container = JArray.Load(reader);
				if (objectType == typeof(DynamicRow))
				{
					var dynamicRow = new DynamicRow();
					instance = dynamicRow;
					serializer.Populate(container.CreateReader(), dynamicRow.Items);
				}
				else if (objectType == typeof(DynamicItem))
				{
					var dynamicTable = new DynamicTable();
					instance = dynamicTable;
					serializer.Populate(container.CreateReader(), dynamicTable.Rows);
				}
				else
					throw new EtoException("Invalid object graph");
			}
			else
			{
				container = JObject.Load(reader);
				if (container["$type"] == null)
				{
					if (container["Rows"] != null)
						instance = new DynamicTable();
					else if (container["Control"] != null)
						instance = new DynamicControl();
					else
						throw new EtoException("Could not infer the type of object to create");

					serializer.Populate(container.CreateReader(), instance);
				}
				else
				{
					var type = Type.GetType((string)container["$type"]);
					if (!typeof(DynamicItem).IsAssignableFrom(type))
					{
						var dynamicControl = new DynamicControl();
						dynamicControl.Control = serializer.Deserialize(container.CreateReader()) as Control;
						instance = dynamicControl;
					}
					else
					{
						instance = serializer.Deserialize(container.CreateReader());
					}
				}
			}
			if (objectType == typeof(DynamicRow) && instance.GetType() != typeof(DynamicRow))
			{
				var row = new DynamicRow();
				row.Items.Add(instance as DynamicItem);
				return row;
			}

			return instance;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
}

