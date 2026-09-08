using System;
using DataType;

namespace Game.UserData.Model
{
    public record VegetableModel(string UniqueId, FarmId FarmId, DateTime StartAt, DateTime EndAt) : IModel
    {
        public DateTime UpdateAt { get; } = DateTime.Now;
    }
}