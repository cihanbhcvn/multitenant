﻿namespace WebAPI.Models.DTOs
{
    public class LoginReq
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string TenantId { get; set; }
    }
}
