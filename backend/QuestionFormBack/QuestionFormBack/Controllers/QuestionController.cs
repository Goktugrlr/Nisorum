using Microsoft.AspNetCore.Mvc;
using Npgsql;
using QuestionFormBack.Models;

namespace QuestionFormBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public QuestionController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        // GET: api/question
        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
                SELECT Id, Title, Question_Content, User_Id, Created_At, Vote
                FROM Questions
            ";

            var questions = new List<Question>();
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
                            var question = new Question
                            {
                                Id = myReader.GetInt32(0),
                                Title = myReader.GetString(1),
                                Question_Content = myReader.GetString(2),
                                UserId = myReader.GetInt32(3),
                                CreatedAt = myReader.GetDateTime(4),
                                Vote = myReader.GetInt32(5) 
                            };
                            questions.Add(question);
                        }
                    }
                }
            }
            return new JsonResult(questions);
        }

        // POST: api/question
        [HttpPost]
        public JsonResult Post(Question question)
        {
            string query = @"
                INSERT INTO Questions (Title, Question_Content, User_Id, Created_At, Vote) 
                VALUES (@Title, @Question_Content, @UserId, @CreatedAt, @Vote)
            ";

            string sqlDataSource = _configuration.GetConnectionString("DBConnection");
            using (var myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (var myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Title", question.Title);
                    myCommand.Parameters.AddWithValue("@Question_Content", question.Question_Content);
                    myCommand.Parameters.AddWithValue("@UserId", question.UserId);
                    myCommand.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                    myCommand.Parameters.AddWithValue("@Vote", question.Vote);
                    myCommand.ExecuteNonQuery();
                }
            }

            return new JsonResult("Added Successfully");
        }

        // PUT: api/question
        [HttpPut]
        public JsonResult Put(Question question)
        {
            string query = @"
                UPDATE Questions
                SET Title = @Title,
                    Question_Content = @Question_Content,
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
                    myCommand.Parameters.AddWithValue("@Id", question.Id);
                    myCommand.Parameters.AddWithValue("@Title", question.Title);
                    myCommand.Parameters.AddWithValue("@Question_Content", question.Question_Content);
                    myCommand.Parameters.AddWithValue("@UserId", question.UserId);
                    myCommand.Parameters.AddWithValue("@Vote", question.Vote);
                    myCommand.ExecuteNonQuery();
                }
            }

            return new JsonResult("Updated Successfully");
        }


        // DELETE: api/question/{id}
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"
                DELETE FROM Questions
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