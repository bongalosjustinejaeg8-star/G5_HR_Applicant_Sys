using HRApplicantSystem.Data.Models;
using HRApplicantSystem.Data.Repositories;
using HRApplicantSystem.Shared.Helpers;
using HRApplicantSystem.Services.Interfaces;

namespace HRApplicantSystem.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IApplicantAccountRepository _applicantAccountRepo;
    private readonly IUserRepository _userRepo;
    private readonly PasswordHasher _passwordHasher;

    public AuthService(
        IApplicantAccountRepository applicantAccountRepo,
        IUserRepository userRepo,
        PasswordHasher passwordHasher)
    {
        _applicantAccountRepo = applicantAccountRepo;
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
    }
    
    public async Task<ApplicantAccount?> LoginApplicantAsync(string email, string password)
    {
        // step 1: find account by email in DB
        var account = await _applicantAccountRepo.GetByEmailAsync(email);

        // step 2: if no account found, return null (wrong email)
        if (account == null) return null;

        // step 3: if account is disabled, reject login
        if (!account.IsActive) return null;

        // step 4: verify password against stored hash
        // "does this plain password match the hash in DB?"
        bool isPasswordValid = _passwordHasher.Verify(password, account.PasswordHash);

        // step 5: if password wrong, return null
        if (!isPasswordValid) return null;

        // step 6: everything checks out, return the account
        return account;
    }

    public async Task<User?> LoginHRAsync(string email, string password)
    {
        // same exact flow as LoginApplicantAsync
        // but queries Users table instead of ApplicantAccounts
        var user = await _userRepo.GetByEmailAsync(email);

        if (user == null) return null;

        if (!user.IsActive) return null;

        bool isPasswordValid = _passwordHasher.Verify(password, user.PasswordHash);

        if (!isPasswordValid) return null;

        return user;
    }

    public async Task<bool> RegisterApplicantAsync(string email, string password)
    {
        // step 1: check if email already exists (duplicate prevention)
        var existing = await _applicantAccountRepo.GetByEmailAsync(email);

        // step 2: if account already exists, reject registration
        if (existing != null) return false;

        // step 3: hash the password before saving
        // NEVER store plain text passwords in DB
        string hashedPassword = _passwordHasher.Hash(password);

        // step 4: create new account object
        var newAccount = new ApplicantAccount
        {
            Email = email,
            PasswordHash = hashedPassword,
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        // step 5: save to database and return result
        return await _applicantAccountRepo.CreateAsync(newAccount);
    }

    public async Task<bool> ChangePasswordAsync(string accountId, string newPassword)
    {
        // step 1: find the account
        var account = await _applicantAccountRepo.GetByIdAsync(accountId);

        // step 2: if not found, return false
        if (account == null) return false;

        // step 3: hash the new password
        account.PasswordHash = _passwordHasher.Hash(newPassword);

        // step 4: save updated account to DB
        return await _applicantAccountRepo.UpdateAsync(account);
    }
}