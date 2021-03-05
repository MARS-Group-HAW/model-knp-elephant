using System;
using Mars.Interfaces.Layers;
using SOHCarModel.Model;

namespace KrugerNationalPark.Agents
{
    /// <summary>
    ///     This class represents the agent layer implementation for the <see cref="KnpCar" /> and keeps references
    ///     to all other required layer e.g., the <see cref="ElephantLayer" />
    /// </summary>
    public class KnpCarLayer : CarLayer
    {
        /// <summary>
        ///     Creates an new instance of the <see cref="KnpCarLayer" /> which done by the runtime system
        ///     In order to get access to ther layer, define them as an additional parameter.
        /// </summary>
        /// <example>
        ///     public KnpCarLayer(ElephantLayer elephantLayer, KNPGISRasterShadeLayer waterLayer)
        /// </example>
        /// <param name="elephantLayer"></param>
        public KnpCarLayer(ElephantLayer elephantLayer)
        {
            ElephantLayer = elephantLayer;
        }

        public ElephantLayer ElephantLayer { get; }

        /// <summary>
        ///     Initializes the layer with layerInitData.
        ///     Use this instead of the constructor, as it is
        ///     guaranteed to be called in the correct load order.
        ///     <pre>This layer was successfully added to its container.</pre>
        ///     <post>
        ///         This layer is in a state which allows
        ///         it to start the simulation.
        ///     </post>
        ///     <param name="layerInitData">
        ///         A data type holding the
        ///         information of how to initialize a layer.
        ///     </param>
        ///     <param name="registerAgentHandle"> </param>
        /// </summary>
        /// <returns>True if init finished successfully, false otherwise</returns>
        public override bool InitLayer(LayerInitData layerInitData, RegisterAgent registerAgentHandle,
            UnregisterAgent unregisterAgentHandle)
        {
            var result = base.InitLayer(layerInitData, registerAgentHandle, unregisterAgentHandle);
            Console.WriteLine($"[KnpCarLayer]: Created {Driver.Count} Agents");
            return result;
        }
    }
}