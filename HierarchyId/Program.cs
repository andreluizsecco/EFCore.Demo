using Microsoft.EntityFrameworkCore;

using (var db = new EmpresaContext())
{
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();

    db.Empregados.Add(new Empregado { Nome = "José", Hierarquia = HierarchyId.Parse("/"), DataAdmissao = new DateTime(2010, 1, 1) });
    db.Empregados.Add(new Empregado { Nome = "Paulo", Hierarquia = HierarchyId.Parse("/1/"), DataAdmissao = new DateTime(2011, 1, 1) });
    db.Empregados.Add(new Empregado { Nome = "João", Hierarquia = HierarchyId.Parse("/2/"), DataAdmissao = new DateTime(2012, 1, 1) });
    db.Empregados.Add(new Empregado { Nome = "Pedro", Hierarquia = HierarchyId.Parse("/1/1/"), DataAdmissao = new DateTime(2013, 1, 1) });
    db.Empregados.Add(new Empregado { Nome = "Maria", Hierarquia = HierarchyId.Parse("/2/1/"), DataAdmissao = new DateTime(2014, 1, 1) });
    db.Empregados.Add(new Empregado { Nome = "Fabio", Hierarquia = HierarchyId.Parse("/1/2/"), DataAdmissao = new DateTime(2015, 1, 1) });
    db.Empregados.Add(new Empregado { Nome = "Felipe", Hierarquia = HierarchyId.Parse("/1/3/"), DataAdmissao = new DateTime(2015, 1, 1) });
    db.Empregados.Add(new Empregado { Nome = "Mateus", Hierarquia = HierarchyId.Parse("/2/1/1/"), DataAdmissao = new DateTime(2016, 1, 1) });
    db.Empregados.Add(new Empregado { Nome = "Lucas", Hierarquia = HierarchyId.Parse("/1/3/1/"), DataAdmissao = new DateTime(2017, 1, 1) });
    //db.Empregados.Add(new Empregado { Nome = "Mateus", Hierarquia = HierarchyId.Parse("/1/-1/"), DataAdmissao = new DateTime(2016, 1, 1) });
    //db.Empregados.Add(new Empregado { Nome = "Mateus", Hierarquia = HierarchyId.Parse("/1/1.5/"), DataAdmissao = new DateTime(2016, 1, 1) });

    db.SaveChanges();
    
    // Listar por nível
    foreach (var nivel in Enumerable.Range(0, 5))
    {
        var empregados = await db.Empregados
            .Where(empregado => empregado.Hierarquia.GetLevel() == nivel)
            .ToListAsync();

        Console.WriteLine($"Empregado do nível {nivel}: {string.Join(", ", empregados.Select(e => e.Nome))}");
    }

    Console.WriteLine("----------------------");

    ImprimirArvoreDeEmpregados();

    Console.WriteLine("----------------------");

    #region CONSULTANDO HIERARQUIAS
    
    var nomeDoEmpregado = "Paulo";

    var hierarquiaDoEmpregado = db.Empregados
        .Single(empregado => empregado.Nome == nomeDoEmpregado).Hierarquia;
    
    // Obter o chefe direto de um empregado
    var chefe = await db.Empregados
        .SingleOrDefaultAsync(chefe => chefe.Hierarquia == db.Empregados
            .Single(empregado => empregado.Nome == nomeDoEmpregado).Hierarquia
            .GetAncestor(1));

    Console.WriteLine($"O Chefe direto do empregado {nomeDoEmpregado} é {chefe?.Nome}");

    Console.WriteLine("----------------------");

    // Obter o chefe do chefe de um empregado (Alternativa caso já tenha a hierarquia do empregado)
    var chefeDoChefe = await db.Empregados
        .SingleOrDefaultAsync(chefe => chefe.Hierarquia == hierarquiaDoEmpregado.GetAncestor(2));

    Console.WriteLine($"O Chefe do chefe do empregado {nomeDoEmpregado} é {chefeDoChefe?.Nome ?? "Ninguém"}");

    Console.WriteLine("----------------------");
    
    // Obter todos os chefes de um empregado
    var chefes = await db.Empregados
            .Where(chefe => db.Empregados
                .Single(empregado => empregado.Nome == nomeDoEmpregado && chefe.EmpregadoId != empregado.EmpregadoId)
                    .Hierarquia.IsDescendantOf(chefe.Hierarquia))
            .OrderByDescending(chefe => chefe.Hierarquia.GetLevel())
            .ToListAsync();

    Console.WriteLine($"Os chefes do {nomeDoEmpregado} são: {string.Join(", ", chefes.Select(e => e.Nome))}");

    Console.WriteLine("----------------------");

    // Obter os subordinados diretos de um chefe
    var subordinadosDiretos = await db.Empregados
            .Where(empregado => empregado.Hierarquia.GetAncestor(1) == db.Empregados
                .Single(empregado => empregado.Nome == nomeDoEmpregado).Hierarquia)
            .ToListAsync();

    Console.WriteLine($"Os subordinados diretos do chefe {nomeDoEmpregado} são: {string.Join(", ", subordinadosDiretos.Select(e => e.Nome))}");

    Console.WriteLine("----------------------");

    // Obter todos os subordinados de um chefe
    var subordinados = await db.Empregados
            .Where(empregado => empregado.Hierarquia.IsDescendantOf(
                db.Empregados
                    .Single(chefe => chefe.Nome == nomeDoEmpregado && chefe.EmpregadoId != empregado.EmpregadoId).Hierarquia))
            .OrderBy(empregado => empregado.Hierarquia.GetLevel())
            .ToListAsync();

    Console.WriteLine($"Os subordinados do chefe {nomeDoEmpregado} são: {string.Join(", ", subordinados.Select(e => e.Nome))}");

    #endregion

    Console.WriteLine("----------------------");
    
    #region ALTERANDO HIERARQUIAS
    
    var nomeDoEmpregadoAlterado = "Felipe";
    var nomeNovoChefeDoEmpregado = "José";

    var empregadoAlterado = db.Empregados
        .Single(empregado => empregado.Nome == nomeDoEmpregadoAlterado);

    var chefeAtual = await db.Empregados
        .SingleOrDefaultAsync(chefe => chefe.Hierarquia == empregadoAlterado.Hierarquia
            .GetAncestor(1));

    var novoChefe = db.Empregados
        .Single(empregado => empregado.Nome == nomeNovoChefeDoEmpregado);

    // Alterar a hierarquia de um empregado e seus subordinados
    var empregadoESubordinados = await db.Empregados
        .Where(empregado => empregado.Hierarquia.IsDescendantOf(empregadoAlterado.Hierarquia))
        .ToListAsync();

    foreach (var empregado in empregadoESubordinados)
        empregado.Hierarquia = empregado.Hierarquia.GetReparentedValue(chefeAtual?.Hierarquia, novoChefe.Hierarquia)!;

    var subordinadosDiretosAtuaisDoNovoChefe = await db.Empregados
        .Where(empregado => empregado.Hierarquia.GetAncestor(1) == novoChefe.Hierarquia)
        .OrderBy(empregado => empregado.Hierarquia.GetLevel())
        .ToListAsync();

    Console.WriteLine($"O empregado {nomeDoEmpregadoAlterado} e seus subordinados passaram a ser subordinados do: {nomeNovoChefeDoEmpregado}");

    ImprimirArvoreDeEmpregados();
    
    #endregion

    void ImprimirArvoreDeEmpregados()
    {
        var todosOsEmpregados = db.Empregados
            .OrderBy(e => e.Hierarquia)
            .ToList();

        Console.WriteLine("Árvore de empregados:");

        foreach (var empregado in todosOsEmpregados)
        {
            int nivel = empregado.Hierarquia.GetLevel();

            string prefixo = new string(' ', nivel * 4);
            string estrutura = nivel > 0 ? prefixo + "|--- " : "";

            Console.WriteLine(estrutura + empregado.Nome);
        }
    }
}

public class EmpresaContext : DbContext
{
    public DbSet<Empregado> Empregados { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=(localdb)\mssqllocaldb;Database=EFCore.Demo;Trusted_Connection=True;",
            s => s.UseHierarchyId());
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Empregado>().Property(e => e.Nome).HasColumnType("varchar(255)");
        modelBuilder.Entity<Empregado>().Property(e => e.DataAdmissao).HasColumnType("date");
    }
}

public class Empregado
{
    public int EmpregadoId { get; set; }
    public HierarchyId Hierarquia { get; set; }
    public string Nome { get; set; }
    public DateTime DataAdmissao { get; set; }
}