namespace Downlink.Core.Diagnostics
{
    [System.Serializable]
    public class FormatNotFoundException : NotFoundException
    {
        public FormatNotFoundException() { }
        public FormatNotFoundException(string message) : base(message) { }
        public FormatNotFoundException(string message, System.Exception inner) : base(message, inner) { }
        protected FormatNotFoundException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}