using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WeatherStationAPI.Data.Models
{
    public class WeatherData
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public virtual string _ObjId
        {
            get { return _id.ToString(); }
        }
        [BsonElement("Device Name")]
        public string DeviceName { get; set; }

        [BsonDateTimeOptions]
        public DateTime Time { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Location { get; set; }

        [BsonElement("Temperature (°C)")]
        public double TemperatureC { get; set; }

        [BsonElement("Atmospheric Pressure (kPa)")]
        public double AtmosphericPressure { get; set; }

        [BsonElement("Precipitation mm/h")]
        public double Precipitation { get; set; }

        [BsonElement("Solar Radiation (W/m2)")]
        public double SolarRadiation { get; set; }

        private double? temperatureF;

        [BsonElement("Temperature (°F)")]
        public double? TemperatureF
        {
            get
            {
                return temperatureF.HasValue ? temperatureF : (TemperatureC * 1.8) + 32;
            }
            set
            {
                temperatureF = value.HasValue ? value : (TemperatureC * 1.8) + 32; ;
            }
        }

    }
}
