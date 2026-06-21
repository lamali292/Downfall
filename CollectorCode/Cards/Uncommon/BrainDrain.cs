using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class BrainDrain : CollectorCardModel
{
    public BrainDrain() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithVars(new DamageVar(6, ValueProp.Move | ValueProp.Unblockable | ValueProp.Unpowered).WithUpgrade(1));
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target?.Monster == null) return;
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        if (ModelDb.CardPool<CollectibleCardPool>()
                .AllCards.Cast<ICollectible>()
                .FirstOrDefault(e => e.GetMonsterModel().Id == cardPlay.Target.Monster.Id) is not CollectorCardModel
            collectible) return;
        var card = Owner.Creature.CombatState!.CreateCard(collectible, Owner);
        if (IsUpgraded)
            CardCmd.Upgrade(card);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }
}