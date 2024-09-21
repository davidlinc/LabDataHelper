using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabDataHelper
{
	public class MoveHelper : ClientPipeHelper
	{
		public double position { get; private set; }
		public event Action<double> onPositionChanged;
		public MoveHelper(string name) : base(name)
		{
			beforeSend += d => { if (d[0] == 0) { d[0] = (byte)InfoType.Update; }; };
			onReceive += d => { position = BitConverter.ToDouble(d, 0);onPositionChanged(position);
				Thread.Sleep(100);
			};

		}

		public void move(double Dposition)
		{
			Operate(d =>
			{
				d[0] = (byte)InfoType.Move;
				Array.Copy(BitConverter.GetBytes(Dposition), 0, d, 1, 8);
			});
		}
		public void moveTo(double Dposition)
		{
			Operate(d =>
			{
				d[0] = (byte)InfoType.MoveTo;
				Array.Copy(BitConverter.GetBytes(Dposition), 0, d, 1, 8);
			});
		}

	}
}
