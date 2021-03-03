using Microsoft.VisualStudio.SharePoint;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace CKS.Dev2015.VisualStudio.SharePoint.Content
{
    /// <summary>
    /// SPMetal Definition CKSProperties.
    /// </summary>
    class SPMetalDefinitionProperties
    {
        #region Nested Classes

        /// <summary>
        /// SPMetal Def Properties description provider.
        /// </summary>
        class SPMetalDefinitionPropertiesDescriptionProvider : TypeConverter
        {
            #region Methods

            /// <summary>
            /// Returns a collection of properties for the type of array specified by the value parameter, using the specified context and attributes.
            /// </summary>
            /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
            /// <param name="value">An <see cref="T:System.Object"/> that specifies the type of array for which to get CKSProperties.</param>
            /// <param name="attributes">An array of type <see cref="T:System.Attribute"/> that is used as a filter.</param>
            /// <returns>
            /// A <see cref="T:System.ComponentModel.PropertyDescriptorCollection"/> with the properties that are exposed for this data type, or null if there are no CKSProperties.
            /// </returns>
            public override PropertyDescriptorCollection GetProperties(
                ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                SPMetalDefinitionProperties configProperties = (SPMetalDefinitionProperties)value;
                PropertyDescriptorCollection properties = base.GetProperties(context, value, attributes);
                if (configProperties.Mode == SPMetalDefinitionSource.CurrentDeploymentSite)
                {
                    PropertyDescriptor urlDescriptor = properties["Url"];
                    properties.Remove(urlDescriptor);
                    properties.Add(new ReadOnlyPropertyDescriptor("Url", null)
                    {
                        InnerDescriptor = urlDescriptor,
                        ReadOnlyEvaluator = p => true
                    });
                }
                return properties;
            }

            #endregion
        }

        /// <summary>
        /// Read only property descriptor.
        /// </summary>
        class ReadOnlyPropertyDescriptor : PropertyDescriptor
        {
            #region Properties

            /// <summary>
            /// Gets or sets the inner descriptor.
            /// </summary>
            /// <value>The inner descriptor.</value>
            public PropertyDescriptor InnerDescriptor { get; set; }

            /// <summary>
            /// Gets or sets the read only evaluator.
            /// </summary>
            /// <value>The read only evaluator.</value>
            public Func<PropertyDescriptor, bool> ReadOnlyEvaluator { get; set; }

            /// <summary>
            /// When overridden in a derived class, gets the type of the component this property is bound to.
            /// </summary>
            /// <value></value>
            /// <returns>A <see cref="T:System.Type"/> that represents the type of component this property is bound to. When the <see cref="M:System.ComponentModel.PropertyDescriptor.GetValue(System.Object)"/> or <see cref="M:System.ComponentModel.PropertyDescriptor.SetValue(System.Object,System.Object)"/> methods are invoked, the object specified might be an instance of this type.</returns>
            public override Type ComponentType
            {
                get { return InnerDescriptor.ComponentType; }
            }

            /// <summary>
            /// When overridden in a derived class, gets a value indicating whether this property is read-only.
            /// </summary>
            /// <value></value>
            /// <returns>true if the property is read-only; otherwise, false.</returns>
            public override bool IsReadOnly
            {
                get
                {
                    return ReadOnlyEvaluator != null ?
                        ReadOnlyEvaluator(InnerDescriptor) :
                        InnerDescriptor.IsReadOnly;
                }
            }

            /// <summary>
            /// When overridden in a derived class, gets the type of the property.
            /// </summary>
            /// <value></value>
            /// <returns>A <see cref="T:System.Type"/> that represents the type of the property.</returns>
            public override Type PropertyType
            {
                get { return InnerDescriptor.PropertyType; }
            }

            #endregion

            #region Methods

            /// <summary>
            /// When overridden in a derived class, returns whether resetting an object changes its value.
            /// </summary>
            /// <param name="component">The component to test for reset capability.</param>
            /// <returns>
            /// true if resetting the component changes its value; otherwise, false.
            /// </returns>
            public override bool CanResetValue(object component)
            {
                return InnerDescriptor.CanResetValue(component);
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ReadOnlyPropertyDescriptor"/> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="attrs">The attrs.</param>
            public ReadOnlyPropertyDescriptor(string name, Attribute[] attrs)
                : base(name, attrs)
            {
            }

            /// <summary>
            /// When overridden in a derived class, gets the current value of the property on a component.
            /// </summary>
            /// <param name="component">The component with the property for which to retrieve the value.</param>
            /// <returns>
            /// The value of a property for a given component.
            /// </returns>
            public override object GetValue(object component)
            {
                return InnerDescriptor.GetValue(component);
            }


            /// <summary>
            /// When overridden in a derived class, resets the value for this property of the component to the default value.
            /// </summary>
            /// <param name="component">The component with the property value that is to be reset to the default value.</param>
            public override void ResetValue(object component)
            {
                InnerDescriptor.ResetValue(component);
            }

            /// <summary>
            /// When overridden in a derived class, sets the value of the component to a different value.
            /// </summary>
            /// <param name="component">The component with the property value that is to be set.</param>
            /// <param name="value">The new value.</param>
            public override void SetValue(object component, object value)
            {
                InnerDescriptor.SetValue(component, value);
            }

            /// <summary>
            /// When overridden in a derived class, determines a value indicating whether the value of this property needs to be persisted.
            /// </summary>
            /// <param name="component">The component with the property to be examined for persistence.</param>
            /// <returns>
            /// true if the property should be persisted; otherwise, false.
            /// </returns>
            public override bool ShouldSerializeValue(object component)
            {
                return InnerDescriptor.ShouldSerializeValue(component);
            }

            #endregion
        }

        #endregion

        #region Constants

        const string PropertyMode = "CKS.Dev.SharePoint.SPMetalDefinition.Mode";
        const string PropertyUrl = "CKS.Dev.SharePoint.SPMetalDefinition.Url";
        const string PropertySerialization = "CKS.Dev.SharePoint.SPMetalDefinition.Serialization";

        #endregion

        #region Fields

        IDictionary<string, string> _properties;
        ISharePointProjectItem _owner;
        SPMetalDefinitionTypeProvider _definition;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the mode.
        /// </summary>
        /// <value>The mode.</value>
        [DefaultValue(SPMetalDefinitionSource.CurrentDeploymentSite)]
        [Category(PropertyCategory.DevTools)]
        [RefreshProperties(RefreshProperties.All)]
        public SPMetalDefinitionSource Mode
        {
            get
            {
                string mode;
                _properties.TryGetValue(PropertyMode, out mode);
                return mode != null ? (SPMetalDefinitionSource)Enum.Parse(typeof(SPMetalDefinitionSource), mode) : SPMetalDefinitionSource.CurrentDeploymentSite;
            }
            set
            {
                _properties[PropertyMode] = value.ToString();
                _definition.ForceRegenerate(_owner, this);
            }
        }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>The URL.</value>
        [DefaultValue((string)null)]
        [Category(PropertyCategory.DevTools)]
        public string Url
        {
            get
            {
                if (Mode == SPMetalDefinitionSource.CurrentDeploymentSite)
                {
                    return _owner.Project.SiteUrl.ToString();
                }
                else
                {
                    string url;
                    _properties.TryGetValue(PropertyUrl, out url);
                    return url;
                }
            }
            set
            {
                _properties[PropertyUrl] = value;
                _definition.ForceRegenerate(_owner, this);
            }
        }

        /// <summary>
        /// Gets or sets the serialization.
        /// </summary>
        /// <value>The serialization.</value>
        [DefaultValue(SPMetalDefinitionSerialization.None)]
        [Category(PropertyCategory.DevTools)]
        public SPMetalDefinitionSerialization Serialization
        {
            get
            {
                string mode;
                _properties.TryGetValue(PropertySerialization, out mode);
                return mode != null ? (SPMetalDefinitionSerialization)Enum.Parse(typeof(SPMetalDefinitionSerialization), mode) : SPMetalDefinitionSerialization.None;
            }
            set
            {
                _properties[PropertySerialization] = value.ToString();
                _definition.ForceRegenerate(_owner, this);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="SPMetalDefinitionProperties"/> class.
        /// </summary>
        /// <param name="definition">The definition.</param>
        /// <param name="owner">The owner.</param>
        /// <param name="properties">The CKSProperties.</param>
        public SPMetalDefinitionProperties(SPMetalDefinitionTypeProvider definition,
            ISharePointProjectItem owner,
            IDictionary<string, string> properties)
        {
            _owner = owner;
            _properties = properties;
            _definition = definition;
        }

        #endregion
    }
}
