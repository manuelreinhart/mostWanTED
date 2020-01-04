using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Common.Tools;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace QuestionCreator.Controllers
{
    [Route("/")]
    [ApiController]
    public class QuestionCreatorController : ControllerBase
    {               
        // POST api/values
        [HttpPost]
        public async Task PostAsync()
        {
            using (var streamContent = new StreamContent(Request.Body))
            {
                var httpClient = new HttpClient();
                httpClient.BaseAddress = await ServiceDiscovery.Singleton.GetServiceUrlByTag("DatabaseService");

                var response = await httpClient.PostAsync("Database/insert", streamContent);
                var content = await response.Content.ReadAsStringAsync();

                Response.StatusCode = (int)response.StatusCode;
                Response.ContentType = response.Content.Headers.ContentType?.ToString();
                Response.ContentLength = response.Content.Headers.ContentLength;

                await Response.WriteAsync(content);

            }
            
        }

    }
}
