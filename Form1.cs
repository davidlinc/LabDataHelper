using DVLib.LabDataHelper;
using System.Text;

namespace LabDataHelper
{
	public partial class Form1 : Form
	{
		DataManager manager = new DataManager("数据");
		int lastSelect;
		public Form1()
		{
			InitializeComponent();
			manager.OnChnage += onChange;
			updateInfo();
			textBox2.KeyDown += (o, e) => {
			if(e.KeyCode == Keys.Enter)
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
		void updateInfo()
		{

			textBox1.Text = manager.name;
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
			manager.name = textBox1.Text;
		}

		private void button3_Click(object sender, EventArgs e)
		{
			new Form2(manager).ShowDialog();
		}

		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{
			manager.describe = richTextBox1.Text;
		}

		void updateSetInfo()
		{
			if (comboBox1.SelectedItem is DataSet)
			{
				DataSet set = (DataSet)comboBox1.SelectedItem;
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < set.Count; i++)
				{
					sb.AppendLine(set[i].ToString());
				}
				sb.AppendLine("平均值:" + set.Mean + " 极限偏差:" + (set.Max - set.Min));

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
				try
				{
					manager.addValue(comboBox1.SelectedIndex, double.Parse(textBox2.Text));
					textBox2.Text = "";
					textBox2.Focus();
				}
				catch { }
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
			if (!Directory.Exists("data"))
			{
				Directory.CreateDirectory("data");
			}
			manager.save("data\\" + manager.name + ".data");
		}

		private void button6_Click(object sender, EventArgs e)
		{
			if (File.Exists("data\\" + manager.name + ".data"))
			{
				manager.load("data\\" + manager.name + ".data");
				updateCombo1();
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
	}
}