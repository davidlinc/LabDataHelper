using DVOSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LabDataHelper
{
	public class AngleDataHelper : ClientPipeHelper
	{
		short[] datas = new short[200];
		public event Action<short[]> onDataUpdate = (d) => { };
		public event Action<AngleDataHelper> onFail = (d) => { };
		public AngleDataHelper(string name) : base(name)
		{
			onReceive += d =>
			{
				int L = BitConverter.ToInt32( d.AsSpan().Slice(0,4));
				if(L>0)
				{
					datas = new short[L];
					Span<byte> bytes = d;
					Span<short> shorts = datas;
					bytes.Slice(4,L*2).CopyTo(MemoryMarshal.Cast<short, byte>(shorts));
					onDataUpdate(datas);
				}
				else if(L<0)
				{
					onFail(this);
				}
			};
		}

		public void update()
		{
			Operate(d => d[0] = (byte)InfoType.Update);

		}
	}
}
