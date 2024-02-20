using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Interface
{
    public interface IRequestRepository
    {
        void Add(Request request);
        Request Get(int id);
        List<Request> GetAll(int userId);
        void Update(Request request);
        List<Request> GetRequestFromStatusId (int statusId);
        List<Request> GetAll();
    }
}
