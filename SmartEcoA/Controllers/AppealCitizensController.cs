using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartEcoA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEcoA.Controllers
{
    [Route("{language}/api/[controller]")]
    [ApiController]
    public class AppealCitizensController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppealCitizensController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class QuestionAndAnswers
        {
            public Question Question { get; set; }
            public IEnumerable<Answer> Answers { get; set; }
        }

        // GET: api/AppealCitizens
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IList<QuestionAndAnswers>>> GetQuestionsAndAnswers()
        {
            var questions = _context.Question.Include(q => q.ApplicationUser).OrderByDescending(q => q.DateTime).ToList();
            var answers = _context.Answer.Include(a => a.Question).ToList();
            var questionsAndAnswers = new List<QuestionAndAnswers>();
            try
            {
                questionsAndAnswers = questions
                    .AsEnumerable()
                    .GroupJoin(
                    answers,
                    question => question,
                    answer => answer.Question,
                    (q, ansCollection) => new QuestionAndAnswers
                    {
                        Question = q,
                        Answers = ansCollection.OrderBy(a => a.DateTime).ToList()
                    }).ToList();
            }
            catch (Exception ex)
            { 

            }
            return questionsAndAnswers;
        }

        // GET: api/AppealCitizens
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<Question>> GetQuestion(int id)
        {
            var question = await _context.Question.FindAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            return question;
        }

        // POST: api/AppealCitizens
        [HttpPost]
        [Route("PostQuestion")]
        [Authorize]
        public async Task<ActionResult<Question>> PostQuestion(Question question)
        {
            question.ApplicationUserId = User.Claims.First(c => c.Type == "Id").Value;
            question.DateTime = DateTime.Now;
            _context.Question.Add(question);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQuestion", new { id = question.Id }, question);
        }

        // POST: api/AppealCitizens
        [HttpPost]
        [Route("PostAnswer")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<Answer>> PostAnswer(Answer answer)
        {
            try
            {
                answer.ApplicationUserId = User.Claims.First(c => c.Type == "Id").Value;
                answer.DateTime = DateTime.Now;
                answer.QuestionId = answer.Question.Id;
                answer.Question = null;
                _context.Answer.Add(answer);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {

            }

            return CreatedAtAction("GetAnswer", new { id = answer.Id }, answer);
        }

        // DELETE: api/AppealCitizens/5
        [HttpDelete]
        [Route("DeleteQuestion/{id}")]
        [Authorize]
        public async Task<ActionResult<Question>> DeleteQuestion(int id)
        {
            var question = await _context.Question.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            _context.Question.Remove(question);
            await _context.SaveChangesAsync();

            return question;
        }

        // DELETE: api/AppealCitizens/5
        [HttpDelete]
        [Route("DeleteAnswer/{id}")]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<ActionResult<Answer>> DeleteAnswer(int id)
        {
            var answer = await _context.Answer.FindAsync(id);
            if (answer == null)
            {
                return NotFound();
            }

            _context.Answer.Remove(answer);
            await _context.SaveChangesAsync();

            return answer;
        }
    }
}
