using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace contentstack.CMA
{
    /// <summary>
    /// Represents a set of options to configure a Stack.
    /// </summary>
    public class ContentstackOptions
    {
        /// <summary>
        /// The api key used when communicating with the ContentStack API.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The authtoken used when communicating with the ContentStack API.
        /// </summary>
        public string Authtoken { get; set; }

        /// <summary>
        /// The Host used to set host url for the ContentStack API.
        /// </summary>
        public string Host { get; set; }

        /// <summary>
        /// The branch header in the API request to fetch or manage modules located within specific branches.
        /// </summary>
        public string Branch { get; set; }

        /// <summary>
        /// The Version number for the ContentStack API.
        /// </summary>
        public string Version { get; set; }
    }
}

