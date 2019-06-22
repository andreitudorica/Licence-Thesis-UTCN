﻿using Itinero;
using Itinero.Attributes;
using Itinero.LocalGeo;
using Itinero.Profiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LiveTrafficServer
{
    public static class EdgeWeights
    {
        public static void AddWeightsToRouterDb(RouterDb routerDb)
        {
            for (int i = 1; i <= 50; i++)
            {
                routerDb.EdgeProfiles.Add(new AttributeCollection(
                new Itinero.Attributes.Attribute("highway", "residential"),
                new Itinero.Attributes.Attribute("custom-speed", i + "")));
            }
        }



        public static void SetWeight(RouterDb routerDb, uint edgeId, int i)
        {
            try
            {
                // update the speed profile of this edge.
                var edgeData = routerDb.Network.GetEdge(edgeId).Data;
                var asdf = routerDb.EdgeProfiles.Get(edgeData.Profile);
                edgeData.Profile = (ushort)(87 + i);
                asdf = routerDb.EdgeProfiles.Get(edgeData.Profile);
                routerDb.Network.UpdateEdgeData(edgeId, edgeData);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);
            }
        }

        public static void DecreaseWeight(RouterDb routerDb, uint edgeId)
        {
            try
            {
                // update the speed profile of this edge.
                var edge = routerDb.Network.GetEdge(edgeId);
                var edgeData = edge.Data;
                var edgeProfile = edgeData.Profile;
                var carCount = Int32.Parse(routerDb.EdgeProfiles.Get(edgeProfile).Where(o=>o.Key=="car-count").First().Value);
                var dist = edgeData.Distance;
                if (carCount > 0)
                    carCount--;
                var occupancy = (carCount * 4) / dist;
                if ((int)(50 - 50 * occupancy) <= 0)
                    edgeData.Profile = (ushort)(Startup.profilesStart + carCount * 50);
                else
                    edgeData.Profile = (ushort)(Startup.profilesStart + carCount * 50 + (int)(50 - 50 * occupancy));
                routerDb.Network.UpdateEdgeData(edgeId, edgeData);
            }
            catch (Exception e)
            {
                int a = 1;
                a++;
                Console.WriteLine(e.Message);
            }
        }

        public static void IncreaseWeight(RouterDb routerDb, uint edgeId)
        {
            try
            {
                // update the speed profile of this edge.
                var edge = routerDb.Network.GetEdge(edgeId);
                var edgeData = edge.Data;
                var edgeProfile = edgeData.Profile;

                if (edgeProfile < Startup.profilesStart)
                {
                    edgeData.Profile = (ushort)Startup.profilesStart;
                    routerDb.Network.UpdateEdgeData(edgeId, edgeData);
                    edge = routerDb.Network.GetEdge(edgeId);
                    edgeData = edge.Data;
                    edgeProfile = edgeData.Profile;
                }
                var carCount = Int32.Parse(routerDb.EdgeProfiles.Get(edgeProfile).Where(o => o.Key == "car-count").First().Value);
                var dist = edgeData.Distance;
                if (carCount < 100)
                    carCount++;
                var occupancy = (carCount * 4) / dist;
                if ((int)(50 - 50 * occupancy) <= 0)
                    edgeData.Profile = (ushort)(Startup.profilesStart + carCount * 50);
                else
                    edgeData.Profile = (ushort)(Startup.profilesStart + carCount * 50 + (int)(50 - 50 * occupancy));
                routerDb.Network.UpdateEdgeData(edgeId, edgeData);
            }
            catch (Exception e)
            {
                int a = 1;
                a++;
                Console.WriteLine(e.Message);
            }
        }

        public static void HandleChange(float previousEdgeLon, float previousEdgeLat, float currentEdgeLon, float currentEdgeLat)
        {
            uint previousEdgeId = 0, currentEdgeId = 0;
            RouterPoint resolvedPrevious, resolvedCurrent;
            Coordinate currentEdgeLocation, previousEdgeLocation;
            var customCar = DynamicVehicle.Load(System.IO.File.ReadAllText(CommonVariables.PathToCommonFolder + CommonVariables.CustomCarProfileFileName));
            RouterDb routerDb = Startup.routerDb;
            var router = new Router(routerDb);

            try
            {
                if (previousEdgeLon != 0)
                {
                    previousEdgeLocation = new Coordinate(previousEdgeLon, previousEdgeLat);
                    resolvedPrevious = router.Resolve(customCar.Shortest(), previousEdgeLocation);
                    previousEdgeId = resolvedPrevious.EdgeId;
                }


                if (currentEdgeLon != 0)
                {
                    currentEdgeLocation = new Coordinate(currentEdgeLon, currentEdgeLat);
                    resolvedCurrent = router.Resolve(customCar.Shortest(), currentEdgeLocation);
                    currentEdgeId = resolvedCurrent.EdgeId;
                }

                if (previousEdgeLon != 0 && currentEdgeId == 0)
                {
                    DecreaseWeight(routerDb, (uint)previousEdgeId);
                }

                if (currentEdgeLon != 0 && previousEdgeId == 0)
                {
                    IncreaseWeight(routerDb, (uint)currentEdgeId);
                }

                if (previousEdgeId != 0 && currentEdgeId != 0 && previousEdgeId != currentEdgeId)
                {
                    DecreaseWeight(routerDb, (uint)previousEdgeId);
                    IncreaseWeight(routerDb, (uint)currentEdgeId);
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.Message);

            }
        }
    }
}