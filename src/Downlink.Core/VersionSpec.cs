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
    }
}
