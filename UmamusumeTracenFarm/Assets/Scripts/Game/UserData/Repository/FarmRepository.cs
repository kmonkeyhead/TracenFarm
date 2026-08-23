using Game.UserData.Model;

namespace Game.UserData.Repository
{
    public class FarmRepository : GenericRepository<int, FarmModel>
    {
        public FarmRepository() : base(model => model.Id)
        {
        }
    }
}