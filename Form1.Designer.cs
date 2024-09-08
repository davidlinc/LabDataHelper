namespace LabDataHelper
{
	partial class Form1
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			textBox1 = new TextBox();
			richTextBox1 = new RichTextBox();
			comboBox1 = new ComboBox();
			textBox2 = new TextBox();
			button1 = new Button();
			button2 = new Button();
			richTextBox2 = new RichTextBox();
			richTextBox3 = new RichTextBox();
			comboBox2 = new ComboBox();
			button3 = new Button();
			button4 = new Button();
			button5 = new Button();
			button6 = new Button();
			button7 = new Button();
			label1 = new Label();
			label2 = new Label();
			label3 = new Label();
			label4 = new Label();
			button8 = new Button();
			button9 = new Button();
			SuspendLayout();
			// 
			// textBox1
			// 
			textBox1.Location = new Point(104, 38);
			textBox1.Margin = new Padding(4, 4, 4, 4);
			textBox1.Name = "textBox1";
			textBox1.Size = new Size(278, 38);
			textBox1.TabIndex = 0;
			textBox1.TextChanged += textBox1_TextChanged;
			// 
			// richTextBox1
			// 
			richTextBox1.Location = new Point(104, 98);
			richTextBox1.Margin = new Padding(4, 4, 4, 4);
			richTextBox1.Name = "richTextBox1";
			richTextBox1.Size = new Size(278, 285);
			richTextBox1.TabIndex = 1;
			richTextBox1.Text = "";
			richTextBox1.TextChanged += richTextBox1_TextChanged;
			// 
			// comboBox1
			// 
			comboBox1.FormattingEnabled = true;
			comboBox1.Location = new Point(500, 44);
			comboBox1.Margin = new Padding(4, 4, 4, 4);
			comboBox1.Name = "comboBox1";
			comboBox1.Size = new Size(287, 39);
			comboBox1.TabIndex = 2;
			comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
			// 
			// textBox2
			// 
			textBox2.Location = new Point(502, 417);
			textBox2.Margin = new Padding(4, 4, 4, 4);
			textBox2.Name = "textBox2";
			textBox2.Size = new Size(285, 38);
			textBox2.TabIndex = 3;
			textBox2.TextChanged += textBox2_TextChanged;
			// 
			// button1
			// 
			button1.Location = new Point(645, 463);
			button1.Margin = new Padding(4, 4, 4, 4);
			button1.Name = "button1";
			button1.Size = new Size(142, 46);
			button1.TabIndex = 4;
			button1.Text = "添加";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// button2
			// 
			button2.Location = new Point(1285, 40);
			button2.Margin = new Padding(4, 4, 4, 4);
			button2.Name = "button2";
			button2.Size = new Size(146, 46);
			button2.TabIndex = 5;
			button2.Text = "修改";
			button2.UseVisualStyleBackColor = true;
			button2.Click += button2_Click;
			// 
			// richTextBox2
			// 
			richTextBox2.Location = new Point(500, 100);
			richTextBox2.Margin = new Padding(4, 4, 4, 4);
			richTextBox2.Name = "richTextBox2";
			richTextBox2.Size = new Size(287, 285);
			richTextBox2.TabIndex = 6;
			richTextBox2.Text = "";
			richTextBox2.TextChanged += richTextBox2_TextChanged;
			// 
			// richTextBox3
			// 
			richTextBox3.Location = new Point(967, 100);
			richTextBox3.Margin = new Padding(4, 4, 4, 4);
			richTextBox3.Name = "richTextBox3";
			richTextBox3.ReadOnly = true;
			richTextBox3.Size = new Size(634, 285);
			richTextBox3.TabIndex = 7;
			richTextBox3.Text = "";
			// 
			// comboBox2
			// 
			comboBox2.FormattingEnabled = true;
			comboBox2.Location = new Point(967, 44);
			comboBox2.Margin = new Padding(4, 4, 4, 4);
			comboBox2.Name = "comboBox2";
			comboBox2.Size = new Size(310, 39);
			comboBox2.TabIndex = 8;
			comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
			// 
			// button3
			// 
			button3.Location = new Point(809, 39);
			button3.Margin = new Padding(4, 4, 4, 4);
			button3.Name = "button3";
			button3.Size = new Size(150, 46);
			button3.TabIndex = 9;
			button3.Text = "添加";
			button3.UseVisualStyleBackColor = true;
			button3.Click += button3_Click;
			// 
			// button4
			// 
			button4.Location = new Point(1441, 40);
			button4.Margin = new Padding(4, 4, 4, 4);
			button4.Name = "button4";
			button4.Size = new Size(146, 46);
			button4.TabIndex = 10;
			button4.Text = "删除";
			button4.UseVisualStyleBackColor = true;
			button4.Click += button4_Click;
			// 
			// button5
			// 
			button5.Location = new Point(12, 558);
			button5.Margin = new Padding(4, 4, 4, 4);
			button5.Name = "button5";
			button5.Size = new Size(150, 46);
			button5.TabIndex = 11;
			button5.Text = "保存";
			button5.UseVisualStyleBackColor = true;
			button5.Click += button5_Click;
			// 
			// button6
			// 
			button6.Location = new Point(168, 558);
			button6.Margin = new Padding(4, 4, 4, 4);
			button6.Name = "button6";
			button6.Size = new Size(150, 46);
			button6.TabIndex = 12;
			button6.Text = "加载";
			button6.UseVisualStyleBackColor = true;
			button6.Click += button6_Click;
			// 
			// button7
			// 
			button7.Location = new Point(324, 558);
			button7.Margin = new Padding(4, 4, 4, 4);
			button7.Name = "button7";
			button7.Size = new Size(150, 46);
			button7.TabIndex = 13;
			button7.Text = "清空";
			button7.UseVisualStyleBackColor = true;
			button7.Click += button7_Click;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(406, 41);
			label1.Margin = new Padding(4, 0, 4, 0);
			label1.Name = "label1";
			label1.Size = new Size(86, 31);
			label1.TabIndex = 14;
			label1.Text = "数据组";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(406, 103);
			label2.Margin = new Padding(4, 0, 4, 0);
			label2.Name = "label2";
			label2.Size = new Size(62, 31);
			label2.TabIndex = 15;
			label2.Text = "描述";
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.Location = new Point(12, 102);
			label3.Margin = new Padding(4, 0, 4, 0);
			label3.Name = "label3";
			label3.Size = new Size(62, 31);
			label3.TabIndex = 17;
			label3.Text = "描述";
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Location = new Point(12, 40);
			label4.Margin = new Padding(4, 0, 4, 0);
			label4.Name = "label4";
			label4.Size = new Size(86, 31);
			label4.TabIndex = 16;
			label4.Text = "数据包";
			// 
			// button8
			// 
			button8.Location = new Point(809, 89);
			button8.Margin = new Padding(4, 4, 4, 4);
			button8.Name = "button8";
			button8.Size = new Size(150, 46);
			button8.TabIndex = 18;
			button8.Text = "移除";
			button8.UseVisualStyleBackColor = true;
			button8.Click += button8_Click;
			// 
			// button9
			// 
			button9.Location = new Point(500, 463);
			button9.Margin = new Padding(4);
			button9.Name = "button9";
			button9.Size = new Size(142, 46);
			button9.TabIndex = 19;
			button9.Text = "运行";
			button9.UseVisualStyleBackColor = true;
			button9.Click += button9_Click_1;
			// 
			// Form1
			// 
			AutoScaleDimensions = new SizeF(14F, 31F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(1703, 616);
			Controls.Add(button9);
			Controls.Add(button8);
			Controls.Add(label3);
			Controls.Add(label4);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(button7);
			Controls.Add(button6);
			Controls.Add(button5);
			Controls.Add(button4);
			Controls.Add(button3);
			Controls.Add(comboBox2);
			Controls.Add(richTextBox3);
			Controls.Add(richTextBox2);
			Controls.Add(button2);
			Controls.Add(button1);
			Controls.Add(textBox2);
			Controls.Add(comboBox1);
			Controls.Add(richTextBox1);
			Controls.Add(textBox1);
			Margin = new Padding(4, 4, 4, 4);
			Name = "Form1";
			Text = "计数宝";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private TextBox textBox1;
		private RichTextBox richTextBox1;
		private ComboBox comboBox1;
		private TextBox textBox2;
		private Button button1;
		private Button button2;
		private RichTextBox richTextBox2;
		private RichTextBox richTextBox3;
		private ComboBox comboBox2;
		private Button button3;
		private Button button4;
		private Button button5;
		private Button button6;
		private Button button7;
		private Label label1;
		private Label label2;
		private Label label3;
		private Label label4;
		private Button button8;
		private Button button9;
	}
}