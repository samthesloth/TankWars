using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace Model
{

    [JsonObject(MemberSerialization.OptIn)]
    class Projectile
    {
        [JsonProperty(PropertyName = "proj")]
        private int ID;

        [JsonProperty(PropertyName = "loc")]
        private Vector2D location;

        [JsonProperty(PropertyName = "dir")]
        private Vector2D orientation;

        [JsonProperty(PropertyName = "died")]
        private bool died = false;

        [JsonProperty(PropertyName = "owner")]
        private int owner;

        public Projectile()
        {

        }

        public Projectile(int ID, Vector2D location, Vector2D orientation, int owner)
        {
            this.ID = ID;
            this.location = location;
            this.orientation = orientation;
            this.owner = owner;
        }
    }
}
