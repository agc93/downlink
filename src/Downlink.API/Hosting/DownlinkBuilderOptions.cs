namespace Downlink.Hosting
{
    [System.Flags]
    public enum DownlinkBuilderOptions
    {
        None = 0,
        SkipDefaultPatterns = 1,
        SkipDefaultHandlers = 2,
        SkipDefaultSchemeClients = 4
    }
}