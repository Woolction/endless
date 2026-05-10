using Microsoft.AspNetCore.Http;
using Application.Contents.Dtos;
using Domain.Common.Enums;
using MediatR;

namespace Application.Contents.Create;

public record class ContentCreateCommand(
    Guid UserId, Guid? ChannelId, IFormFile? ContentFile, IFormFile? PrewievPhoto,
    string Title, string? Description, ContentType ContentType) : IRequest<Result<ContentDto>>;