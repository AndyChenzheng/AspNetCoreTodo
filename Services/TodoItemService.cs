using System;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreTodo.Data;
using AspNetCoreTodo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTodo.Services
{
    public class TodoItemService:ITodoItemService
    {
        //数据库上下文
        private readonly ApplicationDbContext _context;

        public TodoItemService(ApplicationDbContext context)
        {
            this._context = context;

        }

        public async Task<TodoItem[]> GetIncompleteItemAsync(IdentityUser user)
        {
            var items = await _context.Items
                .Where(x => x.IsDone == false)
                .Where(x=>x.UserId==user.Id)
                .ToArrayAsync();
            return items;
        }

        public async Task<bool> AddItemAsync(TodoItem newItem, IdentityUser user)
        {
            newItem.Id=Guid.NewGuid();
            newItem.IsDone = false;
            newItem.DueAt=DateTimeOffset.Now.AddDays(3);
            newItem.UserId = user.Id;
            _context.Items.Add(newItem);
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }

        public async Task<bool> MarkDoneAsync(Guid id, IdentityUser user)
        {
            var item = await _context.Items.Where(x => x.Id == id)
                .Where(x=>x.UserId==user.Id)
                .SingleOrDefaultAsync();

            if (item == null) return false;
            item.IsDone = true;
            var saveResult = await _context.SaveChangesAsync();
            return saveResult == 1;
        }
    }
}