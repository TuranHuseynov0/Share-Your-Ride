using ShareYourRide.Domain.Entities;
using ShareYourRide.Infrastructure.Data;
using ShareYourRide.Infrastructure.Repositories.Interfaces;
using System.Threading;
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
        public IGenericRepository<TrajectoryWaypoint> TrajectoryWaypoints { get; }
        public IGenericRepository<RideApplication> RideApplications { get; }
        public IGenericRepository<Wallet> Wallets { get; }
        public IGenericRepository<WalletTransaction> WalletTransactions { get; }
        public IGenericRepository<VehicleBrand> VehicleBrands { get; }
        public IGenericRepository<VehicleModel> VehicleModels { get; }
        public IGenericRepository<VehicleColor> VehicleColors { get; }
        public IGenericRepository<Review> Reviews { get; }
        public IGenericRepository<ChatThread> ChatThreads { get; }
        public IGenericRepository<ChatMessage> ChatMessages { get; }


        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Users = new GenericRepository<User>(context);
            Vehicles = new GenericRepository<Vehicle>(context);
            VehicleImages = new GenericRepository<VehicleImage>(context);
            Stops = new GenericRepository<Stop>(context);
            Trajectories = new GenericRepository<Trajectory>(context);
            TrajectoryWaypoints = new GenericRepository<TrajectoryWaypoint>(context);
            RideApplications = new GenericRepository<RideApplication>(context);
            Wallets = new GenericRepository<Wallet>(context);
            WalletTransactions = new GenericRepository<WalletTransaction>(context);
            VehicleBrands = new GenericRepository<VehicleBrand>(context);
            VehicleModels = new GenericRepository<VehicleModel>(context);
            VehicleColors = new GenericRepository<VehicleColor>(context);
            Reviews = new GenericRepository<Review>(context);
            ChatThreads = new GenericRepository<ChatThread>(context);
            ChatMessages = new GenericRepository<ChatMessage>(context);
        }

        public void Dispose() => _context.Dispose();

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await _context.SaveChangesAsync(cancellationToken);
    }
}