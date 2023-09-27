using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChooseMemeServer
{
    public class Client
    {
        private TcpClient _tcpClient;

        private int _id;

        private string? _nickname;

        private Lobby? _lobby;

        public Client(int id, TcpClient t)
        {
            _lobby = null;
            _id = id;
            _tcpClient = t;
        }

        public TcpClient GetTcpClient()
        {
            return _tcpClient;
        }

        public int getId()
        {
            return _id;
        }

        public void setNickname(string nickname)
        {
            _nickname = nickname;
        }

        public string? getNickname()
        {
            return _nickname;
        }

        public Lobby? getLobby()
        {
            return _lobby;
        }

        public void setLobby(Lobby? lobby)
        {
            _lobby = lobby;
        }
    }
}
