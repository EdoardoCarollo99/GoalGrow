namespace GoalGrow.API.DTOs.Responses
{
    /// <summary>
    /// Standard API response wrapper
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public static ApiResponse<T> SuccessResponse(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message ?? "Operation completed successfully",
                Data = data,
                Timestamp = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Success response without data payload (for operations like logout, delete, etc.)
        /// </summary>
        public static ApiResponse<T> SuccessResponse(string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message ?? "Operation completed successfully",
                Data = default,
                Timestamp = DateTime.UtcNow
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Errors = errors ?? new List<string>(),
                Timestamp = DateTime.UtcNow
            };
        }
    }
}
