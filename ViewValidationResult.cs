namespace Penguin.Web.Dynamic
{
    /// <summary>
    /// Represents the result of validating a single path
    /// </summary>
    public class ViewValidationResult
    {
        /// <summary>
        /// If true, this path exists on disk
        /// </summary>
        public bool Exists { get; protected set; }

        /// <summary>
        /// The path that was checked
        /// </summary>
        public string Path { get; protected set; }

        internal ViewValidationResult(string path, bool exists)
        {
            this.Path = path;
            this.Exists = exists;
        }

        /// <summary>
        /// ToString returns the path and the bool representing its existence for debug view
        /// </summary>
        /// <returns>Returns the path and the bool representing its existence for debug view</returns>
        public override string ToString() => $"{this.Path} - {this.Exists}";
    }
}