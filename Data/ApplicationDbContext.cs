using System;
using System.Collections.Generic;
using System.Text;
using Api_CasaDeShow.Models;
using Microsoft.EntityFrameworkCore;

namespace Api_CasaDeShow.Data
{
    public class ApplicationDbContext :DbContext
    {
        public DbSet<Local> Locais { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}