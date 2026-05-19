using MediatR;

namespace Application.Comments.Update;

public record class UpdateCommentCommand(
    string Text) : IRequest;

