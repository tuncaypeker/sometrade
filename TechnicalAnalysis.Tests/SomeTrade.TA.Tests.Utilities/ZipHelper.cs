using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace SomeTrade.TA.Tests.Utilities
{
    public static class ZipHelper
    {
        public static List<string> ReadLines(string path, string fileName)
        {
            using (ZipArchive zip = ZipFile.Open(path, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    if (entry.Name == fileName)
                    {
                        var lines = new List<string>();

                        using (var stream = entry.Open())
                        using (var reader = new StreamReader(stream))
                        {
                            while (!reader.EndOfStream)
                            {
                                lines.Add(reader.ReadLine());
                            }

                            return lines;
                        }
                    }
                }
            }

            return null;
        }
    }
}
