using System;
using System.Collections.Generic;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InventoryManagement.Application.ResultHelpers
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; } 
        public T ? Data { get; private set; }       
        public string ? ErrorMessage { get; private set; }
        protected Result(bool isSuccess, T? value, string error)
        {
            IsSuccess = isSuccess;
            Data = value;
            ErrorMessage = error;
        }
        public static Result<T> Success(T value) => new(true, value, string.Empty);
        public static Result<T> Failure(string error) => new(false, default, error);

    }
}
