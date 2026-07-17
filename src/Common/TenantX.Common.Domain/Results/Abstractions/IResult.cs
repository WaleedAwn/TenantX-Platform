using TenantX.Common.Domain.Results;

namespace TenantX.Common.Domain.Results.Abstractions;

public interface IResult
{
	List<Error>? Errors { get; }

	bool IsSuccess { get; }
}

public interface IResult<out TValue> : IResult
{
	TValue Value { get; }
}
