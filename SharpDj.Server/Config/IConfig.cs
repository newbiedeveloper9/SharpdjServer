namespace SharpDj.Server.Application.Management.Config
{
    public interface IConfig
    {
        short Port { get; set; }
        string Ip { get; set; }
        short? RSAKeySize { get; set; }
        bool Logging { get; set; }
    }
}