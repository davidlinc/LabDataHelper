using DVOSLib;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabDataHelper
{
	public class ControlHelper
	{
		string name;

		public double position {  get;private set; }
		public event Action<double> onPositionChanged;
		public ControlHelper(string name)
		{
			this.name = name;	
		}
		Action<double> move;

		Action<double> moveTo;

		public void Move(double dx)
		{
			if(move!=null)
			{
				move(dx);
			}
		}
		public void MoveTo(double d)
		{
			if (moveTo!=null)
			{
				moveTo(d);
			}
		}
		public void strat()
		{

			Task.Run(() =>
			{
				try
				{
					using (NamedPipeClientStream cs = new NamedPipeClientStream("dvconnect"))
					{
						cs.Connect();
						byte[] bytes1 = new byte[9];
						move = d =>
						{
							lock (bytes1)
							{

								bytes1[0] = 2;
								Array.Copy(BitConverter.GetBytes(d), 0, bytes1, 1, 8);
							}
						};
						moveTo = d =>
						{
							lock (bytes1)
							{
								bytes1[0] = 1;
								Array.Copy(BitConverter.GetBytes(d), 0, bytes1, 1, 8);
							}
						};
						byte[] bytes = new byte[8];
						while (true)
						{
							lock (bytes1)
							{
								cs.Write(bytes1, 0, bytes1.Length);
								bytes1[0] = 0;
							}

							cs.Read(bytes);
							position = BitConverter.ToDouble(bytes);
							onPositionChanged(position);
						}
					}
				}
				catch (Exception e)
				{
					move = null;
					moveTo = null;
					DVOS.writeLine(e.Message);
				}
			});
		}
	}
}
