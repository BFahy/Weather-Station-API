using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace WeatherStationAPI.Data.Models
{
    public class WeatherDataDTO
    {
        public WeatherDataDTO(double tempC, double atmPress, double precip, DateTime time, string deviceName)
        {       
            DeviceName = deviceName;
            TemperatureC = tempC;
            AtmosphericPressure = atmPress;
            Precipitation = precip;
            Time = time;
        }

        public string DeviceName { get; set; }

        [BsonDateTimeOptions]
        public DateTime Time { get; set; }             
        

        [BsonElement("Temperature (°C)")]
        public double TemperatureC { get; set; }

        [BsonElement("Atmospheric Pressure (kPa)")]
        public double AtmosphericPressure { get; set; }

        [BsonElement("Precipitation mm/h")]
        public double Precipitation { get; set; }        

    }
}
