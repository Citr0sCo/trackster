using HomeBoxLanding.Api.Core.Shell;
using HomeBoxLanding.Api.Features.Stats;
using HomeBoxLanding.Api.Features.Stats.Types;
using Moq;
using NUnit.Framework;

namespace HomeBoxLanding.Api.Tests.Features.Stats.GivenARequestToGetStats;

[TestFixture]
public class WhenCpuUsageIsRequested
{
    private StatsResponse _result;

    [SetUp]
    public void SetUp()
    {
        var shellService = new Mock<IShellService>();
        shellService.Setup(x => x.RunOnHost("docker stats --no-stream")).Returns("CONTAINER ID   NAME                  CPU %     MEM USAGE / LIMIT     MEM %     NET I/O           BLOCK I/O         PIDS\ncadfaabdcc30   home-app              0.01%     86.6MiB / 7.764GiB    1.09%     398kB / 1.35MB    2.47MB / 0B       21\n6f3c77afa92f   unifi-poller          0.01%     7.898MiB / 7.764GiB   0.10%     17.8MB / 27.6MB   0B / 0B           6\nd9f2bfe0c08e   trilium               0.02%     60.61MiB / 7.764GiB   0.76%     804kB / 648kB     2.54MB / 41.2MB   11\n44bdccccdcdb   portainer             0.00%     39.35MiB / 7.764GiB   0.49%     11.7MB / 64.9MB   87.5MB / 178MB    7\nd061d4d944a6   uptime-kuma           0.22%     389.9MiB / 7.764GiB   4.90%     17.5MB / 28.8MB   565MB / 8.91GB    12\nd7da6786b226   flaresolverr          0.01%     296.8MiB / 7.764GiB   3.73%     21.7MB / 1.91MB   638MB / 131MB     7\n2931f709694f   flood                 0.13%     68.12MiB / 7.764GiB   0.86%     135MB / 75.6MB    143MB / 383MB     12\n301ea8d7a4f0   xteve                 0.00%     270.8MiB / 7.764GiB   3.41%     40.2MB / 178MB    45.5MB / 469MB    11\n41ef7050fede   firefly-iii           0.00%     193.8MiB / 7.764GiB   2.44%     10.9MB / 14.5MB   246MB / 21MB      11\n0b84dbcbdde0   matomo                0.00%     341.6MiB / 7.764GiB   4.30%     30.7MB / 31.2MB   305MB / 30.4MB    11\n75fd7f67166f   mariadb               0.01%     133.3MiB / 7.764GiB   1.68%     27.2MB / 29.4MB   111MB / 26.7MB    7\n22d5e2263e94   influxdb2             0.05%     339.4MiB / 7.764GiB   4.27%     940MB / 102MB     327MB / 2.9GB     13\nc4cbdf796ffe   telegraf              0.06%     55.53MiB / 7.764GiB   0.70%     3.06MB / 9.57MB   205MB / 0B        9\n0f302162f39e   planka                0.00%     139.6MiB / 7.764GiB   1.76%     304kB / 2.15MB    190MB / 0B        12\n49e870621fc7   postgres              0.00%     55.31MiB / 7.764GiB   0.70%     10.4MB / 13.5MB   96.9MB / 23.6MB   8\na76a5bd8cfe7   varken                0.01%     22.01MiB / 7.764GiB   0.28%     39.7MB / 19MB     3.51MB / 91MB     1\na2ef74a3d2ff   tautulli              0.02%     107.1MiB / 7.764GiB   1.35%     118MB / 53.2MB    81MB / 536MB      23\n20cb3401ddb2   grafana               0.01%     128.9MiB / 7.764GiB   1.62%     55.4MB / 110MB    285MB / 64.1MB    15\na24af6eb946f   overseerr             0.01%     254.6MiB / 7.764GiB   3.20%     25.1MB / 51.5MB   263MB / 87.6MB    23\n590dbe58c857   authelia              0.02%     35.48MiB / 7.764GiB   0.45%     365kB / 6.7MB     92.3MB / 188kB    9\ncf5f5da323af   mealie                0.10%     207.2MiB / 7.764GiB   2.61%     1.61MB / 3.28MB   228MB / 16.5MB    15\ndfe45832a10f   nginx-proxy-manager   0.01%     178.9MiB / 7.764GiB   2.25%     125MB / 179MB     227MB / 168MB     20");
            
        var subject = new StatsService(shellService.Object, new FakeStatsServiceCache());
        _result = subject.GetServerStats();
    }
        
    [Test]
    public void ThenMemoryUsageIsReturnedCorrectly()
    {
        Assert.That(_result.Stats.First(x => x.Name == "home-app").CpuUsage.Percentage, Is.EqualTo(0.01));
    }
    
}