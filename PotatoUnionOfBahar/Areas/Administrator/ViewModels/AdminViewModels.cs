using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PotatoUnionOfBahar.Areas.Administrator.ViewModels
{
    public class AdminViewModels
    {
        public PotatoUnionOfBahar.Areas.Administrator.Models.Admin Admin { get; set; }
        public IList<PotatoUnionOfBahar.Areas.Administrator.Models.PostCategory> PostCaegories { get; set; }
        public IList<PotatoUnionOfBahar.Areas.Administrator.Models.Post> Posts { get; set; }
        public IList<PotatoUnionOfBahar.Areas.Administrator.Models.Comment> Comments { get; set; }
    }
}