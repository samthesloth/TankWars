using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetworkUtil
{

    public static class Networking
    {
        /////////////////////////////////////////////////////////////////////////////////////////
        // Server-Side Code
        /////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Starts a TcpListener on the specified port and starts an event-loop to accept new clients.
        /// The event-loop is started with BeginAcceptSocket and uses AcceptNewClient as the callback.
        /// AcceptNewClient will continue the event-loop.
        /// </summary>
        /// <param name="toCall">The method to call when a new connection is made</param>
        /// <param name="port">The the port to listen on</param>
        public static TcpListener StartServer(Action<SocketState> toCall, int port)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, port);
            Tuple<Action<SocketState>, TcpListener> temp = new Tuple<Action<SocketState>, TcpListener>(toCall, listener);

            try
            {
                listener.Start();
                listener.BeginAcceptSocket(AcceptNewClient, temp);
            }

            catch
            {
                SocketState ss = new SocketState(toCall, null);
                ss.ErrorOccured = true;
                ss.ErrorMessage = "Connection Failed";
                ss.OnNetworkAction(ss);
            }

            return listener;
        }

        /// <summary>
        /// To be used as the callback for accepting a new client that was initiated by StartServer, and 
        /// continues an event-loop to accept additional clients.
        ///
        /// Uses EndAcceptSocket to finalize the connection and create a new SocketState. The SocketState's
        /// OnNetworkAction should be set to the delegate that was passed to StartServer.
        /// Then invokes the OnNetworkAction delegate with the new SocketState so the user can take action. 
        /// 
        /// If anything goes wrong during the connection process (such as the server being stopped externally), 
        /// the OnNetworkAction delegate should be invoked with a new SocketState with its ErrorOccured flag set to true 
        /// and an appropriate message placed in its ErrorMessage field. The event-loop should not continue if
        /// an error occurs.
        ///
        /// If an error does not occur, after invoking OnNetworkAction with the new SocketState, an event-loop to accept 
        /// new clients should be continued by calling BeginAcceptSocket again with this method as the callback.
        /// </summary>
        /// <param name="ar">The object asynchronously passed via BeginAcceptSocket. It must contain a tuple with 
        /// 1) a delegate so the user can take action (a SocketState Action), and 2) the TcpListener</param>
        private static void AcceptNewClient(IAsyncResult ar)
        {
            Tuple<Action<SocketState>, TcpListener> temp = (Tuple<Action<SocketState>, TcpListener>)ar.AsyncState;

            try
            {
                SocketState ss = new SocketState(temp.Item1, temp.Item2.EndAcceptSocket(ar));
                ss.OnNetworkAction(ss);
                temp.Item2.BeginAcceptSocket(AcceptNewClient, temp);
            }

            catch
            {
                SocketState ss = new SocketState(temp.Item1, null);
                ss.ErrorOccured = true;
                ss.ErrorMessage = "Connection Failed";
                ss.OnNetworkAction(ss);
            }

            
        }

        /// <summary>
        /// Stops the given TcpListener.
        /// </summary>
        public static void StopServer(TcpListener listener)
        {
            try
            {
                listener.Stop();
            }

            catch
            {
                
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////
        // Client-Side Code
        /////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Begins the asynchronous process of connecting to a server via BeginConnect, 
        /// and using ConnectedCallback as the method to finalize the connection once it's made.
        /// 
        /// If anything goes wrong during the connection process, toCall should be invoked 
        /// with a new SocketState with its ErrorOccured flag set to true and an appropriate message 
        /// placed in its ErrorMessage field. Between this method and ConnectedCallback, toCall should 
        /// only be invoked once on error.
        ///
        /// This connection process should timeout and produce an error (as discussed above) 
        /// if a connection can't be established within 3 seconds of starting BeginConnect.
        /// 
        /// </summary>
        /// <param name="toCall">The action to take once the connection is open or an error occurs</param>
        /// <param name="hostName">The server to connect to</param>
        /// <param name="port">The port on which the server is listening</param>
        public static void ConnectToServer(Action<SocketState> toCall, string hostName, int port)
        {
            // TODO: This method is incomplete, but contains a starting point 
            //       for decoding a host address

            // Establish the remote endpoint for the socket.
            IPHostEntry ipHostInfo;
            IPAddress ipAddress = IPAddress.None;
            SocketState temp = new SocketState(toCall, null);

            // Determine if the server address is a URL or an IP
            try
            {
                ipHostInfo = Dns.GetHostEntry(hostName);
                bool foundIPV4 = false;
                foreach (IPAddress addr in ipHostInfo.AddressList)
                    if (addr.AddressFamily != AddressFamily.InterNetworkV6)
                    {
                        foundIPV4 = true;
                        ipAddress = addr;
                        break;
                    }
                // Didn't find any IPV4 addresses
                if (!foundIPV4)
                {
                    temp.ErrorOccured = true;
                    temp.ErrorMessage = "Couldn't find IPv4";
                    temp.OnNetworkAction(temp);
                }
            }
            catch (Exception)
            {
                // see if host name is a valid ipaddress
                try
                {
                    ipAddress = IPAddress.Parse(hostName);
                }
                catch (Exception)
                {
                    temp.ErrorOccured = true;
                    temp.ErrorMessage = "Host name not valid";
                    temp.OnNetworkAction(temp);
                }
            }

            // Create a TCP/IP socket.
            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // This disables Nagle's algorithm (google if curious!)
            // Nagle's algorithm can cause problems for a latency-sensitive 
            // game like ours will be 
            socket.NoDelay = true;

            temp = new SocketState(toCall, socket);

            try
            {
                bool success = socket.BeginConnect(ipAddress, port, ConnectedCallback, temp).AsyncWaitHandle.WaitOne(3000, true);
                if (!success)
                {
                    temp.ErrorOccured = true;
                    temp.ErrorMessage = "Connection Timeout Error";
                    temp.OnNetworkAction(temp);
                }
            }

            catch
            {
                temp.ErrorOccured = true;
                temp.ErrorMessage = "Connection Failed";
                temp.OnNetworkAction(temp);
            }
        }

        /// <summary>
        /// To be used as the callback for finalizing a connection process that was initiated by ConnectToServer.
        ///
        /// Uses EndConnect to finalize the connection.
        /// 
        /// As stated in the ConnectToServer documentation, if an error occurs during the connection process,
        /// either this method or ConnectToServer (not both) should indicate the error appropriately.
        /// 
        /// If a connection is successfully established, invokes the toCall Action that was provided to ConnectToServer (above)
        /// with a new SocketState representing the new connection.
        /// 
        /// </summary>
        /// <param name="ar">The object asynchronously passed via BeginConnect</param>
        private static void ConnectedCallback(IAsyncResult ar)
        {
            SocketState ss = (SocketState)ar.AsyncState;

            try
            {
                ss.TheSocket.EndConnect(ar);
                ss.OnNetworkAction(ss);
            }

            catch
            {
                if (ss.ErrorOccured)
                    return;
                ss.ErrorOccured = true;
                ss.ErrorMessage = "Connection Failed";
                ss.OnNetworkAction(ss);
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////
        // Server and Client Common Code
        /////////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Begins the asynchronous process of receiving data via BeginReceive, using ReceiveCallback 
        /// as the callback to finalize the receive and store data once it has arrived.
        /// The object passed to ReceiveCallback via the AsyncResult should be the SocketState.
        /// 
        /// If anything goes wrong during the receive process, the SocketState's ErrorOccured flag should 
        /// be set to true, and an appropriate message placed in ErrorMessage, then the SocketState's
        /// OnNetworkAction should be invoked. Between this method and ReceiveCallback, OnNetworkAction should only be 
        /// invoked once on error.
        /// 
        /// </summary>
        /// <param name="state">The SocketState to begin receiving</param>
        public static void GetData(SocketState state)
        {
            try
            {
                state.TheSocket.BeginReceive(state.buffer, 0, state.buffer.Length, SocketFlags.None, ReceiveCallback, state);
            }

            catch
            {
                lock (state)
                {
                    state.ErrorOccured = true;
                    state.ErrorMessage = "Connection Failed";
                    state.OnNetworkAction(state);
                }
            }
        }

        /// <summary>
        /// To be used as the callback for finalizing a receive operation that was initiated by GetData.
        /// 
        /// Uses EndReceive to finalize the receive.
        ///
        /// As stated in the GetData documentation, if an error occurs during the receive process,
        /// either this method or GetData (not both) should indicate the error appropriately.
        /// 
        /// If data is successfully received:
        ///  (1) Read the characters as UTF8 and put them in the SocketState's unprocessed data buffer (its string builder).
        ///      This must be done in a thread-safe manner with respect to the SocketState methods that access or modify its 
        ///      string builder.
        ///  (2) Call the saved delegate (OnNetworkAction) allowing the user to deal with this data.
        /// </summary>
        /// <param name="ar"> 
        /// This contains the SocketState that is stored with the callback when the initial BeginReceive is called.
        /// </param>
        private static void ReceiveCallback(IAsyncResult ar)
        {
            SocketState ss = (SocketState)ar.AsyncState;

            try
            {
                int temp = ss.TheSocket.EndReceive(ar);

                if(temp == 0)
                {
                    ss.ErrorOccured = true;
                    ss.ErrorMessage = "Receive Failed";
                }

                lock (ss)
                {
                    ss.data.Append(Encoding.UTF8.GetString(ss.buffer, 0, temp));
                }

                ss.OnNetworkAction(ss);
            }

            catch
            {
                if (ss.ErrorOccured)
                    return;
                ss.ErrorOccured = true;
                ss.ErrorMessage = "Receive Failed";
                ss.OnNetworkAction(ss);
            }
        }

        /// <summary>
        /// Begin the asynchronous process of sending data via BeginSend, using SendCallback to finalize the send process.
        /// 
        /// If the socket is closed, does not attempt to send.
        /// 
        /// If a send fails for any reason, this method ensures that the Socket is closed before returning.
        /// </summary>
        /// <param name="socket">The socket on which to send the data</param>
        /// <param name="data">The string to send</param>
        /// <returns>True if the send process was started, false if an error occurs or the socket is already closed</returns>
        public static bool Send(Socket socket, string data)
        {
            if(socket.Connected)
            {
                try
                {
                    byte[] temp = Encoding.UTF8.GetBytes(data);
                    socket.BeginSend(temp, 0, temp.Length, SocketFlags.None, SendCallback, socket);
                    return true;
                }

                catch
                {
                    socket.Close();
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// To be used as the callback for finalizing a send operation that was initiated by Send.
        ///
        /// Uses EndSend to finalize the send.
        /// 
        /// This method must not throw, even if an error occured during the Send operation.
        /// </summary>
        /// <param name="ar">
        /// This is the Socket (not SocketState) that is stored with the callback when
        /// the initial BeginSend is called.
        /// </param>
        private static void SendCallback(IAsyncResult ar)
        {
            Socket temp = (Socket)ar.AsyncState;

            try
            {
                temp.EndSend(ar);
            }

            catch
            {

            }
        }


        /// <summary>
        /// Begin the asynchronous process of sending data via BeginSend, using SendAndCloseCallback to finalize the send process.
        /// This variant closes the socket in the callback once complete. This is useful for HTTP servers.
        /// 
        /// If the socket is closed, does not attempt to send.
        /// 
        /// If a send fails for any reason, this method ensures that the Socket is closed before returning.
        /// </summary>
        /// <param name="socket">The socket on which to send the data</param>
        /// <param name="data">The string to send</param>
        /// <returns>True if the send process was started, false if an error occurs or the socket is already closed</returns>
        public static bool SendAndClose(Socket socket, string data)
        {
            if (socket.Connected)
            {
                try
                {
                    byte[] temp = Encoding.UTF8.GetBytes(data);
                    socket.BeginSend(temp, 0, temp.Length, SocketFlags.None, SendAndCloseCallback, socket);
                    return true;
                }

                catch
                {
                    socket.Close();
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// To be used as the callback for finalizing a send operation that was initiated by SendAndClose.
        ///
        /// Uses EndSend to finalize the send, then closes the socket.
        /// 
        /// This method must not throw, even if an error occured during the Send operation.
        /// 
        /// This method ensures that the socket is closed before returning.
        /// </summary>
        /// <param name="ar">
        /// This is the Socket (not SocketState) that is stored with the callback when
        /// the initial BeginSend is called.
        /// </param>
        private static void SendAndCloseCallback(IAsyncResult ar)
        {
            Socket temp = (Socket)ar.AsyncState;

            try
            {
                temp.EndSend(ar);
                temp.Close();
            }

            catch
            {
                temp.Close();
            }
        }

    }
}

