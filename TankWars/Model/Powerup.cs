// Authors: Nicholas Vaskelis and Sam Peters

using Newtonsoft.Json;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Powerup
    {
        //Property for unique ID
        [JsonProperty(PropertyName = "power")]
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

        //Property for if powerup is dead or not
        [JsonProperty(PropertyName = "died")]
        public bool died
        {
            get; private set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Powerup()
        {
        }

        /// <summary>
        /// Constructor for setting properties
        /// </summary>
        public Powerup(int ID, Vector2D location, bool died)
        {
            this.ID = ID;
            this.location = location;
            this.died = died;
        }

        /// <summary>
        /// Updates properties of powerup
        /// </summary>
        public void UpdatePowerup(int ID, Vector2D location, bool died)
        {
            this.ID = ID;
            this.location = location;
            this.died = died;
        }
    }
}