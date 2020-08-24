namespace SharpDj.Server.Entity
{
    public class RoleClaim
    {
        public int Id { get; set; }
        public Claim Type { get; set; }
        public ServerRole Role { get; set; }
    }
}
