using System.Numerics;
using Newtonsoft.Json;
using OpenTabletDriver.Tablet;

namespace OpenTabletDriver.Desktop.RPC
{
    [JsonObject]
    public class DebugReportData
    {
        [JsonConstructor]
        private DebugReportData()
        {
        }

        public DebugReportData(string deviceName, IDeviceReport report)
        {
            var type = report.GetType();

            DeviceName = deviceName;
            ReportType = type.FullName!;
            Raw = ReportFormatter.GetStringRaw(report.Raw);
            Formatted = ReportFormatter.GetStringFormat(report);

            RawPosition = (report as ITabletReport)?.Position;
        }

        public string DeviceName { set; get; } = string.Empty;
        public string ReportType { set; get; } = string.Empty;
        public string Raw { set; get; } = string.Empty;
        public string Formatted { set; get; } = string.Empty;

        public Vector2? RawPosition { set; get; }
    }
}
