namespace RotaLivreMobile.Helpers
{
    public static class AppConfig
    {
        public static string BaseUrl => 
        DeviceInfo.Platform == DevicePlatform.Android 
        ? "http://10.0.2.2:7015/api/" 
        : "http://localhost:7015/api/";

        public static string ServerUrl => BaseUrl.Replace("api/", "");
    }
}