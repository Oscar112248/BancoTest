using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public sealed class Result<T>
    {
        public bool IsSuccess { get; init; }
        public string? Message { get; init; }
        public T? Data { get; init; }
        public int StatusCode { get; init; }

        private Result(bool success, int statusCode, string? message = null, T? data = default)
        {
            IsSuccess = success;
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        public static Result<T> Success(T data, string? message = null)
            => new(true, StatusCodes.Status200OK, message, data);

        public static Result<T> Success(string? message = null)
            => new(true, StatusCodes.Status200OK, message);

        public static Result<T> Failure(string message, int statusCode = StatusCodes.Status400BadRequest)
            => new(false, statusCode, message);
    }
}
