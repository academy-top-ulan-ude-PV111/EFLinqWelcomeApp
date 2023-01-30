using Microsoft.EntityFrameworkCore;

namespace EFLinqWelcomeApp
{
    class Country
    {
        public int Id { set; get; }
        public string Title { set; get; } = null!;
        List<Company> Companies { set; get; } = new();
    }
    class Company
    {
        public int Id { set; get; }
        public string Title { set; get; } = null!;
        public Country? Country { set; get; }
        public int CountryId { set; get; }
        public List<Employe> Employes { set; get; } = new();
    }

    class Position
    {
        public int Id { set; get; }
        public string Title { set; get; } = null!;
        List<Employe> Employes { set; get; } = new();
    }

    class Employe
    {
        public int Id { set; get; }
        public string Name { set; get; } = null!;
        public Company? Company { set; get; }
        public int CompanyId { set; get; }
        public Position? Position { set; get; }
        public int PositionId { set; get; }

    }

    class AppContext : DbContext
    {
        public DbSet<Employe> Employes { set; get; }
        public DbSet<Company> Companies { set; get; }
        public DbSet<Country> Countries { set; get; }
        public DbSet<Position> Positions { set; get; }
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

                Country[] countries = new Country[]
                {
                    new(){ Title = "Russia" },
                    new(){ Title = "Usa" },
                    new(){ Title = "China" },
                };

                Company[] companies = new Company[]
                {
                    new(){ Title = "Yandex", Country = countries[0] },
                    new(){ Title = "Google", Country = countries[1] },
                    new(){ Title = "Mail Group", Country = countries[0] },
                    new(){ Title = "Ozon", Country = countries[0] },
                    new(){ Title = "Xiaomi", Country = countries[2] },
                    new(){ Title = "Amazon", Country = countries[1] },
                };

                Position[] positions = new Position[]
                {
                    new(){ Title = "Developer"},
                    new(){ Title = "Manager"},
                    new(){ Title = "Tester"},
                };

                Employe[] employes = new Employe[]
                {
                    new(){ Name = "Bob", Company = companies[0], Position = positions[0] },
                    new(){ Name = "Tim", Company = companies[1], Position = positions[1] },
                    new(){ Name = "Jim", Company = companies[2], Position = positions[0] },
                    new(){ Name = "Sam", Company = companies[0], Position = positions[1] },
                    new(){ Name = "Tom", Company = companies[2], Position = positions[0] },
                    new(){ Name = "Leo", Company = companies[0], Position = positions[2] },
                    new(){ Name = "Joe", Company = companies[1], Position = positions[0] },
                    new(){ Name = "Max", Company = companies[3], Position = positions[0] },
                    new(){ Name = "Ann", Company = companies[4], Position = positions[0] },
                    new(){ Name = "Ben", Company = companies[3], Position = positions[1] },
                    new(){ Name = "Poul", Company = companies[4], Position = positions[2] },
                    new(){ Name = "Don", Company = companies[5], Position = positions[0] },
                    new(){ Name = "Mall", Company = companies[5], Position = positions[1] },
                    new(){ Name = "Rick", Company = companies[5], Position = positions[2] },

                };

                context.Countries.AddRange(countries);
                context.Positions.AddRange(positions);
                context.Companies.AddRange(companies);
                context.Employes.AddRange(employes);

