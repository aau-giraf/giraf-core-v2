using giraf_core_v2.Legacy.Abstractions;
using giraf_core_v2.Legacy.Foodplanner.Dtos;

namespace giraf_core_v2.Legacy.Foodplanner;

// Every Foodplanner legacy endpoint, grouped by legacy controller in legacy order.
// One line per endpoint: Map{Method}<Request, Response, Adapter>("path copied from the old controller").
// The Users. prefix is the folder/namespace, so two controllers can both have e.g. a CreateAdapter
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

        // UsersController (GetBearerTest is not ported: it only made test tokens for the old backend)
        foodplanner.MapLegacyPost<Users.CreateRequest, int, Users.CreateAdapter>("/api/Users/Create");
        foodplanner.MapLegacyPost<Users.CreateUserChildrenRequest, int, Users.CreateUserChildrenAdapter>("/api/Users/CreateUserChildren");
        foodplanner.MapLegacyPost<Users.LoginRequest, UserCredsDTO, Users.LoginAdapter>("/api/Users/Login");
        foodplanner.MapLegacyPost<Users.LoginChildRequest, UserCredsDTO, Users.LoginChildAdapter>("/api/Users/LoginChild");
        foodplanner.MapLegacyPut<Users.UpdatePinCodeRequest, NoBody, Users.UpdatePinCodeAdapter>("/api/Users/UpdatePinCode");
        foodplanner.MapLegacyPost<Users.CheckPinCodeRequest, UserCredsDTO, Users.CheckPinCodeAdapter>("/api/Users/CheckPinCode");
        foodplanner.MapLegacyGet<Users.EmailExistsRequest, Users.EmailExistsResponse, Users.EmailExistsAdapter>("/api/Users/EmailExists");
        foodplanner.MapLegacyGet<NoInput, Users.HasPinCodeResponse, Users.HasPinCodeAdapter>("/api/Users/HasPinCode");
        foodplanner.MapLegacyGet<NoInput, UserDTO, Users.GetLoggedInAdapter>("/api/Users/GetLoggedIn");
        foodplanner.MapLegacyPut<Users.UpdateLoggedInRequest, NoBody, Users.UpdateLoggedInAdapter>("/api/Users/UpdateLoggedIn");
        foodplanner.MapLegacyPut<Users.UpdatePasswordRequest, NoBody, Users.UpdatePasswordAdapter>("/api/Users/UpdatePassword");
        foodplanner.MapLegacyDelete<NoInput, NoBody, Users.DeleteLoggedInUserAdapter>("/api/Users/DeleteLoggedInUser");
    }
}