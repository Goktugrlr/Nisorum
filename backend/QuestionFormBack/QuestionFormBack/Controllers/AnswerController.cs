using Microsoft.AspNetCore.Mvc;
using Npgsql;
using QuestionFormBack.Models;


namespace QuestionFormBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public AnswerController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        // GET: api/answer
        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
                SELECT Id, Answer_Content, Question_Id, User_Id, Created_At, Vote
                FROM Answers
            ";

            var answers = new List<Answer>();
            string sqlDataSource = _configuration.GetConnectionString("DBConnection");
            using (var myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (var myCommand = new NpgsqlCommand(query, myCon))
                {
                    using (var myReader = myCommand.ExecuteReader())
                    {
                        while (myReader.Read())
                        {
                            var answer = new Answer
                            {
                                Id = myReader.GetInt32(0),
                                Answer_Content = myReader.GetString(1),
                                QuestionId = myReader.GetInt32(2),
                                UserId = myReader.GetInt32(3),
                                CreatedAt = myReader.GetDateTime(4),
                                Vote = myReader.GetInt32(5)
                            };
                            answers.Add(answer);
                        }
                    }
                }
            }
            return new JsonResult(answers);
        }

        // POST: api/answer
        [HttpPost]
        public JsonResult Post(Answer answer)
        {
            string query = @"
                INSERT INTO Answers (Answer_Content, Question_Id, User_Id, Created_At, Vote) 
                VALUES (@Answer_Content, @QuestionId, @UserId, @CreatedAt, @Vote)
            ";

            string sqlDataSource = _configuration.GetConnectionString("DBConnection");
            using (var myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (var myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Answer_Content", answer.Answer_Content);
                    myCommand.Parameters.AddWithValue("@QuestionId", answer.QuestionId);
                    myCommand.Parameters.AddWithValue("@UserId", answer.UserId);
                    myCommand.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    myCommand.Parameters.AddWithValue("@Vote", 0);  
                    myCommand.ExecuteNonQuery();
                }
            }

            return new JsonResult("Added Successfully");
        }

        // PUT: api/answer
        [HttpPut]
        public JsonResult Put(Answer answer)
        {
            string query = @"
                UPDATE Answers
                SET Answer_Content = @Answer_Content,
                    Question_Id = @QuestionId,
                    User_Id = @UserId,
                    Vote = @Vote
                WHERE Id = @Id
            ";

            string sqlDataSource = _configuration.GetConnectionString("DBConnection");
            using (var myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (var myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Id", answer.Id);
                    myCommand.Parameters.AddWithValue("@Answer_Content", answer.Answer_Content);
                    myCommand.Parameters.AddWithValue("@QuestionId", answer.QuestionId);
                    myCommand.Parameters.AddWithValue("@UserId", answer.UserId);
                    myCommand.Parameters.AddWithValue("@Vote", answer.Vote);
                    myCommand.ExecuteNonQuery();
                }
            }

            return new JsonResult("Updated Successfully");
        }


        // DELETE: api/answer/{id}
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"
                DELETE FROM Answers
                WHERE Id = @Id
            ";

            string sqlDataSource = _configuration.GetConnectionString("DBConnection");
            using (var myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (var myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Id", id);
                    myCommand.ExecuteNonQuery();
                }
            }

            return new JsonResult("Deleted Successfully");
        }
    }
}