                context.SaveChanges();
            }
        }
        static void LinqFirst()
        {
            using (AppContext context = new())
            {
                // linq operations
                //var employes = (from employe in context.Employes
                //                                      .Include(e => e.Company)
                //                where employe.CompanyId == 1
                //                select employe).ToList();

                // linq methods
                var employes = context.Employes
                                      .Include(e => e.Company)
                                      .Where(e => e.CompanyId == 1);

                foreach (var employe in employes)
                    Console.WriteLine($"{employe.Name} {employe?.Company?.Title}");
            }
        }
        static void LinqWhereFirstLast()
        {
            using (AppContext context = new())
            {
                // operations
                //var employes = (from employe in context.Employes.Include(e => e.Company)
                //               where employe.Company!.Title != "Google"
                //               select employe).ToList();
                // methods
                var employes = context.Employes
                                      .Include(e => e.Company)
                                      //.Where(e => e.Company!.Title != "Google");
                                      .Where(e => EF.Functions.Like(e.Company.Title!, "%Google%"));

                foreach (var employe in employes)
                    Console.WriteLine($"{employe.Name} {employe?.Company?.Title}");

                Employe employe1 = context.Employes
                                      .Include(e => e.Company)
                                      //.Where(e => e.Company!.Title != "Google");
                                      .Where(e => EF.Functions.Like(e.Company.Title!, "%Google%"))
                                      .FirstOrDefault()!;
                Console.WriteLine(employe1.Name + " " + employe1.Company.Title);
            }
        }
        static void LinqSelectOrderBy()
        {
            using (AppContext context = new())
            {
                // methods
                //var employes = context.Employes
                //                      .OrderBy(e => e.Company!.Title)
                //                        .ThenBy(e => e.Name)
                //                      //.OrderByDescending(e => e.Company!.Title)
                //                      .Select(e => new
                //                      {
                //                          Name = e.Name,
                //                          CompanyName = e.Company!.Title
                //                      });
                //                      //.OrderBy(e => e.CompanyName);

                // operations
                var employes = from employe in context.Employes
                               orderby employe.Company!.Title descending, employe.Name
                               select new
                               {
                                   Name = employe.Name,
                                   CompanyName = employe.Company!.Title
                               };


                foreach (var employe in employes)
                    Console.WriteLine($"{employe.Name} {employe.CompanyName}");
            }
        }
        static void LinqJoin()
        {
            using (AppContext context = new())
            {
                // methods
                //var employes = context.Employes.Include(e => e.Position)
                //                     .Join(context.Companies,
                //                     e => e.CompanyId,
                //                     c => c.Id,
                //                     (e, c) => new { 
                //                         Name = e.Name,
                //                         Position = e.Position!.Title,
                //                         CompanyName = c.Title
                //                     });

                // operations
                //var employes = from employe in context.Employes.Include(e => e.Position)
                //               join company in context.Companies
                //                on employe.CompanyId equals company.Id
                //               orderby company.Title, employe.Position
                //               select new {
                //                   Name = employe.Name,
                //                   Position = employe.Position!.Title,
                //                   CompanyName = company.Title
                //               };

                //foreach (var item in employes)
                //    Console.WriteLine($"{item.Name} {item.Position} {item.CompanyName}");


                // operations
                //var employes = from e in context.Employes
                //               join c in context.Companies on e.CompanyId equals c.Id
                //               join cnt in context.Countries on c.CountryId equals cnt.Id
                //               orderby cnt.Title, c.Title
                //               select new
                //               {
                //                   Name = e.Name,
                //                   Company = c.Title,
                //                   Country = cnt.Title
                //               };

                // methods
                //var employes = context.Employes
                //                      .Join(context.Companies,
                //                      e => e.CompanyId,
                //                      c => c.Id,
                //                      (e, c) => new { CountryId = c.CountryId, Name = e.Name, Company = c.Title })
                //                      .Join(context.Countries,
                //                      a => a.CountryId,
                //                      cnt => cnt.Id,
                //                      (a, cnt) => new
                //                      {
                //                          Name = a.Name,
                //                          Company = a.Company,
                //                          Country = cnt.Title
                //                      })
                //                      .OrderBy(a => a.Country)
                //                        .ThenBy(a => a.Company);


                //foreach (var item in employes)
                //    Console.WriteLine($"{item.Name}\t{item.Country}\t{item.Company}");

            }
        }
        static void LinqGroupBy()
        {
            using (AppContext context = new())
            {
                // operations
                //var companies = from e in context.Employes
                //                group e by e.Company!.Title into c
                //                select new
                //                {
                //                    c.Key,
                //                    Count = c.Count()
                //                };

                // methods
                var companies = context.Employes
                                       .GroupBy(e => e.Company!.Title)
                                       .Select(c => new
                                       {
                                           c.Key,
                                           Count = c.Count()
                                       }); ;


                foreach (var item in companies)
                    Console.WriteLine($"{item.Key}\t{item.Count}");

            }
        }
        static void Main(string[] args)
        {
            //AddData();

            //LinqFirst();
            //LinqWhereFirstLast();
            //LinqSelectOrderBy();
            //LinqJoin();
            //LinqGroupBy();

            using (AppContext context = new())
            {
                string company = "Rambler";
                //bool result = context.Employes.Any(e => e.Company!.Title == company);

                //bool result = context.Employes.All(e => e.Company != null);

                //if (result)
                //    Console.WriteLine("Yes");
                //else
                //    Console.WriteLine("No");

                int employesCount = context.Employes.Count();
                int yandexCount = context.Employes
                                         .Where(e => e.Company!.Title == "Yandex")
                                         .Count();

                int minLength = context.Companies.Min(c => c.Title.Length);
                int maxLength = context.Companies
                                       .Where(c => c.Country!.Title == "Russia")
                                       .Max(c => c.Title.Length);

                double avgLength = context.Companies.Average(c => c.Title.Length);

                int totalLength = context.Companies.Sum(c => c.Title.Length);

                Console.WriteLine(employesCount);
                Console.WriteLine(yandexCount);
                Console.WriteLine(minLength);
                Console.WriteLine(maxLength);
                Console.WriteLine(avgLength);
                Console.WriteLine(totalLength);
            }


        }
    }
}