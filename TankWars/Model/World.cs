// Authors: Nicholas Vaskelis and Sam Peters

using System.Collections.Generic;

namespace TankWars
{
    public class World
    {
        // Dictionaries to hold all of the items in the world
        private Dictionary<int, Tank> tanks;
        private Dictionary<int, Projectile> projectiles;
        private Dictionary<int, Wall> walls;
        private Dictionary<int, Powerup> powerups;

        //Size of world
        private int size;

        //Game Properties
        public int timePerFrame
        {
            get; set;
        }
        public int projectileSpeed
        {
            get; set;
        }
        public int projectileDelay
        {
            get; set;
        }
        public int tankSpeed
        {
            get; set;
        }
        public int respawnTime
        {
            get; set;
        }
        public const int tankSize = 60;
        public const int wallSize = 50;
        public int maxPowers
        {
            get; set;
        }
        public int maxPowerDelay
        {
            get; set;
        }

        //Property to define if walls have been loaded into the game
        public bool WallsLoaded
        {
            get; private set;
        }

        /// <summary>
        /// Constructor for world which takes in a size and initializes instance variables
        /// </summary>
        public World(int size)
        {
            tanks = new Dictionary<int, Tank>();
            projectiles = new Dictionary<int, Projectile>();
            walls = new Dictionary<int, Wall>();
            powerups = new Dictionary<int, Powerup>();
            this.size = size;
        }

        /// <summary>
        /// Returns size of world
        /// </summary>
        public int GetSize()
        {
            return size;
        }

        /// <summary>
        /// Updates, adds, or removes tank from dictionary
        /// </summary>
        public void UpdateTank(int ID, Vector2D location, Vector2D orientation, Vector2D aiming, string name, int hp, int score, bool died, bool disconnected)
        {
            //If tank exists...
            if (tanks.ContainsKey(ID))
            {
                tanks[ID].UpdateTank(ID, location, orientation, aiming, name, hp, score, died, disconnected);
                //If it's disconnected, remove from dictionary
                if (tanks[ID].disconnected)
                {
                    tanks.Remove(ID);
                    return;
                }
            }
            else
            {
                //Otherwise, add the new tank to the dictionary
                tanks.Add(ID, new Tank(ID, location, orientation, aiming, name, hp, score, died, disconnected));
            }
        }

        /// <summary>
        /// Updates, adds, or removes projectile from dictionary
        /// </summary>
        public void UpdateProjectile(int ID, Vector2D location, Vector2D orientation, int owner, bool died)
        {
            //If projectile exists...
            if (projectiles.ContainsKey(ID))
            {
                projectiles[ID].UpdateProjectile(ID, location, orientation, owner, died);
                //If it's dead, remove it from dictionary
                if (died)
                {
                    projectiles.Remove(ID);
                    return;
                }
            }
            else
            {
                //Otherwise, add the new projectile to the dictionary
                projectiles.Add(ID, new Projectile(ID, location, orientation, owner, died));
            }
        }

        /// <summary>
        /// Adds wall to the walls dictionary. Never removed
        /// </summary>
        public void AddWall(int ID, Vector2D p1, Vector2D p2)
        {
            walls.Add(ID, new Wall(ID, p1, p2));
        }

        /// <summary>
        /// Updates, adds, or removes powerup from dictionary
        /// </summary>
        public void UpdatePowerup(int ID, Vector2D location, bool died)
        {
            //If powerup exists...
            if (powerups.ContainsKey(ID))
            {
                powerups[ID].UpdatePowerup(ID, location, died);
                //If died, remove from dictionary
                if (died)
                {
                    powerups.Remove(ID);
                    return;
                }
            }
            else
            {
                //Otherwise, add new powerup to dictionary
                powerups.Add(ID, new Powerup(ID, location, died));
            }
        }

        /// <summary>
        /// Returns IEnumerable of Tanks dictionary
        /// </summary>
        public IEnumerable<Tank> GetTanks()
        {
            foreach (Tank t in tanks.Values)
                yield return t;
        }

        /// <summary>
        /// Returns IEnumerable of Projectiles dictionary
        /// </summary>
        public IEnumerable<Projectile> GetProjectiles()
        {
            foreach (Projectile p in projectiles.Values)
                yield return p;
        }

        /// <summary>
        /// Returns IEnumerable of Walls dictionary
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Wall> GetWalls()
        {
            foreach (Wall w in walls.Values)
                yield return w;
        }

        /// <summary>
        /// Returns IEnumerable of Powerups dictionary
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Powerup> GetPowerups()
        {
            foreach (Powerup p in powerups.Values)
                yield return p;
        }

        /// <summary>
        /// Called when all walls have been received
        /// </summary>
        public void LoadWalls()
        {
            WallsLoaded = true;
        }

        /// <summary>
        /// Returns true if tank is in dictionary. Also sets out t to that tank if it exists
        /// </summary>
        public bool GetTank(int ID, out Tank t)
        {
            //If it exists, set it and return true
            if (tanks.ContainsKey(ID))
            {
                t = tanks[ID];
                return true;
            }
            //Otherwise, set t to null and return false
            else
            {
                t = null;
                return false;
            }
        }
    }
}