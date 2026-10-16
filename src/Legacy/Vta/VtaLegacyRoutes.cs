using giraf_core_v2.Legacy.Abstractions;

namespace giraf_core_v2.Legacy.Vta;

// Every VTA legacy endpoint, grouped by legacy controller in legacy order
public static class VtaLegacyRoutes
{
    public static void MapVtaLegacyApi(this WebApplication app)
    {
        var vta = app.MapLegacyApp<VtaCallerResolver>("/legacy/vta");

        // AdminController

        // ArtefactsController

        // AssetsController

        // BoardsController

        // CategoriesController

        // ContactsController

        // MigrationController

        // RelationController

        // SavedArtefactsController

        // SyncController

        // UsersController
    }
}