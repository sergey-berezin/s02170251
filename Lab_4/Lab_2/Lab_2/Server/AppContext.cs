using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class AppContext:DbContext
    {
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<BlobPicture> BlobPictures { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder o)
        => o.UseSqlite(@"Data Source = D:\7_sem_prac\Lab_2\Lab_2\WpfApp1\myDataBase.db").UseLazyLoadingProxies();
    }

    public class Picture
    {
        public int Id { get; set; }
        public string Filename { get; set; }
        public int Label { get; set; }
        
        public virtual BlobPicture Blob { get; set; }
    }

    public class BlobPicture
    {
        [ForeignKey("Picture")]
        public int Id { get; set; }
        public byte[] Pixels { get; set;  }
        //public int Width { get; set; }
        //public int Height { get; set; }

        public virtual Picture Picture { get; set; }
    }
}
