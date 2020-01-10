using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace contentstack.CMA
{
    internal class Config
    {
        #region Private Variable
        private string _Protocol;
        private string _Host;
        private string _Port;
        private string _Version;
        #endregion

        #region Public Properties
        public string ApiKey { get; set; }

        public string AppUid { get; set; }

        public string AuthToken { get; set; }

        public string Port { 
            get { return this._Port ?? "443"; }
            set { this._Port = value; }
        }
       
        public string Protocol { 
            get { return this._Protocol ?? "https"; }
            set { this._Protocol = value; }
        }

        public string Host { 
            get { return _Host ?? HostURL; }
            set { this._Host = value; }
        }


        public string Version
        {
            get { return this._Version ?? "v3"; }
            set { this._Version = value; }
        }


        public string BaseUrl
        {
            get
            {
                string BaseURL = string.Format("{0}://{1}/{2}",
                                              this.Protocol.Trim('/').Trim('\\'),
                                              this.Host.Trim('/').Trim('\\'),
                                              this.Version.Trim('/').Trim('\\'));
                return BaseURL;
            }
        }

        #endregion

        #region Internal

       
        internal string HostURL
        {
            get
            {
                return "cdn.contentstack.io";
            }
        }
        #endregion
    }
}
