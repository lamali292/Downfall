using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Cards.Token;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Interfaces;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class ItLooksTasty : SlimeBossCardModel, IHasConsumeEffect
{
    public ItLooksTasty() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(8, 2);
        WithUpgradingCardTip<Lick>();
        WithTip(SlimeBossTip.Consume);
    }

    protected override Artist Artist => Artist.Get<Opal>();

    public async Task ConsumeEffect(PlayerChoiceContext ctx, Creature creature, AttackCommand command, int amount)
    {
        await DownfallCardCmd.GiveCard<Lick>(Owner, PileType.Hand, upgraded: IsUpgraded);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}