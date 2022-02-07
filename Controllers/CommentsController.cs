using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheBlogProject.Data;
using TheBlogProject.Models;

namespace TheBlogProject.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BlogUser> _userManager;

        public CommentsController(ApplicationDbContext context, UserManager<BlogUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Comments

        public async Task<IActionResult> OriginalIndex()
        {
            var originalComments = await _context.Comments.ToListAsync();
            return View("Index", originalComments);
        }

        public async Task<IActionResult> ModeratedIndex()
        {
            var moderatedComments = await _context.Comments.Where(c => c.Moderated != null).ToListAsync();
            return View("Index", moderatedComments);
        }

        //public async Task<IActionResult> DeletedIndex()
        //{
        //    var deletedComments = await _context.Comments.ToListAsync();
        //    return View("Index", deletedComments);
        //}

        public async Task<IActionResult> Index()
        {
            if (!User.IsInRole("Administrator"))
            {
                return RedirectToAction("AdminError", "Home");
            }

            else
            {
                var applicationDbContext = _context.Comments.Include(c => c.BlogUser).Include(c => c.Moderator).Include(c => c.Post);
                return View(await applicationDbContext.ToListAsync());
            }
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound("id null");
            }

            var comment = await _context.Comments
                .Include(c => c.BlogUser)
                .Include(c => c.Moderator)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound("comment null");
            }

            return View(comment);
        }

        //GET: Comments/Create
        //public IActionResult Create()
        //{
        //    ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id");
        //    ViewData["ModeratorId"] = new SelectList(_context.Users, "Id", "Id");
        //    ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Abstract");
        //    return View();
        //}

        // POST: Comments/Create-the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Body,Post")] Comment comment)
        //public async Task<IActionResult> Create(int id, string slug)
        {
            if (ModelState.IsValid)
            {
                //var comment = await _context.Comments.FindAsync(id);
                //var newComment = await _context.Comments.Include(c => c.Post).FirstOrDefaultAsync(c => c.Id == comment.Id);

                comment.BlogUserId = _userManager.GetUserId(User);
                comment.Created = DateTime.Now;

                _context.Add(comment);
                await _context.SaveChangesAsync();
                //return RedirectToAction("Details", "Posts", new { slug = comment.Post.Slug }, "commentSection");
                return RedirectToAction("Index", "Home");

            }

            return View();
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound("id null");
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound("comment null");
            }
            ViewData["BlogUserId"] = new SelectList(_context.Users, "Id", "Id", comment.BlogUserId);
            ViewData["ModeratorId"] = new SelectList(_context.Users, "Id", "Id", comment.ModeratorId);
            ViewData["PostId"] = new SelectList(_context.Posts, "Id", "Abstract", comment.PostId);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Body")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var newComment = await _context.Comments.Include(c => c.Post).FirstOrDefaultAsync(c => c.Id == comment.Id);

                try
                {
                    newComment.Body = comment.Body;
                    newComment.Updated = DateTime.Now;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Posts", new { slug = newComment.Post.Slug }, "commentSection");
            }
            
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.BlogUser)
                .Include(c => c.Moderator)
                .Include(c => c.Post)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string slug)
        {
            var comment = await _context.Comments.FindAsync(id);
            var newComment = await _context.Comments.Include(c => c.Post).FirstOrDefaultAsync(c => c.Id == comment.Id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Posts", new { slug = comment.Post.Slug }, "commentSection");
        }

        public async Task<IActionResult> Moderate(int id, [Bind("Id,Body,ModeratedBody,ModerationType")] Comment comment)
        {
            if (id != comment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var newComment = await _context.Comments.Include(c => c.Post).FirstOrDefaultAsync(c => c.Id == comment.Id);
                try
                {
                    newComment.ModeratedBody = comment.ModeratedBody;
                    newComment.ModerationType = comment.ModerationType;

                    newComment.Moderated = DateTime.Now;
                    newComment.ModeratorId = _userManager.GetUserId(User);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.Id))
                    {
                        return NotFound();
                    }

                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Details", "Posts", new { slug = newComment.Post.Slug }, "commentSection");
            }
            return View(comment);
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.Id == id);
        }
    }
}
