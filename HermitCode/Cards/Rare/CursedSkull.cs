using BaseLib.Abstracts;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Cards.Rare;

public class CursedSkull : HermitCardModel
{
    public CursedSkull() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithTip(HermitKeywords.DeadOn);
        WithTip(StaticHoverTip.ReplayStatic);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override Artist Artist => Artist.Get<DawnablesAwakened>();

    public override bool CanBeGeneratedInCombat => false;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var card = (await CardSelectCmd.FromHand(ctx, Owner, prefs, null, this)).FirstOrDefault();
        if (card == null) return;
        var deadOnReplay = CardModifier.Modifiers(card).OfType<DeadOnReplay>().FirstOrDefault();
        if (deadOnReplay == null)
            CardModifier.AddModifier<DeadOnReplay>(card);
        else
            deadOnReplay.Value += 1;
    }
}

public class DeadOnReplay : DownfallCardModifier
{
    private bool IsDeadOn => Owner != null && HermitCmd.IsInDeadOnState(Owner);
    private int ModVal => Value * (Owner?.Owner.Creature.HasPower<SnipePower>() ?? false ? 2 : 1);
    public override bool ShouldGlowGold => IsDeadOn;
    public int Value { get; set; } = 1;

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        return card == Owner && IsDeadOn ? playCount + ModVal : playCount;
    }

    public override void ModifyDescription(Creature? target, ref string description)
    {
        var loc = Description;
        DynamicVars.AddTo(loc);
        loc.Add("Replay", ModVal);
        description += $"\n{loc.GetFormattedText()}";
    }
}