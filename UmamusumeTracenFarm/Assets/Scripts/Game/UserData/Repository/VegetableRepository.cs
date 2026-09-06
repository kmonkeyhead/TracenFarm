using Game.UserData.Model;

namespace Game.UserData.Repository
{
    public class VegetableRepository : GenericRepository<string, VegetableModel>
    {
        public VegetableRepository() : base(model => model.UniqueId)
        {
        }
    }
}