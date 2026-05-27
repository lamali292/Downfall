using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Interfaces;

public interface IHasPyre
{
    CardModel? PyredCard { get; set; }
}