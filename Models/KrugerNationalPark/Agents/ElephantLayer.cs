using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using KrugerNationalPark.Layers;
using Mars.Components.Environments;
using Mars.Components.Layers;
using Mars.Components.Services;
using Mars.Interfaces.Environments;
using Mars.Interfaces.Layers;
using Mars.Interfaces.Layers.Initialization;
using NetTopologySuite.Geometries;

namespace KrugerNationalPark.Agents
{
    public class ElephantLayer : AbstractActiveLayer, ISteppedActiveLayer
    {
        private readonly IDictionary<int, ElephantHerd> _herdMap;
        private readonly NormalDistributionGenerator _normalDistributionGenerator;
        private readonly RasterFenceLayer _rasterFenceLayer;
        private readonly RasterShadeLayer _shadeLayer;
        private readonly RasterTempLayer _temperatureLayer;
        private readonly RasterVegetationLayer _vegetationLayerDgvm;
        private readonly VectorWaterLayer _waterPotentialLayer;
        private RegisterAgent _registerAgent;
        private UnregisterAgent _unregisterAgent;


        public ElephantLayer
        (
            RasterVegetationLayer vegetationLayerDgvm,
            VectorWaterLayer waterPotentialLayer,
            RasterTempLayer temperatureLayer,
            RasterFenceLayer rasterFenceLayer,
            RasterShadeLayer shadeLayer)
        {
            _waterPotentialLayer = waterPotentialLayer;
            _vegetationLayerDgvm = vegetationLayerDgvm;
            Entities = new ConcurrentDictionary<Guid, Elephant>();
            _herdMap = new ConcurrentDictionary<int, ElephantHerd>();

            var baseExtent = new Envelope(vegetationLayerDgvm.Extent);
            baseExtent.ExpandedBy(waterPotentialLayer.Extent);
            baseExtent.ExpandedBy(temperatureLayer.Extent);
            baseExtent.ExpandedBy(rasterFenceLayer.Extent);
            baseExtent.ExpandedBy(shadeLayer.Extent);

            Environment = GeoHashEnvironment<Elephant>.BuildByBBox(new BoundingBox(baseExtent), 1000);
            _temperatureLayer = temperatureLayer;
            _shadeLayer = shadeLayer;
            _rasterFenceLayer = rasterFenceLayer;
            _normalDistributionGenerator = new NormalDistributionGenerator(35, 30);
        }

        public GeoHashEnvironment<Elephant> Environment { get; }

        public ConcurrentDictionary<Guid, Elephant> Entities { get; set; }

        bool ILayer.InitLayer
            (LayerInitData layerLayerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            base.InitLayer(layerLayerInitData, registerAgentHandle, unregisterAgentHandle);
            //params needed for calf spawn
            _registerAgent = registerAgentHandle;
            _unregisterAgent = unregisterAgentHandle;

            var agentInitConfig =
                layerLayerInitData.AgentInitConfigs.FirstOrDefault(mapping => mapping.Type.MetaType == typeof(Elephant));

            if (agentInitConfig != null)
            {
                // Spawn all elephant agents
                Entities = AgentManager.GetAgentsByAgentInitConfig<Elephant>
                (agentInitConfig, registerAgentHandle, unregisterAgentHandle,
                    new List<ILayer>
                    {
                        this, _waterPotentialLayer, _temperatureLayer, _shadeLayer, _vegetationLayerDgvm,
                        _rasterFenceLayer
                    },
                    Environment);
                Console.WriteLine("[ElephantLayer]: Created " + Entities.Count + " Agents");

                // create herd objects
                var listOfHerds =
                    Entities.Values.AsParallel().GroupBy(elephant => elephant.HerdId).Select(grp => grp.ToList())
                        .ToList();
                Console.WriteLine("[ElephantLayer]: Created " + listOfHerds.Count + " Herds");

                foreach (var h in listOfHerds)
                {
                    var leader = h.FirstOrDefault(e => e.Leading);
                    if (leader == null)
                    {
                        leader = h.FirstOrDefault();
                        if (leader == null)
                            throw new Exception("There is a herd without elephants, which is impossible!");

                        leader.Leading = true;
                    }

                    var other = h.Where(e => !e.Leading).ToList();
                    _herdMap.Add(leader.HerdId, new ElephantHerd(leader.HerdId, leader, other));
                }

                Console.WriteLine("[ElephantLayer]: Filled Herds");
                return true;
            }

            return false;
        }

