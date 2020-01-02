using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using Common.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ResultProcessor.Controllers
{
    [Route("/")]
    [ApiController]
    public class ResultProcessorController : ControllerBase
    {        
        // PUT api/values/5
        [HttpPut("/submit/{id}/{answer}")]
        public async Task PutAsync(string id, int answer)
        {

            var question = await this.GetQuestionAsync(id);
            if (question == null || question.votes == null)
            {
                Response.StatusCode = 400;
                await Response.WriteAsync($"Question with id {id} not found!");
                return;
            }

            question.votes.Add(answer);

            var httpClient = new HttpClient();
            httpClient.BaseAddress = await ServiceDiscovery.Singleton.GetServiceUrlByTag("DatabaseService");

            try
            {
                var jsonString = JsonConvert.SerializeObject(question);
                var body = new StringContent(jsonString, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("Database/insert", body);
                var content = await response.Content.ReadAsStringAsync();

                Response.StatusCode = (int)response.StatusCode;
                
                await Response.WriteAsync(content);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 404;
                await Response.WriteAsync("Page not found!");
            }

        }

        private async Task<Question> GetQuestionAsync(string id)
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = await ServiceDiscovery.Singleton.GetServiceUrlByTag("DatabaseService");

            var response = await httpClient.GetAsync("Database");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return null;
            
            var questions = JsonConvert.DeserializeObject<List<Question>>(content);

            foreach (var question in questions)
            {                
                if (question != null && question.id == id)
                    return question;
            }
            return null;
            
        }

    }
}
