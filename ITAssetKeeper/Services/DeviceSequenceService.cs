using ITAssetKeeper.Data;
using Microsoft.EntityFrameworkCore;

namespace ITAssetKeeper.Services;

public class DeviceSequenceService : IDeviceSequenceService
{
    private readonly ITAssetKeeperDbContext _context;
    private readonly ILogger<DeviceSequenceService> _logger;

    public DeviceSequenceService(
        ITAssetKeeperDbContext context,
        ILogger<DeviceSequenceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // 機器管理IDの採番を行うメソッド
    public async Task<int> GetNextManagementIdAsync()
    {
        _logger.LogInformation("GetNextManagementIdAsync 開始");
        try
        {
            _logger.LogInformation("GetNextManagementIdAsync UPDATE 実行開始");
            // UPDATE 文を実行
            // Id=1 のレコードの LastUsedNumber を 1 増加させる
            await _context.Database.ExecuteSqlRawAsync(@"
                UPDATE DeviceSequences
                SET LastUsedNumber = LastUsedNumber + 1
                WHERE Id = 1;
                ");
            _logger.LogInformation("GetNextManagementIdAsync UPDATE 実行成功");

            // UPDATE では値を返さないので、別途 SELECT で取得する
            // Id=1 のレコードを対象にLastUsedNumber カラムだけを取り出す
            var next = await _context.DeviceSequences
                .Where(x => x.Id == 1)
                .Select(x => x.LastUsedNumber)
                .SingleAsync();
            _logger.LogInformation("GetNextManagementIdAsync SELECT 実行成功 NextManagementId={NextManagementId}", next);

            // 更新後の LastUsedNumber を返す
            // 呼び出すたびに 1, 2, 3, ... と順番に取得できる
            _logger.LogInformation("GetNextManagementIdAsync 終了");
            return next;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetNextManagementIdAsync 例外発生");

            // 例外が発生した場合は、採番処理に失敗したことを示す例外をスローする
            // 採番が失敗したら継続不可能なので包んで投げる
            throw new InvalidOperationException(
                "ManagementId の採番処理に失敗しました。" +
                "（DeviceSequences テーブルの更新に問題が発生した可能性があります）",
                ex
            );
        }
    }
}
