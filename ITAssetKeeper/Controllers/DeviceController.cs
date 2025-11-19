using ITAssetKeeper.Data;
using ITAssetKeeper.Models.Enums;
using ITAssetKeeper.Models.ViewModels.Device;
using ITAssetKeeper.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITAssetKeeper.Controllers;

public class DeviceController : Controller
{
    private readonly IDeviceService _deviceService;

    public DeviceController(IDeviceService deviceService)
    {
        _deviceService = deviceService;
    }

    // GET: Device/Index
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Index(DeviceListViewModel model)
    {
        // 検索メソッド実行、結果を取得
        var vm = await _deviceService.SearchDevicesAsync(model);

        // ビューに渡す
        return View(vm);
    }

    // GET: Device/GetSortedList
    // JSから呼び出す
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetSortedList(DeviceColumns sortKey, SortOrders sortOrder, DeviceListViewModel model)
    {
        // SortKey と SortOrderをモデルに反映させる
        model.SortKeyValue = sortKey;
        model.SortOrderValue = sortOrder;

        // 結果を取得
        var vm = await _deviceService.SearchDevicesAsync(model);

        // 部分ビューに渡す
        return PartialView("_DeviceListPartial", vm);
    }

    // GET: Device/GetPagedList
    // JSから呼び出す
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetPagedList(DeviceListViewModel model)
    {
        // 結果を取得
        var vm = await _deviceService.SearchDevicesAsync(model);

        // 部分ビューに渡す
        return PartialView("_DeviceListPartial", vm);
    }

