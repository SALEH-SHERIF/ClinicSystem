using System.Reflection.Metadata.Ecma335;

namespace ClinicSystem.Models
{
	public class ApiResponse<T>
	{
		public bool Success { get; set; }
		public string Message { get; set; } = string.Empty;
		public T? Data { get; set; }
        public ApiResponse(){}
        public ApiResponse(bool _Success , string msg , T? data= default)
        {
            Success = _Success ;
            Message = msg ;
            Data = data ;
        }
        public static ApiResponse<T> SuccessResponse( string msg , T? data = default)=>new(true , msg , data);
        public static ApiResponse<T> Failure(string msg) =>new(false , msg);
	}
}
