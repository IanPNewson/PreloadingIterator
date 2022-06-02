
using PreloadingIterator;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

Thread.CurrentThread.Name = "Main";

var port = 8932;
var server = new TcpListener(IPAddress.Any, port);
server.Start();
Task.Run(() =>
{
    Thread.CurrentThread.Name = "TcpListener";
    while (true)
    {
        var client = server.AcceptTcpClient();
        Task.Run(() =>
        {
            Thread.CurrentThread.Name = client.Client.RemoteEndPoint!.ToString();
            PreloadingImageBytesIterator? iterator = null;
            try
            {
                var files = new List<string>();

                var file = new StringBuilder();
                var buffer = new byte[1];
                var read = -1;
                char? previous = null;
                while ((read = client.Client.Receive(buffer)) > 0)
                {
                    for (var i = 0; i < read; i++)
                    {
                        var @byte = buffer[i];
                        if (@byte == (byte)'\n')
                        {
                            if (previous == '\n')
                            {
                                buffer = buffer.Take(read).Skip(i).ToArray();
                                goto filelistfinished;
                            }
                            else
                            {
                                if (file.Length > 0)
                                    files.Add(file.ToString());
                                file.Clear();
                            }
                            previous = (char)@byte;
                        }
                        else
                        {
                            previous = (char)@byte;
                            file.Append(previous);
                        }
                    }
                }

            filelistfinished:

                Console.WriteLine($"Received request for {files.Count} files");

                var fileInfos = files.Select(x => new FileInfo(x)).ToList();
                iterator = new PreloadingImageBytesIterator(
                    fileInfos,
                    PreloadingIteratorMemoryLimits.Max
                        .WithMaxMemoryGb(8)
                        .WithFreeLimitGb(1),
                    targetSize:new System.Drawing.Size(1920,1080)
                );

                var getByte = new Func<byte>(() =>
                {
                    var buffer = new byte[1];
                    client.Client.Receive(buffer);
                    return buffer[0];
                });

                var j = 0;
                foreach (var img in iterator)
                {
                    Console.WriteLine($"Sending {img.File.FullName}");

                    //Console.WriteLine($"Waiting for file request");
                    getByte();
                    //Console.WriteLine($"Sending file of length {img.Length}");
                    client.Client.Send(img.Content.Length.ToString().Ascii());
                    client.Client.Send("\n".Ascii());
                    getByte();
                    client.Client.Send($"{img.Size.Width}x{img.Size.Height}".Ascii());
                    client.Client.Send("\n".Ascii());
                    getByte();
                    client.Client.Send(img.Content);

                    ++j;
                    if ((j % 100) == 0)
                    {
                        Console.WriteLine($"Sent {j}/{fileInfos.Count}, preloaded range {iterator.Window.start}-{iterator.Window.end} for a total of {iterator.Window.end - iterator.Window.start}");
                    }
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                iterator?.Dispose();
                GC.Collect();
            }
        });
    }
});

Console.WriteLine($"Waiting for connections on port {port}, type 'exit' and press enter to exit");

while (Console.ReadLine() != "exit") ;


public static class Ext
{
    public static byte[] Ascii(this string str) => Encoding.ASCII.GetBytes(str);
}

