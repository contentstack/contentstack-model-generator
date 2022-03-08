using System;
using System.IO;

namespace contentstack.CMA
{
    internal class ContentstackConstants
    {
        #region Private Variable
        private string _ContentTypes = "content_types";
      
        #endregion

        #region Private Constructors
        private ContentstackConstants()
        {

        }
        #endregion

        #region Public Properties
        public string ContentTypeUid { get; set; }
        public string EntryUid { get; set; }


        public string Content_Types
        {
            get { return this._ContentTypes ?? "content_types"; }
            set { this._ContentTypes = value; }
        }

        public string Entries
        {
            get { return this._ContentTypes ?? "entries"; }
            set { this._ContentTypes = value; }
        }

        #endregion

        #region Public Functions
        internal static ContentstackConstants Instance
        {
            get
            {
                return new ContentstackConstants();
            }
        }
        #endregion
    }
}
