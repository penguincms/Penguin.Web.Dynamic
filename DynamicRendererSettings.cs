using Microsoft.Extensions.FileProviders;
using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using Penguin.Reflection.Serialization.Abstractions.Wrappers;
using System;
using System.Diagnostics.Contracts;

namespace Penguin.Web.Dynamic
{
    /// <summary>
    /// Settings for the dynamic renderer
    /// </summary>
    public class DynamicRendererSettings
    {
        /// <summary>
        /// A link to the base path for views for the editor.
        /// Points to Paths.EditBase
        /// </summary>
        public string BasePath { get; set; } = Paths.EDIT_BASE;

        /// <summary>
        /// The name of the view used to render dynamic objects
        /// Default: Edit
        /// </summary>
        public string DynamicViewName { get; set; } = "Edit";

        /// <summary>
        /// Only render Dynamic or Exact Matches. No intermediate types
        /// </summary>
        public bool ExactOnly { get; set; }

        /// <summary>
        /// A File Provider implementation used for checking for view
        /// </summary>
        public IFileProvider FileProvider { get; set; }

        /// <summary>
        /// The IMetaProperty that references the object being resolved
        /// </summary>
        public IMetaProperty Property { get; set; }

        /// <summary>
        /// Render base types before inherited types
        /// </summary>
        public bool StartAtBottom { get; set; } = false;

        /// <summary>
        /// An optional override type for the renderer
        /// </summary>
        public IMetaType Type { get; set; }

        /// <summary>
        /// If no type is set, the renderer can attempt to use this string value to determine routing
        /// </summary>
        public string TypeFullName { get; set; }

        /// <summary>
        /// Constructs a new instance of the settings object
        /// </summary>
        /// <param name="type">An optional override type for the renderer</param>
        /// <param name="property">The IMetaProperty that references the object being resolved</param>
        /// <param name="fileProvider">A file provider used for checking for the existence of views</param>
        public DynamicRendererSettings(IMetaType type, IMetaProperty property, IFileProvider fileProvider)
        {
            Contract.Requires(type != null);

            this.Type = type;
            this.TypeFullName = this.Type.FullName;
            this.Property = property;
            this.FileProvider = fileProvider;
        }

        /// <summary>
        /// Constructs a new instance of the settings object
        /// </summary>
        /// <param name="property">The IMetaProperty that references the object being resolved</param>
        /// <param name="fileProvider">A file provider used for checking for views</param>
        public DynamicRendererSettings(IMetaProperty property, IFileProvider fileProvider)
        {
            Contract.Requires(property != null);

            this.Type = property.Type;
            this.TypeFullName = this.Type.FullName;
            this.Property = property;
            this.FileProvider = fileProvider;
        }

        /// <summary>
        /// Constructs a new instance of the settings object
        /// </summary>
        /// <param name="type">An optional override type for the renderer</param>
        /// <param name="fileProvider">A file provider used for checking for the existence of views</param>
        public DynamicRendererSettings(Type type, IFileProvider fileProvider) : this(new MetaTypeHolder(type), null, fileProvider)
        {
        }

        /// <summary>
        /// Constructs a new instance of the settings object
        /// </summary>
        /// <param name="property">The IMetaProperty that references the object being resolved</param>
        /// <param name="fileProvider">A file provider used for checking for the existence of views</param>
        public DynamicRendererSettings(System.Reflection.PropertyInfo property, IFileProvider fileProvider) : this(new MetaPropertyHolder(property), fileProvider)
        {
        }

        /// <summary>
        /// Constructs a new instance of the settings object
        /// </summary>
        /// <param name="o">An IMetaObject instance to render out</param>
        /// <param name="fileProvider">A file provider used for checking for the existence of views</param>
        public DynamicRendererSettings(IMetaObject o, IFileProvider fileProvider)
        {
            Contract.Requires(o != null);

            this.FileProvider = fileProvider;
            this.Type = o.Type;
            this.TypeFullName = this.Type.FullName;
        }

        /// <summary>
        /// Constructs a new instance of the settings object
        /// </summary>
        /// <param name="typeFullName">If no type is set, the renderer can attempt to use this string value to determine routing</param>
        /// <param name="fileProvider">A file provider used for checking for the existence of views</param>
        public DynamicRendererSettings(string typeFullName, IFileProvider fileProvider)
        {
            this.FileProvider = fileProvider;
            this.TypeFullName = typeFullName;
        }
    }
}