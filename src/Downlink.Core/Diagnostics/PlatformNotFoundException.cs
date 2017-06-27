namespace Downlink.Core.Diagnostics
{
    [System.Serializable]
    public class PlatformNotFoundException : NotFoundException
    {
        public PlatformNotFoundException() { }
        public PlatformNotFoundException( string message ) : base( message ) { }
        public PlatformNotFoundException( string message, System.Exception inner ) : base( message, inner ) { }
        protected PlatformNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
    }
}