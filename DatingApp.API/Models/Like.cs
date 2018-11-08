namespace DatingApp.API.Models
{
    public class Like
    {
        public int LikerId { get; set; } // id of user liking another user
        public int LikeeId { get; set; } // id of user being liked by another user
        public User Liker { get; set; }
        public User Likee { get; set; }
    }
}