using ContactPro.Data;
using ContactPro.Models;
using ContactPro.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace ContactPro.Services
{
    public class AddressBookService : IAddressBookService
    {

        #region Fields
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        #endregion

        #region Constructor
        public AddressBookService(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        #endregion

        #region Add Contact to Category
        public async Task AddContactToCategoryAsync(int categoryId, int contactId)
        {
            try
            {
                //Check to see if category passed in is in the contact already
                if(!await IsContactInCategoryAsync(categoryId, contactId))
                {
                    Contact? contact = await _context.Contacts.FindAsync(contactId);
                    Category? category = await _context.Categories.FindAsync(categoryId);

                    if(category is not null && contact is not null)
                    {
                        category.Contacts.Add(contact);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Get Contact Categories
        public async Task<ICollection<Category>> GetContactCategoriesAsync(int contactId)
        {
            try
            {
                //Go to categories table, select * categories where c.contactId == contactId

                 Contact? contact = await _context.Contacts
                              .Include(c => c.Categories)
                              .FirstOrDefaultAsync(c => c.Id == contactId);

                //Get categories from contact and return them.
                List<Category> categories = contact!.Categories.ToList();

                return categories;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Get User Contacts
        public IEnumerable<Contact> GetAllUserContacts(string userId)
        {
            try
            {
                AppUser appUser = _context.Users.Find(userId)!;

                IEnumerable<Contact> contacts = appUser.Contacts.OrderBy(c => c.LastName)
                                                                    .ThenBy(c => c.FirstName)
                                                                    .ToList();

                return contacts;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Get User Contacts By Category
        public IEnumerable<Contact> GetUserContactsByCategory(string userId, int categoryId)
        {
            try
            {
                AppUser appUser = _context.Users.Find(userId)!;

                IEnumerable<Contact> contacts = appUser.Categories.FirstOrDefault(c => c.Id == categoryId)!
                                                       .Contacts.OrderBy(c => c.LastName)
                                                       .ThenBy(c => c.FirstName)
                                                       .ToList();

                return contacts;
            }
            catch (Exception)
            {
                throw;
            }
        } 
        #endregion

        #region Get Contact Category Ids
        public async Task<ICollection<int>> GetContactCategoryIdsAsync(int contactId)
        {
            try
            {
                Contact? contact = await _context.Contacts
                                           .Include(c => c.Categories)
                                           .FirstOrDefaultAsync(c => c.Id == contactId);

                List<Category> categories = contact!.Categories.ToList();

                List<int> categoryIds = new();

                foreach(var category in categories)
                {
                    categoryIds.Add(category.Id);
                }

                return categoryIds;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Get User Categories
        public async Task<IEnumerable<Category>> GetUserCategoriesAsync(string userId)
        {
            try
            {
                AppUser? appUser = await _context.Users
                                                 .Include(u => u.Contacts)
                                                    .ThenInclude(c => c.Categories)
                                                 .Include(u => u.Categories)
                                                 .FirstOrDefaultAsync(u => u.Id == userId);

                List<Category> categories = appUser!.Categories.OrderBy(c => c.Name).ToList();

                return categories;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Is Contact in Category?
        public async Task<bool> IsContactInCategoryAsync(int categoryId, int contactId)
        {
            try
            {
                Contact? contact = await _context.Contacts.FindAsync(contactId);

                if (contact is not null)
                {
                    return await _context.Categories
                                     .Include(c => c.Contacts)
                                     .Where(c => c.Id == categoryId && c.Contacts.Contains(contact))
                                     .AnyAsync();
                }
                else return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Remove Contact From Category
        public async Task RemoveContactFromCategory(int categoryId, int contactId)
        {
           throw new NotImplementedException();
        }
        #endregion

        #region Search for Contacts
        public async Task<IEnumerable<Contact>> SearchForContacts(string searchString, string userId)
        {
            try
            {
                if (!String.IsNullOrEmpty(searchString))
                {
                    //Go to contacts table,
                    //search for all contacts where one of their properties
                    //contain the searchString and they have a matching userId
                    IEnumerable<Contact>? contacts =
                    await _context.Contacts.Where(c => (c.FirstName!.ToLower().Contains(searchString.ToLower()) && c.AppUserId == userId) ||
                                                       (c.LastName!.ToLower().Contains(searchString.ToLower()) && c.AppUserId == userId) ||
                                                       (c.Email!.ToLower().Contains(searchString.ToLower()) && c.AppUserId == userId) ||
                                                       (c.Address1!.ToLower().Contains(searchString.ToLower()) && c.AppUserId == userId) ||
                                                       (c.Address2!.ToLower().Contains(searchString.ToLower()) && c.AppUserId == userId) ||
                                                       (c.City!.ToLower().Contains(searchString.ToLower()) && c.AppUserId == userId) ||
                                                       (c.PhoneNumber!.ToLower().Contains(searchString.ToLower()) && c.AppUserId == userId))
                                                       .ToListAsync();
                    return contacts; 
                }
                else
                {
                    return Enumerable.Empty<Contact>();
                }
            }
            catch (Exception)
            {
                throw;
            }
        } 
        #endregion
    }
}
