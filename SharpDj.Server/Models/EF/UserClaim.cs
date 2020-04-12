namespace SharpDj.Server.Models.EF
{
    public class UserClaim
    {
        public int Id { get; set; }

        public User User { get; set; }
        public Claim Type { get; set; }
    }
}
