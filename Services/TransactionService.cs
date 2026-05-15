using Microsoft.EntityFrameworkCore;
using Practical_Assignment.Data;
using Practical_Assignment.DTOs;
using Practical_Assignment.Models;
using Practical_Assignment.Services.Interfaces;

namespace Practical_Assignment.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly AppDbContext _context;

        public TransactionService(AppDbContext context)
        {
            _context = context;
        }

        public ApiResponse<object> Deposit(string studentId, decimal amount)
        {
            // 1. Validate input
            if (amount <= 0)
            {
                return new ApiResponse<object> { Status = "error", Message = "Amount must be greater than 0" };
            }

            // 2. Load wallet
            var wallet = _context.Wallets.FirstOrDefault(w => w.StudentId == studentId);
            if (wallet == null)
            {
                return new ApiResponse<object> { Status = "error", Message = "Wallet not found" };
            }

            // 4. Process & update balance
            wallet.Balance += amount;

            // 5. Log transaction
            var transaction = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                WalletId = wallet.WalletId,
                Type = "DEPOSIT",
                Amount = amount,
                Description = "Account Deposit",
                Timestamp = DateTime.UtcNow,
                IsSuccess = true
            };
            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            // 6. Return confirmation
            return new ApiResponse<object>
            {
                Status = "success",
                Message = "Deposit successful",
                Data = new
                {
                    transactionId = transaction.TransactionId,
                    type = transaction.Type,
                    amount = transaction.Amount,
                    newBalance = wallet.Balance,
                    timestamp = transaction.Timestamp
                }
            };
        }

        public ApiResponse<object> Pay(string studentId, decimal amount, ServiceType serviceType)
        {
            // 1. Validate input
            if (amount <= 0)
            {
                return new ApiResponse<object> { Status = "error", Message = "Amount must be greater than 0" };
            }

            // 2. Load wallet
            var wallet = _context.Wallets.FirstOrDefault(w => w.StudentId == studentId);
            if (wallet == null)
            {
                return new ApiResponse<object> { Status = "error", Message = "Wallet not found" };
            }

            // 3. Check balance
            if (wallet.Balance < amount)
            {
                // 5. Log transaction (fail)
                var failedTx = new Transaction
                {
                    TransactionId = Guid.NewGuid(),
                    WalletId = wallet.WalletId,
                    Type = "PAYMENT",
                    Amount = amount,
                    Description = $"Payment for {serviceType}",
                    Timestamp = DateTime.UtcNow,
                    IsSuccess = false
                };
                _context.Transactions.Add(failedTx);
                _context.SaveChanges();

                return new ApiResponse<object> { Status = "error", Message = "Insufficient balance" };
            }

            // 4. Process & update balance
            wallet.Balance -= amount;

            // 5. Log transaction (success)
            var transaction = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                WalletId = wallet.WalletId,
                Type = "PAYMENT",
                Amount = amount,
                Description = $"Payment for {serviceType}",
                Timestamp = DateTime.UtcNow,
                IsSuccess = true
            };
            _context.Transactions.Add(transaction);
            _context.SaveChanges();

            // 6. Return confirmation
            return new ApiResponse<object>
            {
                Status = "success",
                Message = "Payment successful",
                Data = new
                {
                    transactionId = transaction.TransactionId,
                    type = transaction.Type,
                    amount = transaction.Amount,
                    newBalance = wallet.Balance,
                    timestamp = transaction.Timestamp
                }
            };
        }

        public ApiResponse<object> Transfer(string senderStudentId, string receiverStudentId, decimal amount)
        {
            // 1. Validate input
            if (amount <= 0)
            {
                return new ApiResponse<object> { Status = "error", Message = "Amount must be greater than 0" };
            }

            if (senderStudentId == receiverStudentId)
            {
                return new ApiResponse<object> { Status = "error", Message = "Cannot transfer to self" };
            }

            // 2. Load wallet
            var senderWallet = _context.Wallets.FirstOrDefault(w => w.StudentId == senderStudentId);
            if (senderWallet == null)
            {
                return new ApiResponse<object> { Status = "error", Message = "Sender wallet not found" };
            }

            var receiverWallet = _context.Wallets.FirstOrDefault(w => w.StudentId == receiverStudentId);
            if (receiverWallet == null)
            {
                return new ApiResponse<object> { Status = "error", Message = "Receiver must exist" };
            }

            // 3. Check balance
            if (senderWallet.Balance < amount)
            {
                // 5. Log transaction (fail)
                var failedTx = new Transaction
                {
                    TransactionId = Guid.NewGuid(),
                    WalletId = senderWallet.WalletId,
                    Type = "TRANSFER_OUT",
                    Amount = amount,
                    Description = $"Transfer to {receiverStudentId}",
                    ReceiverStudentId = receiverStudentId,
                    Timestamp = DateTime.UtcNow,
                    IsSuccess = false
                };
                _context.Transactions.Add(failedTx);
                _context.SaveChanges();

                return new ApiResponse<object> { Status = "error", Message = "Insufficient balance" };
            }

            // 4. Process & update balance
            senderWallet.Balance -= amount;
            receiverWallet.Balance += amount;

            // 5. Log transaction (success)
            var senderTx = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                WalletId = senderWallet.WalletId,
                Type = "TRANSFER_OUT",
                Amount = amount,
                Description = $"Transfer to {receiverStudentId}",
                ReceiverStudentId = receiverStudentId,
                Timestamp = DateTime.UtcNow,
                IsSuccess = true
            };

            var receiverTx = new Transaction
            {
                TransactionId = Guid.NewGuid(),
                WalletId = receiverWallet.WalletId,
                Type = "TRANSFER_IN",
                Amount = amount,
                Description = $"Transfer from {senderStudentId}",
                ReceiverStudentId = senderStudentId, // Store sender in ReceiverStudentId to track other party
                Timestamp = senderTx.Timestamp,
                IsSuccess = true
            };

            _context.Transactions.Add(senderTx);
            _context.Transactions.Add(receiverTx);
            _context.SaveChanges();

            // 6. Return confirmation
            return new ApiResponse<object>
            {
                Status = "success",
                Message = "Transfer successful",
                Data = new
                {
                    transactionId = senderTx.TransactionId,
                    type = senderTx.Type,
                    amount = senderTx.Amount,
                    newBalance = senderWallet.Balance,
                    timestamp = senderTx.Timestamp
                }
            };
        }
    }
}
