namespace Game399.Shared.Diagnostics
{
    public interface IGameLog
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message);
    }
}