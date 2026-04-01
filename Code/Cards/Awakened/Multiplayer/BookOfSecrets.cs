using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Cards.Awakened.Multiplayer;

[Pool(typeof(AwakenedCardPool))]
public class BookOfSecrets : AwakenedCardModel
{
    public BookOfSecrets() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(DownfallKeyword.Conjure);
        WithKeywords(CardKeyword.Exhaust);
        WithBlock(6);
    }
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        // TODO - i only create multiplayer desyncs here.
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

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Exhaust);
    }
}