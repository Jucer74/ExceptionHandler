using Application.Exceptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace WebApi.Middleware
{
    /// <summary>
    /// Handler the exeptions
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            const string BUSINESSEXCEPTION_CONTROLLED_MESSAGE = "BusinessException controlled";
            const string EXCEPTION_CONTROLLED_MESSAGE = "Exception not controlled";
            const string HASSTARTED_MESSAGE = "The response has already started, middleware will not be executed";

            if (httpContext == null)
            {
                return;
            }

            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                if (httpContext.Response.HasStarted)
                {
                    //Log.Warning(HASSTARTED_MESSAGE);
                    throw;
                }

                string logMessageException = EXCEPTION_CONTROLLED_MESSAGE;
                if (ex.GetBaseException() is BusinessException)
                {
                    ex = ex.GetBaseException();
                    logMessageException = BUSINESSEXCEPTION_CONTROLLED_MESSAGE;
                }

                await httpContext.HandleExceptionAsync(ex);
                //Log.Warning(ex, logMessageException);
                //Log.Error(ex, ex.Message);
            }
        }
    }
}