using System.Globalization;
using Champ.ChampCode.Core;
using Champ.ChampCode.Events;
using Champ.ChampCode.Stance;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.DynamicVars;

public class BerserkerSkillVar(decimal baseAmount) : DynamicVar("BerserkerSkill", baseAmount)
{
    private ChampStanceModel Stance => (ChampStanceModel)_owner!;

    public decimal Calculate()
    {
        if (!CombatManager.Instance.IsInProgress || !Stance.IsMutable)
            return _baseValue;
        var result =  ChampHook.ModifySkillBonus<VigorPower>(Stance.CombatState, Stance, (int)_baseValue);
        PreviewValue = result;
        return result;
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        PreviewValue = Calculate();
    }

    protected override decimal GetBaseValueForIConvertible() => Calculate();
    public override string ToString() => Calculate().ToString(CultureInfo.InvariantCulture);
}
