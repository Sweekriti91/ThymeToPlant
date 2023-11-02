using System;
using System.Collections.Generic;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace ThymeToPlant.Models
{
    public partial class PlantZoneDataItem
    {
        [JsonPropertyName("zone")]
        public string Zone { get; set; }

        [JsonPropertyName("coordinates")]
        public Coordinates Coordinates { get; set; }

        [JsonPropertyName("temperature_range")]
        public string TemperatureRange { get; set; }
    }

    public partial class Coordinates
    {
        [JsonPropertyName("lat")]
        public double Lat { get; set; }

        [JsonPropertyName("lon")]
        public double Lon { get; set; }
    }
}

