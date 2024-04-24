using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace IdentityServerDemo
{
    public class SecuritySettings
    {
        public List<Client> Clients { get; set; } = new List<Client>();
        public List<ApiResource> ApiResources { get; set; } = new List<ApiResource>();
        public List<ApiScope> ApiScopes { get; set; } = new List<ApiScope>();
        public List<IdentityResource> IdentityResources { get; set; } = new List<IdentityResource>();
        public List<TestUser> Users { get; set; } = new List<TestUser>();
    }
}