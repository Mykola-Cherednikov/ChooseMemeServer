using Assets.Scripts.DTO;
using ChooseMemeServer;
using ChooseMemeServer.DTO;
using System.Text.Json;

public class Queries
{
    public static string ReturnLobbiesQuery(List<Lobby> lobbies)
    {
        List<ShortLobbyDTO> shortLobbiesDTO = new List<ShortLobbyDTO>();

        foreach (var lobby in lobbies)
        {
            ShortLobbyDTO shortLobbyDTO = new ShortLobbyDTO();
            shortLobbyDTO.id = lobby.getId();
            shortLobbyDTO.name = lobby.getName();
            shortLobbyDTO.numOfClients = lobby.getNumOfClients();
            shortLobbyDTO.maxNumOfClients = lobby.getMaxNumOfClients();
            shortLobbiesDTO.Add(shortLobbyDTO);
        }
        ArrayOfShortLobbiesDTO arrayOfShortLobbiesDTO = new ArrayOfShortLobbiesDTO();
        arrayOfShortLobbiesDTO.shortLobbiesDTO = shortLobbiesDTO.ToArray();

        return MultiplayerEvents.ReturnLobbies.ToString() + "\n" + JsonSerializer.Serialize(arrayOfShortLobbiesDTO) + "\n";
    }

    public static string ReturnLobbyQuery(Lobby lobby, Client c)
    {
        LobbyDTO lobbyDTO = new LobbyDTO();
        lobbyDTO.id = lobby.getId();
        lobbyDTO.name = lobby.getName();
        lobbyDTO.numOfClients = lobby.getNumOfClients();
        lobbyDTO.maxNumOfClients = lobby.getMaxNumOfClients();
        lobbyDTO.isHost = (c == lobby.GetHost());

        List<ClientDTO> clientsDTO = new List<ClientDTO>();
        foreach (var client in lobby.getClients())
        {
            ClientDTO clientDTO = new ClientDTO();
            clientDTO.id = client.getId();
            clientDTO.name = client.getNickname();
            clientsDTO.Add(clientDTO);
        }
        lobbyDTO.clientsDTO = new ArrayOfClientsDTO();
        lobbyDTO.clientsDTO.clientsDTO = clientsDTO.ToArray();

        return MultiplayerEvents.ReturnLobby.ToString() + "\n" + JsonSerializer.Serialize(lobbyDTO) + "\n";
    }

    public static string ClientConnectedToLobbyQuery(Client client)
    {
        ClientDTO clientDTO = new ClientDTO();
        clientDTO.id = client.getId();
        clientDTO.name = client.getNickname();

        return MultiplayerEvents.ClientConnectedToLobby.ToString() + "\n" + JsonSerializer.Serialize(clientDTO) + "\n";
    }

    public static string ClientDisconnectedFromLobbyQuery(Client client)
    {
        ClientDTO clientDTO = new ClientDTO();
        clientDTO.id = client.getId();
        clientDTO.name = client.getNickname();

        return MultiplayerEvents.ClientDisconnectedFromLobby.ToString() + "\n" + JsonSerializer.Serialize(clientDTO) + "\n";
    }
}
