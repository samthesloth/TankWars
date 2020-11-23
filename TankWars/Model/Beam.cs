// Authors: Nicholas Vaskelis and Sam Peters

using Newtonsoft.Json;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Beam
    {
        //Property for unique ID
        [JsonProperty(PropertyName = "beam")]
        public int ID
        {
            get; private set;
        }

        //Property for Vector2D origin
        [JsonProperty(PropertyName = "org")]
        public Vector2D origin
        {
            get; private set;
        }

        //Property for Vector2D direction
        [JsonProperty(PropertyName = "dir")]
        public Vector2D direction
        {
            get; private set;
        }

        //Property for tank ID of who shot the beam
        [JsonProperty(PropertyName = "owner")]
        public int owner
        {
            get; private set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Beam()
        {
        }

        /// <summary>
        /// Constructor that sets properties
        /// </summary>
        public Beam(int ID, Vector2D origin, Vector2D direction, int owner)
        {
            this.ID = ID;
            this.origin = origin;
            this.direction = direction;
            this.owner = owner;
        }
    }
}