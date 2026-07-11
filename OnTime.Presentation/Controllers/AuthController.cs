using System;
using System.Threading.Tasks;

using MediatR;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

using OnTime.Application.Domain;
using OnTime.Application.Features.Auth.Commands;
using OnTime.Site.Constants;
using OnTime.Site.ViewModels;

namespace OnTime.Site.Controllers;

public class AuthController : Controller
{
    private readonly IMediator mediator;

    public AuthController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel { PageTitle = "Registar - On Time" });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return PartialView(ViewNames.RegistrationForm, model);

        var command = new RegisterCommand(
            model.Email,
            model.Password,
            model.PasswordConfirmation,
            model.FirstName,
            model.LastName,
            model.PhoneNumber);

        var result = await this.mediator.Send(command);
        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error!.Message);
            return PartialView(ViewNames.RegistrationForm, model);
        }

        return PartialView(ViewNames.RegisterSuccess, model.Email);
    }

    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel { PageTitle = "Entrar - On Time" });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return PartialView(ViewNames.LoginForm, model);

        var result = await this.mediator.Send(new LoginCommand(model.Email, model.Password));
        if (result.IsFailure)
        {
            ModelState.AddModelError(string.Empty, result.Error!.Message);
            return PartialView(ViewNames.LoginForm, model);
        }

        var status = result.Value;
        if (status != LoginStatus.Success)
        {
            switch (status)
            {
                case LoginStatus.LockedOut:
                    ModelState.AddModelError(string.Empty, "A sua conta está bloqueada.");
                    break;
                case LoginStatus.NotAllowed:
                    ModelState.AddModelError(string.Empty,
                        "A sua conta ainda não foi confirmada. Verifique o seu e-mail ou clique no link abaixo para reenviar.");
                    model.IsResendConfirmationVisible = true;
                    model.UnconfirmedEmail = model.Email;
                    break;
            }

            return PartialView(ViewNames.LoginForm, model);
        }

        return string.IsNullOrWhiteSpace(returnUrl)
            ? Redirect("/")
            : LocalRedirect(returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(string? returnUrl = null)
    {
        await this.mediator.Send(new LogoutCommand());
        return string.IsNullOrWhiteSpace(returnUrl)
            ? Redirect("/")
            : LocalRedirect(returnUrl);
    }

    [HttpGet]
    public IActionResult GoogleLogin(string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(GoogleCallback), "Auth", new { returnUrl });
        var properties = new AuthenticationProperties
        {
            RedirectUri = redirectUrl,
            Items = { ["LoginProvider"] = ExternalProviderNames.Google }
        };
        return Challenge(properties, ExternalProviderNames.Google);
    }

    [HttpGet]
    public async Task<IActionResult> GoogleCallback(string? returnUrl = null, string? remoteError = null)
    {
        var result = await this.mediator.Send(new GoogleCallbackCommand(returnUrl, remoteError));
        if (result.IsSuccess)
        {
            return LocalRedirect(result.Value!);
        }

        ModelState.AddModelError(string.Empty, result.Error!.Message);
        return View(ViewNames.Login, new LoginViewModel { PageTitle = "Entrar - On Time" });
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string code)
    {
        if (userId == null || code == null)
        {
            return RedirectToAction(nameof(Login));
        }

        var result = await this.mediator.Send(new ConfirmEmailCommand(userId, code));
        if (result.IsSuccess)
        {
            var model = new ConfirmEmailViewModel
            {
                PageTitle = "Confirmar E-mail - On Time",
                IsSucceeded = true,
                Email = result.Value!
            };
            return View(model);
        }

        return NotFound(result.Error!.Message);
    }

    [HttpGet]
    public IActionResult ForgotPassword() =>
        View(new ForgotPasswordViewModel { PageTitle = "Recuperar Palavra-passe - On Time" });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return PartialView(ViewNames.ForgotPasswordForm, model);

        var result = await this.mediator.Send(new ForgotPasswordCommand(model.Email));

        if (result.IsFailure)
        {
            if (result.Error!.Code == "ForgotPasswordFailed")
            {
                return PartialView(ViewNames.ForgotPasswordFailed);
            }

            ModelState.AddModelError(string.Empty, result.Error.Message);
            return PartialView(ViewNames.ForgotPasswordForm, model);
        }

        return PartialView(ViewNames.ForgotPasswordSuccess);
    }

    [HttpGet]
    public IActionResult ResetPassword(string? code = null)
    {
        return code == null
            ? BadRequest("É necessário um código para repor a palavra-passe.")
            : View(new ResetPasswordViewModel { PageTitle = "Repor Palavra-passe - On Time", Code = code });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return PartialView(ViewNames.ResetPasswordForm, model);

        var result = await this.mediator.Send(new ResetPasswordCommand(model.Email, model.Code, model.Password));
        if (result.IsSuccess)
        {
            return PartialView(ViewNames.ResetPasswordSuccess);
        }

        ModelState.AddModelError(string.Empty, result.Error!.Message);
        return PartialView(ViewNames.ResetPasswordForm, model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendEmailConfirmation(string email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return BadRequest("E-mail inválido.");
        }

        var result = await this.mediator.Send(new ResendEmailConfirmationCommand(email));

        if (result.IsSuccess)
        {
            return PartialView(ViewNames.ResendEmailConfirmationSuccess);
        }

        ModelState.AddModelError(string.Empty, result.Error!.Message);
        return PartialView(ViewNames.ResendEmailConfirmationError);
    }

    [HttpGet]
    public IActionResult AccessDenied() =>
        View(ViewNames.AccessDenied, new BaseViewModel { PageTitle = "Acesso Recusado - On Time" });
}