using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Domain.Entities.Identities
{
    public class JwtToken
    {
        public string Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
