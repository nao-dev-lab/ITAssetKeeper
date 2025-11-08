using ITAssetKeeper.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ITAssetKeeper.Controllers
{
    // Login用コントローラー
    public class LoginController : Controller
    {
        /*// フィールド
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        // コンストラクタ
        public LoginController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) 
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }*/

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login()
        {
            return View();
        }*/
    }
}
