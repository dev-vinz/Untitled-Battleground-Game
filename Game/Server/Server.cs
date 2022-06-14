using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Entities.Characters;
using Newtonsoft.Json;
using Entities.Characters.Tier1;
using Newtonsoft.Json.Linq;
using Utilities.ExtensionMethods;
using Entities.Characters.Tier2;
using Entities.Characters.Tier3;
using Entities.Characters.Tier4;
using Entities.Characters.Tier5;
using Entities.Characters.Tier6;
using Game.Phase;

#nullable enable

namespace Game.Server
{
	public class Server
	{
		public static readonly int TCP_PORT = 8001;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private readonly string ip;

		private Socket?[] clients;
		private readonly TcpListener listener;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string IP
		{
			get { return ip; }
		}

		protected int NbClients
		{
			get { return clients.Length; }
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		/// <see cref="https://stackoverflow.com/questions/19387086/how-to-set-up-tcplistener-to-always-listen-and-accept-multiple-connections"/>
		/// <seealso cref="https://stackoverflow.com/questions/6803073/get-local-ip-address"/>
		public Server(int nbClients)
		{
			IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
			string? ip = host.AddressList.Last(i => i.AddressFamily == AddressFamily.InterNetwork)?.ToString();

			if (ip != null)
			{
				this.ip = ip;

				clients = new Socket[nbClients];
				listener = new TcpListener(IPAddress.Parse(ip), TCP_PORT);
			}
			else
			{
				throw new ArgumentNullException(nameof(ip));
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public void Stop()
		{
			foreach (Socket? socket in clients)
			{
				socket?.Close();
			}

			listener.Stop();
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PRIVATE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */


		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         PROTECTED METHODS                         *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		protected string Read(int clientIndex)
		{
			byte[] tabResponse = new byte[100000];
			Socket? socket = clients[clientIndex];

			int? nbBytes = socket?.Receive(tabResponse);
			string strData = "";

			/* Transform bytes to JSON string */
			for (int k = 0; k < nbBytes; k++)
			{
				strData += Convert.ToChar(tabResponse[k]);
			}

			return strData;
		}

		protected void RemoveClient(int index)
		{
			clients[index]?.Disconnect(false);
			clients[index] = null;
			clients = clients.Where(c => c is not null).ToArray();
		}

		protected void Send(string data)
		{
			foreach (Socket? socket in clients)
			{
				SendToClient(socket, data);
			}
		}

		protected void SendToClient(Socket? client, string data)
		{
			if (listener is null) return;

			ASCIIEncoding asen = new ASCIIEncoding();
			client?.Send(asen.GetBytes(data));
		}

		protected void Start()
		{
			try
			{
				listener.Start();

				//Console.WriteLine($"The server is running at port {TCP_PORT}...");
				//Console.WriteLine("The local End point is  :\t" + listener.LocalEndpoint);
			}
			catch (Exception)
			{
				throw;
			}
		}

		protected void WaitForClients()
		{
			int clientsConnected = 0;

			//Console.WriteLine("Attente des joueurs...");

			while (clientsConnected < NbClients)
			{
				Socket socket = listener.AcceptSocket();
				SendToClient(socket, clientsConnected.ToString());

				clients[clientsConnected] = socket;

				clientsConnected++;

				//Console.WriteLine($"Joueur {clientsConnected} connecté à la partie");
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                              INDEXERS                             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         OPERATORS OVERLOAD                        *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

	}
}
