using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zonosis.Shared.Entities
{
    public class UserFavorites
    {
        public User? User { get; set; }
        public string? UserId { get; set; }
        public Pet? Pet { get; set; }
        public int PetId { get; set; }
    }
}
