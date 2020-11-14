using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace Model
{

    [JsonObject(MemberSerialization.OptIn)]
    public class Beam
    {
        [JsonProperty(PropertyName = "beam")]
        public int ID
        {
            get;
        }

        [JsonProperty(PropertyName = "org")]
        public Vector2D origin
        {
            get;
        }

        [JsonProperty(PropertyName = "dir")]
        public Vector2D direction
        {
            get;
        }

        [JsonProperty(PropertyName = "owner")]
        public int owner
        {
            get;
        }

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
