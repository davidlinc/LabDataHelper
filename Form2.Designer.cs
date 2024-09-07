namespace LabDataHelper
{
	partial class Form2
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			textBox1 = new TextBox();
			richTextBox1 = new RichTextBox();
			button1 = new Button();
			label1 = new Label();
			label2 = new Label();
			SuspendLayout();
			// 
			// textBox1
			// 
			textBox1.Location = new Point(110, 29);
			textBox1.Name = "textBox1";
			textBox1.Size = new Size(200, 38);
			textBox1.TabIndex = 0;
			// 
			// richTextBox1
			// 
			richTextBox1.Location = new Point(110, 91);
			richTextBox1.Name = "richTextBox1";
			richTextBox1.Size = new Size(200, 192);
			richTextBox1.TabIndex = 1;
			richTextBox1.Text = "";
			// 
			// button1
			// 
			button1.Location = new Point(353, 29);
			button1.Name = "button1";
			button1.Size = new Size(150, 46);
			button1.TabIndex = 2;
			button1.Text = "添加";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new Point(12, 32);
			label1.Name = "label1";
			label1.Size = new Size(62, 31);
			label1.TabIndex = 3;
			label1.Text = "名称";
			// 
			// label2
			// 
			label2.AutoSize = true;
			label2.Location = new Point(12, 94);
			label2.Name = "label2";
			label2.Size = new Size(62, 31);
			label2.TabIndex = 4;
			label2.Text = "描述";
			// 
			// Form2
			// 
			AutoScaleDimensions = new SizeF(14F, 31F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 316);
			Controls.Add(label2);
			Controls.Add(label1);
			Controls.Add(button1);
			Controls.Add(richTextBox1);
			Controls.Add(textBox1);
			Name = "Form2";
			Text = "添加数据组";
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion

		private TextBox textBox1;
		private RichTextBox richTextBox1;
		private Button button1;
		private Label label1;
		private Label label2;
	}
}