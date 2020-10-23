using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using KrugerNationalPark.Agents;
using KrugerNationalPark.Layers;
using Mars.Common.Collections;
using Mars.Common.Core.Collections;
using Mars.Common.Core.Logging;
using Mars.Common.Core.Logging.Enums;
using Mars.Components.Starter;
using Mars.Core.Model.Entities;
using Mars.Core.Simulation.Entities;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.IO.Converters;
using SOHMultimodalModel.Output.Trips;

namespace KrugerNationalParkStarter
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var watch = Stopwatch.StartNew();
            var description = new ModelDescription();

            // Turning logger on or off
            LoggerFactory.SetLogLevel(LogLevel.Off);

            // First register each layer at the runtime system
            description.AddLayer<RasterTempLayer>();
            description.AddLayer<RasterFenceLayer>();
            description.AddLayer<RasterShadeLayer>();
            description.AddLayer<RasterVegetationLayer>();
            description.AddLayer<VectorWaterLayer>();
            description.AddLayer<ElephantLayer>();
            description.AddLayer<KnpCarLayer>();

            // Second register the agent types with their respective layer type
            description.AddAgent<KnpCarDriver, KnpCarLayer>();
            description.AddAgent<Elephant, ElephantLayer>();
            description.AddEntity<KnpCar>();

            // Starting up
            SimulationWorkflowState result = null;
            if (args != null)
            {
                if (args.Any(s => s.Equals("-l")))
                {
                    LoggerFactory.SetLogLevel(LogLevel.Info);
                    LoggerFactory.ActivateConsoleLogging();
                }

                if (args.Any(s => s.Equals("-sm")))
                {
                    var index = args.IndexOf(s => s == "-sm");
                    var file = File.ReadAllText(args[index + 1]);
                    var simConfig = SimulationConfig.Deserialize(file);

                    var starter = SimulationStarter.Start(description, simConfig);
                    result = starter.Run();
                }
                else
                {
                    throw new ArgumentOutOfRangeException();
                }
            }

            // Generate proprietary trips output
            GenerateTripResult(result);
            watch.Stop();
            Console.WriteLine($"Simulation finished and last {watch.Elapsed}");
        }


        private static void GenerateTripResult(SimulationWorkflowState result)
        {
            var writer = new GeoJsonWriter();
            var featureCollection = new FeatureCollection();

            var jsonConverters = writer.SerializerSettings.Converters
                .Where(converter => converter is CoordinateConverter);

            foreach (var jsonConverter in jsonConverters) writer.SerializerSettings.Converters.Remove(jsonConverter);

            writer.SerializerSettings.Converters.Add(new TripPositionCoordinateConverter());

            var runtimeModelExecutionGroup = result.Model.ExecutionGroups[1];
            foreach (var tickClient in runtimeModelExecutionGroup)
            {
                if (tickClient is KnpCarDriver driver)
                {
                    var trip = driver.Trip;
                    if (trip.Count >= 2)
                    {
                        var path = new LineString(trip.ToArray());
                        featureCollection.Add(new Feature(path, new AttributesTable()));
                    }
                }
            }

            File.WriteAllText("cars.geojson", writer.Write(featureCollection));

            if (Directory.Exists("tmp"))
                Directory.Delete("tmp", true);
        }
    }
}