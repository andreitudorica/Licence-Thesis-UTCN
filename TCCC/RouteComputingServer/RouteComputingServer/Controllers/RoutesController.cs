﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Itinero;
using Itinero.LocalGeo;
using Itinero.Profiles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RouteComputingServer.Services;

namespace RouteComputingServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoutesController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            var a = MapsService.routerDb;
            return "Route Computing Server is Running. Waiting for commands.";
        }

        [HttpGet("GetRoute")]
        public IActionResult GetRoute(string profile, float startLat, float startLon, float endLat, float endLon, bool mapRefresh)
        {

            try
            {
                if (mapRefresh)
                {
                    MapsService.LoadMaps();
                }
                Route route;
                if (profile == "shortest")
                    route = MapsService.router.Calculate(MapsService.customCar.Shortest(), new Coordinate(startLat, startLon), new Coordinate(endLat, endLon));
                else
                    route = MapsService.router.Calculate(MapsService.customCar.Fastest(), new Coordinate(startLat, startLon), new Coordinate(endLat, endLon));
                string routeJson = route.ToGeoJson();
                return Ok(routeJson);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
