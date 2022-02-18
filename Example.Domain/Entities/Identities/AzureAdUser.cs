using System;
using System.Collections.Generic;
using System.Text;

namespace Example.Domain.Entities.Identities
{
    public class AzureAdUser
    {
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string JobTitle { get; set; }
        public string Mail { get; set; }
        public string MobilePhone { get; set; }
        public string OfficeLocation { get; set; }
        public string PreferredLanguage { get; set; }
        public string Surname { get; set; }
        public string UserPrincipalName { get; set; }
        public Guid Id { get; set; }
    }
}
