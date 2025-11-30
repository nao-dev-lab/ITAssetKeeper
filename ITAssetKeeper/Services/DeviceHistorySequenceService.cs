using ITAssetKeeper.Data;
using Microsoft.EntityFrameworkCore;

namespace ITAssetKeeper.Services;

// HistoryId自動採番用
public class DeviceHistorySequenceService : IDeviceHistorySequenceService
{
    private readonly ITAssetKeeperDbContext _context;
    private readonly ILogger<DeviceHistorySequenceService> _logger;

    public DeviceHistorySequenceService(
        ITAssetKeeperDbContext context,
        ILogger<DeviceHistorySequenceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // 履歴IDの採番を行うメソッド
    public async Task<int> GetNextHistoryIdAsync()
    {
        _logger.LogInformation("GetNextHistoryIdAsync 開始");
        try
        {
            _logger.LogInformation("GetNextHistoryIdAsync DeviceHistorySequences テーブル更新開始");

            // UPDATE 文を実行
            // Id=1 のレコードの LastUsedNumber を 1 増加させる
            await _context.Database.ExecuteSqlRawAsync(@"
                UPDATE DeviceHistorySequences
                SET LastUsedNumber = LastUsedNumber + 1
                WHERE Id = 1;
                ");
            _logger.LogInformation("GetNextHistoryIdAsync DeviceHistorySequences テーブル更新成功");

            // UPDATE では値を返さないので、別途 SELECT で取得する
            // Id=1 のレコードを対象にLastUsedNumber カラムだけを取り出す
            var next = await _context.DeviceHistorySequences
                .Where(x => x.Id == 1)
                .Select(x => x.LastUsedNumber)
                .SingleAsync();
            _logger.LogInformation("GetNextHistoryIdAsync 更新後の LastUsedNumber 取得成功 LastUsedNumber={LastUsedNumber}", next);

            // 更新後の LastUsedNumber を返す
            // 呼び出すたびに 1, 2, 3, ... と順番に取得できる
            _logger.LogInformation("GetNextHistoryIdAsync 終了");
            return next;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetNextHistoryIdAsync 例外発生");

            // 例外が発生した場合は、採番処理に失敗したことを示す例外をスローする
            // 採番が失敗したら継続不可能なので包んで投げる
            throw new InvalidOperationException(
                "HistoryId の採番処理に失敗しました。" +
                "（DeviceHistorySequences テーブルの更新に問題が発生した可能性があります）",
                ex
            );
        }
    }
}
