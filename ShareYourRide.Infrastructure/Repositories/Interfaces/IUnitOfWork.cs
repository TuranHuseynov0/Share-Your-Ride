using ShareYourRide.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Vehicle> Vehicles { get; }
        IGenericRepository<VehicleImage> VehicleImages { get; }
        IGenericRepository<Stop> Stops { get; }
        IGenericRepository<Trajectory> Trajectories { get; }
        IGenericRepository<RideApplication> RideApplications { get; }
        IGenericRepository<Wallet> Wallets { get; }
        IGenericRepository<WalletTransaction> WalletTransactions { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
