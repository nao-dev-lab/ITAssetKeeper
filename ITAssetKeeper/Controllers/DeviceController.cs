using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Device;
using ITAssetKeeper.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITAssetKeeper.Controllers;

public class DeviceController : Controller
{
    private readonly IDeviceService _deviceService;
    private readonly ILogger<DeviceController> _logger;
    public DeviceController(IDeviceService deviceService, ILogger<DeviceController> logger)
    {
        _deviceService = deviceService;
        _logger = logger;
    }

    // GET: Device/Index
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index(DeviceListViewModel model)
    {
        _logger.LogInformation("Index(GET) 開始");

        // 検索メソッド実行、結果を取得
        var vm = await _deviceService.SearchDevicesAsync(model, User);
        _logger.LogInformation("Index(GET) 検索完了 件数={Count}", vm.Devices?.Count);

        // 詳細検索で何か値が入れられたかをチェックし、プロパティにセット
        vm.IsSearchExecuted = vm.HasAnyFilter;
        _logger.LogInformation("Index(GET) 詳細検索実行フラグ IsSearchExecuted={IsSearchExecuted}", vm.IsSearchExecuted);

        // 検索結果が0件の場合、メッセージをセット
        if (vm.Devices == null || vm.Devices.Count == 0)
        {
            _logger.LogInformation("Index(GET) 検索結果0件");
            TempData["ErrorMessage"] = "該当する機器が見つかりませんでした。検索条件を変更して再度お試しください。";
        }

        // ビューに渡す
        _logger.LogInformation("Index(GET) 終了");
        return View(vm);
    }

    // GET: Device/GetSortedList
    // JSから呼び出す
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetSortedList(DeviceColumns sortKey, SortOrders sortOrder, DeviceListViewModel model)
    {
        _logger.LogInformation("GetSortedList 開始 SortKey={Key}, SortOrder={Order}", sortKey, sortOrder);

        // SortKey と SortOrderをモデルに反映させる
        model.SortKeyValue = sortKey;
        model.SortOrderValue = sortOrder;

        // 結果を取得
        var vm = await _deviceService.SearchDevicesAsync(model, User);
        _logger.LogInformation("GetSortedList 完了 件数={Count}", vm.Devices?.Count);

        // 部分ビューに渡す
        return PartialView("_DeviceListPartial", vm);
    }

    // GET: Device/GetPagedList
    // JSから呼び出す
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPagedList(DeviceListViewModel model)
    {
        _logger.LogInformation("GetPagedList 開始 Page={Page}, Size={Size}", model.PageNumber, model.PageSize);

        // 結果を取得
        var vm = await _deviceService.SearchDevicesAsync(model, User);
        _logger.LogInformation("GetPagedList 完了 件数={Count}", vm.Devices?.Count);

        // 部分ビューに渡す
        return PartialView("_DeviceListPartial", vm);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        _logger.LogInformation("Create(GET) 表示開始");

        // ドロップダウン用の項目を詰めて、ビューに渡す
        var model = new DeviceCreateViewModel();
        _deviceService.InitializeCreateView(model);

        _logger.LogInformation("Create(GET) 表示完了");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(DeviceCreateViewModel model)
    {
        _logger.LogInformation("Create(POST) 開始");

        // モデルが null なら一覧に戻す
        if (model == null)
        {
            _logger.LogWarning("Create(POST) モデルが null");
            TempData["ErrorMessage"] = "不正なデータが指定されました。";
            return RedirectToAction(nameof(Index));
        }

        // 入力エラーがあった場合はビューに戻す
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Create(POST) ModelState エラー");
            model = _deviceService.InitializeCreateView(model);
            return View(model);
        }

        // ログインユーザー名を取得
        string userName = User.Identity.Name;

        // ユーザー情報が取得できなかった場合、機器登録画面に戻す
        if (string.IsNullOrEmpty(userName))
        {
            _logger.LogWarning("Create(POST) ユーザー情報取得失敗");
            TempData["ErrorMessage"] = "認証情報の取得に失敗しました。";
            return RedirectToAction(nameof(Create));
        }

        // クエリ実行結果の状態のエントリ数を取得
        var result = await _deviceService.RegisterNewDeviceAsync(model, userName);

        _logger.LogInformation("Create(POST) 登録結果 件数={Count}", result);

        if (result > 0)
        {
            _logger.LogInformation("Create(POST) 登録成功");
            TempData["ToastMessage"] = "登録が完了しました。";
        }
        else
        {
            _logger.LogWarning("Create(POST) 登録失敗");
            TempData["FailureMessage"] = "登録に失敗しました。もう一度お試しください。";
        }

        _logger.LogInformation("Create(POST) 終了");

        // 機器登録画面に戻す
        return RedirectToAction(nameof(Create));
    }


    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        _logger.LogInformation("Details 開始 Id={Id}", id);

