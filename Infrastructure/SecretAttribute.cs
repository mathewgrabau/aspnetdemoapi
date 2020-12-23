using System;

namespace DemoApi.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class SecretAttribute : Attribute
    {

    }
}
