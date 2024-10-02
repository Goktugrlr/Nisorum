namespace QuestionFormBack.Models
{
   public class Vote
    {
        public int Id { get; set; }       
        public int UserId { get; set; }       
        public int PostId { get; set; }       
        public string VoteType { get; set; }  
        public int VoteValue { get; set; }    
        public DateTime CreatedAt { get; set; } = DateTime.Now; 
    }
}