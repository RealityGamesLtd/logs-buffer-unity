# TODO

# Description
Logs Buffer package gives you a possibility to store and handle custom created logs in various ways, by using this package user can get information about logs that were created in the application. Logs can be stored in buffer which is configured by user in the code. Logs are stored as a LogItem instance which holds information about their message, creation time, and type (Error, Warning,...)

This package replaces Unity default Debug.unityLogger.logHandler reference and wraps standard logging methods like: 
Debug.Log(), Debug.LogWarning(), Debug.LogError(), etc.
Logs Buffer gives a possibility to add specific listener on log created event and handle it in a custom way.

# Usage
