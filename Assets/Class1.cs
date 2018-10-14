// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var class1 = Class1.FromJson(jsonString);

namespace QuickType0
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Class1
    {
        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("ct")]
        public Ct Ct { get; set; }

        [JsonProperty("rl")]
        public Rl[] Rl { get; set; }

        [JsonProperty("pgcnt")]
        public long Pgcnt { get; set; }
    }

    public partial class Ct
    {
        [JsonProperty("iv")]
        public long Iv { get; set; }

        [JsonProperty("ivcv")]
        public long Ivcv { get; set; }

        [JsonProperty("tag")]
        public long Tag { get; set; }

        [JsonProperty("tn")]
        public string Tn { get; set; }

        [JsonProperty("vmcrr")]
        public long Vmcrr { get; set; }

        [JsonProperty("vmcm")]
        public string Vmcm { get; set; }
    }

    public partial class Rl
    {
        [JsonProperty("rid")]
        public long Rid { get; set; }

        [JsonProperty("rn")]
        public string Rn { get; set; }

        [JsonProperty("uid")]
        public long Uid { get; set; }

        [JsonProperty("nn")]
        public string Nn { get; set; }

        [JsonProperty("cid1")]
        public long Cid1 { get; set; }

        [JsonProperty("cid2")]
        public long Cid2 { get; set; }

        [JsonProperty("cid3")]
        public long Cid3 { get; set; }

        [JsonProperty("iv")]
        public long Iv { get; set; }

        [JsonProperty("av")]
        public string Av { get; set; }

        [JsonProperty("ol")]
        public long Ol { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("c2url")]
        public C2Url C2Url { get; set; }

        [JsonProperty("c2name")]
        public C2Name C2Name { get; set; }

        [JsonProperty("icdata")]
        public Dictionary<string, Icdatum> Icdata { get; set; }

        [JsonProperty("dot")]
        public long Dot { get; set; }

        [JsonProperty("subrt")]
        public long Subrt { get; set; }

        [JsonProperty("topid")]
        public long Topid { get; set; }

        [JsonProperty("bid")]
        public long Bid { get; set; }

        [JsonProperty("gldid")]
        public long Gldid { get; set; }

        [JsonProperty("rs1")]
        public string Rs1 { get; set; }

        [JsonProperty("rs16")]
        public string Rs16 { get; set; }

        [JsonProperty("utag")]
        public Utag[] Utag { get; set; }

        [JsonProperty("rpos")]
        public long Rpos { get; set; }

        [JsonProperty("rgrpt")]
        public long Rgrpt { get; set; }

        [JsonProperty("rkic")]
        public Rkic Rkic { get; set; }

        [JsonProperty("rt")]
        public long Rt { get; set; }

        [JsonProperty("ot")]
        public long Ot { get; set; }

        [JsonProperty("clis")]
        public long Clis { get; set; }

        [JsonProperty("chanid")]
        public long Chanid { get; set; }

        [JsonProperty("icv1")]
        public Icv1[][] Icv1 { get; set; }
    }

    public partial class Icdatum
    {
        [JsonProperty("url")]
        [JsonConverter(typeof(ParseIntegerConverter))]
        public long Url { get; set; }

        [JsonProperty("w")]
        public long W { get; set; }

        [JsonProperty("h")]
        public long H { get; set; }
    }

    public partial class Icv1
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("score")]
        public long Score { get; set; }

        [JsonProperty("w")]
        public long W { get; set; }

        [JsonProperty("h")]
        public long H { get; set; }
    }

    public partial class Utag
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public enum C2Name { 颜值};

    public enum C2Url { DirectoryGameXx, DirectoryGameYz };

    public enum Rkic { Empty };

    public partial class Class1
    {
        public static Class1 FromJson(string json)
        {
            return JsonConvert.DeserializeObject<Class1>(json, QuickType.Converter.Settings);
        }
    }

    public static class Serialize
    {
        public static string ToJson(this Class1 self)
        {
            return JsonConvert.SerializeObject(self, QuickType.Converter.Settings);
        }
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                C2NameConverter.Singleton,
                C2UrlConverter.Singleton,
                RkicConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class C2NameConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(C2Name) || t == typeof(C2Name?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "颜值")
            {
                return C2Name.颜值;
            }
            throw new Exception("Cannot unmarshal type C2Name");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (C2Name)untypedValue;
            if (value == C2Name.颜值)
            {
                serializer.Serialize(writer, "颜值");
                return;
            }
            throw new Exception("Cannot marshal type C2Name");
        }

        public static readonly C2NameConverter Singleton = new C2NameConverter();
    }

    internal class C2UrlConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(C2Url) || t == typeof(C2Url?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "/directory/game/XX":
                    return C2Url.DirectoryGameXx;
                case "/directory/game/yz":
                    return C2Url.DirectoryGameYz;
            }
            throw new Exception("Cannot unmarshal type C2Url");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (C2Url)untypedValue;
            switch (value)
            {
                case C2Url.DirectoryGameXx:
                    serializer.Serialize(writer, "/directory/game/XX");
                    return;
                case C2Url.DirectoryGameYz:
                    serializer.Serialize(writer, "/directory/game/yz");
                    return;
            }
            throw new Exception("Cannot marshal type C2Url");
        }

        public static readonly C2UrlConverter Singleton = new C2UrlConverter();
    }

    internal class ParseIntegerConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseIntegerConverter Singleton = new ParseIntegerConverter();
    }

    internal class RkicConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Rkic) || t == typeof(Rkic?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "")
            {
                return Rkic.Empty;
            }
            throw new Exception("Cannot unmarshal type Rkic");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Rkic)untypedValue;
            if (value == Rkic.Empty)
            {
                serializer.Serialize(writer, "");
                return;
            }
            throw new Exception("Cannot marshal type Rkic");
        }

        public static readonly RkicConverter Singleton = new RkicConverter();
    }
}
