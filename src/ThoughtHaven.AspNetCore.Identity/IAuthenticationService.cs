﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace ThoughtHaven.AspNetCore.Identity
{
    public interface IAuthenticationService<T>
    {
        Task Login(T login, AuthenticationProperties properties);
        Task<T> Authenticate();
        Task Logout();
    }
}