        public override void PostTick()
        {
            // CHECK: this must be harmonized to real elephant numbers

//            // culling elephants (goes on to including 1994)
//            // a year is calculated with 8766 hours to include leap years
//            // the culling quotas used for this come from the book
//            // "Elephant Management" - Scholes 2009
//            // every 3 days a herd is killed (if neccessary)
//            if (_currentTick % 72 != 0) return;
//            // 1989
//            if (_currentTick < 8766 && ElephantMap.Count > 7468)
//            {
//                KillElephantHerd();
//            }
//            // 1990
//            else if (_currentTick < 17532 && ElephantMap.Count > 7287)
//            {
//                KillElephantHerd();
//            }
//            // 1991
//            else if (_currentTick < 26298 && ElephantMap.Count > 7470)
//            {
//                KillElephantHerd();
//            }
//            // 1992
//            else if (_currentTick < 35064 && ElephantMap.Count > 7632)
//            {
//                KillElephantHerd();
//            }
//            // 1993
//            else if (_currentTick < 43830 && ElephantMap.Count > 7834)
//            {
//                KillElephantHerd();
//            }
//            // 1994
//            else if (_currentTick < 52596 && ElephantMap.Count > 7806)
//            {
//                KillElephantHerd();
//            }
        }

        public Elephant GetLeadingElephantByHerd(int herdId)
        {
            _herdMap.TryGetValue(herdId, out var herd);
            return herd?.LeadingElephant;
        }

        public void SpawnCalf(ElephantLayer elephantLayer, double latitude, double longitude, int herdId,
            double biomassCellDifference = 1.0, double satietyMultiplier = 1.0, int tickSearchForFood = 1,
            int biomassNeighbourSearchLvl = 1,
            double minDehydration = 100)
        {
            var newElephant = new Elephant(elephantLayer,
                _registerAgent, _unregisterAgent, Environment,
                _waterPotentialLayer, _vegetationLayerDgvm, _rasterFenceLayer, _temperatureLayer, _shadeLayer,
                Guid.NewGuid(), latitude, longitude, herdId, "ELEPHANT_NEWBORN",
                false, biomassCellDifference, satietyMultiplier, tickSearchForFood,
                biomassNeighbourSearchLvl, minDehydration);

            Entities.TryAdd(newElephant.ID, newElephant);
        }

        private void KillElephantHerd()
        {
            var herdId = _herdMap.Keys.FirstOrDefault();
            _herdMap.TryGetValue(herdId, out var myHerd);
            if (myHerd != null)
            {
                var leadingCow = myHerd.LeadingElephant;
                var otherElephants = myHerd.OtherElephants;
                leadingCow.Die(MattersOfDeath.Culling);
                Entities.TryRemove(leadingCow.ID, out _);
                foreach (var el in otherElephants)
                {
                    el.Die(MattersOfDeath.Culling);
                    Entities.TryRemove(el.ID, out _);
                }

                _herdMap.Remove(herdId);
            }
            else
            {
                Console.WriteLine("[ElephantLayer] error killing a herd");
            }
        }

        public int GetNextNormalDistribution()
        {
            return (int) _normalDistributionGenerator.GetNext();
        }
    }

    public class NormalDistributionGenerator
    {
        private readonly double _maximumDeviation;
        private readonly double _meanValue;
        private readonly Random _rand = new Random();
        private readonly double _standardDeviation;

        public NormalDistributionGenerator(double meanValue, double maximumDeviation)
        {
            _meanValue = meanValue;
            _maximumDeviation = maximumDeviation;
            //Since the normal distribution is theoretically infinite, you can't have a hard cap on your range.
            //So we cut every number that is not in the 99.73% (three standard deviations). Therefore the maximum deviation is devided by 3 to get the standard deviation.
            //Hopefully this description is formally correct ;-) Otherwise look a the example in the class documentation.
            _standardDeviation = maximumDeviation / 3;
        }

        public double GetNext()
        {
            //code partly from http://stackoverflow.com/questions/218060/random-gaussian-variables
            var u1 = _rand.NextDouble();
            var u2 = _rand.NextDouble();
            var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                Math.Sin(2.0 * Math.PI * u2);
            var random = _meanValue + _standardDeviation * randStdNormal;

            if (random < _meanValue - _maximumDeviation) return _meanValue - _maximumDeviation;

            if (random > _meanValue + _maximumDeviation) return _meanValue + _maximumDeviation;

            return random;
        }
    }
}