using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Guardian.GuardianCode.Cards.Token;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Relics;

[Pool(typeof(GuardianRelicPool))]
public class BronzeGear() : GuardianRelicModel(RelicRarity.Starter)
{
    public override bool HasUponPickupEffect => true;

    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<GuardianGear>();
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner || Owner.PlayerCombatState is not { TurnNumber: 1 }) return;
        await DownfallCardCmd.GiveCard<GearUp>(player, PileType.Hand);
    }

    public override async Task AfterObtained()
    {
        var card = Owner.RunState.Rng.CombatCardGeneration
            .NextItem(GuardianModelDb.AllGems.Where(e => e.Rarity == CardRarity.Common))?
            .ToCard.ToMutable();
        if (card == null) return;
        Owner.RunState.AddCard(card, Owner);
        var addResult = await CardPileCmd.Add(card, PileType.Deck);
        CardCmd.PreviewCardPileAdd(addResult);
    }
}