namespace Downlink.Core.Diagnostics
{
    [System.Serializable]
    public class VersionParseException : System.Exception
    {
        public VersionParseException() { }
        public VersionParseException( string message ) : base( message ) { }
        public VersionParseException( string message, System.Exception inner ) : base( message, inner ) { }
        protected VersionParseException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
    }
}