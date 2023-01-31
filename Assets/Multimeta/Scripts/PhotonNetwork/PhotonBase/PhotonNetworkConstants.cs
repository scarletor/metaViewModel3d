// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Exit Games GmbH">
//   Exit Games GmbH, 2012
// </copyright>
// <summary>
//   The "Particle" demo is a load balanced and Photon Cloud compatible "coding" demo.
//   The focus is on showing how to use the Photon features without too much "game" code cluttering the view.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

namespace ExitGames.Client.Multimeta
{
    /// <summary>
    /// Class to define a few constants used in this demo (for event codes, properties, etc).
    /// </summary>
    /// <remarks>
    /// These values are something made up for this particular demo! 
    /// You can define other values (and more) in your games, as needed.
    /// </remarks>
    public static class PhotonNetworkConstants
    {
        /// <summary>(1) Event defining a color of a player.</summary>
        public const byte EvPlayerInfo = 1;

        /// <summary>(2) Event defining the position of a player.</summary>
        public const byte EvPosition = 2;

        /// <summary>Types available as map / level / scene.</summary>
        public enum MapType { Metaverse }
    }
}