namespace DemoApi.Infrastructure
{
    public interface IEtagHandlerFeature
    {
        bool NoneMatch(IEtaggable entity);
    }
}