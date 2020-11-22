// Authors: Nicholas Vaskelis and Sam Peters

using Newtonsoft.Json;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Tank
    {
        //Property for tank's unique ID
        [JsonProperty(PropertyName = "tank")]
        public int ID
        {
            get; private set;
        }

        //Property for Vector2D location
        [JsonProperty(PropertyName = "loc")]
        public Vector2D location
        {
            get; private set;
        }

        //Property for Vector2D orientation
        [JsonProperty(PropertyName = "bdir")]
        public Vector2D orientation
        {
            get; private set;
        }

        //Property for Vector2D aiming
        [JsonProperty(PropertyName = "tdir")]
        public Vector2D aiming
        {
            get; private set;
        }

        //Property for tank's name
        [JsonProperty(PropertyName = "name")]
        public string name
        {
            get; private set;
        }

        //Property for tank's hp
        [JsonProperty(PropertyName = "hp")]
        public int hitPoints
        {
            get; private set;
        }

        //Property for tank's score
        [JsonProperty(PropertyName = "score")]
        public int score
        {
            get; private set;
        }

        //Property for if the tank is dead
        [JsonProperty(PropertyName = "died")]
        public bool died
        {
            get; private set;
        }

        //Property for if the tank is connected
        [JsonProperty(PropertyName = "dc")]
        public bool disconnected
        {
            get; private set;
        }

        //Propterty for if the tank is joined
        [JsonProperty(PropertyName = "join")]
        public bool joined
        {
            get; private set;
        }

        /// <summary>
        /// Default constructor for tank
        /// </summary>
        public Tank()
        {
        }

        /// <summary>
        /// Constructor for tank that sets properties
        /// </summary>
        public Tank(int ID, Vector2D location, Vector2D orientation, Vector2D aiming, string name, int hp, int score, bool died, bool disconnected)
        {
            this.ID = ID;
            this.location = location;
            this.orientation = orientation;
            this.name = name;
            this.aiming = aiming;
            this.hitPoints = hp;
            this.score = score;
            this.died = died;
            this.disconnected = disconnected;
        }

        /// <summary>
        /// Updates the tank's properties
        /// </summary>
        public void UpdateTank(int ID, Vector2D location, Vector2D orientation, Vector2D aiming, string name, int hp, int score, bool died, bool disconnected)
        {
            this.ID = ID;
            this.location = location;
            this.orientation = orientation;
            this.name = name;
            this.aiming = aiming;
            this.hitPoints = hp;
            this.score = score;
            this.died = died;
            this.disconnected = disconnected;
        }
    }
}