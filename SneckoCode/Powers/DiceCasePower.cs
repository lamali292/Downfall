using BaseLib.Patches.Localization;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Platform;
using MegaCrit.Sts2.Core.Runs;
using Snecko.SneckoCode.Cards.Token;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Powers;

public class DiceCasePower : SneckoPowerModel, IAddDumbVariablesToPowerDescription, IInstancedPerTarget
{
    public DiceCasePower()
    {
        WithTip<SoulRoll>();
    }
    
    public Creature? TargetCreature { get; set; }
    
    public override PowerInstanceType InstanceType => CustomPowerInstanceType.InstancedPerTarget;

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext crx, ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        var otherPlayer = TargetCreature?.Player;
        if (otherPlayer == null) return;
        await DownfallCardCmd.GiveCards<SoulRoll>(otherPlayer, PileType.Hand, Amount, CardPilePosition.Top,
            creator: Owner.Player);
    }

    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        // cant use Target because hovertip only visible for the target player
        var otherCreature = TargetCreature;
        switch (otherCreature)
        {
            case { IsMonster: true, Monster: not null }:
                description.Add("OtherName", otherCreature.Monster.Title);
                break;
            case {Player: not null}:
                description.Add("OtherName", PlatformUtil.GetPlayerName(RunManager.Instance.NetService.Platform, otherCreature.Player.NetId));
                break;
            default:
                description.Add("OtherName", "???");
                break;
        }
    }


  
}