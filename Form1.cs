using DVLib.LabDataHelper;
using DVOSLib;
using System.Runtime.InteropServices;
using System.Text;

namespace LabDataHelper
{
	public partial class Form1 : Form
	{
		DataManager manager = new DataManager("数据");
		MathObjectManager managerM = new MathObjectManager();
		DataConverter converter;
		Settings settings = new Settings();
		string unit;
		int lastSelect;
		public Form1()
		{
			InitializeComponent();
			comboBox3.TextChanged += nameChanged;
			comboBox3.Click += (o, e) => { updateFiles(); };
			DVOS.stringWriter = (s) => { richTextBox4.Text += s; };
			manager.OnChnage += onChange;
			if (File.Exists("settings.data"))
			{
				settings.load("settings.data");
				comboBox3.Text = settings.lastName;
				manager.name = settings.lastName;
			}
			updateInfo();
			textBox2.KeyDown += (o, e) =>
				{
					if (e.KeyCode == Keys.Enter)
					{
						try
						{
							manager.addValue(comboBox1.SelectedIndex, double.Parse(textBox2.Text));
							textBox2.Text = "";
							textBox2.Focus();
						}
						catch { }
					}
				};
		}
		void updateFiles()
		{
			string path = "data";
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			DirectoryInfo directory = new DirectoryInfo(path);
			comboBox3.Items.Clear();
			foreach (FileInfo file in directory.GetFiles())
			{
				if (file.Extension.ToLower().Equals(".data"))
				{
					comboBox3.Items.Add(file.Name.Remove(file.Name.Length - 5));
				}
			}
		}
		public int charToInt(char c)
		{
			if (c >= '0' && c <= '9')
			{
				return c - '0';
			}
			if (c >= 'a' && c <= 'f')
			{
				return (int)(c - 'a' + 10);
			}
			return 0;
		}

		int c = 16;
		int b = 16 * 16;
		int a = 16 * 16 * 16;
		public int getFrom16(ReadOnlySpan<char> data)
		{
			int v = ((charToInt(data[0]) * c + charToInt(data[1]) + charToInt(data[2]) * a + charToInt(data[3]) * b));
			return v;
		}
		public string clean(string raw)
		{
			List<char> chars = new List<char>();
			foreach (var c in raw)
			{
				if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f'))
				{
					chars.Add(c);
				}
			}
			return new string(chars.ToArray());
		}
		public Int16[] readLZZ(string s)
		{
			s = clean(s.ToLower());

			ReadOnlySpan<char> s1 = s.AsSpan();

			int l = s.Length / 4;
			int b = s.Length / 2;
			int j = 0;
			Int16[] r = new Int16[l];
			for (int i = 0; i < s1.Length; i += 4)
			{

				r[j] = (short)getFrom16(s1.Slice(i, 4));
				j++;
			}

			return r;
		}
		void nameChanged(object sender, EventArgs e)
		{
			manager.name = comboBox3.Text;
		}
		void updateInfo()
		{

			comboBox3.Text = manager.name;
			richTextBox1.Text = manager.describe;

		}
		void updateSelect1()
		{
			object s1 = comboBox1.SelectedItem;
			if (s1 is DataSet)
			{
				richTextBox2.Text = ((DataSet)s1).describe;

			}
			else
			{
				richTextBox2.Text = "";
			}
		}
		void onChange(DataSet dataSet, EventType type)
		{
			switch (type)
			{
				case EventType.NewSet:
				case EventType.RemoveSet:
					comboBox1.Text = "";
					updateCombo1();
					comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
					updateSelect1();
					lastSelect = -1;
					break;
				case EventType.ChangeName:
				case EventType.ChangeText:
					break;
				case EventType.NewValue:
				case EventType.ChangeValue:
				case EventType.RemoveValue:
					lastSelect = -1;
					updateCombo2();
					comboBox2.SelectedItem = null;
					comboBox2.Text = "";
					updateSetInfo();
					break;
			}
		}
		public void updateCombo1()
		{
			object lastSelect = comboBox1.SelectedItem;
			comboBox1.Items.Clear();
			for (int i = 0; i < manager.Count; i++)
			{
				comboBox1.Items.Add(manager[i]);
				if (manager[i] == lastSelect)
				{
					comboBox1.SelectedIndex = i;
				}
			}
			if (comboBox1.SelectedItem is null)
			{
				richTextBox3.Text = "";
			}
		}
		public void updateCombo2()
		{
			object lastSelect = comboBox1.SelectedItem;
			comboBox2.Items.Clear();
			if (lastSelect is DataSet)
			{
				DataSet dataSet = (DataSet)lastSelect;

				for (int i = 0; i < dataSet.Count; i++)
				{
					comboBox2.Items.Add(dataSet[i]);
				}
			}

		}
		private void textBox1_TextChanged(object sender, EventArgs e)
		{
			manager.name = comboBox3.Text;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			new Form2(manager).ShowDialog();
		}

		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{
			manager.describe = richTextBox1.Text;
		}

