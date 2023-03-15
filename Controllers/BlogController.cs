using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using blogbackend.Models;
using blogbackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace blogbackend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BlogController : ControllerBase
    {
        private readonly BlogService _data;
        public BlogController(BlogService dataFromService)
        {
            _data = dataFromService;
        }

        [HttpPost]
        [Route("AddBlogItem")]

        public bool AddBlogItem(BlogItemModel newBlogItem)
        {
            return _data.AddBlogItem(newBlogItem);
        }

        [HttpGet]
        [Route("GetAllBlogItems")]

        public IEnumerable<BlogItemModel> GetAllBlogItems()
        {
            return _data.GetAllBlogItems();
        }
        
        [HttpGet]
        [Route("GetItemsByUserId/{UserId}")]

        public IEnumerable<BlogItemModel> GetItemsByUserId(int UserId)
        {
            return _data.GetItemsByUserId(UserId);
        }

        [HttpGet]
        [Route("GetItemsByCategory/{category}")]
        public IEnumerable<BlogItemModel> GetItemsByCategory (string category)
        {
            return _data.GetItemsByCategory(category);
        }

        [HttpGet]
        [Route("GetItemsByDate/{date}")]

        public IEnumerable<BlogItemModel> GetItemsByDate (string date)
        {
            return _data.GetItemsByDate(date);
        }

        [HttpGet]
        [Route("GetPublishedItems")]
        public IEnumerable<BlogItemModel> GetPublishedItems ()
        {
            return _data.GetPublishedItems();
        }

        [HttpGet]
        [Route("GetItemsByTag/{Tags}")]
        public List<BlogItemModel> GetItemsByTag(string Tag)
        {
            return _data.GetItemsByTag(Tag);
        }
    }
}