using DVLib.LabDataHelper;
using DVOSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabDataHelper
{
	public class AngleControl
	{
		AngleDataHelper angle;
		MoveHelper move;
		double f = 500;
		 public double angleRealtime { get; private set; }
		volatile bool stable = false;
		volatile Queue<double> posQueue=new Queue<double>();
		public double posRealtime { get; private set; }
		public double refAngle { get; private set; }
		volatile bool changed = false;
		public double refPos { get; private set; } = 15;

		public double refAngleRealtime { get { return Math.Atan((posRealtime - refPos + refAngle) / f); } }
		int max = 8;
		SourceOperator selectIndex=d=> { return d[0]; };
		Action<double> onAngleUpdate;
		Action<AngleDataHelper> onError;
		short[] rawData;
		double lastMove = 0;
		public AngleControl(AngleDataHelper angle,MoveHelper move)
		{
			this.angle = angle;
			this.move = move;

			move.onPositionChanged += d => { posRealtime = d;
				addData(d);
			} ;
			angle.onFail += (d) => {
				if (this.onError != null) { this.onError(d); this.onError = null; this.onAngleUpdate = null; };	
			};
			angle.onDataUpdate +=d=>{
	            rawData = new short[d.Length];
				lock(rawData)
				{
				Array.Copy(d,rawData, rawData.Length);
				}
			
				double sum = 0;
				int count = 0;
				double k = 1 / 65535.0 * 20 / 1.2;
				for (int i = 0; i < d.Length; i++)
				{
					if(selectIndex(i)>0)
					{
						sum += Math.Atan(d[i]*k/ f ) * 1000000;
						count++;
					}
				}
				sum/=count;
				angleRealtime = sum;
				if(this.onAngleUpdate!=null)
				{
					this.onAngleUpdate(sum);
					this.onAngleUpdate = null;
					this.onError = null;
				}
				else
				{

				}
			};
		}

		public void addData(double a)
		{
			lock(posQueue)
			{
	while(posQueue.Count>=max)
			{
				posQueue.Dequeue();
			}
			posQueue.Enqueue(a);

				bool ts;
				if(posQueue.Count==max&&changed)
				{
					ts = true;
				}else
				{
					ts = false;
				}
            foreach (var item in posQueue)
            {
                if(item!=a)
				{
					ts = false;
						changed = true; 
						break;
				}
            }
				stable = ts;
			}
		
        }
		public void Move(double dx)
		{
			move.move(dx);
			stable = false;
			lastMove = dx;
			changed = false;
		}
		public void MoveTo(double x)
		{
			move.moveTo(x);
			stable = false;
			lastMove = x - posRealtime;
			changed = false;
		}
		public void setIndexSelect(SourceOperator so)
		{
			this.selectIndex = so;
		}
		void wait()
		{
			stable = false;
			while (!stable)
			{
			}
		}


		public  void moveAndRecordRaw(double dx,int times, DataManager ma)
		{


			Task.Run(() =>
			{
				int name = 0;
				int index;
				bool stop = true;
				index = ma.addNewData(name.ToString(), posRealtime.ToString());



				onAngleUpdate = (s) =>
				{
					lock (rawData)
					{
						for (int j = 0; j < rawData.Length; j++)
						{
							if (selectIndex(j) > 0)
							{

								ma.addValue(index, rawData[j], false);
							}

						}
						foreach (var item in rawData)
						{
						}
					}
					Volatile.Write(ref stop, false);
				};
				onError = s =>
					Volatile.Write(ref stop, false); ;
				angle.update();
				while (Volatile.Read(ref stop))
				{
				}
				name++;
				for (int i = 0; i < times; i++)
				{

					Move(dx);
					wait();
					index = ma.addNewData(name.ToString(), posRealtime.ToString());
					name++;
					onAngleUpdate = (s) =>
					{
						lock (rawData)
						{
							for (int j = 0; j < rawData.Length; j++)
							{
								if (selectIndex(j) > 0)
								{

									ma.addValue(index, rawData[j], false);
								}

							}
						}
						Volatile.Write(ref stop, false);
					};
					onError = s =>
					Volatile.Write(ref stop, false); ;
					angle.update();

					Volatile.Write(ref stop, true);
					while (Volatile.Read(ref stop))
					{
					}


				}

			});
		}

		public void Peak(double dir,double error=2,int maxTry = 100,bool setRef=false)
		{
			if(maxTry<=0)return;
			if(dir==0)
			{
				dir = 1;
			}
			Task.Run(() => {
				angle.update();
				bool waitF = true;

				onAngleUpdate = d => {
					Volatile.Write(ref waitF, false); ;
				};

				onError = d => {
					Volatile.Write(ref waitF, false);
				};
				while (Volatile.Read(ref waitF)) {  };

				if ( Math.Abs(Math.Abs(angleRealtime)-Math.Abs(dir)) < error&&dir*lastMove>0)
				{ 
					if(Math.Abs(posRealtime - 14.8) < 1 )
					{
					
						 if(angleRealtime * dir > 0)
						{
							if(setRef)
							{

                         refPos = posRealtime;
					     refAngle = Math.Tan(angleRealtime/1000000)*f;
							}
						return;
						}
						 else
						{

							Move((dir-angleRealtime )/ 12000);
							wait();
							Peak(dir,error, maxTry - 1);
							return;
						}


					}
					
			    }

				if ((angleRealtime-dir)*dir>0|| (angleRealtime - dir) * lastMove > 0|| Math.Abs(posRealtime - 14.8) > 1.2)
				{
					if(dir>0)
					{
						
						MoveTo(13.6);
						wait();
						Move(0.001);
					}
					else
					{
						MoveTo(16);
						wait();
						Move(-0.001);
					}
					wait();
					Peak(dir,error,maxTry - 1);
					return;
				}
				if(Math.Abs(angleRealtime-dir)<200)
				{
					if(Math.Abs(dir)<100&&dir*angleRealtime<0)
                    {

					Move((-dir-angleRealtime )/ 36000);
                    }
					else
                    {

					Move((dir - angleRealtime) / 36000);
					}
				}
				else
				{

					Move((dir-angleRealtime) / 16000);
				}
				wait();
				Peak(dir,error, maxTry - 1);
			});

			
		}

		

	}
}
