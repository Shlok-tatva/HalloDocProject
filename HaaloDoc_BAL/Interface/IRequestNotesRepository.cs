using HalloDoc_DAL.Models;

namespace HalloDoc_BAL.Interface
{
    public interface IRequestNotesRepository
    {
        public void Add(Requestnote note);
        Requestnote Get(int requestid);
        void Update(Requestnote note);
    }
}