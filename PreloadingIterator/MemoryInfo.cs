using System.Diagnostics;
using System.Management;

namespace PreloadingIterator;

public class MemoryInfo
{

    public MemoryInfo()
    {
        ObjectQuery wql = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
        ManagementObjectSearcher searcher = new ManagementObjectSearcher(wql);
        var result = searcher.Get().Cast<ManagementObject>().Single();

        this.TotalVisibleMemorySize = (ulong)result["TotalVisibleMemorySize"] * 1024;
        this.FreePhysicalMemory = (ulong)result["FreePhysicalMemory"] * 1024;
        this.TotalVirtualMemorySize = (ulong)result["TotalVirtualMemorySize"] * 1024;
        this.FreeVirtualMemory = (ulong)result["FreeVirtualMemory"] * 1024;

        var process = Process.GetCurrentProcess();
        using (var pc = new PerformanceCounter())
        {
            pc.CategoryName = "Process";
            pc.CounterName = "Working Set - Private";
            pc.InstanceName = process.ProcessName;  
            this.ProcessUsedMemory = Convert.ToUInt64(pc.NextValue());
        }
    }

    public ulong ProcessUsedMemory { get; }
    public ulong TotalVisibleMemorySize { get; }
    public ulong FreePhysicalMemory { get; }
    public ulong TotalVirtualMemorySize { get; }
    public ulong FreeVirtualMemory { get; }
}
