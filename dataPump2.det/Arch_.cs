using System.IO;
using Ionic.Zip;

namespace dataPump2.det.Arch_
{
    /* *
     * Класс для работы с архивами
     * Используем стандартную работу
     * взято с MSDN))
     * */
    public class Arch_
    {
        public static bool Compress(FileInfo fileToCompress)
        {
            bool res_ = true;
            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.AddFile(fileToCompress.FullName);
                    zip.Save("Archive.zip");
                }
            }
            catch
            {
                res_ = false;
            }
            return res_;
        }

        public static bool Decompress(FileInfo fileToDecompress,string folder)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            bool res_ = true;
            try
            {
                string zipToUnpack = fileToDecompress.FullName;
                string unpackDirectory = folder;
                using (ZipFile zip1 = ZipFile.Read(zipToUnpack))
                {
                    // here, we extract every entry, but we could extract conditionally
                    // based on entry name, size, date, checkbox status, etc.  
                    foreach (ZipEntry e in zip1)
                    {
                        e.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                    }
                }
            }
            catch
            {
                res_ = false;
            }
            return res_;
        }
    }
}
