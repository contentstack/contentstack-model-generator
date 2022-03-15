﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

namespace contentstack.CMA
{
    /// <summary>
    /// The ContentstackException class is abstraction of general exception class.
    /// </summary>
    [Serializable]
    public class ContentstackException : Exception
    {
        #region Private Variables
        private string _ErrorMessage = string.Empty;
        
        #endregion

        #region Public Variables
        /// <summary>
        /// This is http response status code of REST request to Contentstack.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// This is error message.
        /// </summary>
        public new string Message { get; set; }

        /// <summary>
        /// This is error message.
        /// </summary>
        [JsonProperty("error_message")]
        public string ErrorMessage
        {
            get
            {
                return this._ErrorMessage;
            }
            set
            {
                this._ErrorMessage = value;
                this.Message = value;
            }
        }

        /// <summary>
        /// This is error code.
        /// </summary>
        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }

        /// <summary>
        /// Set of errors in detail.
        /// </summary>
        [JsonProperty("errors")]
        public Dictionary<string, object> Errors { get; set; }

        #endregion

        #region Public Constructors
        /// <summary>
        /// The ContentstackException class is abstraction of general exception class.
        /// </summary>
        public ContentstackException()
        {
        }

        /// <summary>
        /// The ContentstackException class is abstraction of general exception class.
        /// </summary>
        /// <param name="errorMessage"> Error Message</param>
        public ContentstackException(string message)
            : base(message)
        {
            this.ErrorMessage = message;
        }

        /// <summary>
        /// The ContentstackException class is abstraction of general exception class.
        /// </summary>
        /// <param name="exception"> Exception</param>
        public ContentstackException(Exception exception)
            : base(exception.Message, exception.InnerException)
        {
            this.ErrorMessage = exception.Message;
        }

        protected ContentstackException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ContentstackException(string message, Exception innerException) : base(message, innerException)
        {
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        #endregion


    }
}
