// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using QuickType;
//
//    var videoData = VideoData.FromJson(jsonString);

namespace QuickType
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class VideoData
    {
        [JsonProperty("bmsg")]
        public Bmsg Bmsg { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }

        [JsonProperty("timestamp")]
        public string Timestamp { get; set; }
    }

    public partial class Bmsg
    {
        [JsonProperty("t")]
        public string T { get; set; }

        [JsonProperty("f")]
        public string F { get; set; }

        [JsonProperty("mid")]
        public string Mid { get; set; }

        [JsonProperty("sp")]
        public string Sp { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("pp")]
        public long Pp { get; set; }

        [JsonProperty("ps")]
        public string Ps { get; set; }

        [JsonProperty("pt")]
        public long Pt { get; set; }

        [JsonProperty("vlist")]
        public Vlist[] Vlist { get; set; }

        [JsonProperty("bossType")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long BossType { get; set; }

        [JsonProperty("aQipuId")]
        public long AQipuId { get; set; }

        [JsonProperty("qiyiProduced")]
        public long QiyiProduced { get; set; }

        [JsonProperty("allNum")]
        public long AllNum { get; set; }

        [JsonProperty("pg")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Pg { get; set; }

        [JsonProperty("isBossMixer")]
        public long IsBossMixer { get; set; }

        [JsonProperty("ic")]
        public long Ic { get; set; }

        [JsonProperty("pgt")]
        public long Pgt { get; set; }

        [JsonProperty("aid")]
        public long Aid { get; set; }

        [JsonProperty("pm")]
        public long Pm { get; set; }

        [JsonProperty("pn")]
        public long Pn { get; set; }

        [JsonProperty("cid")]
        public long Cid { get; set; }
    }

    public partial class Vlist
    {
        [JsonProperty("mdown")]
        public long Mdown { get; set; }

        [JsonProperty("wmarkPos")]
        public long WmarkPos { get; set; }

        [JsonProperty("publishTime")]
        public long PublishTime { get; set; }

        [JsonProperty("vpic")]
        public string Vpic { get; set; }

        [JsonProperty("tvQipuId")]
        [JsonConverter(typeof(DecodingChoiceConverter))]
        public long TvQipuId { get; set; }

        [JsonProperty("payMarkUrl")]
        public string PayMarkUrl { get; set; }

        [JsonProperty("purType")]
        public long PurType { get; set; }

        [JsonProperty("shortTitle")]
        public string ShortTitle { get; set; }

        [JsonProperty("type")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Type { get; set; }

        [JsonProperty("lgh")]
        public object[] Lgh { get; set; }

        [JsonProperty("isProduced")]
        public long IsProduced { get; set; }

        [JsonProperty("vurl")]
        public string Vurl { get; set; }

        [JsonProperty("plcdown")]
        public Dictionary<string, long> Plcdown { get; set; }

        [JsonProperty("vid")]
        public string Vid { get; set; }

        [JsonProperty("timeLength")]
        public long TimeLength { get; set; }

        [JsonProperty("pd")]
        public long Pd { get; set; }

        [JsonProperty("vn")]
        public string Vn { get; set; }

        [JsonProperty("payMark")]
        public long PayMark { get; set; }

        [JsonProperty("exclusive")]
        public long Exclusive { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("vt")]
        public string Vt { get; set; }

        [JsonProperty("pds")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Pds { get; set; }
    }

    public partial class VideoData
    {
        public static VideoData FromJson(string json)
        {
            return JsonConvert.DeserializeObject<VideoData>(json, QuickType.Converter.Settings);
        }
    }

    public static class Serialize
    {
        public static string ToJson(this VideoData self)
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
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t)
        {
            return t == typeof(long) || t == typeof(long?);
        }

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

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class DecodingChoiceConverter : JsonConverter
    {
        public override bool CanConvert(Type t)
        {
            return t == typeof(long) || t == typeof(long?);
        }

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            switch (reader.TokenType)
            {
                case JsonToken.Integer:
                    var integerValue = serializer.Deserialize<long>(reader);
                    return integerValue;
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    long l;
                    if (Int64.TryParse(stringValue, out l))
                    {
                        return l;
                    }
                    break;
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
            serializer.Serialize(writer, value);
            return;
        }

        public static readonly DecodingChoiceConverter Singleton = new DecodingChoiceConverter();
    }
}
