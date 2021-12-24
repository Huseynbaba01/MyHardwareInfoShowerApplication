using System;
using System.Diagnostics;
using System.Windows.Forms;
using ComputerInfo.Define;
using HardwareInfoShowerApplication.Info;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using System.Management;

namespace HardwareInfoShowerApplication
{
    public partial class Main : Form
    {
        private CPU cpu;
        private Bios bios;
        private RAM ram;
        private Disk disk;
        private GPU gpu;
        private OS os;
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            cpu = new CPU();
            bios = new Bios();
            ram = new RAM();
            disk = new Disk();
            gpu = new GPU();
            os = new OS();
            populateMBInfo();
            populateCPUInfo();
            populateRAMInfo();
            populateStorageInfo();
            populateGPUInfo();
            populateOSInfo();


        }

        private void tp_MBoard_Click(object sender, EventArgs e)
        {
            populateMBInfo();
        }

        private void populateMBInfo()
        {
            lbl_base_board_manufacturer.Text = bios.BaseManufacturer;
            lbl_base_board_product.Text = bios.BaseProduct;
            lbl_base_board_version.Text = bios.BaseVersion;

            lbl_bios_release_date.Text = bios.ReleaseDate;
            lbl_bios_version.Text = bios.BiosVersion;
            lbl_bios_vendor.Text = bios.BiosVendor;

            lbl_system_manufacturer.Text = bios.SystemManufacturer;
            lbl_system_version.Text = bios.SystemVersion;

            String Manufacturer = bios.BaseManufacturer;
            if (Manufacturer.ToLower().Contains("msi"))
                wb_mb_logo.Url = new Uri(Constants.MSI_LOGO);
            else if (Manufacturer.ToLower().Contains("asus"))
                wb_mb_logo.Url = new Uri(Constants.ASUS_LOGO);
            else if (Manufacturer.ToLower().Contains("asrock"))
                wb_mb_logo.Url = new Uri(Constants.ASROCK_LOGO);
            else if (Manufacturer.ToLower().Contains("gigabyte"))
                wb_mb_logo.Url = new Uri(Constants.GIGABYTE_LOGO);
            else if (Manufacturer.ToLower().Contains("intel"))
                wb_mb_logo.Url = new Uri(Constants.INTEL_LOGO);
            else if (Manufacturer.ToLower().Contains("biostar"))
                wb_mb_logo.Url = new Uri(Constants.BIOSTAR_LOGO);
            else if (Manufacturer.ToLower().Contains("dell"))
                wb_mb_logo.Url = new Uri(Constants.DELL_LOGO);
            else if (Manufacturer.ToLower().Contains("hp"))
                wb_mb_logo.Url = new Uri(Constants.HP_LOGO);
        }
        private void populateCPUInfo()
        {
            PerformanceCounter cpuCounter;

            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            progress_cpu_util.Value = (int) (cpuCounter.NextValue());
            lbl_cpu_percentage.Text = 100 - cpuCounter.NextValue() + " %";
            lbl_clock.Text = cpu.CurrentClock/1000f+"GHz";
            lbl_current_voltage.Text = cpu.Voltage + " V";
            lbl_l2cache.Text = cpu.L2CacheSize + " MB";
            lbl_l3cache.Text = cpu.L3CacheSize + " MB";
            lbl_cores.Text = cpu.CoreCount + "";
            lbl_threads.Text = cpu.ThreadCount + "";
//          chart1.Series[0].YValuesPerPoint = cpuCounter.NextValue();


        }
        private void populateRAMInfo()
        {
            PerformanceCounter ramCounter
                = new PerformanceCounter("Memory", "Available MBytes");

            lbl_ram_manufacturer.Text = ram.Manufacturer;
            lbl_ram_frequency.Text = ram.Speed + " MHz";
            lbl_ram_voltage.Text = ram.Voltage + " V";
            lbl_p_ram_size.Text = ram.PysicalSize + " Bytes";
            lbl_v_ram_size.Text = ram.VirtualSize + " Bytes";

            //progress_p_ram.Value = ((int) ramCounter.NextValue())/((int)(ram.PysicalSize/1000000));
/*            lbl_p_ram_per.Text = ((int)ramCounter.NextValue()) / ((int)(ram.PysicalSize / 1000000)) + " %";
            lbl_v_ram_per.Text = ramCounter.RawValue+"";*/
        }
        private void populateStorageInfo()
        {
            DriveInfo[] driver = disk.VolumeList.ToArray();
            int count = Math.Min(disk.VolumeCount, 6);
            List<Label> driveList = new List<Label>(6) { 
                                                        lbl_disk_1, 
                                                        lbl_disk_2, 
                                                        lbl_disk_3, 
                                                        lbl_disk_4, 
                                                        lbl_disk_5, 
                                                        lbl_disk_6, 
                                                        lbl_disk_7, 
                                                        lbl_disk_8 };
            List<Label> drivePerList = new List<Label>(6) { 
                                                        lbl_case_1,
                                                        lbl_case_2, 
                                                        lbl_case_3,
                                                        lbl_case_4, 
                                                        lbl_case_5,
                                                        lbl_case_6, 
                                                        lbl_case_7,
                                                        lbl_case_8 };
            List<ProgressBar> driveProgressList = new List<ProgressBar>(6) {
                                                        progress_drive_1,
                                                        progress_drive_2,
                                                        progress_drive_3,
                                                        progress_drive_4,
                                                        progress_drive_5,
                                                        progress_drive_6,
                                                        progress_drive_7,
                                                        progressBar1 };
            for (int i = 0; i < count; i++)
            {
                driveList[i].Text = driver[i].Name;
                if (driver[i].IsReady)
                {
                    drivePerList[i].Text = String.Format("{0:F2} GB / {1:F2} GB", (driver[i].TotalFreeSpace / 1024f / 1024f / 1024f), (driver[i].TotalSize / 1024f / 1024f / 1024f));
                    int value = (int)(100 * driver[i].TotalFreeSpace / driver[i].TotalSize);
                    driveProgressList[i].Value = value;
                    if (value >= 20)
                    {
                        driveProgressList[i].ForeColor = Color.Red;
                        MessageBox.Show("Your " + driver[i] + " drive's free space is going to over soon! :)");
                    }
                }
                else
                {
                    drivePerList[i].Text = "Not ready";
                    drivePerList[i].Text = "--";
                }

            }
        }
        private void populateGPUInfo()
        {
            String company = gpu.AdapterCompatiability.ToLower();
            if (company.Contains("nvidia"))
                wb_gpu.Url = new Uri(Constants.NVIDIA_LOGO);
            else if (company.Contains("intel"))
                wb_gpu.Url = new Uri(Constants.INTEL_LOGO);
            else
                wb_gpu.Url = new Uri(Constants.AMD_LOGO);

            DateTime driverTime = DateTime.ParseExact(gpu.DriverDate, "yyyyMMddHHmmss", null);
            lbl_gpu_caption.Text = gpu.Caption;
            lbl_gpu_manufacturer.Text = gpu.AdapterCompatiability;
            lbl_v_p_n.Text = gpu.VideoProcessor;
            lbl_gpu_ram.Text = gpu.AdapterRAM;
            lbl_driver_version.Text = gpu.DriverVersion;
            lbl_driver_date.Text = gpu.DriverDate;
            lbl_c_r_r.Text = gpu.CurrentRefreshRate;
            lbl_max_r_r.Text = gpu.MaxRefreshRate;
            lbl_min_r_r.Text = gpu.MinRefreshRate;
            lbl_c_r.Text = gpu.VideoModeDescription;
            lbl_driver_date.Text = String.Format("{0}/{1}/{2}", driverTime.Month, driverTime.Day, driverTime.Year);

        }
        private void populateOSInfo()
        {
            lbl_os_name.Text = os.Caption;
            lbl_arch.Text = os.Architecture;
            lbl_version.Text = os.Version;
            lbl_s_number.Text = os.SerialNumber;
            lbl_c_code.Text = os.ContryCode;
            lbl_c_t_zone_code.Text = os.CurrentTimeZone;
            lbl_MUI_l_code.Text = os.MUILanguages;
            lbl_hint_os_l_code.Text = os.Language;

            DateTime time = os.InstallTime;
            lbl_install_time.Text = time.ToString();
            time = ManagementDateTimeConverter.ToDateTime(os.LastBootUpTime);
            lbl_hint_last_b_up_time.Text = time.ToString();
        }

