using Microsoft.EntityFrameworkCore;

namespace EFLinqWelcomeApp
{
    class Company
    {
        public int Id { set; get; }
        public string Title { set; get; }
        public List<Employe> Employes { set; get; }
    }

    class Employe
    {
        public int Id { set; get; }
        public string Name { set; get; }
        public Company? Company { set; get; }
        public int CompanyId { set; get; }
    }

    class AppContext : DbContext
    {
        public DbSet<Employe> Employes { set; get; }
        public DbSet<Company> Companies { set; get; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CompanyDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }
    }
    internal class Program
    {
        static void AddData()
        {
            using(AppContext context = new())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                Company[] companies = new Company[]
                {
                    new(){ Title = "Yandex" },
                    new(){ Title = "Mail Group" },
                    new(){ Title = "Ozon" },
                };

                Employe[] employes = new Employe[]
                {
                    new(){ Name = "Bob", Company = companies[0] },
                    new(){ Name = "Tim", Company = companies[1] },
                    new(){ Name = "Jim", Company = companies[2] },
                    new(){ Name = "Sam", Company = companies[0] },
                    new(){ Name = "Tom", Company = companies[2] },
                    new(){ Name = "Leo", Company = companies[0] },
                    new(){ Name = "Joe", Company = companies[1] },
                };

                context.Employes.AddRange(employes);
                context.Companies.AddRange(companies);

                context.SaveChanges();
            }
        }
        static void Main(string[] args)
        {
            //AddData();

            using(AppContext context = new())
            {
                var employes = (from employe in context.Employes
                                                      .Include(e => e.Company)
                               where employe.CompanyId == 1
                               select employe).ToList();

                foreach(var employe in employes)
                    Console.WriteLine(employe.Name);
            }
        }
    }
}