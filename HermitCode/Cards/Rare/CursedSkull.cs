using BaseLib.Abstracts;
using Downfall.DownfallCode.Abstract;
using Hermit.HermitCode.Core;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Patches;
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
        WithTip(HermitKeywords.DeadOn);
        WithKeyword(CardKeyword.Exhaust);
    }

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

    private static bool HasNotEffectAlready(CardModel cardModel)
    {
        return !CardModifier.Modifiers(cardModel).OfType<DeadOnReplay>().Any();
    }
}

public class DeadOnReplay : DownfallCardModifier
{
    public bool IsDeadOn
    {
        get
        {
            var pileType = Owner?.Pile?.Type;
            var inHand = pileType == PileType.Hand;
            var inPlay = pileType == PileType.Play;

            // only evaluate the relevant sub-condition, mirroring the original short-circuit
            var isDeadOnInHand = inHand && IsDeadOnInHand;
            var wasPlayedDeadOn = inPlay && WasThisPlayedDeadOn;

            return Owner != null && (isDeadOnInHand || wasPlayedDeadOn);
        }
    }

    private bool IsDeadOnInHand => Owner != null && HermitCmd.IsDeadOnInCurrentHandState(Owner);

    private bool WasThisPlayedDeadOn => DeadOnPatch.LastPlayed == Owner && DeadOnPatch.LastWasDeadOn;

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