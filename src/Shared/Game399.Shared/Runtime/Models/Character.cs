using System;

namespace Game399.Shared.Models
{
    public class Character
    {
        public string Name { get; set; }
        public ObservableValue<int> Health { get; } = new();
        public ObservableValue<int> Damage { get; } = new();
        public ObservableValue<int> Armor { get; } = new();
    }
}