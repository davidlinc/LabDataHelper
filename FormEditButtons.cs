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
	public partial class FormEditButtons : Form
	{
		Action<string> action;
		public FormEditButtons(string t, Action<string> action)
		{
			InitializeComponent();
			this.action = action;
			richTextBox1.Text = t;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			action(richTextBox1.Text);
			Close();
		}
	}
}
