namespace LabDataHelper
{
	partial class FormEditButtons
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
			folderBrowserDialog1 = new FolderBrowserDialog();
			richTextBox1 = new RichTextBox();
			button1 = new Button();
			SuspendLayout();
			// 
			// richTextBox1
			// 
			richTextBox1.Location = new Point(12, 12);
			richTextBox1.Name = "richTextBox1";
			richTextBox1.Size = new Size(561, 426);
			richTextBox1.TabIndex = 0;
			richTextBox1.Text = "";
			// 
			// button1
			// 
			button1.Location = new Point(608, 392);
			button1.Name = "button1";
			button1.Size = new Size(150, 46);
			button1.TabIndex = 1;
			button1.Text = "确定";
			button1.UseVisualStyleBackColor = true;
			button1.Click += button1_Click;
			// 
			// FormEditButtons
			// 
			AutoScaleDimensions = new SizeF(14F, 31F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(800, 450);
			Controls.Add(button1);
			Controls.Add(richTextBox1);
			Name = "FormEditButtons";
			Text = "编辑按钮";
			ResumeLayout(false);
		}

		#endregion

		private FolderBrowserDialog folderBrowserDialog1;
		private RichTextBox richTextBox1;
		private Button button1;
	}
}