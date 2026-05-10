using Application.Contents.Dtos;

namespace Application.Contents.Search;

public record class SearchedContentDto(
    ContentDto ContentDto, double Score);