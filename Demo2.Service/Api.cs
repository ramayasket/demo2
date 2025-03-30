using Demo2.Core;
using Microsoft.AspNetCore.Mvc;

namespace Demo2.Service
{
    [ApiController]
    [Route("demo2")]
    public class Api(Demonstrator demonstrator) : ControllerBase
    {
        //
        // Получить последнее вычисленное значение
        //
        [HttpGet("read")]
        public async Task<Entry> Read() => await demonstrator.Read();

        //
        // Выполнить вычисление
        //
        [HttpPost("invoke")]
        public async Task<Entry> Invoke() => await demonstrator.Invoke();
    }
}
