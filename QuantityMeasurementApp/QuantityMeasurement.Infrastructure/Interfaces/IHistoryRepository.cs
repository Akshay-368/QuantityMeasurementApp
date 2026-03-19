using QuantityMeasurement.Infrastructure.Models;

namespace QuantityMeasurement.Infrastructure.Interfaces;

public interface IHistoryRepository
{
    void Save(HistoryRecord history);

    List<HistoryRecord> GetHistory();

    void ClearHistory();
}