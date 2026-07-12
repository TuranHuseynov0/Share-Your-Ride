using ShareYourRide.Domain.Entities;
using ShareYourRide.Infrastructure.Data;
using ShareYourRide.Infrastructure.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Repositories.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public IGenericRepository<User> Users { get; }
        public IGenericRepository<Vehicle> Vehicles { get; }
        public IGenericRepository<VehicleImage> VehicleImages { get; }
        public IGenericRepository<Stop> Stops { get; }
        public IGenericRepository<Trajectory> Trajectories { get; }
        public IGenericRepository<RideApplication> RideApplications { get; }
        public IGenericRepository<Wallet> Wallets { get; }
        public IGenericRepository<WalletTransaction> WalletTransactions { get; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Users = new GenericRepository<User>(context);
            Vehicles = new GenericRepository<Vehicle>(context);
            VehicleImages = new GenericRepository<VehicleImage>(context);
            Stops = new GenericRepository<Stop>(context);
            Trajectories = new GenericRepository<Trajectory>(context);
            RideApplications = new GenericRepository<RideApplication>(context);
            Wallets = new GenericRepository<Wallet>(context);
            WalletTransactions = new GenericRepository<WalletTransaction>(context);
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
