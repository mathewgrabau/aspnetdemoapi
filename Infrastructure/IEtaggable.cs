namespace DemoApi.Infrastructure
{
    public interface IEtaggable
    {
        string GetEtag();
    }
}