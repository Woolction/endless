using Application.Channels.Dtos;

namespace Application.Channels.Search;

public record class SearchedChannelDto(
    ChannelDto ChannelsDto, double Score);