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

#nullable enable

namespace Game.Server
{
	public class Server
	{
		public static int TCP_PORT = 8001;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private readonly string ip;
		private readonly int nbClients;
		private readonly Socket[] clients;
		private readonly TcpListener listener;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string IP
		{
			get { return ip; }
		}

		public int NbClients
		{
			get { return nbClients; }
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
				this.nbClients = nbClients;
				this.clients = new Socket[this.nbClients];
				this.listener = new TcpListener(IPAddress.Parse(ip), TCP_PORT);

				Start();
			}
			else
			{
				throw new ArgumentNullException(nameof(ip));
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public void ApplyTurn()
		{
			/* Wait for players comp */
			Thread[] threads = new Thread[nbClients];
			Character[][] tabCharacters = new Character[nbClients][];

			for (int i = 0; i < nbClients; i++)
			{
				threads[i] = new Thread((object? oIndex) =>
				{
					if (oIndex is null) return;

					int index = (int)oIndex;

					byte[] tabResponse = new byte[1000];
					Socket socket = clients[index];

					int nbBytes = socket.Receive(tabResponse);
					string strCharacters = "";

					/* Transform bytes to JSON string */
					for (int k = 0; k < nbBytes; k++)
					{
						strCharacters += Convert.ToChar(tabResponse[k]);
					}

					string[]? tabStr = JsonConvert.DeserializeObject<string[]>(strCharacters);

					IEnumerable<JObject>? objects = tabStr?.Select(c => JObject.Parse(c));
					string?[] characterNames = objects?.Select(o => o.First?.First?.ToString())?.GetNotNullValues() ?? Array.Empty<string>();

					if (characterNames.Length < 1) return;

					tabCharacters[index] = new Character[characterNames.Length];

					for (int k = 0; k < characterNames.Length; k++)
					{
						string? name = characterNames[k];

						switch (name)
						{
							case Ant.NAME:
								tabCharacters[index][k] = new Ant();
								break;
							default:
								throw new ArgumentOutOfRangeException(nameof(name), name, null);
						}
					}

					Console.WriteLine($"Got {characterNames.Length} pets from Player {index + 1} : {string.Join("; ", characterNames)}");
				});

				threads[i].Start(i);
			}

			foreach (Thread thread in threads)
			{
				thread.Join();
			}

			/* Apply turn - Make battle */

			// TODO

			/* Apply turn - Send results to clients */
			SendResults();
		}

		public void Stop()
		{
			foreach (Socket socket in clients)
			{
				socket.Close();
			}

			listener.Stop();
		}

		public void WaitForClients()
		{
			int clientsConnected = 0;

			Console.WriteLine("Attente des joueurs...");

			while (clientsConnected < nbClients)
			{
				Socket socket = listener.AcceptSocket();
				clients[clientsConnected] = socket;

				clientsConnected++;

				Console.WriteLine($"Joueur {clientsConnected} connecté à la partie");
			}
		}

		///https://stackoverflow.com/questions/19387086/how-to-set-up-tcplistener-to-always-listen-and-accept-multiple-connections
		private void StartListening()
		{
			try
			{

				int i = 0;
				while (true)
				{
					if (i < this.nbClients)
					{
						Console.WriteLine("Waiting for a connection.....");
						Socket s = listener.AcceptSocket();
						i++;

						Console.WriteLine(i);
						Console.WriteLine("Connection accepted from " + s.RemoteEndPoint);

						var childSocketThread = new Thread(() =>
						{

							byte[] b = new byte[100];

							int k = s.Receive(b);
							Console.WriteLine("Recieved...");
							for (int i = 0; i < k; i++)
								Console.Write(Convert.ToChar(b[i]));

							ASCIIEncoding asen = new ASCIIEncoding();
							s.Send(asen.GetBytes("The string was recieved by the server."));
							Console.WriteLine("\nSent Acknowledgement");

							/* clean up */
							s.Close();
							i--;
							listener.Stop();

						});

						childSocketThread.Start();
					}
					else
					{
						Console.WriteLine("Number of clients reached");
						break;
					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Error..... " + e.StackTrace);
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PRIVATE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private void Start()
		{
			try
			{
				listener.Start();

				Console.WriteLine($"The server is running at port {TCP_PORT}...");
				Console.WriteLine("The local End point is  :\t" + listener.LocalEndpoint);
			}
			catch (Exception)
			{
				throw;
			}
		}

		private void SendResults()
		{
			foreach (Socket socket in clients)
			{
				ASCIIEncoding asen = new ASCIIEncoding();
				socket.Send(asen.GetBytes("Nique ta race <3"));
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         PROTECTED METHODS                         *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                              INDEXERS                             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         OPERATORS OVERLOAD                        *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

	}
}
