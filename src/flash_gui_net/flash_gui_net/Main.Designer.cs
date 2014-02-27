namespace flash_gui_net
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.injectButton = new System.Windows.Forms.Button();
            this.pickButton = new System.Windows.Forms.Button();
            this.pidBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.setinfoButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.paramList = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.valueBox = new System.Windows.Forms.TextBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.injectButton);
            this.groupBox1.Controls.Add(this.pickButton);
            this.groupBox1.Controls.Add(this.pidBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(317, 73);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // injectButton
            // 
            this.injectButton.Location = new System.Drawing.Point(236, 42);
            this.injectButton.Name = "injectButton";
            this.injectButton.Size = new System.Drawing.Size(75, 20);
            this.injectButton.TabIndex = 3;
            this.injectButton.Text = "Inject";
            this.injectButton.UseVisualStyleBackColor = true;
            this.injectButton.Click += new System.EventHandler(this.injectButton_Click);
            // 
            // pickButton
            // 
            this.pickButton.Location = new System.Drawing.Point(236, 16);
            this.pickButton.Name = "pickButton";
            this.pickButton.Size = new System.Drawing.Size(75, 20);
            this.pickButton.TabIndex = 2;
            this.pickButton.Text = "Pick process";
            this.pickButton.UseVisualStyleBackColor = true;
            this.pickButton.Click += new System.EventHandler(this.pickButton_Click);
            // 
            // pidBox
            // 
            this.pidBox.Location = new System.Drawing.Point(71, 16);
            this.pidBox.MaxLength = 5;
            this.pidBox.Name = "pidBox";
            this.pidBox.Size = new System.Drawing.Size(128, 20);
            this.pidBox.TabIndex = 1;
            this.pidBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.pidBox_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Process ID";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.setinfoButton);
            this.groupBox2.Controls.Add(this.saveButton);
            this.groupBox2.Controls.Add(this.paramList);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.valueBox);
            this.groupBox2.Location = new System.Drawing.Point(12, 91);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(317, 162);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Flash info";
            // 
            // setinfoButton
            // 
            this.setinfoButton.Location = new System.Drawing.Point(236, 133);
            this.setinfoButton.Name = "setinfoButton";
            this.setinfoButton.Size = new System.Drawing.Size(75, 20);
            this.setinfoButton.TabIndex = 5;
            this.setinfoButton.Text = "Set info";
            this.setinfoButton.UseVisualStyleBackColor = true;
            this.setinfoButton.Click += new System.EventHandler(this.setinfoButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(260, 45);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(51, 20);
            this.saveButton.TabIndex = 4;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // paramList
            // 
            this.paramList.FormattingEnabled = true;
            this.paramList.Items.AddRange(new object[] {
            "OS Name",
            "OS Language",
            "Resolution",
            "Flash Version (Part 1)",
            "Flash Version (Part 2)"});
            this.paramList.Location = new System.Drawing.Point(9, 19);
            this.paramList.Name = "paramList";
            this.paramList.Size = new System.Drawing.Size(140, 134);
            this.paramList.TabIndex = 2;
            this.paramList.SelectedIndexChanged += new System.EventHandler(this.paramList_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(155, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Value";
            // 
            // valueBox
            // 
            this.valueBox.Location = new System.Drawing.Point(195, 19);
            this.valueBox.MaxLength = 64;
            this.valueBox.Name = "valueBox";
            this.valueBox.Size = new System.Drawing.Size(116, 20);
            this.valueBox.TabIndex = 0;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(271, 256);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(58, 13);
            this.linkLabel1.TabIndex = 2;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "(c) kaimi.ru";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 272);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Flash Info Changer";
            this.Load += new System.EventHandler(this.Main_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button injectButton;
        private System.Windows.Forms.Button pickButton;
        private System.Windows.Forms.TextBox pidBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button setinfoButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.ListBox paramList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox valueBox;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}

