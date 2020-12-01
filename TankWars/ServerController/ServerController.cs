using System;
using NetworkUtil;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace TankWars
{
    public class ServerController
    {
        //The game world
        private World world;

        private Dictionary<SocketState, int> users;
        private int userCount = 1;

        /// <summary>
        /// Default constructor
        /// </summary>
        public ServerController()
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

            if(users.ContainsKey(state))
            {
                Networking.Send(state.TheSocket, users[state].ToString());
                Networking.Send(state.TheSocket, world.GetSize().ToString());
                state.OnNetworkAction = update;
                lock (state)
                {
                    Networking.GetData(state);
                }
            }
            else
            {

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
                    lock(world)
                    {
                        users.Add(state, userCount);
                        //Change position to random
                        world.UpdateTank(userCount++, new Vector2D(0,0), new Vector2D(0, 0), new Vector2D(0, 0), s, 3, 0, false, false);
                    }
                }

                lock (state)
                {
                    //Remove the processed part
                    state.RemoveData(0, s.Length);
                }
            }
        }





        public void readSettings()
        {

        }
    }
}
