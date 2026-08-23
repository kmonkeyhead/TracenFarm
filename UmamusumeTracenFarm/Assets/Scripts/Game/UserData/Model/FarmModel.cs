using System;

namespace Game.UserData.Model
{
    public record FarmModel(int Id, int Value) : IModel
    {
        public DateTime UpdateAt { get; } = DateTime.Now;
    }
}