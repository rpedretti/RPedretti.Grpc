using Windows.ApplicationModel.Resources;

#nullable enable
namespace RPedretti.Grpc.Uwp.Client.Helpers
{
    internal static class ResourceExtensions
    {
        private static readonly ResourceLoader _resLoader = new ResourceLoader();

        public static string GetLocalized(this string resourceKey)
        {
            return _resLoader.GetString(resourceKey);
        }
    }
}
#nullable restore
