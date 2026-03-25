using Application.DTOs.Request.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.ChangePassword
{
    public interface IChangePasswordService
    {
        Task<(bool Success, string Message)> ChangePasswordAsync(Guid userId, ChangePasswordRequest request);
    }
}
