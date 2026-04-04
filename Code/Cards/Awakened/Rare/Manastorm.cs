using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Rare;

[Pool(typeof(AwakenedCardPool))]
public class Manastorm : AwakenedCardModel
{
    public Manastorm() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(14, 4);
        WithTip(DownfallKeyword.Conjure);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState == null) return;
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await AwakenedCmd.Conjure(Owner, CombatState);
        await AwakenedCmd.Conjure(Owner, CombatState);
    }
}