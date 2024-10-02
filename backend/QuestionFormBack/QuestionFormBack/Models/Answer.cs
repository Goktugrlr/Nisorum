namespace QuestionFormBack.Models
{
    public class Answer
    {
        public int Id { get; set; }      
        public string Answer_Content { get; set; } 
        public int QuestionId { get; set; } 
        public int UserId { get; set; }       
        public DateTime CreatedAt { get; set; }
        public int Vote { get; set; }
    }
}
