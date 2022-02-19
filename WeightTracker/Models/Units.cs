using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WeightTracker.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Units
    {
        Imperial,
        Metric
    }
}
