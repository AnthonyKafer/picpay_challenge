using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using picpay_challenge.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace picpay_challenge.Middleware
{
    public class GlobalExeceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExeceptionHandlingMiddleware> _logger;
        public GlobalExeceptionHandlingMiddleware(ILogger<GlobalExeceptionHandlingMiddleware> logger) => _logger = logger;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (HttpException ex)
            {
                await HandleHttpException(context, ex._status, ex.Message);
            }
            catch (Exception ex)
            {
                await HandleHttpException(context, HttpStatusCode.InternalServerError, ex.Message);
            }


        }
        public static async Task HandleHttpException(HttpContext context, HttpStatusCode status, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            var errorResponse = new
            {
                status = status,
                error = message
            };

            await context.Response.WriteAsJsonAsync(errorResponse);

        }
    }
}
