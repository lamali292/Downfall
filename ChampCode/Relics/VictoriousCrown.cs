using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.CustomEnums;
using Champ.ChampCode.Events;
using Champ.ChampCode.Extensions;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Relics;

[Pool(typeof(ChampRelicPool))]
public class VictoriousCrown : ChampRelicModel, IOnFinisher
{

    public VictoriousCrown() : base(RelicRarity.Starter)
    {
        WithTip(ChampTip.Stance);
        WithTip(ChampKeyword.TriggerSkillBonus);
        WithTip(ChampTip.Finisher);
        WithCards(1);
    }
    
    private CardPlay? _triggeringCardPlay;
    private bool _usedThisTurn;

    public async Task OnFinisher(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var player = cardPlay.Card.Owner;
        if (_usedThisTurn || player != Owner) return;
        Flash();
        await CardPileCmd.Draw(ctx, 2, player);
        _triggeringCardPlay = cardPlay;
        _usedThisTurn = true;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || _triggeringCardPlay != cardPlay) return;
        await ChampCmd.EnterRandomStance(choiceContext, Owner);
        _triggeringCardPlay = null;
        Status = RelicStatus.Normal;
    }


    public override async Task BeforeHandDraw(
        Player player,
        PlayerChoiceContext ctx,
        ICombatState combatState)
    {
        if (player != Owner) return;
        _usedThisTurn = false;
        Status = RelicStatus.Active;
        if (Owner.PlayerCombatState is not { TurnNumber: 1 }) return;
        Flash();
        await ChampCmd.EnterDifferentStance(ctx, player);
        var stance = Owner.ChampStance;
        await stance.SkillBonus(ctx);
        await MyCommonActions.Draw(this, ctx);
    }
}