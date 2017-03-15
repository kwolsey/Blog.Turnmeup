﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using Blog.Turnmeup.DAL.Models;
using Blog.Turnmeup.DL.Models;
using Blog.Turnmeup.DL.Repositories;
using Microsoft.AspNetCore.Identity;

namespace Blog.Turnmeup.DL.Services
{
    public class UsersService : IUsersService
    {
        private readonly IUsersRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUserValidator<AppUser> _userValidator;
        private readonly IPasswordValidator<AppUser> _passwordValidator;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly SignInManager<AppUser> _signInManager;


        public UsersService(IUsersRepository repository, IMapper mapper, IUserValidator<AppUser> userValidator, IPasswordValidator<AppUser> passwordValidator, IPasswordHasher<AppUser> passwordHasher, SignInManager<AppUser> signInManager)
        {
            _repository = repository;
            _mapper = mapper;
            _userValidator = userValidator;
            _passwordValidator = passwordValidator;
            _passwordHasher = passwordHasher;
            _signInManager = signInManager;
        }

        public IQueryable<UserResponseModel> Get()
        {

            var returnedList = new List<UserResponseModel>();
            _repository.Get().ToList().ForEach(u =>
            {
                returnedList.Add(_mapper.Map<AppUser, UserResponseModel>(u));
            });

            return returnedList.AsQueryable();
        }

        public UserResponseModel GetByEmail(string email)
        {
            return _mapper.Map<AppUser, UserResponseModel>(_repository.GetByEmail(email));
        }

        public Task<IdentityResult> Create(UserResponseModel user, string password)
        {
            return _repository.Create(_mapper.Map<AppUser, UserResponseModel>(user), password);
        }

        public async Task<IdentityResult> Delete(UserResponseModel user)
        {
            return await _repository.Delete(_mapper.Map<AppUser, UserResponseModel>(user));
        }

        public  async Task<IdentityResult> Update(UserResponseModel user)
        {
            return await _repository.Update(_mapper.Map<AppUser, UserResponseModel>(user));
        }

        public async Task<IdentityResult> ValidatePassword(UserResponseModel user, string password)
        {
            var appUser = _mapper.Map<AppUser, UserResponseModel>(user);
           return await _passwordValidator.ValidateAsync(_repository.GetUserManager(), appUser, password);
        }

        public async Task<IdentityResult> ValidateUser(UserResponseModel user)
        {
            var appUser = _mapper.Map<AppUser, UserResponseModel>(user);
            return await _userValidator.ValidateAsync(_repository.GetUserManager(), appUser);
        }

        public  string HashPassword(UserResponseModel user, string password)
        {
            var appUser = _mapper.Map<AppUser, UserResponseModel>(user);
            return  _passwordHasher.HashPassword( appUser, password);
        }

        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<SignInResult> PasswordSignInAsync(UserResponseModel user, string password, bool lockoutOnFailure, bool isPersistent)
        {
            var appUser = _mapper.Map<AppUser, UserResponseModel>(user);
           return  await _signInManager.PasswordSignInAsync(appUser, password,  isPersistent, lockoutOnFailure);
        }

    }
}