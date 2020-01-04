using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DatabaseService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DatabaseController : ControllerBase
    {       
   
        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAsync()
        {            
            var questions = await CosmosDbClient.Singleton.Get<object>();
            return questions;
        }                

        // POST api/values
        [HttpPost("insert")]
        public async Task PostAsync()
        {
            using (var reader = new StreamReader(Request.Body))
            {
                var body = reader.ReadToEnd();
                var question = JsonConvert.DeserializeObject<Question>(body);
                if (question.id == null)
                    question.id = new Random().Next(1000, 9999).ToString();
                await CosmosDbClient.Singleton.Insert(question);                
            }            
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
