using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WeatherStationAPI.Data.Models;
using WeatherStationAPI.Data.Repository;

namespace WeatherStationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherStationController : ControllerBase
    {
        WeatherStationRepository _wsRepo;
        public WeatherStationController()
        {
            _wsRepo = new WeatherStationRepository();
        }

        /// <summary>
        /// Retrieves all weather data from relevant data storage
        /// </summary>
        /// <returns>List of weather data</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public IActionResult GetAllData()
        {
            return Ok(_wsRepo.GetAll());
        }

        /// <summary>
        /// Retrieves a single weather data document by ID
        /// </summary>
        /// <param name="objid">Object ID of weather data to be retrieved</param>
        /// <returns></returns>
        [HttpGet("{objid}")]
        public IActionResult GetSingleByID(string objid)
        {            
            return Ok(_wsRepo.GetSingleById(objid));
        }

        /// <summary>
        /// Create a weather data entry
        /// </summary>
        /// <param name="weatherData">Weather data entry</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateData(WeatherData weatherData)
        {
            
            _wsRepo.InsertData(weatherData);

            return CreatedAtAction("CreateData", weatherData);
        }

        /// <summary>
        /// Retrieves largest precipitation weather station entry in last 5 months
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetForPrecipitation5Months")]
        public IActionResult FindMaxPrecipitation()
        {
            WeatherData wD = _wsRepo.GetMaxForPrecipitation();
            return Ok(wD.Precipitation);
        }

        /// <summary>
        /// Returns station name, temp(C), atmospheric pressure, precipitation, date/time for specific station between specified date/time
        /// </summary>
        /// <param name="deviceName">Name of station</param>
        /// <param name="date1">Format: DD-MM-YYYY-HH</param>
        /// <param name="date2">Format: DD-MM-YYYY-HH</param>
        /// <returns>station ID, temp(C), atmospheric pressure, precipitation, date/time</returns>
        [HttpGet("GetDateInformation")]
        public IActionResult FindDateInformation(string deviceName, string date1, string date2)
        {
            if(_wsRepo.GetDateInformation(deviceName, date1, date2) != null)
            {
                var a = _wsRepo.GetDateInformation(deviceName, date1, date2);
                var returnList = new List<WeatherDataDTO>();

                foreach(WeatherData item in a)
                {
                    returnList.Add(new WeatherDataDTO(item.TemperatureC, item.AtmosphericPressure, item.Precipitation, item.Time, item.DeviceName));
                }

                return Ok(returnList);
                //return Ok(_wsRepo.GetDateInformation(id, date1, date2));
            } 
            else
            {
                return BadRequest();
            }
            
        }

        /// <summary>
        /// Updates weather station latitude and longitude entry at id given
        /// </summary>
        /// <param name="objid">Target Object ID</param>
        /// <param name="newLatitude">New latitude entry</param>
        /// <param name="newLongitude">New longitude entry</param>
        /// <returns></returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpPut]
        public IActionResult UpdateStationLatAndLong(string objid, double newLatitude, double newLongitude)
        {
            _wsRepo.UpdateLatLong(objid, newLatitude, newLongitude);
                
            return NoContent();
        }

        /// <summary>
        /// Allows for creation of multiple weather data entries at once
        /// </summary>
        /// <param name="weatherData">Format: '[{}, {}]' where entry is set between curly brackets</param>
        /// <returns></returns>
        [HttpPost("CreateMultiple")]
        public IActionResult InsertMultipleEntries(List<WeatherData> weatherData)
        {

            _wsRepo.InsertMultipleData(weatherData);

            return CreatedAtAction("CreateData", weatherData);
        }
    }
}
