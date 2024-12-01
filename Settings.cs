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
	public	string lastName="";
		public string lastPath="";
		public int sleepTime=0;
		public string f16="";
		public int max=0;
		public bool reF=false;
		public bool f16f=false;
		public double r2=0;
		public string f16code="";
		List<(string, string)> buttons=new List<(string, string)>();

		public Settings()
		{
		}

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
			stream.writeInt(buttons.Count);
			for (int i = 0; i < buttons.Count; i++) {
				stream.writeString(buttons[i].Item1);
				stream.writeString(buttons[i].Item2);
			}
		}

		public string getButtomString()
		{
			StringBuilder sb = new StringBuilder();
			foreach
				(var button in buttons) {
			
			sb.Append(button.Item1);
		    sb.Append(":");
			sb.Append(button.Item2);
			sb.Append(";\n");
			}
			return sb.ToString();
		}

		public void readButtons(string text)
		{
			buttons.Clear();
			text=text.Replace("\n","");
			var s = text.Split(';',StringSplitOptions.RemoveEmptyEntries);

			for (int i = 0; i < s.Length; i++)
			{
				
				var ss = s[i].Split(':',StringSplitOptions.RemoveEmptyEntries);
				buttons.Add((ss[0], ss[1]));
			}
			

		}

		public (string,string)[] getButtons()
		{
			return buttons.ToArray();
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
			int c= stream.readInt();
			buttons.Clear();
			for (int i = 0; i <c; i++)
			{
				buttons.Add((stream.readString(), stream.readString()));	
			}
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

		internal void addButton(string item11, string item12)
		{
			buttons.Add((item11, item12));
		}
	}
}
