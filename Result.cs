#nullable enable
using System;

namespace AP.Utilities
{
	public class Result<TValue,TError>
	{
		public readonly TValue? Value;
		public readonly TError? Error;

		public bool IsSuccess => Value != null;
		public bool IsFailure => Error != null;

		private Result(TValue value) => Value = value;

		private Result(TError error) => Error = error;

		public static implicit operator Result<TValue, TError>(TValue value) => new Result<TValue, TError>(value);

		public static implicit operator Result<TValue, TError> (TError error)=> new Result<TValue, TError>(error);

		public Result<TValue, TError> Match(Func<TValue, Result<TValue, TError>> success, Func<TError, Result<TValue, TError>> failure)
		{
			return IsSuccess ? success(Value!) : failure(Error!);
		}
	}
}