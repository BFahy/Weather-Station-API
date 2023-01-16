using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using WeatherStationAPI.Data.Models;
using WeatherStationAPI.Data.Services;

namespace WeatherStationAPI.Data.Repository
{
    public class UserDataRepository : IUserDataRepository
    {
        IMongoDatabase _db;
        IMongoCollection<UserData> _users;
        FilterDefinitionBuilder<UserData> _builder;
        public UserDataRepository()
        {
            _db = MongoConnector.GetLocalDatabase("WeatherStation");
            _users = _db.GetCollection<UserData>("Users");
            _builder = Builders<UserData>.Filter;
        }
        
        public UserData AuthenticateUser(string APIKey, string requiredAccess)
        {
            var filter = Builders<UserData>.Filter.Eq(c => c.ApiKey, APIKey);
            var user = _users.Find(filter).FirstOrDefault();

            if (user == null || !user.Role.Equals(requiredAccess))
            {
                return null;
            }
            return user;
        }

        public string CreateUser(UserData newUser)
        {
            var filter = _builder.And(
                _builder.Eq(c => c.Name, newUser.Name),
                _builder.Eq(c => c.Email, newUser.Email));
            var existingUser = _users.Find(filter).FirstOrDefault();
            if(existingUser != null)
            {
                return "";
            }

            newUser.ApiKey = Guid.NewGuid().ToString();
            _users.InsertOne(newUser);
            return newUser.ApiKey;
        }
        public int RemoveIdleUsers(DateTime lastLogin)
        {
            throw new NotImplementedException();
        }

        public void RemoveSingleUser(string id)
        {
            var filter = _builder.Eq(c => c._id, ObjectId.Parse(id));
            _users.DeleteOne(filter);
        }

        public void RemoveMultipleUsers(string role)
        {
            var bson = new BsonDocument();
            using (var temp = new BsonDocumentWriter(bson))
            {
                temp.WriteStartDocument();
                temp.WriteName("Role");
                temp.WriteString(role);
                temp.WriteEndDocument();
            }
            _users.DeleteMany(bson);
        }

        public void UpdateLoginTime(string APIKey, DateTime loginTime)
        {

            var filter = Builders<UserData>.Filter.Eq(c => c.ApiKey, APIKey);
            var update = Builders<UserData>.Update.Set(c => c.LastAccessed, loginTime);

            _users.UpdateOne(filter, update);
        }

        public bool UpdateMultipleRoles(string id, string property, object value)
        {
            UpdateResult result = null;
            string[] temp = id.Split('-');

            foreach(var item in temp)
            {
                var filter = _builder.And(_builder.Eq(c => c._id, ObjectId.Parse(item)), _builder.Exists(property));
                var update = Builders<UserData>.Update.Set(property, value);
                result = _users.UpdateOne(filter, update);
            }


            return result.ModifiedCount > 0;
        }

        public List<UserData> GetAllUsers()
        {
            return _users.Find(_builder.Empty).ToList();
        }
    }
}
