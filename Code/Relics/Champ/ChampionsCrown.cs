using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.Champ.Token;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Relics.Champ;

[Pool(typeof(ChampRelicPool))]
public class ChampionsCrown : ChampRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;

    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<VictoriousCrown>();
    }
    
    
    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext choiceContext,
        CombatState combatState)
    {
        if (player != Owner || combatState.RoundNumber > 1) return;
        var card = combatState.CreateCard<Inspiration>(Owner);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, true);
        Flash();
    }
}