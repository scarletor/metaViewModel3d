// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Exit Games GmbH">
//   Exit Games GmbH, 2012
// </copyright>
// <summary>
//   The "Particle" demo is a load balanced and Photon Cloud compatible "coding" demo.
//   The focus is on showing how to use the Photon features without too much "game" code cluttering the view.
// </summary>
// <author>developer@photonengine.com</author>
// --------------------------------------------------------------------------------------------------------------------

namespace ExitGames.Client.Multimeta
{
    using global::Photon.Realtime;

    /// <summary>
    /// Room for the Particle demo extends Room and has additional properties and methods.
    /// </summary>
    /// <remarks>
    /// You could implement the properties (GridSize, etc) also in other ways.
    /// Example: Override CacheProperties and whenever a property is cached, update backing fields with the new values.
    ///
    /// The GameLogic makes sure the properties are set when we create rooms.
    /// The values might change while the room is in use. In this demo, we change the GridSize, e.g..
    /// </remarks>
    public class MultimetaRoom : Room
    {
        /// <summary>Fetches the custom room-property "map" or returns "forest" if that is not available.</summary>
        public string Map
        {
            get
            {
                return PhotonNetworkConstants.MapType.Metaverse.ToString();
            }
        }

        /// <summary>
        /// Uses the base constructor to initialize this ParticleRoom.
        /// </summary>
        protected internal MultimetaRoom(string roomName, RoomOptions opt)
            : base(roomName, opt)
        {

        }

        /// <summary>
        /// Gets the group for the specified position (in this case the quadrant).
        /// </summary>
        /// <remarks>
        /// For simplicity, this demo splits the grid into 4 quadrants, no matter which size the grid has.
        ///
        /// Groups can be used to split up a room into regions but also could be used to separate teams, etc.
        /// Groups use a byte as id which starts with 1 and goes up, depending on how many we use.
        /// Group 0 would be received by everyone, so we skip that.
        /// </remarks>
        /// <returns>The group a position is belonging to.</returns>
        public byte GetGroup(int x, int y)
        {
            //int groupsPerAxis = 2;
            //int tilesPerGroup = this.GridSize/groupsPerAxis;

            //return (byte) (1 + (x/tilesPerGroup) + ((y/tilesPerGroup)*groupsPerAxis));

            return 0;
        }
    }
}
