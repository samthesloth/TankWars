using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TankWars;

namespace Model
{

    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {
        [JsonProperty(PropertyName = "tank")]
        public int ID
        {
            get;
        }

        [JsonProperty(PropertyName = "loc")]
        public Vector2D location
        {
            get;
        }

        [JsonProperty(PropertyName = "bdir")]
        public Vector2D orientation
        {
            get;
        }

        [JsonProperty(PropertyName = "tdir")]
        public Vector2D aiming
        {
            get;
        }

        [JsonProperty(PropertyName = "name")]
        public string name
        {
            get;
        }

        [JsonProperty(PropertyName = "hp")]
        public int hitPoints
        {
            get;
        }

        [JsonProperty(PropertyName = "score")]
        public int score
        {
            get;
        }

        [JsonProperty(PropertyName = "died")]
        public bool died
        {
            get;
        }

        [JsonProperty(PropertyName = "dc")]
        public bool disconnected
        {
            get;
        }

        [JsonProperty(PropertyName = "join")]
        public bool joined
        {
            get;
        }

        public Tank()
        {

        }

        public Tank(int ID, Vector2D location, Vector2D orientation, Vector2D aiming, string name, int hp, int score, bool died)
        {
            this.ID = ID;
            this.location = location;
            this.orientation = orientation;
            this.name = name;
            this.aiming = aiming;
            this.hitPoints = hp;
            this.score = score;
            this.died = died;
        }

        public void UpdateTank(int ID, Vector2D location, Vector2D orientation, Vector2D aiming, string name, int hp, int score, bool died)
        {
            this.ID = ID;
            this.location = location;
            this.orientation = orientation;
            this.name = name;
            this.aiming = aiming;
            this.hitPoints = hp;
            this.score = score;
            this.died = died;
        }
    }
}
