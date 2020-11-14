using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace Model
{

    [JsonObject(MemberSerialization.OptIn)]
    public class Powerup
    {
        [JsonProperty(PropertyName = "power")]
        public int ID
        {
            get;
        }

        [JsonProperty(PropertyName = "loc")]
        public Vector2D location
        {
            get;
        }

        [JsonProperty(PropertyName = "died")]
        public bool died
        {
            get;
        }

        public Powerup()
        {

        }

        public Powerup(int ID, Vector2D location, bool died)
        {
            this.ID = ID;
            this.location = location;
            this.died = died;
        }

        public void UpdatePowerup(int ID, Vector2D location, bool died)
        {
            this.ID = ID;
            this.location = location;
            this.died = died;
        }
    }
}
