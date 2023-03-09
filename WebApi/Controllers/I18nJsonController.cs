using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class I18nJsonController : ControllerBase
    {
        private readonly ILogger<I18nJsonController> _logger;
        private readonly IStringLocalizer<I18nJsonController> _stringLocalizer;

        public I18nJsonController(ILogger<I18nJsonController> logger, IStringLocalizer<I18nJsonController> stringLocalizer)
        {
            _logger = logger;
            _stringLocalizer = stringLocalizer;
        }

        [HttpGet]
        public IActionResult Get([FromQuery]string key)
        {
            var result = _stringLocalizer[key].ToString();
            return Ok(new { message = result });
        }

        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var result = _stringLocalizer.GetAllStrings(false);
            return Ok(new { i18n = result });
        }
    }
}
