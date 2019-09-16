using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameFoRest
{
    public static class Parameters
    {
        public static double BonusTriggerTime = 250;
        public static int CellSpeed = 400;
        public static int PlayTime = 60;
        public static bool UseMap = true;
        public static readonly Shape[,] Map = new Shape[8, 8]{
            {Shape.A ,Shape.B ,Shape.A ,Shape.B ,Shape.A ,Shape.B ,Shape.A ,Shape.B},
            {Shape.C ,Shape.D ,Shape.C ,Shape.D ,Shape.C ,Shape.E ,Shape.C ,Shape.D},
            {Shape.A ,Shape.E ,Shape.A ,Shape.B ,Shape.A ,Shape.E ,Shape.A ,Shape.B},
            {Shape.C ,Shape.D ,Shape.E ,Shape.D ,Shape.C ,Shape.D ,Shape.E ,Shape.D},
            {Shape.A ,Shape.B ,Shape.E ,Shape.B ,Shape.A ,Shape.E ,Shape.A ,Shape.B},
            {Shape.C ,Shape.E ,Shape.C ,Shape.D ,Shape.C ,Shape.E ,Shape.C ,Shape.D},
            {Shape.A ,Shape.B ,Shape.E ,Shape.B ,Shape.A ,Shape.B ,Shape.A ,Shape.B},
            {Shape.C ,Shape.D ,Shape.C ,Shape.D ,Shape.C ,Shape.D ,Shape.C ,Shape.D}
        };
    }
}
