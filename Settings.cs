using DVOSLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabDataHelper
{
	public class Settings
	{
	public	string lastName;

		public void write(InfoStream stream)
		{
			stream.writeString(lastName);
		}

		public void read(InfoStream stream)
		{
			lastName = stream.readString();
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
		public void load(string path)
		{
			FileStream fileStream = new FileStream(path, FileMode.Open);
			byte[] bytes = new byte[fileStream.Length];
			fileStream.Read(bytes, 0, bytes.Length);
			InfoStream info = new InfoStream(bytes, 1);
			read(info);
			fileStream.Close();

		}
	}
}
