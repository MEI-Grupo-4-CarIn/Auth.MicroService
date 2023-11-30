using Auth.MicroService.Domain.Enums;
using System;
using System.Text.RegularExpressions;

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
        public Role RoleId { get; private set; }
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
            Role roleId,
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
            Role roleId,
            bool status,
            DateTime creationDateUtc,
            DateTime? lastUpdateDateUtc)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentException("First name cannot be null or empty.", nameof(firstName));
            }
            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException("Last name cannot be null or empty.", nameof(lastName));
            }
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            }
            if (!IsValidEmail(email))
            {
                throw new ArgumentException("Email is not in a valid format.", nameof(email));
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));
            }
            if (password.Length < 8)
            {
                throw new ArgumentException("Password must have at least 8 characters.", nameof(password));
            }
            if (birthDate > DateTime.UtcNow)
            {
                throw new ArgumentException("Birth date cannot be a future date", nameof(birthDate));
            }

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
            DateTime birthDate,
            Role roleId = Role.Driver,
            bool status = false)
        {
            return User.Create(
                null,
                firstName,
                lastName,
                email,
                password,
                birthDate,
                roleId,
                status,
                DateTime.Now,
                null);
        }

        public static User SetUserHashedPassword(User user, string hashedPassword)
        {
            return new User(
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                hashedPassword,
                user.BirthDate,
                user.RoleId,
                user.Status,
                user.CreationDateUtc,
                user.LastUpdateDateUtc);
        }

        public static User SetUserActivation(User user, Role? roleId, bool? status)
        {
            return new User(
                user.UserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Password,
                user.BirthDate,
                roleId ?? user.RoleId,
                status ?? user.Status,
                user.CreationDateUtc,
                user.LastUpdateDateUtc);
        }

        public User Update(
            string firstName,
            string lastName,          
            Role roleId,
            bool status)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.RoleId = roleId;
            this.Status = status;

            return this;
        }

        private static bool IsValidEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            if (string.IsNullOrEmpty(email))
                return false;
            Regex regex = new Regex(emailPattern);
            return regex.IsMatch(email);
        }
    }
 }
