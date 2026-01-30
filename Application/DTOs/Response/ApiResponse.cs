namespace Application.DTOs.Response
{
    /// <summary>
    /// Generic API Response wrapper cho tất cả các endpoints
    /// </summary>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Trạng thái thành công của request
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Thông báo kết quả (thành công hoặc lỗi)
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Dữ liệu trả về (nullable)
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Danh sách các lỗi (nếu có)
        /// </summary>
        public List<string>? Errors { get; set; }

        /// <summary>
        /// Constructor mặc định
        /// </summary>
        public ApiResponse()
        {
        }

        /// <summary>
        /// Constructor cho response thành công
        /// </summary>
        public ApiResponse(T data, string message = "Success")
        {
            Success = true;
            Message = message;
            Data = data;
        }

        /// <summary>
        /// Constructor cho response thất bại
        /// </summary>
        public ApiResponse(string message, List<string>? errors = null)
        {
            Success = false;
            Message = message;
            Errors = errors;
            Data = default;
        }

        /// <summary>
        /// Tạo response thành công
        /// </summary>
        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
        {
            return new ApiResponse<T>(data, message);
        }

        /// <summary>
        /// Tạo response thất bại
        /// </summary>
        public static ApiResponse<T> FailureResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>(message, errors);
        }
    }

    /// <summary>
    /// Non-generic API Response (cho endpoints không trả về dữ liệu)
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Trạng thái thành công của request
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Thông báo kết quả
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Danh sách các lỗi
        /// </summary>
        public List<string>? Errors { get; set; }

        /// <summary>
        /// Constructor mặc định
        /// </summary>
        public ApiResponse()
        {
        }

        /// <summary>
        /// Constructor cho response thành công
        /// </summary>
        public ApiResponse(string message = "Success")
        {
            Success = true;
            Message = message;
        }

        /// <summary>
        /// Constructor cho response thất bại
        /// </summary>
        public ApiResponse(string message, List<string>? errors)
        {
            Success = false;
            Message = message;
            Errors = errors;
        }

        /// <summary>
        /// Tạo response thành công
        /// </summary>
        public static ApiResponse SuccessResponse(string message = "Success")
        {
            return new ApiResponse(message);
        }

        /// <summary>
        /// Tạo response thất bại
        /// </summary>
        public static ApiResponse FailureResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse(message, errors);
        }
    }
}
