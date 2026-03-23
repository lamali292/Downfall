namespace Downfall.Code.Cards.CardModels;

public record EncodeContext(bool IsFromFunction, int SlotIndex = 0)
{
    public static readonly EncodeContext Direct = new(false);
}