		void readDescribe(string s)
		{

			s = s.Trim();
			var p = s.findString("unit=");
			if (p.Count > 0)
			{
				int pos = p.First();
				var v = s.AsSpan(pos + 5);
				int end = v.findFirstChar(';');
				unit = v.Slice(0, end).ToString();

			}
			p = s.findString("run{");
			if (p.Count > 0)
			{
				int pos = p.First();
				var v = s.AsSpan(pos + 4);
				int end = v.findFirstChar('}');
				string runs = v.Slice(0, end).ToString();
				string[] strings = runs.Split(';', StringSplitOptions.RemoveEmptyEntries);
				foreach (var vvv in strings)
				{
					managerM.Add(vvv);
				}
			}
			p = s.findString("data=");
			if (p.Count > 0)
			{
				int pos = p.First();
				var v = s.AsSpan(pos + 5);
				int end = v.findFirstChar(';');
				string ss = v.Slice(0, end).ToString();
				var cv = managerM.Run(ss);
				converter = d => cv.getValue(d);

			}

		}
		void updateSetInfo()
		{
			if (comboBox1.SelectedItem is DataSet)
			{
				DataSet set = (DataSet)comboBox1.SelectedItem;
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < set.Count; i++)
				{

					if (converter != null && unit != null)
					{
						sb.AppendLine(set[i].ToString() + " " + set[i, converter].ToString() + unit);

					}
					else
					{
						sb.AppendLine(set[i].ToString());

					}
				}
				if (converter != null && unit != null)
				{

					sb.AppendLine("数据:" + set.Count + " 平均值:" + set.getMean(converter) + unit + " 极限偏差:" + (converter(set.Max - set.Min)) + unit);
				}
				else
				{

					sb.AppendLine("数据:" + set.Count + " 平均值:" + set.Mean + " 极限偏差:" + (set.Max - set.Min));
				}
				richTextBox3.Text = sb.ToString();
				richTextBox3.Select(richTextBox3.Text.Length - 1, 0);
				richTextBox3.ScrollToCaret();
			}
		}
		private void button2_Click(object sender, EventArgs e)
		{
			if (comboBox1.SelectedItem is DataSet && lastSelect > -1)
			{
				try
				{
					manager.changeValue(comboBox1.SelectedIndex, lastSelect, double.Parse(comboBox2.Text));
				}
				catch { }
			}
		}

		private void button1_Click(object sender, EventArgs e)
		{
			if (comboBox1.SelectedItem is DataSet)
			{

				manager.addValue(comboBox1.SelectedIndex, managerM.Run(textBox2.Text).getValue(1));
				textBox2.Text = "";
				textBox2.Focus();

			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			updateSelect1();
			updateCombo2();
			lastSelect = -1;
			comboBox2.SelectedItem = null;
			comboBox2.Text = "";
			updateSetInfo();
		}

		private void textBox2_TextChanged(object sender, EventArgs e)
		{

		}

		private void button4_Click(object sender, EventArgs e)
		{
			if (comboBox1.SelectedItem is DataSet)
			{
				try
				{
					manager.removeValue(comboBox1.SelectedIndex, comboBox2.SelectedIndex);

				}
				catch { }
			}
		}

		private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
		{
			lastSelect = comboBox2.SelectedIndex;
		}

		private void richTextBox2_TextChanged(object sender, EventArgs e)
		{
			if (comboBox1.SelectedItem is DataSet)
			{
				manager.changeDescribe(comboBox1.SelectedIndex, richTextBox2.Text);
			}
		}

		private void button5_Click(object sender, EventArgs e)
		{

			converter = null;
			unit = null;
			managerM.clear();
			if (!Directory.Exists("data"))
			{
				Directory.CreateDirectory("data");
			}
			manager.save("data\\" + manager.name + ".data");
			readDescribe(manager.describe);
			settings.lastName = manager.name;
			settings.save("settings.data");
		}

		private void button6_Click(object sender, EventArgs e)
		{
			if (File.Exists("data\\" + manager.name + ".data"))
			{
				manager.load("data\\" + manager.name + ".data");
				updateCombo1();
				richTextBox1.Text = manager.describe;
				converter = null;
				unit = null;
				managerM.clear();
				readDescribe(manager.describe);
			}

		}

		private void button7_Click(object sender, EventArgs e)
		{
			manager.clear();

			updateCombo1();
			updateCombo2();

			comboBox1.Text = "";
			comboBox2.Text = "";
		}

		private void button8_Click(object sender, EventArgs e)
		{
			if (comboBox1.SelectedItem is DataSet)
			{
				manager.removeDate(comboBox1.SelectedIndex);
			}
		}


		private void button9_Click_1(object sender, EventArgs e)
		{

			managerM.Run(textBox2.Text);
			textBox2.Text = "";
			textBox2.Focus();


		}

		private void button10_Click(object sender, EventArgs e)
		{
			Int16[] is16 = readLZZ(richTextBox4.Text);
			richTextBox4.Text = "";
			int index;
			if (comboBox1.SelectedItem is DataSet)
			{
				index = comboBox1.SelectedIndex;
			}
			else
			{
				index = manager.addNewData("16进制", "");
			}
			int max = (int)numericUpDown1.Value;
			int j = 0;
			foreach (Int16 i in is16)
			{
				manager.addValue(index, i);
				j++;
				if(j == max)
				{
					break;
				}
			}
		}

		private void richTextBox4_TextChanged(object sender, EventArgs e)
		{


		}
	}
}