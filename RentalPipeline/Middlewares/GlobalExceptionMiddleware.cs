using RentalPipeline.Exceptions;
using System.Net;
using System.Text.Json;

namespace RentalPipeline.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var (statusCode, message) = exception switch
            {
                NaoEncontradoException ex => (HttpStatusCode.NotFound, ex.Message),
                RegraDeNegocioException ex => (HttpStatusCode.UnprocessableEntity, ex.Message),
                ConflitoConcorrenciaException ex => (HttpStatusCode.Conflict, ex.Message),
                ArgumentException ex => (HttpStatusCode.BadRequest, ex.Message),
                _ => (HttpStatusCode.InternalServerError, "Ocorreu um erro interno no servidor.")
            };

            context.Response.StatusCode = (int)statusCode;

            return context.Response.WriteAsJsonAsync(new
            {
                status = (int)statusCode,
                error = message
            });
        }
    }
}
