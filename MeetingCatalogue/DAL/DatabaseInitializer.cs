using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MeetingCatalogue.DAL
{
    public class DatabaseInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<MeetingCatalogueContext>
    {
    }
}