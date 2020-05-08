using System;
using System.Diagnostics;
using Mars.Components.Layers;
using Mars.Interfaces.Layer;
using Mars.Interfaces.Layer.Initialization;

namespace KrugerNationalPark.Layers
{
    public class KNPTimeKeeperLayer : AbstractActiveLayer, ISteppedActiveLayer
    {
        private long _currentTick;
        private readonly Stopwatch _stopWatch;

        public KNPTimeKeeperLayer()
        {
            _stopWatch = new Stopwatch();
        }

        public override bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            _stopWatch.Restart();
            return base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
        }

        public void Tick()
        {
            if (Context.CurrentTimePoint.Value.DayOfWeek == DayOfWeek.Monday && Context.CurrentTimePoint.Value.Hour == 1)
            {
                Console.Write(".");
            }

            if (Context.CurrentTimePoint.Value.Day == 1 && Context.CurrentTimePoint.Value.Hour == 1)
            {
                Console.Write(" ");
            }

            if (!IsNextYearTick()) return;
            Console.Write(Context.CurrentTimePoint.Value.Year + " in " + _stopWatch.Elapsed.Days + "d " +
                          _stopWatch.Elapsed.Hours + "h " + _stopWatch.Elapsed.Minutes + "m " +
                          _stopWatch.Elapsed.Seconds + "s ");
            Console.WriteLine();
            _stopWatch.Restart();
        }

        private bool IsNextYearTick()
        {
            return Context.CurrentTimePoint.Value.Day == Context.StartTimePoint.Value.Day &&
                   Context.CurrentTimePoint.Value.Hour == 1 &&
                   Context.CurrentTimePoint.Value.Month == Context.StartTimePoint.Value.Month &&
                   Context.CurrentTimePoint.Value.Year > Context.StartTimePoint.Value.Year;
        }
    }
}