namespace Downlink.Core
{
    public class FileMetadata
    {
        public FileMetadata(double sizeinBytes, string fileName) {

        }
        public double SizeInBytes { get; }
        public string FileName { get; }
        public bool Public { get; set;} = true;
    }
}
