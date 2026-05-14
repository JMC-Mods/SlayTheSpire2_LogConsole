using MegaCrit.Sts2.Core.Logging;

namespace LogConsole.Core;

public readonly record struct LogEntry(
    long Sequence,
    DateTime Time,
    LogLevel Level,
    string Message);
