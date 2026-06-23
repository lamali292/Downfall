using BaseLib.Patches.Features;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;

namespace Hermit.HermitCode.Cards.Curse;

[Pool(typeof(CurseCardPool))]
public sealed class MementoCard : HermitCardModel
{
    public MementoCard() : base(0, CardType.Curse, CardRarity.Curse, DownfallTargetType.MeAndEnemies)
    {
        WithPower<VulnerablePower>(1);
        WithKeyword(CardKeyword.Retain);
    }

    protected override Artist Artist => Artist.Get<AlexMdle>();

    public override int MaxUpgradeLevel => 0;
    public override bool CanBeGeneratedByModifiers => false;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await CommonActions.Apply<VulnerablePower>(ctx, this, play);
    }

    private static bool IsMultiplayer => (RunManager.Instance.DebugOnlyGetState()?.Players.Count ?? 1) > 1;

    protected override void AddExtraArgsToDescription(LocString description)
    { 
        description.Add("Multiplayer", IsMultiplayer);
    }
}