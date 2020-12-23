﻿using Microsoft.AspNetCore.Http;

namespace DemoApi.Infrastructure
{
    public static class HttpRequestExtensions
    {
        public static IEtagHandlerFeature GetEtagHandler(this HttpRequest request)
        {
            return request.HttpContext.Features.Get<IEtagHandlerFeature>();
        }
    }
}
