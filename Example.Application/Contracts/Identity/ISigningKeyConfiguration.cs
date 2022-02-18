using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Application.Contracts.Identity
{
    public interface ISigningKeyConfiguration
    {
        SigningCredentials SigningCredentials { get; }
        SecurityKey SecurityKey { get; }
    }
}
