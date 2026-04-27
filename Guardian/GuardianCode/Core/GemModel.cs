using BaseLib.Abstracts;
using BaseLib.Extensions;
using Godot;
using Guardian.GuardianCode.Cards.Abstract;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Core;

public abstract class GemModel : AbstractModel, ICustomModel
{
    public override bool ShouldReceiveCombatHooks => true;

    private string IconName => Id.Entry
        .RemovePrefix()
        .ToLowerInvariant();

    public string IconPath => $"{IconName}.png".GemPath();
    public virtual Color GemColor => Colors.White;
    public virtual CardRarity Rarity => CardRarity.Common;
    public LocString TitleLocString => new("gems", Id.Entry + ".title");
    public LocString Description => new("gems", Id.Entry + ".description");

    public CardModel ToCard => ModelDb.CardPool<GuardianCardPool>().AllCards.OfType<IGemCard>()
        .Where(c => c.GemModel == this).Cast<CardModel>().First();

    public virtual IEnumerable<IHoverTip> ExtraHoverTips => [];
    public abstract Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay);
}