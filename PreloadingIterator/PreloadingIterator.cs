using MoreLinq;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;

namespace PreloadingIterator;

public class PreloadingIterator<TIn, TOut> : IEnumerable<TOut>, IDisposable
    where TOut : class
{
    private bool _alive = true;

    private TOut?[] transformed;

    private PreloadedWindow _window = new PreloadedWindow();
    public PreloadedWindow Window { get => new PreloadedWindow(_window); }

    public int Count { get; }

    public PreloadingIterator(
        IEnumerable<TIn> iterator,
        Func<TIn, TOut> transform,
        ulong freeMemoryLimitKb = 1024 * 1024,
        int? numThreads = null,
        Func<MemoryInfo,bool>? canPreload = null)
    {
        Iterator = iterator ?? throw new ArgumentNullException(nameof(iterator));
        Transform = transform ?? throw new ArgumentNullException(nameof(transform));
        FreeMemoryLimitKb = freeMemoryLimitKb;

        this.Count = this.Iterator.Count();
        canPreload = canPreload ?? new Func<MemoryInfo,bool>(memInfo => this.CanPreload(memInfo));

        transformed = new TOut[this.Count];

        numThreads = numThreads ?? Environment.ProcessorCount - 1;
        var threads = new Thread[numThreads.Value];

        for (var _i = 0; _i < numThreads; _i++)
        {
            var i = _i;

            new Thread(() =>
            {
                for (var j = i; j < transformed.Length && _alive; j += numThreads.Value)
                {
                    if (!_alive) break;
                    var spins = 0;
                    while (!canPreload(new MemoryInfo()) && _alive)
                    {
                        Thread.Sleep(100);
                        ++spins;
                        if (spins >= 10)
                        {
                            GC.Collect();
                            spins = 0;
                        }
                    }

                    var @in = iterator.ElementAt(j);
                    var @out = this.Transform(@in);

                    lock (transformed)
                    {
                        if (_alive)
                            transformed[j] = @out;
                        if (_window.end == null || _window.end < j)
                            _window.end = j;
                    }
                }
            })
            {
                Name = $"PreloadingImageIterator thread #{i}"
            }
                .Start();
        }
    }

    ~PreloadingIterator()
    {
        _alive = false;
    }

    private bool CanPreload(MemoryInfo memInfo)
    {
        return memInfo.FreePhysicalMemory >= this.FreeMemoryLimitKb;
    }

    public IEnumerable<TIn> Iterator { get; }
    public Func<TIn, TOut> Transform { get; }
    public ulong FreeMemoryLimitKb { get; }

    public IEnumerator<TOut> GetEnumerator()
    {
        var count = this.Iterator.Count();
        for (var i = 0; i < count; i++)
        {
            TOut? @out = null;
            while (transformed[i] == null)
            {
                Thread.Sleep(100);
            }
            @out = transformed[i];
            yield return @out;
            (@out as IDisposable)?.Dispose();
            transformed[i] = null;
            if (_window.start == null)
                _window.start = 0;
            else
                ++_window.start;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Dispose()
    {
        _alive = false;
        (this.Iterator as IDisposable)?.Dispose();
        foreach (var @out in this.transformed
            .Where(x => x != null)
            .OfType<IDisposable>())
            if (@out is IDisposable d)
                d?.Dispose();
    }
}
