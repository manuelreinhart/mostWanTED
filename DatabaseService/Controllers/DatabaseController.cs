using System;
using System.Collections.Generic;
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
        public async Task<ActionResult<IEnumerable<Question>>> GetAsync()
        {            
            var questions = await CosmosDbClient.Singleton.Get<Question>();
            return questions;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetAsync(string id)
        {
            dynamic item = new
            {
                id = id,
                Name = "test",
                age = 4
            };            
            await CosmosDbClient.Singleton.Insert(item);

            return "value";
        }

        // POST api/values
        [HttpPost("insert")]
        public void Post([FromBody] string value)
        {
            List<object> questions = JsonConvert.DeserializeObject<List<object>>(value);
            

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
