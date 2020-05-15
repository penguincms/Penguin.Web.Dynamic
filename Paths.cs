namespace Penguin.Web.Dynamic
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public static class Paths
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// The path to check for admin dynamic rendering views
        /// </summary>
        public const string ADMIN_RENDER_BASE = "/Areas/Admin/Views/Render/";

        /// <summary>
        /// The path that the dynamic wrapper resides in
        /// </summary>
        public const string EDIT_BASE = "/Areas/Admin/Views/Edit/";

        /// <summary>
        /// The path that the dynamic client visible renderers exists in
        /// </summary>
        public const string RENDER_BASE = "/Views/Render/";
    }
}