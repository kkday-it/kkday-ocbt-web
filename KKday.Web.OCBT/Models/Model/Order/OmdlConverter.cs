using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KKday.Web.OCBT.Models.Model.Order
{
    public class OmdlConverter
    {
        public class ModuleDataConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {


                if (reader.TokenType == JsonToken.StartArray)
                {
                    List<ModuleModel> obj = new List<ModuleModel>();
                    JToken token = JToken.Load(reader);

                    obj = token.ToObject<List<ModuleModel>>();
                    return obj;

                }
                else
                {
                    if (reader.TokenType == JsonToken.StartObject)
                    {
                        JToken token = JToken.Load(reader);
                        ModuleModel toModelobj = token.ToObject<ModuleModel>();
                        List<ModuleModel> obj = new List<ModuleModel>();
                        obj.Add(toModelobj);
                        return obj;
                    }
                    return null;
                }





            }
            
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }

        public class MealDataConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {


                if (reader.TokenType == JsonToken.StartObject)
                {
                    
                    JToken token = JToken.Load(reader);

                    var obj = token.ToObject<MealType>();
                    return obj.type;

                }
                else
                {
                    if (reader.TokenType == JsonToken.String)
                    {
                        JToken token = JToken.Load(reader);
                        var obj = token.ToObject<string>();
                        return obj;
                    }
                    return null;
                }





            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }

        public class AppDataConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(string);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {


                if (reader.TokenType == JsonToken.StartObject)
                {

                    JToken token = JToken.Load(reader);

                    var obj = token.ToObject<AppType>();
                    return obj.type;

                }
                else
                {
                    if (reader.TokenType == JsonToken.String)
                    {
                        JToken token = JToken.Load(reader);
                        var obj = token.ToObject<string>();
                        return obj;
                    }
                    return null;
                }





            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                serializer.Serialize(writer, value);
            }
        }
    }
}
