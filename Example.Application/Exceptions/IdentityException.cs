using Microsoft.AspNetCore.Identity;

namespace Example.Application.Exceptions
{
    public class IdentityException : ApplicationException
    {
        public List<string> IdentityErrors { get; set; }

        public IdentityException(IEnumerable<IdentityError> identityErrors)
        {
            IdentityErrors = new List<string>();

            foreach (var validationError in identityErrors)
                IdentityErrors.Add(validationError.Description);
        }
    }
}
