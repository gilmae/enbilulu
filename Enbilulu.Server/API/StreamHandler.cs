using System;
using libEnbilulu;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IO;

namespace EnbiluluServer
{
    [Route("/streams")]
    [ApiController]
    public class StreamsHandler : ControllerBase
    {
        private Enbilulu _engine;
        public StreamsHandler(Enbilulu engine)
        {
            _engine = engine;
        }

        [HttpGet]
        public async Task<ActionResult> GetStreams() {

            var streams = await _engine.ListStreams();

            return Ok(streams);
        }

        [HttpGet]
        [Route("{name}")]
        public async Task<ActionResult> GetStream(string name)
        {
            var stream = await _engine.GetStream(name);
            if (stream == null)
            {
                return NotFound();
            }
            return Ok(stream);
        }

        [HttpGet]
        [Route("{name}/{point}/{limit}")]
        public async Task<ActionResult> GetRecords(string name, int point, int limit)
        {
            var data = await _engine.GetRecords(name, point, limit);
            return Ok(data);
        }

        [HttpPost]
        [Route("{name}")]
        public async Task<ActionResult> CreateStream([FromRoute] string name)
        {
            var stream = await _engine.CreateStream(name);
            return Ok(stream);
        }

        [HttpPost]
        [Route("{name}/point")]
        public async Task<ActionResult> PutRecord(string name)
        {
            string data = "";

            using (StreamReader reader = new StreamReader(this.Request.Body))
            {
                data = await reader.ReadToEndAsync();
            }

            try
            {
                var point = await _engine.PutRecord(name, data);
            }
            catch (ArgumentException)
            {
                return NotFound("Stream Not Found");
            }

            var stream = await _engine.GetStream(name);

            return Ok(stream);
        }


    }
}
