using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace EduInstitutesApp.Models
{
    public class Manager
    {
        public static Frame MainFrame { get; set; }
    }
}

//private static EduInstitutDBEntities _context;

//public static EduInstitutDBEntities GetContext()
//{
//    if (_context == null)
//    {
//        _context = new EduInstitutDBEntities();
//    }
//    return _context;
//}

//public string GetPhoto
//{
//    get
//    {
//        if (Photo is null)
//            return null;
//        return Directory.GetCurrentDirectory() + @"\Images\" + Photo.Trim();
//    }
//}
