using DVLib.LabDataHelper;
using DVOSLib;
using MathBase;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Runtime.InteropServices;
using System.Text;

namespace LabDataHelper
{
	public partial class Form1 : Form
	{
		DataManager manager = new DataManager("数据");
		MathObjectManager managerM = new MathObjectManager();
		DataConverter converter;
		DataConverter refConverter;
		Helper helper;
		MoveHelper move = new MoveHelper("dvconnect");
		LevelGetter getter = new LevelGetter();
		DoubleStack<string> codeRecord = new();
		double maxRef = double.PositiveInfinity;

		DataSet baseSet
		{
			get
			{

				if (comboBox4.SelectedItem is DataSet)
				{
					return (DataSet)comboBox4.SelectedItem;
				}
				return null;
			}
		}
		Settings settings = new Settings();
		string unit;
		int lastSelect = -1;

		int lastSelectDataset = -1;
		string fsNmae;
		public Form1()
		{
			InitializeComponent();
			getter.register('(', 1);
			getter.register(')', -1);
			getter.register('{', 1);
			getter.register('}', -1);
			getter.register('[', 1);
			getter.register(']', -1);
			comboBox1.KeyDown += (o, e) =>
			{

				if (e.KeyCode == Keys.Enter && lastSelectDataset > -1 && lastSelectDataset < manager.Count)
				{
					manager[lastSelectDataset].name = comboBox1.Text;
					updateCombo1();
					comboBox1.SelectedIndex = lastSelectDataset;
				}
			};

			textBox2.MouseWheel += (o, e) =>
			{
				if (e.Delta > 0)
				{
					lastCode();
				}
				else
				{
					nextCode();
				}
			};

			helper = new Helper(manager);
			saveFileDialog1.FileOk += (o, e) =>
			{

				string s = saveFileDialog1.FileName;
				fsNmae = s;

			};
			comboBox3.TextChanged += nameChanged;
			comboBox3.Leave += (o, e) => { updateFiles(); };
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

		void registerFunc(
			)
		{

			managerM.regiseterMethod("setText", (a, b) =>
			{
				if (b != null && b.Length > 0)
				{
					string s = b[0].Item1;
					var v = managerM.Run(b[0].Item1);
					double d = v.getValue(2);
					if (!double.IsNaN(d))
					{
						s = d.ToString();
					}
					if (v is RuntimeMathObject)
					{
						var ss = v.AsRuntime().valueOut;
						if (ss != null)
						{
							s = ss.ToString();
						}
					}
					richTextBox4.Text += s + "\n";
					return (s, (data) => 1);
				}
				return (null, (data) => 0);
			});
			managerM.regiseterMethod("getName", (a, b) =>
			{

				return (this.Text, d => 0);
			});
		}
		void addVisualFx()
		{
			//	foreach(var v in contr)
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
		int d = 1;
		int[] num = new int[] { 16 * 16 * 16, 16 * 16, 16, 1 };
		int[] defaultNum = new int[] { 0, 1, 2, 3 };
		public int getFrom16(ReadOnlySpan<char> data, int[] i = null)
		{
			if (i == null)
			{
				i = defaultNum;
			}
			int v = ((charToInt(data[0]) * num[i[0]] + charToInt(data[1]) * num[i[1]] + charToInt(data[2]) * num[i[2]] + charToInt(data[3]) * num[i[3]]));
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
		public Int16[] readLZZ(string s, int[] order = null)
		{
			s = clean(s.ToLower());

			ReadOnlySpan<char> s1 = s.AsSpan();

			int l = s.Length / 4;
			int j = 0;
			Int16[] r = new Int16[l];
			for (int i = 0; i < s1.Length; i += 4)
			{
				r[j] = (short)getFrom16(s1.Slice(i, 4), order);
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
					updateSetInfo(converter, unit);
					break;
			}
		}
		public void updateCombo1()
		{
			object lastSelect = comboBox1.SelectedItem;
			object lastSelect2 = comboBox4.SelectedItem;
			comboBox1.Items.Clear();
			comboBox4.Items.Clear();
			for (int i = 0; i < manager.Count; i++)
			{
				comboBox1.Items.Add(manager[i]);
				comboBox4.Items.Add(manager[i]);
				if (manager[i] == lastSelect)
				{
					comboBox1.SelectedIndex = i;
				}
				if (manager[i] == lastSelect2)
				{
					comboBox4.SelectedIndex = i;
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

			s = s.Replace(" ", "").Replace("\n", "");

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
				var v = s.AsSpan(pos + 3);
				int end = v.findEnd('{', '}');
				string runs = v.Slice(1, end).ToString();

				string[] strings = runs.cutZeroLevel(';', getter);
				foreach (var vvv in strings)
				{
					managerM.Run(vvv).getValue();
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
			p = s.findString("dataRef=");
			if (p.Count > 0)
			{
				int pos = p.First();
				var v = s.AsSpan(pos + 8);
				int end = v.findFirstChar(';');
				string ss = v.Slice(0, end).ToString();
				var cv = managerM.Run(ss);
				refConverter = d => cv.getValue(d);

			}
			p = s.findString("maxRef=");
			if (p.Count > 0)
			{
				int pos = p.First();
				var v = s.AsSpan(pos + 7);
				int end = v.findFirstChar(';');
				string ss = v.Slice(0, end).ToString();
				var cv = managerM.Run(ss);
				maxRef = cv.getValue(d);

			}

		}

		void addWithColor(string text, StringBuilder sb, List<int> pos, List<int> length, int line)
		{

			pos.Add(sb.Length - line);
			length.Add(text.Length);
			sb.Append(text);
		}
		void updateSetInfo(DataConverter converter, string unit)
		{
			if (converter == null)
			{
				converter = (d) => d;

			}
			if (unit == null)
			{
				unit = "";
			}

			if (comboBox1.SelectedItem is DataSet)
			{
				DataSet set = (DataSet)comboBox1.SelectedItem;
				int index = comboBox1.SelectedIndex;
				StringBuilder sb = new StringBuilder();
				List<int> greenPos = new List<int>();
				List<int> greenLength = new List<int>();
				List<int> bluePos = new List<int>();
				List<int> blueLength = new List<int>();
				List<int> redPos = new List<int>();
				List<int> redLength = new List<int>();
				int line = 0;
				for (int i = 0; i < set.Count; i++)
				{
					addWithColor(set[i].ToString(), sb, bluePos, blueLength, line);
					sb.AppendLine(" " + set[i, converter].ToString() + unit);
					line++;
				}
				sb.AppendLine("[" + set.name + "]" + " [数据长度:(" + set.Count + ")] [平均值:(" + set.getMean(converter) + unit + ")] [极限偏差:(" + (converter(set.Max - set.Min)) + unit + ")]");
				line++;
				if (index > 0)
				{
					double dy = converter(set.Mean - manager[index - 1].Mean);
					sb.AppendLine("[变化:(" + dy + unit + ")]");
					line++;

				}
				if (baseSet != null)
				{

					var s = baseSet;
					sb.AppendLine("[" + s.name + "]" + " [数据长度:(" + s.Count + ")] [平均值:(" + s.getMean(converter) + unit + ")] [极限偏差:(" + (converter(s.Max - s.Min)) + unit + ")]");
					line++;
					sb.AppendLine("[比较]" + " [平均值变化:(" + converter(set.Mean - s.Mean) + unit + ")] [极限偏差变化:(" + (converter(set.Max - set.Min - s.Max + s.Min)) + unit + ")]");
					line++;

				}

				if (checkBox2.Checked && manager.Count > 0)
				{
					DataConverter rc = d => d;
					if (refConverter != null)
					{
						rc = refConverter;
					}
					double[] rdata = manager.getDataFromMean(index + 1, converter);
					double[] refdata = manager.getRefData(manager.getDataFromDescribe(index + 1, refConverter), rdata[0]);
					//r2
					double refd = refdata[index].keep(2);
					double readd = rdata[index].keep(2);
					double r2 = DataManager.CalculateRSquared(refdata, rdata);
					sb.AppendLine("[参考]" + "[参考值为: " + refd + unit + "] [测量值为:" + readd + unit + "] ");
					line++;
					if (Math.Abs(refd - readd) < maxRef)
					{
						addWithColor("[相差:" + (refd - readd).keep(2) + unit + "] ", sb, greenPos, greenLength, line);
					}
					else
					{

						addWithColor("[相差:" + (refd - readd).keep(2) + unit + "] ", sb, redPos, redLength, line);
					}
					if (r2 >= (double)numericUpDown4.Value)
					{
						addWithColor("[R2:" + r2 + "]", sb, greenPos, greenLength, line);
					}
					else
					{

						addWithColor("[R2:" + r2 + "]", sb, redPos, redLength, line);
					}

					line++;
				}

				richTextBox3.Text = sb.ToString();
				for (int i = 0; i < blueLength.Count; i++)
				{
					richTextBox3.SelectionStart = bluePos[i];
					richTextBox3.SelectionLength = blueLength[i];
					richTextBox3.SelectionColor = Color.Blue;

				}
				for (int i = 0; i < greenLength.Count; i++)
				{
					richTextBox3.SelectionStart = greenPos[i];
					richTextBox3.SelectionLength = greenLength[i];
					richTextBox3.SelectionColor = Color.Green;
				}
				for (int i = 0; i < redLength.Count; i++)
				{
					richTextBox3.SelectionStart = redPos[i];
					richTextBox3.SelectionLength = redLength[i];
					richTextBox3.SelectionColor = Color.Red;
				}
				richTextBox3.Select(richTextBox3.Text.Length - 1, 0);
				richTextBox3.ScrollToCaret();
			}
			else
			{

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

				manager.addValue(comboBox1.SelectedIndex, managerM.Run(textBox2.Text).getValue(1, 2, 3));
				codeInput();
				textBox2.Focus();

			}
		}

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			updateSelect1();
			updateCombo2();
			lastSelect = -1;
			if (comboBox1.SelectedItem is DataSet)
				lastSelectDataset = comboBox1.SelectedIndex;
			comboBox2.SelectedItem = null;
			comboBox2.Text = "";
			updateSetInfo(converter, unit);
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
			registerFunc();
			readDescribe(manager.describe);
			settings.lastName = manager.name;
			settings.save("settings.data");
		}

		private void button6_Click(object sender, EventArgs e)
		{
			lastSelectDataset = -1;

			if (File.Exists("data\\" + manager.name + ".data"))
			{
				manager.load("data\\" + manager.name + ".data");
				updateCombo1();
				richTextBox1.Text = manager.describe;
				converter = null;
				unit = null;
				managerM.clear();
				registerFunc();
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
			comboBox1.SelectedItem = null;
			lastSelectDataset = -1;
		}

		void codeInput()
		{
			if (textBox2.Text != null && textBox2.Text.Length > 0)
			{
				codeRecord.add(textBox2.Text);
				textBox2.Text = "";
			}


		}

		void nextCode()
		{
			textBox2.Text = codeRecord.rollBack();
		}
		void lastCode()
		{
			textBox2.Text = codeRecord.rollOut();
		}
		private void button9_Click_1(object sender, EventArgs e)
		{
			managerM.Run(textBox2.Text).getValue(1, 2, 3);
			codeInput();
			textBox2.Focus();
		}

		int[] getOrder(string s)
		{
			int[] check = new int[4];
			int[] r = new int[4];
			for (int i = 0; i < 4; i++)
			{
				try
				{
					r[i] = s[i] - '0';
					check[r[i]]++;
				}
				catch (Exception e)
				{
					return defaultNum;
				}

			}

			if (check[0] == 1 && check[1] == 1 && check[2] == 1 && check[3] == 1)
			{
				return r;

			}

			return defaultNum;
		}
		private void button10_Click(object sender, EventArgs e)
		{
			Int16[] is16 = readLZZ(richTextBox4.Text, getOrder(textBox3.Text));
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
			int offset = (int)numericUpDown2.Value;
			int j = 0;
			int c = 0;
			for (int i = 0; i < is16.Length; i++)
			{
				j = offset + i;
				if (j < is16.Length && (!checkBox1.Checked || managerM.Run(textBox1.Text).getValue(j) > 0))
				{
					manager.addValue(index, is16[j]);
					c++;
					if (c >= max)
					{
						break;
					}
				}
			}
			int tt = (int)numericUpDown3.Value;
			for (int i = 0; i < tt; i++)
			{
				double minV = manager[index].Min;
				double maxV = manager[index].Max;
				for (int t = 0; t < manager[index].Count; t++)
				{
					if (manager[index][t] == minV)
					{
						manager.removeValue(index, t);
						break;
					}
				}
				for (int t = 0; t < manager[index].Count; t++)
				{
					if (manager[index][t] == maxV)
					{
						manager.removeValue(index, t);
						break;
					}
				}
			}
		}

		private void richTextBox4_TextChanged(object sender, EventArgs e)
		{


		}

		private void label5_Click(object sender, EventArgs e)
		{

		}

		private void button11_Click(object sender, EventArgs e)
		{
			saveFileDialog1.Filter = "Excel表格|*.xlsx";
			DialogResult r = saveFileDialog1.ShowDialog();
			if (r == DialogResult.OK)
			{
				helper.saveExcel(fsNmae, 2, 2, converter, unit, refConverter);

			}

		}

		private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
		{

			updateSetInfo(converter, unit);
		}

		private void button2_Click_1(object sender, EventArgs e)
		{

		}

		private void button4_Click_1(object sender, EventArgs e)
		{

		}

		private void Form1_Load(object sender, EventArgs e)
		{

		}

		private void textBox1_TextChanged_1(object sender, EventArgs e)
		{

		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void textBox3_TextChanged(object sender, EventArgs e)
		{

		}

		private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
		{

		}

		private void button12_Click(object sender, EventArgs e)
		{
			comboBox4.SelectedItem = null;
			comboBox4.Text = "";
		}

		private void richTextBox3_TextChanged(object sender, EventArgs e)
		{

		}

		private void checkBox2_CheckedChanged(object sender, EventArgs e)
		{

		}

		private void numericUpDown4_ValueChanged(object sender, EventArgs e)
		{

		}

		private void button13_Click(object sender, EventArgs e)
		{
			move.start();
			move.onPositionChanged += d => { label11.Text = d.ToString(); };
		}

		private void label11_Click(object sender, EventArgs e)
		{

		}
	}
}