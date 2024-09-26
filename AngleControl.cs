using DVLib.LabDataHelper;
using DVOSLib;
using MathBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        volatile byte[] zeroPos_=new byte[8] ;
		volatile bool isRunning=false;
        double zeroPos { get { return MemoryMarshal.Cast<byte, double>(zeroPos_)[0]; }

			set { var rd = new Span<byte>(zeroPos_);

				unsafe
				{
					byte* b =(byte*) (&value);
					Span<byte> ptr = new Span<byte>(b, 8);
					ptr.CopyTo(rd);
				}
			}
		
		}
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

		public void slowMAR(double dx, int times, DataManager ma, int maxPerTime = 10)
		{
			Task.Run(() =>
			{
				int loop = times / maxPerTime;
				int rest = times - loop* maxPerTime+1;
				double ddx = dx * loop;
				double p0= angleRealtime;
                double pos = angleRealtime;
				int last = loop - 1;
				for (int i = 0; i < loop; i++)
				{

                    Peak(p0);
                    waitRunning();
					if(i!=0)
					{
						setZero(ma);
					}
                    Peak(pos);
					waitRunning();
					moveAndRecordRaw(ddx, maxPerTime-1, ma, false,i==0);
					waitRunning();
					if(i==last)
					{

                        moveAndRecordRaw(dx, rest, ma, i>0, i == 0);
						waitRunning();
                    }
					pos += dx/0.001*8;
				}
				ma.orderByDescribe();
			});
		}
		public void setZero(DataManager ma)
		{

            isRunning = true;
			Task.Run(() =>
			{
				int index;
			
				onAngleUpdate = (s) =>
				{
						zeroPos = posRealtime - angleRealtime / 8 * 0.001;
                    isRunning = false;
                }; 
				onError = (s) =>
                {
                    isRunning = false;
                };
                angle.update();
            });
            }
		public void waitRunning()
		{
			while (isRunning) ;
		}
		public void moveAndRecordRaw(double dx, int times, DataManager ma, bool deleteFirst = false, bool resetZero=true)
		{

				isRunning = true;
			Task.Run(() =>
			{
				int name = 0;
				int index;
				bool stop = true;

              int first=  index = ma.addNewData(name.ToString(), posRealtime.ToString());



				onAngleUpdate = (s) =>
				{
                    if (resetZero)
                    {

                        zeroPos = posRealtime - angleRealtime / 8 * 0.001;
                    }
                    ma.changeDescribe(index, (double.Parse(ma[index].describe) - zeroPos).ToString());
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
					index = ma.addNewData(name.ToString(),( posRealtime-zeroPos).ToString());
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


				}	if(deleteFirst)
				{
					ma.removeDate(first);
				}
				isRunning = false;
			
			});
		}

		public void Peak(double dir,double error=2,int maxTry = 100,bool setRef=false)
		{
			isRunning = true;
			if (maxTry <= 0)
			{
				isRunning=false;
				return;
			}
				if (dir==0)
			{
				dir = 1;
			}
			Task.Run(() => {
				angle.update();
				bool waitF = true;
				double refPos=0;
				onAngleUpdate = d => {
					Volatile.Write(ref waitF, false); 
			
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
							isRunning = false;
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
