namespace Downlink.Core
{
    public class VersionSpec
    {

        public VersionSpec(string version, string platform, string architecture) {
            if (string.IsNullOrWhiteSpace(version)) throw new System.ArgumentNullException(nameof(version));
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

        public string Summary => $"'{VersionString}' [{(string.IsNullOrWhiteSpace(Platform) ? "unknown" : Platform)}/{(string.IsNullOrWhiteSpace(Architecture) ? "unknown" : Architecture)}]";

        public bool IsStable {get;}

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
            
            // TODO: write your implementation of Equals() here
            var spec = (VersionSpec)obj;
            return obj.ToString() == ToString() &&
                spec.Platform == Platform &&
                spec.Architecture == Architecture;
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
