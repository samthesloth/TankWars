using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace Model
{

    [JsonObject(MemberSerialization.OptIn)]
    class Beam
    {
        [JsonProperty(PropertyName = "beam")]
        private int ID;

        [JsonProperty(PropertyName = "org")]
        private Vector2D origin;

        [JsonProperty(PropertyName = "dir")]
        private Vector2D direction;

        [JsonProperty(PropertyName = "owner")]
        private int owner;

        public Beam()
        {

        }

        public Beam(int ID, Vector2D origin, Vector2D direction, int owner)
        {
            this.ID = ID;
            this.origin = origin;
            this.direction = direction;
            this.owner = owner;
        }
    }
}
