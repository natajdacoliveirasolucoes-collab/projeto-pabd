using Microsoft.AspNetCore.Mvc;

namespace ApiFinanceiro.Exceptions
{
    public class ErrorServiceException : Exception
    {
        private readonly Func<ControllerBase, IActionResult> _result;

        public ErrorServiceException(string message, Func<ControllerBase, IActionResult> result) : base(message) 
        {
            _result = result;
        }

        public IActionResult ToActionResult(ControllerBase controller)
        {
            return _result(controller);
        }
    }
}
