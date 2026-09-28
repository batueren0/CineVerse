using System;
using System.Collections.Generic;
using System.Text;

namespace CineVerse.Application.Common;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T? Data { get; }
    public string ErrorMessage { get; } = string.Empty;

    private Result(bool isSucces, T? data, string errorMessage)
    {
        this.IsSuccess = isSucces;
        this.Data = data;
        this.ErrorMessage = errorMessage;
    }
    public static Result<T> Success(T data) => new(true, data, string.Empty);
    public static Result<T> Failure(string errorMessage) => new(false, default, errorMessage);
}

public class Result
{
    public bool IsSuccess { get; }
    public string ErrorMessage { get; }

    private Result(bool isSucces, string errorMessage)
    {
        this.IsSuccess = isSucces;
        this.ErrorMessage = errorMessage;
    }

    public static Result Success() => new(true, string.Empty);
    public static Result Failure(string errorMessage) => new(false, errorMessage);
}

