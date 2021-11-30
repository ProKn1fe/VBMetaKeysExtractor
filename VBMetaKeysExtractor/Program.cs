using System;
using System.IO;
using System.Linq;
using System.Text;

namespace VBMetaKeysExtractor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("No input file");
                return;
            }
            var metaPath = args[0];
            var metaDir = Path.GetDirectoryName(metaPath);
            var stream = new BinaryReader(new MemoryStream(File.ReadAllBytes(metaPath)));
            // Search for 00 00 10 00 (int = 1048576)
            while (true)
            {
                if (stream.BaseStream.Position > stream.BaseStream.Length - 4)
                    break;
                var reqValue = stream.ReadInt32();
                if (reqValue == 1048576)
                {
                    //Search for name, take last 30 bytes and remove zero
                    stream.BaseStream.Seek(-34, SeekOrigin.Current);
                    var bytes = stream.ReadBytes(30);
                    if (bytes.Count(a => a == 0) > 10)
                    {
                        var name = Encoding.Default.GetString(bytes.Where(a => a != 0).ToArray());
                        File.WriteAllBytes(Path.Combine(metaDir, $"{name}_key.bin"), stream.ReadBytes(1032));
                    }
                    else
                    {
                        stream.BaseStream.Seek(4, SeekOrigin.Current);
                    }
                }
                else
                {
                    stream.BaseStream.Seek(-3, SeekOrigin.Current);
                }
            }
        }
    }
}
