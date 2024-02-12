using HalloDoc_BAL.Repository;
using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Interface
{
    public interface IRConciergeRepository
    {
        void Add(RConcierge rconcierge);
    }
}