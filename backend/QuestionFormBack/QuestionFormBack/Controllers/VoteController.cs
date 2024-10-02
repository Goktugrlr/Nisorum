using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using QuestionFormBack.Models;

namespace QuestionFormBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VotesController : ControllerBase 
    {
        private readonly IConfiguration _configuration;

        public VotesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/votes
        [HttpGet]
        public JsonResult Get()
        {
            string query = @"
                SELECT Id, User_Id, Post_Id, Vote_Type, Vote_Value, Created_At
                FROM Votes
            ";

            var votes = new List<Vote>();
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
                            var vote = new Vote
                            {
                                Id = myReader.GetInt32(0),
                                UserId = myReader.GetInt32(1),
                                PostId = myReader.GetInt32(2),
                                VoteType = myReader.GetString(3),
                                VoteValue = myReader.GetInt32(4),
                                CreatedAt = myReader.GetDateTime(5)
                            };
                            votes.Add(vote);
                        }
                    }
                }
            }
            return new JsonResult(votes);
        }

        // POST: api/votes
        [HttpPost]
        public JsonResult Post(Vote vote)
        {

            if (vote.VoteValue < -1 || vote.VoteValue > 1)
            {
                return new JsonResult("Invalid Vote Value") { StatusCode = 400 };
            }
            string query = @"
                INSERT INTO Votes (User_Id, Post_Id, Vote_Type, Vote_Value, Created_At) 
                VALUES (@UserId, @PostId, @VoteType, @VoteValue, @CreatedAt)
                ON CONFLICT (User_Id, Post_Id, Vote_Type)
                DO UPDATE SET Vote_Value = EXCLUDED.Vote_Value
            ";

            string sqlDataSource = _configuration.GetConnectionString("DBConnection");
            using (var myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (var myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserId", vote.UserId);
                    myCommand.Parameters.AddWithValue("@PostId", vote.PostId);
                    myCommand.Parameters.AddWithValue("@VoteType", vote.VoteType);
                    myCommand.Parameters.AddWithValue("@VoteValue", vote.VoteValue);
                    myCommand.Parameters.AddWithValue("@CreatedAt", vote.CreatedAt);
                    myCommand.ExecuteNonQuery();
                }
            }

            return new JsonResult("Vote Recorded Successfully");
        }

        // PUT: api/votes/{id}
        [HttpPut("{id}")]
        public JsonResult Put(int id, Vote vote)
        {
            string query = @"
                UPDATE Votes
                SET Vote_Value = @VoteValue
                WHERE Id = @Id
            ";

            string sqlDataSource = _configuration.GetConnectionString("DBConnection");
            using (var myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (var myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Id", id);
                    myCommand.Parameters.AddWithValue("@VoteValue", vote.VoteValue);
                    myCommand.ExecuteNonQuery();
                }
            }

            return new JsonResult("Vote Updated Successfully");
        }

        // DELETE: api/votes/{userId}/{postId}/{voteType}
        [HttpDelete("{userId}/{postId}/{voteType}")]
        public JsonResult Delete(int userId, int postId, string voteType)
        {
            string query = @"
                DELETE FROM Votes
                WHERE User_Id = @UserId AND Post_Id = @PostId AND Vote_Type = @VoteType
            ";

            string sqlDataSource = _configuration.GetConnectionString("DBConnection");
            using (var myCon = new NpgsqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (var myCommand = new NpgsqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserId", userId);
                    myCommand.Parameters.AddWithValue("@PostId", postId);
                    myCommand.Parameters.AddWithValue("@VoteType", voteType);
                    myCommand.ExecuteNonQuery();
                }
            }

            return new JsonResult("Vote Removed Successfully");
        }
    }
}
