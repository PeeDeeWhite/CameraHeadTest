namespace Vitec.CameraHead.Models;

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public static class DebugHelpers
{
    /// <summary>
    ///     Logs a debug message with the current UTC time and the name of the calling member.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="memberName">
    ///     The name of the calling member. This parameter is optional and is automatically provided by
    ///     the compiler.
    /// </param>
    public static void Log(string message, [CallerMemberName] string memberName = "")
    {
        Debug.WriteLine($"{DateTime.UtcNow:T} {memberName} {message}");
    }
}