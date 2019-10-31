using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using OpenTracing;

namespace DictionaryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictionaryController : ControllerBase
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly ITracer tracer;

        public DictionaryController(IHostingEnvironment hostingEnvironment, ITracer tracer)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.tracer = tracer;
        }

        [HttpGet]
        public ActionResult<string> Get(string word)
        {
            string dictionaryPath = Path.Combine(hostingEnvironment.ContentRootPath, "dictionary.json");

            var builder = tracer.BuildSpan($"Translate");

            using (var scope = builder.StartActive(true))
            {
                var span = scope.Span;

                try
                {
                    string definition = Dictionary.Search(dictionaryPath, word, out var stat);

                    span.SetTag("word", word);
                    span.SetTag("definition", definition);
                    span.SetTag("recordsRead", stat.RecordsRead);
                    span.SetTag("machine", Environment.MachineName);

                    if (string.IsNullOrEmpty(definition))
                    {
                        span.Log($"Word was not found: {word}");
                        return NotFound();
                    }

                    return definition;
                }
                catch (Exception e)
                {
                    span.SetTag("error", e.Message);
                    throw;
                }
            }
        }
    }
}
