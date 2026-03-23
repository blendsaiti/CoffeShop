namespace CoffeeShop.Models.Interfaces
{
    public interface IAddressRepository
    {
        // Merr të gjitha adresat e përdoruesit
        IEnumerable<Address> GetUserAddresses(string userId);

        // Merr adresën default të përdoruesit
        Address? GetDefaultAddress(string userId);

        // Merr një adresë specifike
        Address? GetAddressById(int addressId);

        // Shto adresë të re
        Address AddAddress(Address address);

        // Përditëso adresën
        Address UpdateAddress(Address address);

        // Fshi adresën
        void DeleteAddress(int addressId);

        // Vendos si default
        void SetDefaultAddress(string userId, int addressId);
    }
}