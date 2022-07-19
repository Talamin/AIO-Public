using robotManager.Helpful;

namespace AIO.Framework
{
    public static class RotationLogger
    {
        public static LogLevel Level = LogLevel.INFO;

        public static void Trace(string log)
        {
            if (Level >= LogLevel.TRACE)
            {
                Logging.WriteDebug($"[RTF]: {log}");
            }
        }

        public static void Fight(string log)
        {
            Logging.WriteFight($"[RTF] {log}");
        }

        public static void LightDebug(string log)
        {
            if (Level >= LogLevel.DEBUG_LIGHT)
            {
                Logging.WriteFight($"[RTF] {log}");
            }
        }

        public static void Debug(string log)
        {
            if (Level >= LogLevel.DEBUG)
            {
                Logging.WriteFight($"[RTF] {log}");
            }
        }

        public enum LogLevel
        {
            INFO = 0,
            DEBUG_LIGHT = 1,
            DEBUG = 3,
            TRACE = 4,
        }
    }
}