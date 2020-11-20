// Authors: Nicholas Vaskelis and Sam Peters

using Newtonsoft.Json;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class ControlCommand
    {
        //Property for direction player is moving (or none)
        [JsonProperty(PropertyName = "moving")]
        public string moving
        {
            get; private set;
        }

        //Property for type of fire (or none)
        [JsonProperty(PropertyName = "fire")]
        public string fire
        {
            get; private set;
        }

        //Property for Vector2D aiming direction
        [JsonProperty(PropertyName = "tdir")]
        public Vector2D direction
        {
            get; private set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ControlCommand()
        {
        }

        /// <summary>
        /// Constructor that sets properties
        /// </summary>
        public ControlCommand(string moving, string fire, Vector2D direction)
        {
            this.moving = moving;
            this.fire = fire;
            this.direction = direction;
        }
    }
}