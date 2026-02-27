using Application.DTOs.Request.User;
using Application.DTOs.Response.Auth;
using AutoMapper;
using Domain.IUnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.User
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<UserResponse> CreateUser(CreateUserRequest model)
        {
            try
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model), "CreateUserRequest model cannot be null.");
                }
                var user = _mapper.Map<Domain.Entities.User>(model);
                user.CreatedAt = DateTime.UtcNow;
                await _unitOfWork.UserRepository.AddAsync(user);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    var result = _mapper.Map<UserResponse>(user);
                    return result;
                }
                throw new Exception("Failed to create user.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteUser(Guid id)
        {
            try
            {
                var existingUser = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false && x.IsActive == true);
                if (existingUser == null)
                {
                    throw new KeyNotFoundException("User not found.");
                }
                existingUser.IsDeleted = true;
                _unitOfWork.UserRepository.Update(existingUser);
                if( await _unitOfWork.SaveChangesAsync() > 0)
                {
                    return true;
                }
                else
                {
                    throw new Exception("Failed to delete user.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<UserResponse>> GetAllUser()
        {
            try
            {
                var users = await _unitOfWork.UserRepository.FindAsync(u => u.IsDeleted == false && u.IsActive == true);
                if(users == null || !users.Any())
                {
                    throw new KeyNotFoundException("No users found.");
                }
                return _mapper.Map<IEnumerable<UserResponse>>(users);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserResponse?> GetUserById(Guid id)
        {
            try
            {
                var user = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(u => u.Id == id && u.IsDeleted == false && u.IsActive == true);
                if (user == null)
                {
                    throw new KeyNotFoundException("User not found.");
                }
                return _mapper.Map<UserResponse>(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserResponse> UpdateUser(Guid id, CreateUserRequest model)
        {
            try
            {
                var existingUser = await _unitOfWork.UserRepository.GetFirstOrDefaultAsync(x => x.Id == id && x.IsDeleted == false && x.IsActive == true);
                if (existingUser == null)
                {
                    throw new KeyNotFoundException("User not found.");
                }
                _mapper.Map(model, existingUser);
                existingUser.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.UserRepository.Update(existingUser);
                if (await _unitOfWork.SaveChangesAsync() > 0)
                {
                    var result = _mapper.Map<UserResponse>(existingUser);
                    return result;
                }
                throw new Exception("Failed to update user.");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
