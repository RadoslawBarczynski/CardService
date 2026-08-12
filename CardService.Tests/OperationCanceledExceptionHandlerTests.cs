using CardService.Api.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardService.Tests
{
    public class OperationCanceledExceptionHandlerTests
    {
        #region Facts

        [Fact]
        public async Task TryHandleAsync_OperationCanceled_WithoutClientAbort_Writes408()
        {
            var logger = LoggerFactory.Create(b => b.AddDebug())
                .CreateLogger<OperationCanceledExceptionHandler>();
            var handler = new OperationCanceledExceptionHandler(logger);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            var handled = await handler.TryHandleAsync(
                context,
                new OperationCanceledException(),
                CancellationToken.None);

            Assert.True(handled);
            Assert.Equal(StatusCodes.Status408RequestTimeout, context.Response.StatusCode);

            context.Response.Body.Position = 0;
            var body = await new StreamReader(context.Response.Body).ReadToEndAsync();
            Assert.Contains("Request timeout", body);
        }

        #endregion Facts
    }
}
