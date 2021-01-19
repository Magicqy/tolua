/*copy from .net by topameng*/

using System;
using System.Reflection;
using System.Text;

namespace LuaInterface
{
    public static class StringBuilderCache
    {
        public static void AppendLineEx(this StringBuilder sb, string str = null)
        {
            if (str == null)
            {
                sb.Append("\r\n");
            }
            else
            {
                sb.Append(str).Append("\r\n");
            }
        }

        [ThreadStatic]
        static StringBuilder _cache = new StringBuilder(256);
        private const int MAX_BUILDER_SIZE = 512;

        public static StringBuilder Acquire(int capacity = 256)
        {
            StringBuilder sb = _cache;

            if (sb != null && sb.Capacity >= capacity)
            {
                _cache = null;
                sb.Clear();
                return sb;
            }

            return new StringBuilder(capacity);
        }

        public static string GetStringAndRelease(StringBuilder sb)
        {
            string str = sb.ToString();
            Release(sb);
            return str;
        }

        public static void Release(StringBuilder sb)
        {
            if (sb.Capacity <= MAX_BUILDER_SIZE)
            {
                _cache = sb;
            }
        }
    }
}
