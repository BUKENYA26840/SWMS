using Practical_Assignment.Data;
using Practical_Assignment.DTOs;
using Practical_Assignment.Models;
using Practical_Assignment.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Practical_Assignment.Services
{
    public class AuthService : IAuthService
    {
        private static readonly Dictionary<string, string> _tokens = new();
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<string>> LoginAsync(LoginRequest request)
        {
            Student student;
            try
            {
                student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == request.StudentId);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Status = "error",
                    Message = $"DB Error: {ex.Message}. Inner: {ex.InnerException?.Message}"
                };
            }

            if (student == null)
            {
                return new ApiResponse<string>
                {
                    Status = "error",
                    Message = "Account not found"
                };
            }

            if (student.IsLocked)
            {
                return new ApiResponse<string>
                {
                    Status = "error",
                    Message = "Account is locked due to multiple failed login attempts."
                };
            }

            if (student.PIN != request.PIN)
            {
                student.FailedAttempts++;
                if (student.FailedAttempts >= 3)
                {
                    student.IsLocked = true;
                }
                await _context.SaveChangesAsync();

                return new ApiResponse<string>
                {
                    Status = "error",
                    Message = "Invalid PIN"
                };
            }

            // Successful login
            student.FailedAttempts = 0;
            await _context.SaveChangesAsync();

            var token = Guid.NewGuid().ToString();
            _tokens[token] = student.StudentId;

            return new ApiResponse<string>
            {
                Status = "success",
                Message = "Login successful",
                Data = token
            };
        }

        public async Task<ApiResponse<object>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                if (await _context.Students.AnyAsync(s => s.StudentId == request.StudentId))
                {
                    return new ApiResponse<object>
                    {
                        Status = "error",
                        Message = "Account already exists"
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    Status = "error",
                    Message = $"DB Error: {ex.Message}. Inner: {ex.InnerException?.Message}"
                };
            }

            var student = new Student
            {
                StudentId = request.StudentId,
                FullName = request.FullName,
                PIN = request.PIN,
                IsLocked = false,
                FailedAttempts = 0
            };

            var wallet = new Wallet
            {
                WalletId = Guid.NewGuid(),
                StudentId = student.StudentId,
                Balance = 0, // Initial balance 0 for new registrations
                CreatedAt = DateTime.UtcNow
            };

            _context.Students.Add(student);
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();

            return new ApiResponse<object>
            {
                Status = "success",
                Message = "Registration successful! You can now login."
            };
        }

        public async Task<ApiResponse<object>> LogoutAsync(string token)
        {
            if (_tokens.ContainsKey(token))
            {
                _tokens.Remove(token);
                return await Task.FromResult(new ApiResponse<object>
                {
                    Status = "success",
                    Message = "Logout successful"
                });
            }

            return await Task.FromResult(new ApiResponse<object>
            {
                Status = "error",
                Message = "Invalid or missing token"
            });
        }

        public static string? GetStudentIdFromToken(string token)
        {
            if (_tokens.TryGetValue(token, out var studentId))
            {
                return studentId;
            }
            return null;
        }
    }
}
