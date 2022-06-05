namespace ExTools.Connection.Models
{
    public sealed class ConnectionOptions
    {
        public ConnectionType ConnectionType { get; set; }
        public string Database { get; set; }
        public string DataSource { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public string SecurePassword { get; set; }
        public string User { get; set; }
    }
}