using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TankWars
{

    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {
        [JsonProperty(PropertyName = "tank")]
        public int ID
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "loc")]
        public Vector2D location
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "bdir")]
        public Vector2D orientation
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "tdir")]
        public Vector2D aiming
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "name")]
        public string name
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "hp")]
        public int hitPoints
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "score")]
        public int score
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "died")]
        public bool died
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "dc")]
        public bool disconnected
        {
            get; private set;
        }

        [JsonProperty(PropertyName = "join")]
        public bool joined
        {
            get; private set;
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
