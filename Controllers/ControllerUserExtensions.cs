using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ApiFinanceiro.Controllers
{
    internal static class ControllerUserExtensions
    {
        public static int? GetUsuarioId(this ControllerBase controller)
        {
            var value = controller.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : null;
        }
    }
}
