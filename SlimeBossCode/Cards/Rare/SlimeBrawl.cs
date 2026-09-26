using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

// Simplification: "Command ALL Split Slimes to attack the targeted enemy" uses each slime's normal Command()
// targeting (mostly random/all-opponents) rather than forcing every slime onto this card's specific target -
// forcing a target would need every SlimeModel.Command() to accept an override, out of scope for this pass.
[Pool(typeof(SlimeBossCardPool))]
public class SlimeBrawl : SlimeBossCardModel
{
    public SlimeBrawl() : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithPower<WeakPower>(2, 1);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
        await SlimeBossCmd.CommandAll(ctx, Owner, 1, this);
    }
}