    [HttpGet]
    public IActionResult Cancel(string returnUrl)
    {
        // キャンセルボタンが押された時の処理
        // まだ作成していない
        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        // ドロップダウン用の項目を詰めて、ビューに渡す
        var model = new DeviceCreateViewModel();
        _deviceService.InitializeCreateView(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(DeviceCreateViewModel model)
    {
        // 入力エラーがあった場合はビューに戻す
        if (!ModelState.IsValid)
        {
            model = _deviceService.InitializeCreateView(model);
            return View(model);
        }

        // ログインユーザー名を取得
        string userName = User.Identity.Name;

        // ユーザー情報が取得できなかった場合、機器登録画面に戻す
        if (string.IsNullOrEmpty(userName))
        {
            TempData["ErrorMessage"] = "認証情報の取得に失敗しました。";
            return RedirectToAction(nameof(Create));
        }

        // クエリ実行結果の状態のエントリ数を取得
        var result = await _deviceService.RegisterNewDeviceAsync(model, userName);

        if (result > 0)
        {
            TempData["ToastMessage"] = "登録が完了しました。";
        }
        else
        {
            TempData["FailureMessage"] = "登録に失敗しました。もう一度お試しください。";
        }

        // 機器登録画面に戻す
        return RedirectToAction(nameof(Create));
    }


    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Details(int id)
    {
        // 対象の機器データを取得
        var dto = await _deviceService.GetDeviceDetailsByIdAsync(id);

        // 取得できなければ一覧に戻す
        if (dto == null)
        {
            TempData["ErrorMessage"] = "対象の機器が見つかりません。";
            return RedirectToAction(nameof(Index));
        }

        // ビューにDTOを渡す
        return View(dto);
    }


    [HttpGet]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<IActionResult> Edit(int id)
    {
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

        // 対象の機器データをビューモデルで取得
        var model = await _deviceService.InitializeEditView(id, role);

        // 取得できなければ一覧に戻す
        if (model == null)
        {
            TempData["ErrorMessage"] = "対象の機器が見つかりません。";
            return RedirectToAction(nameof(Index));
        }

        // ビューにDTOを渡す
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin, Editor")]
    public async Task<IActionResult> Edit(DeviceEditViewModel model)
    {
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

        // 入力エラーがあった場合は、
        // SelectList ＆ ReadOnly 制御だけ再設定し、ビューに戻す
        if (!ModelState.IsValid)
        {
            await _deviceService.RestoreEditViewSettingsAsync(model, role);

            return View(model);
        }

        // ログインユーザー名を取得
        string userName = User.Identity.Name;

        // ユーザー情報が取得できなかった場合、機器登録画面に戻す
        if (string.IsNullOrEmpty(userName))
        {
            TempData["ErrorMessage"] = "認証情報の取得に失敗しました。";
            return RedirectToAction(nameof(Create));
        }

        // クエリ実行結果の状態のエントリ数を取得
        var result = await _deviceService.UpdateDeviceAsync(model, role, userName);

        if (result == -1)
        {
            TempData["ErrorMessage"] = "対象の機器が見つかりませんでした。もう一度お試しください。";
        }
        else if (result > 0)
        {
            //TempData["SuccessMessage"] = "更新が完了しました。";
            TempData["ToastMessage"] = "更新が完了しました。";
        }
        else
        {
            TempData["FailureMessage"] = "更新に失敗しました。もう一度お試しください。";
        }

        // 該当の機器情報画面に戻す
        return RedirectToAction(nameof(Details), new { id = model.IdHidden });
    }


    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        // 対象の機器データを取得
        var model = await _deviceService.GetDeleteDeviceByIdAsync(id);

        // 取得できなければ一覧に戻す
        if (model == null)
        {
            TempData["ErrorMessage"] = "対象の機器が見つかりません。";
            return RedirectToAction(nameof(Index));
        }

        // ビューにモデルを渡す
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // Id が不正なら中断
        if (id <= 0)
        {
            return Json(new { success = false, message = "削除対象のIDが不正です。" });
        }

        // ログインユーザー名を取得
        string deleteByuserName = User.Identity.Name;

        // ユーザー情報が取得できなかった場合
        if (string.IsNullOrEmpty(deleteByuserName))
        {
            return Json(new { success = false, message = "認証情報が取得できませんでした。" });
        }

        // 論理削除処理を実施
        var result = await _deviceService.DeleteDeviceAsync(id, deleteByuserName);

        // 失敗判定
        if (result <= 0)
        {
            if (result == -1)
            {
                return Json(new { success = false, message = "対象の機器が見つかりませんでした。" });
            }
            else
            {
                return Json(new { success = false, message = "削除に失敗しました。" });
            }
        }

        // 遷移先(Index) で Toast 再表示できるよう保存
        TempData["ToastMessage"] = "削除が完了しました。";

        // 成功
        return Json(new { success = true, message = "削除が完了しました。" });
    }


    // ※廃止※
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //[Authorize(Roles = "Admin")]
    //public async Task<IActionResult> DeleteConfirmed(DeviceDeleteViewModel model)
    //{
    //    // Id が不正なら中断
    //    if (model.IdHidden <= 0)
    //    {
    //        TempData["ErrorMessage"] = "削除対象のIDが不正です。最初からやり直してください。";
    //        return RedirectToAction(nameof(Index));
    //    }

    //    // ログインユーザー名を取得
    //    string deleteByuserName = User.Identity.Name;

    //    // ユーザー情報が取得できなかった場合は機器情報画面に戻す
    //    if (deleteByuserName == null)
    //    {
    //        TempData["FailureMessage"] = "認証情報が取得できませんでした。もう一度お試しください。";
    //        return RedirectToAction(nameof(Details), new { id = model.IdHidden });
    //    }

    //    // 論理削除処理を実施
    //    // クエリ実行結果の状態のエントリ数を取得
    //    var result = await _deviceService.DeleteDeviceAsync(model.IdHidden, deleteByuserName);

    //    // 失敗していたら該当の機器情報画面に戻す
    //    if (result <= 0)
    //    {
    //        if (result == -1)
    //        {
    //            TempData["ErrorMessage"] = "対象の機器が見つかりませんでした。もう一度お試しください。";
    //        }
    //        else
    //        {
    //            TempData["FailureMessage"] = "削除に失敗しました。もう一度お試しください。";
    //        }

    //        return RedirectToAction(nameof(Details), new { id = model.IdHidden });
    //    }

    //    // 成功したら機器一覧に戻す
    //    TempData["SuccessMessage"] = "削除が完了しました。";
    //    return RedirectToAction(nameof(Index));
    //}
}
