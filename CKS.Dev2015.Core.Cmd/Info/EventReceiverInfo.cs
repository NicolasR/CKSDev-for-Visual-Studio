using System;

namespace CKS.Dev2015.VisualStudio.SharePoint.Commands.Info
{
    /// <summary>
    /// Contains basic data about a list event receiver on the SharePoint site. This class is 
    /// serializable so that instances of it can be sent between the Visual Studio and 
    /// SharePoint command assemblies.
    /// </summary>
    [Serializable]
    public class EventReceiverInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the list id.
        /// </summary>
        public Guid ListId { get; set; }

        /// <summary>
        /// Gets or sets the assembly.
        /// </summary>
        public string Assembly { get; set; }

        /// <summary>
        /// Gets or sets the class.
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// Gets or sets the event type.
        /// </summary>
        public int EventType { get; set; }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        #endregion
    }
}
