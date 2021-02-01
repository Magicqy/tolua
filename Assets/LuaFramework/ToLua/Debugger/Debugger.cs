#if USE_LUA_STANDALONE
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

        private static DummyLogHandler dummyLogHandler = new DummyLogHandler();
        private static ILogHandler currLogHandler = dummyLogHandler;
        private static ILogHandler customLogHandler;

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
#else
using UnityEngine;
using System;
using System.Text;

namespace LuaInterface
{
    public interface ILogger
    {
        void Log(string msg, string stack, LogType type);
    }

    public static class Debugger
    {
        public static bool useLog = true;
        public static string threadStack = string.Empty;
        public static ILogger logger = null;

        private static CString sb = new CString(256);

        static Debugger()
        {
            for (int i = 24; i < 70; i++)
            {
                StringPool.PreAlloc(i, 2);
            }
        }

        //减少gc alloc
        static string GetLogFormat(string str)
        {
            DateTime time = DateTime.Now;
            //StringBuilder sb = StringBuilderCache.Acquire();

            //sb.Append(ConstStringTable.GetTimeIntern(time.Hour))
            //    .Append(":")
            //    .Append(ConstStringTable.GetTimeIntern(time.Minute))
            //    .Append(":")
            //    .Append(ConstStringTable.GetTimeIntern(time.Second))
            //    .Append(".")
            //    .Append(time.Millisecond)
            //    .Append("-")
            //    .Append(Time.frameCount % 999)
            //    .Append(": ")
            //    .Append(str);

            //return StringBuilderCache.GetStringAndRelease(sb);

            sb.Clear();
            sb.Append(ConstStringTable.GetTimeIntern(time.Hour))
                .Append(":")
                .Append(ConstStringTable.GetTimeIntern(time.Minute))
                .Append(":")
                .Append(ConstStringTable.GetTimeIntern(time.Second))
                .Append(".")
                .Append(time.Millisecond)
                .Append("-")
                .Append(Time.frameCount % 999)
                .Append(": ")
                .Append(str);

            String dest = StringPool.Alloc(sb.Length);                        
            sb.CopyToString(dest);
            return dest;
        }

        public static void Log(string str)
        {
            str = GetLogFormat(str);            

            if (useLog)
            {
                Debug.Log(str);
            }
            else if (logger != null)
            {
                //普通log节省一点记录堆栈性能和避免调用手机系统log函数
                logger.Log(str, string.Empty, LogType.Log);
            }

            StringPool.Collect(str);
        }

        public static void Log(object message)
        {
            Log(message.ToString());
        }

        public static void Log(string str, object arg0)
        {
            string s = string.Format(str, arg0);
            Log(s);
        }

        public static void Log(string str, object arg0, object arg1)
        {
            string s = string.Format(str, arg0, arg1);
            Log(s);
        }

        public static void Log(string str, object arg0, object arg1, object arg2)
        {
            string s = string.Format(str, arg0, arg1, arg2);
            Log(s);
        }

        public static void Log(string str, params object[] param)
        {
            string s = string.Format(str, param);
            Log(s);
        }

        public static void LogWarning(string str)
        {
            str = GetLogFormat(str);            

            if (useLog)
            {
                Debug.LogWarning(str);
            }
            else if (logger != null)
            {
                string stack = StackTraceUtility.ExtractStackTrace();
                logger.Log(str, stack, LogType.Warning);
            }

            StringPool.Collect(str);
        }

        public static void LogWarning(object message)
        {
            LogWarning(message.ToString());
        }

        public static void LogWarning(string str, object arg0)
        {
            string s = string.Format(str, arg0);
            LogWarning(s);
        }

        public static void LogWarning(string str, object arg0, object arg1)
        {
            string s = string.Format(str, arg0, arg1);
            LogWarning(s);
        }

        public static void LogWarning(string str, object arg0, object arg1, object arg2)
        {
            string s = string.Format(str, arg0, arg1, arg2);
            LogWarning(s);
        }

        public static void LogWarning(string str, params object[] param)
        {
            string s = string.Format(str, param);
            LogWarning(s);
        }

        public static void LogError(string str)
        {
            str = GetLogFormat(str);            

            if (useLog)
            {
                Debug.LogError(str);
            }
            else if (logger != null)
            {
                string stack = StackTraceUtility.ExtractStackTrace();
                logger.Log(str, stack, LogType.Error);
            }

            StringPool.Collect(str);
        }

        public static void LogError(object message)
        {
            LogError(message.ToString());
        }

        public static void LogError(string str, object arg0)
        {
            string s = string.Format(str, arg0);
            LogError(s);
        }

        public static void LogError(string str, object arg0, object arg1)
        {
            string s = string.Format(str, arg0, arg1);
            LogError(s);
        }

        public static void LogError(string str, object arg0, object arg1, object arg2)
        {
            string s = string.Format(str, arg0, arg1, arg2);
            LogError(s);
        }

        public static void LogError(string str, params object[] param)
        {
            string s = string.Format(str, param);
            LogError(s);
        }


        public static void LogException(Exception e)
        {
            threadStack = e.StackTrace;            
            string str = GetLogFormat(e.Message);            

            if (useLog)
            {
                Debug.LogError(str);
            }
            else if (logger != null)
            {
                logger.Log(str, threadStack, LogType.Exception);
            }

            StringPool.Collect(str);
        }

        public static void LogException(string str, Exception e)
        {
            threadStack = e.StackTrace;            
            str = GetLogFormat(str + e.Message);            

            if (useLog)
            {
                Debug.LogError(str);
            }
            else if (logger != null)
            {
                logger.Log(str, threadStack, LogType.Exception);
            }

            StringPool.Collect(str);
        }
    }
}
#endif