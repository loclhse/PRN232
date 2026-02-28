using Application.DTOs.Request.User;
using Application.DTOs.Response;
using Application.DTOs.Response.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.User
{
    public interface IUserService
    {
        Task<UserResponse> CreateUser(CreateUserRequest model);
        Task<UserResponse> UpdateUser(Guid id, CreateUserRequest model);
        Task<bool> DeleteUser(Guid id);
        Task<IEnumerable<UserResponse>> GetAllUser();
        Task<UserResponse?> GetUserById(Guid id);
    }
}
