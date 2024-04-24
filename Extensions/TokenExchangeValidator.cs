using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace IdentityServerDemo
{
    public class TokenExchangeGrantValidator : IExtensionGrantValidator
    {
        private readonly ITokenValidator _validator;
        private readonly IResourceStore _resourceStore;
        private readonly IClientSecretValidator _clientValidator;
        private readonly IHttpContextAccessor _context;

        public TokenExchangeGrantValidator(ITokenValidator validator, IResourceStore resourceStore, IClientSecretValidator clientValidator, IHttpContextAccessor context)
        {
            _validator = validator;
            _resourceStore = resourceStore;
            _clientValidator = clientValidator;
            _context = context;
        }

        public string GrantType => "token-exchange";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
//            var audienceName = context.Request.Raw.Get("audience");
//            var audience = await _resourceStore.FindApiResourceAsync(audienceName);

            var userToken = context.Request.Raw.Get("subject_token");

            if (string.IsNullOrEmpty(userToken))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }

            var clientValidationResult = await _clientValidator.ValidateAsync(_context.HttpContext);
            
            var result = await _validator.ValidateAccessTokenAsync(userToken);
            if (result.IsError)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
                return;
            }
//          // get user's identity
            var sub = result.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

            context.Result = new GrantValidationResult(sub, GrantType, new []
            {
                new Claim("act",
                    JsonConvert.SerializeObject(new {sub = clientValidationResult.Client.ClientId}), 
                    IdentityServerConstants.ClaimValueTypes.Json) 
            });
            
            return;
        }
    }
}
