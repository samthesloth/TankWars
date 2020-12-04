using System;
using NetworkUtil;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Diagnostics;
using System.Text;

namespace TankWars
{
    public class Server
    {
        //The game world
        private World world;
        private Dictionary<SocketState, int> users;
        private Dictionary<int, ControlCommand> controls;
        List<Tank> tanks;
        List<Projectile> projs;
        List<Powerup> powerups;
        List<Beam> beams;
        private int wallCount = 1;
        private int projCount = 1;
        private int beamCount = 1;
        private int powerupCount = 1;

        //timers
        private Dictionary<Tank, int> tanksFired;
        private Dictionary<Tank, int> tanksRespawning;
        private int powerUpTimer;

        static void Main(string[] args)
        {
            Stopwatch w = new Stopwatch();
            Console.WriteLine("Console started.");
            Server server = new Server();
            server.readSettings(@"..\..\..\..\Resources\settings.xml");
            Console.WriteLine("Settings loaded.");
            server.startServer();
            Console.WriteLine("Server started.");
            w.Start();
            while (true)
            {
                while (w.ElapsedMilliseconds < server.world.timePerFrame)
                {
                    //wait
                }

                w.Restart();

                server.updateWorld();

                foreach (SocketState s in new List<SocketState>(server.users.Keys))
                {
                    server.sendWorld(s);
                }
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Server()
        {
            users = new Dictionary<SocketState, int>();
            controls = new Dictionary<int, ControlCommand>();
            tanksFired = new Dictionary<Tank, int>();
            tanksRespawning = new Dictionary<Tank, int>();
            powerUpTimer = 0;
        }

        public void startServer()
        {
            Networking.StartServer(FirstContact, 11000);
        }

        private void FirstContact(SocketState state)
        {
            if (state.ErrorOccured)
            {
                return;
            }

            state.OnNetworkAction = sendStartUpInfo;

            lock (state)
            {
                Networking.GetData(state);
            }
        }

        private void sendStartUpInfo(SocketState state)
        {
            StringBuilder temp = new StringBuilder();
            temp.Append(state.ID.ToString() + "\n");
            temp.Append(world.GetSize().ToString() + "\n");
            foreach (Wall w in world.GetWalls())
            {
                temp.Append(JsonConvert.SerializeObject(w) + "\n");
            }

            if (state.ErrorOccured)
            {
                return;
            }

            ProcessMessage(state);
            state.OnNetworkAction = update;
            Networking.Send(state.TheSocket, temp.ToString());

            lock(users)
            {
                users.Add(state, (int)state.ID);
            }

            lock (state)
            {
                Networking.GetData(state);
            }
        }

        private void update(SocketState state)
        {
            if (state.ErrorOccured)
            {
                return;
            }

            ProcessMessage(state);

            lock (state)
            {
                Networking.GetData(state);
            }
        }

        private void sendWorld(SocketState s)
        {
            if (s.ErrorOccured)
            {
                return;
            }

            StringBuilder temp = new StringBuilder();

            foreach (Tank t in tanks)
            {
                temp.Append(JsonConvert.SerializeObject(t) + "\n");
            }
            foreach (Projectile p in projs)
            {
                temp.Append(JsonConvert.SerializeObject(p) + "\n");
            }
            foreach (Powerup p in powerups)
            {
                temp.Append(JsonConvert.SerializeObject(p) + "\n");
            }
            foreach (Beam b in beams)
            {
                temp.Append(JsonConvert.SerializeObject(b) + "\n");
            }

            Networking.Send(s.TheSocket, temp.ToString());
        }


        private bool CheckTankWallCollision(Vector2D position)
        {
            bool temp = false;
            lock(world)
            {
                double x = (double) position.GetX(), y = (double) position.GetY();
                foreach(Wall w in world.GetWalls())
                {
                    Vector2D p1 = w.p1, p2 = w.p2;
                    double topX = Math.Max(p1.GetX() + 55, p2.GetX() + 55);
                    double botX = Math.Min(p1.GetX() - 55, p2.GetX() - 55);
                    double leftY = Math.Min(p1.GetY() - 55, p2.GetY() - 55);
                    double rightY = Math.Max(p1.GetY() + 55, p2.GetY() + 55);
                    if (x > botX && x < topX && y > leftY && y < rightY)
                        temp = true;
                }
            }
            return temp;
        }

        private bool CheckProjectileWallCollision(Vector2D position)
        {
            bool temp = false;
            lock (world)
            {
                double x = (double)position.GetX(), y = (double)position.GetY(), worldsize = world.GetSize() / 2;
                foreach (Wall w in world.GetWalls())
                {
                    Vector2D p1 = w.p1, p2 = w.p2;
                    double topX = Math.Max(p1.GetX() + 25, p2.GetX() + 25);
                    double botX = Math.Min(p1.GetX() - 25, p2.GetX() - 25);
                    double leftY = Math.Min(p1.GetY() - 25, p2.GetY() - 25);
                    double rightY = Math.Max(p1.GetY() + 25, p2.GetY() + 25);
                    if (x > botX && x < topX && y > leftY && y < rightY)
                        temp = true;
                }

                if (x < -worldsize || x > worldsize || y < -worldsize || y > worldsize)
                {
                    temp = true;
                }

            }
            return temp;
        }

        private bool CheckProjectileTankCollision(Vector2D position, out Tank hitTank)
        {
            lock (world)
            {
                foreach (Tank t in world.GetTanks())
                {
                    if ((position - t.location).Length() <= 30)
                    {
                        hitTank = t;
                        return true;
                    }
                }
            }
            hitTank = null;
            return false;
        }

        /// <summary>
        /// Determines if a ray interescts a circle
        /// </summary>
        /// <param name="rayOrig">The origin of the ray</param>
        /// <param name="rayDir">The direction of the ray</param>
        /// <param name="center">The center of the circle</param>
        /// <param name="r">The radius of the circle</param>
        /// <returns></returns>
        public static bool Intersects(Vector2D rayOrig, Vector2D rayDir, Vector2D center, double r)
        {
            // ray-circle intersection test
            // P: hit point
            // ray: P = O + tV
            // circle: (P-C)dot(P-C)-r^2 = 0
            // substituting to solve for t gives a quadratic equation:
            // a = VdotV
            // b = 2(O-C)dotV
            // c = (O-C)dot(O-C)-r^2
            // if the discriminant is negative, miss (no solution for P)
            // otherwise, if both roots are positive, hit

            double a = rayDir.Dot(rayDir);
            double b = ((rayOrig - center) * 2.0).Dot(rayDir);
            double c = (rayOrig - center).Dot(rayOrig - center) - r * r;

            // discriminant
            double disc = b * b - 4.0 * a * c;

            if (disc < 0.0)
                return false;

            // find the signs of the roots
            // technically we should also divide by 2a
            // but all we care about is the sign, not the magnitude
            double root1 = -b + Math.Sqrt(disc);
            double root2 = -b - Math.Sqrt(disc);

            return (root1 > 0.0 && root2 > 0.0);
        }

        private List<Tank> CheckBeamTankCollision(Beam b)
        {
            List<Tank> hitTanks = new List<Tank>();
            lock (world)
            {
                foreach (Tank t in world.GetTanks())
                {
                    if (Intersects(b.origin, b.direction, t.location, 30))
                    {
                        hitTanks.Add(t);
                    }
                }
            }
            return hitTanks;
        }

        private void resetPowerupTimer()
        {
            Random r = new Random();
            powerUpTimer = r.Next(0, world.maxPowerDelay);
        }

        /// <summary>
        /// Returns the wraparound position of tank when needed. Otherwise, returns original position.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private Vector2D TankWraparound(Vector2D position)
        {
            double x = (double)position.GetX(), y = (double)position.GetY(), worldsize = world.GetSize()/2;
            Vector2D temp;
            if(x < 15-worldsize)
            {
                temp = new Vector2D(worldsize - 16, y);
            }
            else if(x > worldsize - 15)
            {
                temp = new Vector2D(16-worldsize, y);
            }
            else if (y < 15 - worldsize)
            {
                temp = new Vector2D(x, worldsize - 16);
            }
            else if (y > worldsize - 15)
            {
                temp = new Vector2D(x, 16-worldsize);
            }
            else
            {
                temp = new Vector2D(x, y);
            }

            return temp;
        }

        private void updateWorld()
        {
            lock (world)
            {
                if(powerUpTimer == 0 && new List<Powerup>(world.GetPowerups()).Count < world.maxPowers)
                {
                    Random r = new Random();
                    Vector2D RandLoc = new Vector2D(r.Next(-world.GetSize() / 2 + 16, world.GetSize() / 2 - 16), r.Next(-world.GetSize() / 2 + 16, world.GetSize() / 2 - 16));
                    while (CheckProjectileWallCollision(RandLoc))
                    {
                        RandLoc = new Vector2D(r.Next(-world.GetSize() / 2 + 16, world.GetSize() / 2 - 16), r.Next(-world.GetSize() / 2 + 16, world.GetSize() / 2 - 16));
                    }
                    world.UpdatePowerup(powerupCount++, RandLoc, false);
                    resetPowerupTimer();
                }
                else if(powerUpTimer != 0)
                {
                    powerUpTimer--;
                }

                foreach (Tank t in new List<Tank>(tanksFired.Keys))
                {
                    int temp = tanksFired[t];
                    if (temp > 1)
                        tanksFired[t]--;
                    else
                        tanksFired.Remove(t);
                }

                foreach (Tank t in new List<Tank>(tanksRespawning.Keys))
                {
                    if (t.died)
                    {
                        world.UpdateTank(t.ID, t.location, t.orientation, t.aiming, t.name, t.hitPoints, t.score, false, t.disconnected);
                    }
                    int temp = tanksRespawning[t];
                    if (temp > 1)
                    {
                        tanksRespawning[t]--;
                    }
                    else
                    {
                        tanksRespawning.Remove(t);
                        Random r = new Random();
                        Vector2D RandLoc = new Vector2D(r.Next(-world.GetSize() / 2 + 16, world.GetSize() / 2 - 16), r.Next(-world.GetSize() / 2 + 16, world.GetSize() / 2 - 16));
                        while(CheckTankWallCollision(RandLoc))
                        {
                            RandLoc = new Vector2D(r.Next(-world.GetSize() / 2 + 16, world.GetSize() / 2 - 16), r.Next(-world.GetSize() / 2 + 16, world.GetSize() / 2 - 16));
                        }
                        world.UpdateTank(t.ID, RandLoc, new Vector2D(0, 1), new Vector2D(0, 1), t.name, 3, t.score, t.died, t.disconnected);
                    }
                }

                foreach (Tank t in new List<Tank>(world.GetTanks()))
                {
                    if(t.disconnected == true)
                    {
                        world.UpdateTank(t.ID, new Vector2D(0, 0), new Vector2D(0, 1), new Vector2D(0, 1), t.name, t.hitPoints, 0, t.died, t.disconnected);
                    }
                }

                foreach(SocketState s in new List<SocketState>(users.Keys))
                {
                    if(s.ErrorOccured)
                    {
                        if (world.GetTank(users[s], out Tank t))
                            t.setDisconnected();
                    }
                }

                foreach (int i in new List<int>(controls.Keys))
                {
                    if (world.GetTank(i, out Tank t))
                    {
                        if (t.hitPoints != 0)
                        {
                            ControlCommand c = controls[i];
                            Vector2D orientation = new Vector2D(1, 0);
                            switch (c.moving)
                            {
                                case "none":
                                    orientation = t.orientation;
                                    t.velocity = new Vector2D(0, 0);
                                    break;

                                case "up":
                                    orientation = new Vector2D(0, -1);
                                    t.velocity = new Vector2D(0, -world.tankSpeed);
                                    break;

                                case "down":
                                    orientation = new Vector2D(0, 1);
                                    t.velocity = new Vector2D(0, world.tankSpeed);
                                    break;

                                case "left":
                                    orientation = new Vector2D(-1, 0);
                                    t.velocity = new Vector2D(-world.tankSpeed, 0);
                                    break;

                                case "right":
                                    orientation = new Vector2D(1, 0);
                                    t.velocity = new Vector2D(world.tankSpeed, 0);
                                    break;
                            }
                            Vector2D loc = t.location + t.velocity;
                            if (CheckTankWallCollision(loc))
                            {
                                loc = t.location;
                            }
                            loc = TankWraparound(loc);
                            orientation.Normalize();
                            c.direction.Normalize();

                            world.UpdateTank(t.ID, loc, orientation, c.direction, t.name, t.hitPoints, t.score, t.died, t.disconnected);

                            if (c.fire == "main" && !tanksFired.ContainsKey(t))
                            {
                                world.UpdateProjectile(projCount++, t.location, c.direction, t.ID, false);
                                tanksFired.Add(t, world.projectileDelay);
                            }
                            else if (c.fire == "alt" && t.beams > 0)
                            {
                                world.AddBeam(beamCount++, t.location, c.direction, t.ID);
                                t.beams--;
                            }

                            controls.Remove(i);
                        }
                    }
                }

                beams = new List<Beam>(world.GetBeams());

                foreach (Beam b in new List<Beam>(world.GetBeams()))
                {
                    List<Tank> temp = CheckBeamTankCollision(b);
                    foreach (Tank hitTank in temp)
                    {
                        if (hitTank.ID != b.owner)
                        {
                            if (world.GetTank(b.owner, out Tank t))
                            {
                                world.UpdateTank(hitTank.ID, hitTank.location, hitTank.orientation, hitTank.aiming, hitTank.name, 0, hitTank.score, true, t.disconnected);
                                tanksRespawning.Add(hitTank, world.respawnTime);
                                t.kill();
                            }
                        }
                    }
                    world.RemoveBeam(b.ID);
                }

                foreach (Projectile p in new List<Projectile>(world.GetProjectiles()))
                {
                    Vector2D loc = p.location + p.orientation*world.projectileSpeed;

                    world.UpdateProjectile(p.ID, loc, p.orientation, p.owner, p.died);

                    if (!p.died)
                    {
                        if (CheckProjectileWallCollision(loc))
                        {
                            p.setDied();
                        }
                        else if (CheckProjectileTankCollision(loc, out Tank hitTank))
                        {
                            if (hitTank.ID != p.owner)
                            {
                                p.setDied();
                                hitTank.hit();
                                if (world.GetTank(p.owner, out Tank t) && hitTank.hitPoints == 0)
                                {
                                    world.UpdateTank(hitTank.ID, hitTank.location, hitTank.orientation, hitTank.aiming, hitTank.name, hitTank.hitPoints, hitTank.score, true, t.disconnected);
                                    tanksRespawning.Add(hitTank, world.respawnTime);
                                    t.kill();
                                }
                            }
                        }
                    }
                    
                }

                foreach (Powerup p in new List<Powerup>(world.GetPowerups()))
                {
                    world.UpdatePowerup(p.ID, p.location, p.died);

                    if (!p.died)
                    {
                        if (CheckProjectileTankCollision(p.location, out Tank hitTank))
                        {
                            p.setDied();
                            hitTank.beams++;
                        }
                    }
                }

                tanks = new List<Tank>(world.GetTanks());
                projs = new List<Projectile>(world.GetProjectiles());
                powerups = new List<Powerup>(world.GetPowerups());
            }
        }

        /// <summary>
        /// Processes the message received from the server. If it is a json object, it updates the world accordingly.
        /// Otherwise, it is either the player id or world size, and thus sets those accordingly.
        /// </summary>
        private void ProcessMessage(SocketState state)
        {
            if (state.ErrorOccured)
            {
                return;
            }

            //Gets data and parts from data
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            foreach (string s in parts)
            {
                //If the part has a length of 0, then it is not a complete message
                if (s.Length <= 0)
                {
                    continue;
                }

                //If the part does not end with newline, then the message is incomplete
                if (s[s.Length - 1] != '\n')
                {
                    break;
                }
                //If the part is a json, deserialize
                if (s[0] == '{')
                {
                    lock (controls)
                    {
                        //Get the json object out of the part
                        JObject obj = JObject.Parse(s);
                        JToken type;

                        //Commands
                        type = obj["moving"];
                        if (type != null)
                        {
                            ControlCommand c = JsonConvert.DeserializeObject<ControlCommand>(s);
                            if (!controls.ContainsKey(users[state]))
                                controls.Add(users[state], c);
                            else
                                controls[users[state]] = c;
                        }
                    }
                }
                //If it is not a json object, then it must be the player's name
                else
                {
                    lock (world)
                    {
                        Random r = new Random();
                        Vector2D RandLoc = new Vector2D(r.Next(-world.GetSize() / 2 + 16, world.GetSize() / 2 - 16), r.Next(-world.GetSize() / 2 + 16, world.GetSize() / 2 - 16));
                        while (CheckTankWallCollision(RandLoc))
                        {
                            RandLoc = new Vector2D(r.Next(-world.GetSize() / 2 + 16, world.GetSize() / 2 - 16), r.Next(-world.GetSize() / 2 + 16, world.GetSize() / 2 - 16));
                        }
                        world.UpdateTank((int)state.ID, RandLoc, new Vector2D(0, 1), new Vector2D(0, 1), s, 3, 0, false, false);
                        
                    }
                }

                lock (state)
                {
                    //Remove the processed part
                    state.RemoveData(0, s.Length);
                }
            }
        }

        public void readSettings(String filename)
        {
            using (XmlReader reader = XmlReader.Create(filename))
            {
                while (reader.Read())
                {
                    while (reader.IsStartElement())
                    {
                        if (reader.Name.Equals("GameSettings"))
                        {
                            reader.Read();
                            continue;
                        }

                        if (reader.Name.Equals("UniverseSize"))
                        {
                            world = new World(reader.ReadElementContentAsInt());
                        }

                        else if (reader.Name.Equals("MSPerFrame"))
                        {
                            world.timePerFrame = reader.ReadElementContentAsInt();
                        }

                        else if (reader.Name.Equals("FramesPerShot"))
                        {
                            world.projectileDelay = reader.ReadElementContentAsInt();
                        }

                        else if (reader.Name.Equals("RespawnRate"))
                        {
                            world.respawnTime = reader.ReadElementContentAsInt();
                        }

                        else if (reader.Name.Equals("ShotSpeed"))
                        {
                            world.projectileSpeed = reader.ReadElementContentAsInt();
                        }

                        else if (reader.Name.Equals("PowerUpRespawnRate"))
                        {
                            world.maxPowerDelay = reader.ReadElementContentAsInt();
                        }

                        else if (reader.Name.Equals("TankSpeed"))
                        {
                            world.tankSpeed = reader.ReadElementContentAsInt();
                        }

                        else if (reader.Name.Equals("MaxPowerUps"))
                        {
                            world.maxPowers = reader.ReadElementContentAsInt();
                        }

                        else if(reader.Name.Equals("Wall"))
                        {
                            int x1 = 0, x2 = 0, y1 = 0, y2 = 0;
                            using (XmlReader reader2 = reader.ReadSubtree())
                            {
                                reader2.Read();
                                while (reader2.Read())
                                {
                                    while (reader2.IsStartElement())
                                    {
                                        
                                        if (reader2.Name.Equals("p1"))
                                        {
                                            using (XmlReader reader3 = reader.ReadSubtree())
                                            {
                                                reader3.Read();
                                                while (reader3.Read())
                                                {
                                                    while (reader3.IsStartElement())
                                                    {
                                                        if (reader3.Name.Equals("x"))
                                                        {
                                                            x1 = reader3.ReadElementContentAsInt();
                                                        }

                                                        if (reader3.Name.Equals("y"))
                                                        {
                                                            y1 = reader3.ReadElementContentAsInt();
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        else if (reader2.Name.Equals("p2"))
                                        {
                                            using (XmlReader reader3 = reader.ReadSubtree())
                                            {
                                                reader3.Read();
                                                while (reader3.Read())
                                                {
                                                    while (reader3.IsStartElement())
                                                    {
                                                        if (reader3.Name.Equals("x"))
                                                        {
                                                            x2 = reader3.ReadElementContentAsInt();
                                                        }

                                                        if (reader3.Name.Equals("y"))
                                                        {
                                                            y2 = reader3.ReadElementContentAsInt();
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            world.AddWall(wallCount++, new Vector2D(x1, y1), new Vector2D(x2, y2));
                        }
                    }
                }
            }
        }
    }
}
