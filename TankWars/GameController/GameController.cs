// Authors: Nicholas Vaskelis and Sam Peters

using NetworkUtil;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text.RegularExpressions;

namespace TankWars
{
    public class GameController
    {
        //The player's name that was given by user
        private string PlayerName;

        //Player ID that server gives client
        private int PlayerID = -1;

        //The game world
        private World world;

        //Control command values
        private string movement = "none";
        private string fire = "none";
        private Vector2D aiming = new Vector2D(0, -1);

        //The socket state for the server connection
        private SocketState server;

        //Creates delegates and events to be used to communicate with View
        public delegate void UpdateHandler();
        public event UpdateHandler OnUpdate;
        public event UpdateHandler IDLoaded;
        public event UpdateHandler WorldLoaded;
        public delegate void BeamHandler(Beam b);
        public event BeamHandler BeamFired;

        //Boolean properties for if a key/mouse button is pushed or not
        public bool A
        {
            get; set;
        }
        public bool W
        {
            get; set;
        }
        public bool S
        {
            get; set;
        }
        public bool D
        {
            get; set;
        }
        public bool LMB
        {
            get; set;
        }
        public bool RMB
        {
            get; set;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public GameController()
        {
        }

        /// <summary>
        /// Begins connection with host.
        /// </summary>
        public void Connect(string hostName, string playerName)
        {
            PlayerName = playerName;
            Networking.ConnectToServer(FirstContact, hostName, 11000);
        }

        /// <summary>
        /// Callback for Connect. Sets network action, sets server, sends playername, and finally starts receive
        /// </summary>
        private void FirstContact(SocketState state)
        {
            if(state.ErrorOccured)
            {
                return;
            }
            state.OnNetworkAction = ReceiveStartup;
            server = state;
            Networking.Send(state.TheSocket, PlayerName + "\n");
            lock (state)
            {
                Networking.GetData(state);
            }
        }

        /// <summary>
        /// After receiving initial info, processes the messages and changes network action to ReceiveWorld. Finally, begins receive again
        /// </summary>
        private void ReceiveStartup(SocketState state)
        {
            if (state.ErrorOccured)
            {
                return;
            }

            ProcessMessage(state);
            state.OnNetworkAction = ReceiveWorld;
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

                        //Wall is not loaded
                        if (!world.WallsLoaded)
                        {
                            type = obj["wall"];
                            if (type != null)
                            {
                                Wall w = JsonConvert.DeserializeObject<Wall>(s);
                                world.AddWall(w.ID, w.p1, w.p2);
                            }
                            else
                            {
                                //As soon as we reach a JSON that isn't a wall, the walls are loaded
                                world.LoadWalls();
                                WorldLoaded();
                            }
                        }

                        //If it a tank, update the world
                        type = obj["tank"];
                        if (type != null)
                        {
                            Tank t = JsonConvert.DeserializeObject<Tank>(s);
                            world.UpdateTank(t.ID, t.location, t.orientation, t.aiming, t.name, t.hitPoints, t.score, t.died, t.disconnected);
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
                            BeamFired(b);
                        }
                    }
                }
                //If it is not a json object, then it must be the world size or player id
                else
                {
                    //If player id is not set, then the part is the player id
                    if (PlayerID < 0)
                    {
                        PlayerID = Int32.Parse(s);
                        IDLoaded();
                    }
                    //Otherwise, the part must be the world
                    else
                    {
                        world = new World(Int32.Parse(s));
                    }
                }

                lock (state)
                {
                    //Remove the processed part
                    state.RemoveData(0, s.Length);
                }

                //If OnUpdate is set, call it
                if (OnUpdate != null)
                {
                    OnUpdate();
                }
            }
        }

        /// <summary>
        /// Final callback to be repeated. Processes messages and then begine receive again
        /// </summary>
        private void ReceiveWorld(SocketState state)
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
        /// Changes movement instance variable
        /// </summary>
        public void Move()
        {
            if (W)
                movement = "up";
            else if (S)
                movement = "down";
            else if (A)
                movement = "left";
            else if (D)
                movement = "right";
            else
                movement = "none";
        }

        /// <summary>
        /// Changes fire instance variable
        /// </summary>
        public void Fire()
        {
            if (LMB)
                fire = "main";
            else if (RMB)
                fire = "alt";
            else
                fire = "none";
        }

        /// <summary>
        /// Changes aiming instance variable
        /// </summary>
        public void Aiming(Vector2D aiming)
        {
            this.aiming = aiming;
        }

        /// <summary>
        /// Sends control command using movement, fire, and aiming
        /// </summary>
        public void Send()
        {
            Move();
            Fire();
            ControlCommand cc = new ControlCommand(movement, fire, aiming);
            string command = JsonConvert.SerializeObject(cc) + "\n";
            Networking.Send(server.TheSocket, command);
        }

        /// <summary>
        /// Sends and closes in order to close socket
        /// </summary>
        public void Close()
        {
            if (!(server is null))
            {
                Networking.SendAndClose(server.TheSocket, "");
            }
        }

        /// <summary>
        /// Returns world
        /// </summary>
        public World GetWorld()
        {
            return world;
        }

        /// <summary>
        /// Returns PlayerID
        /// </summary>
        public int GetPlayerID()
        {
            return PlayerID;
        }
    }
}