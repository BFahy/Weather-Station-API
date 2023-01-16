using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WeatherStationAPI.Attributes;
using WeatherStationAPI.Data.Models;
using WeatherStationAPI.Data.Repository;

namespace WeatherStationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDataController : ControllerBase
    {

        private readonly IUserDataRepository _users;
        public UserDataController(IUserDataRepository users)
        {
            _users = users;
        }

        /// <summary>
        /// Retrieves list of all existing users
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public IActionResult GetUsers()
        {
            return Ok(_users.GetAllUsers());
        }

        /// <summary>
        /// Create a user
        /// </summary>
        /// <param name="APIKey">Requires Admin API Key</param>
        /// <param name="newUser">Specify: Name, Email and Role</param>
        /// <returns></returns>
        [APIKey(role: "Admin")]
        [HttpPost]
        public IActionResult CreateUser(string APIKey, UserData newUser)
        {
            /*if(!IsAuthenticated(APIKey, "Admin"))
            {
                return Unauthorized();
            }*/

            var usersAPIKey = _users.CreateUser(newUser);

            return Ok(usersAPIKey);
        }

        /// <summary>
        /// Delete a single user from collection
        /// </summary>
        /// <param name="APIKey">Requires Teacher API Key</param>
        /// <param name="id">Student to be deleted</param>
        /// <returns></returns>
        [APIKey(role: "Teacher")]
        [HttpDelete("DeleteUser/{id}")]
        public IActionResult DeleteUser(string APIKey, string id)
        {
            _users.RemoveSingleUser(id);
            return NoContent();
        }

        /// <summary>
        /// Deletes all users of a specified role
        /// </summary>
        /// <param name="APIKey">Requires Admin API Key</param>
        /// <param name="role">Student, Teacher or Admin</param>
        /// <returns></returns>
        [APIKey(role: "Admin")]
        [HttpDelete("DeleteMany/{role}")]
        public IActionResult DeleteMultipleUsers(string APIKey, string role)
        {            
            _users.RemoveMultipleUsers(role);
            return NoContent();
        }

        /// <summary>
        /// Updates set ID user fields
        /// </summary>
        /// <param name="userPatchDoc">Set operation: "add" -> path -> value</param>
        /// <param name="APIKey">Requires Admin API Key</param>
        /// <param name="id">ObjectID separated by '-'</param>
        /// <returns></returns>
        [APIKey(role: "Admin")]
        [HttpPatch]
        public IActionResult UpdateUserFields([FromBody] JsonPatchDocument<UserData> userPatchDoc, string APIKey, string id)           
        {
            if (userPatchDoc == null)
            {
                return BadRequest();
            }

            var operation = userPatchDoc.Operations.FirstOrDefault();
            var succeeded = _users.UpdateMultipleRoles(id, operation.path, operation.value);

            return succeeded ? Ok() : BadRequest("The update operation has failed.");
        }


        //private bool IsAuthenticated(string APIKey, string requiredAccess)
        //{
        //    if (_users.AuthenticateUser(APIKey, requiredAccess) == null)
        //    {
        //        return false;
        //    }
        //    _users.UpdateLoginTime(APIKey, DateTime.Now);
        //    return true;
        //}

    }
}
