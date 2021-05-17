namespace FW.Entities
{
    public class UserRating
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public int MovieId { get; set; }

        public double Rating { get; set; }
    }
}
