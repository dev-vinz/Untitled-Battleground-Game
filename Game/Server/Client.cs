using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Game.Server
{
	/**
	 * @see https://www.codeproject.com/Articles/1415/Introduction-to-TCP-client-server-in-C
	 */
	public class Client
	{
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                               FIELDS                              *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		private string ip;
		private readonly TcpClient tcpClient;

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public string IP
		{
			get { return ip; }
			protected set { ip = value; }
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                            CONSTRUCTORS                           *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public Client(string ip)
		{
			this.ip = ip;
			this.tcpClient = new TcpClient();
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public void Connect()
		{
			try
			{
				tcpClient.Connect(IP, Server.TCP_PORT);
			}
			catch (Exception)
			{
				throw;
			}
		}

		public void Send(string msgJson)
		{
			Stream stm = this.tcpClient.GetStream();

			ASCIIEncoding asen = new ASCIIEncoding();
			byte[] ba = asen.GetBytes(msgJson);
			Console.WriteLine("Transmitting.....");

			stm.Write(ba, 0, ba.Length);
		}

		public bool Read()
		{
			Stream stm = this.tcpClient.GetStream();

			byte[] bb = new byte[100];
			int k = stm.Read(bb, 0, 100);

			for (int i = 0; i < k; i++)
				Console.Write(Convert.ToChar(bb[i]));

			return false;
		}

		public void Connect2()
		{
			try
			{
				TcpClient tcpclnt = new TcpClient();
				Console.WriteLine("Connecting.....");

				tcpclnt.Connect(this.ip, 8001);
				// use the ipaddress as in the server program

				Console.WriteLine("Connected");
				/*
				Console.Write("Enter the string to be transmitted : ");

				string str = Console.ReadLine();
				Stream stm = tcpclnt.GetStream();

				ASCIIEncoding asen = new ASCIIEncoding();
				byte[] ba = asen.GetBytes(str);
				Console.WriteLine("Transmitting.....");

				stm.Write(ba, 0, ba.Length);

				byte[] bb = new byte[100];
				int k = stm.Read(bb, 0, 100);

				for (int i = 0; i < k; i++)
					Console.Write(Convert.ToChar(bb[i]));

				tcpclnt.Close();
				*/
			}

			catch (Exception e)
			{
				Console.WriteLine("Error..... " + e.StackTrace);
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PRIVATE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



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
