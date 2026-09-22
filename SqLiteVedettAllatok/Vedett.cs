using System;
using System.Collections.Generic;
using System.Text;

namespace SqLiteVedettAllatok
{
    internal class Vedett
    {
        public Vedett(int id, string nev, int ertek, int ev, string osztaly)
        {
            Id = id;
            Nev = nev;
            Ertek = ertek;
            Ev = ev;
            Osztaly = osztaly;
        }

        public int Id { get; set; }
        public string Nev { get; set; }
        public int Ertek { get; set; }
        public int Ev { get; set; }
        public string Osztaly { get; set; }
    }
}
