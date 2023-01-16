using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WeatherStationAPI.Data.Models
{
    public class UserData
    {
        [BsonId]
        public ObjectId _id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public DateTime LastAccessed { get; set; } = DateTime.Now;
        [Required]
        public string Role { get; set; } = "Student";
    }
}
