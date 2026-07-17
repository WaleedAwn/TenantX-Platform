using System.ComponentModel;
using System.Text.Json.Serialization;

using TenantX.Common.Domain.Results.Abstractions;

namespace TenantX.Common.Domain.Results;

public static class Result
{
	public static Success Success => default;
	public static Created Created => default;
	public static Deleted Deleted => default;
	public static Updated Updated => default;
	public static Result<TValue> Successful<TValue>(TValue value)
		=> new(value);

	public static Result<TValue> Failure<TValue>(Error error)
		=> new(error);

	public static Result<TValue> Failure<TValue>(List<Error> errors)
		=> new(errors);

}

public sealed class Result<TValue> : IResult<TValue>
{
	private readonly List<Error>? _errors;

	public bool IsSuccess { get; }

	[JsonConstructor]
	[EditorBrowsable(EditorBrowsableState.Never)]
	// [Obsolete("For serializer only.", true)]
	public Result(TValue? value, List<Error>? errors, bool isSuccess)
	{
		if (isSuccess)
		{
			Value = value ?? throw new ArgumentNullException(nameof(value));
			_errors = [];
			IsSuccess = true;
		}
		else
		{
			if (errors == null || errors.Count == 0)
			{
				throw new ArgumentException("Provide at least one error.", nameof(errors));
			}

			_errors = errors;
			Value = default!;
			IsSuccess = false;
		}
	}

	internal Result(Error error)
	{
		_errors = [error];
		IsSuccess = false;
	}

	internal Result(List<Error> errors)
	{
		if (errors is null || errors.Count == 0)
		{
			throw new ArgumentException("Cannot create a Result<TValue> with an empty list of errors.", nameof(errors));
		}

		_errors = errors;

		IsSuccess = false;
	}

	internal Result(TValue value)
	{
		if (value is null)
		{
			throw new ArgumentNullException(nameof(value));
		}

		Value = value;

		IsSuccess = true;
	}

	public bool IsError => !IsSuccess;

	public List<Error> Errors => IsError ? _errors! : [];

	public TValue Value => IsSuccess ? field! : default!;

	public Error TopError => (_errors?.Count > 0) ? _errors[0] : default;

	public TNextValue Match<TNextValue>(Func<TValue, TNextValue> onValue, Func<List<Error>, TNextValue> onError)
		=> IsSuccess ? onValue(Value!) : onError(Errors);

	public static implicit operator Result<TValue>(TValue value)
		=> new(value);

	public static implicit operator Result<TValue>(Error error)
		=> new(error);

	public static implicit operator Result<TValue>(List<Error> errors)
		=> new(errors);
}


public readonly record struct Success;
public readonly record struct Created;
public readonly record struct Deleted;
public readonly record struct Updated;

