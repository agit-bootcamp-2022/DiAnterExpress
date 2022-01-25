using DiAnterExpress.Models;
using System;
using System.Linq;

namespace DiAnterExpress.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Branches.Any())
            {
                return;
            }

            var branches = new Branch[]
            {
                new Branch{Name ="Di Anter Express Pondok Indah",Address="Jln. Pondok Indah Ruko No.19",City="Jakarta",Phone=0812345679},
                new Branch{Name ="Di Anter Express Kalimalang",Address="Jln. Kalimalang Ruko No.20",City="Jakarta",Phone=0812345678},
                new Branch{Name ="Di Anter Express Pasar Senen",Address="Jln. Pasar Senen Ruko No.21",City="Jakarta",Phone=0812345677},
                new Branch{Name ="Di Anter Express Taman Mini",Address="Jln. Taman Mini Ruko No.22",City="Jakarta",Phone=0812345676},
                new Branch{Name ="Di Anter Express Tanah Abang",Address="Jln. Tanah Abang Ruko No.23",City="Jakarta",Phone=0812345675}           
            };

            foreach (var b in branches)
            {
                context.Branches.Add(b);
            }

            context.SaveChanges();
        }
    }
}
