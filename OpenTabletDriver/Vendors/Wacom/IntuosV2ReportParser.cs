using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Tablet;

namespace OpenTabletDriver.Vendors.Wacom
{
    public class IntuosV2ReportParser : IReportParser<IDeviceReport>
    {
        public virtual IDeviceReport Parse(byte[] data)
        {
            return data[1] switch
            {
                0xC2 => new IntuosV2Filter(data),
                _ => new IntuosV2TabletReport(data)
            };
        }
    }
}