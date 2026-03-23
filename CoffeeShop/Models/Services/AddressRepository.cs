using CoffeeShop.Data;
using CoffeeShop.Models.Interfaces;

namespace CoffeeShop.Models.Services
{
    public class AddressRepository : IAddressRepository
    {
        private readonly CoffeeShopDbContext dbContext;

        public AddressRepository(CoffeeShopDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public IEnumerable<Address> GetUserAddresses(string userId)
        {
            return dbContext.Addresses
                .Where(a => a.UserID == userId)
                .OrderByDescending(a => a.IsDefault)
                .ThenByDescending(a => a.CreatedDate)
                .ToList();
        }

        public Address? GetDefaultAddress(string userId)
        {
            return dbContext.Addresses
                .FirstOrDefault(a => a.UserID == userId && a.IsDefault);
        }

        public Address? GetAddressById(int addressId)
        {
            return dbContext.Addresses.Find(addressId);
        }

        public Address AddAddress(Address address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            address.CreatedDate = DateTime.Now;

            // Nëse kjo është adresa e parë, vendose si default
            if (!dbContext.Addresses.Any(a => a.UserID == address.UserID))
            {
                address.IsDefault = true;
            }

            dbContext.Addresses.Add(address);
            dbContext.SaveChanges();

            return address;
        }

        public Address UpdateAddress(Address address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            var existingAddress = dbContext.Addresses.Find(address.AddressID);

            if (existingAddress != null)
            {
                existingAddress.FirstName = address.FirstName;
                existingAddress.LastName = address.LastName;
                existingAddress.Phone = address.Phone;
                existingAddress.StreetAddress = address.StreetAddress;
                existingAddress.City = address.City;
                existingAddress.PostalCode = address.PostalCode;
                existingAddress.Country = address.Country;

                dbContext.SaveChanges();
            }

            return existingAddress;
        }

        public void DeleteAddress(int addressId)
        {
            var address = dbContext.Addresses.Find(addressId);

            if (address != null)
            {
                dbContext.Addresses.Remove(address);
                dbContext.SaveChanges();
            }
        }

        public void SetDefaultAddress(string userId, int addressId)
        {
            // Largo default nga të gjitha adresat e tjera
            var userAddresses = dbContext.Addresses.Where(a => a.UserID == userId).ToList();
            foreach (var addr in userAddresses)
            {
                addr.IsDefault = false;
            }

            // Vendos adresën e re si default
            var newDefaultAddress = dbContext.Addresses.Find(addressId);
            if (newDefaultAddress != null && newDefaultAddress.UserID == userId)
            {
                newDefaultAddress.IsDefault = true;
            }

            dbContext.SaveChanges();
        }
    }
}