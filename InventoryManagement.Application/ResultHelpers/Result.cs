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
        public ErrorType ErrorType { get; private set; }    
        private Result(bool isSuccess, T? value, string ? error , ErrorType  errorType = ErrorType.Success  )
        {
            IsSuccess = isSuccess;
            Data = value;
            ErrorMessage = error;
            ErrorType = errorType;  
        }
        public static Result<T> Success(T value) => new(true, value, string.Empty);
        public static Result<T> Failure(string error  ,ErrorType errorType) => new(false, default, error  , errorType);

    }
}
