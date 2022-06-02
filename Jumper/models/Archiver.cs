using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SevenZip.Sdk;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Checksum;

namespace Jumper.models
{
    

    public class Archiver
    {
        public enum Type
        {
            SEVENZ,
            TAR,
            ZIP
        }
       
        public static void ArchiveFolder(string dir, Type type)
        {
            switch (type)
            {
                case Type.SEVENZ:
                    string dllpath = Path.GetFullPath(".\\");
                    if (File.Exists(dllpath + "7z.dll"))
                    {
                        SevenZip.SevenZipBase.SetLibraryPath(dllpath + "7z.dll");
                    }
                    else if (File.Exists(dllpath + "7za.dll"))
                    {
                        SevenZip.SevenZipBase.SetLibraryPath(dllpath + "7za.dll");
                    }
                    else
                    {
                        throw new FileNotFoundException("7z.dll or 7za.dll were not in the plugins folder\n Aborting..");
                    }
                    SevenZip.SevenZipCompressor sz = new SevenZip.SevenZipCompressor();
                    sz.CompressionMethod = SevenZip.CompressionMethod.Lzma2;
                    sz.CompressionLevel = SevenZip.CompressionLevel.Fast;
                    sz.CompressionMode = SevenZip.CompressionMode.Create;
                    sz.CompressDirectory(dir, dir + ".cb7");
                    break;
                case Type.TAR:
                    TarArchive tar = TarArchive.CreateOutputTarArchive(new FileStream(dir + ".cbt", FileMode.Create));
                    tar.RootPath = dir.Replace('\\', '/');
                    if (tar.RootPath.EndsWith("/"))
                    {
                        tar.RootPath = tar.RootPath.Remove(tar.RootPath.Length - 1);
                    }
                    TarFromFolder(tar, dir);
                    tar.Close();
                    break;
                case Type.ZIP:
                    FastZip zip = new FastZip();
                    zip.CreateZip(dir+".cbz", @dir,false,"");
                    break;
            }
            foreach(string s in Directory.GetFiles(dir))
            {
                File.Delete(s);
            }
            
            Directory.Delete(dir);
        }

        private static void TarFromFolder(TarArchive tar, string dir)
        {
            TarEntry entry = TarEntry.CreateEntryFromFile(dir);
            tar.WriteEntry(entry, false);
            string[] f = Directory.GetFiles(dir);
            foreach (string name in f)
            {
                entry = TarEntry.CreateEntryFromFile(name);
                tar.WriteEntry(entry, true);
            }
        }
    }
}
