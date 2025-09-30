using Game399.Shared.Diagnostics;

namespace Game399.Tests;

public class EmptyGameLog : IGameLog
{
    public static IGameLog Instance { get; } = new EmptyGameLog();
    
    public void Info(string message)
    {
    }

    public void Warn(string message)
    {
    }

    public void Error(string message)
    {
    }
}