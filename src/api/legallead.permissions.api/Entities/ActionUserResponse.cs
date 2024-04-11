using legallead.jdbc.entities;
using Microsoft.AspNetCore.Mvc;

namespace legallead.permissions.api.Entities
{
    public class ActionUserResponse
    {
        public IActionResult? Result { get; set; }
        public User? User { get; set; }
    }
}
