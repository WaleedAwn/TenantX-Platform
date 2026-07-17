using TenantX.Common.Domain.Results;

namespace TenantX.Common.Application.Exceptions;

public sealed class TenantXException : Exception
{

	public TenantXException(string requestName, Error? error, Exception? innerException = default)
		: base("Application exception", innerException)
	{
		RequestName = requestName;
		Error = error;
	}

	public TenantXException() : base("Application exception")
	{
	}

	public TenantXException(string? message) : base(message)
	{

	}

	public TenantXException(string? message, Exception? innerException) : base(message, innerException)
	{
	}

	public string? RequestName { get; }

	public Error? Error { get; }
}
