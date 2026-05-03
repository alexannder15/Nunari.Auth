using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Nunari.Auth.Application.Interfaces;
using Nunari.Auth.Domain.AggregateRoot;

namespace Nunari.Auth.Application.Services;

public class UserContextService(
    IHttpContextAccessor httpContextAccessor,
    UserManager<User> userManager
) : IUserContextService
{
    private User? _currentUser;

    public async Task<User> GetCurrentUserAsync()
    {
        if (_currentUser != null)
            return _currentUser;

        // ToDo: Don't use hardcoded email in production, this is just for testing purposes
        //check while request is made by a background (schedule) task. DO NOT REMOVE
        if (httpContextAccessor.HttpContext == null)
            _currentUser = await userManager.FindByNameAsync("admin@hotmail.com");
        else
        {
            var contextUser = httpContextAccessor.HttpContext.User;
            _currentUser = await userManager.GetUserAsync(contextUser);
        }

        if (_currentUser != null)
            return _currentUser;

        return _currentUser;
    }
}
