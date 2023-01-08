using System.IO;
namespace CKS.Dev.VisualStudio.SharePoint.Environment
{
    /// <summary>
    /// A thread safe stream reader
    /// </summary>
    public class ThreadSafeStreamReader
    {
        private StreamReader streamReader;

        private string _Text;

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text
        {
            get
            {
                return this._Text;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadSafeStreamReader" /> class.
        /// </summary>
        /// <param name="sr">The sr.</param>
        public ThreadSafeStreamReader(StreamReader sr)
        {
            this.streamReader = sr;
        }

        /// <summary>
        /// Goes this instance.
        /// </summary>
        public void Go()
        {
            this._Text = this.streamReader.ReadToEnd();
        }
    }
}
