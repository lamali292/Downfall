using Downfall.DownfallCode.Extensions;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class FullyLoaded : HermitCardModel
{
    public FullyLoaded() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(CardKeyword.Retain, UpgradeType.Add);
        WithTip(HermitKeywords.Strike);
        WithTip(HermitKeywords.Defend);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        HermitSfx.PlaySpin();
        HermitSfx.PlayReload();
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var strikesAndDefends = Owner.GetDraw()
            .Where(c => c.Tags.Contains(CardTag.Strike) || c.Tags.Contains(CardTag.Defend))
            .ToList();
        await CardPileCmd.Add(strikesAndDefends, PileType.Hand);
    }
}