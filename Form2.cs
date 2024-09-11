using DVLib.LabDataHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabDataHelper
{

	public partial class Form2 : Form
	{
		DataManager dataManager;
		public Form2(DataManager manager)
		{
			InitializeComponent();
			this.dataManager = manager;
			if (dataManager != null && dataManager.Count > 0)

			{

			textBox1.Text = dataManager[dataManager.Count - 1].name;

				richTextBox1.Text = dataManager[dataManager.Count - 1].describe;
				if(dataManager.Count > 1)
				{
		int num = getNum(dataManager[dataManager.Count - 1].name);
				int num2 = getNum(dataManager[dataManager.Count - 2].name);
				int d = num - num2;
				if (d!=0) {
					textBox1.Text = dataManager[dataManager.Count - 1].name.Replace(num.ToString(),(num+d).ToString());
				}
				}
			}
		}

		
		int getNum(string s)
		{
			List<char> list = new List<char>();
			foreach (char c in s)
			{
				if(c>='0'&&c<='9')
				{
					list.Add(c);
				}
			}
			if (list.Count > 0)
			return int.Parse( new string(list.ToArray()));
			return 0;
		}
		private void button1_Click(object sender, EventArgs e)
		{
			lock (dataManager)
			{
				dataManager.addNewData(textBox1.Text, richTextBox1.Text);
			}
			this.Close();
			this.Dispose();
		}

		private void Form2_Load(object sender, EventArgs e)
		{

		}
	}
}
