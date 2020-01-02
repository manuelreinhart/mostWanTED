using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Common.Tools;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace QuestionPublisher.Controllers
{
    [Route("/")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public async Task GetAsync()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = await ServiceDiscovery.Singleton.GetServiceUrlByTag("DatabaseService");
                        
            try
            {
                var response = await httpClient.GetAsync("Database");
                var content = await response.Content.ReadAsStringAsync();

                Response.StatusCode = (int)response.StatusCode;
                Response.ContentType = response.Content.Headers.ContentType.ToString();
                Response.ContentLength = response.Content.Headers.ContentLength;

                await Response.WriteAsync(content);
            }
            catch (Exception ex)
            {
                Response.StatusCode = 404;
                await Response.WriteAsync("Page not found!");
            }
        }

    }
}
