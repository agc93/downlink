namespace Downlink.Core
{
    public class DownlinkMatchConventions
    {
        /// <summary>
        /// Gets or sets whether to require the version in matched filenames
        /// </summary>
        /// <returns><c>true</c> if any matched file must contain the version in the file name.</returns>
        public bool ForceNameMatching {get;set;}
        public static DownlinkMatchConventions Default =>
            new DownlinkMatchConventions {
                ForceNameMatching = false
            };
    }
}