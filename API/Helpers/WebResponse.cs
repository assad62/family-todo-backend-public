namespace API.Helpers
{
    public class WebResponse<T>
    {
        public int StatusCode { get; set; }

        public string Message { get; set; }

        public T Data { get; set; } = default;

        
    }
}
