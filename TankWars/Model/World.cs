using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    class World
    {
        private Dictionary<int, Tank> tanks;
        private Dictionary<int, Projectile> projectiles;
        private Dictionary<int, Wall> walls;
        private Dictionary<int, Beam> beams;
        private Dictionary<int, Powerup> powerups;

        public World()
        {
            tanks = new Dictionary<int, Tank>();
            projectiles = new Dictionary<int, Projectile>();
            walls = new Dictionary<int, Wall>();
            beams = new Dictionary<int, Beam>();
            powerups = new Dictionary<int, Powerup>();
        }

        public Dictionary<int, Tank> GetTanks()
        {
            return tanks;
        }

        public Dictionary<int, Projectile> GetProjectiles()
        {
            return projectiles;
        }

        public Dictionary<int, Wall> GetWals()
        {
            return walls;
        }

        public Dictionary<int, Beam> GetBeams()
        {
            return beams;
        }

        public Dictionary<int, Powerup> GetPowerups()
        {
            return powerups;
        }
    }
}
