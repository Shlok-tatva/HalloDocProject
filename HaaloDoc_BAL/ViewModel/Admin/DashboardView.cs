﻿using HalloDoc_DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDoc_BAL.ViewModel.Admin
{
    public class DashboardView
    {
        public List<Region> regions { get; set; }

        public List<Casetag> casetags {  get; set; }
    }
}