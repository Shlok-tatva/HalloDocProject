using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.ViewModel.Menus
{
    public class MenuItem
    {
        public string Action { get; set; }
        public string DisplayName { get; set; }
    }

    public class Allmenus
    {
        public List<MenuItem> GetFilteredMenuItems(List<string> menus)
        {
            List<MenuItem> availableMenuItems = new List<MenuItem>
            {
                new MenuItem { Action = "Dashboard", DisplayName = "Dashboard" },
                new MenuItem { Action = "ProviderLocation", DisplayName = "Provider Location" },
                new MenuItem { Action = "AdminProfile", DisplayName = "My Profile" },
                new MenuItem { Action = "Provider", DisplayName = "Provider" },

                new MenuItem { Action = "Scheduling", DisplayName = "Scheduling" },
                new MenuItem { Action = "Access", DisplayName = "Account Access" },
                new MenuItem { Action = "UserAccess", DisplayName = "User Access" },
                new MenuItem { Action = "Records", DisplayName = "Records" },
                // Add more menu items as needed
            };

            List<MenuItem> filteredMenuItems = availableMenuItems.Where(item => menus.Contains(item.Action)).ToList();
            return filteredMenuItems;
        }
    }
}
