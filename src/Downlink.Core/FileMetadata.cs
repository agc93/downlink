namespace Downlink.Core
{
    public class FileMetadata
    {
        public FileMetadata(double sizeinBytes, string fileName) {
            SizeInBytes = sizeinBytes >= 0 ? sizeinBytes : throw new System.ArgumentOutOfRangeException(nameof(sizeinBytes));
            FileName = fileName ?? throw new System.ArgumentNullException(nameof(fileName));
        }
        public double SizeInBytes { get; }
        public string FileName { get; }
        public bool Public { get; set;} = true;
    }
}
