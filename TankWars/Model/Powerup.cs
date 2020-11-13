using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace Model
{

    [JsonObject(MemberSerialization.OptIn)]
    class Powerup
    {
        [JsonProperty(PropertyName = "power")]
        private int ID;

        [JsonProperty(PropertyName = "loc")]
        private Vector2D location;

        [JsonProperty(PropertyName = "died")]
        private bool died = false;

        public Powerup()
        {

        }

        public Powerup(int ID, Vector2D location)
        {
            this.ID = ID;
            this.location = location;
        }
    }
}
