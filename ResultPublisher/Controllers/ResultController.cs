using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Models;
using Common.Tools;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ResultPublisher.Controllers
{
    [Route("/")]
    [ApiController]
    public class ResultController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Result>>> GetAsync()
        {
            var serviceBus = new ServiceBusClient();
            await serviceBus.SendMessagesAsync("Results read");
        
            List<Result> results = new List<Result>();

            var questions = await this.GetQuestionsAsync();

            foreach (var question in questions)
            {
                if (question == null || question.question == null || question.votes == null)
                    continue;

                var votes = new List<Vote>();
                foreach (var vote in question.votes)
                {
                    var answer = votes.FirstOrDefault(v => v.answer == vote);

                    if (answer == null)
                        votes.Add(new Vote() { answer = vote, count = 1 });

                    else
                        answer.count++;
                }

                var result = new Result()
                {
                    question = question.question,
                    results = votes
                };
                results.Add(result);

            }

            return results;
        }

        private async Task<List<Question>> GetQuestionsAsync()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = await ServiceDiscovery.Singleton.GetServiceUrlByTag("DatabaseService");

            var response = await httpClient.GetAsync("Database");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return null;

            var questions = JsonConvert.DeserializeObject<List<Question>>(content);
            return questions;
        }
    }
}
