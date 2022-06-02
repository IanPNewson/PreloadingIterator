using PreloadingIterator;
using System.Drawing;
using System.Runtime.InteropServices;

public class PreloadingImageBytesIterator : PreloadingIterator<FileInfo, ImageBytes>
{
    public PreloadingImageBytesIterator(
        IEnumerable<FileInfo> iterator,
        PreloadingIteratorMemoryLimits limits,
        Size? targetSize = null)
        : base(iterator, (FileInfo file) => FileToImageTransform(file, targetSize), canPreload: limits)
    {
    }

    private static ImageBytes FileToImageTransform(FileInfo file, Size? targetSize = null)
    {
        using (var bitmap = (Bitmap)Image.FromFile(file.FullName))
        {
            Bitmap resized;

            if (targetSize.HasValue &&
                targetSize != bitmap.Size)
            {
                resized = new Bitmap(bitmap, targetSize.Value);
            }
            else
            {
                resized = bitmap;
            }

            using (resized)
            {
                var data = resized.LockBits(new Rectangle(Point.Empty, resized.Size), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                var bytes = new byte[data.Stride * resized.Height];

                //Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);
                for (int i = 0; i < bytes.Length; i += 3)
                {
                    bytes[i + 0] = Marshal.ReadByte(data.Scan0 + i + 2);
                    bytes[i + 1] = Marshal.ReadByte(data.Scan0 + i + 1);
                    bytes[i + 2] = Marshal.ReadByte(data.Scan0 + i + 0);
                }

                return (file, resized.Size, bytes);
            }
        }
    }
}

public class ImageBytes
{
    public ImageBytes(FileInfo file, Size size, byte[] content)
    {
        File = file;
        Size = size;
        Content = content;
    }

    public FileInfo File { get; set; }
    public Size Size { get; set; }

    public byte[] Content { get; set; }

    public static implicit operator ImageBytes((FileInfo,Size,byte[]) item) => new ImageBytes(item.Item1, item.Item2, item.Item3);

}