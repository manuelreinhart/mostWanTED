using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Common.Tools;

namespace QuestionCreator.Controllers
{
    [Route("/")]
    [ApiController]
    public class QuestionCreatorController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetAsync()
        {
            var keyVaultUri = await KeyVault.Singleton.GetSecretByKey("asg");
            return new string[] { "value1", "value2", keyVaultUri };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetAsync(string id)
        {
            var secret = await KeyVault.Singleton.GetSecretByKey(id);

            if (secret == null)
                return "secret not found";

            return secret;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

    }
}
