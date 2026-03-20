using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace JobPortal.Domain.Entities;


public class Permission
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!; // code name

    [Required]
    [MaxLength(150)]
    public string DisplayName { get; set; } = default!; // for UI

    [MaxLength(500)]
    public string? Description { get; set; }

    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
