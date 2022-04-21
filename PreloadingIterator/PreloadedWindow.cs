namespace PreloadingIterator;

public record struct PreloadedWindow(int? start, int? end)
{
    public PreloadedWindow(PreloadedWindow window) : this(window.start, window.end)
    {
    }
}
