using Assets.Scripts.DTO;
using ChooseMemeServer.DTO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace ChooseMemeServer
{
    public enum MultiplayerEvents
    {
        Ping, //server
        AskForLobbies, //client
        ReturnLobbies, //server
        SetNickName, //client
        CreateLobby, //client
        ConnectToLobby, //client
        DisconnectFromLobby, //client
        ReturnLobby, //server
        ClientConnectedToLobby, //server 
        ClientDisconnectedFromLobby, //server
        SetNewHost, //server
        StartGame, //client
        StartedGame //server
    }

    public class Server
    {
        private List<Client> _clients;

        private List<Lobby> _lobbies;

        private int _idClient = 1;

        private int _idLobby = 1;

        public Server()
        {
            _clients = new List<Client>();

            _lobbies = new List<Lobby>();

            //CreateTestLobby();

            TcpListener NormalTcpListener = new TcpListener(IPAddress.Parse("192.168.1.10"), 27015);

            NormalTcpListener.Start();

            TcpListener RadminTcpListener = new TcpListener(IPAddress.Parse("26.210.70.154"), 27015);

            RadminTcpListener.Start();

            Task.Run(() =>
            {
                ServerAcceptConnections(NormalTcpListener);
            });

            Task.Run(() =>
            {
                ServerAcceptConnections(RadminTcpListener);
            });

            Task.Run(() =>
            {
                ServerPingConnections();
            });

            Console.WriteLine("Server started");
        }

        private async void ServerPingConnections()
        {
            while (true)
            {
                var array = _clients.ToArray();

                foreach (var client in array)
                {
                    try
                    {
                        await client.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(MultiplayerEvents.Ping.ToString() + "\n"));
                    }
                    catch (Exception)
                    {
                        ClientDisconnect(client);
                    }
                }

                Thread.Sleep(1000);
            }
        }

        private async void ServerAcceptConnections(TcpListener listener)
        {
            while (true)
            {
                if (listener.Pending())
                {
                    TcpClient tcpClient = await listener.AcceptTcpClientAsync();
                    Console.WriteLine("Client connected");
                    Client client = new Client(_idClient, tcpClient);
                    _idClient++;
                    _clients.Add(client);

                    await Task.Run(() =>
                    {
                        ListenClient(client);
                    });
                }
            }
        }

        private async void ListenClient(Client client)
        {
            Queue<string> queries = new Queue<string>();

            try
            {
                while (client.GetTcpClient() != null)
                {
                    byte[] buffer = new byte[1024]; // byte buffer

                    int numOfBytes = await client.GetTcpClient().GetStream().ReadAsync(buffer); // Read from server to buffer

                    string query = Encoding.UTF8.GetString(buffer[..numOfBytes]); // Convert byte buffer to string

                    string[] queryRows = query.Split("\n"); // Split into rows

                    foreach (var row in queryRows)
                    {
                        if (row != string.Empty)
                        {
                            queries.Enqueue(row); // Add querry
                        }
                    }

                    while (queries.Count > 0) // Handle a query
                    {
                        if (queries.TryPeek(out string? result) && Enum.TryParse(result, out MultiplayerEvents quaryHead)) // Try to get query head
                        {
                            queries.Dequeue();
                            switch (quaryHead)
                            {
                                case MultiplayerEvents.SetNickName:
                                    {
                                        NicknameDTO? nicknameDTO = JsonSerializer.Deserialize<NicknameDTO>(queries.Dequeue());

                                        if (nicknameDTO != null)
                                        {
                                            SetNickName(client, nicknameDTO);
                                        }

                                        break;
                                    }
                                case MultiplayerEvents.AskForLobbies:
                                    {
                                        AskForLobbies(client);

                                        break;
                                    }
                                case MultiplayerEvents.CreateLobby:
                                    {
                                        ShortLobbyDTO? lobbyDTO = JsonSerializer.Deserialize<ShortLobbyDTO>(queries.Dequeue());

                                        if (lobbyDTO != null)
                                        {
                                            CreateLobby(client, lobbyDTO);
                                        }

                                        break;
                                    }
                                case MultiplayerEvents.ConnectToLobby:
                                    {
                                        LobbyDTO? lobbyDTO = JsonSerializer.Deserialize<LobbyDTO>(queries.Dequeue());

                                        if (lobbyDTO != null)
                                        {
                                            ConnectToLobby(client, lobbyDTO);
                                        }

                                        break;
                                    }
                                case MultiplayerEvents.DisconnectFromLobby:
                                    {
                                        ClientDisconnectedFromLobby(client);

                                        break;
                                    }
                                case MultiplayerEvents.StartGame:
                                    {
                                        StartGame(client);

                                        break;
                                    }
                            }
                        }
                        else
                        {
                            queries.Dequeue();
                        }
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        private void ClientDisconnect(Client client)
        {
            ClientDisconnectedFromLobby(client);
            _clients.Remove(client);
            client.GetTcpClient().Close();
            Console.WriteLine("Client disconnected");
        }

        private async void AskForLobbies(Client client)
        {
            string query = Queries.ReturnLobbiesQuery(_lobbies);

            await client.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(query));
        }

        private void SetNickName(Client client, NicknameDTO nicknameDTO)
        {
            if (nicknameDTO != null && nicknameDTO.NickName != null)
            {
                client.setNickname(nicknameDTO.NickName);
            }
        }

        private async void CreateLobby(Client client, ShortLobbyDTO lobbyDTO)
        {
            Lobby lobby = new Lobby(_idLobby, lobbyDTO.name != null ? lobbyDTO.name : $"Lobby {_idLobby}", lobbyDTO.maxNumOfClients, client);

            _lobbies.Add(lobby);

            _idLobby++;

            client.setLobby(lobby);

            string query = Queries.ReturnLobbyQuery(lobby, client);

            await client.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(query));
        }

        private async void ConnectToLobby(Client client, LobbyDTO lobbyDTO)
        {
            Lobby lobby = _lobbies.First(f => f.getId() == lobbyDTO.id);

            if(lobby.getNumOfClients() == lobby.getMaxNumOfClients())
            {
                return;
            }

            lobby.AddClient(client);

            client.setLobby(lobby);

            string query = Queries.ReturnLobbyQuery(lobby, client);

            await client.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(query));

            ClientConnectedToLobby(client, lobby);
        }

        private async void ClientConnectedToLobby(Client client, Lobby lobby)
        {
            string query = Queries.ClientConnectedToLobbyQuery(client);

            foreach (var c in lobby.getClients())
            {
                if (c != client)
                {
                    Console.WriteLine($"Client {c.getNickname()} get info about {client.getNickname()}");
                    await c.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(query));
                }
            }
        }

        private async void ClientDisconnectedFromLobby(Client client)
        {
            Lobby? lobby = client.getLobby();

            if (lobby != null)
            {
                lobby.RemoveClient(client);

                if(lobby.getClients().Count() == 0)
                {
                    _lobbies.Remove(lobby);
                }
                else if(client == lobby.GetHost())
                {
                    Client newHostClient = lobby.getClients()[0];
                    lobby.SetHost(newHostClient);

                    string quary = MultiplayerEvents.SetNewHost.ToString() + "\n";

                    await newHostClient.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(quary));
                }

               

                string query = Queries.ClientDisconnectedFromLobbyQuery(client);

                foreach (var c in lobby.getClients())
                {
                    if (c != client)
                    {
                        await c.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(query));
                    }
                }
            }

            client.setLobby(null);
        }

        private async void StartGame(Client c)
        {
            Lobby? l = c.getLobby();

            if(l != null && c == l.GetHost())
            {
                string query = MultiplayerEvents.StartedGame.ToString() + "\n";

                foreach (var player in l.getClients())
                {
                    await player.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(query));
                }

                await Task.Run(() => { GameManagement(l); });
            }
        }

        public void GameManagement(Lobby? l)
        {

        }
    }
}
