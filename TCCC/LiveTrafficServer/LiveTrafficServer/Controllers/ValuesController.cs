﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Itinero;
using Itinero.IO.Osm;
using Itinero.LocalGeo;
using Itinero.Osm.Vehicles;
using Microsoft.AspNetCore.Mvc;

namespace LiveTrafficServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public async Task<string> Get()
        {
            var routerDb = new RouterDb();
            var time =DateTime.Now;
            string result = "";
            /*using (var stream = System.IO.File.OpenRead("D:\\Andrei\\Scoala\\LICENTA\\Maps\\Cluj-Napoca.pbf"))
            {
                routerDb.LoadOsmData(stream, Vehicle.Car);
            }*/
            using (var stream = System.IO.File.OpenRead("D:\\Andrei\\Scoala\\LICENTA\\Maps\\Cluj-Napoca.routerdb"))
            {
                routerDb = RouterDb.Deserialize(stream);
            }
            result += "reading RouteDB: " + (DateTime.Now - time).ToString(@"dd\.hh\:mm\:ss")+" ";


            //change distance of one edge

           

            //routerDb.AddContracted(routerDb.GetSupportedProfile("car"));
            using (var stream = System.IO.File.OpenWrite("D:\\Andrei\\Scoala\\LICENTA\\Maps\\Cluj-Napoca.routerdb"))
            {
                routerDb.Serialize(stream);
            }

            result += " writing RouterDB: " + (DateTime.Now - time).ToString(@"dd\.hh\:mm\:ss\.ff") + " ";
            string apiResponse;
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("http://localhost:62917/api/router/GetRoute?profile=car&startX=46.768293&startY=23.629875&endX=46.752623&endY=23.577261"))
                {
                    apiResponse = await response.Content.ReadAsStringAsync();
                }
            }

            result += " finished computing route: " + (DateTime.Now - time).ToString(@"dd\.hh\:mm\:ss\.ff") + " ";
            return result;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}