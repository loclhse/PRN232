using Application.DTOs.Request.Auth;
using Application.Service.Security;
using Domain.IUnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.ChangePassword
{
    public class ChangePasswordService : IChangePasswordService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;

        public ChangePasswordService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<(bool Success, string Message)> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
        {
            if (request == null)
                return (false, "Dữ liệu không hợp lệ.");

            if (string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.NewPassword) ||
                string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                return (false, "Vui lòng nhập đầy đủ thông tin.");
            }

            if (request.NewPassword != request.ConfirmPassword)
                return (false, "Xác nhận mật khẩu không khớp.");

            if (request.Password == request.NewPassword)
                return (false, "Mật khẩu mới không được trùng với mật khẩu hiện tại.");

            // Đổi "Users" thành đúng tên repo thật trong IUnitOfWork của cốt nếu khác
            var user = await _unitOfWork.UserRepository.GetByIdAsync(userId);
            if (user == null)
                return (false, "Không tìm thấy người dùng.");

            // Đổi "Password" thành đúng tên field thật trong entity User nếu project đang dùng PasswordHash / HashedPassword
            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
                return (false, "Mật khẩu hiện tại không đúng.");

            user.PasswordHash = _passwordHasher.Hash(request.NewPassword);

            // Đổi SaveChangesAsync thành tên hàm thật trong IUnitOfWork nếu project đang dùng CommitAsync / CompleteAsync
            await _unitOfWork.SaveChangesAsync();

            return (true, "Đổi mật khẩu thành công.");
        }
    }

}
