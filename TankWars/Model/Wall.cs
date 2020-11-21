// Authors: Nicholas Vaskelis and Sam Peters

using Newtonsoft.Json;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Wall
    {
        //Property for unique ID
        [JsonProperty(PropertyName = "wall")]
        public int ID
        {
            get; private set;
        }

        //Property for first point
        [JsonProperty(PropertyName = "p1")]
        public Vector2D p1
        {
            get; private set;
        }

        //Property for second point
        [JsonProperty(PropertyName = "p2")]
        public Vector2D p2
        {
            get; private set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Wall()
        {
        }

        /// <summary>
        /// Constructor that sets properties
        /// </summary>
        public Wall(int ID, Vector2D p1, Vector2D p2)
        {
            this.ID = ID;
            this.p1 = p1;
            this.p2 = p2;
        }
    }
}