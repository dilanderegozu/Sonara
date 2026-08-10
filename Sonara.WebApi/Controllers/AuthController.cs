using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sonara.CoreLayer.Entities;
using Sonara.CoreLayer.Interfaces;
using Sonara.DataAccessLayer.Repositories.Implementations;
using Sonara.DataAccessLayer.Repositories.Interfaces;
using Sonara.DtoLayer.Dtos.AuthDto;

namespace Sonara.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IUserMembershipDal _userMembershipDal;
        private readonly IDeviceSessionDal _deviceSessionDal;
        private readonly IMembershipPlanDal _membershipPlanDal;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ITokenService tokenService, IUserMembershipDal userMembershipDal, IDeviceSessionDal deviceSessionDal, IMembershipPlanDal membershipPlanDal)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _userMembershipDal = userMembershipDal;
            _deviceSessionDal = deviceSessionDal;
            _membershipPlanDal = membershipPlanDal;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                RegisteredAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new AuthResponseDto { Success = false, ErrorMessage = errors });
            }
            var token = await _tokenService.CreateTokenAsync(user, null);
            return Ok(new AuthResponseDto { Success = true, Token = token });

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null)
                return Unauthorized(new AuthResponseDto { Success = false, ErrorMessage = "E-posta veya şifre hatalı." });

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);

            if (result.IsLockedOut)
                return Unauthorized(new AuthResponseDto { Success = false, ErrorMessage = "Çok fazla başarısız deneme. Lütfen 10 dakika sonra tekrar deneyin." });

            if (!result.Succeeded)
                return Unauthorized(new AuthResponseDto { Success = false, ErrorMessage = "E-posta veya şifre hatalı." });

            var activeMembership = await _userMembershipDal.GetActiveMembershipByUserIdAsync(user.Id);

    
            var existingSession = await _deviceSessionDal.GetByDeviceIdentifierAsync(user.Id, dto.DeviceIdentifier);

            if (existingSession is not null)
            {
                // Zaten kayıtlı cihaz, sadece aktiviteyi güncelle
                existingSession.LastActivityDate = DateTime.UtcNow;
                _deviceSessionDal.Update(existingSession);
            }
            else
            {
                // Yeni cihaz — limit kontrolü yapılacak
                int maxDeviceCount;

                if (activeMembership is not null)
                {
                    maxDeviceCount = activeMembership.MembershipPlan.MaxDeviceCount;
                }
                else
                {
                    var freePlan = await _membershipPlanDal.GetByNameAsync("Free");
                    maxDeviceCount = freePlan?.MaxDeviceCount ?? 1;
                }

                var activeSessions = await _deviceSessionDal.GetActiveSessionsByUserIdAsync(user.Id);

                if (activeSessions.Count >= maxDeviceCount)
                {
                    var oldestSession = await _deviceSessionDal.GetOldestSessionAsync(user.Id);
                    if (oldestSession is not null)
                    {
                        _deviceSessionDal.Delete(oldestSession);
                   
                    }
                }

                var newSession = new DeviceSession
                {
                    UserId = user.Id,
                    DeviceIdentifier = dto.DeviceIdentifier,
                    DeviceName = dto.DeviceName,
                    LoginDate = DateTime.UtcNow,
                    LastActivityDate = DateTime.UtcNow
                };

                await _deviceSessionDal.AddAsync(newSession);
            }

            await _deviceSessionDal.SaveChangesAsync();
  

            var token = await _tokenService.CreateTokenAsync(user, activeMembership?.MembershipPlan);

            return Ok(new AuthResponseDto { Success = true, Token = token });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null)
                return Ok(new { Message = "Eğer bu e-posta sistemde kayıtlıysa, şifre sıfırlama bilgisi oluşturuldu." });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // GEÇİCİ: email altyapısı kurulana kadar token'ı response'da döndürüyoruz.
            // Production'da bu satır kaldırılıp token e-posta ile gönderilecek.
            return Ok(new { Message = "Reset token oluşturuldu.", ResetToken = token });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null)
                return BadRequest(new AuthResponseDto { Success = false, ErrorMessage = "İşlem gerçekleştirilemedi." });

            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest(new AuthResponseDto { Success = false, ErrorMessage = errors });
            }

            return Ok(new AuthResponseDto { Success = true });
        }
    }
}