        // Id が不正なら一覧に戻す
        if (id <= 0)
        {
            _logger.LogWarning("Details 不正なパラメータ Id={Id}", id);
            TempData["ErrorMessage"] = "不正なパラメータが指定されました。";
            return RedirectToAction(nameof(Index));
        }

        // 対象の機器データを取得
        var device = await _deviceService.GetDeviceDetailsByIdAsync(id);
        _logger.LogInformation("Details 取得結果 Device={Device}", device != null ? "Found" : "NotFound");

        // 取得できなければ一覧に戻す
        if (device == null)
        {
            _logger.LogWarning("Details 対象の機器が見つからない Id={Id}", id);
            TempData["ErrorMessage"] = "対象の機器が見つかりません。";
            return RedirectToAction(nameof(Index));
        }

        // ビューに機器情報のDTOを渡す
        _logger.LogInformation("Details 完了 Id={Id}", id);
        return View(device);
    }


    [HttpGet]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<IActionResult> Edit(int id)
    {
        _logger.LogInformation("Edit(GET) 開始 Id={Id}", id);

        // Id が不正なら一覧に戻す
        if (id <= 0)
        {
            _logger.LogWarning("Edit(GET) 不正なパラメータ Id={Id}", id);
            TempData["ErrorMessage"] = "不正なパラメータが指定されました。";
            return RedirectToAction(nameof(Index));
        }

        // ユーザーのRole情報を取得
        Roles role;
        if (User.IsInRole("Admin"))
        {
            role = Roles.Admin;
        }
        else
        {
            role = Roles.Editor;
        }
        _logger.LogInformation("Edit(GET) ユーザーロール Role={Role}", role);

        // 対象の機器データをビューモデルで取得
        var model = await _deviceService.InitializeEditView(id, role);
        _logger.LogInformation("Edit(GET) 取得結果 Model={Model}", model != null ? "Found" : "NotFound");

        // 取得できなければ一覧に戻す
        if (model == null)
        {
            _logger.LogWarning("Edit(GET) 対象の機器が見つからない Id={Id}", id);
            TempData["ErrorMessage"] = "対象の機器が見つかりません。";
            return RedirectToAction(nameof(Index));
        }

        // ビューにDTOを渡す
        _logger.LogInformation("Edit(GET) 完了 Id={Id}", id);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<IActionResult> Edit(DeviceEditViewModel model)
    {
        _logger.LogInformation("Edit(POST) 開始 Id={Id}", model.IdHidden);

        // モデルが null なら一覧に戻す
        if (model == null)
        {
            _logger.LogWarning("Edit(POST) モデルが null");
            TempData["ErrorMessage"] = "不正なデータが指定されました。";
            return RedirectToAction(nameof(Index));
        }

        // Id が不正なら一覧に戻す
        if (model.IdHidden <= 0)
        {
            _logger.LogWarning("Edit(POST) 不正なパラメータ Id={Id}", model.IdHidden);
            TempData["ErrorMessage"] = "不正なパラメータが指定されました。";
            return RedirectToAction(nameof(Index));
        }

        // ユーザーのRole情報を取得
        Roles role;
        if (User.IsInRole("Admin"))
        {
            role = Roles.Admin;
        }
        else
        {
            role = Roles.Editor;
        }
        _logger.LogInformation("Edit(POST) ユーザーロール Role={Role}", role);

        // 入力エラーがあった場合は、
        // SelectList ＆ ReadOnly 制御を再設定し、ビューに戻す
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Edit(POST) ModelState エラー");
            model = await _deviceService.RestoreEditViewSettingsAsync(model, role);

            // モデルが null なら一覧に戻す
            if (model == null)
            {
                _logger.LogWarning("Edit(POST) モデルが null");
                TempData["ErrorMessage"] = "不正なデータが指定されました。";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // 変更された項目があるかチェック
        var isChanged = await _deviceService.HasDeviceChangedAsync(model);
        _logger.LogInformation("Edit(POST) 変更チェック IsChanged={IsChanged}", isChanged);

        // 変更がない場合は、該当の機器情報画面に戻す
        if (!isChanged)
        {
            _logger.LogInformation("Edit(POST) 変更なし Id={Id}", model.IdHidden);
            await _deviceService.RestoreEditViewSettingsAsync(model, role);
            TempData["ErrorMessage"] = "変更された項目がありません。";
            return View(model);
        }

        // ログインユーザー名を取得
        string userName = User.Identity.Name;

        // ユーザー情報が取得できなかった場合、機器登録画面に戻す
        if (string.IsNullOrEmpty(userName))
        {
            _logger.LogWarning("Edit(POST) ユーザー情報取得失敗");
            TempData["ErrorMessage"] = "認証情報の取得に失敗しました。";
            return RedirectToAction(nameof(Index));
        }

        // クエリ実行結果の状態のエントリ数を取得
        var result = await _deviceService.UpdateDeviceAsync(model, role, userName);
        _logger.LogInformation("Edit(POST) 更新結果 件数={Count}", result);

        if (result == -1)
        {
            _logger.LogWarning("Edit(POST) 対象の機器が見つからない Id={Id}", model.IdHidden);
            TempData["ErrorMessage"] = "対象の機器が見つかりませんでした。もう一度お試しください。";
        }
        else if (result > 0)
        {
            _logger.LogInformation("Edit(POST) 更新成功 Id={Id}", model.IdHidden);
            TempData["ToastMessage"] = "更新が完了しました。";
        }
        else
        {
            _logger.LogWarning("Edit(POST) 更新失敗 Id={Id}", model.IdHidden);
            TempData["FailureMessage"] = "更新に失敗しました。もう一度お試しください。";
        }

        // 該当の機器情報画面に戻す
        _logger.LogInformation("Edit(POST) 完了 Id={Id}", model.IdHidden);
        return RedirectToAction(nameof(Details), new { id = model.IdHidden });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        _logger.LogInformation("DeleteConfirmed 開始 Id={Id}", id);
        
        // Id が不正なら中断
        if (id <= 0)
        {
            _logger.LogWarning("DeleteConfirmed 不正なパラメータ Id={Id}", id);
            return Json(new { success = false, message = "不正なパラメータが指定されました。" });
        }

        // ログインユーザー名を取得
        string deleteByuserName = User.Identity.Name;

        // ユーザー情報が取得できなかった場合
        if (string.IsNullOrEmpty(deleteByuserName))
        {
            _logger.LogWarning("DeleteConfirmed ユーザー情報取得失敗");
            return Json(new { success = false, message = "認証情報が取得できませんでした。" });
        }

        // 論理削除処理を実施
        var result = await _deviceService.DeleteDeviceAsync(id, deleteByuserName);
        _logger.LogInformation("DeleteConfirmed 削除結果 件数={Count}", result);

        // 失敗判定
        if (result <= 0)
        {
            // アクションの戻り値としてJsonResultを返す
            if (result == -1)
            {
                _logger.LogWarning("DeleteConfirmed 対象の機器が見つからない Id={Id}", id);
                return Json(new { success = false, message = "対象の機器が見つかりません。" });
            }
            else
            {
                _logger.LogWarning("DeleteConfirmed 削除失敗 Id={Id}", id);
                return Json(new { success = false, message = "削除に失敗しました。" });
            }
        }

        // 遷移先(Index) で Toast 再表示できるよう保存
        TempData["ToastMessage"] = "削除が完了しました。";

        // 成功
        _logger.LogInformation("DeleteConfirmed 削除成功 Id={Id}", id);
        return Json(new { success = true, message = "削除が完了しました。" });
    }
}
