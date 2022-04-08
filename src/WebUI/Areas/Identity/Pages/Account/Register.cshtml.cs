using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using talker.Application.Common.Interfaces;
using talker.Application.Messages;
using talker.Application.Updates;
using talker.Application.Updates.Queries.GetUpdates;
using talker.Domain.Entities;
using talker.Infrastructure.Identity;
using talker.WebUI.Hubs;

namespace talker.WebUI.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IApplicationDbContext _context;
        private readonly IHubContext<UpdateHub> _hubContext;

        public RegisterModel(
            SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager, 
            ILogger<RegisterModel> logger, 
            IEmailSender emailSender, 
            IApplicationDbContext context, 
            IHubContext<UpdateHub> hubContext)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
            _hubContext = hubContext;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var pictureUrl = "https://www.gravatar.com/avatar/";
                pictureUrl += string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(Input.Email)).Select(s => s.ToString("x2")));
                pictureUrl += "?d=retro&s=200";

                var user = new ApplicationUser
                {
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    UserName = Input.Email,
                    Email = Input.Email,
                    PictureUrl = pictureUrl
                };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _context.UsersConversations.Add(new UserConversation
                    {
                        ConversationId = 1,
                        UserId = user.Id,
                        Role = Domain.Enums.ConversationRole.User
                    });

                    var affectedUser = $"{user.FirstName} {user.LastName}";

                    var messageEntity = new Message
                    {
                        ConversationId = 1,
                        Content = $"added {affectedUser} to the conversation",
                        Type = Domain.Enums.MessageType.AddedUserToConversation
                    };

                    _context.Messages.Add(messageEntity);

                    var adminId = (await _userManager.FindByEmailAsync("a@a.pl")).Id;
                    await _context.SaveChangesAsUserAsync(adminId, CancellationToken.None);

                    await _hubContext.Clients.Groups($"h-{1}").SendAsync("OnConversationUpdate", new ConversationUpdateDto
                    {
                        Id = 1,
                        Type = ConversationUpdateType.UserAddedToConversation,
                        UserId = user.Id
                    });

                    var update = new MessageUpdateDto()
                    {
                        Messages = new List<MessageDto>()
                        {
                            new MessageDto()
                            {
                                Id = messageEntity.Id,
                                ConversationId = 1,
                                Content = messageEntity.Content,
                                Created = TimeZoneInfo.ConvertTimeToUtc(DateTime.Now),
                                CreatedBy = adminId,
                                Type = Domain.Enums.MessageType.AddedUserToConversation,
                            }
                        }
                    };

                    await _hubContext.Clients.Groups($"h-{1}").SendAsync("OnMessageUpdate", update);

                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                     _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"<h2 style=\"font-weight:400;\">Register confirmation</h2><p>Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.</p>");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
