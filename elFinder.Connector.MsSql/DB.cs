using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Dev.elFinder.Connector.MsSql.Models;


namespace Dev.elFinder.Connector.MsSql
{
    static class DB
    {
        private const string DIR_NAME = "directory";

        public static ElfinderFile GetModel(int id)
        {
            using (var context = new FileManagerContext())
            {
                return context.ElfinderFiles.FirstOrDefault(x => x.Id == id && x.IsDelete == false);
            }
        }

        public static void AddModel(ElfinderFile createdFile)
        {
            using (var context = new FileManagerContext())
            {
                context.ElfinderFiles.Add(createdFile);
                context.SaveChanges();
            }
        }

        public static void UpdateModel(ElfinderFile source)
        {
            using (var context = new FileManagerContext())
            {
                context.ElfinderFiles.Attach(source);
                context.Entry(source).State = EntityState.Modified;
                context.SaveChanges();
            }
        }


        public static IEnumerable<ElfinderFile> GetChildDir(int id)
        {
            using (var context = new FileManagerContext())
            {
                return context.ElfinderFiles.Where(x => x.Parent_id == id && x.Mime == DIR_NAME && x.IsDelete == false).ToList();
            }
        }

        public static ElfinderFile GetModelByHash(string hash)
        {
            using (var context = new FileManagerContext())
            {
                return context.ElfinderFiles.FirstOrDefault(x => x.Content == hash && x.IsDelete == false);
            }
        }

        public static ElfinderFile[] GetChildDirFilesByHash(string hash)
        {

            var model = GetModelByHash(hash);
            if (model == null)
                return new ElfinderFile[0];

            using (var context = new FileManagerContext())
            {
                var id = model.Id;
                return context.ElfinderFiles.Where(x => x.Parent_id == id && x.Mime != "directory" && x.IsDelete == false).ToArray();
            }
        }

        public static void DeleteModel(ElfinderFile source)
        {
            using (var context = new FileManagerContext())
            {
                source.IsDelete = true;
                context.ElfinderFiles.Attach(source);
                context.Entry(source).State = EntityState.Modified;

                context.SaveChanges();
            }

        }
    }
}
