using Downfall.Code.Abstract;
using Downfall.Code.Cards.Awakened.Uncommon;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Powers.Awakened;

public class SiphonPower : AwakenedPowerModel
{

  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;
  
  public override LocString Title => ModelDb.Card<Siphon>().TitleLocString;
  public override LocString Description => new("powers",  "TEMPORARY_STRENGTH_POWER.description");
  protected override string SmartDescriptionLocKey =>  "TEMPORARY_STRENGTH_POWER.smartDescription" ;

  protected override IEnumerable<IHoverTip> ExtraHoverTips => [
    HoverTipFactory.FromCard<Siphon>(),
    HoverTipFactory.FromPower<StrengthPower>()
  ];
  

  public override async Task BeforeApplied(
    Creature target,
    decimal amount,
    Creature? applier,
    CardModel? cardSource)
  {
    await PowerCmd.Apply<StrengthPower>(target, amount, applier, cardSource, true);
  }

  public override async Task AfterPowerAmountChanged(
    PowerModel power,
    decimal amount,
    Creature? applier,
    CardModel? cardSource)
  {
    if (power != this || amount == Amount)
      return;
    
    await PowerCmd.Apply<StrengthPower>(Owner, amount, applier, cardSource, true);
  }

  public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
  {
    if (side != Owner.Side)
      return;

    Flash();
    var amountSnapshot = Amount; // capture before Remove zeroes it
    await PowerCmd.Remove(this);
    await PowerCmd.Apply<StrengthPower>(Owner, -amountSnapshot, Owner, null);
  }
}