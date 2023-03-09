using I18nJsonLibrary;
using System.Globalization;

namespace WebApi.Middlewares
{
    public class I18nMiddleware : IMiddleware
    {
        private readonly I18nJsonService _service;

        public I18nMiddleware(I18nJsonService service)
        {
            _service = service;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string? culture = context.Request.Headers.AcceptLanguage.FirstOrDefault();
            _service.ResolveCurrentCulture(culture);

            await next(context);
        }
    }
}
