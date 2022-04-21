namespace PreloadingIterator;

public struct PreloadingIteratorMemoryLimits
{

    public static PreloadingIteratorMemoryLimits Max { get => new PreloadingIteratorMemoryLimits(freeMemoryLimitBytes:0, maxMemoryBytes:null); }

    private PreloadingIteratorMemoryLimits(
        ulong? freeMemoryLimitBytes, 
        ulong? maxMemoryBytes)
    {
        FreeMemoryLimitBytes = freeMemoryLimitBytes;
        MaxMemoryBytes = maxMemoryBytes;
    }

    public PreloadingIteratorMemoryLimits(PreloadingIteratorMemoryLimits limits) : this(limits.FreeMemoryLimitBytes, limits.MaxMemoryBytes)
    {
    }

    public PreloadingIteratorMemoryLimits WithMaxMemoryGb(double max)
    {
        var limits = new PreloadingIteratorMemoryLimits(this);
        limits.MaxMemoryBytes = (ulong?)(max * (1024 * 1024 * 1024));
        return limits;
    }
    public PreloadingIteratorMemoryLimits WithFreeLimitGb(double max)
    {
        var limits = new PreloadingIteratorMemoryLimits(this);
        limits.FreeMemoryLimitBytes = (ulong?)(max * (1024 * 1024 * 1024));
        return limits;
    }

    public ulong? FreeMemoryLimitBytes { get; set; } = null;
    public ulong? FreeMemoryLimitKBytes { get => FreeMemoryLimitBytes / 1024; set => FreeMemoryLimitBytes = value * 1024; }

    public ulong? FreeMemoryLimitMBytes { get => FreeMemoryLimitKBytes / 1024; set => FreeMemoryLimitKBytes = value * 1024; }

    public ulong? FreeMemoryLimitGBytes { get => FreeMemoryLimitMBytes / 1024; set => FreeMemoryLimitMBytes = value * 1024; }

    public ulong? MaxMemoryBytes { get; set; } = null;
    public ulong? MaxMemoryKBytes { get => MaxMemoryBytes / 1024; set => MaxMemoryBytes = value * 1024; }

    public ulong? MaxMemoryMBytes { get => MaxMemoryKBytes / 1024; set => MaxMemoryKBytes = value * 1024; }

    public ulong? MaxMemoruyGBytes { get => MaxMemoryMBytes / 1024; set => MaxMemoryMBytes = value * 1024; }

    public static implicit operator Func<MemoryInfo,bool>(PreloadingIteratorMemoryLimits limits)
    {
        return new Func<MemoryInfo,bool>(memInfo =>
        {
            if (limits.FreeMemoryLimitBytes != null &&
                memInfo.FreePhysicalMemory < limits.FreeMemoryLimitBytes.Value)
                return false;

            if (limits.MaxMemoryBytes != null &&
                memInfo.ProcessUsedMemory >= limits.MaxMemoryBytes.Value)
                return false;

            return true;

        });
    }

}