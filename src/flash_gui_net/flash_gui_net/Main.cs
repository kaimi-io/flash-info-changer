using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

namespace flash_gui_net
{
    public struct FlashInfo
    {
        public string OSName;
        public string OSLanguage;
        public int Width;
        public int Height;
        public string FlashVersion1;
        public string FlashVersion2;
    }

    public partial class Main : Form
    {
        private static string DllName = "FlashInfo.dll";
        private static string InfoFilePath = "%TEMP%\\4b01065a-6d66-45aa-a488-c97d314272e9.dat";

        FlashInfo Info = new FlashInfo();

        public Main()
        {
            InitializeComponent();
        }

        private void Error(string Text)
        {
            MessageBox.Show(Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void pickButton_Click(object sender, EventArgs e)
        {
            Process Modal = new Process();
            Modal.ShowInTaskbar = false;
            Modal.ShowDialog();

            if (!String.IsNullOrEmpty(Modal.Value))
                pidBox.Text = Modal.Value;
        }

        private void pidBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Info.OSName = "Windows ViP";
            Info.OSLanguage = "en";
            Info.Width = 1024;
            Info.Height = 768;
            Info.FlashVersion1 = "LOSE 12,3,456,789";
            Info.FlashVersion2 = "Abobe Shindows";
        }

        private void paramList_SelectedIndexChanged(object sender, EventArgs e)
        {
            int Index = paramList.SelectedIndex;

            switch (Index)
            {
                case 0:
                    valueBox.Text = Info.OSName;
                    break;
                case 1:
                    valueBox.Text = Info.OSLanguage;
                    break;
                case 2:
                    valueBox.Text = Info.Width.ToString() + "x" + Info.Height.ToString();
                    break;
                case 3:
                    valueBox.Text = Info.FlashVersion1;
                    break;
                case 4:
                    valueBox.Text = Info.FlashVersion2;
                    break;
            }
        }

        private void ParseResolution(string Data, ref int Width, ref int Height)
        {
            if(String.IsNullOrEmpty(Data))
            {
                Error("Erroneous resolution");
                return;
            }

            string[] Parts = Data.Split('x');
            if(Parts.Length != 2)
            {
                Error("Erroneous resolution");
                return;
            }

            Width = Int32.Parse(Parts[0]);
            Height = Int32.Parse(Parts[1]);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            int Index = paramList.SelectedIndex;

            switch (Index)
            {
                case 0:
                    Info.OSName = valueBox.Text;
                    break;
                case 1:
                    Info.OSLanguage = valueBox.Text;
                    break;
                case 2:
                    ParseResolution(valueBox.Text, ref Info.Width, ref Info.Height);
                    break;
                case 3:
                    Info.FlashVersion1 = valueBox.Text;
                    break;
                case 4:
                    Info.FlashVersion2 = valueBox.Text;
                    break;
            }
        }

        private void injectButton_Click(object sender, EventArgs e)
        {
            string textPID = pidBox.Text;

            if (String.IsNullOrEmpty(textPID))
            {
                Error("Please, specify process PID");
                return;
            }

            if (!File.Exists(DllName))
            {
                Error("Can't find DLL file to inject");
                return;
            }

            string FullPath = Path.GetFullPath(DllName);

            if (InjectDLL(FullPath, System.Diagnostics.Process.GetProcessById(Int16.Parse(textPID))))
                MessageBox.Show("Injected successfully", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                Error("Failed to inject DLL in target PID");
        }

        private void setinfoButton_Click(object sender, EventArgs e)
        {
            string Path = Environment.ExpandEnvironmentVariables(InfoFilePath);

            if(String.IsNullOrEmpty(Path))
            {
                Error("Can't get info file path");
                return;
            }

            try
            {
                using (FileStream Fs = File.Open(Path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    BinaryWriter Bw = new BinaryWriter(Fs);
                    byte[] FieldData;

                    foreach (var Child in Info.GetType().GetFields())
                    {
                        Object Value = Child.GetValue(Info);

                        if(Value.GetType() == typeof(Int32))
                        {
                            FieldData = new byte[4];
                            int Number = (int) Value;

                            for (int i = 0; i <= 3; i++)
                                FieldData[i] = (byte)((Number >> (i * 8)) & 0x000000FF);
                        }
                        else
                        {
                            FieldData = Encoding.ASCII.GetBytes(Value.ToString());
                            Array.Resize(ref FieldData, 64);
                        }

                        Bw.Write(FieldData);
                    }

                    Bw.Close();
                }
            }
            catch (IOException ex)
            {
                Error(ex.Message);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://kaimi.ru");
        }

        public bool InjectDLL(String DLL, System.Diagnostics.Process Proc)
        {
            UIntPtr bWritten;
            IntPtr hThread;

            // Bear in Mind that this does not check the Existance of the DLL
            byte[] DLLName = Encoding.Unicode.GetBytes(DLL);

            // Get the Handle of the kernel32.dll
            IntPtr hKernel = NativeMethods.LoadLibraryW("kernel32.dll");
            // The the Adress of the LoadLibraryA function. The function which will Load our DLL into the Memory
            UIntPtr LoadLib = NativeMethods.GetProcAddress(hKernel, "LoadLibraryW");
            // Release the Handle, as it is not needed anymore
            NativeMethods.FreeLibrary(hKernel);

            // Error. Maybe Misspelling of LoadLibraryA or broken System? :D
            if (LoadLib == UIntPtr.Zero)
                return false;

            // Open the Process.
            IntPtr ProcHandle = NativeMethods.OpenProcess(NativeMethods.ProcessAccess.AllAccess, false, Proc.Id);
            // Couldn't Open Process. GetLastError() would be usefull
            if (ProcHandle == IntPtr.Zero)
                return false;

            // MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE. 
            // THIS Allocates Memory for the PATH of our Library as LoadLibraryA has one argument. We don't need the EXECUTE FLAG as it is just a text String.
            IntPtr cave = NativeMethods.VirtualAllocEx(ProcHandle, (IntPtr)0, (uint)DLLName.Length, 0x1000 | 0x2000, 0x04);

            // System/Process run out of memory? :/
            if (cave == IntPtr.Zero)
                return false;

            // Write the DLLName
            Boolean WPM = NativeMethods.WriteProcessMemory(ProcHandle, cave, DLLName, (uint)DLLName.Length, out bWritten);
            // GetLastError().
            if (WPM == false)
                return false;

            // We start a NEW Thread with "main()" LoadLibraryA and it's argument located in the cave.
            IntPtr hThr = NativeMethods.CreateRemoteThread(ProcHandle, (IntPtr)0, (uint)0, LoadLib, cave, (uint)0, out hThread);
            // maybe security?
            if (hThr == IntPtr.Zero)
                return false;

            /* Here you could use WaitForSingleObjectEx() to wait for the DLL to Loadup and then clean up, if you care ;)
                VirtualFreeEx(Proc, cave, strlen(DLL), MEM_RELEASE);
                CloseHandle(Proc);
            */

            NativeMethods.WaitForSingleObject(hThr, 1000);

            NativeMethods.CloseHandle(hThr);


            return true;
        }
    }

    internal static class NativeMethods /* provides all the wrappers to the WINAPI */
    {
        [Flags()]
        public enum ProcessAccess : int
        {
            /// <summary>Specifies all possible access flags for the process object.</summary>
            AllAccess = CreateThread | DuplicateHandle | QueryInformation | SetInformation | Terminate | VMOperation | VMRead | VMWrite | Synchronize,
            /// <summary>Enables usage of the process handle in the CreateRemoteThread function to create a thread in the process.</summary>
            CreateThread = 0x2,
            /// <summary>Enables usage of the process handle as either the source or target process in the DuplicateHandle function to duplicate a handle.</summary>
            DuplicateHandle = 0x40,
            /// <summary>Enables usage of the process handle in the GetExitCodeProcess and GetPriorityClass functions to read information from the process object.</summary>
            QueryInformation = 0x400,
            /// <summary>Enables usage of the process handle in the SetPriorityClass function to set the priority class of the process.</summary>
            SetInformation = 0x200,
            /// <summary>Enables usage of the process handle in the TerminateProcess function to terminate the process.</summary>
            Terminate = 0x1,
            /// <summary>Enables usage of the process handle in the VirtualProtectEx and WriteProcessMemory functions to modify the virtual memory of the process.</summary>
            VMOperation = 0x8,
            /// <summary>Enables usage of the process handle in the ReadProcessMemory function to' read from the virtual memory of the process.</summary>
            VMRead = 0x10,
            /// <summary>Enables usage of the process handle in the WriteProcessMemory function to write to the virtual memory of the process.</summary>
            VMWrite = 0x20,
            /// <summary>Enables usage of the process handle in any of the wait functions to wait for the process to terminate.</summary>
            Synchronize = 0x100000
        }

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern IntPtr LoadLibraryW(string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr OpenProcess(ProcessAccess dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, UInt32 flAllocationType, UInt32 flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out UIntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, UIntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, out IntPtr lpThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern int WaitForSingleObject(IntPtr hHandle, int dwMilliseconds);
    }
}
