﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using KrugerNationalPark.Agents;
using KrugerNationalPark.Layers;
using KrugerNationalPark.Output;
using Mars.Common.Core.Collections;
using Mars.Common.Core.Logging;
using Mars.Common.Core.Logging.Enums;
using Mars.Components.Starter;
using Mars.Core.Simulation.Entities;
using Mars.Interfaces.Model;

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

            // Second register the agent types with their respective layer type
            var elephant = description.AddAgent<Elephant, ElephantLayer>();

            // Starting up
            SimulationWorkflowState result = null;
            if (args != null)
            {
                if (args.Any(s => s.Equals("-l")))
                {
                    LoggerFactory.SetLogLevel(LogLevel.Info);
                    LoggerFactory.ActivateConsoleLogging();
                }

                string file;
                if (args.Any(s => s.Equals("-sm")))
                {
                    var index = args.IndexOf(s => s == "-sm");
                    file = File.ReadAllText(args[index + 1]);
                }
                else
                {
                    file = File.ReadAllText("config.json");
                }

                var simConfig = SimulationConfig.Deserialize(file);
                var starter = SimulationStarter.Start(description, simConfig);
                result = starter.Run();
            }

            // Generate proprietary trips output
            TripsOutputAdapter.PrintTripResult(result.Model.ExecutionAgentTypeGroups.Values
                .SelectMany(agents => agents.Values).OfType<ITripSavingAgent>());
            watch.Stop();
            Console.WriteLine($"Simulation finished and last {watch.Elapsed}");
        }
    }
}