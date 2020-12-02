using System;
using NetworkUtil;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace TankWars
{
    public class Server
    {
        //The game world
        private World world;
        private Dictionary<SocketState, Tuple<int, string>> users;
        private Dictionary<SocketState, ControlCommand> controls;
        List<Tank> tanks;
        List<Projectile> projs;
        List<Powerup> powerups;
        List<Beam> beams;
        private int userCount = 1;
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
            Console.WriteLine("Console started.");
            Server server = new Server();
            server.readSettings(@"..\..\..\..\Resources\settings.xml");
            Console.WriteLine("Settings loaded.");
            server.startServer();
            Console.WriteLine("Server started.");
            while (true)
            {
                server.updateWorld();

                List<SocketState> temp = new List<SocketState>(server.users.Keys);
                foreach (SocketState s in temp)
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
            users = new Dictionary<SocketState, Tuple<int, string>>();
            controls = new Dictionary<SocketState, ControlCommand>();
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
            if (state.ErrorOccured)
            {
                return;
            }

            ProcessMessage(state);
            state.OnNetworkAction = update;

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
            foreach(Tank t in tanks)
            {
                Networking.Send(s.TheSocket, JsonConvert.SerializeObject(t) + "\n");
            }
            foreach (Projectile p in projs)
            {
                Networking.Send(s.TheSocket, JsonConvert.SerializeObject(p) + "\n");
            }
            foreach (Powerup p in powerups)
            {
                Networking.Send(s.TheSocket, JsonConvert.SerializeObject(p) + "\n");
            }
            foreach (Beam b in beams)
            {
                Networking.Send(s.TheSocket, JsonConvert.SerializeObject(b) + "\n");
            }
        }

        private void updateWorld()
        {
            lock (world)
            {
                foreach (SocketState s in users.Keys)
                {
                    if(s.ErrorOccured)
                    {
                        world.UpdateTank(users[s].Item1, new Vector2D(0, 0), new Vector2D(0, 0), new Vector2D(0, 0), users[s].Item2, 0, 0, true, true);
                    }



                    if(!world.GetTank(users[s].Item1, out Tank t))
                    {
                        //Change position to random
                        world.UpdateTank(users[s].Item1, new Vector2D(0, 0), new Vector2D(0, 0), new Vector2D(0, 0), users[s].Item2, 3, 0, false, false);
                    }
                    else if(!tanksRespawning.ContainsKey(t) && t.hitPoints == 0)
                    {
                        //Change position to random
                        world.UpdateTank(users[s].Item1, new Vector2D(0, 0), new Vector2D(0, 0), new Vector2D(0, 0), users[s].Item2, 3, 0, false, false);
                    }
                }

                foreach (SocketState s in controls.Keys)
                {
                    world.GetTank(users[s].Item1, out Tank t);
                    if (t.hitPoints != 0)
                    {
                        ControlCommand c = controls[s];
                        Vector2D orientation = new Vector2D(0, 0); ;
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
                        world.UpdateTank(t.ID, t.location + t.velocity, orientation, c.direction, t.name, t.hitPoints, t.score, t.died, t.disconnected);

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
                    }
                }

                //foreach (Projectile p in projs)
                //{
                    
                //}
                //foreach (Powerup p in powerups)
                //{
                    
                //}
                //foreach (Beam b in beams)
                //{
                    
                //}

                foreach (Tank t in tanksFired.Keys)
                {
                    int temp = tanksFired[t];
                    if (temp > 1)
                        tanksFired[t]--;
                    else
                        tanksFired.Remove(t);
                }

                foreach (Tank t in tanksRespawning.Keys)
                {
                    int temp = tanksRespawning[t];
                    if (temp > 1)
                    {
                        tanksRespawning[t]--;
                    }
                    else
                    {
                        tanksRespawning.Remove(t);
                    }
                }

                foreach (Tank t in tanksRespawning.Keys)
                {
                    int temp = tanksRespawning[t];
                    if (temp > 1)
                        tanksRespawning[t]--;
                    else
                        tanksRespawning.Remove(t);
                }

                tanks = new List<Tank>(world.GetTanks());
                projs = new List<Projectile>(world.GetProjectiles());
                powerups = new List<Powerup>(world.GetPowerups());
                beams = new List<Beam>(world.GetBeams());
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
                        type = obj["controlcommand"];
                        if (type != null)
                        {
                            ControlCommand c = JsonConvert.DeserializeObject<ControlCommand>(s);
                            controls.Add(state, c);
                        }
                    }
                }
                //If it is not a json object, then it must be the player's name
                else
                {
                    lock (users)
                    {
                        users.Add(state, new Tuple<int, string>(userCount++, s));
                    }
                    Networking.Send(state.TheSocket, users[state].ToString() + "\n");
                    Networking.Send(state.TheSocket, world.GetSize().ToString() + "\n");
                    foreach (Wall w in world.GetWalls())
                    {
                        Networking.Send(state.TheSocket, JsonConvert.SerializeObject(w) + "\n");
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
