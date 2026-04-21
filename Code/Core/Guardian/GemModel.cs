using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Core.Guardian;

public abstract class GemModel : AbstractModel, ICustomModel
{
    public override bool ShouldReceiveCombatHooks => true;

    private string IconName => Id.Entry
        .RemovePrefix()
        .ToLowerInvariant();

    public string IconPath => $"{IconName}.png".GemPath();
    public LocString TitleLocString => new("gems", Id.Entry + ".title");
    public LocString Description => new("gems", Id.Entry + ".description");

    public virtual IEnumerable<IHoverTip> ExtraHoverTips => [];
    public abstract Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay);
}