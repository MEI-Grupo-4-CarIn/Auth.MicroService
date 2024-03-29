﻿namespace Auth.MicroService.Domain.Entities
{
    public class UserInfo
    {
        public int UserId { get; init; }
        public string UserFullName { get; init; }
        public string Email { get; init; }
        public string Role { get; init; }
        public bool Status { get; init; }
        public string CreationDate { get; init; }
        public string LastUpdateDate { get; init; }
    }
}
