using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Penguin.Web.Dynamic
{
    /// <summary>
    /// Represents the result of validating a view path for existence
    /// </summary>
    public class ViewPathValidation
    {
        /// <summary>
        /// True if any path was found
        /// </summary>
        public bool FoundPath => ValidationResults.Any(r => r.Exists);

        /// <summary>
        /// A collection of results for the checked paths
        /// </summary>
        public List<ViewValidationResult> ValidationResults { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="fileProvider"></param>
        /// <param name="pathsToCheck">A list of {0} format strings to inject the path into</param>
        public ViewPathValidation(string path, IFileProvider fileProvider, List<string> pathsToCheck = null)
        {
            if (path is null)
            {
                throw new System.ArgumentNullException(nameof(path));
            }

            if (fileProvider is null)
            {
                throw new System.ArgumentNullException(nameof(fileProvider));
            }

            path = path.Replace(".cshtml", "");

            if (path.StartsWith("~", System.StringComparison.Ordinal))
            {
                path = path[1..];
            }

            ValidationResults = new List<ViewValidationResult>();

            pathsToCheck ??= new List<string>()
                {
                    "~{0}.cshtml"
                };

            foreach (string thisPath in pathsToCheck)
            {
                string toCheck = string.Format(CultureInfo.CurrentCulture, thisPath, path);
                ValidationResults.Add(new ViewValidationResult(toCheck, fileProvider.GetFileInfo(toCheck).Exists));
            }
        }

        /// <summary>
        /// Returns the first matching result
        /// </summary>
        /// <param name="SurpressError">If false, throws a file not found exception if no matches are found</param>
        /// <returns>A matching path or null if errors are surpressed</returns>
        public string Result(bool SurpressError = false)
        {
            return !SurpressError && !FoundPath
                ? throw new FileNotFoundException("No views found in the following paths: \r\n\r\n" + string.Join("\r\n", ValidationResults.Select(v => v.Path)))
                : (ValidationResults.FirstOrDefault(r => r.Exists)?.Path);
        }
    }
}