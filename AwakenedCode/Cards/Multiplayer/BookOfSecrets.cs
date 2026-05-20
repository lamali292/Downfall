using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Cards.Multiplayer;

[Pool(typeof(AwakenedCardPool))]
public class BookOfSecrets : AwakenedCardModel
{
    public BookOfSecrets() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithConjure();
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
        WithBlock(6);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        // TODO - i only create multiplayer desyncs here. need to look into this again. is probably easy but too lazy right now
        /*if (CombatState == null) return;
        var spell = AwakenedCmd.GetSpellbook(Owner);
        var nextSpell = spell?.NextSpell;
        if (nextSpell == null) return;
        foreach (var creature in CombatState.GetTeammatesOf(Owner.Creature).Where(c => c is { IsAlive: true, IsPlayer: true }))
        {
            var player = creature.Player;
            if (player == null || player == Owner) continue;


            var card = CombatState.CreateCard(ModelDb.GetById<CardModel>(nextSpell.Id), player);
            var result = await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, true);
            if (result.success)
                CardCmd.PreviewCardPileAdd(result, 0.1f);
        }*/
    }
}