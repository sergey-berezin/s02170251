using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server
{
    [ApiController]
    [Route("api/[controller]")]
    public class DatabaseController :ControllerBase
    {
        [HttpGet]
        public async Task<List<int>> GetStatistic ()
        {
            Console.WriteLine("get stats request");
            List<int> stats = new List<int>();
            using (AppContext context = new AppContext())
            {
                for (int i = 0; i < 10; ++i)
                    stats.Add(0);

                foreach (var p in context.Pictures)
                {
                    stats[p.Label] += 1;
                }
            }

            return stats;
        }
        
        [HttpDelete]
        public void Clear(int id)
        {
            Console.WriteLine("Clear request");
            using (AppContext context = new AppContext())
            {
                context.Pictures.RemoveRange(context.Pictures);
                context.BlobPictures.RemoveRange(context.BlobPictures);
                context.SaveChanges();
            }
        }
    }
}
