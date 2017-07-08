namespace Downlink.Hosting
{
    [System.Flags]
    public enum DownlinkBuilderOptions
    {
        None = 0,
        SkipDefaultPatterns = 1,
        SkipDefaultHandlers = 2,
        SkipDefaultSchemeClients = 4,
        SkipDefaultStorage = 8
    }

    internal class DownlinkBuilderDefaults {
        internal DownlinkBuilderDefaults(DownlinkBuilderOptions opts) {
            RegisterDefaultPatterns = !opts.HasFlag(DownlinkBuilderOptions.SkipDefaultPatterns);
            RegisterDefaultHandlers = !opts.HasFlag(DownlinkBuilderOptions.SkipDefaultHandlers);
            RegisterDefaultSchemeClients = !opts.HasFlag(DownlinkBuilderOptions.SkipDefaultSchemeClients);
            RegisterDefaultStorage = !opts.HasFlag(DownlinkBuilderOptions.SkipDefaultStorage);
        }

        internal bool RegisterDefaultPatterns {get;}
        internal bool RegisterDefaultHandlers {get;}
        internal bool RegisterDefaultSchemeClients {get;}
        internal bool RegisterDefaultStorage {get;}
    }
}