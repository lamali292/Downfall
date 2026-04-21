using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Rooms;

namespace Downfall.Code.Cards.Champ.Rare;

[Pool(typeof(ChampCardPool))]
public class CheapShot : ChampCardModel
{
    public CheapShot() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithDamage(5);
        WithKeywords(CardKeyword.Exhaust);
        WithTip(StaticHoverTip.Stun);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (CombatState?.Encounter == null || cardPlay.Target == null) return;
        if (CombatState.Encounter.RoomType == RoomType.Boss)
        {
            await CommonActions.CardAttack(this, cardPlay, 3).Execute(ctx);
        }
        else
        {
            await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
            await CreatureCmd.Stun(cardPlay.Target);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}