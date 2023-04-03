using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Forms;
using System.Management;
using System.Runtime.InteropServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using MetroFramework.Controls;
using LibreHardwareMonitor.Hardware;
using LibreHardwareMonitor;

namespace InfoTool
{
    public partial class Form1 : MetroForm
    {
        private long counter;
        private float cpu;
        private float ram;
        private ulong installedMemory;

       

        public Form1()
        {
            InitializeComponent();

        }

     private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string key = string.Empty;
            switch (toolStripComboBox1.SelectedItem.ToString())
            {
                case "CPU":
  
                    key = "Win32_Processor";
                    break;
                case "Graphics card":
                    key = "Win32_VideoController";
                    break;
                case "Chipset":
                    key = "Win32_IDEController";
                    break;
                case "Battery":
                    key = "Win32_Battery";
                    break;
                case "BIOS":
                    key = "Win32_BIOS";
                    break;
                case "RAM":
                    key = "Win32_PhysicalMemory";
                    break;
                case "Cache":
                    key = "Win32_CacheMemory";
                    break;
                case "USB":
                    key = "Win32_USBController";
                    break;
                case "Disk":
                    key = "Win32_DiskDrive";
                    break;
                case "Logical disks":
                    key = "Win32_LogicalDisk";
                    break;
                case "Keyboard":
                    key = "Win32_Keyboard";
                    break;
                case "Network":
                    key = "Win32_NetworkAdapter";
                    break;
                case "Users":
                    key = "Win32_Account";
                    break;
                default:
                    key = "Win32_Processor";
                    break;
            }
            GetHardWareInfo(key, listView1);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            toolStripComboBox1.SelectedIndex = 0;
            string cpuDesc="";
            string cpuThreads="";
            ManagementObjectSearcher searcher2 = new ManagementObjectSearcher("SELECT Name FROM Win32_Processor");
            foreach (ManagementObject obj in searcher2.Get())
            {
                cpuDesc = obj["Name"].ToString();
            }
            ManagementObjectSearcher searcher3 = new ManagementObjectSearcher("SELECT NumberOfLogicalProcessors FROM Win32_Processor");
            foreach (ManagementObject obj in searcher3.Get())
            {
                cpuThreads = obj["NumberOfLogicalProcessors"].ToString();
            }

            metroLabel12.Text = cpuDesc + " " + cpuThreads + "Threads";
            // toolStripComboBox1.SelectedIndex = 0;
            MEMORYSTATUSEX mEMORYSTATUSEX= new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(mEMORYSTATUSEX))
            {
                installedMemory = mEMORYSTATUSEX.ullTotalPhys;
            }
            metroLabel10.Text = Convert.ToString(installedMemory/1000000000) + " GB";

            timer1.Interval = 1000;

            timer1.Start();
        }

        private void GetHardWareInfo(string key, System.Windows.Forms.ListView list)
        {
            list.Items.Clear();
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM " + key);
            try
            {
                foreach (ManagementObject obj in searcher.Get())
                {
                    if (obj.Properties.Count == 0)
                    {
                        MessageBox.Show("Unable to receive info", "Error",
                       MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    ListViewGroup listViewGroup;
                    try
                    {
                        listViewGroup = list.Groups.Add(obj["Name"].ToString(),
                        obj["Name"].ToString());
                    }
                    catch (Exception ex)
                    {
                        listViewGroup = list.Groups.Add(obj.ToString(), obj.ToString());
                    }
                    foreach (PropertyData data in obj.Properties)
                    {
                        ListViewItem item = new ListViewItem(listViewGroup);
                        if (list.Items.Count % 2 == 0)
                        {
                            item.BackColor = Color.WhiteSmoke;
                        }
                        item.Text = data.Name;

                        if (data.Value != null && !string.IsNullOrEmpty(data.Value.ToString()))
                        {
                            string resStr = string.Empty;
                            switch (data.Value.GetType().ToString())
                            {
                                case "System.String[]":
                                    string[] stringData = data.Value as string[];
                                    //string resStr1 = string.Empty;
                                    foreach (string s in stringData)
                                    {
                                        resStr += s + " ";
                                    }
                                    item.SubItems.Add(resStr);
                                    break;
                                case "System.UInt16[]":
                                    ushort[] ushortData = data.Value as ushort[];
                                    //string resStr2 = string.Empty;
                                    foreach (ushort us in ushortData)
                                    {
                                        resStr += us.ToString() + " ";
                                    }
                                    item.SubItems.Add(resStr);
                                    break;
                                default:
                                    item.SubItems.Add(data.Value.ToString());
                                    break;
                            }
                            list.Items.Add(item);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            }
        }
    

private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(tabControl1.SelectedTab == tabPage2)
            {
                toolStripComboBox1.SelectedIndex = 1;

            }
        }

        private void metroLabel3_Click(object sender, EventArgs e)
        {

        }

        private void metroLabel6_Click(object sender, EventArgs e)
        {

        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLength;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true) ]
        static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        private void timer1_Tick(object sender, EventArgs e)
        {
            counter++;
            cpu = performanceCPU.NextValue();
            ram = performanceRam.NextValue();


            metroProgressBar1.Value = (int)cpu;
            metroProgressBar2.Value = (int)ram;

            metroLabel3.Text = Convert.ToString(Math.Round(cpu, 1)) + "%";
            metroLabel5.Text = Convert.ToString(Math.Round(ram / 1073741824, 1)) + "%";
        

            metroLabel9.Text = Convert.ToString(Math.Round((ram/ 100* installedMemory)/ 1073741824, 1)) + " GB";
            metroLabel11.Text = Convert.ToString(Math.Round((installedMemory -  ram / 100 * installedMemory) / 1073741824, 1)) + " GB";

            chart1.Series["CPU load"].Points.AddY(cpu);
            chart2.Series["RAM used"].Points.AddY(ram);
            if (counter >= 100) { 
                chart1.Series["CPU load"].Points.RemoveAt(0);
                chart2.Series["RAM used"].Points.RemoveAt(0);
            }
        }
    }
}
