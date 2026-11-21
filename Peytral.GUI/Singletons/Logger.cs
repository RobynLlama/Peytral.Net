using System;
using Godot;
using Peytral.Observations;

public static class Logger
{
  public static event Action<LogEventArgs> OnReceivedLog;
  private static void PushLog(LogEventArgs args)
  {
    var output = $"[{args.Sender}] {args.Message}";
    switch (args.Level)
    {
      case SeverityLevel.Alert:
      case SeverityLevel.Critical:
        GD.PushError(output);
        break;
      case SeverityLevel.Warning:
        GD.PushWarning(output);
        break;
      default:
        GD.Print(output);
        break;
    }

    OnReceivedLog?.Invoke(args);
  }

  public class LogSink(string sender)
  {
    public readonly string Sender = sender;
    public void LogMessage(string message)
      => Log(SeverityLevel.Message, message);
    public void LogWarning(string message)
      => Log(SeverityLevel.Warning, message);
    public void LogError(string message)
      => Log(SeverityLevel.Alert, message);
    public void Log(SeverityLevel level, string message)
      => PushLog(new(level, Sender, message));
  }
}

public readonly record struct LogEventArgs(SeverityLevel Level, string Sender, string Message) { }
