using Newtonsoft.Json;
using System;
using NetworkUtil;
using System.Text.RegularExpressions;
using Model;
using Newtonsoft.Json.Linq;
using TankWars;

namespace GameController
{
    public class GameController
    {
        private string PlayerName;
        private int PlayerID = -1;
        private World world;
        private string movement;
        private string fire;
        private Vector2D direction;
        private SocketState server;

        private void Connect(string hostName, string playerName)
        {
            PlayerName = playerName;
            Networking.ConnectToServer(FirstContact, hostName, 11000);
        }

        private void FirstContact(SocketState state)
        {
            state.OnNetworkAction = ReceiveStartup;
            server = state;
            Networking.Send(state.TheSocket, PlayerName);
            Networking.GetData(state);
        }

        private void ReceiveStartup(SocketState state)
        {
            ProcessMessage(state);
            state.OnNetworkAction = ReceiveWorld;
            Networking.GetData(state);
        }

        /// <summary>
        /// Processed the message received from the server. If it is a json object, it updates the world accordingly.
        /// Otherwise, it is either the player id or world size, and thus sets those accordingly. 
        /// </summary>
        private void ProcessMessage(SocketState state)
        {
            //Gets data and parts from data
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");

            foreach (string s in parts)
            {
                //If the part has a length of 0, then it is not a complete message
                if (parts.Length == 0)
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
                    //Get the json object out of the part
                    JObject obj = JObject.Parse(s);
                    JToken type;

                    //Wall
                    if (!world.WallsLoaded)
                    {
                        type = obj["wall"];
                        if (type != null)
                        {
                            Wall w = JsonConvert.DeserializeObject<Wall>(s);
                            world.AddWall(w.ID, w.topLeft, w.bottomRight);
                        }
                        else
                            world.LoadWalls();
                    }

                    //If it a tank, update the world
                    type = obj["tank"];
                    if (type != null)
                    {
                        Tank t = JsonConvert.DeserializeObject<Tank>(s);
                        world.UpdateTank(t.ID, t.location, t.orientation, t.aiming, t.name, t.hitPoints, t.score, t.died);
                    }

                    //Projectile
                    type = obj["proj"];
                    if (type != null)
                    {
                        Projectile p = JsonConvert.DeserializeObject<Projectile>(s);
                        world.UpdateProjectile(p.ID, p.location, p.orientation, p.owner, p.died);
                    }

                    //Powerup
                    type = obj["power"];
                    if (type != null)
                    {
                        Powerup p = JsonConvert.DeserializeObject<Powerup>(s);
                        world.UpdatePowerup(p.ID, p.location, p.died);
                    }

                    //Beam
                    type = obj["beam"];
                    if (type != null)
                    {
                        Beam b = JsonConvert.DeserializeObject<Beam>(s);
                        world.AddBeam(b.ID, b.origin, b.direction, b.owner);
                    }
                }
                //If it is not a json object, then it must be the world size or player id
                else
                {
                    //If player id is not set, then the part is the player id
                    if (PlayerID < 0)
                    {
                        PlayerID = Int32.Parse(s);
                    }
                    //Otherwise, the part must be the world
                    else
                    {
                        world = new World(Int32.Parse(s));
                    }
                }
                //Remove the processed part
                state.RemoveData(0, s.Length);
            }
        }

        private void ReceiveWorld(SocketState state)
        {
            ProcessMessage(state);
            Networking.GetData(state);
        }

        public void Move(string movement)
        {
            this.movement = movement;
        }

        public void Fire(string fire)
        {
            this.fire = fire;
        }

        public void Direction(Vector2D direction)
        {
            this.direction = direction;
        }

        public void Send()
        {
            ControlCommand cc = new ControlCommand(movement, fire, direction);
            string command = JsonConvert.SerializeObject(cc);
            Networking.Send(server.TheSocket, command);
        }
    }
}
