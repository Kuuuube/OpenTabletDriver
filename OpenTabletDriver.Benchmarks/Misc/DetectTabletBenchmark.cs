using BenchmarkDotNet.Attributes;
using OpenTabletDriver.Daemon;
using Microsoft.Extensions.DependencyInjection;
using OpenTabletDriver.Desktop;
using OpenTabletDriver.Plugin.Components;
using System.Threading.Tasks;
using BenchmarkDotNet.Engines;

namespace OpenTabletDriver.Benchmarks.Misc
{
    public class DetectTabletBenchmark
    {
        private DriverDaemon driverDaemon = new DriverDaemon(new DriverBuilder()
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddSingleton<IDeviceConfigurationProvider, DesktopDeviceConfigurationProvider>();
                    serviceCollection.AddSingleton<IReportParserProvider, DesktopReportParserProvider>();
                })
                .Build<Driver>(out _)
            );
        private readonly Consumer consumer = new Consumer();

        [Benchmark]
        public async Task<bool> DetectTabletsAsync()
        {
            (await driverDaemon.DetectTablets()).Consume(consumer);
            return true;
        }
    }
}
