using BaseLib.Utils;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class SoulLitLamp : CollectorRelicModel
{
    public SoulLitLamp()
    {
        WithTip(typeof(Ember));
    }
     
    
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    
    public override Task AfterObtained()
    {
        EssenceModel.AddEssence(Owner, 3);
        return Task.CompletedTask;
    }

    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext ctx,
        ICombatState combatState)
    {
        if (player != Owner) return;
        await DownfallCardCmd.GiveCard<Ember>(Owner, PileType.Hand);
    }
}