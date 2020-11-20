using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TankWars
{

    [JsonObject(MemberSerialization.OptIn)]
    public class Beam
    {
        [JsonProperty(PropertyName = "beam")]
        public int ID
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "org")]
        public Vector2D origin
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "dir")]
        public Vector2D direction
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "owner")]
        public int owner
        {
            get; private set;
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
