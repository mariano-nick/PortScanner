using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;

namespace PortScanner
{
    /// <summary>
    /// Port Scanner - Enter an IP address and a port range where the program will then attempt to find open ports on the given computer by connecting to each of them. On any successful connections mark the port as open.
    /// 
    /// http://www.internet-computer-security.com/Firewall/Protocols/Ports-Protocols-IP-Addresses.html
    /// 
    /// write a client too? that listens on a specific port, which opens up a connection and opens some random ports? then we verify with this one, should also log a source of truth on the client
    /// </summary>
    public class PortScanner
    {
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            PortScannerOptions options = new PortScannerOptions();
            options.IpAddressString = "172.226.85.43";
            options.PortSearchCeiling = 443;
            options.PortSearchFloor = 443;

            IpConnection ipconn = new IpConnection();
            ipconn.MakeConnection(options);

            stopWatch.Stop();
            Console.WriteLine("Milliseconds: " + stopWatch.ElapsedMilliseconds);
            Console.Write("Successful ports: ");
            Console.WriteLine(string.Join(", ", ipconn.SuccessfulPorts.ToArray()));
            Console.ReadLine();
        }
    }

    // input ip address
    // input port range
    // 65,535 ports total
    public class PortScannerOptions
    {
        private const int _totalPorts = 65535;

        public string IpAddressString { get; set; }
        public string PortRange { get; set; }
        public int PortSearchCeiling { get; set; }
        public int PortSearchFloor { get; set; }
        public int TotalPorts { get { return _totalPorts; }}
    }

    public class IpConnection
    {
        private List<int> _successfulPorts = new List<int>();
        private Socket _socket = new Socket(SocketType.Dgram, ProtocolType.Udp);

        public List<int> SuccessfulPorts { get { return _successfulPorts; } }

        public void MakeConnection(PortScannerOptions options)
        {
            IPAddress ipAddrObj;
            if (IPAddress.TryParse(options.IpAddressString, out ipAddrObj))
            {
                for (int i = options.PortSearchFloor; i <= options.PortSearchCeiling; i++)
                {
                    IPEndPoint iPEndPoint = new IPEndPoint(ipAddrObj, i);
                    TcpClient tcpClient = new TcpClient(iPEndPoint);
                    Thread.Sleep(1000);
                    if (tcpClient.Connected)
                    {
                        Console.WriteLine("Connected");
                    }
                }

                //Parallel.For(options.PortSearchFloor, options.PortSearchCeiling + 1, i =>
                //{
                //    TryConnection(ipAddrObj, i);
                //});
            }
            else
            {
                Console.WriteLine("Failed to connect to socket");
            }
        }

        public void TryConnection(IPAddress ipAddr, int port)
        {
            try
            {
                _socket.Connect(ipAddr, port);
                Console.WriteLine("Success! " + port);
                _successfulPorts.Add(port);
                _socket.Close();
            }
            catch (SocketException socketE)
            {
                Console.WriteLine(socketE.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to connect to port: " + port + " -- " + e.GetType());
            }
        }
    }
}
