using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Potions;

[Pool(typeof(HexaghostPotionPool))]
public class EctoCoolerPotion : HexaghostPotionModel
{
    public EctoCoolerPotion() : base( PotionRarity.Common, PotionUsage.CombatOnly, TargetType.Self)
    {
        WithTip(HexaghostKeyword.Afterlife);
    }
    
    protected override Artist Artist => Artist.Get<Chimedragon>();


    protected override async Task OnUse(PlayerChoiceContext ctx, Creature? target)
    {
        AssertValidForTargetedPotion(target);
        var player = target.Player;
        if (player == null) return;
        var card = await CardSelectCmd.FromChooseACardScreen(ctx, CardFactory.GetDistinctForCombat(player, player.Character.CardPool.GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint).Where(c => c.Keywords.Contains(HexaghostKeyword.Afterlife)), 3, player.RunState.Rng.CombatCardGeneration).ToList(), player, true);
        if (card == null)
            return;
        card.SetToFreeThisTurn();
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }
}