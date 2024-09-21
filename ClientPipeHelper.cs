using DVOSLib;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabDataHelper
{
	enum InfoType:byte
	{
		Stop,MoveTo,Move,Update
	}
	public class ClientPipeHelper
	{
		string name;
		int bufferSizeSend = 9;
		int bufferSizeReceive = 1024;
		public event Action<string> info = (d) => { };
		public event Action<byte[]> beforeSend = (d)=> { };
		public event Action<byte[]> onReceive = (d) => { };

		volatile byte[] send;
		volatile byte[] receive;
		public event Action<Exception> onError;
		public bool connected {  get;private set; }

		Action<Action<byte[]>> operator0;
		volatile bool run = false;
		NamedPipeClientStream n;
		public ClientPipeHelper(string name)
		{
			this.name = name;
		}
		public ClientPipeHelper setBufferSize(int Send,int Receive)
		{
			this.bufferSizeSend = Send;
			this.bufferSizeReceive = Receive;
			return this;
		}
		public void Operate(Action<byte[]> Operator) {
		
			if(operator0!=null&&Operator!=null)
			{
				operator0(Operator);

			}
			else
			{
			}
		}
		public void stop()
		{
			run = false;
			if(n!=null)
			{
				n.Write(new byte[9]);
				n.Dispose();
			}
		}
		public void start()
		{
			run = true;
			Task.Run(() =>
			{
				 send = new byte[bufferSizeSend];
				 receive = new byte[bufferSizeReceive];
				operator0 = a => { lock (send) { a(send);
						info("set:" + send[0]);
					} };
				try
				{
					using(NamedPipeClientStream ncs=new NamedPipeClientStream(name))
					{
						ncs.Connect();
						connected = true;
						info("已连接");
						while (run)
						{
							lock (send)
							{
                            beforeSend(send);
							}
							info("start Wait");
							while (send[0] == 0) { Thread.Sleep(5); }
							//info("Stop Wait");
							lock (send)
							{
							ncs.Write(send);
							ncs.Flush();
							info("Send:" + send[0]);
								send[0] = 0;
							}
							ncs.Read(receive,0,receive.Length);
							onReceive(receive);
						}

					}
				}
				catch (Exception e)
				{
					connected = false;
					onError(e);
				}

			});
		}
	}
}
