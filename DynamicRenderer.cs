using Loxifi;
using Microsoft.Extensions.FileProviders;
using Penguin.Extensions.Collections;
using Penguin.Extensions.String;
using Penguin.Reflection.Abstractions;
using Penguin.Reflection.Serialization.Abstractions.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace Penguin.Web.Dynamic
{
    /// <summary>
    /// Analyzes a serialized object and returns a list of validated view paths representing
    /// Prefered rendering paths
    /// </summary>
    public class DynamicRenderer
    {
        /// <summary>
        /// A list of ViewResults that exist on disk
        /// </summary>
        public IEnumerable<ViewValidationResult> ExistingHandlers => Results.Where(r => r.Exists);

        /// <summary>
        /// Figure out what this does and comment it
        /// </summary>
        public bool HasHtml { get; set; }

        /// <summary>
        /// True if there are any views set up to handle this object
        /// </summary>
        public bool HasMatch => Results.Any(r => r.Exists);

        /// <summary>
        /// True if the only view that can handle this object is the recursive view
        /// </summary>
        public bool IsDynamic { get; set; }

        /// <summary>
        /// Chooses the first existing view with the highest priority
        /// </summary>
        public ViewValidationResult Match => Results.Where(r => r.Exists).First();

        /// <summary>
        /// The path to the view of highest matching priority
        /// </summary>
        public string MatchedPath => Match.Path;

        /// <summary>
        /// A list of results for potential paths, as well as whether or not the paths exist on disk
        /// </summary>
        public List<ViewValidationResult> Results { get; }

        /// <summary>
        /// Constructs a new instance of this renderer
        /// </summary>
        /// <param name="type">An optional override type</param>
        /// <param name="property">The IMetaProperty leading to this serialized object</param>
        /// <param name="fileProvider">A file provider used for checking for the existence of views</param>
        public DynamicRenderer(IMetaType type, IMetaProperty property, IFileProvider fileProvider) : this(new DynamicRendererSettings(type, property, fileProvider))
        {
        }

        /// <summary>
        /// Constructs a new instance of this renderer
        /// </summary>
        /// <param name="property">The IMetaProperty leading to this serialized object</param>
        /// <param name="fileProvider">A file provider used for checking for the existence of views </param>
        public DynamicRenderer(IMetaProperty property, IFileProvider fileProvider) : this(new DynamicRendererSettings(property, fileProvider))
        {
        }

        /// <summary>
        /// Constructs a new instance of this renderer
        /// </summary>
        /// <param name="settings">A collection of settings for the dynamic renderer</param>
        public DynamicRenderer(DynamicRendererSettings settings)
        {
            if (settings is null)
            {
                throw new System.ArgumentNullException(nameof(settings));
            }

            List<string> renderingOrder = null;

            if (settings.Type is null)
            {
                renderingOrder = new List<string> { settings.TypeFullName };
            }
            else
            {
                renderingOrder = new List<string>();

                IMetaType CascadeType = settings.Type.CoreType == CoreType.Collection ? settings.Type.CollectionType : settings.Type;

                string Prepend = string.Empty;

                if (settings.Type.CoreType == CoreType.Collection)
                {
                    Prepend = settings.Type.IsArray ? "Array." : $"{settings.Type.Namespace}.{settings.Type.Name.To("`")}.";
                }

                do
                {
                    string TypeName = CascadeType.FullName;

                    if (settings.Type.CoreType == CoreType.Collection && settings.Type.IsArray)
                    {
                        TypeName = CascadeType.FullName.To("[");
                    }

                    if (settings.Type.IsNullable)
                    {
                        TypeName = CascadeType.FullName.To("`") + "." + CascadeType.FullName.FromLast("[").To(",");
                    }

                    if (settings.Property != null && settings.Property.Attributes.AnyNotNull())
                    {
                        //search based on attribute
                        foreach (IMetaAttribute a in settings.Property.Attributes.Where(a => !a.IsInherited))
                        {
                            renderingOrder.Add($"{Prepend}@{a.Instance.Value}");
                        }
                    }

                    renderingOrder.Add($"{Prepend}{TypeName}");

                    if (CascadeType.Attributes.AnyNotNull())
                    {
                        //search based on attribute
                        foreach (IMetaAttribute a in CascadeType.Attributes.Where(a => !a.IsInherited))
                        {
                            renderingOrder.Add($"{Prepend}@{a.Instance.Value}");
                        }
                    }

                    CascadeType = CascadeType.BaseType;
                }
                while (!settings.ExactOnly && CascadeType != null);

                //for all objects in namespace
                renderingOrder.Add($"{settings.Type.Namespace}.{Prepend}$Object");

                if (settings.StartAtBottom)
                {
                    renderingOrder.Reverse();
                }
            }

            Results = new List<ViewValidationResult>();

            foreach (string thisType in renderingOrder)
            {
                string @namespace = thisType.Remove(Constants.RootNamespace + ".").Replace(".", "/");
                ViewPathValidation pathValidation = new(settings.BasePath + @namespace, settings.FileProvider);

                foreach (ViewValidationResult result in pathValidation.ValidationResults)
                {
                    Results.Add(result);
                }
            }

            if (!HasMatch)
            {
                IsDynamic = true;
                ViewPathValidation pathValidation = new(settings.BasePath + settings.DynamicViewName, settings.FileProvider);

                foreach (ViewValidationResult result in pathValidation.ValidationResults)
                {
                    Results.Add(result);
                }
            }
        }
    }
}