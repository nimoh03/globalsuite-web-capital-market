#pragma warning disable CS8618
using System;
using System.Collections.Generic;

namespace GlobalSuite.Core.Exceptions
{
    public class ApiException : AppException
    {
        public int StatusCode { get; set; }
        public bool IsModelValidatonError { get; set; }
        public IEnumerable<ValidationError> Errors { get; set; }
        public string ReferenceErrorCode { get; set; }
        public string ReferenceDocumentLink { get; set; }
        public object CustomError { get; set; }
        public bool IsCustomErrorObject { get; set; } = false;

        public ApiException(string message,
          int statusCode,
          string errorCode = default,
          string refLink = default) :
          base(message)
        {
            StatusCode = statusCode;
            ReferenceErrorCode = errorCode;
            ReferenceDocumentLink = refLink;
        }


        public ApiException(IEnumerable<ValidationError> errors, int statusCode)
        {
            IsModelValidatonError = true;
            StatusCode = statusCode;
            Errors = errors;
        }

        public ApiException(Exception ex, int statusCode) : base(ex.Message)
        {
            StatusCode = statusCode;
        }
    }
}