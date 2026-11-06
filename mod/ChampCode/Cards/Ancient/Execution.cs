using BaseLib.Utils;
using Champ.ChampCode.Core;
using Champ.ChampCode.Interfaces;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Cards.Ancient;

[Pool(typeof(ChampCardPool))]
public class Execution : ChampCardModel
{
    public Execution() : base(2, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
    {
        WithDamage(6, 3);
        WithRepeat(4);
        WithFinisher();
        // WithTip(ChampTip.Stance);
    }
    
    public override bool CanBeGeneratedInCombat => false;

    protected override Artist Artist => Artist.Get<GoofballMcgee>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, 4, "vfx/vfx_attack_blunt", null, "heavy_attack.mp3")
            .WithAttackerAnim(Core.Champ.GetJumpAnimIfApplicable(Owner.Character),
                Core.Champ.GetJumpAttackDelayIfApplicable(Owner.Character))
            .Execute(ctx);
        await ChampCmd.PlayFinisher(ctx, cardPlay, Finisher);
    }

    public override FinisherDescriptor Finisher => new(KeepsStance: true, RepeatCount: () => 2);
}