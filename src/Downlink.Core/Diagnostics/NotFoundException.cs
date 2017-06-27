namespace Downlink.Core.Diagnostics
{
    [System.Serializable]
    public abstract class NotFoundException : System.Exception
    {
        protected NotFoundException() { }
        protected NotFoundException( string message ) : base( message ) { }
        protected NotFoundException( string message, System.Exception inner ) : base( message, inner ) { }
        protected NotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
    }
}