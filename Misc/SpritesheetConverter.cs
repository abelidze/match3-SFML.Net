//using System;
//using System.Collections.Generic;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using Match3.Animation;

//namespace Match3.Misc
//{
//    class SpritesheetConverter : JsonConverter
//    {
//        public override bool CanConvert(Type objectType)
//        {
//            return typeof(Spritesheet).IsAssignableFrom(objectType);
//        }

//        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
//        {
//            JObject obj = JObject.Load(reader);
//            Spritesheet sheet = new Spritesheet(
//                (string) obj["name"], (string) obj["path"], (int) obj["width"], (int) obj["height"]);

//            foreach (JProperty prop in obj["sprites"]) {
//                var sprite = prop.Value.ToObject<AnimatedSprite>(serializer);
//                sheet.Sprites.Add(prop.Name, sprite);
//            }
//            return sheet;
//        }

//        public override bool CanWrite
//        {
//            get { return false; }
//        }

//        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}