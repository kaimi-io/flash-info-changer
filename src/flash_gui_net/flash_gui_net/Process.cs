using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace flash_gui_net
{
    public partial class Process : Form
    {
        private string _Value;
        static private string Separator = " - ";

        public Process()
        {
            InitializeComponent();
        }

        private void Process_Load(object sender, EventArgs e)
        {
            processListBox.Items.Clear();

            System.Diagnostics.Process[] ProcessList = System.Diagnostics.Process.GetProcesses();

            Array.Sort
            (
                ProcessList,
                delegate(System.Diagnostics.Process Proc1, System.Diagnostics.Process Proc2)
                {
                    return Proc1.ProcessName.CompareTo(Proc2.ProcessName);
                }
            );

            foreach (System.Diagnostics.Process Process in ProcessList)
            {
                processListBox.Items.Add(Process.ProcessName + Separator + Process.Id.ToString());
            }
        }

        public string Value
        {
            get { return this._Value; }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string SelectedItem = processListBox.SelectedItem.ToString();

            if (!String.IsNullOrEmpty(SelectedItem))
            {
                string[] Parts = SelectedItem.Split(new string[] { Separator }, StringSplitOptions.None);
                _Value = Parts[1];
            }
            

            Close();
        }
    }
}
