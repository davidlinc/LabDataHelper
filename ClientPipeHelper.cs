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
		Stop,Move,MoveTo,Update
	}
	public class ClientPipeHelper
	{
		string name;
		int bufferSizeSend = 9;
		int bufferSizeReceive = 1024;
		public event Action<byte[]> beforeSend;
		public event Action<byte[]> onReceive;
		public bool connected {  get;private set; }

		Action<Action<byte[]>> operator0;
		bool run = false;
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
		}
		public void stop()
		{
			run = false;
		}
		public void start()
		{
			run = true;
			Task.Run(() =>
			{
				byte[] send = new byte[bufferSizeSend];
				byte[] receive = new byte[bufferSizeReceive];
				operator0 = a => { lock (send) { a(send); } };
				try
				{
					using(NamedPipeClientStream ncs=new NamedPipeClientStream(name))
					{
						ncs.Connect();
						connected = true;
						while(run)
						{
							lock(send)
							{
                            beforeSend(send);
							while (send[0] == 0) ;
							ncs.Write(send);
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
				}

			});
		}
	}
}
