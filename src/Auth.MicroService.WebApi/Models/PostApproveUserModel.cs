﻿using Auth.MicroService.Domain.Enums;
using System.Text.Json.Serialization;

namespace Auth.MicroService.WebApi.Models
{
    public class PostApproveUserModel
    {
        [JsonRequired]
        public int Id { get; set; }

        public Role Role { get; set; }
    }
}
