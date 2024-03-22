using crafts_api.context;
using crafts_api.models.domain;

namespace crafts_api.models
{
    public class DbInitializer
    {
        public static void Seed(DatabaseContext context)
        {

            if (!context.Categories.Any())
            {
                context.AddRange(
                    new Category
                    {
                        Name = "Woodworking",
                        SkName = "Drevo"
                    },
                    new Category
                    {
                        Name = "Metalworking",
                        SkName = "Kováčstvo"
                    },
                    new Category
                    {
                        Name = "Leatherworking",
                        SkName = "Kožušníctvo"
                    }
                                                                                            );
                context.SaveChanges();
            }
        }
    }
}
