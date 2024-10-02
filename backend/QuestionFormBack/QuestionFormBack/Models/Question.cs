namespace QuestionFormBack.Models
{
    public class Question
    {
        public int Id { get; set; }            
        public string Title { get; set; }    
        public string Question_Content{ get; set; } 
        public int UserId { get; set; }        
        public DateTime CreatedAt { get; set; }
        public int Vote {  get; set; }
    }
}