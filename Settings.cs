using DVOSLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabDataHelper
{
	public struct Settings
	{
	public	string lastName;
		public string lastPath;
		public int sleepTime;
		public string f16;
		public int max;
		public bool reF;
		public bool f16f;
		public double r2;
		public string f16code;
		public void write(InfoStream stream)
		{
			stream.writeString(lastName);
			stream.writeString(lastPath);
			stream.writeString(f16);
			stream.writeString(f16code);
			stream.writeInt(sleepTime);
			stream.writeInt(max);
			stream.writeBool(reF);
			stream.writeBool(f16f);
			stream.writeDouble(r2);
		}

		public void read(InfoStream stream)
		{
			lastName = stream.readString();
			lastPath = stream.readString();
			f16 = stream.readString();
			f16code = stream.readString();
			sleepTime = stream.readInt();
			max = stream.readInt();
			reF=stream.readBool();
			f16f=stream.readBool();
			r2 = stream.readDouble();
		}

		public void save(string path)
		{
			FileStream fs = new FileStream(path, FileMode.Create);
			InfoStream info = new InfoStream();
			write(info);
			fs.Write(info.getToSave(), 0, info.Length);
			fs.Flush();
			fs.Close();
		}
		public bool load(string path)
		{
			FileStream fileStream = new FileStream(path, FileMode.Open);
			try {	
				;
			byte[] bytes = new byte[fileStream.Length];
			fileStream.Read(bytes, 0, bytes.Length);
			InfoStream info = new InfoStream(bytes, 1);
			read(info);
			return true;
			}
			catch{
			return false;
			}
			finally
			{

				fileStream.Close();
			}
		

		}
	}
}
