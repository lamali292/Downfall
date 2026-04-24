using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Guardian.Token;
using Downfall.Code.Commands;
using Downfall.Code.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Relics.Guardian;

[Pool(typeof(GuardianRelicPool))]
public class BronzeGear : GuardianRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;


    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<GuardianGear>();
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner || combatState.RoundNumber > 1) return;
        await DownfallCardCmd.GiveCard<GearUp>(player, PileType.Hand);
    }

    public override async Task AfterObtained()
    {
        var card = DownfallModelDb.AllGems.Where(e => e.Rarity == CardRarity.Common).TakeRandom(1, Owner.RunState.Rng.CombatCardGeneration).FirstOrDefault()?.ToCard.ToMutable();
        if (card == null) return;
        Owner.RunState.AddCard(card, Owner);
        var addResult = await CardPileCmd.Add(card, PileType.Deck);
        CardCmd.PreviewCardPileAdd(addResult);
    }
}