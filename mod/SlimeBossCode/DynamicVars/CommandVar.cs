using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Events;

namespace SlimeBoss.SlimeBossCode.DynamicVars;

public class CommandVar : DynamicVar
{
    public CommandVar(decimal value) : base("Command", value)
    {
        this.WithTooltip();
    }

    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks)
    {
        PreviewValue = IntValue;
    }
}