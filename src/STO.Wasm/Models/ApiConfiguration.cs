namespace STO.Wasm.Models
{
    /// <summary>
    /// Used to strongly type the "ApiConfiguration" appsettings section
    /// </summary>
    public class ApiConfiguration
    {
        /// <summary>
        /// The host url with protocol and port for the Api.
        /// </summary>
        public string ApiHost { get; set; }
    }
}