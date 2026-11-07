namespace SolTK.CommonUtil;

using System;
using System.Diagnostics;

public sealed class UnimplementedException : NotImplementedException
{
    public string CallerMember { get; }
    public string CallerFile { get; }
    public int CallerLine { get; }

    public UnimplementedException(string? message = null)
        : base(message ?? "Unimplemented")
    {
        // Frame 0 = this constructor
        // Frame 1 = the method that did `new UnimplementedException(...)`
        var frame = new StackFrame(1, needFileInfo: true);
        var method = frame.GetMethod();

        CallerMember = method?.Name ?? "<unknown>";
        CallerFile   = frame.GetFileName() ?? "<unknown>";
        CallerLine   = frame.GetFileLineNumber();
    }

    public override string Message =>
        $"{base.Message} at {Path.GetFileName(CallerFile)}:{CallerLine} ({CallerMember})";
}