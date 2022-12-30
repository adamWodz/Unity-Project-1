﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.GameplayControl
{
    public class Mission
    {
        public Planet start { get; set; }
        public Planet end { get; set; }
        public int points;
        public bool IsCompletedByPlayer()
        {
            return ConnectedPlanets.ArePlanetsInOneGroup(PlayerGameData.groupsOfConnectedPlanets, start, end);
        }
    }
}