using ContactPro.Data;
using ContactPro.Models;
using ContactPro.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContactPro.Services
{
    public class AddressBookService : IAddressBookService
    {

        #region Fields
        private readonly ApplicationDbContext _context; 
        #endregion

        #region Constructor
        public AddressBookService(ApplicationDbContext context)
        {
            _context = context;
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
        public Task<ICollection<Category>> GetContactCategoriesAsync(int contactId)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Get Contact Category Ids
        public Task<ICollection<int>> GetContactCategoryIdsAsync(int contactId)
        {
            throw new NotImplementedException();
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
        public Task RemoveContactFromCategory(int categoryId, int contactId)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Search for Contacts
        public IEnumerable<Contact> SearchForContacts(string searchString, string userId)
        {
            throw new NotImplementedException();
        } 
        #endregion
    }
}
