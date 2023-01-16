using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherStationAPI.Data.Models;

namespace WeatherStationAPI.Data.Repository
{
    public interface IUserDataRepository
    {
        string CreateUser(UserData newUser);
        UserData AuthenticateUser(string APIKey, string requiredAccess);
        int RemoveIdleUsers(DateTime lastLogin);
        void RemoveSingleUser( string id);
        void RemoveMultipleUsers(string role);
        void UpdateLoginTime(string APIKey, DateTime loginTime);
        bool UpdateMultipleRoles(string id, string property, object value);
        List<UserData> GetAllUsers();
    }
}
