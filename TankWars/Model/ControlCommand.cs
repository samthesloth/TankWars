using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TankWars
{

    [JsonObject(MemberSerialization.OptIn)]
    public class ControlCommand
    {
        [JsonProperty(PropertyName = "moving")]
        public string moving
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "fire")]
        public string fire
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "tdir")]
        public Vector2D direction
        {
            get; private set;
        }

        public ControlCommand()
        {

        }

        public ControlCommand(string moving, string fire, Vector2D direction)
        {
            this.moving = moving;
            this.fire = fire;
            this.direction = direction;
        }
    }
}
