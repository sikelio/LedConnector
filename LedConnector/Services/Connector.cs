using System.Net;
using System.Net.Sockets;

namespace LedConnector.Services
{
    class Connector
    {
        private readonly TcpClient _client;

        private readonly IPAddress _address;
        private readonly int _port;

        public Connector(string address, int port)
        {
            _client = new();
            _address = IPAddress.Parse(address);
            _port = port;
        }

        public async Task<TcpClient> Connect()
        {
            await _client.ConnectAsync(_address, _port);

            return _client;
        }
    }
}
