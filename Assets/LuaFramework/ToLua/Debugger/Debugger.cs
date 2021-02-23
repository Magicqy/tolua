namespace LuaInterface
{
    public static class Debugger
    {
        public enum LogType
        {
            Log,
            Warning,
            Error,
            Exception,
        }

        public interface ILogHandler
        {
            void LogMessage(LogType logType, object arg0);
            void LogMessage(LogType logType, string format, object arg0);
            void LogMessage(LogType logType, string format, object arg0, object arg1);
            void LogMessage(LogType logType, string format, object arg0, object arg1, object arg2);
            void LogMessage(LogType logType, string format, params object[] args);
        }

        private class DummyLogHandler : ILogHandler
        {
            void ILogHandler.LogMessage(LogType logType, object arg0) { }
            void ILogHandler.LogMessage(LogType logType, string format, object arg0) { }
            void ILogHandler.LogMessage(LogType logType, string format, object arg0, object arg1) { }
            void ILogHandler.LogMessage(LogType logType, string format, object arg0, object arg1, object arg2) { }
            void ILogHandler.LogMessage(LogType logType, string format, params object[] args) { }
        }

#if USE_LUA_STANDALONE
        private class DefaultLogHandler : ILogHandler
        {
            void ILogHandler.LogMessage(LogType logType, object arg0)
            {
                System.Console.WriteLine(arg0);
            }

            void ILogHandler.LogMessage(LogType logType, string format, object arg0)
            {
                System.Console.WriteLine(format, arg0);
            }

            void ILogHandler.LogMessage(LogType logType, string format, object arg0, object arg1)
            {
                System.Console.WriteLine(format, arg0, arg1);
            }

            void ILogHandler.LogMessage(LogType logType, string format, object arg0, object arg1, object arg2)
            {
                System.Console.WriteLine(format, arg0, arg1, arg2);
            }

            void ILogHandler.LogMessage(LogType logType, string format, params object[] args)
            {
                System.Console.WriteLine(format, args);
            }
        }
#else
        private class DefaultLogHandler : ILogHandler
        {
            void ILogHandler.LogMessage(LogType logType, object arg0)
            {
                switch (logType)
                {
                    case LogType.Log:
                        UnityEngine.Debug.Log(arg0); break;
                    case LogType.Warning:
                        UnityEngine.Debug.LogWarning(arg0); break;
                    case LogType.Error:
                        UnityEngine.Debug.LogError(arg0); break;
                    case LogType.Exception:
                        UnityEngine.Debug.LogError(arg0); break;
                }
            }

            void ILogHandler.LogMessage(LogType logType, string format, object arg0)
            {
                switch (logType)
                {
                    case LogType.Log:
                        UnityEngine.Debug.LogFormat(format, arg0); break;
                    case LogType.Warning:
                        UnityEngine.Debug.LogWarningFormat(format, arg0); break;
                    case LogType.Error:
                        UnityEngine.Debug.LogErrorFormat(format, arg0); break;
                    case LogType.Exception:
                        UnityEngine.Debug.LogErrorFormat(format, arg0); break;
                }
            }

            void ILogHandler.LogMessage(LogType logType, string format, object arg0, object arg1)
            {
                switch (logType)
                {
                    case LogType.Log:
                        UnityEngine.Debug.LogFormat(format, arg0, arg1); break;
                    case LogType.Warning:
                        UnityEngine.Debug.LogWarningFormat(format, arg0, arg1); break;
                    case LogType.Error:
                        UnityEngine.Debug.LogErrorFormat(format, arg0, arg1); break;
                    case LogType.Exception:
                        UnityEngine.Debug.LogErrorFormat(format, arg0, arg1); break;
                }
            }

            void ILogHandler.LogMessage(LogType logType, string format, object arg0, object arg1, object arg2)
            {
                switch (logType)
                {
                    case LogType.Log:
                        UnityEngine.Debug.LogFormat(format, arg0, arg1, arg2); break;
                    case LogType.Warning:
                        UnityEngine.Debug.LogWarningFormat(format, arg0, arg1, arg2); break;
                    case LogType.Error:
                        UnityEngine.Debug.LogErrorFormat(format, arg0, arg1, arg2); break;
                    case LogType.Exception:
                        UnityEngine.Debug.LogErrorFormat(format, arg0, arg1, arg2); break;
                }
            }

            void ILogHandler.LogMessage(LogType logType, string format, params object[] args)
            {
                switch (logType)
                {
                    case LogType.Log:
                        UnityEngine.Debug.LogFormat(format, args); break;
                    case LogType.Warning:
                        UnityEngine.Debug.LogWarningFormat(format, args); break;
                    case LogType.Error:
                        UnityEngine.Debug.LogErrorFormat(format, args); break;
                    case LogType.Exception:
                        UnityEngine.Debug.LogErrorFormat(format, args); break;
                }
            }
        }
#endif

        private static DummyLogHandler dummyLogHandler = new DummyLogHandler();
        private static ILogHandler currLogHandler = dummyLogHandler;
        private static ILogHandler customLogHandler;

        static Debugger()
        {
            customLogHandler = new DefaultLogHandler();
        }

        public static void SetCustomLogHandler(ILogHandler handler)
        {
            customLogHandler = handler;
        }

        public static bool useLog
        {
            get { return currLogHandler != dummyLogHandler; }
            set { currLogHandler = value ? customLogHandler : dummyLogHandler; }
        }

        public static void Log(object arg0) { currLogHandler.LogMessage(LogType.Log, arg0); }
        public static void Log(string format, object arg0) { currLogHandler.LogMessage(LogType.Log, format, arg0); }
        public static void Log(string format, object arg0, object arg1) { currLogHandler.LogMessage(LogType.Log, format, arg0, arg1); }
        public static void Log(string format, object arg0, object arg1, object arg2) { currLogHandler.LogMessage(LogType.Log, format, arg0, arg1, arg2); }
        public static void Log(string format, params object[] args) { currLogHandler.LogMessage(LogType.Log, format, args); }

        public static void LogWarning(object arg0) { currLogHandler.LogMessage(LogType.Warning, arg0); }
        public static void LogWarning(string format, object arg0) { currLogHandler.LogMessage(LogType.Warning, format, arg0); }
        public static void LogWarning(string format, object arg0, object arg1) { currLogHandler.LogMessage(LogType.Warning, format, arg0, arg1); }
        public static void LogWarning(string format, object arg0, object arg1, object arg2) { currLogHandler.LogMessage(LogType.Warning, format, arg0, arg1, arg2); }
        public static void LogWarning(string format, params object[] args) { currLogHandler.LogMessage(LogType.Warning, format, args); }

        public static void LogError(object arg0) { currLogHandler.LogMessage(LogType.Error, arg0); }
        public static void LogError(string format, object arg0) { currLogHandler.LogMessage(LogType.Error, format, arg0); }
        public static void LogError(string format, object arg0, object arg1) { currLogHandler.LogMessage(LogType.Error, format, arg0, arg1); }
        public static void LogError(string format, object arg0, object arg1, object arg2) { currLogHandler.LogMessage(LogType.Error, format, arg0, arg1, arg2); }
        public static void LogError(string format, params object[] args) { currLogHandler.LogMessage(LogType.Error, format, args); }

        public static void LogException(System.Exception e) { currLogHandler.LogMessage(LogType.Exception, e); }
        public static void LogException(string message, System.Exception e) { currLogHandler.LogMessage(LogType.Exception, "{0} {1}", message, e); }
    }
}