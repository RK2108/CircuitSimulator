using Microsoft.EntityFrameworkCore;

public class CircuitDbContext : DbContext
{
    public DbSet<Circuit> Circuits
    {
        get;
        set;
    }

    public DbSet<Component> Components
    {
        get;
        set;
    }

    public DbSet<Wire> Wires
    {
        get;
        set;
    }

    public CircuitDbContext(DbContextOptions<CircuitDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Circuit>().ToTable("Circuit");
        modelBuilder.Entity<Component>().ToTable("Component");
        modelBuilder.Entity<Wire>().ToTable("Wire");

        modelBuilder.Entity<Circuit>().HasKey(c => c.CircuitId);
        modelBuilder.Entity<Component>().HasKey(c => c.ComponentId);
        modelBuilder.Entity<Wire>().HasKey(w => w.WireId);

        modelBuilder.Entity<Component>().HasDiscriminator<string>("ComponentType")
                                        .HasValue<Resistor>("Resistor")
                                        .HasValue<Battery>("Battery")
                                        .HasValue<Lamp>("Lamp");

        base.OnModelCreating(modelBuilder);
    }
}