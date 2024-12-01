using DVLib.LabDataHelper;
using DVOSLib;
using MathBase;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LabDataHelper
{

	internal class TaskFlag
	{
		internal volatile bool keepRunning = true;

		void stop()
		{
			keepRunning = false;
		}
	}
	public class AngleControl
	{
		AngleDataHelper angle;
		MoveHelper move;
		double f = 500;
		public static int max = 8;
		 public double angleRealtime { get; private set; }
		double fl;

		bool taskRun { get { if (current != null) return current.keepRunning;else return false; } set { if (current != null) current.keepRunning = value; } }
		public double focalLength { get { return fl; } set { fl = value;convertFactor = 1000000/ value; } }
		public double convertFactor { get; private set; }
		double fov = 8726.646;
		TaskFlag current;
		volatile bool stable = false;
		volatile Queue<double> posQueue=new Queue<double>();
		public double posRealtime { get; private set; }
		volatile bool changed = false;
        volatile byte[] zeroPos_=new byte[8] ;
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

	
		SourceOperator selectIndex=d=> { return d[0]; };
		Action<double> onAngleUpdate;
		Action<AngleDataHelper> onError;
		short[] rawData;
		double lastMove = 0;
		bool IntMove = false;
		double adjustAngle = 200;

		public double pos2angle(double dPos)
		{
			return  dPos  * convertFactor;
		}

		public double angle2pos(double dangle)
		{
			return dangle / convertFactor;
		}
		public AngleControl(AngleDataHelper angle,MoveHelper move)
		{
			focalLength = 500;
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
				double k = 0.3051804167;
				for (int i = 0; i < d.Length; i++)
				{
					if(selectIndex(i)>0)
					{
						sum += d[i]*k;
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

		public void stopTask()
		{
			taskRun = false;
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
			if(IntMove&&Math.Abs(dx)<1)
			{
				dx=Math.Sign(dx);
			}
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

		public void setZero(DataManager ma)
		{

			Task.Run(() =>
			{
				int index;
			
				onAngleUpdate = (s) =>
				{
						zeroPos = posRealtime - angle2pos( angleRealtime) ;
                
                }; 
				onError = (s) =>
                {
                };
                angle.update();
            });
            }
		public bool Peak(double dir, double error = 2, int maxTry = 100)
		{
            if (current==null)
            {	Task.Run(async () => {


					current = new TaskFlag();
					await Peak_(dir,error,maxTry);
					current = null;
				});
				return true;
			
			}
            else
            {

				return false;
            }

        }

	
		public bool record(DataManager ma)
		{
			if (current == null)
			{
				Task.Run(async () => {


					current = new TaskFlag();
					await record_(ma);
					current = null;



				});
				return true;
			}
			else
			{
				return false;
			}
		}
		public bool moveAndRecordRaw(double dx, int times, DataManager ma)
		{
			if (current == null)
			{
				Task.Run(async () => {

				
					current = new TaskFlag();
					await moveAndRecordRaw_(dx, times, ma);
					current = null;
			


			});
				return true;
		    }
			else
			{
					return false;
			}
		}
		 async Task record_(DataManager ma)
		{
			await Task.Run(() =>
			{
				int name = 0;
				int index;
				bool stop = true;



				int first = index = ma.addNewData(name.ToString(), posRealtime.ToString());
				onAngleUpdate = (s) =>
				{

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

			});
		}
		 async Task moveAndRecordRaw_(double dx, int times, DataManager ma)
		{
		  await	Task.Run(() =>
			{
				int name = 0;
				int index;
				bool stop = true;

              int first=  index = ma.addNewData(name.ToString(), posRealtime.ToString());



				onAngleUpdate = (s) =>
				{
                   
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
				for (int i = 0; i < times&&taskRun; i++)
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


				}	
			
			});
		}

		public void Peakf(double dir, double error = 8, int maxTry = 100)
		{
			IntMove = true;
			if (maxTry <= 0)
			{
				return;
			}
			if (dir == 0)
			{
				dir = 1;
			}
			Task.Run(() => {
				angle.update();
				bool waitF = true;
				double refPos = 0;
				onAngleUpdate = d => {
					Volatile.Write(ref waitF, false);

				};

				onError = d => {
					Volatile.Write(ref waitF, false);
				};
				while (Volatile.Read(ref waitF)) { };

				if (Math.Abs(angleRealtime - dir) < error)
				{
					if (angleRealtime * dir > 0)
					{

						return;
					}
				}

				
				if (Math.Abs(angleRealtime - dir) < 200)
				{
				

					Move((dir - angleRealtime) /-9.7/4);
					
				}
				else
				{

					Move((dir - angleRealtime) /-9.7*0.75);
				}
				wait();
				Peakf(dir, error, maxTry - 1);
			});



		}
		 async Task Peak_(double dir,double error=2,int maxTry = 100)
		{
           
            IntMove = false;
			if (maxTry <= 0)
			{
				return;
			}
				if (dir==0)
			{
				dir = 0.0001;
			}
			
			await Task.Run(async () => {
				
				bool waitF = true;
				while (maxTry > 0&&taskRun)
				{
					Volatile.Write(ref waitF, true);
					onAngleUpdate = d => {
						Volatile.Write(ref waitF, false);

					};

					onError = d => {
						Volatile.Write(ref waitF, false);
					};
					angle.update();


					while (Volatile.Read(ref waitF)) {
					
					
					};
					double toMoveAngle = dir - angleRealtime;
					double distance=Math.Abs(toMoveAngle);
					double absAngle=Math.Abs(angleRealtime);
					double toMoveFull = angle2pos(toMoveAngle);
					//DVOS.writeLine("实际角度:" + angleRealtime + " 移动角度:" + toMoveAngle);

					//DVOS.writeLine("预计移动距离:" +toMoveFull );

					if (Math.Abs(toMoveAngle) < error)
					{
						DVOS.writeLine("已归零！" );
						return;

					}
					else if (toMoveFull * dir < 0&& Math.Abs(toMoveAngle) < error*10)
					{
					
						double m = angle2pos(-Math.Sign(dir) * adjustAngle);	
					
						Move(m);
						//DVOS.writeLine("e1 移动距离:" + m);
						wait();

					}
					else
					{
						f = 0.8;
						if(absAngle<50)
						{
							f = 0.6;
							if(absAngle<25)
							{
								f = 0.3;
							}
						}

						f = toMoveFull * f;

						//DVOS.writeLine("e2 移动距离:" + f);
						Move(f);
						wait();
					}
					maxTry--;
				}

			

				
				//await Peak_(dir,error, maxTry - 1);
            });

			
                
		}

		

	}
}
