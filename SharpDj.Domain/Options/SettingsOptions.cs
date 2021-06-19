namespace SharpDj.Domain.Options
{
    public class SettingsOptions
    {
        public const string Settings = "Settings";

        public string Ip { get; set; }
        public int Port { get; set; }
        public int? RSAKeySize { get; set; }
        public bool Logging { get; set; }
    }
}
