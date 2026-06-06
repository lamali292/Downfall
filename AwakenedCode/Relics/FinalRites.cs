//using Awakened.AwakenedCode.Core;
  //using Awakened.AwakenedCode.CustomEnums;
  //using Awakened.AwakenedCode.Events;
  //using BaseLib.Utils;
  //using Downfall.DownfallCode.Commands;
  //using MegaCrit.Sts2.Core.Combat;
  //using MegaCrit.Sts2.Core.Entities.Cards;
  //using MegaCrit.Sts2.Core.Entities.Relics;
  //using MegaCrit.Sts2.Core.GameActions.Multiplayer;
  //using MegaCrit.Sts2.Core.Models;
  //using MegaCrit.Sts2.Core.Models.Powers;
  //using MegaCrit.Sts2.Core.Rooms;
  //
  //namespace Awakened.AwakenedCode.Relics;
  //
  //[Pool(typeof(AwakenedRelicPool))]
  //public class FinalRites : AwakenedRelicModel, IOnChant
  //{
  //    private int _chantedThisCombat;
  //
  //    public FinalRites() : base(RelicRarity.Rare)
  //    {
  //        WithPower<RitualPower>(1);
  //        WithTip(AwakenedTip.Chant);
  //    }
  //
  //    public override int DisplayAmount => _chantedThisCombat;
  //    public override bool ShowCounter => CombatManager.Instance.IsInProgress;
  //
  //    public async Task OnCardChanted(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay, bool firstTime)
  //    {
  //        if (card.Owner != Owner || !firstTime || _chantedThisCombat >= 3) return;
  //        _chantedThisCombat++;
  //        InvokeDisplayAmountChanged();
  //        if (_chantedThisCombat < 3) return;
  //        Flash();
  //        await MyCommonActions.ApplySelf<RitualPower>(ctx, this);
  //    }
  //
  //
  //    public override Task BeforeCombatStart()
  //    {
  //        InvokeDisplayAmountChanged();
  //        return Task.CompletedTask;
  //    }
  //
  //    public override Task AfterCombatEnd(CombatRoom _)
  //    {
  //        InvokeDisplayAmountChanged();
  //        return Task.CompletedTask;
  //    }
  //}