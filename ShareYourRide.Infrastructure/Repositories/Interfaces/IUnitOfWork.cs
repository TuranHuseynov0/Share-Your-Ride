using ShareYourRide.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace ShareYourRide.Infrastructure.Repositories.Interfaces
{
    public interface IUnitOfWork : System.IDisposable
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<Vehicle> Vehicles { get; }
        IGenericRepository<VehicleImage> VehicleImages { get; }
        IGenericRepository<Stop> Stops { get; }
        IGenericRepository<Trajectory> Trajectories { get; }
        IGenericRepository<TrajectoryWaypoint> TrajectoryWaypoints { get; }   // YENİ yer
        IGenericRepository<RideApplication> RideApplications { get; }
        IGenericRepository<Wallet> Wallets { get; }
        IGenericRepository<WalletTransaction> WalletTransactions { get; }
        IGenericRepository<VehicleBrand> VehicleBrands { get; }
        IGenericRepository<VehicleModel> VehicleModels { get; }
        IGenericRepository<VehicleColor> VehicleColors { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}