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

        private Dictionary<SocketState, int> users;
        private int userCount = 1;

        static void Main(string[] args)
        {
            Server server = new Server();
            server.readSettings("test");
            server.startServer();
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Server()
        {
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
                    lock (world)
                    {
                        //Get the json object out of the part
                        JObject obj = JObject.Parse(s);
                        JToken type;

                        //Commands
                        type = obj["controlcommand"];
                        if (type != null)
                        {
                            ControlCommand c = JsonConvert.DeserializeObject<ControlCommand>(s);
                            //do thing
                        }
                    }
                }
                //If it is not a json object, then it must be the player's name
                else
                {
                    lock (world)
                    {
                        users.Add(state, userCount);
                        //Change position to random
                        world.UpdateTank(userCount++, new Vector2D(0, 0), new Vector2D(0, 0), new Vector2D(0, 0), s, 3, 0, false, false);
                    }
                    Networking.Send(state.TheSocket, users[state].ToString());
                    Networking.Send(state.TheSocket, world.GetSize().ToString());
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
                            continue;
                        }

                        if (reader.Name.Equals("UniverseSize"))
                        {
                            world = new World(reader.ReadContentAsInt());
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
                            using (XmlReader reader2 = reader.ReadSubtree())
                            {

                            }
                        }
                    }
                }
            }
        }
    }
}
