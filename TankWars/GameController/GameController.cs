using Newtonsoft.Json;
using System;
using NetworkUtil;
using System.Text.RegularExpressions;

namespace GameController
{
    public class GameController
    {
        private string PlayerName;
        private int PlayerID;

        private void Connect(string hostName, string playerName)
        {
            PlayerName = playerName;
            Networking.ConnectToServer(FirstContact, hostName, 11000);
        }

        private void FirstContact(SocketState state)
        {
            state.OnNetworkAction = ReceiveStartup;
            Networking.Send(state.TheSocket, PlayerName);
            Networking.GetData(state);
        }

        private void ReceiveStartup(SocketState state)
        {
            ProcessMessage(state);
            state.OnNetworkAction = ReceiveWorld;
            Networking.GetData(state);
        }

        private void ProcessMessage(SocketState state)
        {
            string totalData = state.GetData();
            string[] parts = Regex.Split(totalData, @"(?<=[\n])");
            
            foreach(string s in parts)
            {
                if(parts.Length == 0)
                {
                    continue;
                }

                if(s[s.Length - 1] != '\n')
                {
                    break;
                }

                if(s[0] == '{')
                {
                    //json object
                }
                else
                {
                    //integers (world size and id)
                }

                state.RemoveData(0, s.Length);
            }
        }

        private void ReceiveWorld (SocketState state)
        {
            string[] data = state.GetData().Split('\n');
        }
    }
}
