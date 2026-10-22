using giraf_core_v2.Legacy.Abstractions;

namespace giraf_core_v2.Legacy.Foodplanner;

// Every Foodplanner legacy endpoint, grouped by legacy controller in legacy order
public static class FoodplannerLegacyRoutes
{
    public static void MapFoodplannerLegacyApi(this WebApplication app)
    {
        var foodplanner = app.MapLegacyApp<FoodplannerCallerResolver>("/legacy/foodplanner");

        // AdminController

        // ChildrensController

        // ClassroomsController

        // FeedbackChatController

        // ImagesController

        // IngredientsController

        // MealsController

        // OneTimePasswordController

        // PackedIngredientController

        // SubIngredientRelationController

        // SubIngredientsController

        // UsersController
    }
}