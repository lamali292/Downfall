using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Abstract;

public class PowerTooltipSource(Func<PowerModel, IHoverTip> tip) : AbstractTooltipSource<PowerModel>(tip);

public class AbstractTooltipSource<T>(Func<T, IHoverTip> tip)
    where T : AbstractModel
{
    public IHoverTip Tip(T model)
    {
        return tip(model);
    }

    public static implicit operator AbstractTooltipSource<T>(Type t)
    {
        if (t.IsAssignableTo(typeof(PowerModel)))
            return new AbstractTooltipSource<T>(_ =>
                HoverTipFactory.FromPower(ModelDb.GetById<PowerModel>(ModelDb.GetId(t))));
        if (t.IsAssignableTo(typeof(CardModel)))
            return new AbstractTooltipSource<T>(_ =>
                HoverTipFactory.FromCard(ModelDb.GetById<CardModel>(ModelDb.GetId(t))));
        return t.IsAssignableTo(typeof(PotionModel))
            ? new AbstractTooltipSource<T>(_ =>
                HoverTipFactory.FromPotion(ModelDb.GetById<PotionModel>(ModelDb.GetId(t))))
            : throw new Exception($"Unable to generate hovertip from type {t}");
    }

    public static implicit operator AbstractTooltipSource<T>(CardKeyword keyword)
    {
        return new AbstractTooltipSource<T>(_ => HoverTipFactory.FromKeyword(keyword));
    }

    public static implicit operator AbstractTooltipSource<T>(StaticHoverTip staticTip)
    {
        return new AbstractTooltipSource<T>(_ => HoverTipFactory.Static(staticTip));
    }
}