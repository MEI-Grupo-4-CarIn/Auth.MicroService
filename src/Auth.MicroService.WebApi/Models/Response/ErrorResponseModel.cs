namespace Auth.MicroService.WebApi.Models.Response
{
    /// <summary>
    /// The error response model.
    /// </summary>
    public class ErrorResponseModel
    {
        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message { get; set; }
    }
}