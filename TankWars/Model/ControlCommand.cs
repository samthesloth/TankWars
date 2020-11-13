using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace Model
{

    [JsonObject(MemberSerialization.OptIn)]
    class ControlCommand
    {
        [JsonProperty(PropertyName = "moving")]
        private string moving;

        [JsonProperty(PropertyName = "fire")]
        private string fire;

        [JsonProperty(PropertyName = "tdir")]
        private Vector2D direction;

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
