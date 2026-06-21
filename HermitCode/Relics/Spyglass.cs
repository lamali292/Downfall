using Hermit.HermitCode.Core;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     Spyglass
/// </summary>
public sealed class Spyglass : HermitRelicModel
{
    public Spyglass() : base(RelicRarity.Uncommon)
    {
        WithTip<ConcentrationPower>();
    }
}