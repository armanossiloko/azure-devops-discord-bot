namespace Azure.Discord.Bot.Service;

public static class Extensions
{
	public static LogLevel ToLogLevel(this LogSeverity severity)
	{
		return severity switch
		{
			LogSeverity.Critical => LogLevel.Critical,
			LogSeverity.Error => LogLevel.Error,
			LogSeverity.Warning => LogLevel.Warning,
			LogSeverity.Info => LogLevel.Information,
			LogSeverity.Debug => LogLevel.Debug,
			_ => LogLevel.Trace,
		};
	}

	public static string EnsureTrailing(this string value, char character)
	{
		if (string.IsNullOrEmpty(value))
			return value;

		return value.EndsWith(character) ? value : $"{value}{character}";
	}

	public static string EnsureTrailing(this string value, string appender)
	{
		if (string.IsNullOrEmpty(value))
			return value;

		return value.EndsWith(appender) ? value : $"{value}{appender}";
	}

	public static string WrapWith(this string value, char character)
	{
		if (string.IsNullOrEmpty(value))
			return value;

		return $"{character}{value}{character}";
	}

	public static string WrapWith(this string value, string @string)
	{
		if (string.IsNullOrEmpty(value))
			return value;

		return $"{@string}{value}{@string}";
	}

}