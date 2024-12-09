// BSD 2-Clause License
//
// Copyright (c) 2024, Elvonia
//
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
//
// 1. Redistributions of source code must retain the above copyright notice,
//    this list of conditions and the following disclaimer.
//
// 2. Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
// POSSIBILITY OF SUCH DAMAGE.

using System.Diagnostics;
using System.Reflection;
using MelonLoader;

namespace PoY_VR.Mod
{
    public static class Logger
    {
        public static void Log(string message, LogType logType = LogType.Info)
        {
            StackFrame stackFrame;

            if (logType == LogType.Error)
            {
                stackFrame = new StackTrace().GetFrame(2);
            }
            else
            {
                stackFrame = new StackTrace().GetFrame(1);
            }

            MethodBase method = stackFrame.GetMethod();
            string className = method.DeclaringType.Name;
            string methodName = method.Name;
            string logMessage = $"[{className}.{methodName}] {message}";

            switch (logType)
            {
                case LogType.Info:
                    MelonLogger.Msg(logMessage);
                    break;
                case LogType.Debug:
                    MelonLogger.Msg($"[DEBUG] {logMessage}");
                    break;
                case LogType.Warning:
                    MelonLogger.Warning(logMessage);
                    break;
                case LogType.Error:
                    MelonLogger.Error(logMessage);
                    break;
                default:
                    MelonLogger.Msg(logMessage);
                    break;
            }
        }

        public static void Info(string message)
        {
            Log(message, LogType.Info);
        }

        public static void Debug(string message)
        {
            Log(message, LogType.Debug);
        }

        public static void Warning(string message)
        {
            Log(message, LogType.Warning);
        }

        public static void Error(string message)
        {
            Log(message, LogType.Error);
        }

        public enum LogType
        {
            Info,
            Debug,
            Warning,
            Error
        }
    }
}
