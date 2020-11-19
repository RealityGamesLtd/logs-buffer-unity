# TODO

# Description
Logs Buffer package gives you a possibility to store and handle custom created logs in various ways, by using this package user can get information about logs that were created in the application. Logs can be stored in buffer which is configured by user in the code. Logs are stored as a `LogItem` class instance which holds information about their message, creation time, and type (`Error`, `Warning`,...)

This package replaces Unity default Debug.unityLogger.logHandler reference and encapsulates standard logging methods like: 
```
Debug.Log();
Debug.LogWarning();
Debug.LogError();
```
Logs Buffer gives a possibility to add specific listener on log created event and handle it in a custom way.

# Usage
To call a log use default Unity logging methods like `Debug.Log(...), Debug.LogWarning(...), Debug.LogError(...)`
Logs Buffer package provides class `LogsBuffer` which holds static fields:
```
Action<LogItem> OnLog;
```
Event to which user can subscribe and add a custom functionality created by user to log created call -> ```LogsBuffer.OnLog += SaveLog;```
```
LogItem[] Logs { get { return logsBuffer.ToArray(); } }
```
Array which holds all created logs in the application. By referencing this property user can get `N-sized Logs[N]` array where `N` is the size of configured buffer size.

Example buffer configuration:
```
LogsBuffer.Init(LoggerConfig());
```
where LoggerConfig is a Scriptable Object configuration file created in editor.