namespace Downlink.Core.Diagnostics
{
    [System.Serializable]
    public class ArchitectureNotFoundException : NotFoundException
    {
        public ArchitectureNotFoundException() { }
        public ArchitectureNotFoundException( string message ) : base( message ) { }
        public ArchitectureNotFoundException( string message, System.Exception inner ) : base( message, inner ) { }
        protected ArchitectureNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context ) : base( info, context ) { }
    }
}