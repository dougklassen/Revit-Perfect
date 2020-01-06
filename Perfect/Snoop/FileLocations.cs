using System;

namespace DougKlassen.Revit.Snoop
{
    public sealed class FileLocations
    {
        private FileLocations()
        {
        }

        private static readonly Lazy<FileLocations> lazy
            = new Lazy<FileLocations>(() => new FileLocations());

        public static FileLocations Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        public String ConfigFileName
        {
            get
            {
                return "SnoopConfig.json";
            }
        }
        public String HomeDirectoryPath;
        public String ConfigFilePath
        {
            get
            {
                return HomeDirectoryPath + ConfigFileName;
            }
        }
    }
}
