namespace Downlink.Core
{
    public class VersionSpec
    {

        public VersionSpec(string version, string platform, string architecture) {
            VersionString = version;
            Platform = platform ?? "any";
            Architecture = architecture ?? "any";
        }

        private VersionSpec(string version) {
            VersionString = version;
        }

        public static implicit operator string(VersionSpec v)
        {
            return v.ToString();
        }

        /*public static implicit operator VersionSpec(string s) {
            return new VersionSpec(s);
        } */

        public string Platform { get; } = string.Empty;

        public string Architecture { get; } = string.Empty;
        private string VersionString { get; }

        public string Format { get; set; } = string.Empty;

        public string Summary => $"'{VersionString}' [{(Platform ?? "unknown")}/{(Architecture ?? "unknown")}]";

        public override string ToString()
        {
            return VersionString;
        }

        // override object.Equals
        public override bool Equals (object obj)
        {
            //
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //
            
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            var spec = (VersionSpec)obj;
            return obj.ToString() == ToString() &&
                spec.Platform == Platform &&
                spec.Architecture == Architecture &&
                spec.Format == Format;
        }

        public override int GetHashCode() {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (Architecture ?? "").GetHashCode();
                hash = hash * 23 + (Platform ?? "").GetHashCode();
                hash = hash * 23 + (VersionString ?? "").GetHashCode();
                return hash;
            }
        } 
    }
}