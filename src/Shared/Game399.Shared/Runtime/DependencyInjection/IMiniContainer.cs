namespace Game399.Shared.DependencyInjection
{
    public interface IMiniContainer
    {
        T Resolve<T>();
    }
}