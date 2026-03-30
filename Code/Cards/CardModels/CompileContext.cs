namespace Downfall.Code.Cards.CardModels;

public record CompileContext(int SlotIndex, int TotalCards)
{
    public bool IsFirst => SlotIndex == 0;
    public bool IsLast => SlotIndex == TotalCards - 1;
    public bool IsMiddle => !IsFirst && !IsLast;
}