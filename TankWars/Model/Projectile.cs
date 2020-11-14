using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace Model
{

    [JsonObject(MemberSerialization.OptIn)]
    public class Projectile
    {
        [JsonProperty(PropertyName = "proj")]
        public int ID
        {
            get;
        }

        [JsonProperty(PropertyName = "loc")]
        public Vector2D location
        {
            get;
        }

        [JsonProperty(PropertyName = "dir")]
        public Vector2D orientation
        {
            get;
        }

        [JsonProperty(PropertyName = "died")]
        public bool died
        {
            get;
        }

        [JsonProperty(PropertyName = "owner")]
        public int owner
        {
            get;
        }

        public Projectile()
        {

        }

        public Projectile(int ID, Vector2D location, Vector2D orientation, int owner, bool died)
        {
            this.ID = ID;
            this.location = location;
            this.orientation = orientation;
            this.owner = owner;
            this.died = died;
        }

        public void UpdateProjectile(int ID, Vector2D location, Vector2D orientation, int owner, bool died)
        {
            this.ID = ID;
            this.location = location;
            this.orientation = orientation;
            this.owner = owner;
            this.died = died;
        }
    }
}
