using ITAssetKeeper.Batch;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    // Functions Worker のデフォルト設定を使用
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        // DI コンテナに SnapshotSyncService を登録
        services.AddSingleton<ISnapshotSyncService, SnapshotSyncService>();
    })
    .Build();

host.Run();
