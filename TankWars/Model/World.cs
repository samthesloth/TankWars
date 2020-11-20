using System;
using System.Collections.Generic;
using System.Text;

namespace TankWars
{
    public class World
    {
        private Dictionary<int, Tank> tanks;
        private Dictionary<int, Projectile> projectiles;
        private Dictionary<int, Wall> walls;
        private Dictionary<int, Beam> beams;
        private Dictionary<int, Powerup> powerups;
        private int size;
        public bool WallsLoaded
        {
            get; private set;
        }

        public World(int size)
        {
            tanks = new Dictionary<int, Tank>();
            projectiles = new Dictionary<int, Projectile>();
            walls = new Dictionary<int, Wall>();
            beams = new Dictionary<int, Beam>();
            powerups = new Dictionary<int, Powerup>();
            this.size = size;
        }

        public int GetSize()
        {
            return size;
        }

        public void UpdateTank(int ID, Vector2D location, Vector2D orientation, Vector2D aiming, string name, int hp, int score, bool died)
        {
            if (tanks.ContainsKey(ID))
            {
                if(died)
                {
                    tanks.Remove(ID);
                    return;
                }
                tanks[ID].UpdateTank(ID, location, orientation, aiming, name, hp, score, died);
            }
            else
                tanks.Add(ID, new Tank(ID, location, orientation, aiming, name, hp, score, died));
        }

        public void UpdateProjectile(int ID, Vector2D location, Vector2D orientation, int owner, bool died)
        {
            if (projectiles.ContainsKey(ID))
            {
                if (died)
                {
                    projectiles.Remove(ID);
                    return;
                }
                projectiles[ID].UpdateProjectile(ID, location, orientation, owner, died);
            }
            else
                projectiles.Add(ID, new Projectile(ID, location, orientation, owner, died));
        }

        public void AddWall (int ID, Vector2D topLeft, Vector2D bottomRight)
        {
            walls.Add(ID, new Wall(ID, topLeft, bottomRight));
        }

        public void AddBeam(int ID, Vector2D origin, Vector2D direction, int owner)
        {
            beams.Add(ID, new Beam(ID, origin, direction, owner));
        }

        public void RemoveBeam(int ID)
        {
            beams.Remove(ID);
        }

        public void UpdatePowerup(int ID, Vector2D location, bool died)
        {
            if (powerups.ContainsKey(ID))
            {
                if (died)
                {
                    projectiles.Remove(ID);
                    return;
                }
                powerups[ID].UpdatePowerup(ID, location, died);
            }
            else
                powerups.Add(ID, new Powerup(ID, location, died));
        }

        public IEnumerable<Tank> GetTanks()
        {
            foreach(Tank t in tanks.Values)
                yield return t;
        }

        public IEnumerable<Projectile> GetProjectiles()
        {
            foreach(Projectile p in projectiles.Values) 
                yield return p;
        }

        public IEnumerable<Wall> GetWalls()
        {
            foreach (Wall w in walls.Values)
                yield return w;
        }

        public IEnumerable<Beam> GetBeams()
        {
            foreach (Beam b in beams.Values)
                yield return b;
        }

        public IEnumerable<Powerup> GetPowerups()
        {
            foreach (Powerup p in powerups.Values)
                yield return p;
        }

        public void LoadWalls()
        {
            WallsLoaded = true;
        }

        public bool GetTank(int ID, out Tank t)
        {
            if(tanks.ContainsKey(ID))
            {
                t = tanks[ID];
                return true;
            }
            else
            {
                t = null;
                return false;
            }
        }
    }
}
