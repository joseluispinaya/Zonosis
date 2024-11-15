using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zonosis.Shared.Enumerations;

namespace Zonosis.Shared.DTOs
{
    public class UserDetailDTO
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public UserType UserType { get; set; }
        public ICollection<PetDetailRelDTO>? PetDetailRelDto { get; set; }
    }
}
