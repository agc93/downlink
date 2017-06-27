namespace Downlink.Core
{
    public class VersionSpec
    {

        public VersionSpec(string version, string platform, string architecture) {
            VersionString = version;
            Platform = platform ?? string.Empty;
            Architecture = architecture ?? string.Empty;
        }

        private VersionSpec(string version) {
            VersionString = version;
        }

        public static implicit operator string(VersionSpec v)
        {
            return v.ToString();
        }

        public static implicit operator VersionSpec(string s) {
            return new VersionSpec(s);
        }

        public string Platform { get; } = string.Empty;

        public string Architecture { get; } = string.Empty;
        private string VersionString { get; }

        public bool IsStable {get;}

        public override string ToString()
        {
            return VersionString;
        }
    }
}
