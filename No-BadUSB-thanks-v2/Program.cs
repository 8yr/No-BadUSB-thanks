using System;
using System.Diagnostics;
using System.Management;
class BadUSB
{
    static void Main()
    {
        Console.Title = "BadUSB's remover, (CTRL + C) to exit, @SirMBA";
        while (true)
        {
            var searcher = new ManagementObjectSearcher(@"SELECT * FROM Win32_PnPEntity WHERE Description LIKE '%Keyboard%'");
            foreach (var device in searcher.Get())
            {
                string deviceId = device["DeviceID"].ToString();
                if (deviceId.Contains("PID_"))
                {
                    int pidIndex = deviceId.IndexOf("PID_") + 4;
                    string pid = deviceId.Substring(pidIndex, 4);
                    if (!pid.StartsWith("00"))
                    {
                        int vidStartIndex = deviceId.IndexOf("HID");
                        int vidEndIndex = deviceId.IndexOf("&", deviceId.IndexOf("PID_"));
                        if (vidStartIndex != -1 && vidEndIndex != -1)
                        {
                            string shortDeviceId = deviceId.Substring(vidStartIndex, vidEndIndex - vidStartIndex);
                            Console.WriteLine("BadUSB found: " + shortDeviceId);
                            removeDevice(shortDeviceId);
                        }
                    }
                }
                if (deviceId.Contains("HID\\{"))
                {
                    int startIndex = deviceId.IndexOf("HID\\{");
                    int endIndex = deviceId.IndexOf("}", startIndex) + 1;
                    if (startIndex != -1 && endIndex != -1)
                    {
                        string shortDeviceId = deviceId.Substring(startIndex, endIndex - startIndex);
                        Console.WriteLine("BadUSB found: " + shortDeviceId);
                        removeDevice(shortDeviceId);
                    }
                }
            }
        }
    }
    static void removeDevice(string deviceId)
    {
        string devConPath = @"devcon.exe";
        string arguments = $"remove \"{deviceId}*\"";
        ProcessStartInfo processStartInfo = new ProcessStartInfo
        {
            FileName = devConPath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        try
        {
            using (Process process = Process.Start(processStartInfo))
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                Console.WriteLine(output);
                if (process.ExitCode == 0)
                {
                    Console.WriteLine($"Device {deviceId} removed successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to remove device {deviceId}. Exit code: {process.ExitCode}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}