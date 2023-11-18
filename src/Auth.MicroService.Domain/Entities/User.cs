﻿using System;
using System.Runtime.CompilerServices;

namespace Auth.MicroService.Domain.Entities
{
    public class User
    {
        public int? UserId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public DateTime BirthDate { get; private set; }
        public int RoleId { get; private set; }
        public bool Status { get; private set; }
        public DateTime CreationDateUtc { get; private set; }
        public DateTime? LastUpdateDateUtc { get; private set; }

        private User(
            int? userId,
            string firstName,
            string lastName,
            string email,
            string password,
            DateTime birthDate,
            int roleId,
            bool status,
            DateTime creationDateUtc,
            DateTime? lastUpdateDateUtc)
        {
            this.UserId = userId;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Email = email;
            this.Password = password;
            this.BirthDate = birthDate;
            this.RoleId = roleId;
            this.Status = status;
            this.CreationDateUtc = creationDateUtc;
            this.LastUpdateDateUtc = lastUpdateDateUtc;
        }

        public static User Create(
            int? userId,
            string firstName,
            string lastName,
            string email,
            string password,
            DateTime birthDate,
            int roleId,
            bool status,
            DateTime creationDateUtc,
            DateTime? lastUpdateDateUtc)
        {
            // Validations

            return new User(
                userId,
                firstName,
                lastName,
                email,
                password,
                birthDate,
                roleId,
                status,
                creationDateUtc,
                lastUpdateDateUtc);
        }

        public static User CreateNewUser(
            string firstName,
            string lastName,
            string email,
            string password,
            DateTime birthDate)
        {
            return User.Create(
                null,
                firstName,
                lastName,
                email,
                password,
                birthDate,
                3,
                false,
                DateTime.Now,
                null);
            }
    }
 }
