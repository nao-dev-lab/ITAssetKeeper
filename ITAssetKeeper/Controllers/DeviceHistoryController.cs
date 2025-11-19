using ITAssetKeeper.Data;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Device;
using ITAssetKeeper.Models.ViewModels.DeviceHistory;
using ITAssetKeeper.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITAssetKeeper.Controllers;

public class DeviceHistoryController : Controller
{
    private readonly IDeviceHistoryService _deviceHistoryService;

    public DeviceHistoryController(IDeviceHistoryService deviceHistoryService)
    {
        _deviceHistoryService = deviceHistoryService;
    }

    // GET: DeviceHistory/Index
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index(DeviceHistoryViewModel model)
    {
        // 検索メソッド実行、結果を取得
        var vm = await _deviceHistoryService.SearchHistoriesAsync(model);

        // ビューに渡す
        return View(vm);
    }

    // GET: DeviceHistory/GetSortedList
    // JSから呼び出す
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetSortedList(DeviceHistoryColumns sortKey, SortOrders sortOrder, DeviceHistoryViewModel model)
    {
        // SortKey と SortOrderをモデルに反映させる
        model.SortKeyValue = sortKey;
        model.SortOrderValue = sortOrder;

        // 結果を取得
        var vm = await _deviceHistoryService.SearchHistoriesAsync(model);

        // 部分ビューに渡す
        return PartialView("_DeviceHistoryListPartial", vm);
    }

    // GET: DeviceHistory/GetPagedList
    // JSから呼び出す
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPagedList(DeviceHistoryViewModel model)
    {
        // 結果を取得
        var vm = await _deviceHistoryService.SearchHistoriesAsync(model);

        // 部分ビューに渡す
        return PartialView("_DeviceHistoryListPartial", vm);
    }


}
