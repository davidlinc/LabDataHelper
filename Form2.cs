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
	}
}
