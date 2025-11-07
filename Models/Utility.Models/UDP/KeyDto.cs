namespace Utility.Models.UDP
{
    public record KeyDto(Guid Guid, string Name);

    public record GuidDto(Guid Source);

    public record TypeDto(string InName, string OutName);

    public record TypeGuidDto(Guid Guid, GuidDto GuidDto, TypeDto TypeDto);
}