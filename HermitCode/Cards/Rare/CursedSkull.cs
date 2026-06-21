using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Abstract;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Cards.Rare;

public class CursedSkull : HermitCardModel
{
    public CursedSkull() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithKeyword(CardKeyword.Exhaust);
    }
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 1);
        var card = (await CardSelectCmd.FromHand(ctx, Owner, prefs, HasNotEffectAlready, this)).FirstOrDefault();
        if (card == null) return;
        CardModifier.AddModifier<DeadOnReplay>(card);
    }

    private static bool HasNotEffectAlready(CardModel cardModel)
    {
        return !CardModifier.Modifiers(cardModel).OfType<DeadOnReplay>().Any();
    }
    
    
}

public class DeadOnReplay : DownfallCardModifier
{
    public bool IsDeadOn => Owner != null && (HermitCmd.IsDeadOnInCurrentHandState(Owner) ||
                                               (PatchDeadOnCapture.LastPlayed == Owner && PatchDeadOnCapture.LastWasDeadOn));

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        return card == Owner && IsDeadOn ? playCount + (card.Owner.Creature.HasPower<SnipePower>() ? 2 : 1) : playCount;
    }

    public override void ModifyDescription(Creature? target, ref string description)
    {
        var loc = Description;
        DynamicVars.AddTo(loc);
        loc.Add("Replay", Owner?.Owner.Creature.HasPower<SnipePower>() ?? false ? 2 : 1);
        description += $"\n{loc.GetFormattedText()}";
    }

    public override bool ShouldGlowGold => IsDeadOn; 
}