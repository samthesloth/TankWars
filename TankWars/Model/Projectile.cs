// Authors: Nicholas Vaskelis and Sam Peters

using Newtonsoft.Json;

namespace TankWars
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Projectile
    {
        //Property for unique ID
        [JsonProperty(PropertyName = "proj")]
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

        //Property for Vector2D direction
        [JsonProperty(PropertyName = "dir")]
        public Vector2D orientation
        {
            get; private set;
        }

        //Property for if projectile is dead or not
        [JsonProperty(PropertyName = "died")]
        public bool died
        {
            get; private set;
        }

        //Property for tank ID of who shot projectile
        [JsonProperty(PropertyName = "owner")]
        public int owner
        {
            get; private set;
        }

        /// <summary>
        /// Default constructor for projectile
        /// </summary>
        public Projectile()
        {
        }

        /// <summary>
        /// Constructor that sets properties
        /// </summary>
        public Projectile(int ID, Vector2D location, Vector2D orientation, int owner, bool died)
        {
            this.ID = ID;
            this.location = location;
            this.orientation = orientation;
            this.owner = owner;
            this.died = died;
        }

        /// <summary>
        /// Updates properties with parameters
        /// </summary>
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