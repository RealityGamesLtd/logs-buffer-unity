using UnityEngine;

namespace Config
{
    public interface ILoggerConfig
    {
        int BufferSize { get; }
        LogType BufferLogLevel { get; }
        bool ShouldLogToConsole { get; }
        LogType ConsoleLogLevel { get; }
    }
}