        private void tp_CPU_Click(object sender, EventArgs e)
        {
            populateCPUInfo();
        }

        private void tp_RAM_Click(object sender, EventArgs e)
        {
            populateRAMInfo();
        }

        private void tp_Storage_Click(object sender, EventArgs e)
        {
            populateStorageInfo();
        }

        private void tp_GPU_Click(object sender, EventArgs e)
        {
            populateGPUInfo();
        }

        private void tabs_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lbl_hint_clock_Click(object sender, EventArgs e)
        {

        }

        private void lbl_hint_current_voltage_Click(object sender, EventArgs e)
        {

        }

        private void lbl_hint_l2cache_Click(object sender, EventArgs e)
        {

        }

        private void lbl_hint_l3chache_Click(object sender, EventArgs e)
        {

        }

        private void lbl_clock_Click(object sender, EventArgs e)
        {

        }

        private void lbl_current_voltage_Click(object sender, EventArgs e)
        {

        }

        private void lbl_l2cache_Click(object sender, EventArgs e)
        {

        }

        private void lbl_l3cache_Click(object sender, EventArgs e)
        {

        }

        private void lbl_hint_cores_Click(object sender, EventArgs e)
        {

        }

        private void lbl_hint_threads_Click(object sender, EventArgs e)
        {

        }

        private void lbl_cores_Click(object sender, EventArgs e)
        {

        }

        private void lbl_threads_Click(object sender, EventArgs e)
        {

        }

        private void gb_cpu_Enter(object sender, EventArgs e)
        {

        }

        private void lbl_p_ram_size_Click(object sender, EventArgs e)
        {

        }

        private void gb_base_info_Enter(object sender, EventArgs e)
        {

        }

        private void gb_info_codes_Enter(object sender, EventArgs e)
        {

        }

        private void gb_other_info_Enter(object sender, EventArgs e)
        {

        }

        private void tp_OS_Click(object sender, EventArgs e)
        {
            populateOSInfo();
        }
    }
}
