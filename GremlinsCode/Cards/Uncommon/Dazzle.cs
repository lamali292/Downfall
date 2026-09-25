using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Gremlins.GremlinsCode.Core;
using Gremlins.GremlinsCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Gremlins.GremlinsCode.Cards.Uncommon;

[Pool(typeof(GremlinsCardPool))]
public class Dazzle : GremlinsCardModel
{
    public Dazzle() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9, 4);
        WithPower<StrengthPower>(2);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var shouldStealStrength = Owner.Creature.GetPowerAmount<WizPower>() >= 3;
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (shouldStealStrength)
        {
            await GremlinsCmd.Steal<StrengthPower>(ctx, cardPlay, this);
        }
    }
}