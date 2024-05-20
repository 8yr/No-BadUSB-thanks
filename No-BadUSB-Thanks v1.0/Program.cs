using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Threading;
class Program
{
    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool BlockInput([MarshalAs(UnmanagedType.Bool)] bool fBlockIt);
    static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("BadUSB Monitor is running... \nYou can now use your computer as usual.");
            ManagementEventWatcher watcher = new ManagementEventWatcher();
            WqlEventQuery query = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_LogicalDisk'");
            watcher.EventArrived += (sender, e) =>
            {
                ManagementBaseObject instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
                string driveLetter = instance["DeviceID"].ToString();
                if (IsDriveRemovable(driveLetter))
                {
                    string Label = VolumeLabel(driveLetter);
                    try
                    {
                        if (Label != null || Label.Contains("CIRCUITPY"))
                        {
                            Console.WriteLine("USB-Label contains='CIRCUITPY' Ejecting...");
                            if (BlockInput(true))
                            {
                                Console.WriteLine("Keyboard and mouse input blocked for safety...");
                                EjectDrive(driveLetter);
                            }
                            else
                            {
                                Console.WriteLine("Failed to block keyboard and mouse input... \nPlease make sure to remove the USB device during the restart process.");
                                Thread.Sleep(4000);
                                Process.Start("shutdown", "/r /t 0");
                            }
                        }
                        else
                        {
                            Console.WriteLine("USB-Label does not contain='CIRCUITPY' Checking for .dd % .bin files...");
                        }
                        string[] filesdd = Directory.GetFiles(driveLetter, "*.dd", SearchOption.AllDirectories);
                        string[] filesbin = Directory.GetFiles(driveLetter, "*.bin", SearchOption.AllDirectories);
                        if (filesbin.Length > 0 || filesdd.Length > 0)
                        {
                            if (filesbin.Length > 0)
                            {
                                Console.WriteLine("Found .bin files. Ejecting USB drive...");
                                foreach (string file in filesbin)
                                {
                                    File.Delete(file);
                                }
                            }
                            else if (filesdd.Length > 0)
                            {
                                Console.WriteLine("Found .dd files. Ejecting USB drive...");
                                foreach (string file in filesdd)
                                {
                                    File.Delete(file);
                                }
                            }
                            if (BlockInput(true))
                            {
                                Console.WriteLine("Keyboard and mouse input blocked for safety");
                                EjectDrive(driveLetter);
                            }
                            else
                            {
                                Console.WriteLine("Failed to block keyboard and mouse input... \nPlease make sure to remove the USB device during the restart process.");
                                Thread.Sleep(4000);
                                Process.Start("shutdown", "/r /t 0");
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("There was an error while dealing with the USB device...\nDetails :" + ex.Message + "\nFor your safety, a restart process will start. Please make sure to remove the USB device during the restart process.");
                        Thread.Sleep(4000);
                        Process.Start("shutdown", "/r /t 0");
                    }
                }
            };
            watcher.Query = query;
            watcher.Start();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            watcher.Stop();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
            Environment.Exit(0);
        }
    }
    static bool IsDriveRemovable(string driveLetter)
    {
        DriveInfo driveInfo = new DriveInfo(driveLetter);
        return driveInfo.DriveType == DriveType.Removable;
    }
    static string VolumeLabel(string driveLetter)
    {
        try
        {
            DriveInfo driveInfo = new DriveInfo(driveLetter);
            return driveInfo.VolumeLabel;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();
            Environment.Exit(0);
        }
        return null;
    }
    static void EjectDrive(string driveLetter)
    {
        string scriptContent = "$driveEject = New-Object -comObject Shell.Application\n" +
                                $"$driveEject.Namespace(17).ParseName(\"{driveLetter}\").InvokeVerb(\"Eject\")";
        string scriptPath = "tmp.ps1";
        File.WriteAllText(scriptPath, scriptContent);
        ProcessStartInfo psi = new ProcessStartInfo();
        psi.FileName = "powershell.exe";
        psi.Arguments = "-ExecutionPolicy Bypass -File \"" + scriptPath + "\"";
        psi.WindowStyle = ProcessWindowStyle.Hidden;
        try
        {
            using (Process process = Process.Start(psi))
            {
                process.WaitForExit();
                int exitCode = process.ExitCode;
                if (exitCode == 0)
                {
                    Console.WriteLine("PowerShell script executed successfully.");
                }
                else
                {
                    Console.WriteLine("An error occurred while executing the PowerShell script... \nFor your safety, a restart process will start. Please make sure to remove the USB device during the restart process.");
                    Thread.Sleep(4000);
                    Process.Start("shutdown", "/r /t 0");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while executing the PowerShell script... \nDetails :" + ex.Message + "\nFor your safety, a restart process will start. Please make sure to remove the USB device during the restart process.");
            Thread.Sleep(4000);
            Process.Start("shutdown", "/r /t 0");
        }
        finally
        {
            File.Delete(scriptPath);
            Console.WriteLine("For your safety, a restart process will start... \nPlease make sure to remove the USB device during the restart process.");
            Thread.Sleep(4000);
            Process.Start("shutdown", "/r /t 0");
        }
    }
}