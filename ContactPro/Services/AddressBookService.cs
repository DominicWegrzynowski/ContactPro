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

                 Contact? contact = _context.Contacts
                              .Include(c => c.Categories)
                              .FirstOrDefault(c => c.Id == contactId);

                //Get categories from contact and return them.
                List<Category> categories = contact.Categories.ToList();

                return categories;
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
                Contact? contact = _context.Contacts
                                           .Include(c => c.Categories)
                                           .FirstOrDefault(c => c.Id == contactId);

                List<Category> categories = contact.Categories.ToList();

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
            List<Category> categories = new List<Category>();
            try
            {
                categories = await _context.Categories.Where(c => c.AppUserId == userId)
                                                      .OrderBy(c => c.Name)
                                                      .ToListAsync();

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
                //Go to contacts table,
                //search for all contacts where one of their properties
                //contain the searchString and they have a matching userId
                IEnumerable<Contact>? contacts = 
                await _context.Contacts.Where(c => (c.FirstName.Contains(searchString) && c.AppUserId == userId) ||
                                                   (c.LastName.Contains(searchString) && c.AppUserId == userId) ||
                                                   (c.Email.Contains(searchString) && c.AppUserId == userId) ||
                                                   (c.ZipCode.Contains(searchString) && c.AppUserId == userId) ||
                                                   (c.PhoneNumber.Contains(searchString) && c.AppUserId == userId) ||
                                                   (c.City.Contains(searchString) && c.AppUserId == userId) ||
                                                   (c.State.ToString().Contains(searchString) && c.AppUserId == userId))
                                                   .ToListAsync();
                return contacts;
            }
            catch (Exception)
            {
                throw;
            }
        } 
        #endregion
    }
}
