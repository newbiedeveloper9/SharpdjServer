namespace SharpDj.Domain.Entity
{
    public class UserClaim
    {
        public int Id { get; set; }

        public User User { get; set; }
        public Claim Type { get; set; }
    }
}
