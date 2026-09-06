using System;

namespace Game.UserData.Model
{
    public record VegetableModel(string UniqueId, int FarmId, DateTime StartAt, DateTime EndAt) : IModel
    {
        public DateTime UpdateAt { get; } = DateTime.Now;
    }
}