using System.Collections.Generic;
using System.Linq;
using Config;
using CustomLogger;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    [TestFixture]
    public class LogsBufferTest
    {
        [SetUp]
        public void Setup()
        {
            LogsBuffer.Instance?.ClearBufferedLogs();
        }

        [Test]
        public void LogsForLogLogLevel()
        {
            LogAssert.ignoreFailingMessages = true;
            LogsBuffer.Init(new MockLoggerConfig
            {
                BufferSize = 10,
                BufferLogLevel = LogType.Log
            });

            Debug.Log("TestLog");
            Debug.LogWarning("TestWarning");
            Debug.LogError("TestError");

            var logs = LogsBuffer.Instance.Logs;
            Assert.AreEqual(3, logs.Length);
            Assert.IsTrue(logs.Where(i => i.logType == LogType.Log).FirstOrDefault() != null);
            Assert.IsTrue(logs.Where(i => i.logType == LogType.Warning).FirstOrDefault() != null);
            Assert.IsTrue(logs.Where(i => i.logType == LogType.Error).FirstOrDefault() != null);
        }

        [Test]
        public void LogsForWarningLogLevel()
        {
            LogAssert.ignoreFailingMessages = true;
            LogsBuffer.Init(new MockLoggerConfig
            {
                BufferSize = 10,
                BufferLogLevel = LogType.Warning
            });

            Debug.Log("TestLog");
            Debug.LogWarning("TestWarning");
            Debug.LogError("TestError");

            var logs = LogsBuffer.Instance.Logs;
            Assert.IsTrue(logs.Where(i => i.logType == LogType.Warning).FirstOrDefault() != null);
            Assert.IsTrue(logs.Where(i => i.logType == LogType.Error).FirstOrDefault() != null);
            Assert.IsTrue(logs.Where(i => i.logType == LogType.Log).FirstOrDefault() == null);
            Assert.AreEqual(2, logs.Length);
        }

        [Test]
        public void LogsForErrorLogLevel()
        {
            LogAssert.ignoreFailingMessages = true;
            LogsBuffer.Init(new MockLoggerConfig
            {
                BufferSize = 10,
                BufferLogLevel = LogType.Error
            });

            Debug.Log("TestLog");
            Debug.LogWarning("TestWarning");
            Debug.LogError("TestError");

            var logs = LogsBuffer.Instance.Logs;
            Assert.AreEqual(1, logs.Length);
            Assert.IsTrue(logs.Where(i => i.logType == LogType.Log).FirstOrDefault() == null);
            Assert.IsTrue(logs.Where(i => i.logType == LogType.Warning).FirstOrDefault() == null);
            Assert.IsTrue(logs.Where(i => i.logType == LogType.Error).FirstOrDefault() != null);
        }

        [Test]
        public void BufferSizeAndItemsOrder()
        {
            LogAssert.ignoreFailingMessages = true;
            var loopIterations = 1001;
            var bufferSize = loopIterations - 1;
            var logFormat = "Test{0}[{1}]";

            LogsBuffer.Init(new MockLoggerConfig
            {
                BufferSize = bufferSize,
                BufferLogLevel = LogType.Error
            });

            for (var logIndex = 0; logIndex < loopIterations; logIndex++)
            {
                Debug.LogFormat(logFormat, LogType.Log, logIndex);
                Debug.LogErrorFormat(logFormat, LogType.Error, logIndex);
            }

            var logs = LogsBuffer.Instance.Logs;
            Assert.AreEqual(bufferSize, logs.Length);
            Assert.IsTrue(logs.Where(i => i.message == string.Format(logFormat, LogType.Error, 0)).FirstOrDefault() == null);

            for (var logIndex = 0; logIndex < bufferSize; logIndex++)
            {
                Assert.AreEqual(string.Format(logFormat, LogType.Error, logIndex + 1), logs[logIndex].message);
            }
        }

        [Test]
        public void OnLogEvent()
        {
            LogAssert.ignoreFailingMessages = true;
            // setup
            var logsList = new List<LogsBuffer.LogItem>();
            LogsBuffer.OnLog += SaveLog;

            var loopIterations = 10;
            var logTextFormat = "WarnintText{0}";
            LogsBuffer.Init(new MockLoggerConfig());

            for (var logIndex = 0; logIndex < loopIterations; logIndex++)
            {
                Debug.LogWarningFormat(logTextFormat, logIndex);
            }

            Assert.AreEqual(logsList.Count, loopIterations);
            for (var logIndex = 0; logIndex < loopIterations; logIndex++)
            {
                Assert.AreEqual(LogType.Warning, logsList[logIndex].logType);
                Assert.AreEqual(string.Format(logTextFormat, logIndex), logsList[logIndex].message);
            }

            // clean up
            LogsBuffer.OnLog -= SaveLog;

            void SaveLog(LogsBuffer.LogItem logItem) => logsList.Add(logItem);
        }

        private class MockLoggerConfig : ILoggerConfig
        {
            public int BufferSize { get; set; } = 100;

            public LogType BufferLogLevel { get; set; } = LogType.Log;

            public bool ShouldLogToConsole { get; set; } = false;

            public LogType ConsoleLogLevel { get; set; } = LogType.Log;
        }
    }
}
