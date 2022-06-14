using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Game.Phase;
using Newtonsoft.Json;

namespace Game.Server
{
	/// <see cref="https://www.codeproject.com/Articles/1415/Introduction-to-TCP-client-server-in-C"/>
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
			
			tcpClient = new TcpClient();
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                           PUBLIC METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		public int Connect()
		{
			try
			{
				tcpClient.Connect(IP, Server.TCP_PORT);

				Stream stm = tcpClient.GetStream();
				byte[] bytes = new byte[10];

				int nbBytes = stm.Read(bytes, 0, bytes.Length);
				string pId = "";

				for (int k = 0; k < nbBytes; k++)
				{
					pId += Convert.ToChar(bytes[k]);
				}

				return int.Parse(pId);
			}
			catch (Exception)
			{
				throw;
			}
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                          PRIVATE METHODS                          *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         PROTECTED METHODS                         *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		protected string Read()
		{
			Stream stm = tcpClient.GetStream();
			byte[] bytes = new byte[100000];

			int nbBytes = stm.Read(bytes, 0, bytes.Length);
			string strData = "";

			for (int k = 0; k < nbBytes; k++)
			{
				strData += Convert.ToChar(bytes[k]);
			}

			return strData;
		}

		protected void Send(string msgJson)
		{
			Stream stm = tcpClient.GetStream();

			ASCIIEncoding asen = new ASCIIEncoding();
			byte[] bytes = asen.GetBytes(msgJson);

			stm.Write(bytes, 0, bytes.Length);
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                              INDEXERS                             *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */



		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                         OPERATORS OVERLOAD                        *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

	}
}
