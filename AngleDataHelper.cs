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
		public event Action<short[]> onDataUpdate;
		public AngleDataHelper(string name) : base(name)
		{
			onReceive += d =>
			{
				Span<byte> bytes = d;
				Span<short> shorts = datas;
				bytes.CopyTo(MemoryMarshal.Cast<short, byte>(shorts));
				onDataUpdate(datas);
			};
		}

		public void update()
		{
			Operate(d => d[0] = (byte)InfoType.Update);
		}
	}
}
