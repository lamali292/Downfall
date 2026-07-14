using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Localization;

namespace Downfall.DownfallCode.Abstract;

public class DownfallCardModifier : CardModifier, ICustomModel
{
    public virtual bool ShouldGlowGold => false;
    
    protected LocString Description => new("card_modifiers", Id.Entry + ".description");
}


