using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.ViewModel.Admin
{
    public class CreateEditAccessView
    {
        public int roleId { get; set; }
        public string roleName { get; set; }

        public int accoutType {  get; set; }

        public string accessTypeString { get; set; }

        public int[] menusforRole { get; set; }

        public List<Menu> allmenus { get; set; }

    }
}
