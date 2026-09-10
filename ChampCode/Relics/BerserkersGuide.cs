using BaseLib.Utils;
using Champ.ChampCode.Core;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Relics;

[Pool(typeof(ChampRelicPool))]
public class BerserkersGuide : ChampRelicModel
{
    public BerserkersGuide() : base(RelicRarity.Common)
    {
        WithPower<VigorPower>(3);
    }
    
    
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner) return;
        Flash();
        await MyCommonActions.ApplySelf<VigorPower>(ctx, this); }
}