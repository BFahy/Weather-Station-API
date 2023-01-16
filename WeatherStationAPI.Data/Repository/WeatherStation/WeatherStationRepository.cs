using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using WeatherStationAPI.Data.Models;
using WeatherStationAPI.Data.Services;

namespace WeatherStationAPI.Data.Repository
{
    public class WeatherStationRepository
    {
        IMongoCollection<WeatherData> _collection;
        FilterDefinitionBuilder<WeatherData> _builder;
        public WeatherStationRepository(string connectionString = "")
        {
            IMongoDatabase db;
            if (!String.IsNullOrEmpty(connectionString))
            {
                db = MongoConnector.GetDatabase(connectionString, "WeatherStation");
            }
            else
            {
                db = MongoConnector.GetLocalDatabase("WeatherStation");
            }            
            // Added to allow for using the filter throughout the repository
            _builder = Builders<WeatherData>.Filter;

            _collection = db.GetCollection<WeatherData>("ClimateData");
        }

        public void InsertData(WeatherData weatherData)
        {
            _collection.InsertOne(weatherData);
        }

        public List<WeatherData> GetAll()
        {
            return _collection.Find(_builder.Empty).Limit(1000).ToList();
        }

        public WeatherData GetSingleById(string objId)
        {
            var filter = _builder.Eq(c => c._id, ObjectId.Parse(objId));

            return _collection.Find(filter).FirstOrDefault();

        }

        /** Finds the maximum precipitation recorded in the last 5 Months (single)
         * 
         */
        public WeatherData GetMaxForPrecipitation()
        {
            // Sets filter to check for a precipitation value -> then if created in the last 5 months
            var filter = _builder
                .And(_builder.Gt(c => c.Precipitation, 0),
                _builder.Lt(c => c.Time, DateTime.UtcNow.AddMonths(-5)));

            return _collection.Find(filter).SortByDescending(c => c.Precipitation).FirstOrDefault();

            // Applies filter and finds max precipitation result, limited to 1 from collection
            //return _collection.Find(filter).SortByDescending(c => c.Precipitation).Limit(1).ToList();
        }

        public List<WeatherData> GetDateInformation(string deviceName, string date1, string date2)
        {
            try
            {
                // DD-MM-YYYY-HH
                string[] temp = date1.Split('-');
                var c = new DateTime(
                    Int32.Parse(temp[2].PadLeft(1, '0')), Int32.Parse(temp[1].PadLeft(1, '0')),
                    Int32.Parse(temp[0]), Int32.Parse(temp[3].PadLeft(1, '0')), 0, 0).ToLocalTime();

                temp = date2.Split('-');
                var c2 = new DateTime(
                    Int32.Parse(temp[2].PadLeft(1, '0')), Int32.Parse(temp[1].PadLeft(1, '0')),
                    Int32.Parse(temp[0]), Int32.Parse(temp[3].PadLeft(1, '0')), 0, 0).ToLocalTime();

                var filter = (_builder.And(
                _builder.Eq(c => c.DeviceName, deviceName),
                _builder.Gte(c => c.Time, c),
                _builder.Lt(c => c.Time, c2)));    


                return _collection.Find(filter).ToList();
            }
            catch (Exception e)
            {
                return null;
            }
            
        }

        public void UpdateLatLong(string objid, double newLatitude, double newLongitude)
        {
            try
            {
                var filter = _builder.Eq(c => c._id, ObjectId.Parse(objid));

                var newLat = Builders<WeatherData>.Update.Set(c => c.Latitude, newLatitude);
                var newLong = Builders<WeatherData>.Update.Set(c => c.Longitude, newLongitude);
                var newLocation = Builders<WeatherData>.Update.Set(c => c.Location, newLatitude.ToString() + "," + newLongitude.ToString());

                _collection.UpdateOne(filter, newLat);
                _collection.UpdateOne(filter, newLong);
                _collection.UpdateOne(filter, newLocation);
            }
            catch(Exception e)
            {
                Console.Write(e.Message);
            }
        }

        
        public void InsertMultipleData(List<WeatherData> weatherData)
        {
            //var weatherCollection = new List<WeatherData>() {weatherData};
            
            _collection.InsertMany(weatherData);
        }
    }
}
