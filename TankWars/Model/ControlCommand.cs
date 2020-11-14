using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace Model
{

    [JsonObject(MemberSerialization.OptIn)]
    public class ControlCommand
    {
        [JsonProperty(PropertyName = "moving")]
        public string moving
        {
            get;
        }

        [JsonProperty(PropertyName = "fire")]
        public string fire
        {
            get;
        }

        [JsonProperty(PropertyName = "tdir")]
        public Vector2D direction
        {
            get;
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
