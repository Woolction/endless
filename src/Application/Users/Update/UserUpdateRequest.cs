using Microsoft.AspNetCore.Http;
using Domain.Common.Enums;

namespace Application.Users.Update;

public record class UserUpdateRequest(
    string? Name, string? Description, UserRole Role, IFormFile? AvatarPhoto);