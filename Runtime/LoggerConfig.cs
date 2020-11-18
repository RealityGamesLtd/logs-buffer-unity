using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(fileName = nameof(LoggerConfig), menuName = "Config/Logger")]
    public class LoggerConfig : ScriptableObject, ILoggerConfig
    {
        [SerializeField] int bufferSize;
        [SerializeField] LogType bufferLogLevel;
        [SerializeField] bool shouldLogToConsole;
        [SerializeField] LogType consoleLogLevel;

        public int BufferSize { get { return bufferSize; } }
        public LogType BufferLogLevel { get { return bufferLogLevel; } }
        public bool ShouldLogToConsole { get { return shouldLogToConsole; } }
        public LogType ConsoleLogLevel { get { return consoleLogLevel; } }
    }
}
