namespace Chromely.Mvc
{
    internal struct CallbackResponseStruct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CallbackResponseStruct"/> struct.
        /// </summary>
        /// <param name="response">
        /// The response.
        /// </param>
        public CallbackResponseStruct(string response)
        {
            ResponseText = response;
        }

        /// <summary>
        /// Gets or sets the response text.
        /// </summary>
        public string ResponseText { get; set; }
    }
}
