using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

public sealed class Snipe : HermitCardModel
{
    public Snipe() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithKeyword(HermitKeywords.Concentrate, UpgradeType.Add);
        WithTip(HermitKeywords.DeadOn);
        this.WithPower<SnipePower>(1, false);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.ApplySelf<SnipePower>(ctx, this);
    }
}