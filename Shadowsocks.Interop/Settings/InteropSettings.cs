namespace Shadowsocks.Interop.Settings
{
    public class InteropSettings
    {
        public string SsRustPath { get; set; }
        public string V2RayCorePath { get; set; }

        public InteropSettings()
        {
            SsRustPath = "";
            V2RayCorePath = "";
        }
    }
}
