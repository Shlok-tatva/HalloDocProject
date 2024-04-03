using HalloDoc_BAL.Interface;
using HalloDoc_DAL.DataContext;
using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.Repository
{
    public class HealthProfessionalRepository : IHealthProfessionalRepository
    {
        private readonly ApplicationDbContext _context;

        public HealthProfessionalRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public Healthprofessional get(int id)
        {
            return _context.Healthprofessionals.FirstOrDefault(hp => hp.Vendorid == id);

        }
        public List<Healthprofessional> getByProfession(int professionId)
        {
            return _context.Healthprofessionals.Where(hp => hp.Isdeleted == false && (professionId == 0 || hp.Profession == (int)professionId)).ToList();
        }

        public void Add(Healthprofessional healthprofessional)
        {
            _context.Healthprofessionals.Add(healthprofessional);
            _context.SaveChanges();
        }
        public void Update(Healthprofessional healthprofessional)
        {
            _context.Healthprofessionals.Update(healthprofessional);
            _context.SaveChanges();
        }
        public void Delete(int id)
        {
            Healthprofessional healthprofessional = get(id);
            healthprofessional.Isdeleted = true;
            Update(healthprofessional);
        }

    }
}
