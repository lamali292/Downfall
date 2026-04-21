using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Awakened.Uncommon;

[Pool(typeof(AwakenedCardPool))]
public class MysticOrder : AwakenedCardModel
{
    public MysticOrder() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(DownfallTip.Conjure);
        WithCards(2, 1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(CombatState);
        await CommonActions.Draw(this, ctx);
        await AwakenedCmd.Conjure(Owner, CombatState);
    }
}